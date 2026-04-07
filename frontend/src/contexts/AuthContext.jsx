import { createContext, useContext, useState, useCallback, useMemo } from 'react'
import { authService } from '../services/api'

// =========================================================
// AuthContext: Estado global de autenticación
// =========================================================
// Permite que cualquier componente acceda al usuario autenticado
// sin necesidad de prop drilling. Es el patrón recomendado en React
// para estado global cuando no se necesita un gestor de estado complejo.

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  // useState: guarda el usuario autenticado.
  // Se inicializa desde localStorage para persistir la sesión entre recargas de página.
  const [user, setUser] = useState(() => {
    const storedUser = localStorage.getItem('user')
    return storedUser ? JSON.parse(storedUser) : null
  })

  // useCallback: memoriza la función login para que no se recree en cada render.
  // Es importante aquí porque se pasa como dependencia en otros hooks de componentes hijos.
  const login = useCallback(async (email, password) => {
    const response = await authService.login({ email, password })
    const { token, email: userEmail, role } = response.data

    // Guardar el JWT en localStorage para persistir la sesión
    localStorage.setItem('token', token)
    // Guardar datos básicos del usuario para mostrarlos en la UI sin decodificar el JWT
    const userData = { email: userEmail, role }
    localStorage.setItem('user', JSON.stringify(userData))

    setUser(userData)
    return userData
  }, [])

  // useCallback: memoriza la función logout.
  // Limpia el estado local y el localStorage, efectivamente cerrando la sesión.
  const logout = useCallback(() => {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    setUser(null)
  }, [])

  // useMemo: evita que el objeto del contexto se recree en cada render del Provider.
  // Sin useMemo, todos los consumidores del contexto se re-renderizarían innecesariamente.
  const contextValue = useMemo(() => ({
    user,
    isAuthenticated: !!user,
    isAdmin: user?.role === 'Admin',
    login,
    logout,
  }), [user, login, logout])

  return (
    <AuthContext.Provider value={contextValue}>
      {children}
    </AuthContext.Provider>
  )
}

// Hook personalizado para consumir el AuthContext de forma segura.
// Lanza un error descriptivo si se usa fuera del AuthProvider.
export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth debe usarse dentro de un AuthProvider')
  }
  return context
}
