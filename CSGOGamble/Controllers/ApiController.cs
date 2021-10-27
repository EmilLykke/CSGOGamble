using CSGOGamble.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace CSGOGamble.Controllers
{
    public class ApiController : Controller
    {
        private CsgoBettingEntities1 databaseManager;
        IHubContext connectionManager = GlobalHost.ConnectionManager.GetHubContext<BettingHub>();
        public ApiController() {
            databaseManager = new CsgoBettingEntities1();
        }

        private IAuthenticationManager authenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [HttpPost]
        public ActionResult Bet(string amountString, string color)
        {
            double amount = Double.Parse(amountString, CultureInfo.InvariantCulture);
            if (Request.IsAuthenticated)
            {
                if(Session["ActiveBet"] == null)
                {
                    Session["ActiveBet"] = false;
                }
                if ((bool)Session["ActiveBet"] != true)
                {
                    Session["ActiveBet"] = true;
                    string userId = User.Identity.Name;
                    int idint = Int32.Parse(userId);
                    users user = this.databaseManager.users.SingleOrDefault(u => u.ID == idint);
                    if (user != null)
                    {
                        double userAmount = user.amount;
                        if (amount > 0)
                        {
                            if (Math.Round(amount, 2) == amount)
                            {
                                int? roundId = databaseManager.rounds.Max(u => (int?)u.ID);
                                rounds round = databaseManager.rounds.FirstOrDefault(u => u.complete == 0 && u.ID == roundId);

                                if (round != null)
                                {
                                    if (amount <= Math.Round(userAmount, 2))
                                    {
                                        if (color == "counter" || color == "terrorist" || color == "jackpot")
                                        {
                                            this.databaseManager.bets.Add(new bets { amount = amount, roundID = round.ID, color = color, userID = user.ID });
                                            Debug.WriteLine(user.amount);
                                            user.amount = Math.Round(user.amount - amount, 2);
                                            var changes = this.databaseManager.SaveChanges();
                                            connectionManager.Clients.All.sendNewBet(user.username, amount, color);
                                            Session["ActiveBet"] = false;
                                            return Json(new BetResult(user.amount));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Session["ActiveBet"] = false;
                    return Json(new PostError("Internal server error", "An internal server error occured, please contact website administrators at john@doe.com"));
                }
                return Json(new PostError("Multiple bets", "It seems that you have made multiple bets too fast, please wait a few seconds."));
            }
            return Json(new PostError("Not signed in", "Please sign in to place a bet"));
        }
    
        [HttpPost]
        public ActionResult Message(string message)
        {
            if(Request.IsAuthenticated)
            {
                int userId = Int32.Parse(User.Identity.Name);
                users user = databaseManager.users.SingleOrDefault(x => x.ID == userId);
                if(user != null)
                {
                    if (!String.IsNullOrEmpty(message))
                    {
                        if (message.Length < 500)
                        {
                            databaseManager.messages.Add(new messages { message = message, userID = user.ID });
                            connectionManager.Clients.All.sendNewMessage(user.username, message);
                            var changes = this.databaseManager.SaveChanges();
                            return Content("success");
                        }
                        return Json(new PostError("Message too long", "Only 500 characters allowed"));
                    }
                    return Json(new PostError("Empty message", "Please enter a message"));
                }
            }
            return Json(new PostError("Not signed in", "Please sign in to send a message"));
        }
    }
}