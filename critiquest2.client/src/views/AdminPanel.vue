<template>
  <div class="min-h-screen bg-gray-100 p-4">
    <div class="max-w-4xl mx-auto">
      <h1 class="text-3xl font-bold text-gray-800 mb-8">🔧 Panel Testowy</h1>
      <p class="text-gray-600 mb-8">Narzędzia dla deweloperów i testowania funkcjonalności</p>

      <!-- User Summary -->
      <div class="bg-white rounded-lg shadow-md p-6 mb-6">
        <h2 class="text-xl font-semibold mb-4">📊 Podsumowanie użytkownika</h2>

        <div v-if="loading.summary" class="text-center py-4">
          <div class="animate-spin w-6 h-6 border-2 border-blue-500 border-t-transparent rounded-full mx-auto"></div>
        </div>

        <div v-else-if="userSummary" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div class="bg-blue-50 rounded-lg p-4">
            <div class="text-2xl font-bold text-blue-600">{{ userSummary.gachaTickets }}</div>
            <div class="text-blue-800">Bilety Gacha</div>
          </div>

          <div class="bg-green-50 rounded-lg p-4">
            <div class="text-2xl font-bold text-green-600">{{ userSummary.ownedPhilosophers }}</div>
            <div class="text-green-800">Filozofowie w kolekcji</div>
          </div>

          <div class="bg-purple-50 rounded-lg p-4">
            <div class="text-2xl font-bold text-purple-600">{{ userSummary.totalPhilosophers }}</div>
            <div class="text-purple-800">Dostępni filozofowie</div>
          </div>

          <div class="bg-yellow-50 rounded-lg p-4">
            <div class="text-2xl font-bold text-yellow-600">{{ userSummary.collectionPercentage.toFixed(1) }}%</div>
            <div class="text-yellow-800">Kompletność kolekcji</div>
          </div>
        </div>

        <!-- Rarity Breakdown -->
        <div v-if="userSummary?.rarityBreakdown.length > 0" class="mt-6">
          <h3 class="text-lg font-medium mb-3">Podział według rzadkości:</h3>
          <div class="flex flex-wrap gap-2">
            <span v-for="rarity in userSummary.rarityBreakdown"
                  :key="rarity.rarity"
                  :class="getRarityBadgeClass(rarity.rarity)"
                  class="px-3 py-1 rounded-full text-sm font-medium">
              {{ rarity.rarity }}: {{ rarity.count }}
            </span>
          </div>
        </div>

        <button @click="loadUserSummary"
                :disabled="loading.summary"
                class="mt-4 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">
          Odśwież dane
        </button>
      </div>

      <!-- Add Gacha Tickets -->
      <div class="bg-white rounded-lg shadow-md p-6 mb-6">
        <h2 class="text-xl font-semibold mb-4">🎫 Dodaj bilety Gacha</h2>

        <div class="flex items-center gap-4 mb-4">
          <div class="flex-1">
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Liczba biletów do dodania
            </label>
            <input v-model.number="ticketAmount"
                   type="number"
                   min="1"
                   max="100"
                   class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                   placeholder="Wpisz liczbę biletów" />
          </div>

          <button @click="addTickets"
                  :disabled="loading.addTickets || !ticketAmount || ticketAmount < 1"
                  class="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 disabled:cursor-not-allowed">
            <span v-if="loading.addTickets">Dodawanie...</span>
            <span v-else>Dodaj bilety</span>
          </button>
        </div>

        <!-- Quick Add Buttons -->
        <div class="flex flex-wrap gap-2">
          <button v-for="amount in [1, 5, 10, 25, 50]"
                  :key="amount"
                  @click="quickAddTickets(amount)"
                  :disabled="loading.addTickets"
                  class="px-3 py-1 bg-gray-200 text-gray-700 rounded hover:bg-gray-300 disabled:opacity-50">
            +{{ amount }}
          </button>
        </div>
      </div>

      <!-- Reset Collection -->
      <div class="bg-white rounded-lg shadow-md p-6 mb-6">
        <h2 class="text-xl font-semibold mb-4">🗑️ Resetuj kolekcję</h2>
        <p class="text-gray-600 mb-4">
          Usuwa wszystkich filozofów z twojej kolekcji. Ta operacja jest nieodwracalna!
        </p>

        <div class="flex items-center gap-4">
          <label class="flex items-center">
            <input v-model="confirmReset"
                   type="checkbox"
                   class="mr-2" />
            <span class="text-sm text-gray-700">Potwierdzam, że chcę zresetować kolekcję</span>
          </label>

          <button @click="resetCollection"
                  :disabled="loading.resetCollection || !confirmReset"
                  class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 disabled:opacity-50 disabled:cursor-not-allowed">
            <span v-if="loading.resetCollection">Resetowanie...</span>
            <span v-else>Resetuj kolekcję</span>
          </button>
        </div>
      </div>

      <!-- Navigation -->
      <div class="bg-white rounded-lg shadow-md p-6">
        <h2 class="text-xl font-semibold mb-4">🧭 Nawigacja</h2>
        <div class="flex flex-wrap gap-4">
          <router-link to="/gacha"
                       class="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700">
            Gacha
          </router-link>
          <router-link to="/philosophers"
                       class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
            Kolekcja filozofów
          </router-link>
        </div>
      </div>

      <!-- Messages -->
      <div v-if="message" class="fixed bottom-4 right-4 max-w-sm">
        <div :class="[
            'p-4 rounded-lg shadow-lg',
            message.type === 'success' ? 'bg-green-500 text-white' : 'bg-red-500 text-white'
          ]">
          {{ message.text }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '@/services/api'

interface UserSummary {
  userId: string
  gachaTickets: number
  ownedPhilosophers: number
  totalPhilosophers: number
  collectionPercentage: number
  rarityBreakdown: Array<{ rarity: string, count: number }>
}

interface Message {
  text: string
  type: 'success' | 'error'
}

const userSummary = ref<UserSummary | null>(null)
const ticketAmount = ref<number>(5)
const confirmReset = ref(false)
const message = ref<Message | null>(null)

const loading = ref({
  summary: false,
  addTickets: false,
  resetCollection: false
})

const showMessage = (text: string, type: 'success' | 'error' = 'success') => {
  message.value = { text, type }
  setTimeout(() => {
    message.value = null
  }, 3000)
}

const loadUserSummary = async () => {
  loading.value.summary = true
  try {
    const response = await api.get('/admin/user-summary')
    userSummary.value = response.data
  } catch (error: any) {
    console.error('Failed to load user summary:', error)
    showMessage('Błąd podczas ładowania danych użytkownika', 'error')
  } finally {
    loading.value.summary = false
  }
}

const addTickets = async () => {
  if (!ticketAmount.value || ticketAmount.value < 1) return

  loading.value.addTickets = true
  try {
    const response = await api.post('/admin/add-tickets', { amount: ticketAmount.value })
    showMessage(response.data.message)
    ticketAmount.value = 5
    await loadUserSummary()
  } catch (error: any) {
    console.error('Failed to add tickets:', error)
    showMessage(error.response?.data?.message || 'Błąd podczas dodawania biletów', 'error')
  } finally {
    loading.value.addTickets = false
  }
}

const quickAddTickets = async (amount: number) => {
  ticketAmount.value = amount
  await addTickets()
}

const resetCollection = async () => {
  if (!confirmReset.value) return

  loading.value.resetCollection = true
  try {
    const response = await api.post('/admin/reset-collection')
    showMessage(response.data.message)
    confirmReset.value = false
    await loadUserSummary()
  } catch (error: any) {
    console.error('Failed to reset collection:', error)
    showMessage(error.response?.data?.message || 'Błąd podczas resetowania kolekcji', 'error')
  } finally {
    loading.value.resetCollection = false
  }
}

const getRarityBadgeClass = (rarity: string) => {
  const classes = {
    Common: 'bg-gray-200 text-gray-800',
    Uncommon: 'bg-green-200 text-green-800',
    Rare: 'bg-blue-200 text-blue-800',
    Epic: 'bg-purple-200 text-purple-800',
    Legendary: 'bg-yellow-200 text-yellow-800'
  }
  return classes[rarity as keyof typeof classes] || 'bg-gray-200 text-gray-800'
}

onMounted(() => {
  loadUserSummary()
})
</script>
