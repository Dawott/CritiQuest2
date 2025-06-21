<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900">Twój Gimnazjon</h1>
      <p class="mt-2 text-gray-600">Zarządzaj swoją Szkołą</p>

      <!-- Collection Stats -->
      <div v-if="philosophersStore.collectionStats" class="mt-4 grid grid-cols-2 md:grid-cols-4 gap-4">
        <div class="bg-white p-4 rounded-lg shadow">
          <div class="text-2xl font-bold text-primary-600">{{ philosophersStore.collectionStats.owned }}</div>
          <div class="text-sm text-gray-600">Posiadane</div>
        </div>
        <div class="bg-white p-4 rounded-lg shadow">
          <div class="text-2xl font-bold text-green-600">{{ philosophersStore.collectionStats.percentage }}%</div>
          <div class="text-sm text-gray-600">Szkoła</div>
        </div>
        <div class="bg-white p-4 rounded-lg shadow">
          <div class="text-2xl font-bold text-purple-600">{{ legendaryCount }}</div>
          <div class="text-sm text-gray-600">Legendarni</div>
        </div>
        <div class="bg-white p-4 rounded-lg shadow">
          <div class="text-2xl font-bold text-orange-600">{{ epicCount }}</div>
          <div class="text-sm text-gray-600">Epiccy</div>
        </div>
      </div>
    </div>

    <!-- Filter Tabs -->
    <div class="mb-6">
      <div class="border-b border-gray-200">
        <nav class="-mb-px flex space-x-8">
          <button @click="activeTab = 'all'"
                  :class="[
              'py-2 px-1 border-b-2 font-medium text-sm',
              activeTab === 'all'
                ? 'border-primary-500 text-primary-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            ]">
            Wszyscy Filozofowie ({{ philosophersStore.philosophers.length }})
          </button>
          <button @click="activeTab = 'owned'"
                  :class="[
              'py-2 px-1 border-b-2 font-medium text-sm',
              activeTab === 'owned'
                ? 'border-primary-500 text-primary-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            ]">
            Moja Kolekcja ({{ philosophersStore.ownedPhilosophers.length }})
          </button>
          <button @click="activeTab = 'missing'"
                  :class="[
              'py-2 px-1 border-b-2 font-medium text-sm',
              activeTab === 'missing'
                ? 'border-primary-500 text-primary-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            ]">
            Brakuje ({{ missingPhilosophers.length }})
          </button>
        </nav>
      </div>
    </div>

    <!-- Rarity Filter -->
    <div class="mb-6">
      <div class="flex flex-wrap gap-2">
        <button @click="selectedRarity = null"
                :class="[
            'px-3 py-2 rounded-md text-sm font-medium transition-colors',
            selectedRarity === null
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          ]">
          Wszystkie Rzadkie
        </button>
        <button v-for="rarity in availableRarities"
                :key="rarity"
                @click="selectedRarity = rarity"
                :class="[
            'px-3 py-2 rounded-md text-sm font-medium transition-colors',
            selectedRarity === rarity
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          ]">
          <span :class="getRarityColor(rarity)">{{ rarity }}</span>
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="philosophersStore.loading" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
    </div>

    <!-- Error State -->
    <div v-else-if="philosophersStore.error" class="text-center py-12">
      <div class="text-red-600 mb-4">{{ philosophersStore.error }}</div>
      <button @click="loadData" class="btn-primary">Spróbuj ponownie</button>
    </div>

    <!-- Philosophers Grid -->
    <div v-else-if="filteredPhilosophers.length > 0" class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
      <div v-for="philosopher in filteredPhilosophers"
           :key="philosopher.id"
           @click="selectPhilosopher(philosopher)"
           class="card hover:shadow-lg transition-all duration-200 cursor-pointer transform hover:scale-105"
           :class="{
          'ring-2 ring-primary-500': selectedPhilosopher?.id === philosopher.id,
          'opacity-60': !philosopher.isOwned && activeTab !== 'all'
        }">
        <div class="card-body p-4">
          <!-- Philosopher Image -->
          <div class="aspect-square mb-4 relative">
            <img :src="philosopher.imageUrl"
                 :alt="philosopher.name"
                 class="w-full h-full object-cover rounded-lg"
                 @error="handleImageError" />
            <!-- Rarity Badge -->
            <div class="absolute top-2 right-2">
              <span :class="[
                'px-2 py-1 rounded-full text-xs font-bold',
                getRarityBadgeClass(philosopher.rarity)
              ]">
                {{ philosopher.rarity }}
              </span>
            </div>
            <!-- Ownership Status -->
            <div v-if="philosopher.isOwned" class="absolute top-2 left-2">
              <span class="bg-green-500 text-white px-2 py-1 rounded-full text-xs font-bold">
                ✓ Posiadany
              </span>
            </div>
            <!-- Level Badge (if owned) -->
            <div v-if="philosopher.userData" class="absolute bottom-2 left-2">
              <span class="bg-blue-500 text-white px-2 py-1 rounded-full text-xs font-bold">
                Lv.{{ philosopher.userData.level }}
              </span>
            </div>
          </div>

          <!-- Philosopher Info -->
          <div class="text-center">
            <h3 class="font-semibold text-gray-900 mb-1">{{ philosopher.name }}</h3>
            <p class="text-sm text-gray-600 mb-2">{{ philosopher.era }}</p>
            <p class="text-xs text-gray-500 mb-3">{{ philosopher.school }}</p>

            <!-- Stats Preview -->
            <div class="grid grid-cols-5 gap-1 text-xs">
              <div class="text-center">
                <div class="text-blue-600 font-medium">{{ getStatValue(philosopher, 'wisdom') }}</div>
                <div class="text-gray-500">WIE</div>
              </div>
              <div class="text-center">
                <div class="text-green-600 font-medium">{{ getStatValue(philosopher, 'logic') }}</div>
                <div class="text-gray-500">LOG</div>
              </div>
              <div class="text-center">
                <div class="text-purple-600 font-medium">{{ getStatValue(philosopher, 'rhetoric') }}</div>
                <div class="text-gray-500">RET</div>
              </div>
              <div class="text-center">
                <div class="text-orange-600 font-medium">{{ getStatValue(philosopher, 'influence') }}</div>
                <div class="text-gray-500">WPL</div>
              </div>
              <div class="text-center">
                <div class="text-red-600 font-medium">{{ getStatValue(philosopher, 'originality') }}</div>
                <div class="text-gray-500">ORY</div>
              </div>
            </div>

            <!-- Duplicates (if owned) -->
            <div v-if="philosopher.userData && philosopher.userData.duplicates > 0" class="mt-2">
              <span class="text-xs text-gray-500">
                +{{ philosopher.userData.duplicates }} duplikatów
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else class="text-center py-12">
      <div class="mx-auto h-12 w-12 text-gray-400 mb-4">
        🎭
      </div>
      <h3 class="text-lg font-medium text-gray-900 mb-2">
        {{ getEmptyStateTitle() }}
      </h3>
      <p class="text-gray-500 mb-4">
        {{ getEmptyStateMessage() }}
      </p>
      <router-link v-if="activeTab !== 'owned'" to="/gacha" class="btn-primary">
        Odwiedź Wyrocznię
      </router-link>
    </div>

    <!-- Philosopher Detail Modal -->
    <div v-if="selectedPhilosopher" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50" @click="closeModal">
      <div class="bg-white rounded-lg max-w-2xl w-full max-h-screen overflow-y-auto" @click.stop>
        <div class="p-6">
          <!-- Modal Header -->
          <div class="flex justify-between items-start mb-6">
            <div>
              <h2 class="text-2xl font-bold text-gray-900">{{ selectedPhilosopher.name }}</h2>
              <p class="text-gray-600">{{ selectedPhilosopher.era }} • {{ selectedPhilosopher.school }}</p>
            </div>
            <button @click="closeModal" class="text-gray-400 hover:text-gray-600">
              <span class="sr-only">Zamknij</span>
              ✕
            </button>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Image and Basic Info -->
            <div>
              <img :src="selectedPhilosopher.imageUrl"
                   :alt="selectedPhilosopher.name"
                   class="w-full aspect-square object-cover rounded-lg mb-4"
                   @error="handleImageError" />

              <!-- Rarity and Ownership -->
              <div class="flex justify-between items-center mb-4">
                <span :class="[
                  'px-3 py-1 rounded-full text-sm font-bold',
                  getRarityBadgeClass(selectedPhilosopher.rarity)
                ]">
                  {{ selectedPhilosopher.rarity }}
                </span>
                <span v-if="selectedPhilosopher.isOwned" class="bg-green-500 text-white px-3 py-1 rounded-full text-sm font-bold">
                  ✓ Posiadane
                </span>
              </div>

              <!-- Description -->
              <p class="text-gray-700 text-sm leading-relaxed">
                {{ selectedPhilosopher.description }}
              </p>
            </div>

            <!-- Stats and Details -->
            <div>
              <!-- Level and Experience (if owned) -->
              <div v-if="selectedPhilosopher.userData" class="mb-6">
                <h3 class="font-semibold text-gray-900 mb-3">Twój Filozof</h3>
                <div class="bg-gray-50 p-4 rounded-lg">
                  <div class="flex justify-between items-center mb-2">
                    <span class="text-sm text-gray-600">Level</span>
                    <span class="font-bold">{{ selectedPhilosopher.userData.level }}</span>
                  </div>
                  <div class="flex justify-between items-center mb-2">
                    <span class="text-sm text-gray-600">Doświadczenie</span>
                    <span class="font-medium">{{ selectedPhilosopher.userData.experience }} XP</span>
                  </div>
                  <div v-if="selectedPhilosopher.userData.duplicates > 0" class="flex justify-between items-center">
                    <span class="text-sm text-gray-600">Duplikatów</span>
                    <span class="font-medium">+{{ selectedPhilosopher.userData.duplicates }}</span>
                  </div>
                </div>
              </div>

              <!-- Stats -->
              <div class="mb-6">
                <h3 class="font-semibold text-gray-900 mb-3">
                  {{ selectedPhilosopher.userData ? 'Current Stats' : 'Base Stats' }}
                </h3>
                <div class="space-y-3">
                  <div class="flex justify-between items-center">
                    <span class="text-sm text-gray-600">Wiedza</span>
                    <div class="flex items-center">
                      <div class="w-20 bg-gray-200 rounded-full h-2 mr-2">
                        <div class="bg-blue-600 h-2 rounded-full" :style="{ width: `${getStatValue(selectedPhilosopher, 'wisdom')}%` }"></div>
                      </div>
                      <span class="font-medium text-sm">{{ getStatValue(selectedPhilosopher, 'wisdom') }}</span>
                    </div>
                  </div>
                  <div class="flex justify-between items-center">
                    <span class="text-sm text-gray-600">Logika</span>
                    <div class="flex items-center">
                      <div class="w-20 bg-gray-200 rounded-full h-2 mr-2">
                        <div class="bg-green-600 h-2 rounded-full" :style="{ width: `${getStatValue(selectedPhilosopher, 'logic')}%` }"></div>
                      </div>
                      <span class="font-medium text-sm">{{ getStatValue(selectedPhilosopher, 'logic') }}</span>
                    </div>
                  </div>
                  <div class="flex justify-between items-center">
                    <span class="text-sm text-gray-600">Retoryka</span>
                    <div class="flex items-center">
                      <div class="w-20 bg-gray-200 rounded-full h-2 mr-2">
                        <div class="bg-purple-600 h-2 rounded-full" :style="{ width: `${getStatValue(selectedPhilosopher, 'rhetoric')}%` }"></div>
                      </div>
                      <span class="font-medium text-sm">{{ getStatValue(selectedPhilosopher, 'rhetoric') }}</span>
                    </div>
                  </div>
                  <div class="flex justify-between items-center">
                    <span class="text-sm text-gray-600">Wpływ</span>
                    <div class="flex items-center">
                      <div class="w-20 bg-gray-200 rounded-full h-2 mr-2">
                        <div class="bg-orange-600 h-2 rounded-full" :style="{ width: `${getStatValue(selectedPhilosopher, 'influence')}%` }"></div>
                      </div>
                      <span class="font-medium text-sm">{{ getStatValue(selectedPhilosopher, 'influence') }}</span>
                    </div>
                  </div>
                  <div class="flex justify-between items-center">
                    <span class="text-sm text-gray-600">Oryginalność</span>
                    <div class="flex items-center">
                      <div class="w-20 bg-gray-200 rounded-full h-2 mr-2">
                        <div class="bg-red-600 h-2 rounded-full" :style="{ width: `${getStatValue(selectedPhilosopher, 'originality')}%` }"></div>
                      </div>
                      <span class="font-medium text-sm">{{ getStatValue(selectedPhilosopher, 'originality') }}</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Quotes -->
              <div v-if="selectedPhilosopher.quotes && selectedPhilosopher.quotes.length > 0">
                <h3 class="font-semibold text-gray-900 mb-3">Znane Cytaty</h3>
                <div class="space-y-2">
                  <blockquote v-for="(quote, index) in selectedPhilosopher.quotes.slice(0, 2)"
                              :key="index"
                              class="text-sm italic text-gray-600 border-l-4 border-primary-200 pl-3">
                    "{{ quote }}"
                  </blockquote>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { usePhilosophersStore } from '@/stores/philosophers'
