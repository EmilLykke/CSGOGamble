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
        private static CsgoBettingEntities1 databaseManager;

        private static IHubContext connectionManager = GlobalHost.ConnectionManager.GetHubContext<BettingHub>();
        public static DateTime NextRoundTime { get { return nextRoundTime; } }

        private static DateTime nextRoundTime;
        private static void RunBet()
        {
            databaseManager = new CsgoBettingEntities1();
            rounds round;
            int roundNumber;
            round = databaseManager.rounds.SingleOrDefault(r => r.ID == databaseManager.rounds.Max(x => x.ID));
            if (round != null)
            {
                roundNumber = round.number;
                string String = round.RoundKey.secret + "-" + round.RoundKey.@public + "-" + round.number;
                string hashedString = GetStringSha256Hash(String).ToLower();
                long result = Int64.Parse(hashedString.Substring(0, 8), System.Globalization.NumberStyles.HexNumber) % 15;
                round.complete = 1;
                round.outcome = (int)result;
                string resultColor;
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
                round.color = resultColor;
                IQueryable<bets> bets = databaseManager.bets.Where(x => x.roundID == round.ID);
                List<bets> betsList = bets.ToList();
                foreach (var bet in betsList)
                {
                    users user = databaseManager.users.Single(x => x.ID == bet.userID);
                    Debug.WriteLine("User amount: " + user.amount);
                    Debug.WriteLine("Bet amount: " + bet.amount);
                    if (round.color == "counter" && bet.color == "counter")
                    {
                        bet.users.amount += bet.amount * 2;
                    } else if (round.color == "terrorist" && bet.color == "terrorist")
                    {
                        bet.users.amount += bet.amount * 2;
                    } else if (round.color == "jackpot" && bet.color == "jackpot")
                    {
                        bet.users.amount += bet.amount * 14;
                    }
                    connectionManager.Clients.User(bet.users.ID.ToString()).sendNewAmount(bet.users.amount);
                }
                List<rounds> last100 = databaseManager.rounds.OrderByDescending(x => x.ID).Take(100).ToList();
                var counter = 0;
                var terrorist = 0;
                var jackpot = 0;
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
                connectionManager.Clients.All.sendResult(result.ToString(), counter, terrorist, jackpot);
            }
            else
            {
                roundNumber = 0;
            }

            int? intIdt = databaseManager.roundkeys.Max(u => (int?)u.ID);
            roundkeys key;
            if (intIdt != null)
            {
                key = databaseManager.roundkeys.FirstOrDefault(u => u.ID == intIdt);
                if (key != null && key.date.Date == DateTime.UtcNow.Date)
                {
                }
                else
                {
                    key = databaseManager.roundkeys.Add(new roundkeys { secret = GetRandomString(64), @public = GetRandomString(12) });
                    roundNumber = 0;
                }
            }
            else
            {
                key = databaseManager.roundkeys.Add(new roundkeys { secret = GetRandomString(64), @public = GetRandomString(12) });
                roundNumber = 0;
            }
            nextRoundTime = DateTime.UtcNow.AddSeconds(40);
            rounds nextRound = databaseManager.rounds.Add(new rounds { complete = 0, keyID = key.ID, number = roundNumber+1, runtime = nextRoundTime, color = null});
            databaseManager.SaveChanges();
            databaseManager.Dispose();
            connectionManager.Clients.All.sendNext(nextRoundTime);
            Task.Run(() => { WaitBet(nextRoundTime); });
        }

        private static void WaitBet(DateTime runtime)
        {
            while (DateTime.Compare(DateTime.UtcNow, runtime) <= 0)
            {

            }
            RunBet();
        }

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
        public static void Start()
        {
            RunBet();
        }
    }
}