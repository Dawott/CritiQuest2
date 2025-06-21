export interface User {
  id: string
  email: string
  displayName: string
  joinedAt: string
}

export interface RegisterData {
  email: string
  password: string
  displayName: string
}

export interface LoginData {
  email: string
  password: string
}

export interface AuthResponse {
  message: string
  token: string
  user: User
}
