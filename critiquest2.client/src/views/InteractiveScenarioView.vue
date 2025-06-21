<template>
  <div class="interactive-scenario-view min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 py-8 px-4">
    <div class="max-w-4xl mx-auto">
      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center items-center min-h-96">
        <div class="animate-spin rounded-full h-16 w-16 border-b-2 border-purple-600"></div>
      </div>

      <!-- Scenario Content -->
      <div v-else-if="section" class="space-y-6">
        <!-- Header -->
        <div class="bg-white rounded-xl shadow-md p-6">
          <div class="flex items-center justify-between mb-4">
            <button @click="goBack" class="btn-secondary">
              ← Powrót do lekcji
            </button>
            <div class="flex items-center space-x-3">
              <div class="scenario-type-badge" :class="getTypeClass(section.type)">
                {{ getTypeIcon(section.type) }} {{ formatType(section.type) }}
              </div>
              <div v-if="section.isRequired" class="bg-red-100 text-red-600 px-3 py-1 rounded-full text-sm font-medium">
                Wymagane
              </div>
            </div>
          </div>

          <h1 class="text-2xl font-bold text-gray-800 mb-2">{{ section.title }}</h1>
          <p class="text-gray-600 mb-4">{{ section.description }}</p>

          <!-- Progress Bar -->
          <div v-if="userResponse" class="w-full bg-gray-200 rounded-full h-3 mb-2">
            <div class="bg-purple-600 h-3 rounded-full transition-all duration-300"
                 :style="{ width: `${userResponse.completionPercentage}%` }"></div>
          </div>
          <div class="flex justify-between items-center text-sm text-gray-600">
            <span>Szacowany czas: {{ section.estimatedTimeMinutes }} min</span>
            <span v-if="userResponse">Ukończone: {{ userResponse.completionPercentage }}%</span>
          </div>
        </div>

        <!-- Scenario Runner -->
        <div class="bg-white rounded-xl shadow-md p-8">
          <!-- Reflection Scenario -->
          <ReflectionScenario v-if="section.type === 'Reflection'"
                              :configuration="section.configuration"
                              :initial-response="userResponse?.responseData"
                              @response-updated="handleResponseUpdate" />

          <!-- Concept Map Scenario -->
          <ConceptMapScenario v-else-if="section.type === 'ConceptMap'"
                              :configuration="section.configuration"
                              :initial-response="userResponse?.responseData"
                              @response-updated="handleResponseUpdate" />

          <!-- Timeline Scenario -->
          <TimelineScenario v-else-if="section.type === 'Timeline'"
                            :configuration="section.configuration"
                            :initial-response="userResponse?.responseData"
                            @response-updated="handleResponseUpdate" />

          <!-- Comparison Scenario -->
          <ComparisonScenario v-else-if="section.type === 'Comparison'"
                              :configuration="section.configuration"
                              :initial-response="userResponse?.responseData"
                              @response-updated="handleResponseUpdate" />

          <!-- Unknown Type -->
          <div v-else class="text-center py-8 text-gray-500">
            <p>Nieznany typ scenariusza: {{ section.type }}</p>
          </div>
        </div>

        <!-- Actions -->
        <div class="bg-white rounded-xl shadow-md p-6">
          <div class="flex justify-between items-center">
            <div class="text-sm text-gray-600">
              <p v-if="timeSpent > 0">Czas spędzony: {{ formatTime(timeSpent) }}</p>
              <p v-if="userResponse">Ostatni zapis: {{ formatLastSaved(userResponse.lastUpdatedAt) }}</p>
            </div>

            <div class="flex space-x-3">
              <button @click="saveProgress" :disabled="saving" class="btn-secondary">
                <span v-if="saving">Zapisywanie...</span>
                <span v-else>💾 Zapisz postęp</span>
              </button>

              <button @click="completeScenario"
                      :disabled="!canComplete || saving"
                      class="btn-primary"
                      :class="{ 'opacity-50 cursor-not-allowed': !canComplete || saving }">
                <span v-if="saving">Zapisywanie...</span>
                <span v-else-if="userResponse?.isCompleted">✅ Ukończone</span>
                <span v-else>🎯 Oznacz jako ukończone</span>
              </button>
            </div>
          </div>
        </div>

        <!-- Completion Modal -->
        <div v-if="showCompletionModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div class="bg-white rounded-xl p-8 max-w-md mx-4">
            <div class="text-center">
              <div class="text-6xl mb-4">🎉</div>
              <h3 class="text-xl font-bold mb-2">Scenariusz ukończony!</h3>
              <p class="text-gray-600 mb-6">
                Świetnie! Twoje refleksje zostały zapisane.
              </p>
              <div class="flex justify-center space-x-3">
                <button @click="showCompletionModal = false" class="btn-secondary">
                  Kontynuuj edycję
                </button>
                <button @click="goBack" class="btn-primary">
                  Powrót do lekcji
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="text-center py-8">
        <p class="text-red-600 mb-4">{{ error }}</p>
        <button @click="loadScenario" class="btn-primary">Spróbuj ponownie</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useInteractiveScenariosStore } from '@/stores/interactiveScenarios'
