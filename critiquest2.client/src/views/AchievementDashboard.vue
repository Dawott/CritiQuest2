<template>
  <div class="achievement-dashboard">
    <!-- Progress Overview Card -->
    <div class="progress-overview bg-gradient-to-r from-blue-500 to-purple-600 rounded-xl p-6 text-white mb-6">
      <div class="flex items-center justify-between">
        <div>
          <h2 class="text-2xl font-bold mb-2">Lewel {{ progressSummary.currentLevel }}</h2>
          <p class="text-blue-100 mb-4">Filozof w szkoleniu</p>

          <!-- XP Progress Bar -->
          <div class="w-64">
            <div class="flex justify-between text-sm mb-1">
              <span>{{ progressSummary.currentExperience }} XP</span>
              <span>{{ progressSummary.experienceForNextLevel }} XP</span>
            </div>
            <div class="w-full bg-blue-400 bg-opacity-30 rounded-full h-3">
              <div class="bg-white h-3 rounded-full transition-all duration-500 ease-out"
                   :style="{ width: `${progressBarWidth}%` }"></div>
            </div>
            <p class="text-xs text-blue-100 mt-1">
              {{ progressSummary.experienceToNextLevel }} XP do następnego poziomu
            </p>
          </div>
        </div>

        <!-- Level Icon -->
        <div class="text-6xl opacity-20">
          🏛️
        </div>
      </div>
    </div>

    <!-- Quick Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white rounded-lg p-4 shadow-md border-l-4 border-green-500">
        <div class="flex items-center">
          <div class="text-green-500 text-2xl mr-3">🏆</div>
          <div>
            <p class="text-sm text-gray-600">Osiągnięcia</p>
            <p class="text-xl font-bold">{{ progressSummary.unlockedAchievements }}/{{ progressSummary.totalAchievements }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-lg p-4 shadow-md border-l-4 border-blue-500">
        <div class="flex items-center">
          <div class="text-blue-500 text-2xl mr-3">📚</div>
          <div>
            <p class="text-sm text-gray-600">Quizy</p>
            <p class="text-xl font-bold">{{ progressSummary.quizzesCompleted }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-lg p-4 shadow-md border-l-4 border-yellow-500">
        <div class="flex items-center">
          <div class="text-yellow-500 text-2xl mr-3">⭐</div>
          <div>
            <p class="text-sm text-gray-600">Perfekcyjny Wynik</p>
            <p class="text-xl font-bold">{{ progressSummary.perfectScores }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-lg p-4 shadow-md border-l-4 border-purple-500">
        <div class="flex items-center">
          <div class="text-purple-500 text-2xl mr-3">🔥</div>
          <div>
            <p class="text-sm text-gray-600">Streak</p>
            <p class="text-xl font-bold">{{ progressSummary.streakDays }} days</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Achievements -->
    <div v-if="recentAchievements.length > 0" class="mb-6">
      <h3 class="text-lg font-semibold mb-3 flex items-center">
        <span class="mr-2">🎉</span>
        Ostatnie osiągnięcia
      </h3>
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <div v-for="achievement in recentAchievements"
             :key="achievement.achievementId"
             class="bg-gradient-to-r from-yellow-400 to-orange-500 rounded-lg p-4 text-white shadow-lg transform hover:scale-105 transition-transform">
          <div class="flex items-center mb-2">
            <div class="text-2xl mr-3">{{ getAchievementIcon(achievement.achievement.name) }}</div>
            <div>
              <h4 class="font-bold">{{ achievement.achievement.name }}</h4>
              <p class="text-xs opacity-90">{{ formatDate(achievement.unlockedAt) }}</p>
            </div>
          </div>
          <p class="text-sm opacity-90">{{ achievement.achievement.description }}</p>
          <div class="mt-2 text-xs">
            <span class="bg-white bg-opacity-20 px-2 py-1 rounded">
              +{{ achievement.achievement.rewardExperience }} XP
            </span>
            <span class="bg-white bg-opacity-20 px-2 py-1 rounded ml-1">
              +{{ achievement.achievement.rewardGachaTickets }} 🎫
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- All Achievements -->
    <div>
      <div class="flex items-center justify-between mb-4">
        <h3 class="text-lg font-semibold">Wszystkie Osiągnięcia</h3>
        <div class="flex space-x-2">
          <button @click="filter = 'all'"
                  :class="filterButtonClass('all')">
            Wszystkie
          </button>
          <button @click="filter = 'completed'"
                  :class="filterButtonClass('completed')">
            Zakończone
          </button>
          <button @click="filter = 'in-progress'"
                  :class="filterButtonClass('in-progress')">
            W trakcie
          </button>
        </div>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <div v-for="achievement in filteredAchievements"
             :key="achievement.achievementId"
             :class="achievementCardClass(achievement)">
          <div class="flex items-center mb-3">
            <div class="text-2xl mr-3 opacity-75">
              {{ getAchievementIcon(achievement.achievement.name) }}
            </div>
            <div class="flex-1">
              <h4 class="font-bold text-gray-900">{{ achievement.achievement.name }}</h4>
              <p class="text-xs text-gray-600">{{ getTierText(achievement.achievement) }}</p>
            </div>
            <div v-if="achievement.completed" class="text-green-500 text-xl">✓</div>
          </div>

          <p class="text-sm text-gray-600 mb-3">{{ achievement.achievement.description }}</p>

          <!-- Progress Bar -->
          <div class="mb-3">
            <div class="flex justify-between text-xs text-gray-600 mb-1">
              <span>{{ achievement.currentValue }}/{{ achievement.targetValue }}</span>
              <span>{{ Math.round(achievement.progress) }}%</span>
            </div>
            <div class="w-full bg-gray-200 rounded-full h-2">
              <div :class="achievement.completed ? 'bg-green-500' : 'bg-blue-500'"
                   class="h-2 rounded-full transition-all duration-300"
                   :style="{ width: `${Math.min(achievement.progress, 100)}%` }"></div>
            </div>
          </div>

          <!-- Rewards -->
          <div class="flex justify-between items-center">
            <div class="text-xs text-gray-500">
              <span class="mr-2">{{ achievement.achievement.rewardExperience }} XP</span>
              <span>{{ achievement.achievement.rewardGachaTickets }} 🎫</span>
            </div>
            <div v-if="achievement.completed && achievement.unlockedAt" class="text-xs text-green-600">
              {{ formatDate(achievement.unlockedAt) }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, computed, onMounted } from 'vue'
import { profileApi } from '@/services/profileApi'

interface Achievement {
  achievementId: string
  achievement: {
    name: string
    description: string
    rewardExperience: number
    rewardGachaTickets: number
  }
  currentValue: number
  targetValue: number
  completed: boolean
  unlockedAt?: string
  viewed: boolean
  progress: number
}

interface ProgressionSummary {
  currentLevel: number
  currentExperience: number
  experienceForCurrentLevel: number
  experienceForNextLevel: number
  experienceToNextLevel: number
  unlockedAchievements: number
  totalAchievements: number
  quizzesCompleted: number
  perfectScores: number
  streakDays: number
}

export default defineComponent({
  name: 'AchievementDashboard',
  setup() {
    const achievements = ref<Achievement[]>([])
    const progressSummary = ref<ProgressionSummary>({
      currentLevel: 1,
      currentExperience: 0,
      experienceForCurrentLevel: 0,
      experienceForNextLevel: 100,
      experienceToNextLevel: 100,
      unlockedAchievements: 0,
      totalAchievements: 0,
      quizzesCompleted: 0,
      perfectScores: 0,
      streakDays: 0
    })
    const filter = ref<'all' | 'completed' | 'in-progress'>('all')
    const loading = ref(true)

    const progressBarWidth = computed(() => {
      const current = progressSummary.value.currentExperience - progressSummary.value.experienceForCurrentLevel
      const needed = progressSummary.value.experienceForNextLevel - progressSummary.value.experienceForCurrentLevel
      return needed > 0 ? Math.min((current / needed) * 100, 100) : 0
    })

    const recentAchievements = computed(() => {
      return achievements.value
        .filter(a => a.completed && a.unlockedAt)
        .sort((a, b) => new Date(b.unlockedAt!).getTime() - new Date(a.unlockedAt!).getTime())
        .slice(0, 3)
    })

    const filteredAchievements = computed(() => {
      switch (filter.value) {
        case 'completed':
          return achievements.value.filter(a => a.completed)
        case 'in-progress':
          return achievements.value.filter(a => !a.completed && a.currentValue > 0)
        default:
          return achievements.value
      }
    })

    const filterButtonClass = (filterType: string) => {
      const baseClass = 'px-3 py-1 text-sm rounded-md transition-colors'
      return filter.value === filterType
        ? `${baseClass} bg-blue-500 text-white`
        : `${baseClass} bg-gray-200 text-gray-700 hover:bg-gray-300`
    }

    const achievementCardClass = (achievement: Achievement) => {
      const baseClass = 'bg-white rounded-lg p-4 shadow-md border-2 transition-all hover:shadow-lg'
      return achievement.completed
        ? `${baseClass} border-green-200 bg-green-50`
        : `${baseClass} border-gray-200`
    }

    const getAchievementIcon = (name: string): string => {
      const iconMap: Record<string, string> = {
        'Doskonałość Filozofa': '🎯',
        'Myśliciel': '🧠',
        'Szybki Neuron': '⚡',
        'Wyjście z Jaskini': '🕳️',
        'Gimnazjon': '🏛️',
        'Akademia': '📚',
        'Lyceum': '🎓',
        'Biblioteka Aleksandryjska': '📜',
        'Filozof Nocy': '🌙',
        'Śpiący Sokrates': '😴',
        'Poranny Ptaszek': '🐦',
        'Myśliciel': '🤔',
        'Sofista': '💭',
        'Dialektyk': '⚖️',
        'Omnibus Dubitandum': '❓'
      }
      return iconMap[name] || '🏆'
    }

    const getTierText = (achievement: any): string => {
      const totalReward = achievement.rewardExperience + (achievement.rewardGachaTickets * 100)
      if (totalReward >= 1500) return 'Platyna'
      if (totalReward >= 700) return 'Złoto'
      if (totalReward >= 300) return 'Srebro'
      return 'Brąz'
    }

    const formatDate = (dateString?: string): string => {
      if (!dateString) return ''
      return new Date(dateString).toLocaleDateString('pl-PL', {
        day: 'numeric',
        month: 'short'
      })
    }

    const loadData = async () => {
      try {
        loading.value = true
        const [profile, summary] = await Promise.all([
          profileApi.getProfile(),
          // You'll need to create this endpoint in your ProfileController
          fetch('/api/profile/progression-summary').then(r => r.json())
        ])

        achievements.value = profile.achievements
        progressSummary.value = summary
      } catch (error) {
        console.error('Error loading achievement data:', error)
      } finally {
        loading.value = false
      }
    }

    onMounted(() => {
      loadData()
    })

    return {
      achievements,
      progressSummary,
      filter,
      loading,
      progressBarWidth,
      recentAchievements,
      filteredAchievements,
      filterButtonClass,
      achievementCardClass,
      getAchievementIcon,
      getTierText,
      formatDate
    }
  }
})
</script>

<style scoped>
  .achievement-dashboard {
    max-width: 1200px;
    margin: 0 auto;
    padding: 20px;
  }

  @media (max-width: 768px) {
    .achievement-dashboard {
      padding: 10px;
    }
  }
</style>
