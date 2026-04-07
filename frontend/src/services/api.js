import axios from 'axios'

// Instancia de axios con la URL base del backend.
// Centralizar la configuración aquí permite cambiar el endpoint en un solo lugar.
const api = axios.create({
  baseURL: '/api', // El proxy de Vite redirige esto a http://localhost:5000/api
  headers: {
    'Content-Type': 'application/json',
  },
})

// =========================================================
// Interceptor de REQUEST: inyección automática del JWT
// =========================================================
// Se ejecuta ANTES de cada petición HTTP.
// Lee el token del localStorage y lo agrega al header Authorization.
// Así todos los componentes pueden llamar a la API sin preocuparse del token.
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// =========================================================
// Interceptor de RESPONSE: manejo global de errores HTTP
// =========================================================
// Se ejecuta DESPUÉS de recibir cada respuesta HTTP.
// Un 401 significa que el token expiró o es inválido: limpiar sesión y redirigir.
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token inválido o expirado: eliminar de localStorage y forzar login
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      // Redirigir al login sin usar React Router (para que funcione fuera de componentes)
      if (window.location.pathname !== '/login') {
        window.location.href = '/login'
      }
    }
    return Promise.reject(error)
  }
)

// =========================================================
// Servicios de autenticación
// =========================================================
export const authService = {
  login: (credentials) => api.post('/auth/login', credentials),
}

// =========================================================
// Servicios de productos (CRUD)
// =========================================================
export const productService = {
  getAll: () => api.get('/products'),
  getById: (id) => api.get(`/products/${id}`),
  create: (data) => api.post('/products', data),
  update: (id, data) => api.put(`/products/${id}`, data),
  delete: (id) => api.delete(`/products/${id}`),
}

export default api
