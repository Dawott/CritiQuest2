<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
          Zaloguj się do CritiQuest
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          Rozpocznij swoją podróż
        </p>
      </div>

      <!-- Debug info (remove this after fixing) -->
      <div v-if="debugMode" class="bg-blue-50 border border-blue-200 rounded-md p-4 text-sm">
        <h4 class="font-medium text-blue-900 mb-2">Debug Info:</h4>
        <p><strong>Current URL:</strong> {{ window.location.href }}</p>
        <p><strong>API Base:</strong> /api</p>
        <p><strong>Form Data:</strong> {{ JSON.stringify(form) }}</p>
        <p><strong>Last Error:</strong> {{ lastError || 'None' }}</p>
      </div>

      <form class="mt-8 space-y-6" @submit.prevent.stop="handleLogin">
        <div v-if="authStore.error" class="bg-red-50 border border-red-200 rounded-md p-4">
          <p class="text-sm text-red-600">{{ authStore.error }}</p>
        </div>

        <div v-if="lastError" class="bg-orange-50 border border-orange-200 rounded-md p-4">
          <p class="text-sm text-orange-600">Debug: {{ lastError }}</p>
        </div>

        <div class="space-y-4">
          <div>
            <label for="email" class="form-label">Email address</label>
            <input id="email"
                   v-model="form.email"
                   name="email"
                   type="email"
                   autocomplete="email"
                   required
                   class="form-input"
                   placeholder="Enter your email" />
          </div>

          <div>
            <label for="password" class="form-label">Password</label>
            <input id="password"
                   v-model="form.password"
                   name="password"
                   type="password"
                   autocomplete="current-password"
                   required
                   class="form-input"
                   placeholder="Enter your password" />
          </div>
        </div>

        <div class="space-y-2">
          <button type="submit"
                  :disabled="authStore.loading"
                  class="btn-primary w-full">
            <span v-if="authStore.loading">Signing in...</span>
            <span v-else>Sign in</span>
          </button>

          <!-- Quick test buttons (remove after fixing) -->
          <div v-if="debugMode" class="space-y-2">
            <button type="button"
                    @click="testApi"
                    class="btn-secondary w-full text-xs">
              Test API Connection
            </button>
            <button type="button"
                    @click="fillTestData"
                    class="btn-secondary w-full text-xs">
              Fill Test Data
            </button>
          </div>
        </div>

        <div class="text-center">
          <p class="text-sm text-gray-600">
            Don't have an account?
            <router-link to="/register" class="font-medium text-primary-600 hover:text-primary-500">
              Sign up
            </router-link>
          </p>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { reactive, ref } from 'vue'
  import { useRouter } from 'vue-router'
  import { useAuthStore } from '@/stores/auth'

  const router = useRouter()
  const authStore = useAuthStore()

  // Set to true for debugging, false for production
  const debugMode = ref(false)
  const lastError = ref<string>('')

  const form = reactive({
    email: '',
    password: ''
  })

  const handleLogin = async (event?: Event) => {
    // Extra safety to prevent form submission
    if (event) {
      event.preventDefault()
      event.stopPropagation()
    }

    lastError.value = ''

    console.log('Login attempt started')
    console.log('Form data:', { email: form.email, passwordLength: form.password.length })

    try {
      console.log('Calling authStore.login...')
      await authStore.login(form)

      console.log('Login successful, redirecting to dashboard...')
      await router.push('/dashboard')

    } catch (error: any) {
      console.error('Login failed:', error)

      // Enhanced error logging
      if (error.response) {
        console.error('Response error:', error.response.status, error.response.data)
        lastError.value = `HTTP ${error.response.status}: ${JSON.stringify(error.response.data)}`
      } else if (error.request) {
        console.error('Network error - no response received')
        lastError.value = 'Network error: No response from server'
      } else {
        console.error('Error:', error.message)
        lastError.value = `Error: ${error.message}`
      }

      // The authStore should handle setting the error message
    }
  }

  const testApi = async () => {
    try {
      // Use the existing api service from the auth store
      const response = await fetch('/api/auth/test')
      if (response.ok) {
        const data = await response.json()
        lastError.value = `API Test Success: ${JSON.stringify(data)}`
      } else {
        lastError.value = `API Test Failed: ${response.status} ${response.statusText}`
      }
    } catch (error: any) {
      lastError.value = `API Test Error: ${error.message}`
    }
  }

  const fillTestData = () => {
    form.email = 'test@example.com'
    form.password = 'password123'
  }

  // Prevent any form submission events
  const preventSubmit = (event: Event) => {
    event.preventDefault()
    event.stopPropagation()
    return false
  }
</script>
