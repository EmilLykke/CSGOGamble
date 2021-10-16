using CSGOGamble.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSGOGamble.Controllers
{
    public class ApiController : Controller
    {
        private CsgoBettingEntities1 databaseManager = new CsgoBettingEntities1();

        // GET: api'
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bet(double amount, string color)
        {
            if(Request.IsAuthenticated)
            {
                string userId = User.Identity.Name;
                int idint = Int32.Parse(userId);
                users user = this.databaseManager.users.SingleOrDefault(u => u.ID == idint);
                if (user != null)
                {
                    double userAmount = user.amount;
                    if (amount <= userAmount)
                    {
                        int? roundId = databaseManager.rounds.Max(u => (int?)u.ID);
                        rounds round = databaseManager.rounds.FirstOrDefault(u => u.complete == 0 && u.ID == roundId);
                        if (round != null)
                        {
                            this.databaseManager.bets.Add(new bets { amount = amount, roundID = round.ID });
                            user.amount = user.amount - amount;
                            databaseManager.SaveChanges();
                            return Json(new BetResult(user.amount));
                        }
                    }
                } else
                {
                    return Json(new PostError("Internal server error", "An internal server error occured, please contact website administrators at john@doe.com"));
                }
            }
            return Json(new PostError("Not signed in", "Please sign in to place a bet"));
        }
    }
}