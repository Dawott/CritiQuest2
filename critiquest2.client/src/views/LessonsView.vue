<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900">Lekcje Filozofa</h1>
      <p class="mt-2 text-gray-600">Pojmij naukę zdobytą przez wieki</p>
    </div>

    <!-- Stage Filter -->
    <div class="mb-6">
      <div class="flex flex-wrap gap-2">
        <button @click="selectedStage = null"
                :class="[
            'px-4 py-2 rounded-md text-sm font-medium transition-colors',
            selectedStage === null
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          ]">
          All Stages
        </button>
        <button v-for="stage in availableStages"
                :key="stage"
                @click="selectedStage = stage"
                :class="[
            'px-4 py-2 rounded-md text-sm font-medium transition-colors',
            selectedStage === stage
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          ]">
          {{ formatStage(stage) }}
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="lessonsStore.loading" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
    </div>

    <!-- Error State -->
    <div v-else-if="lessonsStore.error" class="text-center py-12">
      <div class="text-red-600 mb-4">{{ lessonsStore.error }}</div>
      <button @click="loadLessons" class="btn-primary">Spróbuj ponownie</button>
    </div>

    <!-- Lessons Grid -->
    <div v-else class="space-y-8">
      <div v-for="(stageLessons, stage) in filteredLessons" :key="stage">
        <h2 class="text-xl font-semibold text-gray-900 mb-4 capitalize">
          {{ formatStage(stage) }}
        </h2>
        <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
          <div v-for="lesson in stageLessons"
               :key="lesson.id"
               class="card hover:shadow-lg transition-shadow duration-200"
               :class="{
              'opacity-75': !isLessonAvailable(lesson),
              'ring-2 ring-green-200': lesson.isCompleted
            }">
            <div class="card-body">
              <!-- Lesson Header -->
              <div class="flex justify-between items-start mb-3">
                <div class="flex-1">
                  <h3 class="text-lg font-medium text-gray-900 mb-1">
                    {{ lesson.title }}
                  </h3>
                  <p class="text-sm text-gray-600 mb-2">
                    {{ lesson.description }}
                  </p>
                </div>
                <div class="ml-2 flex-shrink-0">
                  <span v-if="lesson.isCompleted" class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800">
                    ✓ Zakończona
                  </span>
                  <span v-else-if="!isLessonAvailable(lesson)" class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-gray-100 text-gray-600">
                    🔒 Zablokowana
                  </span>
                  <span v-else class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                    Dostępne
                  </span>
                </div>
              </div>

              <!-- Lesson Info -->
              <div class="space-y-2 mb-4">
                <div class="flex justify-between text-sm">
                  <span class="text-gray-500">Trudność:</span>
                  <span :class="getDifficultyColor(lesson.difficulty)">
                    {{ lesson.difficulty }}
                  </span>
                </div>
                <div class="flex justify-between text-sm">
                  <span class="text-gray-500">Czas do skończenia:</span>
                  <span class="text-gray-700">{{ lesson.estimatedTime }} min</span>
                </div>
                <div class="flex justify-between text-sm">
                  <span class="text-gray-500">Nagroda:</span>
                  <span class="text-gray-700">{{ lesson.rewards.xp }} XP</span>
                </div>
              </div>

              <!-- Progress Bar (if completed) -->
              <div v-if="lesson.userProgress" class="mb-4">
                <div class="flex justify-between text-sm text-gray-600 mb-1">
                  <span>Najlepszy wynik</span>
                  <span>{{ lesson.userProgress.bestScore }}%</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-2">
                  <div class="bg-green-500 h-2 rounded-full"
                       :style="{ width: `${lesson.userProgress.bestScore}%` }"></div>
                </div>
                <div class="text-xs text-gray-500 mt-1">
                  Prób: {{ lesson.userProgress.attempts }} |
                  Czas: {{ formatTime(lesson.userProgress.timeSpent) }}
                </div>
              </div>

              <!-- Philosophical Concepts -->
              <div class="mb-4">
                <div class="text-xs text-gray-500 mb-1">Pojęcia:</div>
                <div class="flex flex-wrap gap-1">
                  <span v-for="concept in lesson.philosophicalConcepts.slice(0, 3)"
                        :key="concept"
                        class="inline-block px-2 py-1 text-xs bg-gray-100 text-gray-600 rounded">
                    {{ formatConcept(concept) }}
                  </span>
                  <span v-if="lesson.philosophicalConcepts.length > 3"
                        class="inline-block px-2 py-1 text-xs bg-gray-100 text-gray-600 rounded">
                    +{{ lesson.philosophicalConcepts.length - 3 }} więcej
                  </span>
                </div>
              </div>

              <!-- Action Button -->
              <div class="flex justify-between items-center">
                <button @click="startLesson(lesson)"
                        :disabled="!isLessonAvailable(lesson)"
                        :class="[
                    'btn-primary',
                    !isLessonAvailable(lesson) && 'opacity-50 cursor-not-allowed'
                  ]">
                  {{ lesson.isCompleted ? 'Review' : 'Start Lesson' }}
                </button>

                <div class="text-xs text-gray-500">
                  {{ lesson.order }}/{{ getStageLessonCount(lesson.stage) }}
                </div>
              </div>

              <!-- Required Philosopher Warning -->
              <div v-if="lesson.requiredPhilosopher && !hasRequiredPhilosopher(lesson)" class="mt-3 p-2 bg-yellow-50 border border-yellow-200 rounded-md">
                <div class="flex items-center">
                  <span class="text-yellow-600 text-xs">
                    ⚠️ Zalecani filozofowie: {{ formatPhilosopherName(lesson.requiredPhilosopher) }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Stage Progress -->
        <div class="mt-4 p-4 bg-gray-50 rounded-lg">
          <div class="flex justify-between items-center text-sm">
            <span class="text-gray-600">Postęp etapów</span>
            <span class="font-medium">
              {{ getStageCompletedCount(stage) }}/{{ stageLessons.length }} lekcji
            </span>
          </div>
          <div class="w-full bg-gray-200 rounded-full h-2 mt-2">
            <div class="bg-primary-600 h-2 rounded-full transition-all duration-300"
                 :style="{ width: `${getStageProgress(stage)}%` }"></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-if="!lessonsStore.loading && !lessonsStore.error && Object.keys(filteredLessons).length === 0" class="text-center py-12">
      <div class="text-gray-500 mb-4">Nie ma lekcji na tym etapie</div>
      <button @click="selectedStage = null" class="btn-secondary">Zobacz wszystkie</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useLessonsStore } from '@/stores/lessons'
