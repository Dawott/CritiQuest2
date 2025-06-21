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

      <form class="mt-8 space-y-6" @submit.prevent="handleLogin">
        <div v-if="authStore.error" class="bg-red-50 border border-red-200 rounded-md p-4">
          <p class="text-sm text-red-600">{{ authStore.error }}</p>
        </div>

        <div class="space-y-4">
          <div>
            <label for="email" class="form-label">Email</label>
            <input id="email"
                   v-model="form.email"
                   name="email"
                   type="email"
                   autocomplete="email"
                   required
                   class="form-input"
                   placeholder="Wpisz email" />
          </div>

          <div>
            <label for="password" class="form-label">Hasło</label>
            <input id="password"
                   v-model="form.password"
                   name="password"
                   type="password"
                   autocomplete="current-password"
                   required
                   class="form-input"
                   placeholder="Wpisz hasło" />
          </div>
        </div>

        <div>
          <button type="submit"
                  :disabled="authStore.loading"
                  class="btn-primary w-full">
            <span v-if="authStore.loading">Loguję się...</span>
            <span v-else>Zaloguj</span>
          </button>
        </div>

        <div class="text-center">
          <p class="text-sm text-gray-600">
            Nie masz konta?
            <router-link to="/register" class="font-medium text-primary-600 hover:text-primary-500">
              Zarejestruj się
            </router-link>
          </p>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const form = reactive({
  email: '',
  password: ''
})

const handleLogin = async () => {
  try {
    await authStore.login(form)
    router.push('/dashboard')
  } catch (error) {
    // Error is handled by the store
  }
}
</script>
