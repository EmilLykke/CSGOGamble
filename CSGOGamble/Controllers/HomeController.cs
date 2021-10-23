using CSGOGamble.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSGOGamble.Controllers
{
    public class HomeController : Controller
    {
        private CsgoBettingEntities1 databaseManager = new CsgoBettingEntities1();

        public ActionResult Index()
        {
            rounds round = databaseManager.rounds.SingleOrDefault(r => r.ID == databaseManager.rounds.Max(z => z.ID));
            List<bets> bets = databaseManager.bets.Where(x => x.roundID == round.ID).ToList();
            List<rounds> last100 = this.databaseManager.rounds.Where(x => x.complete == 1).OrderByDescending(x => x.ID).Take(100).ToList();
            List<rounds> last10 = last100.OrderByDescending(x => x.ID).Take(10).OrderBy(x => x.ID).ToList();
            int counter = 0;
            int terrorist = 0;
            int jackpot = 0;
            foreach (var round100 in last100)
            {
                if (round100.color == "counter")
                {
                    counter++;
                }
                else if (round100.color == "terrorist")
                {
                    terrorist++;
                }
                else
                {
                    jackpot++;
                }
            }
            IndexModel model = new IndexModel(null, double.NaN, last10, bets, counter, terrorist, jackpot);
            if (Request.IsAuthenticated)
            {
                string id = User.Identity.Name;
                int idint = Int32.Parse(id);
                users user = this.databaseManager.users.SingleOrDefault(u => u.ID == idint);
                model.Username = user.username;
                model.Amount = user.amount;
                return View(model);
            } else
            {
                return View(model);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}