import { useProfileStore } from '@/stores/profile'
import type { Lesson } from '@/services/lessonApi'

const router = useRouter()
const lessonsStore = useLessonsStore()
const profileStore = useProfileStore()

const selectedStage = ref<string | null>(null)

const availableStages = computed(() =>
  Object.keys(lessonsStore.lessonsByStage)
)

const filteredLessons = computed(() => {
  if (selectedStage.value) {
    const stage = selectedStage.value
    return { [stage]: lessonsStore.lessonsByStage[stage] || [] }
  }
  return lessonsStore.lessonsByStage
})

const isLessonAvailable = (lesson: Lesson) => {
  // Always allow access to completed lessons
  if (lesson.isCompleted) return true

  // Check if required philosopher is owned
  /*if (lesson.requiredPhilosopher) {
    return hasRequiredPhilosopher(lesson)
  }*/

  return true
}

const hasRequiredPhilosopher = (lesson: Lesson) => {
  if (!lesson.requiredPhilosopher || !profileStore.profile) return true

  return profileStore.profile.progression.unlockedPhilosophers.includes(lesson.requiredPhilosopher)
}

const getDifficultyColor = (difficulty: string) => {
  const colors = {
    beginner: 'text-green-600',
    intermediate: 'text-yellow-600',
    advanced: 'text-orange-600',
    expert: 'text-red-600'
  }
  return colors[difficulty.toLowerCase() as keyof typeof colors] || 'text-gray-600'
}

const getStageLessonCount = (stage: string) => {
  return lessonsStore.lessonsByStage[stage]?.length || 0
}

const getStageCompletedCount = (stage: string) => {
  return lessonsStore.lessonsByStage[stage]?.filter(l => l.isCompleted).length || 0
}

const getStageProgress = (stage: string) => {
  const total = getStageLessonCount(stage)
  const completed = getStageCompletedCount(stage)
  return total > 0 ? Math.round((completed / total) * 100) : 0
}

const formatStage = (stage: string) => {
  return stage.replace(/-/g, ' ')
}

const formatConcept = (concept: string) => {
  return concept.replace(/-/g, ' ')
}

const formatPhilosopherName = (philosopherId: string) => {
  // Simple conversion from ID to display name
  return philosopherId.replace(/-/g, ' ').replace(/\b\w/g, l => l.toUpperCase())
}

const formatTime = (minutes: number) => {
  if (minutes < 60) return `${minutes}m`
  const hours = Math.floor(minutes / 60)
  const mins = minutes % 60
  return `${hours}h ${mins}m`
}

const startLesson = (lesson: Lesson) => {
  if (!isLessonAvailable(lesson)) return

  // Navigate to lesson detail view (you'll need to create this route)
  router.push(`/lessons/${lesson.id}`)
}

const loadLessons = async () => {
  try {
    await lessonsStore.fetchLessons()
    // Also ensure profile is loaded for checking philosopher ownership
    if (!profileStore.profile) {
      await profileStore.fetchProfile()
    }
  } catch (error) {
    console.error('Failed to load lessons:', error)
  }
}

onMounted(() => {
  loadLessons()
})
</script>
