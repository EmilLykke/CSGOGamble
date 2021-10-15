using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace CSGOGamble.Betting
{
    public class mainFunctionality
    {
        private CsgoBettingEntities1 databaseManager = new CsgoBettingEntities1();
        private void RunBet()
        {
            int? intIdt = databaseManager.roundkeys.Max(u => (int?)u.ID);
            string secret_key;
            string client_key;
            int roundNumber;
            roundkey key;
            if (intIdt != null)
            {
                key = databaseManager.roundkeys.FirstOrDefault(u => u.ID == intIdt);
                if(key != null && key.date.Date == DateTime.UtcNow.Date)
                {
                    secret_key = key.secret;
                    client_key = key.@public;
                } else
                {
                    key = this.databaseManager.roundkeys.Add(new roundkey { secret = GetRandomString(64), @public = GetRandomString(12) });
                    this.databaseManager.SaveChanges();
                    secret_key = key.secret;
                    client_key = key.@public;
                }
            }
            else
            {
                key = this.databaseManager.roundkeys.Add(new roundkey { secret = GetRandomString(64), @public = GetRandomString(12) });
                this.databaseManager.SaveChanges();
                secret_key = key.secret;
                client_key = key.@public;
            }

            round round = this.databaseManager.rounds.SingleOrDefault(r => r.ID == this.databaseManager.rounds.Where(x => x.keyid == key.ID).Max(x => x.ID));
            if (round != null)
            {
                roundNumber = round.number + 1;
            }
            else
            {
                roundNumber = 1;
            }
            string String = secret_key + "-" + client_key + "-" + roundNumber;
            string hashedString = GetStringSha256Hash(String).ToLower();
            long result = Int64.Parse(hashedString.Substring(0, 8), System.Globalization.NumberStyles.HexNumber) % 15;
            WaitBet(DateTime.UtcNow.AddSeconds(40));
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