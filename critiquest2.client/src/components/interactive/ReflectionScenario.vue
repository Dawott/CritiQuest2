<template>
  <div class="reflection-scenario">
    <!-- Scenario Description -->
    <div v-if="configuration.scenario" class="scenario-card mb-8">
      <div class="bg-gradient-to-r from-blue-50 to-indigo-50 border border-blue-200 rounded-xl p-6">
        <h3 class="text-lg font-semibold text-blue-900 mb-4 flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-8-3a1 1 0 00-.867.5 1 1 0 11-1.731-1A3 3 0 0113 8a3.001 3.001 0 01-2 2.83V11a1 1 0 11-2 0v-1a1 1 0 011-1 1 1 0 100-2zm0 8a1 1 0 100-2 1 1 0 000 2z" clip-rule="evenodd"></path>
          </svg>
          Scenariusz do rozważenia
        </h3>
        <p class="text-blue-800 leading-relaxed">{{ configuration.scenario }}</p>
      </div>
    </div>

    <!-- Reflection Prompts -->
    <div class="prompts-section space-y-6">
      <div v-for="(prompt, index) in configuration.prompts || []"
           :key="index"
           class="prompt-card bg-white border border-gray-200 rounded-xl p-6 shadow-sm hover:shadow-md transition-shadow">
        <!-- Prompt Question -->
        <div class="mb-4">
          <label class="flex items-start text-gray-800 font-medium">
            <span class="inline-flex items-center justify-center w-8 h-8 bg-purple-100 text-purple-800 rounded-full text-sm font-bold mr-3 mt-1 flex-shrink-0">
              {{ index + 1 }}
            </span>
            <span class="leading-relaxed">{{ prompt }}</span>
          </label>
        </div>

        <!-- Response Textarea -->
        <div class="ml-11">
          <textarea v-model="responses[index]"
                    :placeholder="`Podziel się swoimi przemyśleniami na temat pytania ${index + 1}...`"
                    class="w-full p-4 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-colors resize-none"
                    rows="4"
                    @input="updateResponse"></textarea>

          <!-- Character count -->
          <div class="flex justify-between items-center mt-2 text-sm">
            <span class="text-gray-500">
              {{ (responses[index] || '').length }} znaków
            </span>
            <div v-if="responses[index] && responses[index].length > 50" class="text-green-600 flex items-center">
              <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              Dobra odpowiedź!
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Guidance Section -->
    <div v-if="configuration.guidance" class="guidance-section mt-8">
      <div class="bg-amber-50 border border-amber-200 rounded-xl p-6">
        <h4 class="text-amber-900 font-semibold mb-3 flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path>
          </svg>
          Wskazówka
        </h4>
        <p class="text-amber-800">{{ configuration.guidance }}</p>
      </div>
    </div>

    <!-- Progress Summary -->
    <div class="progress-summary mt-8">
      <div class="bg-gray-50 rounded-xl p-6">
        <div class="flex items-center justify-between mb-4">
          <h4 class="text-gray-800 font-semibold">Twój postęp refleksji</h4>
          <span class="text-sm font-medium px-3 py-1 rounded-full"
                :class="progressClass">
            {{ completedPrompts }} / {{ totalPrompts }} odpowiedzi
          </span>
        </div>

        <!-- Progress Bar -->
        <div class="w-full bg-gray-200 rounded-full h-2 mb-4">
          <div class="h-2 rounded-full transition-all duration-500"
               :class="progressBarClass"
               :style="{ width: `${progressPercentage}%` }"></div>
        </div>

        <!-- Quality Indicators -->
        <div class="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
          <div class="text-center">
            <div class="text-2xl mb-1 font-bold text-purple-600">{{ thoughtfulnessScore }}/5</div>
            <div class="text-gray-600">Głębia</div>
          </div>
          <div class="text-center">
            <div class="text-2xl mb-1 font-bold text-blue-600">{{ totalWords }}</div>
            <div class="text-gray-600">Słów</div>
          </div>
          <div class="text-center">
            <div class="text-2xl mb-1 font-bold text-green-600">{{ averageLength }}</div>
            <div class="text-gray-600">Śr. długość</div>
          </div>
          <div class="text-center">
            <div class="text-2xl mb-1 font-bold text-orange-600">{{ progressPercentage }}%</div>
            <div class="text-gray-600">Ukończone</div>
          </div>
        </div>
      </div>
    </div>

    <!-- Philosophical Connections -->
    <div v-if="showPhilosophicalConnections" class="philosophical-connections mt-8">
      <div class="bg-purple-50 border border-purple-200 rounded-xl p-6">
        <h4 class="text-purple-900 font-semibold mb-4 flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M12.316 3.051a1 1 0 01.633 1.265l-4 12a1 1 0 11-1.898-.632l4-12a1 1 0 011.265-.633zM5.707 6.293a1 1 0 010 1.414L3.414 10l2.293 2.293a1 1 0 11-1.414 1.414l-3-3a1 1 0 010-1.414l3-3a1 1 0 011.414 0zm8.586 0a1 1 0 011.414 0l3 3a1 1 0 010 1.414l-3 3a1 1 0 11-1.414-1.414L16.586 10l-2.293-2.293a1 1 0 010-1.414z" clip-rule="evenodd"></path>
          </svg>
          Wykryte koncepcje filozoficzne
        </h4>
        <div class="space-y-3">
          <p class="text-purple-800">Twoje refleksje łączą się z następującymi koncepcjami:</p>
          <div class="flex flex-wrap gap-2">
            <span v-for="concept in detectedConcepts" :key="concept"
                  class="bg-purple-100 text-purple-800 px-3 py-1 rounded-full text-sm font-medium">
              {{ concept }}
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'

