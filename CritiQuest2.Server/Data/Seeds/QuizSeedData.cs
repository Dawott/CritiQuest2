using CritiQuest2.Server.Model.Entities;
using System.Text.Json;

namespace CritiQuest2.Server.Data.Seeds
{
    public static class QuizSeedData
    {
        public static List<Quiz> GetQuizzes()
        {
            return new List<Quiz>
            {
                new Quiz
                {
                    Id = "quiz-stoicism-intro",
                    LessonId = "stoicism-intro",
                    Title = "Przetestuj Wiedzę Stoicką",
                    Type = QuizType.MultipleChoice,
                    TimeLimit = 600, // 10 minutes
                    PassingScore = 70,
                    PhilosopherBonusJson = JsonSerializer.Serialize(new
                    {
                        philosopherId = "marcus-aurelius",
                        bonusMultiplier = 1.2
                    })
                },
                new Quiz
                {
                    Id = "quiz-existentialism-freedom",
                    LessonId = "existentialism-freedom",
                    Title = "Osiągając egzystencjalną wolność",
                    Type = QuizType.Scenario,
                    TimeLimit = 900, // 15 minutes
                    PassingScore = 75,
                    PhilosopherBonusJson = JsonSerializer.Serialize(new
                    {
                        philosopherId = "simone-de-beauvoir",
                        bonusMultiplier = 1.25
                    })
                }
            };
        }

