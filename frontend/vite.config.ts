/// <reference types="vitest/config" />
import { fileURLToPath } from 'node:url';
import { defineConfig, type ProxyOptions } from 'vite';
import react from '@vitejs/plugin-react';

// Bare imports such as 'components/...' or 'App' resolve from src/, matching
// "baseUrl": "src" in tsconfig.json (CRA supported this out of the box).
const src = fileURLToPath(new URL('./src', import.meta.url));
const srcRoots = ['App', 'assets', 'components', 'dtos', 'models', 'services', 'utils', 'views'];

// The API during local development (`dotnet run` on the https launch profile).
// Override with API_PROXY_TARGET, e.g. http://localhost:5146.
const apiTarget = process.env.API_PROXY_TARGET ?? 'https://localhost:7181';

const toApi = (ws = false): ProxyOptions => ({
  target: apiTarget,
  changeOrigin: true,
  secure: false, // the ASP.NET Core dev certificate is self-signed
  ws,
});

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: srcRoots.map((root) => ({
      find: new RegExp(`^${root}(/|$)`),
      replacement: `${src}/${root}$1`,
    })),
  },
  server: {
    port: 3000,
    strictPort: true,
    // Same routes as nginx in production, so the browser only ever talks to
    // one origin and the auth cookie stays first-party.
    proxy: {
      '/api': toApi(),
      '/notification': toApi(true),
      '/users/': toApi(),
    },
  },
  build: {
    outDir: 'dist',
  },
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: './src/setupTests.ts',
  },
});
