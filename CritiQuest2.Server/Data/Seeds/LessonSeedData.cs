using CritiQuest2.Server.Model.Entities;
using System.Net;
using System.Text.Json;

namespace CritiQuest2.Server.Data.Seeds
{
    public static class LessonSeedData
    {
        public static List<Lesson> GetLessons()
        {
            return new List<Lesson>
            {
                new Lesson
                {
                    Id = "stoicism-intro",
                    Title = "Twierdza umysłu",
                    Description = "Odkryj starożytną filozofię stoicyzmu i dowiedz się, jak budować odporność psychiczną dzięki mądrości Marka Aureliusza i Epikteta.",
                    Stage = "ancient-philosophy",
                    Order = 1,
                    Difficulty = Difficulty.Beginner,
                    EstimatedTime = 15,
                    PhilosophicalConceptsJson = JsonSerializer.Serialize(new[]
                    {
                        "dichotomy-of-control", "virtue-ethics", "cosmic-perspective", "preferred-indifferents"
                    }),
                    RequiredPhilosopher = "marcus-aurelius",
                    ContentJson = JsonSerializer.Serialize(new
                    {
                        sections = new[]
                        {
                            new
                            {
                                id = "intro-section",
                                title = "Wrak statku, od którego wszystko się zaczęło",
                                content = "Około 300 r. p.n.e. bogaty kupiec o imieniu Zeno z Citium żeglował po Morzu Śródziemnym, gdy spotkała go katastrofa. Jego statek rozbił się, a on stracił całą swoją fortunę. Zdruzgotany, zawędrował do ateńskiej księgarni, gdzie odkrył nauki Sokratesa. To przypadkowe spotkanie przekształciło jego stratę w zysk filozofii. „Odbyłem pomyślną podróż, kiedy rozbił się mój statek' - powie później Zenon. Założył stoicyzm, nauczając, że prawdziwe bogactwo nie leży w posiadaniu, ale w mądrości i cnocie. Filozofia ta prowadziła zarówno cesarzy, jak i niewolników przez życiowe burze.",
                                type = "text",
                                mediaUrl = "https://example.com/ancient-athens-port.jpg"
                            },
                            new
                            {
                                id = "dichotomy-section",
                                title = "Potęga dychotomii",
                                content = "Wyobraź sobie, że masz przystąpić do ważnego egzaminu. Ciężko się uczyłeś, ale niepokoisz się o wynik. Stoicy zapytaliby: co dokładnie sprawia, że jesteś niespokojny? Epiktet, urodzony jako niewolnik, który stał się największym rzymskim nauczycielem filozofii, zidentyfikował źródło ludzkiego cierpienia: próbujemy kontrolować to, czego nie możemy, i zaniedbujemy to, co możemy. To, nad czym masz kontrolę: wysiłek i przygotowanie, nastawienie podczas egzaminu, reakcja na wynik, to, nad czym nie masz kontroly: pytania na egzaminie, zachowanie innych, ocena końcowa - oto Dychotomia Kontroli - kamień węgielny stoickiego spokoju.",
                                type = "interactive",
                                mediaUrl = ""
                                //interactionType = "concept-map"
                            }
                        }
                    }),
                    QuizId = "quiz-stoicism-intro",
                    RewardXp = 100,
                    RewardCoins = 50,
                    RewardContentJson = JsonSerializer.Serialize(new[]
                    {
                        "stoicism-daily-practices", "marcus-aurelius-quotes"
                    })
                },
                new Lesson
                {
                    Id = "existentialism-freedom",
                    Title = "Nieznośny ciężar wolności",
                    Description = "Poznaj egzystencjalizm przez pryzmat Simone de Beauvoir i odkryj, dlaczego nasza wolność jest zarówno darem, jak i ciężarem w tworzeniu autentycznego życia.",
                    Stage = "modern-philosophy",
                    Order = 3,
                    Difficulty = Difficulty.Intermediate,
                    EstimatedTime = 25,
                    PhilosophicalConceptsJson = JsonSerializer.Serialize(new[]
                    {
                        "existence-precedes-essence", "bad-faith", "authenticity", "situated-freedom", "the-other"
                    }),
                    RequiredPhilosopher = "simone-de-beauvoir",
                    ContentJson = JsonSerializer.Serialize(new
                    {
                        sections = new[]
                        {
                            new
                            {
                                id = "cafe-scene",
                                title = "Rozmowa w Café de Flore",
                                content = "Paryż, rok 1943. W zadymionej mgle Café de Flore Simone de Beauvoir siedzi i pisze. Na zewnątrz szaleje wojna, ale w jej wnętrzu powstają idee, które zrewolucjonizują nasze myślenie o wolności i odpowiedzialności.",
                                type = "text",
                                mediaUrl = "https://example.com/cafe-de-flore-1940s.jpg"
                            }
                        }
                    }),
                    QuizId = "quiz-existentialism-freedom",
                    RewardXp = 150,
                    RewardCoins = 75,
                    RewardContentJson = JsonSerializer.Serialize(new[]
                    {
                        "bad-faith-analyzer", "authenticity-journal", "de-beauvoir-ethics"
                    })
                }
            };
        }
    }
}
