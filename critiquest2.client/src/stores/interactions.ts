import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/services/api'

export interface InteractionResponse {
  lessonId: string
  sectionId: string
  interactionType: string
  responseData: any
}

export interface SavedInteractionResponse {
  id: string
  lessonId: string
  sectionId: string
  interactionType: string
  responseData: any
  createdAt: string
  updatedAt?: string
}

export const useInteractionsStore = defineStore('interactions', () => {
  const savedResponses = ref<Record<string, SavedInteractionResponse>>({})
  const loading = ref(false)
  const error = ref<string | null>(null)

  const saveInteractionResponse = async (response: InteractionResponse) => {
    loading.value = true
    error.value = null

    try {
      const result = await api.post('/interactions/responses', response)
      const key = `${response.lessonId}-${response.sectionId}`
      savedResponses.value[key] = result.data
      return result.data
    } catch (err: any) {
      error.value = err.message || 'Failed to save interaction response'
      throw err
    } finally {
      loading.value = false
    }
  }

  const getInteractionResponse = async (lessonId: string, sectionId: string) => {
    const key = `${lessonId}-${sectionId}`

    // Return cached if available
    if (savedResponses.value[key]) {
      return savedResponses.value[key]
    }

    loading.value = true
    error.value = null

    try {
      const result = await api.get(`/interactions/responses/${lessonId}/${sectionId}`)
      if (result.data) {
        savedResponses.value[key] = result.data
        return result.data
      }
      return null
    } catch (err: any) {
      if (err.response?.status !== 404) {
        error.value = err.message || 'Failed to fetch interaction response'
      }
      return null
    } finally {
      loading.value = false
    }
  }

  const getAllLessonResponses = async (lessonId: string) => {
    loading.value = true
    error.value = null

    try {
      const result = await api.get(`/interactions/lessons/${lessonId}/responses`)
      const responses = result.data || []

      // Cache the responses
      responses.forEach((response: SavedInteractionResponse) => {
        const key = `${response.lessonId}-${response.sectionId}`
        savedResponses.value[key] = response
      })

      return responses
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch lesson responses'
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    savedResponses,
    loading,
    error,
    saveInteractionResponse,
    getInteractionResponse,
    getAllLessonResponses
  }
})