import type { Philosopher } from '@/services/philosopherApi'

const philosophersStore = usePhilosophersStore()

const activeTab = ref<'all' | 'owned' | 'missing'>('all')
const selectedRarity = ref<string | null>(null)
const selectedPhilosopher = ref<Philosopher | null>(null)

const availableRarities = computed(() => {
  const rarities = new Set(philosophersStore.philosophers.map(p => p.rarity))
  return Array.from(rarities).sort()
})

const missingPhilosophers = computed(() =>
  philosophersStore.philosophers.filter(p => !p.isOwned)
)

const legendaryCount = computed(() =>
  philosophersStore.ownedPhilosophers.filter(p => p.rarity === 'legendary').length
)

const epicCount = computed(() =>
  philosophersStore.ownedPhilosophers.filter(p => p.rarity === 'epic').length
)

const filteredPhilosophers = computed(() => {
  let philosophers = philosophersStore.philosophers

  // Filter by tab
  if (activeTab.value === 'owned') {
    philosophers = philosophers.filter(p => p.isOwned)
  } else if (activeTab.value === 'missing') {
    philosophers = philosophers.filter(p => !p.isOwned)
  }

  // Filter by rarity
  if (selectedRarity.value) {
    philosophers = philosophers.filter(p => p.rarity === selectedRarity.value)
  }

  // Sort by rarity and name
  return philosophers.sort((a, b) => {
    const rarityOrder = { legendary: 5, epic: 4, rare: 3, uncommon: 2, common: 1 }
    const aRarity = rarityOrder[a.rarity.toLowerCase() as keyof typeof rarityOrder] || 0
    const bRarity = rarityOrder[b.rarity.toLowerCase() as keyof typeof rarityOrder] || 0

    if (aRarity !== bRarity) return bRarity - aRarity
    return a.name.localeCompare(b.name)
  })
})

