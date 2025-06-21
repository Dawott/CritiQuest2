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
    token.value = newToken
    localStorage.setItem('auth_token', newToken)
  }

  const clearAuth = () => {
    user.value = null
    token.value = null
    localStorage.removeItem('auth_token')
  }

  const register = async (data: RegisterData) => {
    loading.value = true
    error.value = null

    try {
      const response = await authApi.register(data)
      user.value = response.user
      setToken(response.token)
      return response
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Registration failed'
      throw err
    } finally {
      loading.value = false
    }
  }

  const login = async (data: LoginData) => {
    loading.value = true
    error.value = null

    try {
      const response = await authApi.login(data)
      user.value = response.user
      setToken(response.token)
      return response
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Login failed'
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
    if (!token.value) return

    try {
      const userData = await authApi.getCurrentUser()
      user.value = userData
    } catch (err) {
      clearAuth()
      throw err
    }
  }

  const initializeAuth = async () => {
    if (token.value) {
      try {
        await fetchCurrentUser()
      } catch (err) {
        // Token is invalid, clear it
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
    clearAuth
  }
})
