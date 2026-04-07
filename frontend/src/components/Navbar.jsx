import { useNavigate } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'

/**
 * Barra de navegación superior.
 * Muestra el nombre de la app, el usuario actual, su rol y el botón de Logout.
 * El botón Logout llama a auth.logout() que limpia localStorage y redirige al Login.
 */
export default function Navbar() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    // Limpiar sesión en AuthContext (y localStorage vía el context)
    logout()
    // Redirigir al login después del logout
    navigate('/login', { replace: true })
  }

  return (
    <nav className="navbar">
      <span className="navbar__brand">Serfinsa — Gestión de Productos</span>
      <div className="navbar__user">
        <span>{user?.email}</span>
        <span className={`badge ${user?.role === 'Admin' ? 'badge-admin' : 'badge-user'}`}>
          {user?.role}
        </span>
        <button className="btn btn-logout" onClick={handleLogout}>
          Cerrar sesión
        </button>
      </div>
    </nav>
  )
}
