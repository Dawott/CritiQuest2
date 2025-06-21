<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900">Philosopher Gacha</h1>
      <p class="mt-2 text-gray-600">Use tickets to unlock legendary philosophers</p>
    </div>

    <div class="max-w-md mx-auto">
      <div class="card">
        <div class="card-body text-center">
          <div class="mx-auto h-24 w-24 text-6xl mb-4">
            🎲
          </div>
          <h3 class="text-lg font-medium text-gray-900 mb-2">Philosophy Summon</h3>
          <p class="text-sm text-gray-600 mb-4">
            Tickets available: <span class="font-medium">0</span>
          </p>
          <button class="btn-primary w-full mb-4" disabled>
            Summon Philosopher (1 ticket)
          </button>
          <p class="text-xs text-gray-500">
            Earn tickets by completing lessons and achievements
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
