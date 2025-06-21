<template>
  <div class="timeline-interaction">
    <!-- Instructions -->
    <div class="instructions-card mb-6">
      <div class="bg-blue-50 border border-blue-200 rounded-xl p-6">
        <h4 class="text-blue-900 font-semibold mb-3 flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L10 9.586V6z" clip-rule="evenodd"></path>
          </svg>
          Linia czasu: {{ interactionData.topic }}
        </h4>
        <p class="text-blue-800">{{ interactionData.instructions }}</p>
      </div>
    </div>

    <!-- Timeline Display -->
    <div class="timeline-container bg-white border border-gray-200 rounded-xl p-8 mb-6">
      <div class="relative">
        <!-- Timeline Line -->
        <div class="absolute left-8 top-0 bottom-0 w-1 bg-gradient-to-b from-purple-400 to-blue-400"></div>

        <!-- Timeline Events -->
        <div class="space-y-8">
          <div v-for="(event, index) in allEvents"
               :key="index"
               class="timeline-event relative flex items-start"
               :class="{ 'user-event': event.isUserAdded }">

            <!-- Event Marker -->
            <div class="absolute left-6 w-6 h-6 rounded-full border-4 border-white shadow-lg flex items-center justify-center text-xs font-bold"
                 :class="event.isUserAdded ? 'bg-green-500 text-white' : 'bg-purple-500 text-white'">
              {{ index + 1 }}
            </div>

            <!-- Event Content -->
            <div class="ml-16 flex-1">
              <div class="bg-gray-50 rounded-lg p-4 hover:bg-gray-100 transition-colors"
                   :class="{ 'border-l-4 border-green-400': event.isUserAdded }">

                <div class="flex justify-between items-start mb-2">
                  <span class="text-lg font-semibold text-purple-800">{{ event.year }}</span>
                  <button v-if="event.isUserAdded"
                          @click="removeUserEvent(event.id)"
                          class="text-red-500 hover:text-red-700 text-sm">
                    🗑️
                  </button>
                </div>

                <h5 class="font-medium text-gray-800 mb-2">{{ event.event }}</h5>
                <p class="text-gray-600 text-sm">{{ event.description }}</p>

                <div v-if="event.isUserAdded" class="mt-3">
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Dlaczego to wydarzenie jest ważne?
                  </label>
                  <textarea v-model="event.significance"
                            placeholder="Opisz znaczenie tego wydarzenia..."
                            class="w-full p-2 border border-gray-300 rounded text-sm"
                            rows="2"
                            @input="updateResponse"></textarea>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Add Event Form -->
    <div class="add-event-section bg-gray-50 rounded-xl p-6 mb-6">
      <h4 class="font-semibold text-gray-800 mb-4">Dodaj wydarzenie do linii czasu</h4>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
        <input v-model="newEvent.year"
               placeholder="Rok (np. 1641)"
               class="px-3 py-2 border border-gray-300 rounded-lg">
        <input v-model="newEvent.event"
               placeholder="Nazwa wydarzenia"
               class="px-3 py-2 border border-gray-300 rounded-lg">
        <button @click="addUserEvent"
                :disabled="!canAddEvent"
                class="btn-primary"
                :class="{ 'opacity-50 cursor-not-allowed': !canAddEvent }">
          Dodaj
        </button>
      </div>
      <textarea v-model="newEvent.description"
                placeholder="Opisz wydarzenie i jego znaczenie..."
                class="w-full px-3 py-2 border border-gray-300 rounded-lg"
                rows="3"></textarea>
    </div>

    <!-- Timeline Analysis -->
    <div class="analysis-section bg-gradient-to-r from-purple-50 to-blue-50 rounded-xl p-6">
      <h4 class="font-semibold text-gray-800 mb-4">Analiza linii czasu</h4>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-4 text-center text-sm">
        <div>
          <div class="text-2xl font-bold text-purple-600">{{ allEvents.length }}</div>
          <div class="text-gray-600">Wydarzeń</div>
        </div>
        <div>
          <div class="text-2xl font-bold text-blue-600">{{ userEventsCount }}</div>
          <div class="text-gray-600">Dodanych</div>
        </div>
        <div>
          <div class="text-2xl font-bold text-green-600">{{ timeSpan }}</div>
          <div class="text-gray-600">Lat</div>
        </div>
        <div>
          <div class="text-2xl font-bold text-orange-600">{{ averageEventSpacing }}</div>
          <div class="text-gray-600">Śr. odstęp</div>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'

interface TimelineEvent {
  id?: string
  year: string
  event: string
  description: string
  isUserAdded?: boolean
  significance?: string
}

interface TimelineData {
  topic: string
  events: TimelineEvent[]
  instructions: string
}

interface Props {
  section: {
    id: string
    interactionData: TimelineData
  }
  initialResponse?: any
}

interface Emits {
  (e: 'response-updated', response: any): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// State
const userEvents = ref<TimelineEvent[]>([])
const newEvent = ref({
  year: '',
  event: '',
  description: ''
})

// Computed
const interactionData = computed(() => props.section.interactionData)

const allEvents = computed(() => {
  const combined = [
    ...interactionData.value.events.map(e => ({ ...e, isUserAdded: false })),
    ...userEvents.value.map(e => ({ ...e, isUserAdded: true }))
  ]

  return combined.sort((a, b) => parseInt(a.year) - parseInt(b.year))
})

const userEventsCount = computed(() => userEvents.value.length)

const timeSpan = computed(() => {
  if (allEvents.value.length < 2) return 0
  const years = allEvents.value.map(e => parseInt(e.year)).filter(y => !isNaN(y))
  return Math.max(...years) - Math.min(...years)
})

const averageEventSpacing = computed(() => {
  if (allEvents.value.length < 2) return 0
  return Math.round(timeSpan.value / (allEvents.value.length - 1))
})

const canAddEvent = computed(() => {
  return newEvent.value.year.trim() &&
         newEvent.value.event.trim() &&
         newEvent.value.description.trim() &&
         !isNaN(parseInt(newEvent.value.year))
})

// Methods
const addUserEvent = () => {
  if (!canAddEvent.value) return

  const event: TimelineEvent = {
    id: Date.now().toString(),
    year: newEvent.value.year.trim(),
    event: newEvent.value.event.trim(),
    description: newEvent.value.description.trim(),
    significance: '',
    isUserAdded: true
  }

  userEvents.value.push(event)

  // Reset form
  newEvent.value = { year: '', event: '', description: '' }

  updateResponse()
}

const removeUserEvent = (eventId?: string) => {
  if (!eventId) return
  userEvents.value = userEvents.value.filter(e => e.id !== eventId)
  updateResponse()
}

const updateResponse = () => {
  emit('response-updated', {
    events: userEvents.value,
    analysis: {
      totalEvents: allEvents.value.length,
      userContributions: userEventsCount.value,
      timeSpan: timeSpan.value,
      averageSpacing: averageEventSpacing.value
    }
  })
}

// Initialize
const initializeFromSaved = () => {
  if (props.initialResponse?.events) {
    userEvents.value = props.initialResponse.events
  }
}

// Watch for changes
watch(userEvents, updateResponse, { deep: true })

onMounted(() => {
  initializeFromSaved()
})
</script>
