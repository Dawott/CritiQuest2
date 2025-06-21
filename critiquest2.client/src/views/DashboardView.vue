<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900">
        Witaj ponownie, {{ profileStore.profile?.user.displayName || 'Filozofie' }}!
      </h1>
      <p class="mt-2 text-gray-600">Kontynuuj swoją podróż po wiedzę</p>
    </div>

    <!-- Loading State -->
    <div v-if="profileStore.loading" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
    </div>

    <!-- Dashboard Content -->
    <div v-else-if="profileStore.profile" class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
      <!-- Quick Stats -->
      <div class="card">
        <div class="card-body">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Przegląd progresu</h3>
          <div class="space-y-3">
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-600">Poziom</span>
              <span class="font-medium">{{ profileStore.profile.progression.level }}</span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-600">Doświadczenie</span>
              <span class="font-medium">{{ profileStore.profile.progression.experience }} XP</span>
            </div>
            <div class="mt-2">
              <div class="flex justify-between text-sm text-gray-600 mb-1">
                <span>Postęp do awansu</span>
                <span>{{ levelProgress.percentage }}%</span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-2">
                <div class="bg-primary-600 h-2 rounded-full transition-all duration-300"
                     :style="{ width: `${levelProgress.percentage}%` }"></div>
              </div>
            </div>
            <div class="text-xs text-gray-500 mt-1">
              {{ profileStore.profile.progression.experienceToNextLevel }} XP na kolejny poziom
            </div>
          </div>
        </div>
      </div>

      <!-- Collection Stats -->
      <div class="card">
        <div class="card-body">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Mój Aeropag</h3>
          <div class="space-y-3">
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-600">Filozofowie</span>
              <span class="font-medium">{{ profileStore.profile.philosopherCollection.length }}</span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-600">Nabyta wiedza</span>
              <span class="font-medium">{{ profileStore.profile.progression.completedLessons.length }}</span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-600">Wizyty w Wyroczni</span>
              <span class="font-medium text-primary-600">{{ profileStore.profile.stats.gachaTickets }}</span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-600">Aktualny rekord</span>
              <span class="font-medium text-orange-600">{{ profileStore.profile.stats.streakDays }} dni</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Activity -->
      <div class="card">
        <div class="card-body">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Ostatnia aktywność</h3>
          <div v-if="profileStore.profile.recentActivity.length > 0" class="space-y-3">
            <div v-for="activity in profileStore.profile.recentActivity.slice(0, 3)"
                 :key="activity.lessonId"
                 class="flex justify-between items-center text-sm">
              <div>
                <p class="font-medium text-gray-900 truncate">{{ activity.lessonTitle }}</p>
                <p class="text-gray-500">Wynik: {{ activity.score }}%</p>
              </div>
              <div class="text-xs text-gray-400">
                {{ formatDate(activity.completedAt) }}
              </div>
            </div>
          </div>
          <p v-else class="text-sm text-gray-500">Brak ostatniej aktywności. Zdobądź nową wiedzę!</p>
        </div>
      </div>

      <!-- Achievements -->
      <div class="card">
        <div class="card-body">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Ostatnie osiągnięcia</h3>
          <div v-if="recentAchievements.length > 0" class="space-y-2">
            <div v-for="achievement in recentAchievements.slice(0, 3)"
                 :key="achievement.achievementId"
                 class="flex items-center space-x-2">
              <div class="w-2 h-2 bg-yellow-400 rounded-full"></div>
              <div class="flex-1">
                <p class="text-sm font-medium text-gray-900">{{ achievement.achievement.name }}</p>
                <p class="text-xs text-gray-500">{{ achievement.achievement.description }}</p>
              </div>
            </div>
          </div>
          <p v-else class="text-sm text-gray-500">Brak osiągnięć. Wiem, że możesz więcej!</p>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="card">
        <div class="card-body">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Szybkie akcje</h3>
          <div class="space-y-2">
            <router-link to="/lessons"
                         class="block w-full text-left px-3 py-2 text-sm text-primary-600 hover:text-primary-500 hover:bg-primary-50 rounded-md transition-colors">
              → Kontynuuj naukę
            </router-link>
            <router-link to="/philosophers"
                         class="block w-full text-left px-3 py-2 text-sm text-primary-600 hover:text-primary-500 hover:bg-primary-50 rounded-md transition-colors">
              → Mój Gimnazjon
            </router-link>
            <router-link v-if="profileStore.profile.stats.gachaTickets > 0"
                         to="/gacha"
                         class="block w-full text-left px-3 py-2 text-sm text-primary-600 hover:text-primary-500 hover:bg-primary-50 rounded-md transition-colors">
              → Odwiedź wyrocznię: ({{ profileStore.profile.stats.gachaTickets }} wizyt)
            </router-link>
          </div>
        </div>
      </div>

      <!-- Study Stats -->
      <div class="card">
        <div class="card-body">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Statystyki</h3>
          <div class="space-y-2">
            <div class="flex justify-between items-center text-sm">
              <span class="text-gray-600">Zakończonych Quizów</span>
              <span class="font-medium">{{ profileStore.profile.stats.quizzesCompleted }}</span>
            </div>
            <div class="flex justify-between items-center text-sm">
              <span class="text-gray-600">Perfekcyjne wyniki</span>
              <span class="font-medium text-green-600">{{ profileStore.profile.stats.perfectScores }}</span>
            </div>
            <div class="flex justify-between items-center text-sm">
              <span class="text-gray-600">Czas spędzony</span>
              <span class="font-medium">{{ formatTime(profileStore.profile.stats.totalTimeSpent) }}</span>
            </div>
            <div class="flex justify-between items-center text-sm">
              <span class="text-gray-600">Aktualny etap</span>
              <span class="font-medium capitalize">{{ formatStage(profileStore.profile.progression.currentStage) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Error State -->
    <div v-else-if="profileStore.error" class="text-center py-12">
      <div class="text-red-600 mb-4">{{ profileStore.error }}</div>
      <button @click="loadDashboardData"
              class="btn-primary">
        Retry
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { computed, onMounted } from 'vue'
  import { useProfileStore } from '@/stores/profile'

  const profileStore = useProfileStore()

  const levelProgress = computed(() => profileStore.levelProgress)

  const recentAchievements = computed(() =>
    profileStore.profile?.achievements
      .filter(a => a.completed)
      .sort((a, b) => new Date(b.unlockedAt || '').getTime() - new Date(a.unlockedAt || '').getTime()) || []
  )

  const formatDate = (dateString: string) => {
    const date = new Date(dateString)
    const now = new Date()
    const diffTime = Math.abs(now.getTime() - date.getTime())
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))

    if (diffDays === 1) return 'Dzisiaj'
    if (diffDays === 2) return 'Wczoraj'
    if (diffDays <= 7) return `${diffDays} dni temu`

    return date.toLocaleDateString()
  }

  const formatTime = (minutes: number) => {
    if (minutes < 60) return `${minutes}m`
    const hours = Math.floor(minutes / 60)
    const mins = minutes % 60
    return `${hours}h ${mins}m`
  }

  const formatStage = (stage: string) => {
    return stage.replace(/-/g, ' ')
  }

  const loadDashboardData = async () => {
    try {
      await profileStore.fetchProfile()
      await profileStore.updateStreak() // Update daily streak
    } catch (error) {
      console.error('Failed to load dashboard data:', error)
    }
  }

  onMounted(() => {
    loadDashboardData()
  })
</script>
