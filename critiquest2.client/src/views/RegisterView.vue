<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
          Join CritiQuest2
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          Start your journey through philosophical wisdom
        </p>
      </div>

      <form class="mt-8 space-y-6" @submit.prevent="handleRegister">
        <div v-if="authStore.error" class="bg-red-50 border border-red-200 rounded-md p-4">
          <p class="text-sm text-red-600">{{ authStore.error }}</p>
        </div>

        <div class="space-y-4">
          <div>
            <label for="displayName" class="form-label">Display Name</label>
            <input id="displayName"
                   v-model="form.displayName"
                   name="displayName"
                   type="text"
                   required
                   class="form-input"
                   placeholder="Choose a display name" />
          </div>

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
                   autocomplete="new-password"
                   required
                   class="form-input"
                   placeholder="Create a password" />
          </div>

          <div>
            <label for="confirmPassword" class="form-label">Confirm Password</label>
            <input id="confirmPassword"
                   v-model="form.confirmPassword"
                   name="confirmPassword"
                   type="password"
                   autocomplete="new-password"
                   required
                   class="form-input"
                   placeholder="Confirm your password" />
          </div>
        </div>

        <div>
          <button type="submit"
                  :disabled="authStore.loading || !isFormValid"
                  class="btn-primary w-full">
            <span v-if="authStore.loading">Creating account...</span>
            <span v-else>Create account</span>
          </button>
        </div>

        <div class="text-center">
          <p class="text-sm text-gray-600">
            Already have an account?
            <router-link to="/login" class="font-medium text-primary-600 hover:text-primary-500">
              Sign in
            </router-link>
          </p>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const form = reactive({
  displayName: '',
  email: '',
  password: '',
  confirmPassword: ''
})

const isFormValid = computed(() => {
  return form.password === form.confirmPassword && form.password.length >= 6
})

const handleRegister = async () => {
  if (!isFormValid.value) {
    return
  }

  try {
    await authStore.register({
      displayName: form.displayName,
      email: form.email,
      password: form.password
    })
    router.push('/dashboard')
  } catch (error) {
    // Error is handled by the store
  }
}
</script>
