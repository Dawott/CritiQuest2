import api from "./api"

export interface UserProfile {
  user: {
    id: string
    email: string
    displayName: string
    avatarUrl?: string
    joinedAt: string
    lastActive: string
  }
  progression: {
    level: number
    experience: number
    experienceForNextLevel: number
    experienceForCurrentLevel: number
    experienceToNextLevel: number
    currentStage: string
    completedLessons: string[]
    unlockedPhilosophers: string[]
  }
  stats: {
    totalTimeSpent: number
    streakDays: number
    lastStreakUpdate: string
    quizzesCompleted: number
    perfectScores: number
    gachaTickets: number
  }
  philosopherCollection: Array<{
    id: string
    philosopherId: string
    philosopherName: string
    philosopherRarity: string
    level: number
    experience: number
    duplicates: number
    obtainedAt: string
  }>
  achievements: Array<{
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
  }>
  recentActivity: Array<{
    lessonId: string
    lessonTitle: string
    score: number
    completedAt: string
    timeSpent: number
  }>
}

export interface UserStats {
  totalTimeSpent: number
  streakDays: number
  lastStreakUpdate: string
  quizzesCompleted: number
  perfectScores: number
  gachaTickets: number
  completedLessons: number
  philosopherCount: number
  completedAchievements: number
  averageQuizScore: number
}

export interface UpdateProfileRequest {
  displayName?: string
  avatarUrl?: string
}

export const profileApi = {
  getProfile: (): Promise<UserProfile> =>
    api.get('/profile').then(res => res.data),

  getStats: (): Promise<UserStats> =>
    api.get('/profile/stats').then(res => res.data),

  updateProfile: (data: UpdateProfileRequest) =>
    api.put('/profile', data).then(res => res.data),

  updateStreak: (): Promise<{ streak: number }> =>
    api.post('/profile/streak').then(res => res.data)
}
