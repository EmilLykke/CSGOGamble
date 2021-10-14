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
        private CsgoBettingEntities1 databaseManager = new CsgoBettingEntities1();
        private void RunBet()
        {
            int keyId = databaseManager.roundkeys.Max(x => x.ID);
            roundkey key = databaseManager.roundkeys.FirstOrDefault(x => x.ID == keyId);
            if(key != null)
            {
                if (key.date == DateTime.UtcNow.Date)
                {

                }
            }
        }
        public void SendAllBets(List<betModel> bets)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(bets);
        }
    }
}