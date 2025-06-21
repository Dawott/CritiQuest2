import api from "./api"

export interface GachaSummonRequest {
  ticketCount: number
}

export interface GachaSummonResponse {
  results: GachaSummonResult[]
  remainingTickets: number
  totalExperienceGained: number
}

export interface GachaSummonResult {
  philosopher: {
    id: string
    name: string
    era: string
    school: string
    rarity: string
    imageUrl: string
    description: string
  }
  isNew: boolean
  isDuplicate: boolean
  experienceGained: number
  newLevel: number
}

export interface GachaRatesResponse {
  rarityRates: Record<string, number>
  duplicateExperience: Record<string, number>
}

export interface GachaPreviewResponse {
  availableTickets: number
  totalPhilosophers: number
  ownedPhilosophers: number
  rarityBreakdown: Record<string, number>
  featuredPhilosophers: Array<{
    id: string
    name: string
    era: string
    school: string
    rarity: string
    imageUrl: string
    description: string
  }>
}

export const gachaApi = {
  performSummon: (ticketCount: number = 1): Promise<GachaSummonResponse> =>
    api.post('/gacha/summon', { ticketCount }).then(res => res.data),

  getGachaRates: (): Promise<GachaRatesResponse> =>
    api.get('/gacha/rates').then(res => res.data),

  getGachaPreview: (): Promise<GachaPreviewResponse> =>
    api.get('/gacha/preview').then(res => res.data)
}