        public static List<Question> GetQuestions()
        {
            return new List<Question>
            {
                // Stoicism Quiz Questions
                new Question
                {
                    Id = "q-stoic-1",
                    QuizId = "quiz-stoicism-intro",
                    Text = "Jaki jest klucz do spokojnego życia według stoików?",
                    Type = QuestionType.Single,
                    Order = 1,
                    OptionsJson = JsonSerializer.Serialize(new[]
                    {
                        "Gromadzenie bogactwa i władzy",
                        "Unikanie wszelkich trudnych sytuacji",
                        "Skupianie się na tym, co jest pod naszą kontrolą",
                        "Dążenie do przyjemności i unikanie bólu"
                    }),
                    CorrectAnswersJson = JsonSerializer.Serialize(new[] { "Skupianie się na tym, co jest pod naszą kontrolą" }),
                    Explanation = "Stoicy nauczali, że spokój umysłu wynika ze skupienia się na tym, co możemy kontrolować (nasze myśli, osądy i działania), a nie na zewnętrznych okolicznościach, na które nie mamy wpływu.",
                    PhilosophicalContext = "Zasada ta, znana jako dychotomia kontroli, była kluczowa dla nauczania Epikteta i znajduje się w Medytacjach Marka Aureliusza.",
                    Points = 10
                },
                new Question
                {
                    Id = "q-stoic-2",
                    QuizId = "quiz-stoicism-intro",
                    Text = "Które z poniższych jest w naszej kontroli według stoików?",
                    Type = QuestionType.Multiple,
                    Order = 2,
                    OptionsJson = JsonSerializer.Serialize(new[]
                    {
                        "Nasza ocena wydarzeń",
                        "Działania innych osób",
                        "Nasze reakcje emocjonalne",
                        "Okoliczności zewnętrzne",
                        "Nasze wartości i wybory"
                    }),
                    CorrectAnswersJson = JsonSerializer.Serialize(new[]
                    {
                        "Nasza ocena wydarzeń",
                        "Nasze reakcje emocjonalne",
                        "Nasze wartości i wybory"
                    }),
                    Explanation = "Stoicy rozróżniają między tym, co 'zależy od nas' (nasze osądy, pragnienia, decyzje), a tym, co 'nie zależy od nas' (wydarzenia zewnętrzne, działania innych, nasza reputacja).",
                    PhilosophicalContext = "Klasyfikacja ta pomaga praktykom skoncentrować swoją energię na obszarach, w których mają prawdziwą władzę, zmniejszając frustrację i zwiększając skuteczność.",
                    Points = 15
                },
                new Question
                {
                    Id = "q-stoic-3",
                    QuizId = "quiz-stoicism-intro",
                    Text = "Starannie przygotowałeś się do rozmowy kwalifikacyjnej, ale nie dostałeś posady. Jak poradziłby ci filozof stoicki?",
                    Type = QuestionType.Scenario,
                    Order = 3,
                    OptionsJson = JsonSerializer.Serialize(new[]
                    {
                        "Obwiniaj osobę prowadzącą rozmowę kwalifikacyjną za to, że nie doceniła Twoich kwalifikacji",
                        "Zaakceptuj wynik i skup się na tym, czego możesz się nauczyć z tego doświadczenia",
                        "Zrezygnuj ze swoich celów zawodowych, ponieważ są one wyraźnie nieosiągalne",
                        "Całkowicie zignoruj swoje uczucia związane z odrzuceniem"
                    }),
                    CorrectAnswersJson = JsonSerializer.Serialize(new[] { "Zaakceptuj wynik i skup się na tym, czego możesz się nauczyć z tego doświadczenia" }),
                    Explanation = "Stoicyzm uczy nas akceptować zewnętrzne rezultaty, jednocześnie utrzymując nasze zaangażowanie w cnotę i samodoskonalenie. Na wynik nie mamy wpływu, ale na naszą reakcję już tak.",
                    PhilosophicalContext = "Marek Aureliusz napisał: 'Ogranicz się do teraźniejszości' i 'Przeszkoda w działaniu przyspiesza działanie. To, co stoi na drodze, staje się drogą'.",
                    Points = 20
                },
                new Question
                {
                    Id = "q-stoic-4",
                    QuizId = "quiz-stoicism-intro",
                    Text = "Co ma na celu medytacja 'Widok z góry'?",
                    Type = QuestionType.Single,
                    Order = 4,
                    OptionsJson = JsonSerializer.Serialize(new[]
                    {
                        "Rozwijać nadprzyrodzone moce",
                        "Całkowicie uciekać od ziemskich trosk",
                        "Zyskać perspektywę na nasze problemy poprzez kosmiczne myślenie",
                        "Udowodnić, że indywidualne życie nie ma znaczenia"
                    }),
                    CorrectAnswersJson = JsonSerializer.Serialize(new[] { "Zyskać perspektywę na nasze problemy poprzez kosmiczne myślenie" }),
                    Explanation = "To stoickie ćwiczenie pomaga nam spojrzeć na nasze problemy w odpowiednich proporcjach, wyobrażając sobie siebie z kosmicznej perspektywy, zmniejszając niepokój bez minimalizowania prawdziwych obaw.",
                    PhilosophicalContext = "Marek Aureliusz często wykorzystywał kosmiczną perspektywę w swoich Medytacjach, aby zachować spokój, radząc sobie z presją rządzenia imperium.",
                    Points = 10
                },

                // Existentialism Quiz Questions
                new Question
                {
                    Id = "q-exist-1",
                    QuizId = "quiz-existentialism-freedom",
                    Text = "Co oznacza 'istnienie poprzedza istotę' w filozofii egzystencjalistycznej?",
                    Type = QuestionType.Single,
                    Order = 1,
                    OptionsJson = JsonSerializer.Serialize(new[]
                    {
                        "Fizyczne istnienie jest ważniejsze niż duchowa esencja",
                        "Najpierw istniejemy, a następnie tworzymy nasz cel poprzez nasze wybory",
                        "Istotne cechy determinują nasze istnienie",
                        "Istnienie i esencja to to samo"
                    }),
                    CorrectAnswersJson = JsonSerializer.Serialize(new[] { "Najpierw istniejemy, a następnie tworzymy nasz cel poprzez nasze wybory" }),
                    Explanation = "W przeciwieństwie do przedmiotów stworzonych w określonym celu, ludzie najpierw istnieją, a następnie definiują swoją istotę poprzez swoje działania i wybory.",
                    PhilosophicalContext = "Odwraca to tradycyjną filozofię i religię, które twierdziły, że ludzie mają z góry określoną naturę lub cel. De Beauvoir i Sartre twierdzili, że tworzymy samych siebie.",
                    Points = 10
                },
                new Question
                {
                    Id = "q-exist-2",
                    QuizId = "quiz-existentialism-freedom",
                    Text = "Twój przyjaciel mówi: „Nic nie poradzę na to, że jestem pesymistą, to po prostu moja osobowość'. Z egzystencjalistycznego punktu widzenia jest to przykład:",
                    Type = QuestionType.Scenario,
                    Order = 2,
                    OptionsJson = JsonSerializer.Serialize(new[]
                    {
                        "Autentyczna samoświadomość",
                        "Zła wiara - odmowa wolności wyboru postawy",
                        "Akceptacja ograniczeń w dobrej wierze",
                        "Odwaga egzystencjalna"
                    }),
                    CorrectAnswersJson = JsonSerializer.Serialize(new[] { "Zła wiara - odmowa wolności wyboru postawy" }),
                    Explanation = "Zła wiara polega na zaprzeczaniu naszej podstawowej wolności poprzez udawanie, że nie mamy wyboru. Chociaż możemy mieć tendencje, zawsze zachowujemy wolność wyboru naszej postawy.",
                    PhilosophicalContext = "De Beauvoir analizowała, w jaki sposób ludzie uciekają od lęku przed wolnością w złą wiarę, traktując wybrane postawy jako stałą naturę.",
                    Points = 15
                },
                new Question
                {
                    Id = "q-exist-3",
                    QuizId = "quiz-existentialism-freedom",
                    Text = "Według de Beauvoir autentyczne istnienie wymaga którego z poniższych?",
                    Type = QuestionType.Multiple,
                    Order = 3,
                    OptionsJson = JsonSerializer.Serialize(new[]
                    {
                        "Uznanie naszej radykalnej wolności",
                        "Ignorowanie ograniczeń społecznych",
                        "Szanowanie wolności innych, tak jak pragniemy własnej",
                        "Tworzenie znaczenia poprzez nasze wybory",
                        "Udawanie, że nie mamy ograniczeń"
                    }),
                    CorrectAnswersJson = JsonSerializer.Serialize(new[]
                    {
                        "Uznanie naszej radykalnej wolności",
                        "Szanowanie wolności innych, tak jak pragniemy własnej",
                        "Tworzenie znaczenia poprzez nasze wybory"
                    }),
                    Explanation = "Autentyczność wymaga uznania zarówno naszej wolności, jak i naszej sytuacji, tworzenia znaczenia przy jednoczesnym poszanowaniu równej wolności innych do robienia tego samego.",
                    PhilosophicalContext = "W „Etyce dwuznaczności' de Beauvoir opracowała etykę opartą na uznaniu naszej wspólnej ludzkiej kondycji wolności w ramach ograniczeń.",
                    Points = 20
                }
            };
        }
    }

