<template>
  <div class="quiz-results max-w-4xl mx-auto">
    <!-- Results Header -->
    <div class="bg-white rounded-xl shadow-lg p-8 mb-6">
      <div class="text-center mb-6">
        <div class="mb-4">
          <div v-if="result.passed" class="text-6xl mb-2">🎉</div>
          <div v-else class="text-6xl mb-2">📚</div>
        </div>

        <h1 class="text-3xl font-bold mb-2" :class="result.passed ? 'text-green-600' : 'text-orange-600'">
          {{ result.passed ? 'Gratulacje!' : 'Dobra próba!' }}
        </h1>

        <p class="text-gray-600 text-lg">
          {{ result.passed ? 'Ukończyłeś quiz pomyślnie!' : 'Możesz spróbować ponownie.' }}
        </p>
      </div>

      <!-- Score Display -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
        <div class="text-center p-4 bg-blue-50 rounded-lg">
          <div class="text-3xl font-bold text-blue-600">{{ result.score }}%</div>
          <p class="text-blue-800 font-medium">Wynik końcowy</p>
        </div>

        <div class="text-center p-4 bg-green-50 rounded-lg">
          <div class="text-3xl font-bold text-green-600">{{ result.earnedPoints }}</div>
          <p class="text-green-800 font-medium">Zdobyte punkty</p>
        </div>

        <div class="text-center p-4 bg-purple-50 rounded-lg">
          <div class="text-3xl font-bold text-purple-600">{{ formatTime(result.timeSpent) }}</div>
          <p class="text-purple-800 font-medium">Czas rozwiązywania</p>
        </div>
      </div>

      <!-- Progress Bar -->
      <div class="w-full bg-gray-200 rounded-full h-4 mb-4">
        <div class="h-4 rounded-full transition-all duration-1000"
             :class="result.passed ? 'bg-green-500' : 'bg-orange-500'"
             :style="{ width: `${result.score}%` }"></div>
      </div>

      <p class="text-center text-gray-600">
        {{ result.earnedPoints }} z {{ result.totalPoints }} możliwych punktów
      </p>
    </div>

    <!-- Detailed Results -->
    <div class="bg-white rounded-xl shadow-lg p-8 mb-6">
      <h2 class="text-2xl font-bold text-gray-800 mb-6">Szczegółowe wyniki</h2>

      <div class="space-y-6">
        <div v-for="(questionResult, index) in result.results"
             :key="questionResult.questionId"
             class="question-result-card"
             :class="{ 'correct': questionResult.isCorrect, 'incorrect': !questionResult.isCorrect }">
          <!-- Question Header -->
          <div class="flex items-start justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-800 flex-1 pr-4">
              <span class="text-gray-500 text-base mr-2">{{ index + 1 }}.</span>
              {{ questionResult.question }}
            </h3>
            <div class="flex items-center space-x-2">
              <span v-if="questionResult.isCorrect" class="text-green-600 text-2xl">✓</span>
              <span v-else class="text-red-600 text-2xl">✗</span>
              <span class="text-sm font-medium px-2 py-1 rounded"
                    :class="questionResult.isCorrect ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'">
                {{ questionResult.points }}/{{ questionResult.maxPoints }}
              </span>
            </div>
          </div>

          <!-- Answers -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
            <!-- User Answers -->
            <div>
              <h4 class="font-medium text-gray-700 mb-2">Twoja odpowiedź:</h4>
              <div class="space-y-1">
                <div v-for="answer in questionResult.userAnswers"
                     :key="answer"
                     class="text-sm p-2 rounded"
                     :class="questionResult.isCorrect ? 'bg-green-50 text-green-800' : 'bg-red-50 text-red-800'">
                  {{ answer }}
                </div>
              </div>
            </div>

            <!-- Correct Answers -->
            <div>
              <h4 class="font-medium text-gray-700 mb-2">Prawidłowa odpowiedź:</h4>
              <div class="space-y-1">
                <div v-for="answer in questionResult.correctAnswers"
                     :key="answer"
                     class="text-sm p-2 rounded bg-green-50 text-green-800">
                  {{ answer }}
                </div>
              </div>
            </div>
          </div>

          <!-- Explanation -->
          <div class="bg-blue-50 border-l-4 border-blue-400 p-4 mb-4">
            <h4 class="font-medium text-blue-800 mb-2">Wyjaśnienie:</h4>
            <p class="text-blue-700 text-sm">{{ questionResult.explanation }}</p>
          </div>

          <!-- Philosophical Context -->
          <div v-if="questionResult.philosophicalContext" class="bg-purple-50 border-l-4 border-purple-400 p-4">
            <h4 class="font-medium text-purple-800 mb-2">Kontekst filozoficzny:</h4>
            <p class="text-purple-700 text-sm">{{ questionResult.philosophicalContext }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Performance Analysis -->
    <div class="bg-white rounded-xl shadow-lg p-8 mb-6">
      <h2 class="text-2xl font-bold text-gray-800 mb-6">Analiza wyników</h2>

      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div class="space-y-4">
          <h3 class="text-lg font-semibold text-gray-700">Statystyki</h3>
          <div class="space-y-3">
            <div class="flex justify-between">
              <span class="text-gray-600">Poprawne odpowiedzi:</span>
              <span class="font-medium">{{ correctAnswers }} / {{ totalQuestions }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">Wskaźnik sukcesu:</span>
              <span class="font-medium">{{ Math.round((correctAnswers / totalQuestions) * 100) }}%</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">Średni czas na pytanie:</span>
              <span class="font-medium">{{ Math.round(result.timeSpent / totalQuestions) }}s</span>
            </div>
          </div>
        </div>

        <div class="space-y-4">
          <h3 class="text-lg font-semibold text-gray-700">Rekomendacje</h3>
          <div class="space-y-2">
            <div v-if="result.score >= 90" class="text-green-700 text-sm">
              ⭐ Doskonała znajomość materiału! Możesz przejść do bardziej zaawansowanych tematów.
            </div>
            <div v-else-if="result.score >= 70" class="text-blue-700 text-sm">
              👍 Dobra znajomość materiału. Przejrzyj tematy, w których popełniłeś błędy.
            </div>
            <div v-else-if="result.score >= 50" class="text-orange-700 text-sm">
              📖 Warto powtórzyć materiał z lekcji przed przejściem dalej.
            </div>
            <div v-else class="text-red-700 text-sm">
              🔄 Zalecamy ponowne przejście lekcji i powtórzenie quizu.
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Action Buttons -->
    <div class="flex justify-center space-x-4 mb-8">
      <button @click="$emit('retry')" class="btn-secondary">
        🔄 Spróbuj ponownie
      </button>
      <button @click="$emit('continue')" class="btn-primary">
        ➡️ Kontynuuj naukę
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { computed } from 'vue'
  import type { QuizResult } from '@/services/quizApi'

  interface Props {
    result: QuizResult
  }

  interface Emits {
    (e: 'retry'): void
    (e: 'continue'): void
  }

  const props = defineProps<Props>()
  defineEmits<Emits>()

  const correctAnswers = computed(() => {
    return props.result.results.filter(r => r.isCorrect).length
  })

  const totalQuestions = computed(() => {
    return props.result.results.length
  })

  const formatTime = (seconds: number) => {
    const minutes = Math.floor(seconds / 60)
    const remainingSeconds = seconds % 60

    if (minutes > 0) {
      return `${minutes}min ${remainingSeconds}s`
    }
    return `${remainingSeconds}s`
  }
</script>

<style scoped>
  .question-result-card {
    @apply border-2 rounded-lg p-6 transition-all duration-200;
  }

    .question-result-card.correct {
      @apply border-green-200 bg-green-50;
    }

    .question-result-card.incorrect {
      @apply border-red-200 bg-red-50;
    }

  .btn-primary {
    @apply bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 px-8 rounded-lg transition-colors;
  }

  .btn-secondary {
    @apply bg-gray-200 hover:bg-gray-300 text-gray-800 font-semibold py-3 px-8 rounded-lg transition-colors;
  }
</style>
