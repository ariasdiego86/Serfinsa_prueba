import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { AuthProvider } from './contexts/AuthContext'
import ProtectedRoute from './components/ProtectedRoute'
import Login from './pages/Login'
import Products from './pages/Products'

/**
 * Componente raíz de la aplicación.
 * Estructura:
 * - BrowserRouter: habilita el enrutamiento del lado del cliente con la History API.
 * - AuthProvider: provee el contexto de autenticación a toda la app.
 * - Routes: define el árbol de rutas.
 *   - /login: pública, accesible sin token.
 *   - ProtectedRoute: wrapper que redirige a /login si no hay token.
 *     - /products: privada, requiere JWT válido.
 *   - / → redirige a /products por defecto.
 */
export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          {/* Ruta pública: Login */}
          <Route path="/login" element={<Login />} />

          {/* Rutas protegidas: requieren autenticación */}
          <Route element={<ProtectedRoute />}>
            <Route path="/products" element={<Products />} />
          </Route>

          {/* Ruta por defecto: redirigir a productos */}
          <Route path="/" element={<Navigate to="/products" replace />} />

          {/* Cualquier otra ruta: redirigir a productos */}
          <Route path="*" element={<Navigate to="/products" replace />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  )
}
