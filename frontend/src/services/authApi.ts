import { API_ENDPOINTS, STORAGE_KEYS } from '../constants/app.constants'
import type { LoginRequest, RegisterRequest, AuthResponse } from '../types/auth.types'

class AuthApiService {
  async login(request: LoginRequest): Promise<AuthResponse> {
    const response = await fetch(API_ENDPOINTS.AUTH_LOGIN, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    })

    const data: AuthResponse = await response.json()

    if (data.success && data.token) {
      localStorage.setItem(STORAGE_KEYS.AUTH_TOKEN, data.token)
      if (data.email) {
        localStorage.setItem(STORAGE_KEYS.USER_EMAIL, data.email)
      }
    }

    return data
  }

  async register(request: RegisterRequest): Promise<AuthResponse> {
    const response = await fetch(API_ENDPOINTS.AUTH_REGISTER, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    })

    const data: AuthResponse = await response.json()

    if (data.success && data.token) {
      localStorage.setItem(STORAGE_KEYS.AUTH_TOKEN, data.token)
      if (data.email) {
        localStorage.setItem(STORAGE_KEYS.USER_EMAIL, data.email)
      }
    }

    return data
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEYS.AUTH_TOKEN)
    localStorage.removeItem(STORAGE_KEYS.USER_EMAIL)
  }

  getToken(): string | null {
    return localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN)
  }

  getUserEmail(): string | null {
    return localStorage.getItem(STORAGE_KEYS.USER_EMAIL)
  }

  isAuthenticated(): boolean {
    return !!this.getToken()
  }
}

export const authApi = new AuthApiService()
