import api from './api'

export interface InteractiveSection {
  id: string
  lessonId: string
  title: string
  description: string
  type: string
  orderInLesson: number
  configuration: any
  isRequired: boolean
  estimatedTimeMinutes: number
  userResponse?: UserResponse
}

export interface UserResponse {
  id: string
  responseData: any
  timeSpentSeconds: number
  isCompleted: boolean
  completionPercentage: number
  qualityScore: number
  lastUpdatedAt: string
}

export interface InteractionProgress {
  lessonId: string
  totalSections: number
  completedSections: number
  completionPercentage: number
  totalTimeSpentSeconds: number
  lastActivityAt: string
}

export interface SaveResponseRequest {
  responseData: any
  timeSpentSeconds: number
  isCompleted: boolean
}

export const interactiveScenariosApi = {
  // Get all interactive sections for a lesson
  getLessonSections: (lessonId: string): Promise<InteractiveSection[]> =>
    api.get(`/interactive-scenarios/lessons/${lessonId}/sections`).then(res => res.data),

  // Get specific interactive section
  getSection: (sectionId: string): Promise<InteractiveSection> =>
    api.get(`/interactive-scenarios/sections/${sectionId}`).then(res => res.data),

  // Save user response
  saveResponse: (sectionId: string, request: SaveResponseRequest): Promise<UserResponse> =>
    api.post(`/interactive-scenarios/sections/${sectionId}/responses`, request).then(res => res.data),

  // Get lesson progress
  getLessonProgress: (lessonId: string): Promise<InteractionProgress> =>
    api.get(`/interactive-scenarios/lessons/${lessonId}/progress`).then(res => res.data)
}
