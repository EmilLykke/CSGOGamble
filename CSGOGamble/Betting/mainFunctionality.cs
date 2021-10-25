using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CSGOGamble.Betting
{
    public static class mainFunctionality
    {
        //Dette er vores mainFunctionality, det er egnetlgit blot bare en statisk klasse med et uendeligt loop som kører hver 40 sekund, udover loopet indeholder klassen også nogle private funktioner til at udregne hash og substring som skal bruges når vi udregner udkommet af en runde
        //For at udregne udkommet af en runde bruger vi relativt rundenummer, dette nulstilles hver dag, så bruger vi en public key som alle kan se og en secret key som kun serveren kender
        //Der bliver genereret et nyt Secretkey og public hver dag kl 12, disse bliver indsat i vores roundKeys table inde i databasen.
        //Her opretter vi et tomt objekt som skal indeholde en entity af vores database, den bliver nulstillet hver runde så vi altid har den nyeste information
        private static CsgoBettingEntities1 databaseManager;

        //Her får vi contexten af vores websocket, vores websocket muliggøre live tovejskommunikation mellem server og klient, denne bruges f.eks. når vi starter en ny runde, opdatere brugerens mængde og sender resultatet af en runde
        //Alle kald fra websocket serveren til klienten er håndteret af javascript funktioner i klientens computer.
        private static IHubContext connectionManager = GlobalHost.ConnectionManager.GetHubContext<BettingHub>();

        //Vi deklarere en public DateTime som altid vil indeholde det eksakte tidspunkt hvor den næste runde bliver kørt
        //Denne variable kan tilgås udenfor klassen af hvilken som helst, og opdateres ved hver runde
        public static DateTime NextRoundTime { get { return nextRoundTime; } }

        //Dette er den private variable som kun denne klasse kan tilgå
        private static DateTime nextRoundTime;

        //Hver gang denne funktion køres oprettes en ny ufærdig runde i databasen og resultatet for den nuværende runde udregnes og sendes til alle klienter
        private static void RunBet()
        {
            //Sæt en ny database entity ind i vores databaseManager variabel
            databaseManager = new CsgoBettingEntities1();
            rounds round;
            int roundNumber;
            //Hent den nyeste runde fra databasen
            round = databaseManager.rounds.SingleOrDefault(r => r.ID == databaseManager.rounds.Max(x => x.ID));
            //Hvis der eksistrere en runde i databasen
            if (round != null)
            {
                roundNumber = round.number;
                //String er en sammenfletning af secret key, publickey og rundenummer. klassen rounds har en en til mange relation til roundKeys klassen
                //DVS at et roundKey kan have mange rouns
                string String = round.RoundKey.secret + "-" + round.RoundKey.@public + "-" + round.number;
                //Vi bruger funktionen GetStringSha256Hash til at hashe vores String variable til et sha256 hash, bagefter sørger vi for at bogstaverne er i lowercase
                string hashedString = GetStringSha256Hash(String).ToLower();
                //Denne linje er måske lidt rodet, men først tager vi en substring af vores hash fra karakter 0-8, som vi parser til en Int64, af denne int finder vi restværdien når vi dividere med 15. På den måde er vi sikre på at få et tal mellem 0-14
                long result = Int64.Parse(hashedString.Substring(0, 8), System.Globalization.NumberStyles.HexNumber) % 15;
                //Vi sætter rundens complete værdi i databasen til true (1 da det er en tinyint)
                round.complete = 1;
                //Vi sætter rundens outcome i databasen til det resultat vi har fået
                round.outcome = (int)result;
                //Vi udregner farven på resultatet, alle ulige tal fra 1-13 betyder at farven er counter (Et udtryk fra CSGO som betyder counterterrorist, det er det ene hold i spillet og har et mørkegråt symbol)
                string resultColor;
                //Nu sammenligner vi resultaatet med tre lister af tal og ser hvilken farve vores resultat er 
                if (new List<int> { 1, 3, 5, 7, 9, 11, 13 }.Contains((int)result))
                {
                    resultColor = "counter";
                }
                else if (new List<int> { 2, 4, 6, 8, 10, 12, 14 }.Contains((int)result))
                {
                    resultColor = "terrorist";
                }
                else 
                {
                    resultColor = "jackpot";
                }
                //Sæt rundens farve i databasen til farven vi har regnet os frem til
                round.color = resultColor;
                //Find alle bets fra runden
                IQueryable<bets> bets = databaseManager.bets.Where(x => x.roundID == round.ID);
                //Lav dem til en liste
                List<bets> betsList = bets.ToList();
                //Loop gennem alle bets i et foreach loop
                foreach (var bet in betsList)
                {
                    //I dette if statement tjekker vi om brugeren har gættet rigtigt, altså om brugeren gættede på den farve som runden blev.
                    //Hvis brugeren gættede rigtigt på counter eller terrorist får han dobbelt op hvis han gættede rigtigt på jackpot får han * 14
                    if (round.color == "counter" && bet.color == "counter")
                    {
                        bet.users.amount += bet.amount * 2;
                    } else if (round.color == "terrorist" && bet.color == "terrorist")
                    {
                        bet.users.amount += bet.amount * 2;
                    } else if (round.color == "jackpot" && bet.color == "jackpot")
                    {
                        //Sansyndligehden for at runden bliver jackpot er 1/15 da resultattallet 0 fra vores algoritme repræsentere jackpot
                        //Derfor får brugere som gættede rigtigt på jackpot * 14 tilbage
                        bet.users.amount += bet.amount * 14;
                    }
                    //Nu kalder vi funktionen sendNewAmount i vores websocket, det sender en besked videre til klientens browser så han kan se at han har vundet
                    connectionManager.Clients.User(bet.users.ID.ToString()).sendNewAmount(bet.users.amount);
                }
                //Nu opretter vi en liste med de sidste 100 runder
                List<rounds> last100 = databaseManager.rounds.OrderByDescending(x => x.ID).Take(100).ToList();
                var counter = 0;
                var terrorist = 0;
                var jackpot = 0;
                //Ligesom i HomeController.cs udregner vi andelen af udkom for de forskellige farver i de sidste 100 runder
                foreach (var round100 in last100)
                {
                    if(round100.color == "counter")
                    {
                        counter++;
                    } else if(round100.color == "terrorist")
                    {
                        terrorist++;
                    } else
                    {
                        jackpot++;
                    }
                }
                //Nu sender vi resultatet for runden til alle klienter som på nuværende tidspunkt er på siden, sendResult tager 4 argumenter
                //Resultatet fra runden (et tal fra 0-14), og tre variabler for andel af counter, terrorist og jackpot i de sidste 100 runder
                //Når brugerens browser modtager denne besked trigger det en javascript fuktion som sætter animationen af rouletten i gang
                //Først når animationen er færdig vil brugerens pengemængde opdateres i browseren, hvilket giver en illussion af at det først er der resultatet udregnes 
                //når de i virkeligheden er sket lang tid før på serveren
                connectionManager.Clients.All.sendResult(result.ToString(), counter, terrorist, jackpot);
            }
            //Hvis der ikke eksitere nogle runder i databasen sætter vi variablen roundNumber ved 0 så serveren ved at den skal starte ved runde nummber 0
            else
            {
                roundNumber = 0;
            }

            //Vi finder ID for det nyeste roundKey i databasen
            int? intIdt = databaseManager.roundkeys.Max(u => (int?)u.ID);
            roundkeys key;
            //Hvis der eksitere en roundkey i databasen
            if (intIdt != null)
            {
                //Vi finder den korrensponderende roundKey til det ID vi fandt før
                key = databaseManager.roundkeys.FirstOrDefault(u => u.ID == intIdt);
                //Her tjekker vi om datoen (dag, måned og år) for den nyeste roundKey er den nuværende dato
                if (key != null && key.date.Date == DateTime.UtcNow.Date)
                {
                }
                else
                {
                    //Hvis den ikke er det betyder det at vi skal oprette en ny roundKey i databasen da vi skal oprette en nu hver dag.
                    key = databaseManager.roundkeys.Add(new roundkeys { secret = GetRandomString(64), @public = GetRandomString(12) });
                    //Givet vi lige har oprettet en ny roundKey må runde nummeret være 0
                    roundNumber = 0;
                }
            }
            //Hvis der ikke eksitrere nogle roundKeys i databasen opretter vi blot en ny
            else
            {
                key = databaseManager.roundkeys.Add(new roundkeys { secret = GetRandomString(64), @public = GetRandomString(12) });
                roundNumber = 0;
            }
            //Vi sætter variablen nextRoundTime til 30 sekunder i fremtiden.
            nextRoundTime = DateTime.UtcNow.AddSeconds(30);
            //Nu tilføjer vi en ny runde med alt den information vi har, den sættes til completet = false fordi rundens udkom ikke er udregnet endnu. 
            rounds nextRound = databaseManager.rounds.Add(new rounds { complete = 0, keyID = key.ID, number = roundNumber+1, runtime = nextRoundTime, color = null});
            //Nu gemmer vi alle de ændringer vi har lavet i databasen i denne funktione, det er vigtigt for ellers får vi ikke de nyeste informationer når vi kalder databasen fra andre klasser
            databaseManager.SaveChanges();
            //Og vi smidder vores database entity væk
            databaseManager.Dispose();
            //Vi sender tidspunktet for den næste runde til alle klienter, det er essentielt for at hjemmesiden kan lave en nedtælling til næste runde.
            connectionManager.Clients.All.sendNext(nextRoundTime);
            //Nu kører vi funktionen WaitBet som en Task, waitbet tager vores nextRoundTime variabel
            Task.Run(() => { WaitBet(nextRoundTime); });
        }

        //WaitBet består egnetligt blot af et while loop som tjekker tidsdifferencen mellem nextRoundTime og det nuværende tidspunkt. Det vil bare sige at den venter i 40 sekunder
        private static void WaitBet(DateTime runtime)
        {
            while (DateTime.Compare(DateTime.UtcNow, runtime) <= 0)
            {

            }
            //Bagefter kører den hele RunBet igen, og sådan bliver det egnetligt bare ved
            RunBet();
        }

        //Vores GetRandomString som bruges til at generere secret og public key. Den retunere blot en tilfældig string af en bestemt længde.
        internal static string GetRandomString(int stringLength)
        {
            StringBuilder sb = new StringBuilder();
            int numGuidsToConcat = (((stringLength - 1) / 32) + 1);
            for (int i = 1; i <= numGuidsToConcat; i++)
            {
                sb.Append(Guid.NewGuid().ToString("N"));
            }

            return sb.ToString(0, stringLength);
        }

        //Vores GetStringSha256Hash funktion som retunere sha 256 hash for en string
        internal static string GetStringSha256Hash(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        //Vores Start funktion som kører funktionen RunBet()
        public static void Start()
        {
            RunBet();
        }
    }
}