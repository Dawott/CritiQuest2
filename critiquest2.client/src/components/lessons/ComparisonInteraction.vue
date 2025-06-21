<template>
  <div class="comparison-interaction">
    <!-- Instructions -->
    <div class="instructions-card mb-6">
      <div class="bg-blue-50 border border-blue-200 rounded-xl p-6">
        <h4 class="text-blue-900 font-semibold mb-3 flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M3 4a1 1 0 011-1h4a1 1 0 010 2H6.414l2.293 2.293a1 1 0 11-1.414 1.414L5 6.414V8a1 1 0 01-2 0V4zm9 1a1 1 0 010-2h4a1 1 0 011 1v4a1 1 0 01-2 0V6.414l-2.293 2.293a1 1 0 11-1.414-1.414L13.586 5H12zm-9 7a1 1 0 012 0v1.586l2.293-2.293a1 1 0 111.414 1.414L6.414 15H8a1 1 0 010 2H4a1 1 0 01-1-1v-4zm13-1a1 1 0 011 1v4a1 1 0 01-1 1h-4a1 1 0 010-2h1.586l-2.293-2.293a1 1 0 111.414-1.414L15 13.586V12a1 1 0 011-1z" clip-rule="evenodd"></path>
          </svg>
          Porównanie: {{ interactionData.topic }}
        </h4>
        <p class="text-blue-800">{{ interactionData.instructions }}</p>
      </div>
    </div>

    <!-- Comparison Table -->
    <div class="comparison-table bg-white border border-gray-200 rounded-xl overflow-hidden mb-6">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gradient-to-r from-purple-500 to-blue-500 text-white">
            <tr>
              <th class="px-6 py-4 text-left font-semibold">Kryteria</th>
              <th v-for="item in interactionData.items"
                  :key="item.name"
                  class="px-6 py-4 text-left font-semibold">
                {{ item.name }}
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200">
            <!-- Description Row -->
            <tr class="bg-gray-50">
              <td class="px-6 py-4 font-medium text-gray-800">Opis</td>
              <td v-for="item in interactionData.items"
                  :key="`desc-${item.name}`"
                  class="px-6 py-4 text-gray-600 text-sm">
                {{ item.description }}
              </td>
            </tr>

            <!-- Criteria Rows -->
            <tr v-for="criterion in interactionData.criteria"
                :key="criterion"
                class="hover:bg-gray-50">
              <td class="px-6 py-4 font-medium text-gray-800">{{ criterion }}</td>
              <td v-for="item in interactionData.items"
                  :key="`${criterion}-${item.name}`"
                  class="px-6 py-4">
                <textarea v-model="comparisons[`${item.name}-${criterion}`]"
                          :placeholder="`Oceń ${item.name} pod kątem ${criterion.toLowerCase()}...`"
                          class="w-full p-2 border border-gray-300 rounded text-sm resize-none"
                          rows="2"
                          @input="updateResponse"></textarea>
              </td>
            </tr>

            <!-- User Custom Criteria -->
            <tr v-for="customCriterion in customCriteria"
                :key="`custom-${customCriterion}`"
                class="hover:bg-blue-50 border-l-4 border-blue-400">
              <td class="px-6 py-4 font-medium text-blue-800 flex items-center">
                {{ customCriterion }}
                <button @click="removeCustomCriterion(customCriterion)"
                        class="ml-2 text-red-500 hover:text-red-700">
                  ×
                </button>
              </td>
              <td v-for="item in interactionData.items"
                  :key="`custom-${customCriterion}-${item.name}`"
                  class="px-6 py-4">
                <textarea v-model="comparisons[`${item.name}-${customCriterion}`]"
                          :placeholder="`Oceń ${item.name} pod kątem ${customCriterion.toLowerCase()}...`"
                          class="w-full p-2 border border-blue-300 rounded text-sm resize-none"
                          rows="2"
                          @input="updateResponse"></textarea>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Add Custom Criterion -->
    <div class="add-criterion-section bg-gray-50 rounded-xl p-6 mb-6">
      <h4 class="font-semibold text-gray-800 mb-4">Dodaj własne kryterium porównania</h4>
      <div class="flex space-x-3">
        <input v-model="newCriterion"
               @keyup.enter="addCustomCriterion"
               placeholder="np. Wpływ na społeczeństwo, Oryginalność myśli..."
               class="flex-1 px-3 py-2 border border-gray-300 rounded-lg">
        <button @click="addCustomCriterion"
                :disabled="!newCriterion.trim()"
                class="btn-primary"
                :class="{ 'opacity-50 cursor-not-allowed': !newCriterion.trim() }">
          Dodaj
        </button>
      </div>
    </div>

    <!-- Comparison Summary -->
    <div class="summary-section bg-gradient-to-r from-purple-50 to-blue-50 rounded-xl p-6 mb-6">
      <h4 class="font-semibold text-gray-800 mb-4">Podsumowanie porównania</h4>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-4 text-center text-sm mb-6">
        <div>
          <div class="text-2xl font-bold text-purple-600">{{ totalCriteria }}</div>
          <div class="text-gray-600">Kryteriów</div>
        </div>
        <div>
          <div class="text-2xl font-bold text-blue-600">{{ filledComparisons }}</div>
          <div class="text-gray-600">Wypełnione</div>
        </div>
        <div>
          <div class="text-2xl font-bold text-green-600">{{ completionPercentage }}%</div>
          <div class="text-gray-600">Ukończone</div>
        </div>
        <div>
          <div class="text-2xl font-bold text-orange-600">{{ averageLength }}</div>
          <div class="text-gray-600">Śr. długość</div>
        </div>
      </div>

      <!-- Final Evaluation -->
      <div class="final-evaluation bg-white rounded-lg p-4">
        <h5 class="font-medium text-gray-800 mb-3">Końcowa ocena i wnioski</h5>
        <textarea v-model="finalEvaluation"
                  placeholder="Na podstawie porównania, jakie są główne różnice i podobieństwa? Które podejście uważasz za najbardziej przekonujące i dlaczego?"
                  class="w-full p-3 border border-gray-300 rounded-lg"
                  rows="4"
                  @input="updateResponse"></textarea>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'

