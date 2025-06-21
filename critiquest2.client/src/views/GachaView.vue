<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900">Wyrocznia</h1>
      <p class="mt-2 text-gray-600">Użyj zdobytych biletów by odwiedzić Wyrocznię</p>
    </div>

    <!-- Tickets and Stats -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
      <div class="card">
        <div class="card-body text-center">
          <div class="text-4xl font-bold text-primary-600 mb-2">
            {{ profileStore.profile?.stats.gachaTickets || 0 }}
          </div>
          <div class="text-sm text-gray-600">Dostępne Bilety</div>
        </div>
      </div>

      <div class="card">
        <div class="card-body text-center">
          <div class="text-4xl font-bold text-green-600 mb-2">
            {{ philosophersStore.ownedPhilosophers.length }}
          </div>
          <div class="text-sm text-gray-600">Posiadani Filozofowie</div>
        </div>
      </div>

      <div class="card">
        <div class="card-body text-center">
          <div class="text-4xl font-bold text-purple-600 mb-2">
            {{ collectionPercentage }}%
          </div>
          <div class="text-sm text-gray-600">Kompletność</div>
        </div>
      </div>
    </div>

    <!-- Gacha Rates -->
    <div class="card mb-8">
      <div class="card-body">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Szanse przyzwania</h3>
        <div class="grid grid-cols-2 md:grid-cols-5 gap-4">
          <div class="text-center">
            <div class="text-2xl font-bold text-gray-600 mb-1">50%</div>
            <div class="text-sm text-gray-500">Zwykły</div>
          </div>
          <div class="text-center">
            <div class="text-2xl font-bold text-green-600 mb-1">25%</div>
            <div class="text-sm text-gray-500">Niezwykły</div>
          </div>
          <div class="text-center">
            <div class="text-2xl font-bold text-blue-600 mb-1">15%</div>
            <div class="text-sm text-gray-500">Rzadki</div>
          </div>
          <div class="text-center">
            <div class="text-2xl font-bold text-purple-600 mb-1">8%</div>
            <div class="text-sm text-gray-500">Epicki</div>
          </div>
          <div class="text-center">
            <div class="text-2xl font-bold text-yellow-600 mb-1">2%</div>
            <div class="text-sm text-gray-500">Legendarny</div>
          </div>
        </div>
      </div>
    </div>

    <!-- Gacha Interface -->
    <div class="max-w-2xl mx-auto">
      <div class="card">
        <div class="card-body text-center">
          <div class="mx-auto h-32 w-32 text-8xl mb-6">
            <div class="animate-bounce-gentle">🎲</div>
          </div>

          <h3 class="text-xl font-medium text-gray-900 mb-2">Zapytaj Wyrocznię</h3>
          <p class="text-gray-600 mb-6">
            Przyzwij wielkiego filozofa by pomógł w Twej wędrówce
          </p>

          <!-- Summon Options -->
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-6">
            <button @click="performSummon(1)"
                    :disabled="!canSummon(1) || philosophersStore.loading"
                    :class="[
                'btn-primary py-4',
                (!canSummon(1) || philosophersStore.loading) && 'opacity-50 cursor-not-allowed'
              ]">
              <div class="text-lg font-bold">Pojedyncze Przyzwanie</div>
              <div class="text-sm opacity-75">1 Bilet</div>
            </button>

            <button @click="performSummon(10)"
                    :disabled="!canSummon(10) || philosophersStore.loading"
                    :class="[
                'btn-primary py-4 bg-purple-600 hover:bg-purple-700',
                (!canSummon(10) || philosophersStore.loading) && 'opacity-50 cursor-not-allowed'
              ]">
              <div class="text-lg font-bold">10x Przyzwań</div>
              <div class="text-sm opacity-75">10 Biletów</div>
            </button>
          </div>

          <!-- Loading State -->
          <div v-if="philosophersStore.loading" class="py-8">
            <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600 mx-auto mb-4"></div>
            <p class="text-gray-600">Wzywam Filozofa...</p>
          </div>

          <!-- Error State -->
          <div v-else-if="philosophersStore.error" class="py-4">
            <div class="text-red-600 mb-4">{{ philosophersStore.error }}</div>
          </div>

          <!-- Instructions -->
          <div v-else class="text-sm text-gray-500">
            <p>Zdobądź bilety przez kończenie lekcji i zdobywanie osiągnięć</p>
            <p class="mt-1">Duplikaty dają bonusowe doświadczenie</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Summons History -->
    <div v-if="recentSummons.length > 0" class="mt-8">
      <h3 class="text-lg font-medium text-gray-900 mb-4">Ostatnie przyzwania</h3>
      <div class="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
        <div v-for="(summon, index) in recentSummons.slice(0, 12)"
             :key="index"
             class="card p-3 text-center">
          <div class="aspect-square mb-2 relative">
            <img :src="summon.philosopher.imageUrl"
                 :alt="summon.philosopher.name"
                 class="w-full h-full object-cover rounded"
                 @error="handleImageError" />
            <div class="absolute top-1 right-1">
              <span :class="[
                'px-1 py-0.5 rounded text-xs font-bold',
                getRarityBadgeClass(summon.philosopher.rarity)
              ]">
                {{ summon.philosopher.rarity.substring(0, 1).toUpperCase() }}
              </span>
            </div>
            <div v-if="summon.isNew" class="absolute top-1 left-1">
              <span class="bg-green-500 text-white px-1 py-0.5 rounded text-xs font-bold">
                NOWE
              </span>
            </div>
          </div>
          <div class="text-xs font-medium text-gray-900 truncate">
            {{ summon.philosopher.name }}
          </div>
        </div>
      </div>
    </div>

    <!-- Summon Results Modal -->
    <div v-if="showResults && lastSummonResults" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50" @click="closeResults">
      <div class="bg-white rounded-lg max-w-4xl w-full max-h-screen overflow-y-auto" @click.stop>
        <div class="p-6">
          <!-- Modal Header -->
          <div class="flex justify-between items-center mb-6">
            <h2 class="text-2xl font-bold text-gray-900">Wynik Przyzwania</h2>
            <button @click="closeResults" class="text-gray-400 hover:text-gray-600">
              <span class="sr-only">Zamknij</span>
              ✕
            </button>
          </div>

          <!-- Results Grid -->
          <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 mb-6">
            <div v-for="(result, index) in lastSummonResults.results"
                 :key="index"
                 class="card p-4 text-center transform transition-all duration-500"
                 :class="{
                'ring-2 ring-yellow-400 animate-pulse': result.philosopher.rarity === 'legendary',
                'ring-2 ring-purple-400': result.philosopher.rarity === 'epic',
                'ring-2 ring-green-400': result.isNew
              }">
              <div class="aspect-square mb-3 relative">
                <img :src="result.philosopher.imageUrl"
                     :alt="result.philosopher.name"
                     class="w-full h-full object-cover rounded-lg"
                     @error="handleImageError" />
                <!-- Rarity Badge -->
                <div class="absolute top-2 right-2">
                  <span :class="[
                    'px-2 py-1 rounded-full text-xs font-bold',
                    getRarityBadgeClass(result.philosopher.rarity)
                  ]">
                    {{ result.philosopher.rarity }}
                  </span>
                </div>
                <!-- New/Duplicate Badge -->
                <div class="absolute top-2 left-2">
                  <span :class="[
                    'px-2 py-1 rounded-full text-xs font-bold',
                    result.isNew ? 'bg-green-500 text-white' : 'bg-blue-500 text-white'
                  ]">
                    {{ result.isNew ? 'NEW!' : 'DUP' }}
                  </span>
                </div>
              </div>

              <h3 class="font-semibold text-gray-900 mb-1">{{ result.philosopher.name }}</h3>
              <p class="text-sm text-gray-600 mb-2">{{ result.philosopher.era }}</p>
              <p class="text-xs text-gray-500">{{ result.philosopher.school }}</p>

              <!-- Special Effects for Rare Pulls -->
              <div v-if="result.philosopher.rarity === 'legendary'" class="mt-2">
                <span class="text-yellow-600 text-xs font-bold animate-pulse">⭐ LEGENDA! ⭐</span>
              </div>
              <div v-else-if="result.philosopher.rarity === 'epic'" class="mt-2">
                <span class="text-purple-600 text-xs font-bold">✨ EPICKA! ✨</span>
              </div>
            </div>
          </div>

          <!-- Summary -->
          <div class="bg-gray-50 p-4 rounded-lg mb-4">
            <div class="grid grid-cols-2 md:grid-cols-4 gap-4 text-center">
              <div>
                <div class="text-2xl font-bold text-gray-900">{{ lastSummonResults.results.length }}</div>
                <div class="text-sm text-gray-600">Wszystkich Przyzwań</div>
              </div>
              <div>
                <div class="text-2xl font-bold text-green-600">{{ newPhilosophersCount }}</div>
                <div class="text-sm text-gray-600">Nowi Filozofowie</div>
              </div>
              <div>
                <div class="text-2xl font-bold text-blue-600">{{ duplicatesCount }}</div>
                <div class="text-sm text-gray-600">Duplikaty</div>
              </div>
              <div>
                <div class="text-2xl font-bold text-primary-600">{{ lastSummonResults.remainingTickets }}</div>
                <div class="text-sm text-gray-600">Biletów Zostało</div>
              </div>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex justify-center space-x-4">
            <button @click="closeResults" class="btn-secondary">
              Close
            </button>
            <button v-if="lastSummonResults.remainingTickets > 0"
                    @click="closeResults(); performSummon(1)"
                    class="btn-primary">
              Summon Again
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { usePhilosophersStore } from '@/stores/philosophers'
import { useProfileStore } from '@/stores/profile'
import type { GachaSummonResult } from '@/services/philosopherApi'

