import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { lessonApi, type Lesson, type LessonCompletion } from '@/services/lessonApi'

export const useLessonsStore = defineStore('lessons', () => {
  const lessons = ref<Lesson[]>([])
  const currentLesson = ref<Lesson | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const lessonsByStage = computed(() => {
    const grouped: Record<string, Lesson[]> = {}
    lessons.value.forEach(lesson => {
      if (!grouped[lesson.stage]) {
        grouped[lesson.stage] = []
      }
      grouped[lesson.stage].push(lesson)
    })

    // Sort lessons within each stage by order
    Object.keys(grouped).forEach(stage => {
      grouped[stage].sort((a, b) => a.order - b.order)
    })

    return grouped
  })

  const completedLessons = computed(() =>
    lessons.value.filter(lesson => lesson.isCompleted)
  )

  const availableLessons = computed(() =>
    lessons.value.filter(lesson => !lesson.requiredPhilosopher || lesson.isCompleted)
  )

  const fetchLessons = async (stage?: string) => {
    loading.value = true
    error.value = null

    try {
      const data = await lessonApi.getLessons(stage)
      lessons.value = data
    } catch (err: any) {
      error.value = err.message || 'Nie udało się pobrać lekcji'
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchLesson = async (id: string) => {
    loading.value = true
    error.value = null

    try {
      const data = await lessonApi.getLesson(id)
      currentLesson.value = data

      // Update the lesson in the lessons array if it exists
      const index = lessons.value.findIndex(l => l.id === id)
      if (index !== -1) {
        lessons.value[index] = data
      }

      return data
    } catch (err: any) {
      error.value = err.message || 'Nie udało się pobrać lekcji'
      throw err
    } finally {
      loading.value = false
    }
  }

  const completeLesson = async (id: string, completion: LessonCompletion) => {
    try {
      await lessonApi.completeLesson(id, completion)

      // Refresh the lesson to get updated progress
      await fetchLesson(id)

      // Refresh all lessons to update completion status
      await fetchLessons()

    } catch (err: any) {
      error.value = err.message || 'Nie udało się zakończyć lekcji'
      throw err
    }
  }

  const clearCurrentLesson = () => {
    currentLesson.value = null
  }

  return {
    lessons,
    currentLesson,
    loading,
    error,
    lessonsByStage,
    completedLessons,
    availableLessons,
    fetchLessons,
    fetchLesson,
    completeLesson,
    clearCurrentLesson
  }
})
