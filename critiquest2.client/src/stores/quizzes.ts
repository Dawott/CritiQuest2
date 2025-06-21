import { defineStore } from 'pinia'
import { ref } from 'vue'
import { quizApi, type Quiz, type QuizSubmission, type QuizResult } from '@/services/quizApi'

export const useQuizzesStore = defineStore('quizzes', () => {
  const currentQuiz = ref<Quiz | null>(null)
  const currentAttemptId = ref<string | null>(null)
  const quizResult = ref<QuizResult | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const fetchQuiz = async (id: string) => {
    loading.value = true
    error.value = null

    try {
      const data = await quizApi.getQuiz(id)
      currentQuiz.value = data
      return data
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch quiz'
      throw err
    } finally {
      loading.value = false
    }
  }

  const startAttempt = async (quizId: string) => {
    try {
      const data = await quizApi.startAttempt(quizId)
      currentAttemptId.value = data.attemptId
      return data.attemptId
    } catch (err: any) {
      error.value = err.message || 'Failed to start quiz attempt'
      throw err
    }
  }

  const submitQuiz = async (quizId: string, submission: QuizSubmission) => {
    loading.value = true
    error.value = null

    try {
      const result = await quizApi.submitQuiz(quizId, submission)
      quizResult.value = result
      currentAttemptId.value = null
      return result
    } catch (err: any) {
      error.value = err.message || 'Failed to submit quiz'
      throw err
    } finally {
      loading.value = false
    }
  }

  const getResults = async (attemptId: string) => {
    loading.value = true
    error.value = null

    try {
      const result = await quizApi.getResults(attemptId)
      quizResult.value = result
      return result
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch quiz results'
      throw err
    } finally {
      loading.value = false
    }
  }

  const clearQuiz = () => {
    currentQuiz.value = null
    currentAttemptId.value = null
    quizResult.value = null
  }

  return {
    currentQuiz,
    currentAttemptId,
    quizResult,
    loading,
    error,
    fetchQuiz,
    startAttempt,
    submitQuiz,
    getResults,
    clearQuiz
  }
})
