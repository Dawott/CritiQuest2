import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { profileApi, type UserProfile, type UserStats, type UpdateProfileRequest } from '@/services/profileApi'

export const useProfileStore = defineStore('profile', () => {
  const profile = ref<UserProfile | null>(null)
  const stats = ref<UserStats | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const isLoaded = computed(() => profile.value !== null)

  const levelProgress = computed(() => {
    if (!profile.value) return { percentage: 0, current: 0, next: 0 }

    const { experience, experienceForCurrentLevel, experienceForNextLevel } = profile.value.progression
    const currentLevelExp = experience - experienceForCurrentLevel
    const nextLevelExp = experienceForNextLevel - experienceForCurrentLevel
    const percentage = nextLevelExp > 0 ? Math.round((currentLevelExp / nextLevelExp) * 100) : 0

    return {
      percentage,
      current: currentLevelExp,
      next: nextLevelExp
    }
  })

  const completionStats = computed(() => {
    if (!profile.value) return { lessonsCompleted: 0, philosophersOwned: 0, achievementsUnlocked: 0 }

    return {
      lessonsCompleted: profile.value.progression.completedLessons.length,
      philosophersOwned: profile.value.philosopherCollection.length,
      achievementsUnlocked: profile.value.achievements.filter(a => a.completed).length
    }
  })

  const fetchProfile = async () => {
    loading.value = true
    error.value = null

    try {
      const data = await profileApi.getProfile()
      profile.value = data
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch profile'
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchStats = async () => {
    loading.value = true
    error.value = null

    try {
      const data = await profileApi.getStats()
      stats.value = data
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch stats'
      throw err
    } finally {
      loading.value = false
    }
  }

  const updateProfile = async (data: UpdateProfileRequest) => {
    loading.value = true
    error.value = null

    try {
      await profileApi.updateProfile(data)
      // Refresh profile data
      await fetchProfile()
    } catch (err: any) {
      error.value = err.message || 'Failed to update profile'
      throw err
    } finally {
      loading.value = false
    }
  }

  const updateStreak = async () => {
    try {
      const result = await profileApi.updateStreak()
      // Update the streak in current profile data
      if (profile.value) {
        profile.value.stats.streakDays = result.streak
      }
      return result.streak
    } catch (err: any) {
      error.value = err.message || 'Failed to update streak'
      throw err
    }
  }

  const clearProfile = () => {
    profile.value = null
    stats.value = null
  }

  return {
    profile,
    stats,
    loading,
    error,
    isLoaded,
    levelProgress,
    completionStats,
    fetchProfile,
    fetchStats,
    updateProfile,
    updateStreak,
    clearProfile
  }
})
