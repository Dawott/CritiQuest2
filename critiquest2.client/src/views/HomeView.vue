<template>
  <div class="bg-gradient-to-br from-primary-50 to-indigo-100 min-h-screen">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div class="text-center">
        <h1 class="text-4xl font-bold text-gray-900 sm:text-5xl md:text-6xl">
          Welcome to <span class="text-primary-600">CritiQuest2</span>
        </h1>
        <p class="mt-3 max-w-md mx-auto text-base text-gray-500 sm:text-lg md:mt-5 md:text-xl md:max-w-3xl">
          Embark on an epic journey through the wisdom of the ages. Collect legendary philosophers, master ancient wisdom, and unlock the secrets of critical thinking.
        </p>
        <div class="mt-5 max-w-md mx-auto sm:flex sm:justify-center md:mt-8">
          <div class="rounded-md shadow">
            <router-link to="/register" class="btn-primary w-full sm:w-auto">
              Start Your Journey
            </router-link>
          </div>
          <div class="mt-3 rounded-md shadow sm:mt-0 sm:ml-3">
            <router-link to="/login" class="btn-secondary w-full sm:w-auto">
              Sign In
            </router-link>
          </div>
        </div>
      </div>

      <div class="mt-16 grid grid-cols-1 gap-8 sm:grid-cols-2 lg:grid-cols-3">
        <div class="card">
          <div class="card-body text-center">
            <div class="mx-auto h-12 w-12 text-primary-600 mb-4">
              📚
            </div>
            <h3 class="text-lg font-medium text-gray-900">Interactive Lessons</h3>
            <p class="mt-2 text-sm text-gray-500">
              Learn philosophy through engaging interactive lessons with scenarios, debates, and reflections.
            </p>
          </div>
        </div>

        <div class="card">
          <div class="card-body text-center">
            <div class="mx-auto h-12 w-12 text-primary-600 mb-4">
              🎭
            </div>
            <h3 class="text-lg font-medium text-gray-900">Collect Philosophers</h3>
            <p class="mt-2 text-sm text-gray-500">
              Unlock legendary thinkers from Socrates to Simone de Beauvoir through our unique gacha system.
            </p>
          </div>
        </div>

        <div class="card">
          <div class="card-body text-center">
            <div class="mx-auto h-12 w-12 text-primary-600 mb-4">
              🏆
            </div>
            <h3 class="text-lg font-medium text-gray-900">Earn Achievements</h3>
            <p class="mt-2 text-sm text-gray-500">
              Complete challenges, maintain streaks, and unlock achievements as you progress through your studies.
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