const philosophersStore = usePhilosophersStore()
const profileStore = useProfileStore()

const showResults = ref(false)
const lastSummonResults = ref<GachaSummonResult | null>(null)
const recentSummons = ref<any[]>([])

const collectionPercentage = computed(() => {
  if (!philosophersStore.collectionStats) return 0
  return philosophersStore.collectionStats.percentage
})

const newPhilosophersCount = computed(() => {
  if (!lastSummonResults.value) return 0
  return lastSummonResults.value.results.filter(r => r.isNew).length
})

const duplicatesCount = computed(() => {
  if (!lastSummonResults.value) return 0
  return lastSummonResults.value.results.filter(r => r.isDuplicate).length
})

const canSummon = (ticketCount: number) => {
  const availableTickets = profileStore.profile?.stats.gachaTickets || 0
  return availableTickets >= ticketCount
}

const performSummon = async (ticketCount: number) => {
  if (!canSummon(ticketCount)) return

  try {
    const result = await philosophersStore.performGacha(ticketCount)
    lastSummonResults.value = result
    showResults.value = true

    // Add to recent summons history
    recentSummons.value.unshift(...result.results)

    // Update profile to reflect new ticket count
    await profileStore.fetchProfile()

  } catch (error) {
    console.error('Gacha summon failed:', error)
  }
}

const closeResults = () => {
  showResults.value = false
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

const handleImageError = (event: Event) => {
  const img = event.target as HTMLImageElement
  img.src = '/placeholder-philosopher.jpg'
}

const loadData = async () => {
  try {
    await Promise.all([
      philosophersStore.fetchPhilosophers(),
      philosophersStore.fetchCollection(),
      profileStore.fetchProfile()
    ])
  } catch (error) {
    console.error('Failed to load gacha data:', error)
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
  @keyframes bounceGentle {
    0%, 100% {
      transform: translateY(0);
    }

    50% {
      transform: translateY(-5px);
    }
  }

  .animate-bounce-gentle {
    animation: bounceGentle 2s infinite;
  }
</style>
