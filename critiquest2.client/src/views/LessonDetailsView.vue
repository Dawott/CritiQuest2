<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Loading State -->
    <div v-if="lessonsStore.loading" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
    </div>

    <!-- Error State -->
    <div v-else-if="lessonsStore.error" class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div class="text-center py-12">
        <div class="text-red-600 mb-4">{{ lessonsStore.error }}</div>
        <button @click="loadLesson" class="btn-primary">Spróbuj ponownie</button>
      </div>
    </div>

    <!-- Lesson Content -->
    <div v-else-if="lesson" class="max-w-6xl mx-auto">
      <!-- Header -->
      <div class="bg-white shadow-sm border-b">
        <div class="px-4 sm:px-6 lg:px-8 py-6">
          <div class="flex items-center justify-between">
            <button @click="goBack" class="text-primary-600 hover:text-primary-500 mb-4">
              ← Powrót do lekcji
            </button>
          </div>

          <div class="flex flex-col lg:flex-row lg:items-center lg:justify-between">
            <div class="flex-1">
              <h1 class="text-3xl font-bold text-gray-900">{{ lesson.title }}</h1>
              <p class="mt-2 text-lg text-gray-600">{{ lesson.description }}</p>

              <!-- Lesson Meta -->
              <div class="mt-4 flex flex-wrap gap-4 text-sm text-gray-500">
                <span class="flex items-center">
                  📊 {{ lesson.difficulty }}
                </span>
                <span class="flex items-center">
                  ⏱️ {{ lesson.estimatedTime }} min
                </span>
                <span class="flex items-center">
                  🎯 {{ lesson.rewards.xp }} XP
                </span>
                <span class="flex items-center">
                  📖 {{ formatStage(lesson.stage) }}
                </span>
              </div>

              <!-- Philosophical Concepts -->
              <div class="mt-4">
                <span class="text-sm text-gray-500 mb-2 block">Koncepcje filozoficzne:</span>
                <div class="flex flex-wrap gap-2">
                  <span v-for="concept in lesson.philosophicalConcepts"
                        :key="concept"
                        class="inline-block px-3 py-1 text-xs bg-primary-100 text-primary-800 rounded-full">
                    {{ formatConcept(concept) }}
                  </span>
                </div>
              </div>
            </div>

            <!-- Progress -->
            <div class="mt-6 lg:mt-0 lg:ml-8">
              <div class="bg-gray-50 p-4 rounded-lg min-w-[200px]">
                <div class="text-sm text-gray-600 mb-2">Postęp lekcji</div>
                <div class="flex justify-between text-sm mb-1">
                  <span>{{ currentSectionIndex + 1 }}/{{ lesson.content.sections.length }}</span>
                  <span>{{ Math.round(((currentSectionIndex + 1) / lesson.content.sections.length) * 100) }}%</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-2">
                  <div class="bg-primary-600 h-2 rounded-full transition-all duration-300"
                       :style="{ width: `${((currentSectionIndex + 1) / lesson.content.sections.length) * 100}%` }"></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Content Area -->
      <div class="px-4 sm:px-6 lg:px-8 py-8">
        <div class="bg-white rounded-lg shadow-sm">
          <!-- Section Content -->
          <div class="p-8">
            <div v-if="currentSection" :key="currentSection.id">
              <!-- Section Title -->
              <h2 class="text-2xl font-semibold text-gray-900 mb-6">
                {{ currentSection.title }}
              </h2>

              <!-- Text Content -->
              <div v-if="currentSection.type === 'text'" class="prose prose-lg max-w-none">
                <div v-html="formatContent(currentSection.content)"></div>
              </div>

              <!-- Interactive Content -->
              <InteractiveSection v-if="currentSection.type === 'interactive'"
                                  :section="currentSection"
                                  :lessonId="lesson.id"
                                  class="mb-8" />

              <!-- Reflection Prompts -->
              <div v-if="currentSection.interactionType === 'reflection' && currentSection.interactionData?.prompts"
                   class="bg-primary-50 p-6 rounded-lg">
                <h3 class="text-lg font-medium text-primary-900 mb-4">💭 Czas na refleksję</h3>
                <div class="space-y-4">
                  <div v-for="(prompt, index) in currentSection.interactionData.prompts" :key="index" class="space-y-2">
                    <label class="block text-sm font-medium text-primary-800">{{ prompt }}</label>
                    <textarea v-model="reflectionResponses[index]"
                              class="w-full px-3 py-2 border border-primary-200 rounded-md focus:ring-primary-500 focus:border-primary-500"
                              rows="3"
                              :placeholder="'Napisz swoje przemyślenia...'"></textarea>
                  </div>
                </div>
              </div>

              <!-- Concept Map -->
              <div v-else-if="currentSection.interactionType === 'concept-map'"
                   class="bg-purple-50 p-6 rounded-lg">
                <h3 class="text-lg font-medium text-purple-900 mb-4">🗺️ Mapa pojęć</h3>
                <div class="text-sm text-purple-700 mb-4">
                  Połącz poniższe koncepcje i wyjaśnij ich relacje:
                </div>
                <div class="grid grid-cols-2 md:grid-cols-3 gap-4">
                  <div v-for="concept in lesson.philosophicalConcepts" :key="concept"
                       class="bg-white p-3 rounded border border-purple-200 text-center">
                    {{ formatConcept(concept) }}
                  </div>
                </div>
                <textarea v-model="conceptMapResponse"
                          class="mt-4 w-full px-3 py-2 border border-purple-200 rounded-md focus:ring-purple-500 focus:border-purple-500"
                          rows="4"
                          placeholder="Opisz jak te koncepcje się ze sobą łączą..."></textarea>
              </div>

              <!-- Timeline -->
              <div v-else-if="currentSection.interactionType === 'timeline'"
                   class="bg-yellow-50 p-6 rounded-lg">
                <h3 class="text-lg font-medium text-yellow-900 mb-4">📅 Linia czasu</h3>
                <div class="text-sm text-yellow-700 mb-4">
                  Umieść wydarzenia w chronologicznym porządku:
                </div>
                <!-- Simple timeline placeholder -->
                <div class="space-y-2">
                  <div class="bg-white p-3 rounded border border-yellow-200">
                    Wydarzenia do uporządkowania będą tutaj...
                  </div>
                </div>
              </div>
            </div>

            <!-- Video Content -->
            <div v-else-if="currentSection.type === 'video' && currentSection.mediaUrl" class="space-y-6">
              <div class="prose prose-lg max-w-none">
                <div v-html="formatContent(currentSection.content)"></div>
              </div>
              <div class="bg-gray-100 p-4 rounded-lg">
                <div class="text-center text-gray-600">
                  🎥 Video: {{ currentSection.mediaUrl }}
                  <div class="text-sm mt-2">(Video integration to be implemented)</div>
                </div>
              </div>
            </div>

            <!-- Image Content -->
            <div v-else-if="currentSection.type === 'image' && currentSection.mediaUrl" class="space-y-6">
              <div class="prose prose-lg max-w-none">
                <div v-html="formatContent(currentSection.content)"></div>
              </div>
              <div class="text-center">
                <img :src="currentSection.mediaUrl"
                     :alt="currentSection.title"
                     class="max-w-full h-auto rounded-lg shadow-md mx-auto"
                     @error="handleImageError">
              </div>
            </div>
          </div>
        </div>

        <!-- Navigation -->
        <div class="px-8 py-6 bg-gray-50 rounded-b-lg flex justify-between items-center">
          <button @click="previousSection"
                  :disabled="currentSectionIndex === 0"
                  class="btn-secondary"
                  :class="{ 'opacity-50 cursor-not-allowed': currentSectionIndex === 0 }">
            ← Poprzednia sekcja
          </button>

          <div class="text-sm text-gray-600">
            Sekcja {{ currentSectionIndex + 1 }} z {{ lesson.content.sections.length }}
          </div>

          <button v-if="currentSectionIndex < lesson.content.sections.length - 1"
                  @click="nextSection"
                  class="btn-primary">
            Następna sekcja →
          </button>

          <button v-else @click="completeLesson" class="btn-primary bg-green-600 hover:bg-green-700">
            Zakończ lekcję i przejdź do quizu →
          </button>
        </div>
      </div>
    </div>
  </div>

  <!-- Complete Lesson Modal -->
  <div v-if="showCompleteModal"
       class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50"
       @click="closeCompleteModal">
    <div class="bg-white rounded-lg max-w-md w-full p-6" @click.stop>
      <h3 class="text-lg font-medium text-gray-900 mb-4">🎉 Gratulacje!</h3>
      <p class="text-gray-600 mb-6">
        Ukończyłeś lekcję "{{ lesson?.title }}". Czy chcesz przejść do quizu?
      </p>
      <div class="flex justify-end space-x-3">
        <button @click="closeCompleteModal" class="btn-secondary">
          Później
        </button>
        <button @click="goToQuiz" class="btn-primary">
          Przejdź do quizu
        </button>
      </div>
    </div>
  </div>
  </div>
  <InteractiveScenariosPanel v-if="lesson"
                             :lesson-id="lesson.id"
                             class="mt-8" />
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useLessonsStore } from '@/stores/lessons'
import { useProfileStore } from '@/stores/profile'
  import type { Lesson } from '@/services/lessonApi'
  import InteractiveSection from '@/components/lessons/InteractiveSection.vue'
  import InteractiveScenariosPanel from '@/components/interactive/InteractiveScenariosPanel.vue'

