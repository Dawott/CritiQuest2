<template>
  <div class="question-display">
    <!-- Question Text -->
    <h2 class="text-xl font-semibold text-gray-800 mb-6">{{ question.text }}</h2>

    <!-- Philosophical Context -->
    <div v-if="question.philosophicalContext" class="bg-blue-50 border-l-4 border-blue-400 p-4 mb-6">
      <p class="text-blue-800 text-sm">
        <span class="font-medium">Kontekst filozoficzny:</span> {{ question.philosophicalContext }}
      </p>
    </div>

    <!-- Single Choice Questions -->
    <div v-if="question.type === 'single'" class="space-y-3">
      <div v-for="(option, index) in question.options"
           :key="index"
           class="option-card"
           :class="{ 'selected': isSelected(option) }"
           @click="selectSingleOption(option)">
        <div class="flex items-center">
          <div class="radio-button" :class="{ 'checked': isSelected(option) }">
            <div v-if="isSelected(option)" class="radio-dot"></div>
          </div>
          <span class="ml-3 text-gray-800">{{ option }}</span>
        </div>
      </div>
    </div>

    <!-- Multiple Choice Questions -->
    <div v-else-if="question.type === 'multiple'" class="space-y-3">
      <div v-for="(option, index) in question.options"
           :key="index"
           class="option-card"
           :class="{ 'selected': isSelected(option) }"
           @click="toggleMultipleOption(option)">
        <div class="flex items-center">
          <div class="checkbox" :class="{ 'checked': isSelected(option) }">
            <svg v-if="isSelected(option)" class="w-4 h-4 text-white" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
            </svg>
          </div>
          <span class="ml-3 text-gray-800">{{ option }}</span>
        </div>
      </div>
    </div>

    <!-- Scenario Questions -->
    <div v-else-if="question.type === 'scenario'" class="space-y-4">
      <div class="bg-gray-50 rounded-lg p-6 mb-4">
        <h3 class="font-medium text-gray-800 mb-2">Scenariusz:</h3>
        <p class="text-gray-700">{{ getScenarioContext() }}</p>
      </div>

      <div class="space-y-3">
        <div v-for="(option, index) in question.options"
             :key="index"
             class="option-card scenario-option"
             :class="{ 'selected': isSelected(option) }"
             @click="selectSingleOption(option)">
          <div class="flex items-start">
            <div class="radio-button mt-1" :class="{ 'checked': isSelected(option) }">
              <div v-if="isSelected(option)" class="radio-dot"></div>
            </div>
            <div class="ml-3">
              <p class="text-gray-800 font-medium mb-1">Opcja {{ index + 1 }}</p>
              <p class="text-gray-700">{{ option }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Debate Questions -->
    <div v-else-if="question.type === 'debate'" class="space-y-4">
      <DebateInterface :question="question"
                       :userAnswer="userAnswer"
                       @answer-changed="handleDebateAnswer" />
    </div>

    <!-- Points Display -->
    <div class="mt-6 text-sm text-gray-600 text-right">
      <span class="bg-gray-100 px-3 py-1 rounded-full">
        {{ question.points }} {{ question.points === 1 ? 'punkt' : 'punkty' }}
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { Question } from '@/services/quizApi'
import DebateInterface from './DebateInterface.vue'

interface Props {
  question: Question
  userAnswer: string[]
}

interface Emits {
  (e: 'answer-changed', questionId: string, answers: string[]): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const isSelected = (option: string) => {
  return props.userAnswer.includes(option)
}

const selectSingleOption = (option: string) => {
  emit('answer-changed', props.question.id, [option])
}

const toggleMultipleOption = (option: string) => {
  const currentAnswers = [...props.userAnswer]
  const index = currentAnswers.indexOf(option)

  if (index > -1) {
    currentAnswers.splice(index, 1)
  } else {
    currentAnswers.push(option)
  }

  emit('answer-changed', props.question.id, currentAnswers)
}

const getScenarioContext = () => {
  // Extract scenario context from question text or philosophical context
  return props.question.philosophicalContext || "Rozważ poniższą sytuację i wybierz najbardziej odpowiednią reakcję zgodną z zasadami filozoficznymi."
}

const handleDebateAnswer = (answers: string[]) => {
  emit('answer-changed', props.question.id, answers)
}
</script>

<style scoped>
  .option-card {
    @apply border border-gray-200 rounded-lg p-4 cursor-pointer transition-all duration-200 hover:border-blue-300 hover:bg-blue-50;
  }

    .option-card.selected {
      @apply border-blue-500 bg-blue-50;
    }

  .scenario-option {
    @apply p-6;
  }

  .radio-button {
    @apply w-5 h-5 border-2 border-gray-300 rounded-full flex items-center justify-center;
  }

    .radio-button.checked {
      @apply border-blue-500;
    }

  .radio-dot {
    @apply w-2.5 h-2.5 bg-blue-500 rounded-full;
  }

  .checkbox {
    @apply w-5 h-5 border-2 border-gray-300 rounded flex items-center justify-center;
  }

    .checkbox.checked {
      @apply border-blue-500 bg-blue-500;
    }
</style>
