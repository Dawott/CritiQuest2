import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { philosopherApi, type Philosopher, type OwnedPhilosopher, type GachaSummonResult } from '@/services/philosopherApi'

export const usePhilosophersStore = defineStore('philosophers', () => {
  const philosophers = ref<Philosopher[]>([])
  const collection = ref<OwnedPhilosopher[]>([])
  const currentPhilosopher = ref<Philosopher | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const philosophersByRarity = computed(() => {
    const grouped: Record<string, Philosopher[]> = {}
    philosophers.value.forEach(philosopher => {
      if (!grouped[philosopher.rarity]) {
        grouped[philosopher.rarity] = []
      }
      grouped[philosopher.rarity].push(philosopher)
    })
    return grouped
  })

  const ownedPhilosophers = computed(() =>
    philosophers.value.filter(p => p.isOwned)
  )

  const collectionStats = computed(() => {
    const total = philosophers.value.length
    const owned = ownedPhilosophers.value.length
    const byRarity: Record<string, { total: number; owned: number }> = {}

    philosophers.value.forEach(p => {
      if (!byRarity[p.rarity]) {
        byRarity[p.rarity] = { total: 0, owned: 0 }
      }
      byRarity[p.rarity].total++
      if (p.isOwned) {
        byRarity[p.rarity].owned++
      }
    })

    return {
      total,
      owned,
      percentage: total > 0 ? Math.round((owned / total) * 100) : 0,
      byRarity
    }
  })

  const fetchPhilosophers = async () => {
    loading.value = true
    error.value = null

    try {
      const data = await philosopherApi.getPhilosophers()
      philosophers.value = data
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch philosophers'
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchPhilosopher = async (id: string) => {
    loading.value = true
    error.value = null

    try {
      const data = await philosopherApi.getPhilosopher(id)
      currentPhilosopher.value = data
      return data
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch philosopher'
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchCollection = async () => {
    loading.value = true
    error.value = null

    try {
      const data = await philosopherApi.getCollection()
      collection.value = data
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch collection'
      throw err
    } finally {
      loading.value = false
    }
  }

  const performGacha = async (ticketCount: number = 1): Promise<GachaSummonResult> => {
    loading.value = true
    error.value = null

    try {
      const result = await philosopherApi.performGacha(ticketCount)

      // Refresh philosophers and collection to update ownership status
      await Promise.all([
        fetchPhilosophers(),
        fetchCollection()
      ])

      return result
    } catch (err: any) {
      error.value = err.message || 'Failed to perform gacha summon'
      throw err
    } finally {
      loading.value = false
    }
  }

  const clearCurrentPhilosopher = () => {
    currentPhilosopher.value = null
  }

  return {
    philosophers,
    collection,
    currentPhilosopher,
    loading,
    error,
    philosophersByRarity,
    ownedPhilosophers,
    collectionStats,
    fetchPhilosophers,
    fetchPhilosopher,
    fetchCollection,
    performGacha,
    clearCurrentPhilosopher
  }
})
