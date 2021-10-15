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
        public DateTime NextRound { get; set; }
        public IndexModel(string Username, double Amount, betModel Bets, DateTime NextRound)
        {
            this.Username = Username;
            this.Amount = Amount;
            this.Bets = Bets;
            this.NextRound = NextRound;
        }
    }
}