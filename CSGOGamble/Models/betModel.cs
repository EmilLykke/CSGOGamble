using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSGOGamble.Models
{
    public class betModel
    {

        public int roundId { get; set; }
        public float totalAmount { get; set; }
        public List<bet> bets { get; set; }
        public betModel (int roundId, float totalAmount, List<bet> bets)
        {
            this.bets = bets;
            this.totalAmount = totalAmount;
            this.roundId = roundId;
        }
    }

    public class bet
    {
        public string user_name { get; set; }
        public float amount { get; set; }
        public string color { get; set; }
        public bet(string user_name, float amount, string color)
        {
            this.amount = amount;
            this.user_name = user_name;
            this.color = color;
        }
    }
}