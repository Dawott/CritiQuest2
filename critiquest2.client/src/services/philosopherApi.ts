import api from "./api"

export interface Philosopher {
  id: string
  name: string
  era: string
  school: string
  rarity: string
  baseStats: {
    wisdom: number
    logic: number
    rhetoric: number
    influence: number
    originality: number
  }
  description: string
  imageUrl: string
  quotes: string[]
  specialAbility: any
  isOwned: boolean
  userData?: {
    level: number
    experience: number
    duplicates: number
    currentStats: {
      currentWisdom: number
      currentLogic: number
      currentRhetoric: number
      currentInfluence: number
      currentOriginality: number
    }
    obtainedAt: string
  }
}

export interface OwnedPhilosopher {
  id: string
  level: number
  experience: number
  duplicates: number
  currentStats: {
    currentWisdom: number
    currentLogic: number
    currentRhetoric: number
    currentInfluence: number
    currentOriginality: number
  }
  obtainedAt: string
  philosopher: Philosopher
}

export interface GachaSummonResult {
  results: Array<{
    philosopher: {
      id: string
      name: string
      era: string
      school: string
      rarity: string
      imageUrl: string
    }
    isNew: boolean
    isDuplicate: boolean
  }>
  remainingTickets: number
}

export const philosopherApi = {
  getPhilosophers: (): Promise<Philosopher[]> =>
    api.get('/philosophers').then(res => res.data),

  getPhilosopher: (id: string): Promise<Philosopher> =>
    api.get(`/philosophers/${id}`).then(res => res.data),

  getCollection: (): Promise<OwnedPhilosopher[]> =>
    api.get('/philosophers/collection').then(res => res.data),

  performGacha: (ticketCount: number = 1): Promise<GachaSummonResult> =>
    api.post('/philosophers/gacha/summon', { ticketCount }).then(res => res.data)
}
