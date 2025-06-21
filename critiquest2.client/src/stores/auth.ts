import { defineStore } from 'pinia'
import { ref, computed, readonly } from 'vue'
import type { User, LoginData, RegisterData } from '@/types/auth'
import { authApi } from '@/services/api'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const token = ref<string | null>(localStorage.getItem('auth_token'))
  const loading = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => !!token.value && !!user.value)

  const setToken = (newToken: string) => {
    console.log('🔐 Setting new token:', {
      tokenLength: newToken.length,
      tokenStart: newToken.substring(0, 20) + '...',
      timestamp: new Date().toISOString()
    })

    token.value = newToken
    localStorage.setItem('auth_token', newToken)

    // Verify token was saved
    const savedToken = localStorage.getItem('auth_token')
    console.log('✅ Token saved verification:', {
      saved: !!savedToken,
      matches: savedToken === newToken
    })
  }

  const clearAuth = () => {
    console.log('🧹 Clearing authentication')
    user.value = null
    token.value = null
    localStorage.removeItem('auth_token')
  }

  // Helper function to decode JWT payload (for debugging)
  const decodeTokenPayload = (token: string) => {
    try {
      const payload = token.split('.')[1]
      const decoded = JSON.parse(atob(payload))
      return decoded
    } catch (error) {
      console.error('Failed to decode token:', error)
      return null
    }
  }

  // Helper function to check if token is expired
  const isTokenExpired = (token: string) => {
    try {
      const payload = decodeTokenPayload(token)
      if (!payload || !payload.exp) return true

      const now = Math.floor(Date.now() / 1000)
      return payload.exp < now
    } catch {
      return true
    }
  }

  const validateToken = async () => {
    const currentToken = token.value
    if (!currentToken) {
      console.warn('⚠️ No token to validate')
      return false
    }

    console.log('🔍 Validating token:', {
      hasToken: !!currentToken,
      tokenLength: currentToken.length,
      isExpired: isTokenExpired(currentToken),
      payload: decodeTokenPayload(currentToken)
    })

    try {
      const response = await authApi.getCurrentUser()
      console.log('✅ Token validation successful:', response)
      return true
    } catch (error) {
      console.error('❌ Token validation failed:', error)
      return false
    }
  }

  const register = async (data: RegisterData) => {
    loading.value = true
    error.value = null

    try {
      console.log('📝 Starting registration process')
      const response = await authApi.register(data)

      console.log('✅ Registration successful:', {
        hasUser: !!response.user,
        hasToken: !!response.token,
        userId: response.user?.id
      })

      user.value = response.user
      setToken(response.token)
      return response
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Registration failed'
      console.error('❌ Registration failed:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  const login = async (data: LoginData) => {
    loading.value = true
    error.value = null

    try {
      console.log('🔐 Starting login process for:', data.email)
      const response = await authApi.login(data)

      console.log('✅ Login API successful:', {
        hasUser: !!response.user,
        hasToken: !!response.token,
        userId: response.user?.id,
        userEmail: response.user?.email
      })

      // Set user and token
      user.value = response.user
      setToken(response.token)

      // Verify everything is set correctly
      console.log('🔍 Post-login verification:', {
        userInStore: !!user.value,
        tokenInStore: !!token.value,
        tokenInLocalStorage: !!localStorage.getItem('auth_token'),
        isAuthenticated: isAuthenticated.value
      })

      return response
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Login failed'
      console.error('❌ Login failed:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  const logout = async () => {
    try {
      if (token.value) {
        await authApi.logout()
      }
    } catch (err) {
      // Silent fail for logout
      console.error('Logout error:', err)
    } finally {
      clearAuth()
    }
  }

  const fetchCurrentUser = async () => {
    if (!token.value) {
      console.warn('⚠️ No token available for fetching current user')
      return
    }

    try {
      console.log('👤 Fetching current user')
      const userData = await authApi.getCurrentUser()
      user.value = userData
      console.log('✅ Current user fetched:', userData)
    } catch (err) {
      console.error('❌ Failed to fetch current user:', err)
      clearAuth()
      throw err
    }
  }

  const initializeAuth = async () => {
    const savedToken = localStorage.getItem('auth_token')
    console.log('🚀 Initializing auth:', {
      hasSavedToken: !!savedToken,
      tokenInRef: !!token.value,
      userInRef: !!user.value
    })

    if (savedToken) {
      token.value = savedToken

      // Check if token is expired
      if (isTokenExpired(savedToken)) {
        console.warn('⚠️ Saved token is expired, clearing auth')
        clearAuth()
        return
      }

      try {
        await fetchCurrentUser()
        console.log('✅ Auth initialization successful')
      } catch (err) {
        console.error('❌ Auth initialization failed, clearing token')
        clearAuth()
      }
    }
  }

  return {
    user: readonly(user),
    token: readonly(token),
    loading: readonly(loading),
    error: readonly(error),
    isAuthenticated,
    register,
    login,
    logout,
    fetchCurrentUser,
    initializeAuth,
    clearAuth,
    validateToken // Add this for debugging
  }
})
