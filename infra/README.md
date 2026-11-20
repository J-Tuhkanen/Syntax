# Syntax on Azure

Hobby-scale hosting for Syntax on Azure Container Apps, deployed from GitHub
Actions with OIDC (no stored secrets). Region: Sweden Central. One environment
(`dev`); another one is another `.tfvars` file.

```
                 Internet (HTTPS, managed certificate)
                              │
          ┌───────────────────▼────────────────────┐  Container Apps environment
          │ ca-syntax-frontend-dev  nginx + Vite   │  (Consumption, VNet snet-aca)
          │   /            -> SPA                  │
          │   /api/  /notification/  /users/  ──┐  │
          │ ca-syntax-api-dev  ASP.NET Core  ◄──┘  │  internal ingress, max 1 replica
          │        │ TCP 5432                      │
          │ ca-syntax-postgres-dev  Postgres 17    │  internal TCP ingress, 1 replica
          │        └─ NFS share (SSD v2, 32 GiB)   │
          │ jobs: flyway · pg-backup (nightly) · pg-restore                │
          └───────┬───────────────────────┬────────┘
                  │ service endpoints     │
        Blob storage (avatars,      Key Vault (Postgres password,
        Data Protection keys,       connection string, DP key)
        pg-backups)
```

- **Same origin for everything.** nginx serves the SPA and proxies the API,
  SignalR (WebSockets) and avatars, so the auth cookie is first-party and
  there is no CORS.
- **No public database, no keys.** Postgres and the API have internal ingress
  only. Apps reach Blob storage and Key Vault with per-workload managed
  identities; storage accounts have shared keys disabled.
- **Scale to zero.** Frontend and API scale 0–1; Postgres runs one replica.
- **Budget guard.** A €30 monthly budget on `rg-syntax-dev`; at 100 % of actual
  spend an Automation runbook stops every container app.

| Folder | What it is | Who runs it |
|---|---|---|
| `infra/bootstrap` | State storage, ACR, GitHub identity, environment resource groups, provider registration | You, once, as subscription Owner |
| `infra/main` | Everything in `rg-syntax-dev` | GitHub Actions (`.github/workflows/deploy.yml`) |
| `infra/scripts` | `start-apps.sh`, `postgres-update.sh` | Pipeline and you |
| `db/` | Migrations, local compose, Postgres / Flyway / backup images | Pipeline and local dev |

## 1. One-time setup

