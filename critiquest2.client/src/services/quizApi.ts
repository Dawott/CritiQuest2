import api from "./api"

export interface Quiz {
  id: string
  lessonId: string
  title: string
  type: string
  timeLimit?: number
  passingScore: number
  philosopherBonus?: any
  questions: Question[]
  userAttempts: QuizAttempt[]
}

export interface Question {
  id: string
  text: string
  type: string
  options: string[]
  philosophicalContext: string
  points: number
  order: number
  debateConfig?: any
}

export interface QuizAttempt {
  id: string
  startedAt: string
  completedAt?: string
  score: number
  timeSpent: number
  passed: boolean
}

export interface QuizSubmission {
  attemptId: string
  timeSpent: number
  answers: Array<{
    questionId: string
    selectedAnswers: string[]
  }>
}

export interface QuizResult {
  attemptId: string
  score: number
  passed: boolean
  earnedPoints: number
  totalPoints: number
  timeSpent: number
  results: Array<{
    questionId: string
    question: string
    userAnswers: string[]
    correctAnswers: string[]
    isCorrect: boolean
    explanation: string
    philosophicalContext: string
    points: number
    maxPoints: number
  }>
}

export const quizApi = {
  getQuiz: (id: string): Promise<Quiz> =>
    api.get(`/quizzes/${id}`).then(res => res.data),

  startAttempt: (quizId: string): Promise<{ attemptId: string }> =>
    api.post(`/quizzes/${quizId}/start`).then(res => res.data),

  submitQuiz: (quizId: string, submission: QuizSubmission): Promise<QuizResult> =>
    api.post(`/quizzes/${quizId}/submit`, submission).then(res => res.data),

  getResults: (attemptId: string): Promise<QuizResult> =>
    api.get(`/quizzes/attempts/${attemptId}/results`).then(res => res.data)
}
