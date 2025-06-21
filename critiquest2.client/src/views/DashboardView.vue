<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900">Welcome back, {{ user?.displayName }}!</h1>
      <p class="mt-2 text-gray-600">Continue your philosophical journey</p>
    </div>

    <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
      <div class="card">
        <div class="card-body">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Quick Stats</h3>
          <div class="space-y-2">
            <p class="text-sm text-gray-600">Level: <span class="font-medium">1</span></p>
            <p class="text-sm text-gray-600">Experience: <span class="font-medium">0 XP</span></p>
            <p class="text-sm text-gray-600">Philosophers: <span class="font-medium">0</span></p>
          </div>
        </div>
      </div>

      <div class="card">
        <div class="card-body">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Recent Activity</h3>
          <p class="text-sm text-gray-500">No recent activity yet. Start your first lesson!</p>
        </div>
      </div>

      <div class="card">
        <div class="card-body">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Next Steps</h3>
          <div class="space-y-2">
            <router-link to="/lessons" class="block text-sm text-primary-600 hover:text-primary-500">
              → Start your first lesson
            </router-link>
            <router-link to="/gacha" class="block text-sm text-primary-600 hover:text-primary-500">
              → Try the philosopher gacha
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const user = computed(() => authStore.user)
</script>
