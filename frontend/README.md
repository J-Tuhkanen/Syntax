# Syntax frontend

React 18 + TypeScript, built with [Vite](https://vite.dev).

Requires Node.js 20.19+ (or 22.12+).

```bash
npm install        # first time, and after package.json changes
npm run dev        # http://localhost:3000
npm test           # Vitest
npm run build      # type-check and build into dist/
```

## API during development

The dev server proxies `/api`, `/notification` (SignalR, including WebSockets)
and `/users/` to the API at `https://localhost:7181`, the same routes nginx
proxies in production. Start the backend with the `Syntax.API` launch profile,
or point the proxy elsewhere:

```bash
API_PROXY_TARGET=http://localhost:5146 npm run dev
```

The app only uses relative URLs, so it never needs to know where the API runs.

## Production image

`Dockerfile` builds the app and serves `dist/` with nginx, which also proxies
the API routes to `API_UPSTREAM` (see `nginx/default.conf.template`):

```bash
docker build -t syntax-frontend .
docker run --rm -p 8080:8080 -e API_UPSTREAM=http://host.docker.internal:5146 syntax-frontend
```
