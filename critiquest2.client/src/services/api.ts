import axios from 'axios'
import type { AuthResponse, LoginData, RegisterData } from '@/types/auth'

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor to add auth token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('auth_token')

  // Debug logging
  console.log('🔍 API Request Debug:', {
    url: config.url,
    method: config.method?.toUpperCase(),
    hasToken: !!token,
    tokenStart: token ? token.substring(0, 20) + '...' : 'No token',
    headers: config.headers
  })

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
    console.log('✅ Authorization header added')
  } else {
    console.warn('⚠️ No auth token found in localStorage')
  }

  return config
}, (error) => {
  console.error('❌ Request interceptor error:', error)
  return Promise.reject(error)
})

// Response interceptor to handle auth errors
api.interceptors.response.use(
  (response) => {
    console.log('✅ API Response Success:', {
      url: response.config.url,
      status: response.status,
      data: response.data ? 'Data received' : 'No data'
    })
    return response
  },
  (error) => {
    console.error('❌ API Response Error:', {
      url: error.config?.url,
      status: error.response?.status,
      statusText: error.response?.statusText,
      data: error.response?.data,
      message: error.message
    })

    if (error.response?.status === 401) {
      console.warn('🔐 401 Unauthorized - clearing token and redirecting to login')
      localStorage.removeItem('auth_token')

      // Only redirect if we're not already on the login page
      if (window.location.pathname !== '/login') {
        window.location.href = '/login'
      }
    }
    return Promise.reject(error)
  }
)

export const authApi = {
  register: (data: RegisterData): Promise<AuthResponse> =>
    api.post('/auth/register', data).then(res => res.data),

  login: (data: LoginData): Promise<AuthResponse> =>
    api.post('/auth/login', data).then(res => res.data),

  getCurrentUser: () =>
    api.get('/auth/me').then(res => res.data),

  logout: () =>
    api.post('/auth/logout').then(res => res.data),
}

export default api
