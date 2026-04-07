import { Navigate, Outlet } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'

/**
 * Componente de Ruta Protegida.
 * Si el usuario no está autenticado (no hay token en localStorage),
 * redirige automáticamente a /login.
 * Si está autenticado, renderiza el componente hijo (Outlet).
 *
 * Uso en el router:
 * <Route element={<ProtectedRoute />}>
 *   <Route path="/products" element={<Products />} />
 * </Route>
 */
export default function ProtectedRoute() {
  const { isAuthenticated } = useAuth()

  // Navigate realiza una redirección declarativa.
  // replace=true evita que el usuario pueda volver a la ruta protegida con el botón "atrás".
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />
  }

  // Outlet renderiza el componente hijo de la ruta anidada
  return <Outlet />
}