You need the [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli),
[Terraform](https://developer.hashicorp.com/terraform/install) 1.9 or newer and,
for step 1.4, the [GitHub CLI](https://cli.github.com/). The commands work in
PowerShell and in bash unless noted.

### 1.1 Subscription and sign-in

1. Create a subscription (you'll be its Owner). Its billing currency decides
   what "30" in the budget means; for a Finnish account that is euros.
2. Sign in and pick it:

   ```bash
   az login
   az account set --subscription "<subscription name or id>"
   az account show --query id -o tsv     # the subscription id, used below
   ```

### 1.2 Check the repository's OIDC subject format

GitHub repositories created after 15 July 2026, or opted in to *immutable
subject claims*, put owner and repository IDs into the token subject. This
repository is older, so the default name-based format should apply. Check:

```bash
gh api repos/J-Tuhkanen/Syntax/actions/oidc/customization/sub
```

If `use_immutable_subject` is `true`, also pass
`-var 'github_oidc_subject_prefix=repo:J-Tuhkanen@<owner_id>/Syntax@<repo_id>'`
in the next step (IDs from `gh api repos/J-Tuhkanen/Syntax --jq '.owner.id, .id'`).

### 1.3 Run bootstrap

```bash
cd infra/bootstrap
terraform init
terraform apply -var="subscription_id=<subscription id>"
```

This registers the resource providers, then creates `rg-syntax-shared` (state
storage, container registry, the `id-syntax-github` identity with federated
credentials for `master`, pull requests and the `dev` environment) and the
empty `rg-syntax-dev`. The CI identity gets:

- Contributor on both resource groups,
- Key Vault Secrets Officer and Crypto Officer on `rg-syntax-dev`,
- Role Based Access Control Administrator on both groups, **limited** by a
  condition to AcrPull, Storage Blob Data Contributor, Key Vault Secrets User,
  Key Vault Crypto User and the custom *Syntax Container Apps Power Operator*
  role, and only for service principals,
- Storage Blob Data Contributor on the state account.

Bootstrap keeps its state locally in `infra/bootstrap/terraform.tfstate`
(gitignored). It holds no secrets, but keep a copy: you need it to change
bootstrap later.

### 1.4 Set the GitHub repository variables

From the repository folder:

```bash
terraform -chdir=infra/bootstrap output -raw gh_variable_commands
```

Run the `gh variable set ...` lines it prints. They set `AZURE_CLIENT_ID`,
`AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`, `TF_STATE_RESOURCE_GROUP`,
`TF_STATE_STORAGE_ACCOUNT`, `TF_STATE_CONTAINER` and `ACR_NAME`. None of these
are secrets.

Optional: `gh variable set ALERT_EMAIL --body "you@example.com"` for budget
mails. Without it, budget notifications go to the Owners of the resource group
(which includes you as subscription Owner).

The `dev` GitHub environment is created automatically on the first deployment.
Then, under Settings → Environments → dev, set *Deployment branches* to
`master` only: the environment's federated credential accepts a token from any
branch that names the environment. Add required reviewers there too if you
want to approve deployments by hand.

To add another environment later, add it to `environments` in bootstrap, run
`terraform apply -parallelism=1 ...` (Azure rejects parallel federated
credential writes on one identity), and create `infra/main/environments/<env>.tfvars`.

### 1.5 Before the first push

Terraform, `npm install` and `dotnet build` couldn't be run when this branch
was written, so do these once locally:

```bash
terraform fmt -recursive infra                 # the PR check runs fmt -check
terraform -chdir=infra/main init -backend=false
terraform -chdir=infra/main validate

cd frontend && npm install && npm run build && npm test -- --run
git add package-lock.json                      # makes the image build reproducible
cd .. && dotnet build Syntax.sln
```

Commit any formatting changes and the new `frontend/package-lock.json`.

## 2. Deployments

`.github/workflows/deploy.yml`:

| Trigger | What happens |
|---|---|
| Pull request to `master` | Builds all five images (no push), `terraform fmt -check`, `validate`, `plan` |
| Push to `master`, or *Run workflow* | Builds and pushes the images tagged with the commit SHA. Only if every build succeeds: `terraform apply`, start any stopped apps, run the Flyway job and wait for it, roll out the API, then the frontend, then a smoke test |

The app's URL is in the deploy job's summary and in
`terraform -chdir=infra/main output frontend_url`.

Images (all in the ACR from bootstrap):

| Image | Built from | Tag |
|---|---|---|
| `syntax-frontend` | `frontend/Dockerfile` | commit SHA |
| `syntax-api` | `backend/dockerfile` | commit SHA |
| `syntax-db-migrate` | `db/Dockerfile.migrate` | commit SHA |
| `syntax-pg-backup` | `db/backup/Dockerfile` | commit SHA |
| `syntax-postgres` | `db/postgres/Dockerfile` | `POSTGRES_IMAGE_TAG` in deploy.yml, pushed once |

The first deployment takes 15–20 minutes, most of it creating the Container
Apps environment. If it fails while creating the apps with a Key Vault or
registry authorization error, re-run it: new role assignments can take a few
minutes to apply, and Terraform already waits 90 seconds for them.

Terraform sets the API and frontend images only when it creates those apps;
after that it ignores them, and the pipeline rolls out new images after the
migrations. Changes to anything else on those apps (environment variables,
CPU, probes) are applied by `terraform apply` as usual.

## 3. Database

### Access

There is no public endpoint. Open `psql` inside the Postgres container:

```bash
az containerapp exec -g rg-syntax-dev -n ca-syntax-postgres-dev --command "psql -U syntax -d syntax"
```

The database and its owner are both `syntax`; the API and Flyway connect as
that owner. The password is the `postgres-password` secret in the Key Vault
(`terraform -chdir=infra/main output key_vault_name`).

### Migrations

Add `db/Migrations/V<n>__<description>.sql`. The pipeline bakes the folder
into the `syntax-db-migrate` image and runs it before every API rollout. Run
it by hand with:

```bash
az containerapp job start -g rg-syntax-dev -n caj-syntax-flyway-dev
```

### Backups and restore

`caj-syntax-pgbackup-dev` runs `pg_dump` every night at 01:30 UTC into the
`pg-backups` container (`syntax-<UTC timestamp>.dump`). A lifecycle rule
deletes dumps after 14 days.

```bash
# Back up now
az containerapp job start -g rg-syntax-dev -n caj-syntax-pgbackup-dev

# Restore the newest dump (other sessions are disconnected; the API reconnects)
az containerapp job start -g rg-syntax-dev -n caj-syntax-pgrestore-dev

# Restore a specific dump: point the job at it, start it, then let the next
# terraform apply put the default ("latest") back
az containerapp job update -g rg-syntax-dev -n caj-syntax-pgrestore-dev --args "restore" "syntax-20261120T013000Z.dump"
az containerapp job start  -g rg-syntax-dev -n caj-syntax-pgrestore-dev
```

Follow a job in the portal (job → Execution history → Console logs) or in Log
Analytics (`ContainerAppConsoleLogs_CL`). The storage firewall only admits the
Container Apps subnet; to browse the dumps in the portal, add your IP under the
storage account's *Networking* settings and remove it again afterwards.

### Updating Postgres

Container Apps starts a new revision before stopping the old one. Two
postmasters on one data directory would corrupt it, so:

- Terraform creates the Postgres app once and then ignores its template.
- The pipeline builds and pushes a new `syntax-postgres` image when you change
  `POSTGRES_VERSION` / `POSTGRES_IMAGE_TAG` in deploy.yml, but never rolls it out.
- The image's entrypoint (`db/postgres/wait-for-previous.sh`) waits while a
  previous postmaster still answers, as a safety net.

To roll out a new minor version (or change CPU, memory or environment
variables), back up first, then:

```bash
infra/scripts/postgres-update.sh rg-syntax-dev ca-syntax-postgres-dev \
  <acr>.azurecr.io/syntax-postgres:17.13-1      # extra az containerapp update flags may follow
```

It stops the app, waits until it's stopped, updates it and starts it again.
(bash: Git Bash or WSL on Windows.) Update the Terraform defaults to match, so
a re-created environment gets the same settings.

A major version (17 → 18) changes the data directory format: restore a dump
into a new, empty share instead.

## 4. Budget and automatic shutdown

| Threshold | Action |
|---|---|
| 50 % of €30, actual | email |
| 80 %, actual | email |
| 100 %, forecast | email |
| 100 %, actual | email, and the Action Group runs `Stop-ContainerApps` in `aa-syntax-dev`: every container app in `rg-syntax-dev` is stopped, Postgres last (fast shutdown, nothing lost) |

Things to know:

- **Cost data lags by several hours** (sometimes a day), so the shutdown is a
  safety net that kicks in after the budget is crossed, not a hard cap.
- **Stopped is not free.** The registry, the NFS share, Log Analytics and
  storage keep costing roughly €9–10 a month.
- Each threshold notifies once per month.

Bring everything back by re-running the pipeline (Actions → *Build and deploy*
→ *Run workflow*) or with:

```bash
infra/scripts/start-apps.sh rg-syntax-dev
```

## 5. Expected cost

Rough monthly estimate for light use (USD list prices; Sweden Central and EUR
prices differ a little — check the [pricing
calculator](https://azure.microsoft.com/pricing/calculator/)):

| Item | Estimate |
|---|---|
| Postgres replica, 0.5 vCPU / 1 GiB, always on, mostly idle | ~$11 (after the monthly free grant) |
| Frontend and API, scale to zero | ~$0 (within the free grant) |
| NFS share, SSD provisioned v2, 32 GiB | ~$4 |
| Container Registry Basic | ~$5 |
| Log Analytics (capped at 0.2 GB/day) | $0–3 |
| Blob, Key Vault, Automation, budget | < $1 |
| **Total** | **~$20–24** |

The Postgres line assumes the replica is billed at the idle rate (below 0.01
vCPU and 1,000 bytes/s of traffic). At the active rate it would be about $23
more, and the budget shutdown would trigger. Check *Cost analysis* after the
first few days.

## 6. Running locally

```bash
# Database (Postgres on localhost:2345, migrations applied by Flyway)
cd db
cp .env.example .env            # set POSTGRES_PASSWORD; an existing volume keeps its old password
docker compose up -d

# API (https://localhost:7181)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=2345;Database=SyntaxDb;Username=postgres;Password=<POSTGRES_PASSWORD>" \
  --project backend
dotnet run --project backend --launch-profile Syntax.API

# Frontend (http://localhost:3000, proxies /api, /notification and /users/ to the API)
cd frontend && npm install && npm run dev
```

In Development, avatars are stored under `backend/wwwroot/users`, the Data
Protection keys stay local, and rate limiting is off.

## 7. Design notes and limits

- **One API replica.** SignalR groups live in memory. More replicas would need
  a backplane (for example Azure SignalR Service).
- **Storage "public network access".** Both storage accounts deny all networks
  except `snet-aca` (service endpoint). Turning public network access fully off
  would also block service endpoints, which only private endpoints get around.
  Terraform therefore never touches their data plane
  (`features.storage.data_plane_available = false`).
- **Key Vault** keeps its public endpoint, gated by Entra ID and RBAC, so CI
  can write the generated secret. The Postgres password is also in Terraform
  state, which is why the state account accepts Entra ID sign-in only.
- **Database role.** The app connects as the database owner. For a separate
  least-privilege role, add a migration that creates it and point the
  connection string at it.
- **Rate limiting** is ASP.NET Core's built-in limiter (300 requests a minute
  per client, 10 for login and register); there is no API gateway.
- **Provider version.** The configuration targets azurerm 4.x (`~> 4.81`).
  azurerm 5.0 has been out since July 2026; moving to it means working through
  its upgrade guide.
- **Unverified when written.** `terraform validate` couldn't run while this was
  written. If it complains, the likeliest suspects are the newer arguments:
  `nfs_server_url` (environment storage), `storage_type = "NfsAzureFile"`
  (Postgres volume), `provisioned_billing_model_version`,
  `rbac_authorization_enabled` and `features.storage.data_plane_available`.
- **NFS mount problems** show up only in `ContainerAppSystemLogs_CL`, because
  the container never starts.