import ReflectionScenario from '@/components/interactive/ReflectionScenario.vue'
import ConceptMapScenario from '@/components/interactive/ConceptMapScenario.vue'
import TimelineScenario from '@/components/interactive/TimelineScenario.vue'
import ComparisonScenario from '@/components/interactive/ComparisonScenario.vue'

const route = useRoute()
const router = useRouter()
const store = useInteractiveScenariosStore()

// State
const currentResponse = ref<any>({})
const saving = ref(false)
const showCompletionModal = ref(false)
const startTime = ref(Date.now())
const autoSaveInterval = ref<number | null>(null)

// Computed
const section = computed(() => store.currentSection)
const userResponse = computed(() => section.value?.userResponse)
const loading = computed(() => store.loading)
const error = computed(() => store.error)

const timeSpent = computed(() => {
  const base = userResponse.value?.timeSpentSeconds || 0
  const current = Math.floor((Date.now() - startTime.value) / 1000)
  return base + current
})

const canComplete = computed(() => {
  if (!currentResponse.value) return false

  // Basic completion check - can be enhanced per scenario type
  switch (section.value?.type) {
    case 'Reflection':
      return currentResponse.value.responses?.some((r: string) => r && r.trim().length > 20)
    case 'ConceptMap':
      return currentResponse.value.concepts?.length > 0
    case 'Timeline':
      return currentResponse.value.events?.length > 0
    case 'Comparison':
      return Object.keys(currentResponse.value.comparisons || {}).length > 0
    default:
      return false
  }
})

// Methods
const formatType = (type: string) => {
  const types: Record<string, string> = {
    'Reflection': 'Refleksja',
    'ConceptMap': 'Mapa Pojęć',
    'Timeline': 'Linia Czasu',
    'Comparison': 'Porównanie'
  }
  return types[type] || type
}

const getTypeIcon = (type: string) => {
  const icons: Record<string, string> = {
    'Reflection': '💭',
    'ConceptMap': '🗺️',
    'Timeline': '📅',
    'Comparison': '⚖️'
  }
  return icons[type] || '📝'
}

const getTypeClass = (type: string) => {
  const classes: Record<string, string> = {
    'Reflection': 'bg-purple-100 text-purple-800',
    'ConceptMap': 'bg-blue-100 text-blue-800',
    'Timeline': 'bg-green-100 text-green-800',
    'Comparison': 'bg-orange-100 text-orange-800'
  }
  return `px-3 py-1 rounded-full text-sm font-medium ${classes[type] || 'bg-gray-100 text-gray-600'}`
}

const formatTime = (seconds: number) => {
  const minutes = Math.floor(seconds / 60)
  const remainingSeconds = seconds % 60
  return `${minutes}min ${remainingSeconds}s`
}

const formatLastSaved = (timestamp: string) => {
  const date = new Date(timestamp)
  return date.toLocaleString('pl-PL')
}

const handleResponseUpdate = (response: any) => {
  currentResponse.value = response
}

const saveProgress = async () => {
  if (!section.value || saving.value) return

  saving.value = true
  try {
    await store.saveResponse(
      section.value.id,
      currentResponse.value,
      timeSpent.value,
      false
    )
  } catch (error) {
    console.error('Failed to save progress:', error)
  } finally {
    saving.value = false
  }
}

const completeScenario = async () => {
  if (!section.value || !canComplete.value || saving.value) return

  saving.value = true
  try {
    await store.saveResponse(
      section.value.id,
      currentResponse.value,
      timeSpent.value,
      true
    )
    showCompletionModal.value = true
  } catch (error) {
    console.error('Failed to complete scenario:', error)
  } finally {
    saving.value = false
  }
}

const goBack = () => {
  if (section.value?.lessonId) {
    router.push(`/lessons/${section.value.lessonId}`)
  } else {
    router.push('/lessons')
  }
}

const loadScenario = async () => {
  const sectionId = route.params.sectionId as string
  if (!sectionId) {
    router.push('/lessons')
    return
  }

  try {
    await store.getSection(sectionId)

    // Initialize current response with existing data
    if (userResponse.value?.responseData) {
      currentResponse.value = userResponse.value.responseData
    }
  } catch (error) {
    console.error('Failed to load scenario:', error)
  }
}

const startAutoSave = () => {
  // Auto-save every 30 seconds
  autoSaveInterval.value = setInterval(() => {
    if (currentResponse.value && Object.keys(currentResponse.value).length > 0) {
      saveProgress()
    }
  }, 30000)
}

const stopAutoSave = () => {
  if (autoSaveInterval.value) {
    clearInterval(autoSaveInterval.value)
    autoSaveInterval.value = null
  }
}

// Lifecycle
onMounted(() => {
  loadScenario()
  startAutoSave()
})

onUnmounted(() => {
  stopAutoSave()
  // Save progress on exit if there are changes
  if (currentResponse.value && Object.keys(currentResponse.value).length > 0) {
    saveProgress()
  }
})
</script>

<style scoped>
  .btn-primary {
    @apply bg-purple-600 hover:bg-purple-700 text-white font-semibold py-2 px-6 rounded-lg transition-colors;
  }

  .btn-secondary {
    @apply bg-gray-200 hover:bg-gray-300 text-gray-800 font-semibold py-2 px-6 rounded-lg transition-colors;
  }

  .scenario-type-badge {
    transition: all 0.2s ease;
  }
</style>
