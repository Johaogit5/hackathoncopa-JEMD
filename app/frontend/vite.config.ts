import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// En build de produccion, cualquier import de environments/environment
// se resuelve a environments/environment.prod
export default defineConfig(({ mode }) => ({
  plugins: [react()],
  resolve: {
    alias: mode === 'production'
      ? [{
          find: /.*\/environments\/environment$/,
          replacement: '/src/environments/environment.prod.ts'
        }]
      : []
  },
  server: { host: '0.0.0.0', port: 4200 }
}));
