<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900">Philosophy Lessons</h1>
      <p class="mt-2 text-gray-600">Master the wisdom of the ages through interactive learning</p>
    </div>

    <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
      <div class="card">
        <div class="card-body">
          <h3 class="text-lg font-medium text-gray-900 mb-2">Ancient Philosophy</h3>
          <p class="text-sm text-gray-600 mb-4">Explore the foundations of Western thought</p>
          <div class="flex items-center justify-between">
            <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
              Available
            </span>
            <button class="btn-primary">Start Learning</button>
          </div>
        </div>
      </div>

      <div class="card opacity-60">
        <div class="card-body">
          <h3 class="text-lg font-medium text-gray-900 mb-2">Medieval Philosophy</h3>
          <p class="text-sm text-gray-600 mb-4">Faith and reason in medieval thought</p>
          <div class="flex items-center justify-between">
            <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
              Locked
            </span>
            <button class="btn-secondary" disabled>Locked</button>
          </div>
        </div>
      </div>

      <div class="card opacity-60">
        <div class="card-body">
          <h3 class="text-lg font-medium text-gray-900 mb-2">Modern Philosophy</h3>
          <p class="text-sm text-gray-600 mb-4">Enlightenment and revolutionary ideas</p>
          <div class="flex items-center justify-between">
            <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
              Locked
            </span>
            <button class="btn-secondary" disabled>Locked</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