const getRarityColor = (rarity: string) => {
  const colors = {
    common: 'text-gray-600',
    uncommon: 'text-green-600',
    rare: 'text-blue-600',
    epic: 'text-purple-600',
    legendary: 'text-yellow-600'
  }
  return colors[rarity.toLowerCase() as keyof typeof colors] || 'text-gray-600'
}

const getRarityBadgeClass = (rarity: string) => {
  const classes = {
    common: 'bg-gray-100 text-gray-800',
    uncommon: 'bg-green-100 text-green-800',
    rare: 'bg-blue-100 text-blue-800',
    epic: 'bg-purple-100 text-purple-800',
    legendary: 'bg-yellow-100 text-yellow-800'
  }
  return classes[rarity.toLowerCase() as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const getStatValue = (philosopher: Philosopher, stat: string) => {
  if (philosopher.userData) {
    const currentStats = philosopher.userData.currentStats
    switch (stat) {
      case 'wisdom': return currentStats.currentWisdom
      case 'logic': return currentStats.currentLogic
      case 'rhetoric': return currentStats.currentRhetoric
      case 'influence': return currentStats.currentInfluence
      case 'originality': return currentStats.currentOriginality
    }
  }

  const baseStats = philosopher.baseStats
  switch (stat) {
    case 'wisdom': return baseStats.wisdom
    case 'logic': return baseStats.logic
    case 'rhetoric': return baseStats.rhetoric
    case 'influence': return baseStats.influence
    case 'originality': return baseStats.originality
  }

  return 0
}

const getEmptyStateTitle = () => {
  if (activeTab.value === 'owned') return 'No philosophers in your collection'
  if (activeTab.value === 'missing') return 'You have all philosophers!'
  return 'No philosophers found'
}

const getEmptyStateMessage = () => {
  if (activeTab.value === 'owned') return 'Start collecting legendary thinkers through lessons and gacha!'
  if (activeTab.value === 'missing') return 'Congratulations on completing your collection!'
  return 'Try adjusting your filters to see more philosophers.'
}

const selectPhilosopher = (philosopher: Philosopher) => {
  selectedPhilosopher.value = philosopher
}

const closeModal = () => {
  selectedPhilosopher.value = null
}

const handleImageError = (event: Event) => {
  const img = event.target as HTMLImageElement
  img.src = '/placeholder-philosopher.jpg' // You'll need to add a placeholder image
}

const loadData = async () => {
  try {
    await Promise.all([
      philosophersStore.fetchPhilosophers(),
      philosophersStore.fetchCollection()
    ])
  } catch (error) {
    console.error('Failed to load philosophers data:', error)
  }
}

onMounted(() => {
  loadData()
})
</script>
