using CritiQuest2.Server.Model;
using CritiQuest2.Server.Model.Entities;
using Microsoft.Identity.Client;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CritiQuest2.Server.Data.Seeds
{
    public class InteractiveLessonSeed
    {
        public static void SeedInteractiveScenarios(ApplicationDbContext context)
        {
            if (context.InteractiveSections.Any()) return;

            // Create the reflection scenario from your example
            var interactiveSections = new List<InteractiveSection>
            {
                // Practice Section - Stoic "View from Above" meditation
                new InteractiveSection
                {
                    Id = "practice-section",
                    LessonId = "stoicism-intro",
                    Title = "Twoje pierwsze stoickie ćwiczenie",
                    Description = "Stoicy byli nie tylko teoretykami - opracowali praktyczne ćwiczenia do zastosowania w codziennym życiu. Wypróbuj medytację „Widok z góry', którą praktykował Marek Aureliusz.",
                    Type = InteractionType.Reflection,
                    OrderInLesson = 2,
                    IsRequired = false,
                    EstimatedTimeMinutes = 8,
                    ConfigurationJson = JsonSerializer.Serialize(new ReflectionConfig
                    {
                        Scenario = "Stoicy byli nie tylko teoretykami - opracowali praktyczne ćwiczenia do zastosowania w codziennym życiu. Wypróbuj medytację „Widok z góry', którą praktykował Marek Aureliusz: \n\n1.Zamknij oczy i wyobraź sobie, że wznosisz się ponad swoją obecną lokalizację\n2.Zobacz z góry swój budynek, a następnie swoją okolicę\n3.Kontynuuj wznoszenie: zobacz swoje miasto, kraj, kontynent\n4.Na koniec zobacz Ziemię jako małą niebieską kulę w bezmiarze kosmosu\n5.To ćwiczenie nie minimalizuje prawdziwych problemów, ale daje perspektywę. Jak napisał Marcus: „Obserwuj gwiazdy w ich biegach, tak jakbyś im towarzyszył, i nieustannie zastanawiaj się nad przemianami żywiołów w siebie nawzajem'.",
                        Prompts = new List<string>
                        {
                            "Jakie zmartwienie wydawało się mniejsze z perspektywy kosmosu?",
                            "Jak to zmieniło twoje odczucia na temat sytuacji?",
                            "Kiedy mógłbyś użyć tej techniki w codziennym życiu?"
                        },
                        Guidance = "To ćwiczenie pomaga uzyskać perspektywę na codzienne problemy poprzez kosmiczne myślenie, zmniejszając niepokój bez minimalizowania prawdziwych obaw.",
                        PhilosophicalConcepts = new List<string> { "cosmic-perspective", "dichotomy-of-control", "stoic-meditation", "perspective" }
                    })
                },

                // Existence-Essence Comparison
                new InteractiveSection
                {
                    Id = "existence-essence",
                    LessonId = "existentialism-freedom",
                    Title = "Nie jesteś spinaczem do papieru",
                    Description = "Porównaj tradycyjne i egzystencjalistyczne spojrzenie na naturę człowieka i cel życia.",
                    Type = InteractionType.Comparison,
                    OrderInLesson = 2,
                    IsRequired = false,
                    EstimatedTimeMinutes = 7,
                    ConfigurationJson = JsonSerializer.Serialize(new ComparisonConfig
                    {
                        Title = "Esencja: Predeterminowana vs Tworzona",
                        Description = "Spinacz do papieru ma jasny cel - został zaprojektowany do spinania papierów. Jego istota (cel) poprzedzała jego istnienie. Tradycyjna filozofia i religia nauczały, że ludzie, podobnie jak spinacze, mają z góry określoną istotę - boski cel lub naturalną funkcję. Egzystencjaliści radykalnie to odrzucili.",
                        Comparisons = new List<ComparisonCategory>
                        {
                            new ComparisonCategory
                            {
                                Category = "Spojrzenie Tradycyjne",
                                Items = new List<string>
                                {
                                    "Urodzony z celem",
                                    "Odkryj swoje prawdziwe ja",
                                    "Podążaj za swoją naturą",
                                    "Wypełnij swoje przeznaczenie"
                                }
                            },
                            new ComparisonCategory
                            {
                                Category = "Spojrzenie Egzystencjalistyczne",
                                Items = new List<string>
                                {
                                    "Urodzony bez celu",
                                    "Stwórz siebie",
                                    "Zdefiniuj swoją naturę",
                                    "Napisz swoje przeznaczenie"
                                }
                            }
                        },
                        PhilosophicalConcepts = new List<string> { "existence-precedes-essence", "self-creation", "authenticity", "freedom" }
                    })
                },

                // Authenticity Section - Ethics of Uncertainty
                new InteractiveSection
                {
                    Id = "authenticity-section",
                    LessonId = "existentialism-freedom",
                    Title = "Etyka Niepewności",
                    Description = "Rozważ dylemat moralny związany z autentycznymi wyborami życiowymi w kontekście egzystencjalistycznej etyki.",
                    Type = InteractionType.Reflection,
                    OrderInLesson = 4,
                    IsRequired = false,
                    EstimatedTimeMinutes = 12,
                    ConfigurationJson = JsonSerializer.Serialize(new ReflectionConfig
                    {
                        Scenario = "Otrzymujesz propozycję awansu, który wymagałby zmiany miejsca zamieszkania, pozostawienia przyjaciół i społeczności, którą kochasz. Wzrost wynagrodzenia zapewniłby bezpieczeństwo finansowe, ale praca wydaje się mniej znacząca.",
                        Prompts = new List<string>
                        {
                            "Wymień czynniki, które wydają się wymuszać Twoją decyzję",
                            "Teraz przeformułuj każdy czynnik jako wybór, którego dokonujesz",
                            "Co wybór każdej opcji powiedziałby o Twoich wartościach?",
                            "Jak uznanie swojej wolności wyboru zmienia Twoje samopoczucie?"
                        },
                        Guidance = "Nie ma „właściwej' odpowiedzi - ćwiczenie polega na uznaniu wolności wyboru i wartości,które ten wybór wyraża.Autentyczna osoba nie pyta „Co powinienem zrobić ? ', ale „Jaką osobą staję się poprzez ten wybór?'.",
                        PhilosophicalConcepts = new List<string> { "authenticity", "bad-faith", "situated-freedom", "responsibility", "values" }
                    })
                }
            };

context.InteractiveSections.AddRange(interactiveSections);
context.SaveChanges();
        }
    }

    public class ReflectionConfig
    {
        public string Scenario { get; set; } = string.Empty;
        public List<string> Prompts { get; set; } = new();
        public string Guidance { get; set; } = string.Empty;
        public List<string> PhilosophicalConcepts { get; set; } = new();
    }

    public class ComparisonConfig
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<ComparisonCategory> Comparisons { get; set; } = new();
        public List<string> PhilosophicalConcepts { get; set; } = new();
    }

    public class ComparisonCategory
    {
        public string Category { get; set; } = string.Empty;
        public List<string> Items { get; set; } = new();
    }
}