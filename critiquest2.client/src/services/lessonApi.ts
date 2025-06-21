import api from './api'

export interface Lesson {
  id: string
  title: string
  description: string
  stage: string
  order: number
  difficulty: string
  estimatedTime: number
  philosophicalConcepts: string[]
  requiredPhilosopher?: string
  quizId: string
  rewards: {
    xp: number
    coins: number
    unlockedContent: string[]
  }
  userProgress?: {
    completedAt: string
    score: number
    timeSpent: number
    attempts: number
    bestScore: number
    notes?: string
  }
  isCompleted: boolean
}

export interface LessonCompletion {
  score: number
  timeSpent: number
  notes?: string
}

export const lessonApi = {
  getLessons: (stage?: string): Promise<Lesson[]> =>
    api.get(`/lessons${stage ? `?stage=${stage}` : ''}`).then(res => res.data),

  getLesson: (id: string): Promise<Lesson> =>
    api.get(`/lessons/${id}`).then(res => res.data),

  completeLesson: (id: string, completion: LessonCompletion) =>
    api.post(`/lessons/${id}/complete`, completion).then(res => res.data)
}
