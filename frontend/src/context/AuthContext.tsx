import { createContext, useContext, useState, useCallback, useEffect, type ReactNode } from 'react'
import { authApi } from '../services/authApi'
import type { LoginRequest, RegisterRequest, AuthResponse } from '../types/auth.types'

interface AuthContextType {
  isAuthenticated: boolean
  userEmail: string | null
  loading: boolean
  error: string | null
  login: (request: LoginRequest) => Promise<AuthResponse>
  register: (request: RegisterRequest) => Promise<AuthResponse>
  logout: () => void
  clearError: () => void
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [isAuthenticated, setIsAuthenticated] = useState(authApi.isAuthenticated())
  const [userEmail, setUserEmail] = useState(authApi.getUserEmail())
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    setIsAuthenticated(authApi.isAuthenticated())
    setUserEmail(authApi.getUserEmail())
  }, [])

  const login = useCallback(async (request: LoginRequest): Promise<AuthResponse> => {
    setLoading(true)
    setError(null)

    try {
      const response = await authApi.login(request)

      if (response.success) {
        setIsAuthenticated(true)
        setUserEmail(response.email ?? null)
      } else {
        setError(response.errors?.join(', ') ?? 'Login failed')
      }

      return response
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Login failed'
      setError(errorMessage)
      throw err
    } finally {
      setLoading(false)
    }
  }, [])

  const register = useCallback(async (request: RegisterRequest): Promise<AuthResponse> => {
    setLoading(true)
    setError(null)

    try {
      const response = await authApi.register(request)

      if (response.success) {
        setIsAuthenticated(true)
        setUserEmail(response.email ?? null)
      } else {
        setError(response.errors?.join(', ') ?? 'Registration failed')
      }

      return response
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Registration failed'
      setError(errorMessage)
      throw err
    } finally {
      setLoading(false)
    }
  }, [])

  const logout = useCallback(() => {
    authApi.logout()
    setIsAuthenticated(false)
    setUserEmail(null)
  }, [])

  const clearError = useCallback(() => {
    setError(null)
  }, [])

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        userEmail,
        loading,
        error,
        login,
        register,
        logout,
        clearError,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}
