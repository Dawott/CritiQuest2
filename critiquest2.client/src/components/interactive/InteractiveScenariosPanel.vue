<template>
  <div v-if="sections.length > 0" class="interactive-scenarios-panel">
    <!-- Header -->
    <div class="bg-gradient-to-r from-purple-500 to-blue-500 text-white rounded-xl p-6 mb-6">
      <h3 class="text-xl font-bold mb-2">🎯 Interaktywne scenariusze</h3>
      <p class="text-purple-100">
        Pogłęb swoje zrozumienie przez praktyczne ćwiczenia
      </p>

      <!-- Progress -->
      <div v-if="progress" class="mt-4">
        <div class="flex justify-between text-sm mb-2">
          <span>Postęp: {{ progress.completedSections }}/{{ progress.totalSections }}</span>
          <span>{{ progress.completionPercentage }}%</span>
        </div>
        <div class="w-full bg-purple-300 rounded-full h-2">
          <div class="bg-white h-2 rounded-full transition-all duration-300"
               :style="{ width: `${progress.completionPercentage}%` }"></div>
        </div>
      </div>
    </div>

    <!-- Scenarios List -->
    <div class="space-y-4">
      <div v-for="section in sections"
           :key="section.id"
           class="interactive-scenario-card"
           :class="{ 'completed': section.userResponse?.isCompleted }">

        <div class="bg-white border border-gray-200 rounded-xl p-6 hover:border-purple-300 transition-colors cursor-pointer"
             @click="openScenario(section)">

          <!-- Header -->
          <div class="flex items-start justify-between mb-4">
            <div class="flex items-center">
              <div class="scenario-icon mr-3"
                   :class="getScenarioIconClass(section.type)">
                {{ getScenarioIcon(section.type) }}
              </div>
              <div>
                <h4 class="font-semibold text-gray-800">{{ section.title }}</h4>
                <p class="text-sm text-gray-600">{{ formatScenarioType(section.type) }}</p>
              </div>
            </div>

            <!-- Status -->
            <div class="flex items-center space-x-2">
              <span v-if="section.userResponse?.isCompleted"
                    class="bg-green-100 text-green-800 px-2 py-1 rounded-full text-xs font-medium">
                ✓ Ukończone
              </span>
              <span v-else-if="section.userResponse"
                    class="bg-yellow-100 text-yellow-800 px-2 py-1 rounded-full text-xs font-medium">
                {{ section.userResponse.completionPercentage }}% w trakcie
              </span>
              <span v-else
                    class="bg-gray-100 text-gray-600 px-2 py-1 rounded-full text-xs font-medium">
                Nierozpoczęte
              </span>
            </div>
          </div>

          <!-- Description -->
          <p class="text-gray-700 text-sm mb-4">{{ section.description }}</p>

          <!-- Footer -->
          <div class="flex justify-between items-center text-xs text-gray-500">
            <span>⏱️ {{ section.estimatedTimeMinutes }} min</span>
            <span v-if="section.isRequired" class="bg-red-100 text-red-600 px-2 py-1 rounded">
              Wymagane
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useInteractiveScenariosStore } from '@/stores/interactiveScenarios'
import type { InteractiveSection } from '@/services/interactiveScenariosApi'

interface Props {
  lessonId: string
}

const props = defineProps<Props>()
const router = useRouter()
const store = useInteractiveScenariosStore()

// State
const sections = ref<InteractiveSection[]>([])

// Computed
const progress = computed(() => store.lessonProgress[props.lessonId])

// Methods
const formatScenarioType = (type: string) => {
  const types: Record<string, string> = {
    'Reflection': 'Refleksja',
    'ConceptMap': 'Mapa Pojęć',
    'Timeline': 'Linia Czasu',
    'Comparison': 'Porównanie',
    'Scenario': 'Scenariusz'
  }
  return types[type] || type
}

const getScenarioIcon = (type: string) => {
  const icons: Record<string, string> = {
    'Reflection': '💭',
    'ConceptMap': '🗺️',
    'Timeline': '📅',
    'Comparison': '⚖️',
    'Scenario': '🎭'
  }
  return icons[type] || '📝'
}

const getScenarioIconClass = (type: string) => {
  const classes: Record<string, string> = {
    'Reflection': 'bg-purple-100 text-purple-600',
    'ConceptMap': 'bg-blue-100 text-blue-600',
    'Timeline': 'bg-green-100 text-green-600',
    'Comparison': 'bg-orange-100 text-orange-600',
    'Scenario': 'bg-pink-100 text-pink-600'
  }
  return `w-10 h-10 rounded-full flex items-center justify-center text-lg ${classes[type] || 'bg-gray-100 text-gray-600'}`
}

const openScenario = (section: InteractiveSection) => {
  router.push(`/interactive-scenarios/${section.id}`)
}

const loadData = async () => {
  try {
    sections.value = await store.getLessonSections(props.lessonId)
    await store.getLessonProgress(props.lessonId)
  } catch (error) {
    console.error('Failed to load interactive scenarios:', error)
  }
}

// Initialize
onMounted(() => {
  loadData()
})
</script>

<style scoped>
  .interactive-scenario-card.completed {
    opacity: 0.8;
  }

  .interactive-scenario-card:hover {
    transform: translateY(-1px);
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  }

  .scenario-icon {
    transition: all 0.2s ease;
  }
</style>
