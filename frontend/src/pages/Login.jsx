import { useState, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'

/**
 * Página de Login.
 *
 * Hooks utilizados:
 * - useState: maneja los campos del formulario, errores y estado de carga.
 *   Se justifica porque son valores que cambian con la interacción del usuario.
 * - useCallback: memoriza handleSubmit para que no se recree en cada render.
 *   Aunque aquí el beneficio es menor, es buena práctica en funciones pasadas como handlers.
 */
export default function Login() {
  // useState para los campos del formulario y mensajes de error
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [errors, setErrors] = useState({})
  const [serverError, setServerError] = useState('')
  const [loading, setLoading] = useState(false)

  const { login } = useAuth()
  const navigate = useNavigate()

  // =========================================================
  // Validación en frontend (espejo de las reglas del backend)
  // =========================================================
  const validate = () => {
    const newErrors = {}

    if (!email.trim()) {
      newErrors.email = 'El email es obligatorio.'
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      newErrors.email = 'Ingresa un email válido.'
    }

    if (!password) {
      newErrors.password = 'La contraseña es obligatoria.'
    } else if (password.length < 6) {
      newErrors.password = 'La contraseña debe tener al menos 6 caracteres.'
    }

    return newErrors
  }

  // useCallback: memoriza el handler del formulario.
  // Evita la recreación de la función en cada render del componente.
  const handleSubmit = useCallback(async (e) => {
    e.preventDefault()
    setServerError('')

    // Validar antes de llamar al servidor
    const validationErrors = validate()
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors)
      return
    }
    setErrors({})

    setLoading(true)
    try {
      await login(email, password)
      // Si el login es exitoso, redirigir a la vista de productos
      navigate('/products', { replace: true })
    } catch (err) {
      // Mostrar el mensaje de error del servidor (ProblemDetails del backend)
      const detail = err.response?.data?.detail || err.response?.data?.title
      setServerError(detail || 'Error al iniciar sesión. Intenta nuevamente.')
    } finally {
      setLoading(false)
    }
  }, [email, password, login, navigate])

  return (
    <div className="login-page">
      <div className="login-card">
        <h1 className="login-card__title">Bienvenido</h1>
        <p className="login-card__subtitle">Inicia sesión para gestionar los productos</p>

        {/* Mensaje de error del servidor */}
        {serverError && (
          <div className="alert alert-error">{serverError}</div>
        )}

        <form onSubmit={handleSubmit} noValidate>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1.2rem' }}>

            {/* Campo Email */}
            <div className="form-group">
              <label htmlFor="email">Email</label>
              <input
                id="email"
                type="email"
                className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="admin@serfinsa.com"
                autoComplete="email"
                disabled={loading}
              />
              {errors.email && <span className="error-text">{errors.email}</span>}
            </div>

            {/* Campo Contraseña */}
            <div className="form-group">
              <label htmlFor="password">Contraseña</label>
              <input
                id="password"
                type="password"
                className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="••••••••"
                autoComplete="current-password"
                disabled={loading}
              />
              {errors.password && <span className="error-text">{errors.password}</span>}
            </div>

            {/* Botón de submit */}
            <button
              type="submit"
              className="btn btn-primary"
              disabled={loading}
              style={{ padding: '0.75rem', fontSize: '1rem', marginTop: '0.5rem' }}
            >
              {loading ? (
                <><span className="spinner" /> Iniciando sesión...</>
              ) : (
                'Iniciar sesión'
              )}
            </button>

          </div>
        </form>

        <p style={{ marginTop: '1.5rem', fontSize: '0.8rem', color: '#a0aec0', textAlign: 'center' }}>
          Usuario por defecto: <strong>admin@serfinsa.com</strong> / <strong>Admin123!</strong>
        </p>
      </div>
    </div>
  )
}
