<template>
  <div class="quiz-container min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 py-8 px-4">
    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center min-h-96">
      <div class="animate-spin rounded-full h-16 w-16 border-b-2 border-blue-600"></div>
    </div>

    <!-- Quiz Interface -->
    <div v-else-if="currentQuiz && currentAttemptId" class="max-w-4xl mx-auto">
      <!-- Quiz Header -->
      <div class="bg-white rounded-xl shadow-md p-6 mb-6">
        <div class="flex justify-between items-center mb-4">
          <h1 class="text-2xl font-bold text-gray-800">{{ currentQuiz.title }}</h1>
          <div v-if="timeRemaining !== null" class="text-lg font-semibold"
               :class="timeRemaining < 60 ? 'text-red-600' : 'text-blue-600'">
            {{ formatTime(timeRemaining) }}
          </div>
        </div>

        <!-- Progress Bar -->
        <div class="w-full bg-gray-200 rounded-full h-3 mb-2">
          <div class="bg-blue-600 h-3 rounded-full transition-all duration-300"
               :style="{ width: `${progress}%` }"></div>
        </div>
        <p class="text-sm text-gray-600">
          Pytanie {{ currentQuestionIndex + 1 }} z {{ currentQuiz.questions.length }}
        </p>
      </div>

      <!-- Question Display -->
      <div class="bg-white rounded-xl shadow-md p-8 mb-6">
        <QuestionDisplay :question="currentQuestion"
                         :userAnswer="userAnswers[currentQuestion.id] || []"
                         @answer-changed="updateAnswer" />
      </div>

      <!-- Navigation -->
      <div class="flex justify-between items-center">
        <button @click="previousQuestion"
                :disabled="currentQuestionIndex === 0"
                class="btn-secondary"
                :class="{ 'opacity-50 cursor-not-allowed': currentQuestionIndex === 0 }">
          ← Poprzednie
        </button>

        <div class="flex space-x-3">
          <button v-if="!isLastQuestion"
                  @click="nextQuestion"
                  class="btn-primary">
            Następne →
          </button>

          <button v-else
                  @click="showSubmitModal = true"
                  class="btn-success">
            Zakończ Quiz
          </button>
        </div>
      </div>
    </div>

    <!-- Submit Confirmation Modal -->
    <div v-if="showSubmitModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl p-8 max-w-md mx-4">
        <h3 class="text-xl font-bold mb-4">Zakończyć quiz?</h3>
        <p class="text-gray-600 mb-6">
          Odpowiedziałeś na {{ answeredCount }} z {{ totalQuestions }} pytań.
          Czy na pewno chcesz zakończyć quiz?
        </p>
        <div class="flex justify-end space-x-3">
          <button @click="showSubmitModal = false" class="btn-secondary">
            Anuluj
          </button>
          <button @click="submitQuiz" class="btn-primary">
            Tak, zakończ
          </button>
        </div>
      </div>
    </div>

    <!-- Results Display -->
    <QuizResults v-if="quizResult"
                 :result="quizResult"
                 @retry="retryQuiz"
                 @continue="goToLessons" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useQuizzesStore } from '@/stores/quizzes'
import QuestionDisplay from '@/components/quiz/QuestionDisplay.vue'
import QuizResults from '@/components/quiz/QuizResults.vue'

const route = useRoute()
const router = useRouter()
const quizzesStore = useQuizzesStore()

// State
const currentQuestionIndex = ref(0)
const userAnswers = ref<Record<string, string[]>>({})
const showSubmitModal = ref(false)
const startTime = ref(Date.now())
const timeRemaining = ref<number | null>(null)

// Computed
const currentQuiz = computed(() => quizzesStore.currentQuiz)
const currentAttemptId = computed(() => quizzesStore.currentAttemptId)
const quizResult = computed(() => quizzesStore.quizResult)
const loading = computed(() => quizzesStore.loading)

