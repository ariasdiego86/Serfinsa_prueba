import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// Configuración de Vite para React 19
// El proxy redirige las llamadas /api al backend en desarrollo,
// evitando problemas de CORS durante el desarrollo local.
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      }
    }
  }
})
