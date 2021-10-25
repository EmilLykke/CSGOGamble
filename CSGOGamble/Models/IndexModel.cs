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
        public betList Bets { get; set; }
        public Last100Model Last100 { get; set; }
        public List<rounds> Last10 { get; set; }
        public IndexModel(string Username, double Amount, List<rounds> Last10, List<bets> Bets, int counter, int terrorist, int jackpot)
        {
            this.Username = Username;
            this.Amount = Amount;
            this.Bets = new betList(Bets);
            this.Last100 = new Last100Model(counter, terrorist, jackpot);
            this.Last10 = Last10;
        }
    }

    public class betList {
        public List<bets> Counter { get; set; }
        public List<bets> Terrorist { get; set; }
        public List<bets> Jackpot { get; set; }
        public double JackpotTotal { get; set; }
        public double TerroristTotal { get; set; }
        public double CounterTotal { get; set; }

        public betList(List<bets> bets)
        {
            this.Counter = bets.Where(x => x.color == "counter").ToList();
            this.Terrorist = bets.Where(x => x.color == "terrorist").ToList();
            this.Jackpot = bets.Where(x => x.color == "jackpot").ToList();
            this.TerroristTotal = this.Terrorist.Sum(x => x.amount);
            this.CounterTotal = this.Counter.Sum(x => x.amount);
            this.JackpotTotal = this.Jackpot.Sum(x => x.amount);
        }
    }
}