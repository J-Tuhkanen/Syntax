# syntax=docker/dockerfile:1

FROM node:24-alpine AS build
WORKDIR /app

# package-lock.json is optional only until it is committed; with it the build
# is reproducible (npm ci).
COPY package.json package-lock.json* ./
RUN if [ -f package-lock.json ]; then npm ci; else npm install --no-audit --no-fund; fi

COPY . .
RUN npm run build

FROM nginxinc/nginx-unprivileged:1.30-alpine

# Substitute only API_* variables into the template, leaving nginx's own
# $variables alone.
ENV NGINX_ENVSUBST_FILTER=^API_

COPY nginx/default.conf.template /etc/nginx/templates/default.conf.template
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 8080
