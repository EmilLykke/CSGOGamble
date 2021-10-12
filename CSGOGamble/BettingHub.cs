using CSGOGamble.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSGOGamble
{
    public class BettingHub : Hub
    {
        public void SendAllBets(List<betModel> bets)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(bets);
        }
    }
}