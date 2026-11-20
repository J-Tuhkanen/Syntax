# Settings for the dev environment. Another environment is another file like
# this one (plus the environment name in infra/bootstrap's `environments`).
#
# Passed by CI instead of being set here:
#   acr_name           TF_VAR_acr_name    (GitHub variable ACR_NAME)
#   alert_email        TF_VAR_alert_email (GitHub variable ALERT_EMAIL, optional)
#   image_tag          -var image_tag=<git sha>
#   postgres_image_tag -var postgres_image_tag=<tag>

environment = "dev"

budget_amount = 30

log_retention_days = 30
log_daily_quota_gb = 0.2

postgres_share_size_gib = 32
backup_retention_days   = 14
