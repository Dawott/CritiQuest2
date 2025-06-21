<template>
  <div class="interactive-section">
    <!-- Section Title -->
    <div class="mb-6">
      <h3 class="text-xl font-semibold text-gray-800 mb-2">{{ section.title }}</h3>
      <div class="flex items-center space-x-2 mb-4">
        <span class="bg-purple-100 text-purple-800 px-3 py-1 rounded-full text-sm font-medium">
          Interaktywne
        </span>
        <span class="bg-blue-100 text-blue-800 px-3 py-1 rounded-full text-sm">
          {{ formatInteractionType(section.interactionType) }}
        </span>
      </div>
    </div>

    <!-- Base Content -->
    <div v-if="section.content" class="prose max-w-none mb-6">
      <div v-html="formatContent(section.content)"></div>
    </div>

    <!-- Interactive Component Based on Type -->
    <div class="interactive-component bg-gray-50 rounded-xl p-6">
      <!-- Reflection Component -->
      <ReflectionInteraction v-if="section.interactionType === 'reflection'"
                             :section="section"
                             :initialResponses="currentResponses"
                             @responses-updated="handleResponsesUpdate" />

      <!-- Concept Map Component -->
      <ConceptMapInteraction v-else-if="section.interactionType === 'concept-map'"
                             :section="section"
                             :initialResponse="currentResponses"
                             @response-updated="handleResponsesUpdate" />

      <!-- Timeline Component -->
      <TimelineInteraction v-else-if="section.interactionType === 'timeline'"
                           :section="section"
                           :initialResponse="currentResponses"
                           @response-updated="handleResponsesUpdate" />

      <!-- Comparison Component -->
      <ComparisonInteraction v-else-if="section.interactionType === 'comparison'"
                             :section="section"
                             :initialResponse="currentResponses"
                             @response-updated="handleResponsesUpdate" />

      <!-- Fallback for unknown types -->
      <div v-else class="text-center py-8 text-gray-500">
        <p>Ten typ interakcji nie jest jeszcze obsługiwany: {{ section.interactionType }}</p>
      </div>
    </div>

    <!-- Progress Indicator -->
    <div class="mt-6 flex items-center justify-between">
      <div class="flex items-center space-x-2">
        <div v-if="completionProgress > 0" class="flex items-center text-sm text-green-600">
          <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
          </svg>
          {{ completionProgress }}% ukończone
        </div>
      </div>

      <button v-if="completionProgress >= 50"
              @click="saveProgress"
              class="btn-primary text-sm"
              :disabled="saving">
        <span v-if="saving">Zapisywanie...</span>
        <span v-else>💾 Zapisz postępy</span>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useInteractionsStore } from '@/stores/interactions'
import ReflectionInteraction from './ReflectionInteraction.vue'
import ConceptMapInteraction from './ConceptMapInteraction.vue'
import TimelineInteraction from './TimelineInteraction.vue'
import ComparisonInteraction from './ComparisonInteraction.vue'

interface LessonSection {
  id: string
  title: string
  content: string
  type: string
  interactionType?: string
  interactionData?: any
}

interface Props {
  section: LessonSection
  lessonId: string
}

const props = defineProps<Props>()
const interactionsStore = useInteractionsStore()

// State
const currentResponses = ref<any>({})
const saving = ref(false)

// Computed
const completionProgress = computed(() => {
  if (!props.section.interactionData) return 0

  switch (props.section.interactionType) {
    case 'reflection':
      const prompts = props.section.interactionData.prompts || []
      const responses = currentResponses.value.responses || []
      const completedPrompts = responses.filter((r: string) => r && r.trim().length > 0).length
      return prompts.length > 0 ? Math.round((completedPrompts / prompts.length) * 100) : 0

    case 'concept-map':
      return currentResponses.value.concepts && currentResponses.value.concepts.length > 0 ? 100 : 0

    case 'timeline':
      const events = currentResponses.value.events || []
      return events.length > 0 ? 100 : 0

    case 'comparison':
      const comparisons = currentResponses.value.comparisons || {}
      return Object.keys(comparisons).length > 0 ? 100 : 0

    default:
      return 0
  }
})

// Methods
const formatContent = (content: string) => {
  return content.replace(/\n/g, '<br>')
}

const formatInteractionType = (type?: string) => {
  const types: Record<string, string> = {
    'reflection': 'Refleksja',
    'concept-map': 'Mapa Pojęć',
    'timeline': 'Linia Czasu',
    'comparison': 'Porównanie'
  }
  return types[type || ''] || type || 'Nieznany'
}

const handleResponsesUpdate = (responses: any) => {
  currentResponses.value = responses
}

const saveProgress = async () => {
  if (!currentResponses.value || saving.value) return

  saving.value = true
  try {
    await interactionsStore.saveInteractionResponse({
      lessonId: props.lessonId,
      sectionId: props.section.id,
      interactionType: props.section.interactionType || 'unknown',
      responseData: currentResponses.value
    })
  } catch (error) {
    console.error('Failed to save interaction progress:', error)
  } finally {
    saving.value = false
  }
}

// Load existing responses on mount
const loadExistingResponses = async () => {
  try {
    const responses = await interactionsStore.getInteractionResponse(
      props.lessonId,
      props.section.id
    )
    if (responses) {
      currentResponses.value = responses.responseData
    }
  } catch (error) {
    console.error('Failed to load existing responses:', error)
  }
}

// Watch for auto-save after significant changes
watch(currentResponses, () => {
  if (completionProgress.value >= 80) {
    // Auto-save when mostly complete
    setTimeout(() => saveProgress(), 2000)
  }
}, { deep: true })

// Initialize
loadExistingResponses()
</script>

<style scoped>
  .btn-primary {
    @apply bg-purple-600 hover:bg-purple-700 text-white font-medium py-2 px-4 rounded-lg transition-colors;
  }

  .prose {
    @apply text-gray-700 leading-relaxed;
  }

    .prose h1, .prose h2, .prose h3, .prose h4 {
      @apply text-gray-800 font-semibold;
    }

    .prose p {
      @apply mb-4;
    }

    .prose strong {
      @apply font-semibold text-gray-800;
    }
</style>
