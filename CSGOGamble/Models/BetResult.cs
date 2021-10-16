using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSGOGamble.Models
{
    public class BetResult
    {
        public bool error = false;
        public double newAmount { get; set; }
        public BetResult(double newAmount)
        {
            this.newAmount = newAmount;
        } 
    }
}