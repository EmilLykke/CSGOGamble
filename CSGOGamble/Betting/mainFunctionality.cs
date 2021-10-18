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
    public class mainFunctionality
    {
        private CsgoBettingEntities1 databaseManager = new CsgoBettingEntities1();

        IHubContext connectionManager = GlobalHost.ConnectionManager.GetHubContext<BettingHub>();

        private void RunBet()
        {
            rounds round;
            round = this.databaseManager.rounds.SingleOrDefault(r => r.ID == this.databaseManager.rounds.Max(x => x.ID));
            if (round != null)
            {
                string String = round.RoundKey.secret + "-" + round.RoundKey.@public + "-" + round.number;
                string hashedString = GetStringSha256Hash(String).ToLower();
                long result = Int64.Parse(hashedString.Substring(0, 8), System.Globalization.NumberStyles.HexNumber) % 15;
                connectionManager.Clients.All.sendResult(result.ToString());
                round.complete = 1;
                round.outcome = (int)result;
                IQueryable<bets> bets = this.databaseManager.bets.Where(x => x.roundID == round.ID);
                List<bets> betsList = bets.ToList();
                foreach (var bet in betsList)
                {
                    string resultColor;
                    if(new List<int> {1, 3, 5, 7, 9, 11, 13}.Contains((int)result) && bet.color == "counter")
                    {
                        resultColor = "counter";
                    } else if (new List<int> {2, 4, 6, 8, 10, 12, 14}.Contains((int)result) && bet.color == "terrorist")
                    {
                        resultColor = "terrorist";
                    } else if ((int)result == 0 && bet.color == "jackpot")
                    {
                        resultColor = "jackpot";
                    }
                }
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
                    key = this.databaseManager.roundkeys.Add(new roundkeys { secret = GetRandomString(64), @public = GetRandomString(12) });
                }
            }
            else
            {
                key = this.databaseManager.roundkeys.Add(new roundkeys { secret = GetRandomString(64), @public = GetRandomString(12) });
            }
            DateTime nextRoundTime = DateTime.UtcNow.AddSeconds(40);
            rounds nextRound = this.databaseManager.rounds.Add(new rounds { complete = 0, keyID = key.ID, number = round.number+1, runtime = nextRoundTime});
            this.databaseManager.SaveChanges();
            connectionManager.Clients.All.sendNext(nextRoundTime);
            Task.Run(() => { this.WaitBet(nextRoundTime); });
        }

        private void WaitBet(DateTime runtime)
        {

            Debug.WriteLine(DateTime.Compare(DateTime.UtcNow, runtime));
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
        public void Start()
        {
            WaitBet(DateTime.UtcNow.AddSeconds(40));
        }
    }
}