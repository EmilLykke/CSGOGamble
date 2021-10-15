using CSGOGamble.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace CSGOGamble
{
    public class BettingHub : Hub
    {
        public void SendAllBets(string bets)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.sendBets(bets);
        }
    }
}