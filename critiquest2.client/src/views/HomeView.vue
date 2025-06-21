<template>
  <div class="min-h-screen bg-gradient-to-br from-indigo-900 via-purple-900 to-primary-900">
    <!-- Hero Section -->
    <div class="relative overflow-hidden">
      <!-- Background pattern -->
      <div class="absolute inset-0 opacity-20">
        <div class="absolute inset-0" style="background-image: radial-gradient(circle at 25% 25%, #ffffff22 1px, transparent 1px), radial-gradient(circle at 75% 75%, #ffffff22 1px, transparent 1px); background-size: 60px 60px;"></div>
      </div>

      <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-20 sm:py-24 lg:py-32">
        <div class="text-center">
          <!-- Logo/Brand -->
          <div class="mb-8">
            <h1 class="text-5xl sm:text-6xl lg:text-7xl font-bold">
              <span class="bg-gradient-to-r from-yellow-400 via-orange-400 to-red-400 bg-clip-text text-transparent">
                CritiQuest
              </span>
              <span class="text-white block sm:inline">2</span>
            </h1>
            <div class="mt-4 text-xl sm:text-2xl text-indigo-200 font-light">
              Podróż przez mądrość wieków
            </div>
          </div>

          <!-- Hero Text -->
          <div class="max-w-4xl mx-auto">
            <h2 class="text-3xl sm:text-4xl lg:text-5xl font-bold text-white leading-tight">
              Zostań <span class="text-yellow-400">filozofem</span><br>
              naszych czasów
            </h2>
            <p class="mt-6 text-lg sm:text-xl text-indigo-200 max-w-3xl mx-auto leading-relaxed">
              Odkryj tajniki myślenia krytycznego przez interaktywne lekcje, zbieraj legendarnych myślicieli
              i rozwijaj swoją mądrość w towarzystwie największych umysłów ludzkości.
            </p>
          </div>

          <!-- CTA Buttons -->
          <div class="mt-10 flex flex-col sm:flex-row gap-4 justify-center items-center">
            <router-link to="/register"
                         class="group relative px-8 py-4 bg-gradient-to-r from-yellow-400 to-orange-500 text-black font-semibold rounded-xl shadow-lg hover:shadow-xl transform hover:scale-105 transition-all duration-200 text-lg">
              <span class="relative z-10">🚀 Rozpocznij przygodę</span>
              <div class="absolute inset-0 bg-gradient-to-r from-yellow-300 to-orange-400 rounded-xl opacity-0 group-hover:opacity-100 transition-opacity duration-200"></div>
            </router-link>
            <router-link to="/login"
                         class="px-8 py-4 bg-white/10 backdrop-blur-sm text-white font-semibold rounded-xl border border-white/20 hover:bg-white/20 transition-all duration-200 text-lg">
              Mam już konto
            </router-link>
          </div>

          <!-- Philosopher Quote -->
          <div class="mt-16 max-w-2xl mx-auto">
            <blockquote class="text-indigo-200 text-lg italic font-light">
              "Nie można wejść dwa razy do tej samej rzeki"
            </blockquote>
            <div class="mt-2 text-indigo-300 text-sm">
              — Heraklit z Efezu
            </div>
          </div>
        </div>
      </div>

      <!-- Floating Elements -->
      <div class="absolute top-20 left-10 w-20 h-20 bg-yellow-400/20 rounded-full blur-xl animate-pulse"></div>
      <div class="absolute bottom-20 right-10 w-32 h-32 bg-purple-400/20 rounded-full blur-xl animate-pulse delay-1000"></div>
    </div>

    <!-- Features Section -->
    <div class="py-20 bg-gradient-to-b from-transparent to-black/20">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl sm:text-4xl font-bold text-white mb-4">
            Jak działa <span class="text-yellow-400">CritiQuest2</span>?
          </h2>
          <p class="text-xl text-indigo-200 max-w-3xl mx-auto">
            Połącz naukę z rozrywką w unikalnym systemie edukacji filozoficznej
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          <!-- Feature 1 -->
          <div class="group relative bg-white/10 backdrop-blur-sm rounded-2xl p-8 border border-white/20 hover:bg-white/15 transition-all duration-300 hover:transform hover:scale-105">
            <div class="absolute -top-4 -left-4 w-12 h-12 bg-gradient-to-br from-blue-400 to-indigo-600 rounded-xl flex items-center justify-center text-2xl">
              📚
            </div>
            <h3 class="text-xl font-semibold text-white mb-4 mt-4">Interaktywne Lekcje</h3>
            <p class="text-indigo-200 leading-relaxed">
              Poznawaj filozofię przez angażujące scenariusze, debaty i refleksje.
              Każda lekcja to podróż przez umysł wielkiego myśliciela.
            </p>
            <div class="mt-6 text-sm text-yellow-400 font-medium">
              • Tekst, video i elementy interaktywne<br>
              • Mapy pojęć i linie czasu<br>
              • Osobiste refleksje i notatki
            </div>
          </div>

          <!-- Feature 2 -->
          <div class="group relative bg-white/10 backdrop-blur-sm rounded-2xl p-8 border border-white/20 hover:bg-white/15 transition-all duration-300 hover:transform hover:scale-105">
            <div class="absolute -top-4 -left-4 w-12 h-12 bg-gradient-to-br from-purple-400 to-pink-600 rounded-xl flex items-center justify-center text-2xl">
              🎭
            </div>
            <h3 class="text-xl font-semibold text-white mb-4 mt-4">Twój Gimnazjon</h3>
            <p class="text-indigo-200 leading-relaxed">
              Zbieraj legendarnych filozofów od Sokratesa po Simone de Beauvoir.
              Każdy myśliciel ma unikalne umiejętności i mądrość.
            </p>
            <div class="mt-6 text-sm text-yellow-400 font-medium">
              • System gacha z filozofami<br>
              • Rzadkość: Zwykli → Legendarni<br>
              • Bonusy do nauki i quizów
            </div>
          </div>

          <!-- Feature 3 -->
          <div class="group relative bg-white/10 backdrop-blur-sm rounded-2xl p-8 border border-white/20 hover:bg-white/15 transition-all duration-300 hover:transform hover:scale-105">
            <div class="absolute -top-4 -left-4 w-12 h-12 bg-gradient-to-br from-green-400 to-emerald-600 rounded-xl flex items-center justify-center text-2xl">
              🏆
            </div>
            <h3 class="text-xl font-semibold text-white mb-4 mt-4">Postęp i Osiągnięcia</h3>
            <p class="text-indigo-200 leading-relaxed">
              Śledzj swoją filozoficzną podróż, zdobywaj osiągnięcia i utrzymuj
              serie nauki. Każdy dzień to krok bliżej mądrości.
            </p>
            <div class="mt-6 text-sm text-yellow-400 font-medium">
              • System poziomów i doświadczenia<br>
              • Serie nauki i streaki<br>
              • Odznaki i nagrody specjalne
            </div>
          </div>

          <!-- Feature 4 -->
          <div class="group relative bg-white/10 backdrop-blur-sm rounded-2xl p-8 border border-white/20 hover:bg-white/15 transition-all duration-300 hover:transform hover:scale-105">
            <div class="absolute -top-4 -left-4 w-12 h-12 bg-gradient-to-br from-red-400 to-orange-600 rounded-xl flex items-center justify-center text-2xl">
              ⚔️
            </div>
            <h3 class="text-xl font-semibold text-white mb-4 mt-4">Debaty Filozoficzne</h3>
            <p class="text-indigo-200 leading-relaxed">
              Testuj swoją wiedzę w debatach z wielkimi myślicielami.
              Rozwijaj umiejętności argumentacji i krytycznego myślenia.
            </p>
            <div class="mt-6 text-sm text-yellow-400 font-medium">
              • AI-wspomagane debaty<br>
              • Różne szkoły filozoficzne<br>
              • Punkty przekonania i retoryki
            </div>
          </div>

          <!-- Feature 5 -->
          <div class="group relative bg-white/10 backdrop-blur-sm rounded-2xl p-8 border border-white/20 hover:bg-white/15 transition-all duration-300 hover:transform hover:scale-105">
            <div class="absolute -top-4 -left-4 w-12 h-12 bg-gradient-to-br from-cyan-400 to-blue-600 rounded-xl flex items-center justify-center text-2xl">
              🧠
            </div>
            <h3 class="text-xl font-semibold text-white mb-4 mt-4">Quizy Adaptacyjne</h3>
            <p class="text-indigo-200 leading-relaxed">
              Sprawdź swoją wiedzę w quizach dostosowanych do Twojego poziomu.
              Otrzymuj bonusy od posiadanych filozofów.
            </p>
            <div class="mt-6 text-sm text-yellow-400 font-medium">
              • Pytania wielokrotnego wyboru<br>
              • Kontekst filozoficzny<br>
              • Bonusy od kolekcji filozofów
            </div>
          </div>

          <!-- Feature 6 -->
          <div class="group relative bg-white/10 backdrop-blur-sm rounded-2xl p-8 border border-white/20 hover:bg-white/15 transition-all duration-300 hover:transform hover:scale-105">
            <div class="absolute -top-4 -left-4 w-12 h-12 bg-gradient-to-br from-yellow-400 to-amber-600 rounded-xl flex items-center justify-center text-2xl">
              🔮
            </div>
            <h3 class="text-xl font-semibold text-white mb-4 mt-4">Wyrocznia</h3>
            <p class="text-indigo-200 leading-relaxed">
              Odwiedzaj Wyrocznię, aby zdobyć nowych filozofów.
              Każda wizyta to szansa na odkrycie nowego mentora.
            </p>
            <div class="mt-6 text-sm text-yellow-400 font-medium">
              • System losowania filozofów<br>
              • Bilety zdobywane za naukę<br>
              • Rzadkie i legendarne postacie
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Philosophy Stages Section -->
    <div class="py-20 bg-gradient-to-b from-black/20 to-black/40">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl sm:text-4xl font-bold text-white mb-4">
            Przejdź przez <span class="text-yellow-400">epoki filozofii</span>
          </h2>
          <p class="text-xl text-indigo-200 max-w-3xl mx-auto">
            Od starożytnej Grecji po współczesne myślenie - poznaj ewolucję ludzkiej mądrości
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
          <!-- Ancient Philosophy -->
          <div class="relative bg-gradient-to-br from-amber-900/40 to-orange-900/40 backdrop-blur-sm rounded-2xl p-8 border border-amber-500/30">
            <div class="flex items-center mb-6">
              <div class="w-16 h-16 bg-gradient-to-br from-amber-400 to-orange-500 rounded-xl flex items-center justify-center text-2xl mr-4">
                🏛️
              </div>
              <div>
                <h3 class="text-2xl font-bold text-white">Filozofia Starożytna</h3>
                <p class="text-amber-200">300 p.n.e. - 500 n.e.</p>
              </div>
            </div>
            <p class="text-amber-100 mb-6 leading-relaxed">
              Odkryj fundamenty zachodniej filozofii. Poznaj stoicyzm, platonizm,
              arystotelizm i inne szkoły myślenia, które kształtują nas do dziś.
            </p>
            <div class="grid grid-cols-2 gap-4 text-sm">
              <div class="bg-white/10 p-3 rounded-lg">
                <div class="font-semibold text-amber-200">Sokrates</div>
                <div class="text-amber-300 text-xs">Poznaj siebie</div>
              </div>
              <div class="bg-white/10 p-3 rounded-lg">
                <div class="font-semibold text-amber-200">Marek Aureliusz</div>
                <div class="text-amber-300 text-xs">Rozmyślania</div>
              </div>
              <div class="bg-white/10 p-3 rounded-lg">
                <div class="font-semibold text-amber-200">Arystoteles</div>
                <div class="text-amber-300 text-xs">Logika i etyka</div>
              </div>
              <div class="bg-white/10 p-3 rounded-lg">
                <div class="font-semibold text-amber-200">Epiktet</div>
                <div class="text-amber-300 text-xs">Dychotomia kontroli</div>
              </div>
            </div>
          </div>

          <!-- Modern Philosophy -->
          <div class="relative bg-gradient-to-br from-purple-900/40 to-indigo-900/40 backdrop-blur-sm rounded-2xl p-8 border border-purple-500/30">
            <div class="flex items-center mb-6">
              <div class="w-16 h-16 bg-gradient-to-br from-purple-400 to-indigo-500 rounded-xl flex items-center justify-center text-2xl mr-4">
                💡
              </div>
              <div>
                <h3 class="text-2xl font-bold text-white">Filozofia Nowożytna</h3>
                <p class="text-purple-200">XVII w. - dziś</p>
              </div>
            </div>
            <p class="text-purple-100 mb-6 leading-relaxed">
              Eksploduj rewolucyjne idee nowożytności. Egzystencjalizm, feminizm,
              postmodernizm - poznaj myślicieli, którzy kwestionują tradycje.
            </p>
            <div class="grid grid-cols-2 gap-4 text-sm">
              <div class="bg-white/10 p-3 rounded-lg">
                <div class="font-semibold text-purple-200">Simone de Beauvoir</div>
                <div class="text-purple-300 text-xs">Druga płeć</div>
              </div>
              <div class="bg-white/10 p-3 rounded-lg">
                <div class="font-semibold text-purple-200">Jean-Paul Sartre</div>
                <div class="text-purple-300 text-xs">Egzystencjalizm</div>
              </div>
              <div class="bg-white/10 p-3 rounded-lg">
                <div class="font-semibold text-purple-200">Friedrich Nietzsche</div>
                <div class="text-purple-300 text-xs">Übermensch</div>
              </div>
              <div class="bg-white/10 p-3 rounded-lg">
                <div class="font-semibold text-purple-200">Immanuel Kant</div>
                <div class="text-purple-300 text-xs">Imperatyw kategoryczny</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Final CTA Section -->
    <div class="py-20 bg-gradient-to-t from-black/60 to-transparent">
      <div class="max-w-4xl mx-auto text-center px-4 sm:px-6 lg:px-8">
        <h2 class="text-3xl sm:text-4xl font-bold text-white mb-6">
          Gotów na <span class="text-yellow-400">filozoficzną przygodę</span>?
        </h2>
        <p class="text-xl text-indigo-200 mb-10 leading-relaxed">
          Dołącz do tysięcy osób, które już odkrywają mądrość wieków.
          Twoja podróż filozoficzna zaczyna się już dziś.
        </p>

        <div class="flex flex-col sm:flex-row gap-6 justify-center items-center">
          <router-link to="/register"
                       class="group relative px-8 py-4 bg-gradient-to-r from-yellow-400 to-orange-500 text-black font-bold rounded-xl shadow-xl hover:shadow-2xl transform hover:scale-105 transition-all duration-300 text-lg">
            <span class="relative z-10">🎓 Zacznij naukę za darmo</span>
            <div class="absolute inset-0 bg-gradient-to-r from-yellow-300 to-orange-400 rounded-xl opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
          </router-link>

          <div class="text-indigo-300 text-sm">
            ✅ Całkowicie za darmo<br>
            ✅ Bez zobowiązań<br>
            ✅ Rozpocznij w 30 sekund
          </div>
        </div>

        <!-- Stats -->
        <div class="mt-16 grid grid-cols-2 md:grid-cols-4 gap-8">
          <div class="text-center">
            <div class="text-3xl font-bold text-yellow-400">50+</div>
            <div class="text-indigo-200 text-sm">Filozofów do odkrycia</div>
          </div>
          <div class="text-center">
            <div class="text-3xl font-bold text-yellow-400">200+</div>
            <div class="text-indigo-200 text-sm">Interaktywnych lekcji</div>
          </div>
          <div class="text-center">
            <div class="text-3xl font-bold text-yellow-400">15+</div>
            <div class="text-indigo-200 text-sm">Szkół filozoficznych</div>
          </div>
          <div class="text-center">
            <div class="text-3xl font-bold text-yellow-400">∞</div>
            <div class="text-indigo-200 text-sm">Możliwości rozwoju</div>
          </div>
        </div>
      </div>
    </div>

    <!-- Footer -->
    <footer class="bg-black/60 backdrop-blur-sm border-t border-white/10 py-8">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center text-indigo-300">
          <div class="text-lg font-semibold mb-2">CritiQuest2</div>
          <p class="text-sm">
            Podróż przez mądrość wieków • {{ new Date().getFullYear() }}
          </p>
          <p class="text-xs mt-2 text-indigo-400">
            "Życie bez badania nie jest warte życia" — Sokrates
          </p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
// No complex logic needed for this view - it's primarily presentational
</script>