const currentQuestion = computed(() => {
  if (!currentQuiz.value) return null
  return currentQuiz.value.questions[currentQuestionIndex.value]
})

const isLastQuestion = computed(() => {
  if (!currentQuiz.value) return false
  return currentQuestionIndex.value === currentQuiz.value.questions.length - 1
})

const progress = computed(() => {
  if (!currentQuiz.value) return 0
  return ((currentQuestionIndex.value + 1) / currentQuiz.value.questions.length) * 100
})

const totalQuestions = computed(() => currentQuiz.value?.questions.length || 0)

const answeredCount = computed(() => {
  return Object.keys(userAnswers.value).filter(questionId =>
    userAnswers.value[questionId] && userAnswers.value[questionId].length > 0
  ).length
})

// Timer management
let timerInterval: number | null = null

const startTimer = () => {
  if (!currentQuiz.value?.timeLimit) return

  timeRemaining.value = currentQuiz.value.timeLimit

  timerInterval = setInterval(() => {
    if (timeRemaining.value && timeRemaining.value > 0) {
      timeRemaining.value--
    } else {
      autoSubmitQuiz()
    }
  }, 1000)
}

const stopTimer = () => {
  if (timerInterval) {
    clearInterval(timerInterval)
    timerInterval = null
  }
}

const formatTime = (seconds: number) => {
  const minutes = Math.floor(seconds / 60)
  const remainingSeconds = seconds % 60
  return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`
}

// Actions
const updateAnswer = (questionId: string, answers: string[]) => {
  userAnswers.value[questionId] = answers
}

const nextQuestion = () => {
  if (!isLastQuestion.value) {
    currentQuestionIndex.value++
  }
}

const previousQuestion = () => {
  if (currentQuestionIndex.value > 0) {
    currentQuestionIndex.value--
  }
}

const submitQuiz = async () => {
  if (!currentAttemptId.value || !currentQuiz.value) return

  showSubmitModal.value = false
  stopTimer()

  const timeSpent = Math.floor((Date.now() - startTime.value) / 1000)

  const submission = {
    attemptId: currentAttemptId.value,
    timeSpent,
    answers: Object.entries(userAnswers.value).map(([questionId, selectedAnswers]) => ({
      questionId,
      selectedAnswers
    }))
  }

  try {
    await quizzesStore.submitQuiz(currentQuiz.value.id, submission)
  } catch (error) {
    console.error('Failed to submit quiz:', error)
    // Handle error appropriately
  }
}

const autoSubmitQuiz = () => {
  showSubmitModal.value = false
  submitQuiz()
}

const retryQuiz = async () => {
  if (!currentQuiz.value) return

  // Reset state
  currentQuestionIndex.value = 0
  userAnswers.value = {}
  startTime.value = Date.now()

  // Start new attempt
  try {
    await quizzesStore.startAttempt(currentQuiz.value.id)
    startTimer()
  } catch (error) {
    console.error('Failed to start new attempt:', error)
  }
}

const goToLessons = () => {
  router.push('/lessons')
}

// Lifecycle
onMounted(async () => {
  const quizId = route.params.id as string
  if (!quizId) {
    router.push('/lessons')
    return
  }

  try {
    await quizzesStore.fetchQuiz(quizId)
    await quizzesStore.startAttempt(quizId)
    startTimer()
  } catch (error) {
    console.error('Failed to load quiz:', error)
    router.push('/lessons')
  }
})

onUnmounted(() => {
  stopTimer()
  quizzesStore.clearQuiz()
})
</script>

<style scoped>
  .btn-primary {
    @apply bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-6 rounded-lg transition-colors;
  }

  .btn-secondary {
    @apply bg-gray-200 hover:bg-gray-300 text-gray-800 font-semibold py-2 px-6 rounded-lg transition-colors;
  }

  .btn-success {
    @apply bg-green-600 hover:bg-green-700 text-white font-semibold py-2 px-6 rounded-lg transition-colors;
  }
</style>
