<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900">Your Philosophers</h1>
      <p class="mt-2 text-gray-600">Collect and manage your philosophical collection</p>
    </div>

    <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
      <!-- Empty state -->
      <div class="col-span-full">
        <div class="text-center py-12">
          <div class="mx-auto h-12 w-12 text-gray-400 mb-4">
            🎭
          </div>
          <h3 class="text-lg font-medium text-gray-900 mb-2">No philosophers yet</h3>
          <p class="text-gray-500 mb-4">Start collecting legendary thinkers through lessons and gacha!</p>
          <router-link to="/gacha" class="btn-primary">
            Try Gacha
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>
