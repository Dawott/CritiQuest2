using CritiQuest2.Server.Model;
using CritiQuest2.Server.Model.Entities;
using System.Text.Json;

namespace CritiQuest2.Server.Data.Seeds
{
    public class InteractiveLessonSeed
    {
        public static void SeedInteractiveScenarios(ApplicationDbContext context)
        {
            if (context.InteractiveSections.Any()) return;

            // Create the reflection scenario from your example
            var reflectionSection = new InteractiveSection
            {
                Id = "existentialism-reflection-1",
                LessonId = "stoicism-intro", // Link to existing lesson
                Title = "Etyka Niepewności - Refleksja",
                Description = "Rozważ dylemat moralny związany z autentycznymi wyborami życiowymi",
                Type = InteractionType.Reflection,
                OrderInLesson = 1,
                IsRequired = false,
                EstimatedTimeMinutes = 10,
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
                    Guidance = "Nie ma \"właściwej\" odpowiedzi - ćwiczenie polega na uznaniu wolności wyboru i wartości, które ten wybór wyraża.",
                    PhilosophicalConcepts = new List<string> { "autentycznosc", "wolnosc", "odpowiedzialnosc", "wybor" }
                })
            };

            context.InteractiveSections.Add(reflectionSection);
            context.SaveChanges();
        }
    }
}