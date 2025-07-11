import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import HomeView from '@/views/HomeView.vue'
import LoginView from '@/views/LoginView.vue'
import RegisterView from '@/views/RegisterView.vue'
import DashboardView from '@/views/DashboardView.vue'
import QuizView from '@/views/QuizView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
      meta: { requiresAuth: false }
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { requiresAuth: false, guestOnly: true }
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView,
      meta: { requiresAuth: false, guestOnly: true }
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: DashboardView,
      meta: { requiresAuth: true }
    },
    {
      path: '/lessons',
      name: 'lessons',
      component: () => import('@/views/LessonsView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/lessons/:id',
      name: 'lesson-details',
      component: () => import('@/views/LessonDetailsView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/philosophers',
      name: 'philosophers',
      component: () => import('@/views/PhilosophersView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/gacha',
      name: 'gacha',
      component: () => import('@/views/GachaView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/quiz/:id',
      name: 'QuizView',
      component: QuizView,
      meta: {
        requiresAuth: true, 
        title: 'Quiz'
      }
    },
    {
      path: '/quiz/:id/results/:attemptId',
      name: 'QuizResults',
      component: QuizView, 
      meta: {
        requiresAuth: true,
        title: 'Wyniki Quizu'
      }
    },
    {
      path: '/interactive-scenarios/:sectionId',
      name: 'InteractiveScenario',
      component: () => import('@/views/InteractiveScenarioView.vue'),
      meta: {
        requiresAuth: true,
        title: 'Scenariusz Interaktywny'
      }
    },
    {
      path: '/admin',
      name: 'Admin',
      component: () => import('@/views/AdminPanel.vue')
    }
  ]
})

// Navigation guard
router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()

  // Initialize auth if not done yet
  if (authStore.token && !authStore.user) {
    try {
      await authStore.initializeAuth()
    } catch (err) {
      // Auth failed, continue to route handler
    }
  }

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (to.meta.guestOnly && authStore.isAuthenticated) {
    next('/dashboard')
  } else {
    next()
  }
})

export default router
