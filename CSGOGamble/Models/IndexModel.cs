using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSGOGamble.Models
{
    public class IndexModel
    {
        public string Username { get; set; }
        public double Amount { get; set; }
        public betModel Bets { get; set; }
        public Last100Model Last100 { get; set; }
        public DateTime? NextRound { get; set; }
        public IndexModel(string Username, double Amount, betModel Bets, DateTime? NextRound, int counter, int terrorist, int jackpot)
        {
            this.Username = Username;
            this.Amount = Amount;
            this.Bets = Bets;
            this.NextRound = NextRound;
            this.Last100 = new Last100Model(counter, terrorist, jackpot);
        }
    }
}