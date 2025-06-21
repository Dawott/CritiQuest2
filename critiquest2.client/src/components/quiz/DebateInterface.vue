<template>
  <div class="debate-interface">
    <!-- Debate Context -->
    <div class="bg-gradient-to-r from-purple-50 to-blue-50 rounded-lg p-6 mb-6">
      <h3 class="text-lg font-bold text-gray-800 mb-3">{{ debateConfig.title }}</h3>
      <p class="text-gray-700 mb-4">{{ debateConfig.context }}</p>
      <div class="bg-white rounded-md p-4">
        <p class="font-medium text-gray-800">{{ debateConfig.question }}</p>
      </div>
    </div>

    <!-- Schools Involved -->
    <div class="mb-6">
      <h4 class="font-medium text-gray-800 mb-3">Szkoły filozoficzne w debacie:</h4>
      <div class="flex flex-wrap gap-2">
        <span v-for="school in debateConfig.schools_involved"
              :key="school"
              class="bg-purple-100 text-purple-800 px-3 py-1 rounded-full text-sm">
          {{ formatSchoolName(school) }}
        </span>
      </div>
    </div>

    <!-- Debate Rounds -->
    <div v-if="!isCompleted" class="space-y-6">
      <div class="text-center">
        <span class="bg-gray-100 px-4 py-2 rounded-full text-sm font-medium">
          Runda {{ currentRound }} z {{ debateConfig.max_rounds }}
        </span>
      </div>

      <!-- Current Round -->
      <div class="bg-white border-2 border-dashed border-gray-300 rounded-lg p-6">
        <h4 class="font-medium text-gray-800 mb-4">Wybierz swój argument:</h4>

        <div class="space-y-3">
          <div v-for="(argument, index) in availableArguments"
               :key="argument.id"
               class="argument-card"
               :class="{
              'selected': selectedArgument?.id === argument.id,
              'strong': getArgumentStrength(argument) === 'strong',
              'weak': getArgumentStrength(argument) === 'weak'
            }"
               @click="selectArgument(argument)">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <p class="text-gray-800 mb-2">{{ argument.text }}</p>
                <p class="text-sm text-gray-600">
                  <span class="font-medium">Podstawa:</span> {{ argument.philosophical_basis }}
                </p>
              </div>
              <div class="ml-4 flex flex-col items-end">
                <div class="conviction-badge" :class="getConvictionClass(argument.conviction_power)">
                  {{ argument.conviction_power }}/10
                </div>
                <div v-if="getArgumentStrength(argument)" class="strength-indicator mt-1">
                  {{ getArgumentStrength(argument) === 'strong' ? '💪' : '⚠️' }}
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="mt-6 flex justify-center">
          <button @click="makeArgument"
                  :disabled="!selectedArgument"
                  class="btn-primary"
                  :class="{ 'opacity-50 cursor-not-allowed': !selectedArgument }">
            Przedstaw Argument
          </button>
        </div>
      </div>

      <!-- Opponent's Response (if any) -->
      <div v-if="opponentResponse" class="bg-red-50 border border-red-200 rounded-lg p-6">
        <h4 class="font-medium text-red-800 mb-2">Odpowiedź przeciwnika:</h4>
        <p class="text-red-700">{{ opponentResponse }}</p>
      </div>
    </div>

    <!-- Debate Results -->
    <div v-else class="bg-gradient-to-r from-green-50 to-blue-50 rounded-lg p-6">
      <h3 class="text-lg font-bold mb-4">Wyniki Debaty</h3>

      <div class="grid grid-cols-2 gap-4 mb-4">
        <div class="text-center">
          <div class="text-2xl font-bold" :class="finalResult.winner === 'user' ? 'text-green-600' : 'text-red-600'">
            {{ finalResult.winner === 'user' ? 'Zwycięstwo!' : 'Porażka' }}
          </div>
          <p class="text-sm text-gray-600">Ostateczny wynik</p>
        </div>
        <div class="text-center">
          <div class="text-2xl font-bold text-blue-600">{{ finalResult.conviction_score }}/100</div>
          <p class="text-sm text-gray-600">Siła przekonania</p>
        </div>
      </div>

      <!-- Learning Insights -->
      <div v-if="finalResult.learning_insights.length > 0" class="mb-4">
        <h4 class="font-medium text-gray-800 mb-2">Wnioski z debaty:</h4>
        <ul class="space-y-1">
          <li v-for="insight in finalResult.learning_insights" :key="insight" class="text-sm text-gray-700">
            • {{ insight }}
          </li>
        </ul>
      </div>

      <!-- Philosophical Growth -->
      <div v-if="finalResult.philosophical_growth.length > 0">
        <h4 class="font-medium text-gray-800 mb-2">Rozwój filozoficzny:</h4>
        <div class="space-y-2">
          <div v-for="growth in finalResult.philosophical_growth" :key="growth.concept" class="flex justify-between items-center">
            <span class="text-sm text-gray-700">{{ formatConcept(growth.concept) }}</span>
            <div class="w-20 bg-gray-200 rounded-full h-2">
              <div class="bg-blue-600 h-2 rounded-full" :style="{ width: `${growth.understanding}%` }"></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { ref, computed, onMounted } from 'vue'
  import type { Question } from '@/services/quizApi'

  interface DebateArgument {
    id: string
    text: string
    philosophical_basis: string
    strength_against: string[]
    weakness_against: string[]
    school_bonus: string[]
    conviction_power: number
    requires_philosopher?: string
  }

  interface DebateResult {
    winner: 'user' | 'opponent'
    totalRounds: number
    conviction_score: number
    learning_insights: string[]
    philosophical_growth: { concept: string; understanding: number }[]
  }

  interface Props {
    question: Question
    userAnswer: string[]
  }

  interface Emits {
    (e: 'answer-changed', answers: string[]): void
  }

  const props = defineProps<Props>()
  const emit = defineEmits<Emits>()

  // State
  const currentRound = ref(1)
  const selectedArgument = ref<DebateArgument | null>(null)
  const userArguments = ref<DebateArgument[]>([])
  const opponentResponse = ref<string | null>(null)
  const isCompleted = ref(false)
  const finalResult = ref<DebateResult | null>(null)

  // Mock arguments - in real implementation, these would come from your database
  const mockArguments: DebateArgument[] = [
    {
      id: 'arg-1',
      text: 'Cnota jest jedynym prawdziwym dobrem, wszystkie inne rzeczy są obojętne.',
      philosophical_basis: 'Stoicyzm',
      strength_against: ['hedonism'],
      weakness_against: ['pragmatism'],
      school_bonus: ['stoicism'],
      conviction_power: 8
    },
    {
      id: 'arg-2',
      text: 'Przyjemność i unikanie bólu to naturalne cele życia.',
      philosophical_basis: 'Hedonizm',
      strength_against: ['stoicism'],
      weakness_against: ['virtue-ethics'],
      school_bonus: ['hedonism'],
      conviction_power: 6
    },
    {
      id: 'arg-3',
      text: 'Powinniśmy dążyć do tego, co przynosi największe szczęście największej liczbie ludzi.',
      philosophical_basis: 'Utylitaryzm',
      strength_against: ['egoism'],
      weakness_against: ['individual-rights'],
      school_bonus: ['utilitarianism'],
      conviction_power: 7
    }
  ]

  const debateConfig = computed(() => props.question.debateConfig!)

  const availableArguments = computed(() => {
    // Filter arguments that haven't been used yet
    const usedIds = userArguments.value.map(arg => arg.id)
    return mockArguments.filter(arg => !usedIds.includes(arg.id))
  })

  const formatSchoolName = (school: string) => {
    return school.replace(/-/g, ' ').replace(/\b\w/g, l => l.toUpperCase())
  }

  const formatConcept = (concept: string) => {
    return concept.replace(/-/g, ' ').replace(/\b\w/g, l => l.toUpperCase())
  }

  const getArgumentStrength = (argument: DebateArgument) => {
    const currentSchools = debateConfig.value.schools_involved
    const hasStrength = argument.strength_against.some(school => currentSchools.includes(school))
    const hasWeakness = argument.weakness_against.some(school => currentSchools.includes(school))

    if (hasStrength && !hasWeakness) return 'strong'
    if (hasWeakness && !hasStrength) return 'weak'
    return null
  }

  const getConvictionClass = (power: number) => {
    if (power >= 8) return 'conviction-high'
    if (power >= 6) return 'conviction-medium'
    return 'conviction-low'
  }

  const selectArgument = (argument: DebateArgument) => {
    selectedArgument.value = argument
  }

  const makeArgument = () => {
    if (!selectedArgument.value) return

    userArguments.value.push(selectedArgument.value)

    // Simulate opponent response
    setTimeout(() => {
      opponentResponse.value = generateOpponentResponse(selectedArgument.value!)
      selectedArgument.value = null

      if (currentRound.value >= debateConfig.value.max_rounds) {
        completeDebate()
      } else {
        currentRound.value++
        // Clear opponent response for next round
        setTimeout(() => {
          opponentResponse.value = null
        }, 3000)
      }
    }, 1000)
  }

  const generateOpponentResponse = (userArgument: DebateArgument) => {
    const responses = [
      `Twój argument o ${userArgument.philosophical_basis.toLowerCase()} pomija ważne aspekty...`,
      `Choć ${userArgument.philosophical_basis} ma swoje zalety, nie uwzględnia...`,
      `Twoje stanowisko oparte na ${userArgument.philosophical_basis.toLowerCase()} można skrytykować...`,
    ]
    return responses[Math.floor(Math.random() * responses.length)]
  }

  const completeDebate = () => {
    isCompleted.value = true

    // Calculate results
    const totalConviction = userArguments.value.reduce((sum, arg) => sum + arg.conviction_power, 0)
    const averageConviction = totalConviction / userArguments.value.length
    const convictionScore = Math.round(averageConviction * 10)

    finalResult.value = {
      winner: convictionScore >= 70 ? 'user' : 'opponent',
      totalRounds: currentRound.value,
      conviction_score: convictionScore,
      learning_insights: [
        'Rozważyłeś różne perspektywy filozoficzne',
        'Nauczyłeś się argumentować swoją pozycję',
        'Poznałeś kontrastujące podejścia do etyki'
      ],
      philosophical_growth: [
        { concept: 'logical-reasoning', understanding: Math.min(100, convictionScore + 20) },
        { concept: 'ethical-thinking', understanding: Math.min(100, convictionScore + 15) }
      ]
    }

    // Emit the result
    emit('answer-changed', [JSON.stringify(finalResult.value)])
  }

  onMounted(() => {
    // If we have an existing answer, restore the debate state
    if (props.userAnswer.length > 0) {
      try {
        const savedResult = JSON.parse(props.userAnswer[0])
        if (savedResult.winner) {
          finalResult.value = savedResult
          isCompleted.value = true
        }
      } catch (e) {
        // Ignore parsing errors
      }
    }
  })
</script>

<style scoped>
  .argument-card {
    @apply border border-gray-200 rounded-lg p-4 cursor-pointer transition-all duration-200 hover:border-purple-300 hover:bg-purple-50;
  }

    .argument-card.selected {
      @apply border-purple-500 bg-purple-50;
    }

    .argument-card.strong {
      @apply border-green-300 bg-green-50;
    }

    .argument-card.weak {
      @apply border-red-300 bg-red-50;
    }

  .conviction-badge {
    @apply px-2 py-1 rounded-full text-xs font-bold;
  }

  .conviction-high {
    @apply bg-green-100 text-green-800;
  }

  .conviction-medium {
    @apply bg-yellow-100 text-yellow-800;
  }

  .conviction-low {
    @apply bg-red-100 text-red-800;
  }

  .btn-primary {
    @apply bg-purple-600 hover:bg-purple-700 text-white font-semibold py-2 px-6 rounded-lg transition-colors;
  }
</style>
