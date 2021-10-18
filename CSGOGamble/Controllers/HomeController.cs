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
            rounds round = this.databaseManager.rounds.SingleOrDefault(r => r.ID == this.databaseManager.rounds.Max(x => x.ID));
            betModel bets = new betModel(30, 10.0, new List<bet> {new bet("aske", 10.0, "dice"), new bet("aske", 10.0, "dice")});
            IndexModel model = new IndexModel(null, double.NaN, bets, round?.runtime);

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