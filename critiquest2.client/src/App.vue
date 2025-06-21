<template>
  <div id="app" class="min-h-screen bg-gray-50">
    <nav v-if="isAuthenticated" class="bg-white shadow-sm border-b">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex items-center">
            <router-link to="/dashboard" class="text-xl font-bold text-primary-600">
              CritiQuest2
            </router-link>
            <div class="ml-8 flex space-x-4">
              <router-link to="/dashboard" class="nav-link">Dashboard</router-link>
              <router-link to="/lessons" class="nav-link">Lessons</router-link>
              <router-link to="/philosophers" class="nav-link">Philosophers</router-link>
              <router-link to="/gacha" class="nav-link">Gacha</router-link>
            </div>
          </div>

          <div class="flex items-center space-x-4">
            <span class="text-sm text-gray-700">{{ user?.displayName }}</span>
            <button @click="handleLogout" class="btn-secondary">
              Logout
            </button>
          </div>
        </div>
      </div>
    </nav>

    <main class="flex-1">
      <router-view />
    </main>

    <!-- Loading overlay -->
    <div v-if="authLoading" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white p-6 rounded-lg">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600 mx-auto"></div>
        <p class="mt-2 text-sm text-gray-600">Loading...</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { computed, onMounted } from 'vue'
  import { useAuthStore } from './stores/auth'
  import { useRouter } from 'vue-router'

  const authStore = useAuthStore()
  const router = useRouter()

  const isAuthenticated = computed(() => authStore.isAuthenticated)
  const user = computed(() => authStore.user)
  const authLoading = computed(() => authStore.loading)

  const handleLogout = async () => {
    await authStore.logout()
    router.push('/login')
  }

  onMounted(async () => {
    await authStore.initializeAuth()
  })
</script>

<style scoped>
  .nav-link {
    @apply px-3 py-2 rounded-md text-sm font-medium text-gray-700 hover:text-primary-600 hover:bg-primary-50 transition-colors;
  }

    .nav-link.router-link-active {
      @apply text-primary-600 bg-primary-50;
    }

  .btn-secondary {
    @apply px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 transition-colors;
  }
</style>
