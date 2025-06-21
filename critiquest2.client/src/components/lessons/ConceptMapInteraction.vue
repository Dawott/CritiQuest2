<template>
  <div class="concept-map-interaction">
    <!-- Instructions -->
    <div class="instructions-card mb-6">
      <div class="bg-blue-50 border border-blue-200 rounded-xl p-6">
        <h4 class="text-blue-900 font-semibold mb-3 flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd"></path>
          </svg>
          Mapa pojęć
        </h4>
        <p class="text-blue-800">{{ interactionData.instructions }}</p>
      </div>
    </div>

    <!-- Central Concept -->
    <div class="central-concept mb-8">
      <div class="text-center">
        <div class="inline-block bg-gradient-to-r from-purple-500 to-blue-500 text-white px-8 py-4 rounded-xl shadow-lg">
          <h3 class="text-xl font-bold">{{ interactionData.centralConcept }}</h3>
          <p class="text-purple-100 text-sm mt-1">Centralne pojęcie</p>
        </div>
      </div>
    </div>

    <!-- Interactive Canvas -->
    <div class="concept-canvas bg-white border-2 border-gray-200 rounded-xl p-8 mb-6" ref="canvasContainer">
      <!-- Grid background -->
      <div class="absolute inset-0 opacity-20"
           style="background-image: radial-gradient(circle, #e5e7eb 1px, transparent 1px); background-size: 20px 20px;"></div>

      <!-- Related Concepts -->
      <div class="relative min-h-96">
        <!-- Pre-defined concepts (if any) -->
        <div v-for="(concept, index) in predefinedConcepts"
             :key="`predefined-${index}`"
             class="concept-node predefined"
             :style="getPredefindConceptPosition(index)">
          <div class="bg-gray-100 border-2 border-gray-300 rounded-lg px-4 py-2 text-sm font-medium text-gray-700 shadow-sm">
            {{ concept }}
          </div>
        </div>

        <!-- User-added concepts -->
        <div v-for="(concept, index) in userConcepts"
             :key="`user-${index}`"
             class="concept-node user-added"
             :style="concept.position"
             @mousedown="startDragging(index, $event)">
          <div class="bg-green-100 border-2 border-green-300 rounded-lg px-4 py-2 text-sm font-medium text-green-700 shadow-md cursor-move hover:bg-green-50 transition-colors">
            {{ concept.text }}
            <button @click="removeConcept(index)" class="ml-2 text-red-500 hover:text-red-700">
              ×
            </button>
          </div>
        </div>

        <!-- Connection lines -->
        <svg class="absolute inset-0 w-full h-full pointer-events-none" style="z-index: 1;">
          <line v-for="connection in connections"
                :key="`${connection.from}-${connection.to}`"
                :x1="connection.x1" :y1="connection.y1"
                :x2="connection.x2" :y2="connection.y2"
                stroke="#8b5cf6" stroke-width="2" stroke-dasharray="5,5"
                class="connection-line">
          </line>
        </svg>
      </div>
    </div>

    <!-- Add Concept Input -->
    <div class="add-concept-section bg-gray-50 rounded-xl p-6 mb-6">
      <h4 class="font-semibold text-gray-800 mb-4">Dodaj powiązane pojęcie</h4>
      <div class="flex space-x-3">
        <input v-model="newConceptText"
               @keyup.enter="addConcept"
               placeholder="Wpisz pojęcie związane z tematem..."
               class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-purple-500" />
        <button @click="addConcept"
                :disabled="!newConceptText.trim()"
                class="btn-primary"
                :class="{ 'opacity-50 cursor-not-allowed': !newConceptText.trim() }">
          Dodaj
        </button>
      </div>
      <p class="text-sm text-gray-600 mt-2">
        💡 Wskazówka: Możesz przeciągać pojęcia po kanwie aby je przestawiać
      </p>
    </div>

    <!-- Relationship Builder -->
    <div v-if="userConcepts.length > 1" class="relationships-section bg-white border border-gray-200 rounded-xl p-6 mb-6">
      <h4 class="font-semibold text-gray-800 mb-4">Opisz relacje między pojęciami</h4>
      <div class="space-y-4">
        <div v-for="(relationship, index) in relationships" :key="index"
             class="relationship-item bg-gray-50 rounded-lg p-4">
          <div class="flex items-center space-x-3 mb-2">
            <select v-model="relationship.from" class="px-3 py-2 border border-gray-300 rounded">
              <option v-for="concept in allConcepts" :key="concept" :value="concept">{{ concept }}</option>
            </select>
            <span class="text-gray-500">↔</span>
            <select v-model="relationship.to" class="px-3 py-2 border border-gray-300 rounded">
              <option v-for="concept in allConcepts" :key="concept" :value="concept">{{ concept }}</option>
            </select>
            <button @click="removeRelationship(index)" class="text-red-500 hover:text-red-700">
              🗑️
            </button>
          </div>
          <input v-model="relationship.description"
                 placeholder="Opisz jak te pojęcia są ze sobą związane..."
                 class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm">
        </div>
        <button @click="addRelationship" class="btn-secondary text-sm">
          + Dodaj relację
        </button>
      </div>
    </div>

    <!-- Concept Map Summary -->
    <div class="summary-section bg-gradient-to-r from-purple-50 to-blue-50 rounded-xl p-6">
      <h4 class="font-semibold text-gray-800 mb-4">Podsumowanie mapy pojęć</h4>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-4 text-center text-sm">
        <div>
          <div class="text-2xl font-bold text-purple-600">{{ allConcepts.length }}</div>
          <div class="text-gray-600">Pojęć</div>
        </div>
        <div>
          <div class="text-2xl font-bold text-blue-600">{{ relationships.length }}</div>
          <div class="text-gray-600">Relacji</div>
        </div>
        <div>
          <div class="text-2xl font-bold text-green-600">{{ complexityScore }}</div>
          <div class="text-gray-600">Złożoność</div>
        </div>
        <div>
          <div class="text-2xl font-bold text-orange-600">{{ completionPercentage }}%</div>
          <div class="text-gray-600">Ukończenie</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'

