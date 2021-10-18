using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSGOGamble.Models
{
    public class Last100Model
    {
        public int counter { get; set; }
        public int terrorist { get; set; }
        public int jackpot { get; set; }
        public Last100Model(int counter, int terrorist, int jackpot)
        {
            this.counter = counter;
            this.terrorist = terrorist;
            this.jackpot = jackpot;
        }
    }
}