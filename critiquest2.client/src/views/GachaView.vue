<template>
  <div class="min-h-screen bg-gradient-to-br from-purple-900 via-blue-900 to-indigo-900 p-4">
    <div class="max-w-6xl mx-auto">
      <!-- Header -->
      <div class="text-center mb-8">
        <h1 class="text-4xl font-bold text-white mb-2">🎭 Wyrocznia</h1>
        <p class="text-blue-200">Odkryj wielkich myślicieli ludzkości</p>
      </div>

      <!-- Ticket Counter -->
      <div class="bg-white/10 backdrop-blur-sm rounded-lg p-6 mb-8 text-center">
        <div class="flex items-center justify-center gap-4">
          <div class="text-yellow-400 text-3xl">🎫</div>
          <div>
            <div class="text-2xl font-bold text-white">{{ profileStore.profile?.stats.gachaTickets || 0 }}</div>
            <div class="text-blue-200">Dostępne bilety</div>
          </div>
        </div>
      </div>

      <!-- Gacha Rates Info -->
      <div v-if="showRates" class="bg-white/10 backdrop-blur-sm rounded-lg p-6 mb-8">
        <h3 class="text-xl font-semibold text-white mb-4">Szanse na rzadkość</h3>
        <div class="grid grid-cols-2 md:grid-cols-5 gap-4">
          <div v-for="(rate, rarity) in gachaRates?.rarityRates" :key="rarity"
               class="text-center">
            <div :class="getRarityColor(rarity)" class="text-lg font-semibold">
              {{ rarity }}
            </div>
            <div class="text-white">{{ rate.toFixed(1) }}%</div>
          </div>
        </div>
      </div>

      <!-- Summon Controls -->
      <div class="bg-white/10 backdrop-blur-sm rounded-lg p-6 mb-8">
        <div class="flex flex-col sm:flex-row gap-4 items-center justify-center">
          <div class="flex gap-2">
            <button v-for="count in [1, 3, 5, 10]"
                    :key="count"
                    @click="selectedTicketCount = count"
                    :class="[
                'px-4 py-2 rounded-lg font-semibold transition-all',
                selectedTicketCount === count
                  ? 'bg-purple-600 text-white'
                  : 'bg-white/20 text-white hover:bg-white/30'
              ]"
                    :disabled="(profileStore.profile?.stats.gachaTickets || 0) < count">
              {{ count }}x
            </button>
          </div>

          <button @click="performSummon"
                  :disabled="loading || (profileStore.profile?.stats.gachaTickets || 0) < selectedTicketCount"
                  class="px-8 py-3 bg-gradient-to-r from-purple-600 to-pink-600 text-white font-bold rounded-lg
                   hover:from-purple-700 hover:to-pink-700 disabled:opacity-50 disabled:cursor-not-allowed
                   transform transition-all hover:scale-105 active:scale-95">
            <span v-if="loading" class="flex items-center gap-2">
              <div class="animate-spin w-4 h-4 border-2 border-white/30 border-t-white rounded-full"></div>
              Przyzywanie...
            </span>
            <span v-else>Przyzwij Filozofa!</span>
          </button>
        </div>

        <div class="text-center mt-4">
          <button @click="showRates = !showRates"
                  class="text-blue-300 hover:text-white transition-colors">
            {{ showRates ? 'Ukryj' : 'Pokaż' }} szanse na poziom rzadkości
          </button>
        </div>
      </div>

      <!-- Results Display -->
      <div v-if="summonResults.length > 0" class="mb-8">
        <h2 class="text-2xl font-bold text-white text-center mb-6">Wyniki przyzwania</h2>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div v-for="(result, index) in summonResults"
               :key="index"
               class="bg-white/10 backdrop-blur-sm rounded-lg p-6 text-center transform transition-all
                   hover:scale-105 animate-fadeInUp"
               :style="{ animationDelay: `${index * 0.1}s` }">
            <!-- Philosopher Image -->
            <div class="relative mb-4">
              <img :src="result.philosopher.imageUrl || '/placeholder-philosopher.jpg'"
                   :alt="result.philosopher.name"
                   class="w-24 h-24 mx-auto rounded-full border-4"
                   :class="getRarityBorderClass(result.philosopher.rarity)"
                   @error="handleImageError" />

              <!-- New/Duplicate Badge -->
              <div :class="[
                  'absolute -top-2 -right-2 px-2 py-1 text-xs font-bold rounded-full',
                  result.isNew
                    ? 'bg-green-500 text-white'
                    : 'bg-yellow-500 text-black'
                ]">
                {{ result.isNew ? 'NOWY!' : 'DUPLIKAT' }}
              </div>
            </div>

            <!-- Philosopher Info -->
            <h3 class="text-lg font-bold text-white mb-2">{{ result.philosopher.name }}</h3>
            <div :class="getRarityColor(result.philosopher.rarity)" class="text-sm font-semibold mb-2">
              {{ result.philosopher.rarity }} • {{ result.philosopher.era }}
            </div>
            <div class="text-blue-200 text-sm mb-4">{{ result.philosopher.school }}</div>

            <!-- Experience Gained -->
            <div v-if="result.experienceGained > 0" class="bg-purple-600/30 rounded-lg p-3">
              <div class="text-yellow-400 font-semibold">+{{ result.experienceGained }} EXP</div>
              <div class="text-white text-sm">Poziom {{ result.newLevel }}</div>
            </div>
            <div v-else class="bg-green-600/30 rounded-lg p-3">
              <div class="text-green-300 font-semibold">Dodano do Gimnazjonu!</div>
            </div>
          </div>
        </div>

        <!-- Summary -->
        <div v-if="lastSummonResponse" class="text-center mt-6">
          <div class="bg-white/10 backdrop-blur-sm rounded-lg p-4 inline-block">
            <div class="text-white">
              <span class="font-semibold">Pozostałe bilety:</span> {{ lastSummonResponse.remainingTickets }}
            </div>
            <div v-if="lastSummonResponse.totalExperienceGained > 0" class="text-yellow-400">
              <span class="font-semibold">Całkowite doświadczenie:</span> +{{ lastSummonResponse.totalExperienceGained }}
            </div>
          </div>
        </div>

        <div class="text-center mt-4">
          <button @click="clearResults"
                  class="px-4 py-2 bg-white/20 text-white rounded-lg hover:bg-white/30 transition-colors">
            Wyczyść wyniki
          </button>
        </div>
      </div>

      <!-- Collection Quick View -->
      <div class="bg-white/10 backdrop-blur-sm rounded-lg p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-xl font-semibold text-white">Twoja kolekcja</h3>
          <router-link to="/philosophers"
                       class="text-blue-300 hover:text-white transition-colors">
            Zobacz wszystkie →
          </router-link>
        </div>

        <div v-if="philosophersStore.loading" class="text-center py-8">
          <div class="animate-spin w-8 h-8 border-2 border-blue-300/30 border-t-blue-300 rounded-full mx-auto mb-4"></div>
          <div class="text-blue-200">Ładowanie kolekcji...</div>
        </div>

        <div v-else-if="philosophersStore.ownedPhilosophers.length === 0" class="text-center py-8">
          <div class="text-6xl mb-4">📚</div>
          <div class="text-white font-semibold mb-2">Kolekcja jest pusta</div>
          <div class="text-blue-200">Odwiedź Wyrocznię, aby zdobyć pierwszego filozofa!</div>
        </div>

        <div v-else class="grid grid-cols-3 sm:grid-cols-6 md:grid-cols-8 gap-4">
          <div v-for="philosopher in philosophersStore.ownedPhilosophers.slice(0, 16)"
               :key="philosopher.id"
               class="relative group cursor-pointer"
               @click="$router.push('/philosophers')">
            <img :src="philosopher.imageUrl || '/placeholder-philosopher.jpg'"
                 :alt="philosopher.name"
                 class="w-full aspect-square rounded-lg border-2 transition-transform group-hover:scale-105"
                 :class="getRarityBorderClass(philosopher.rarity)"
                 @error="handleImageError" />
            <div class="absolute inset-x-0 bottom-0 bg-black/70 text-white text-xs p-1 rounded-b-lg">
              {{ philosopher.name }}
            </div>
          </div>

          <div v-if="philosophersStore.ownedPhilosophers.length > 16"
               class="flex items-center justify-center bg-white/20 rounded-lg aspect-square">
            <div class="text-white text-center">
              <div class="text-2xl font-bold">+{{ philosophersStore.ownedPhilosophers.length - 16 }}</div>
              <div class="text-xs">więcej</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { ref, onMounted, computed } from 'vue'
  import { usePhilosophersStore } from '@/stores/philosophers'
  import { useAuthStore } from '@/stores/auth'
  import { useProfileStore } from '@/stores/profile'
  import { gachaApi, type GachaSummonResponse, type GachaRatesResponse } from '@/services/gachaApi'

  const philosophersStore = usePhilosophersStore()
  const authStore = useAuthStore()
  const profileStore = useProfileStore()

  const loading = ref(false)
  const selectedTicketCount = ref(1)
  const showRates = ref(false)
  const summonResults = ref<any[]>([])
  const lastSummonResponse = ref<GachaSummonResponse | null>(null)
  const gachaRates = ref<GachaRatesResponse | null>(null)

  const performSummon = async () => {
    if (loading.value) return

    loading.value = true
    summonResults.value = []

    try {
      const response = await gachaApi.performSummon(selectedTicketCount.value)
      lastSummonResponse.value = response
      summonResults.value = response.results

      // Refresh user data and collection
      await Promise.all([
        profileStore.fetchProfile(),
        philosophersStore.fetchCollection(),
        philosophersStore.fetchPhilosophers()
      ])

    } catch (error: any) {
      console.error('Gacha summon failed:', error)
      // You might want to show a toast notification here
    } finally {
      loading.value = false
    }
  }

  const clearResults = () => {
    summonResults.value = []
    lastSummonResponse.value = null
  }

  const getRarityColor = (rarity: string) => {
    const colors = {
      Common: 'text-gray-400',
      Uncommon: 'text-green-400',
      Rare: 'text-blue-400',
      Epic: 'text-purple-400',
      Legendary: 'text-yellow-400'
    }
    return colors[rarity as keyof typeof colors] || 'text-gray-400'
  }

  const getRarityBorderClass = (rarity: string) => {
    const classes = {
      Common: 'border-gray-400',
      Uncommon: 'border-green-400',
      Rare: 'border-blue-400',
      Epic: 'border-purple-400',
      Legendary: 'border-yellow-400'
    }
    return classes[rarity as keyof typeof classes] || 'border-gray-400'
  }

  const handleImageError = (event: Event) => {
    const img = event.target as HTMLImageElement
    img.src = '/placeholder-philosopher.jpg'
  }

  const loadData = async () => {
    try {
      const [rates] = await Promise.all([
        gachaApi.getGachaRates(),
        philosophersStore.fetchPhilosophers(),
        philosophersStore.fetchCollection(),
        profileStore.fetchProfile(),
        profileStore.fetchStats()
      ])
      gachaRates.value = rates
    } catch (error) {
      console.error('Failed to load gacha data:', error)
    }
  }

  onMounted(() => {
    loadData()
  })
</script>

<style scoped>
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

  .animate-fadeInUp {
    animation: fadeInUp 0.5s ease-out;
  }
</style>
