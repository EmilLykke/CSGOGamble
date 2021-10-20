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
        public List<rounds> Last10 { get; set; }
        public IndexModel(string Username, double Amount, List<rounds> Last10, betModel Bets, int counter, int terrorist, int jackpot)
        {
            this.Username = Username;
            this.Amount = Amount;
            this.Bets = Bets;
            this.Last100 = new Last100Model(counter, terrorist, jackpot);
            this.Last10 = Last10;
        }
    }
}