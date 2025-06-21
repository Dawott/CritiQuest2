using CritiQuest2.Server.Model.Entities;
using System.Net;
using System.Text.Json;

namespace CritiQuest2.Server.Data.Seeds
{
    public static class PhilosopherSeedData
    {
        public static List<Philosopher> GetPhilosophers()
        {
            return new List<Philosopher>
            {
                new Philosopher
                {
                    Id = "marcus-aurelius",
                    Name = "Marek Aureliusz",
                    Era = "Starożytność",
                    School = "Stoicyzm",
                    Rarity = Rarity.Legendary,
                    Wisdom = 95,
                    Logic = 85,
                    Rhetoric = 75,
                    Influence = 90,
                    Originality = 70,
                    Description = "Cesarz rzymski i filozof stoicki, który napisał „Rozmyślania'. Mistrz praktycznej mądrości i utrzymywania wewnętrznego spokoju pośród chaosu. Jego filozoficzna zbroja czyni go niemal odpornym na ataki emocjonalne.",
                    ImageUrl = "./src/assets/marcus-aurelius.jpg",
                    QuotesJson = JsonSerializer.Serialize(new[]
                    {
                        "Masz władzę nad swoim umysłem - nie nad wydarzeniami zewnętrznymi. Uświadom to sobie, a znajdziesz siłę.",
                        "Przeszkoda w działaniu przyspiesza działanie. To, co stoi na drodze, staje się drogą.",
                        "Do szczęśliwego życia potrzeba bardzo niewiele; wszystko jest w tobie.",
                        "Najlepszą zemstą jest nie być jak twój wróg.",
                        "To, co robimy teraz, odbija się echem w wieczności."
                    }),
                    SpecialAbilityJson = JsonSerializer.Serialize(new
                    {
                        name = "Stoicka Forteca",
                        description = "Staje się odporny na wszystkie osłabienia na 3 tury i zyskuje +20 do wszystkich statystyk obronnych.",
                        cooldown = 5,
                        effect = new
                        {
                            type = "special",
                            target = "self",
                            stats = new { wisdom = 20, logic = 20, rhetoric = 20 },
                            duration = 3
                        }
                    })
                },
                new Philosopher
                {
                    Id = "simone-de-beauvoir",
                    Name = "Simone de Beauvoir",
                    Era = "Współczesność",
                    School = "Egzystencjalizm",
                    Rarity = Rarity.Epic,
                    Wisdom = 85,
                    Logic = 80,
                    Rhetoric = 90,
                    Influence = 85,
                    Originality = 95,
                    Description = "Pionierka filozofii feministycznej i myślicielka egzystencjalistyczna. Autorka 'Drugiej płci', zrewolucjonizowała rozumienie płci i wolności. Jej zdolność do dekonstrukcji konstruktów społecznych jest niezrównana.",
                    ImageUrl = "./src/assets/simone-de-beauvoir.jpg",
                    QuotesJson = JsonSerializer.Serialize(new[]
                    {
                        "Kobietą się nie rodzi, kobietą się staje.",
                        "Jestem zbyt inteligentna, zbyt wymagająca i zbyt zaradna, by ktokolwiek mógł przejąć nade mną całkowitą kontrolę.",
                        "Autentyczność wymaga od nas zmierzenia się z naszą wolnością i związaną z nią udręką.",
                        "Zmień swoje życie już dziś. Nie stawiaj na przyszłość, działaj teraz, bez zwłoki.",
                        "Nie chodzi o to, by kobiety po prostu odbierały władzę mężczyznom, bo to nie zmieniłoby niczego w świecie."
                    }),
                    SpecialAbilityJson = JsonSerializer.Serialize(new
                    {
                        name = "Uderzenie Wyzwolenia",
                        description = "Usuwa wszystkie modyfikacje statystyk obu graczy i zadaje obrażenia w oparciu o różnicę.",
                        cooldown = 4,
                        effect = new
                        {
                            type = "transform",
                            target = "both",
                            duration = 1
                        }
                    })
                },
                new Philosopher
                {
                    Id = "diogenes",
                    Name = "Diogenes",
                    Era = "Starożytność",
                    School = "Cynizm",
                    Rarity = Rarity.Epic,
                    Wisdom = 90,
                    Logic = 70,
                    Rhetoric = 70,
                    Influence = 85,
                    Originality = 95,
                    Description = "Kontrowersyjny filozof grecki, twórca szkoły cyników",
                    ImageUrl = "./src/assets/Diogenes.jpg",
                    QuotesJson = JsonSerializer.Serialize(new[]
                    {
                        "Bieda jest nauczycielką i podporą filozofii, bo do czego filozofia nakłania słowami, do tego bieda zmusza w praktyce.",
                        "Gdybym był zawodnikiem, to czy zbliżając się do mety miałbym zwolnić kroku, czy raczej jeszcze bardziej przyśpieszyć?",
                        "Jedynym państwem dobrze urządzonym byłoby państwo obejmujące cały świat.",
                        "Słońce także kloaki nawiedza, a nie maże się.",
                        "Jestem obywatelem świata."
                    }),
                    SpecialAbilityJson = JsonSerializer.Serialize(new
                    {
                        name = "Oto Człowiek!",
                        description = "Niweluje dodatkowe modyfikatory obu graczy",
                        cooldown = 4,
                        effect = new
                        {
                            type = "transform",
                            target = "both",
                            duration = 1
                        }
                    })
                },
                new Philosopher
                {
                    Id = "socrates",
                    Name = "Sokrates",
                    Era = "Starożytność",
                    School = "Etyka",
                    Rarity = Rarity.Legendary,
                    Wisdom = 100,
                    Logic = 85,
                    Rhetoric = 90,
                    Influence = 90,
                    Originality = 80,
                    Description = "Rewolucyjny filozof grecki, inspirator wielu nurtów filozoficznych i etyki",
                    ImageUrl = "./src/assets/Socrates.jpg",
                    QuotesJson = JsonSerializer.Serialize(new[]
                    {
                        "Najmądrzejszy jest, który wie, czego nie wie.",
                        "Sprawiedliwość jest odmianą mądrości.",
                        "W całym życiu szanuj prawdę tak, by twoje słowa były bardziej wiarygodne od przyrzeczeń innych.",
                        "Wszelka dusza jest nieśmiertelna.",
                        "Ateny są jak ospały koń, a ja jak giez, który próbuje go ożywić."
                    }),
                    SpecialAbilityJson = JsonSerializer.Serialize(new
                    {
                        name = "Metoda Sokratejska",
                        description = "Placeholder",
                        cooldown = 4,
                        effect = new
                        {
                            type = "transform",
                            target = "both",
                            duration = 1
                        }
                    })
                },
                new Philosopher
                {
                    Id = "avicenna",
                    Name = "Awicenna",
                    Era = "Wczesne Średniowiecze",
                    School = "Metafizyka",
                    Rarity = Rarity.Rare,
                    Wisdom = 65,
                    Logic = 85,
                    Rhetoric = 55,
                    Influence = 80,
                    Originality = 70,
                    Description = "Lekarz i filozof islamski, kontynuujący nauki Arystotelesa. Skupiał się na filozofii natury, metafizyce i kosmologii",
                    ImageUrl = "./src/assets/Avicenna.jpg",
                    QuotesJson = JsonSerializer.Serialize(new[]
                    {
                        "Przeto odrębność człowieka polega na pojmowaniu i osądzaniu idei ogólnych i wyciąganiu rzeczy nieznanych z nauk i sztuk. To wszystko jest w mocy jednej duszy.",
                        "Lekarz ignorant jest adiutantem śmierci.",
                        "Szerokość życia jest ważniejsza niż długość życia.",
                        "Ten co wie, że wie – tego słuchajcie, ten co wie, że nie wie – tego pouczcie, ten co nie wie, że wie – tego obudźcie, ten co nie wie, że nie wie – tego zostawcie samego sobie."
                    }),
                    SpecialAbilityJson = JsonSerializer.Serialize(new
                    {
                        name = "Bóg i Natura",
                        description = "Placeholder",
                        cooldown = 4,
                        effect = new
                        {
                            type = "transform",
                            target = "both",
                            duration = 1
                        }
                    })
                },
                new Philosopher
                {
                    Id = "camus",
                    Name = "Albert Camus",
                    Era = "Współczesność",
                    School = "Absurdyzm",
                    Rarity = Rarity.Legendary,
                    Wisdom = 70,
                    Logic = 85,
                    Rhetoric = 80,
                    Influence = 85,
                    Originality = 85,
                    Description = "Pisarz oraz dziennikarz. Jedna z barwniejszych postaci powojennego egzystencjalizmu",
                    ImageUrl = "./src/assets/Camus.jpg",
                    QuotesJson = JsonSerializer.Serialize(new[]
                    {
                        "Człowiek jest jedynym stworzeniem, które nie godzi się być tym, czym jest.",
                        "Aby wypełnić ludzkie serce, wystarczy walka prowadząca ku szczytom. Trzeba sobie wyobrażać Syzyfa szczęśliwym.",
                        "Przyzwyczajenie się do rozpaczy jest gorsze niż sama rozpacz",
                        "W ludziach więcej rzeczy zasługuje na podziw niż na pogardę."
                    }),
                    SpecialAbilityJson = JsonSerializer.Serialize(new
                    {
                        name = "Syzyf",
                        description = "Placeholder",
                        cooldown = 4,
                        effect = new
                        {
                            type = "transform",
                            target = "both",
                            duration = 1
                        }
                    })
                },
                new Philosopher
                {
                    Id = "locke",
                    Name = "John Locke",
                    Era = "Barok",
                    School = "Empiryzm",
                    Rarity = Rarity.Epic,
                    Wisdom = 90,
                    Logic = 80,
                    Rhetoric = 60,
                    Influence = 85,
                    Originality = 80,
                    Description = "Jeden z prekursorów klasycznego liberalizmu oraz teorii ekonomicznej. W filozofii rozważał tematy społeczne i naukowe",
                    ImageUrl = "./src/assets/Locke.jpg",
                    QuotesJson = JsonSerializer.Serialize(new[]
                    {
                        "Nic nie zachodzi bez przyczyny.",
                        "Wolność od absolutnej, arbitralnej władzy jest tak konieczna i ściśle złączona z samozachowaniem człowieka, że ten nie może ich rozdzielić, gdyż utraciłby wtedy samozachowanie wraz z życiem",
                        "Stopień ekscentryczności w społeczeństwie jest proporcjonalny do zawartego w nim geniuszu, materialnego wigoru i odwagi moralnej.",
                        "Naturalną wolnością człowieka jest bycie niezależnym od jakiejkolwiek nadrzędnej władzy na Ziemi oraz niepodleganie woli lub władzy prawodawczej człowieka, ale bycie rządzonym jedynie przez prawo Natury."
                    }),
                    SpecialAbilityJson = JsonSerializer.Serialize(new
                    {
                        name = "Zmysł Wewnętrzny",
                        description = "Placeholder",
                        cooldown = 4,
                        effect = new
                        {
                            type = "transform",
                            target = "both",
                            duration = 1
                        }
                    })
                }
            };
        }
    }
}