    public static class DebateArgumentSeedData
    {
        public static List<DebateArgument> GetDebateArguments()
        {
            return new List<DebateArgument>
            {
                new DebateArgument
                {
                    Id = "arg-free-will-stoic",
                    Text = "Prawdziwa wolność nie pochodzi z nieograniczonych wyborów, ale ze zrozumienia tego, co jest pod naszą kontrolą i cnotliwego działania w tych granicach.",
                    PhilosophicalBasis = "Stoicka koncepcja wolności poprzez akceptację i dychotomia kontroli",
                    StrengthAgainstJson = JsonSerializer.Serialize(new[] { "arg-absolute-freedom", "arg-libertarian-free-will" }),
                    WeaknessAgainstJson = JsonSerializer.Serialize(new[] { "arg-existential-freedom", "arg-social-determinism" }),
                    SchoolBonusJson = JsonSerializer.Serialize(new[] { "Stoicyzm" }),
                    ConvictionPower = 75,
                    RequiresPhilosopher = "marcus-aurelius"
                },
                new DebateArgument
                {
                    Id = "arg-existential-freedom",
                    Text = "Jesteśmy skazani na bycie wolnymi - nawet brak wyboru sam w sobie jest wyborem. Nasza wolność jest absolutna i przerażająca.",
                    PhilosophicalBasis = "Egzystencjalistyczna radykalna wolność i odpowiedzialność",
                    StrengthAgainstJson = JsonSerializer.Serialize(new[] { "arg-social-determinism", "arg-fate-based" }),
                    WeaknessAgainstJson = JsonSerializer.Serialize(new[] { "arg-neuroscience-determinism", "arg-practical-constraints" }),
                    SchoolBonusJson = JsonSerializer.Serialize(new[] { "Egzystencjalizm" }),
                    ConvictionPower = 80,
                    RequiresPhilosopher = "simone-de-beauvoir"
                },
                new DebateArgument
                {
                    Id = "arg-social-determinism",
                    Text = "Nasze wybory są w dużej mierze zdeterminowane przez struktury społeczne, warunki ekonomiczne i programowanie kulturowe. Indywidualna wolność jest w większości iluzją.",
                    PhilosophicalBasis = "Determinizm socjologiczny i analiza strukturalna",
                    StrengthAgainstJson = JsonSerializer.Serialize(new[] { "arg-absolute-freedom", "arg-existential-freedom" }),
                    WeaknessAgainstJson = JsonSerializer.Serialize(new[] { "arg-free-will-stoic", "arg-pragmatic-freedom" }),
                    SchoolBonusJson = JsonSerializer.Serialize(new[] { "Teoria Krytyczna", "Marksizm" }),
                    ConvictionPower = 70,
                    RequiresPhilosopher = "camus"
                }
            };
        }
    }
}