interface ReflectionConfig {
  scenario?: string
  prompts: string[]
  guidance?: string
  philosophicalConcepts?: string[]
}

interface Props {
  configuration: ReflectionConfig
  initialResponse?: any
}

interface Emits {
  (e: 'response-updated', response: any): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// State
const responses = ref<string[]>([])

// Computed
const totalPrompts = computed(() => props.configuration.prompts?.length || 0)

const completedPrompts = computed(() => {
  return responses.value.filter(r => r && r.trim().length > 0).length
})

const progressPercentage = computed(() => {
  return totalPrompts.value > 0 ? Math.round((completedPrompts.value / totalPrompts.value) * 100) : 0
})

const progressClass = computed(() => {
  const percentage = progressPercentage.value
  if (percentage === 100) return 'bg-green-100 text-green-800'
  if (percentage >= 50) return 'bg-yellow-100 text-yellow-800'
  return 'bg-gray-100 text-gray-600'
})

const progressBarClass = computed(() => {
  const percentage = progressPercentage.value
  if (percentage === 100) return 'bg-green-500'
  if (percentage >= 50) return 'bg-yellow-500'
  return 'bg-gray-400'
})

const thoughtfulnessScore = computed(() => {
  const avgLength = responses.value.reduce((sum, r) => sum + (r || '').length, 0) / Math.max(responses.value.length, 1)
  if (avgLength > 200) return 5
  if (avgLength > 150) return 4
  if (avgLength > 100) return 3
  if (avgLength > 50) return 2
  return avgLength > 0 ? 1 : 0
})

const totalWords = computed(() => {
  return responses.value.reduce((sum, r) => {
    return sum + (r || '').split(/\s+/).filter(word => word.length > 0).length
  }, 0)
})

const averageLength = computed(() => {
  const nonEmptyResponses = responses.value.filter(r => r && r.trim().length > 0)
  if (nonEmptyResponses.length === 0) return 0
  const totalLength = nonEmptyResponses.reduce((sum, r) => sum + r.length, 0)
  return Math.round(totalLength / nonEmptyResponses.length)
})

const showPhilosophicalConnections = computed(() => {
  return completedPrompts.value >= Math.ceil(totalPrompts.value * 0.5) && totalWords.value > 50
})

const detectedConcepts = computed(() => {
  const allText = responses.value.join(' ').toLowerCase()
  const concepts = []

  // Basic keyword detection for philosophical concepts
  const conceptMap: Record<string, string[]> = {
    'Wolność wyboru': ['wybór', 'decyzj', 'wolność', 'wybiera'],
    'Etyka wartości': ['wartości', 'wartość', 'znaczeni', 'ważne'],
    'Autentyczność': ['autentyczn', 'prawdziw', 'szczer', 'uczciw'],
    'Odpowiedzialność': ['odpowiedzialn', 'konsekwencj', 'skutk', 'winny'],
    'Relacje społeczne': ['społeczn', 'relacj', 'ludzi', 'innych', 'wspólnot'],
    'Sens życia': ['sens', 'znaczenie', 'cel', 'dlaczego']
  }

  for (const [concept, keywords] of Object.entries(conceptMap)) {
    if (keywords.some(keyword => allText.includes(keyword))) {
      concepts.push(concept)
    }
  }

  return concepts
})

// Methods
const updateResponse = () => {
  const responseData = {
    responses: responses.value,
    thoughtfulnessScore: thoughtfulnessScore.value,
    detectedConcepts: detectedConcepts.value,
    stats: {
      completedPrompts: completedPrompts.value,
      totalWords: totalWords.value,
      averageLength: averageLength.value,
      progressPercentage: progressPercentage.value
    }
  }

  emit('response-updated', responseData)
}

const initializeResponses = () => {
  const promptCount = props.configuration.prompts?.length || 0

  if (props.initialResponse?.responses) {
    responses.value = [...props.initialResponse.responses]
    // Ensure we have enough slots
    while (responses.value.length < promptCount) {
      responses.value.push('')
    }
  } else {
    responses.value = new Array(promptCount).fill('')
  }
}

// Watchers
watch(responses, updateResponse, { deep: true })

watch(() => props.configuration, () => {
  initializeResponses()
}, { immediate: false })

// Initialize
onMounted(() => {
  initializeResponses()
})
</script>

<style scoped>
  .prompt-card {
    transition: all 0.2s ease;
  }

    .prompt-card:hover {
      transform: translateY(-1px);
    }

  textarea:focus {
    outline: none;
  }

  .scenario-card {
    animation: fadeInUp 0.5s ease-out;
  }

  @keyframes fadeInUp {
    from {
      opacity: 0;
      transform: translateY(20px);
    }

    to {
      opacity: 1;
      transform: translateY(0);
    }
  }
</style>
