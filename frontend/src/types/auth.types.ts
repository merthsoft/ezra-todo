export interface LoginRequest {
    email: string
    password: string
}

export interface RegisterRequest {
    email: string
    password: string
}

export interface AuthResponse {
    success: boolean
    token?: string
    email?: string
    errors?: string[]
}
