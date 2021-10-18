using CSGOGamble.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSGOGamble.Controllers
{
    public class ApiController : Controller
    {
        private CsgoBettingEntities1 databaseManager = new CsgoBettingEntities1();
        IHubContext connectionManager = GlobalHost.ConnectionManager.GetHubContext<BettingHub>();


        [HttpPost]
        public ActionResult Bet(string amountString, string color)
        {
            double amount = Double.Parse(amountString, CultureInfo.InvariantCulture);
            Debug.WriteLine("bet received");
            Debug.WriteLine(amount);
            Debug.WriteLine(User.Identity.Name);
            if(Request.IsAuthenticated)
            {
                string userId = User.Identity.Name;
                int idint = Int32.Parse(userId);
                users user = this.databaseManager.users.SingleOrDefault(u => u.ID == idint);
                if (user != null)
                {
                    double userAmount = user.amount;
                    if (amount > 0)
                    {
                        if (amount <= userAmount)
                        {
                            int? roundId = databaseManager.rounds.Max(u => (int?)u.ID);
                            rounds round = databaseManager.rounds.FirstOrDefault(u => u.complete == 0 && u.ID == roundId);
                            if (round != null)
                            {
                                if (color == "counter" || color == "terrorist" || color == "jackpot")
                                {
                                    this.databaseManager.bets.Add(new bets { amount = amount, roundID = round.ID, color = color, userID = user.ID });
                                    user.amount = user.amount - amount;
                                    databaseManager.SaveChanges();
                                    connectionManager.Clients.All.sendNewBet(user.username, amount, color);
                                    return Json(new BetResult(user.amount));
                                }
                            }
                        }
                    }
                }
                return Json(new PostError("Internal server error", "An internal server error occured, please contact website administrators at john@doe.com"));

            }
            return Json(new PostError("Not signed in", "Please sign in to place a bet"));
        }
    }
}