const route = useRoute()
const router = useRouter()
const lessonsStore = useLessonsStore()
const profileStore = useProfileStore()

const currentSectionIndex = ref(0)
const reflectionResponses = ref<string[]>([])
const conceptMapResponse = ref('')
const showCompleteModal = ref(false)
const startTime = ref(Date.now())

const lesson = computed(() => lessonsStore.currentLesson)
const currentSection = computed(() => {
  if (!lesson.value?.content.sections) return null
  return lesson.value.content.sections[currentSectionIndex.value]
})

const formatContent = (content: string) => {
  return content.replace(/\n/g, '<br>')
}

const formatStage = (stage: string) => {
  return stage.replace(/-/g, ' ')
}

const formatConcept = (concept: string) => {
  return concept.replace(/-/g, ' ')
}

const handleImageError = (event: Event) => {
  const img = event.target as HTMLImageElement
  img.src = 'https://via.placeholder.com/600x400?text=Obraz+niedostępny'
}

const previousSection = () => {
  if (currentSectionIndex.value > 0) {
    currentSectionIndex.value--
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

const nextSection = () => {
  if (lesson.value && currentSectionIndex.value < lesson.value.content.sections.length - 1) {
    currentSectionIndex.value++
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

const completeLesson = () => {
  showCompleteModal.value = true
}

const closeCompleteModal = () => {
  showCompleteModal.value = false
}

const goToQuiz = async () => {
  if (!lesson.value) return

  try {
    const timeSpent = Math.floor((Date.now() - startTime.value) / 1000 / 60) // minutes

    await lessonsStore.completeLesson(lesson.value.id, {
      score: 100, // Full completion score
      timeSpent,
      notes: reflectionResponses.value.filter(r => r.trim()).join('\n\n')
    })

    // Navigate to quiz
    router.push(`/quiz/${lesson.value.quizId}`)
  } catch (error) {
    console.error('Failed to complete lesson:', error)
    // Still navigate to quiz on error
    router.push(`/quiz/${lesson.value.quizId}`)
  }
}

const goBack = () => {
  router.push('/lessons')
}

const loadLesson = async () => {
  const lessonId = route.params.id as string
  if (!lessonId) {
    router.push('/lessons')
    return
  }

  try {
    await lessonsStore.fetchLesson(lessonId)

    // Reset section state for new lesson
    currentSectionIndex.value = 0
    reflectionResponses.value = []
    conceptMapResponse.value = ''
    startTime.value = Date.now()
  } catch (error) {
    console.error('Failed to load lesson:', error)
  }
}

// Reset responses when section changes
watch(currentSection, (newSection) => {
  if (newSection?.interactionType === 'reflection') {
    const promptCount = newSection.interactionData?.prompts?.length || 0
    reflectionResponses.value = new Array(promptCount).fill('')
  }
})

onMounted(() => {
  loadLesson()
})

// Watch for route changes (if navigating between lessons)
watch(() => route.params.id, () => {
  loadLesson()
})
</script>