interface ComparisonItem {
  name: string
  description: string
  properties: Record<string, string>
}

interface ComparisonData {
  topic: string
  items: ComparisonItem[]
  criteria: string[]
  instructions: string
}

interface Props {
  section: {
    id: string
    interactionData: ComparisonData
  }
  initialResponse?: any
}

interface Emits {
  (e: 'response-updated', response: any): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// State
const comparisons = ref<Record<string, string>>({})
const customCriteria = ref<string[]>([])
const newCriterion = ref('')
const finalEvaluation = ref('')

// Computed
const interactionData = computed(() => props.section.interactionData)

const totalCriteria = computed(() =>
  interactionData.value.criteria.length + customCriteria.value.length
)

const totalComparisons = computed(() =>
  interactionData.value.items.length * totalCriteria.value
)

const filledComparisons = computed(() =>
  Object.values(comparisons.value).filter(v => v && v.trim().length > 0).length
)

const completionPercentage = computed(() =>
  totalComparisons.value > 0 ? Math.round((filledComparisons.value / totalComparisons.value) * 100) : 0
)

const averageLength = computed(() => {
  const nonEmptyComparisons = Object.values(comparisons.value).filter(v => v && v.trim().length > 0)
  if (nonEmptyComparisons.length === 0) return 0
  const totalLength = nonEmptyComparisons.reduce((sum, comp) => sum + comp.length, 0)
  return Math.round(totalLength / nonEmptyComparisons.length)
})

// Methods
const addCustomCriterion = () => {
  if (!newCriterion.value.trim()) return

  const criterion = newCriterion.value.trim()
  if (!customCriteria.value.includes(criterion)) {
    customCriteria.value.push(criterion)
    newCriterion.value = ''
    updateResponse()
  }
}

const removeCustomCriterion = (criterion: string) => {
  customCriteria.value = customCriteria.value.filter(c => c !== criterion)

  // Remove related comparisons
  Object.keys(comparisons.value).forEach(key => {
    if (key.endsWith(`-${criterion}`)) {
      delete comparisons.value[key]
    }
  })

  updateResponse()
}

const updateResponse = () => {
  emit('response-updated', {
    comparisons: comparisons.value,
    customCriteria: customCriteria.value,
    finalEvaluation: finalEvaluation.value,
    completionStats: {
      totalCriteria: totalCriteria.value,
      filledComparisons: filledComparisons.value,
      completionPercentage: completionPercentage.value,
      averageLength: averageLength.value
    }
  })
}

// Initialize
const initializeFromSaved = () => {
  if (props.initialResponse) {
    comparisons.value = props.initialResponse.comparisons || {}
    customCriteria.value = props.initialResponse.customCriteria || []
    finalEvaluation.value = props.initialResponse.finalEvaluation || ''
  }
}

// Watch for changes
watch([comparisons, customCriteria, finalEvaluation], updateResponse, { deep: true })

onMounted(() => {
  initializeFromSaved()
})
</script>

<style scoped>
  .btn-primary {
    @apply bg-purple-600 hover:bg-purple-700 text-white font-medium py-2 px-4 rounded-lg transition-colors;
  }

  .timeline-event {
    transition: all 0.2s ease;
  }

    .timeline-event:hover {
      transform: translateX(4px);
    }

  .comparison-table th {
    position: sticky;
    top: 0;
    z-index: 10;
  }

  textarea:focus {
    outline: none;
    border-color: #8b5cf6;
    box-shadow: 0 0 0 2px rgba(139, 92, 246, 0.2);
  }
</style>
