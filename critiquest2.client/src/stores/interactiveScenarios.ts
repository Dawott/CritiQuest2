import { defineStore } from 'pinia'
import { ref } from 'vue'
import { interactiveScenariosApi, type InteractiveSection, type UserResponse, type InteractionProgress } from '@/services/interactiveScenariosApi'

export const useInteractiveScenariosStore = defineStore('interactiveScenarios', () => {
  const lessonSections = ref<Record<string, InteractiveSection[]>>({})
  const currentSection = ref<InteractiveSection | null>(null)
  const lessonProgress = ref<Record<string, InteractionProgress>>({})
  const loading = ref(false)
  const error = ref<string | null>(null)

  const getLessonSections = async (lessonId: string) => {
    loading.value = true
    error.value = null

    try {
      const sections = await interactiveScenariosApi.getLessonSections(lessonId)
      lessonSections.value[lessonId] = sections
      return sections
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch interactive sections'
      throw err
    } finally {
      loading.value = false
    }
  }

  const getSection = async (sectionId: string) => {
    loading.value = true
    error.value = null

    try {
      const section = await interactiveScenariosApi.getSection(sectionId)
      currentSection.value = section
      return section
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch section'
      throw err
    } finally {
      loading.value = false
    }
  }

  const saveResponse = async (sectionId: string, responseData: any, timeSpent: number, isCompleted: boolean = false) => {
    try {
      const response = await interactiveScenariosApi.saveResponse(sectionId, {
        responseData,
        timeSpentSeconds: timeSpent,
        isCompleted
      })

      // Update the current section with the new response
      if (currentSection.value?.id === sectionId) {
        currentSection.value.userResponse = response
      }

      // Update sections in cache
      Object.values(lessonSections.value).forEach(sections => {
        const section = sections.find(s => s.id === sectionId)
        if (section) {
          section.userResponse = response
        }
      })

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to save response'
      throw err
    }
  }

  const getLessonProgress = async (lessonId: string) => {
    try {
      const progress = await interactiveScenariosApi.getLessonProgress(lessonId)
      lessonProgress.value[lessonId] = progress
      return progress
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch progress'
      throw err
    }
  }

  return {
    lessonSections,
    currentSection,
    lessonProgress,
    loading,
    error,
    getLessonSections,
    getSection,
    saveResponse,
    getLessonProgress
  }
})
