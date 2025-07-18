﻿using CritiQuest2.Server.Model.Entities;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Xml.Linq;

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
                            },
                            new
                            {
                                id = "marcus-section",
                                title = "Cesarz z Medytacjami",
                                content = "Jego „Rozmyślania' ukazują człowieka używającego filozofii jako zbroi przeciwko korupcji władzy absolutnej. 'Masz władzę nad swoim umysłem - nie nad wydarzeniami zewnętrznymi. Uświadom to sobie, a znajdziesz siłę' - pisał, prowadząc kampanie wojskowe na granicy Dunaju. Najpotężniejszy człowiek na świecie znalazł swoją największą siłę nie w swoich legionach, ale w filozofii stoickiej. Jeśli potrafiła ona poprowadzić cesarza przez plagi, wojny i zdrady, wyobraź sobie, co może zrobić z codziennymi wyzwaniami.",
                                type = "text",
                                mediaUrl = "https://example.com/marcus-aurelius-meditation.jpg"
                            },
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
                            },
                            new
                            {
                                id = "bad-faith-section",
                                title = "Wygodne więzienie złej wiary",
                                content = "De Beauvoir obserwowała ludzi na całym świecie uciekających od swojej wolności w coś, co ona i Sartre nazywali „złą wiarą' - zaprzeczeniem naszej fundamentalnej wolności wyboru. Rozważmy następujące przykłady: „Nic nie poradzę na to, że jestem zły, taki już jestem', 'Muszę zostać w tej pracy, nie mam wyboru', 'Społeczeństwo oczekuje, że będę postępował w ten sposób', „Zmusiły mnie do tego moje geny, wychowanie lub okoliczności'. Ale zła wiara bierze te ograniczenia i nadmuchuje je do absolutnego determinizmu. Jest wygodna, ponieważ usuwa niepokój związany z wyborem i odpowiedzialnością. De Beauvoir była szczególnie zainteresowana tym, jak kobiety były zachęcane do przyjęcia złej wiary, do postrzegania swoich ograniczeń jako naturalnych, a nie narzuconych. Jej praca ujawniła, w jaki sposób ucisk działa poprzez przekonywanie ludzi, że nie mają wyboru.",
                                type = "text",
                                mediaUrl = "https://example.com/cafe-de-flore-1940s.jpg"
                            },
                            new
                            {
                                id = "situated-freedom",
                                title = "Wolność w Kontekście",
                                content = "De Beauvoir udoskonaliła egzystencjalizm, podkreślając „usytuowaną wolność'. Nie wybieramy w próżni - wybieramy w określonych sytuacjach, które stwarzają zarówno możliwości, jak i ograniczenia.Kobieta we Francji lat czterdziestych XX wieku stawała przed innymi wyborami niż dziś.Osoba urodzona w ubóstwie napotyka inne ograniczenia niż osoba urodzona w bogactwie.Jednak w każdej sytuacji, bez względu na to, jak bardzo jest ona ograniczona, pozostaje pewien element wyboru. Ten zniuansowany pogląd pozwala uniknąć dwóch skrajności: fantazji o absolutnej wolności (ignorującej rzeczywiste ograniczenia) oraz rozpaczy determinizmu(zaprzeczającej wszelkiej wolności). Wybierając, pomagasz stworzyć sytuację dla przyszłych wyborów - zarówno swoich, jak i innych.",
                                type = "text",
                                mediaUrl = "https://example.com/de-beauvoir-writing.jpg"
                            },
                            new
                            {
                                id = "closing-section",
                                title = "Odwaga do wolności",
                                content = "Wychodząc z tej lekcji, niesie ze sobą egzystencjalistyczne spostrzeżenie: jesteś wolny, a wraz z tą wolnością przychodzi ciężar odpowiedzialności za to, kim się stajesz. Nie jest to ciężar, który należy niechętnie znosić, ale głęboka szansa. W każdym wyborze, małym lub dużym, tworzysz siebie i przyczyniasz się do ludzkiego projektu definiowania tego, co to znaczy być człowiekiem. De Beauvoir pokazała nam, że autentyczność nie polega na znalezieniu siebie - polega na stworzeniu siebie z odwagą, jasnością i szacunkiem dla wolności innych. Pytanie brzmi teraz: Kim zdecydujesz się zostać?",
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