interface ConceptMapData {
  centralConcept: string
  relatedConcepts: string[]
  instructions: string
}

interface UserConcept {
  text: string
  position: { left: string; top: string }
}

interface Relationship {
  from: string
  to: string
  description: string
}

interface Props {
  section: {
    id: string
    interactionData: ConceptMapData
  }
  initialResponse?: any
}

interface Emits {
  (e: 'response-updated', response: any): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// State
const newConceptText = ref('')
const userConcepts = ref<UserConcept[]>([])
const relationships = ref<Relationship[]>([])
const canvasContainer = ref<HTMLElement>()
const dragging = ref<{ index: number; offset: { x: number; y: number } } | null>(null)

// Computed
const interactionData = computed(() => props.section.interactionData)

const predefinedConcepts = computed(() => interactionData.value.relatedConcepts || [])

const allConcepts = computed(() => [
  interactionData.value.centralConcept,
  ...predefinedConcepts.value,
  ...userConcepts.value.map(c => c.text)
])

const connections = computed(() => {
  // Create visual connections based on relationships
  return relationships.value.map(rel => {
    // This is simplified - in a real implementation you'd calculate actual positions
    return {
      from: rel.from,
      to: rel.to,
      x1: Math.random() * 400 + 50,
      y1: Math.random() * 300 + 50,
      x2: Math.random() * 400 + 50,
      y2: Math.random() * 300 + 50
    }
  })
})

const complexityScore = computed(() => {
  const conceptCount = allConcepts.value.length
  const relationCount = relationships.value.length
  return Math.round(conceptCount + (relationCount * 1.5))
})

const completionPercentage = computed(() => {
  const expectedConcepts = Math.max(5, predefinedConcepts.value.length + 3)
  const expectedRelations = Math.max(3, Math.floor(allConcepts.value.length / 2))

  const conceptScore = Math.min(100, (allConcepts.value.length / expectedConcepts) * 60)
  const relationScore = Math.min(100, (relationships.value.length / expectedRelations) * 40)

  return Math.round(conceptScore + relationScore)
})

// Methods
const getPredefindConceptPosition = (index: number) => {
  const angle = (index / predefinedConcepts.value.length) * 2 * Math.PI
  const radius = 150
  const centerX = 200
  const centerY = 200

  return {
    position: 'absolute',
    left: `${centerX + radius * Math.cos(angle)}px`,
    top: `${centerY + radius * Math.sin(angle)}px`
  }
}

const addConcept = () => {
  if (!newConceptText.value.trim()) return

  const newConcept: UserConcept = {
    text: newConceptText.value.trim(),
    position: {
      left: `${Math.random() * 300 + 100}px`,
      top: `${Math.random() * 200 + 100}px`
    }
  }

  userConcepts.value.push(newConcept)
  newConceptText.value = ''
  updateResponse()
}

const removeConcept = (index: number) => {
  const conceptText = userConcepts.value[index].text
  userConcepts.value.splice(index, 1)

  // Remove related relationships
  relationships.value = relationships.value.filter(
    rel => rel.from !== conceptText && rel.to !== conceptText
  )

  updateResponse()
}

const addRelationship = () => {
  if (allConcepts.value.length < 2) return

  relationships.value.push({
    from: allConcepts.value[0],
    to: allConcepts.value[1],
    description: ''
  })

  updateResponse()
}

const removeRelationship = (index: number) => {
  relationships.value.splice(index, 1)
  updateResponse()
}

const startDragging = (index: number, event: MouseEvent) => {
  const rect = canvasContainer.value?.getBoundingClientRect()
  if (!rect) return

  dragging.value = {
    index,
    offset: {
      x: event.clientX - rect.left - parseInt(userConcepts.value[index].position.left),
      y: event.clientY - rect.top - parseInt(userConcepts.value[index].position.top)
    }
  }

  document.addEventListener('mousemove', handleDrag)
  document.addEventListener('mouseup', stopDragging)
}

const handleDrag = (event: MouseEvent) => {
  if (!dragging.value || !canvasContainer.value) return

  const rect = canvasContainer.value.getBoundingClientRect()
  const newX = event.clientX - rect.left - dragging.value.offset.x
  const newY = event.clientY - rect.top - dragging.value.offset.y

  userConcepts.value[dragging.value.index].position = {
    left: `${Math.max(0, Math.min(rect.width - 100, newX))}px`,
    top: `${Math.max(0, Math.min(rect.height - 50, newY))}px`
  }
}

const stopDragging = () => {
  dragging.value = null
  document.removeEventListener('mousemove', handleDrag)
  document.removeEventListener('mouseup', stopDragging)
  updateResponse()
}

const updateResponse = () => {
  emit('response-updated', {
    concepts: userConcepts.value,
    relationships: relationships.value,
    complexity: complexityScore.value,
    completion: completionPercentage.value
  })
}

// Initialize from saved data
const initializeFromSaved = () => {
  if (props.initialResponse) {
    userConcepts.value = props.initialResponse.concepts || []
    relationships.value = props.initialResponse.relationships || []
  }
}

// Watch for changes
watch([userConcepts, relationships], updateResponse, { deep: true })

// Initialize
onMounted(() => {
  initializeFromSaved()
})
</script>

<style scoped>
  .concept-node {
    position: absolute;
    z-index: 10;
  }

  .concept-canvas {
    position: relative;
    min-height: 400px;
  }

  .connection-line {
    opacity: 0.7;
  }

  .btn-primary {
    @apply bg-purple-600 hover:bg-purple-700 text-white font-medium py-2 px-4 rounded-lg transition-colors;
  }

  .btn-secondary {
    @apply bg-gray-200 hover:bg-gray-300 text-gray-700 font-medium py-2 px-4 rounded-lg transition-colors;
  }

  .relationship-item {
    transition: all 0.2s ease;
  }

    .relationship-item:hover {
      background-color: #f9fafb;
    }
</style>
