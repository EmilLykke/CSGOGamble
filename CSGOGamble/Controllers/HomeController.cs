using CSGOGamble.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSGOGamble.Controllers
{
    //Dette er vores homecontroller hvor vi håndtere requests til selve hjemmesiden, der er kun en side - Index - Så det er ret simpelt.
    public class HomeController : Controller
    {
        //Vi opretter et nyt entity af vores database
        private CsgoBettingEntities1 databaseManager;
        
        public HomeController()
        {
            databaseManager = new CsgoBettingEntities1();
        }

        //Her er vores actionresult når folk går ind på /Home/Index
        public ActionResult Index()
        {
            //I de første par linjer finder vi blot de informationer brugeren skal bruge når han/hun kommer ind på hjemmesiden, det er f.eks. hvor mange penge brugeren har, hvad de udkommet af de
            //sidste 10 runder er, udkommet af de sidste 100 runder, hvilke bets der er i den nuværende runde og mere. Al denne information bliver sat ind i en IndexModel som bliver sendt videre med View()
            //Find den nyeste runde
            rounds round = databaseManager.rounds.SingleOrDefault(r => r.ID == databaseManager.rounds.Max(z => z.ID));
            //Find bets for runden
            List<bets> bets = round.bets.ToList();
            //Find de sidste 100 bets
            List<messages> Last10Messages = databaseManager.messages.OrderByDescending(x => x.ID).Take(10).OrderBy(x => x.ID).ToList();
            List<rounds> last100 = this.databaseManager.rounds.Where(x => x.complete == 1).OrderByDescending(x => x.ID).Take(100).ToList();
            List<rounds> last10 = last100.OrderByDescending(x => x.ID).Take(10).OrderBy(x => x.ID).ToList();
            int counter = 0;
            int terrorist = 0;
            int jackpot = 0;
            //Udregn procentdel for hhv. counter, terrorist og jackpot for hvad de sidste 100 runder er blevet.
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
            //Lav en indexmode som tager et brugernavn, mængde af benge, sidste 10 bets, og procentdel af udkom.
            IndexModel model = new IndexModel(null, double.NaN, last10, bets, counter, terrorist, jackpot , Last10Messages);
            //Tjek om brugeren er logget ind
            if (Request.IsAuthenticated)
            {
                string id = User.Identity.Name;
                int idint = Int32.Parse(id);
                users user = this.databaseManager.users.SingleOrDefault(u => u.ID == idint);
                if (user != null)
                {
                    //Ændrer værdierne i indexmodel for username og amount
                    model.Username = user.username;
                    model.Amount = user.amount;
                    //Retuner index.cshtml view
                    return View(model);
                }
                else {
                    HttpContext.GetOwinContext().Authentication.SignOut();
                    return View(model);
                }
            } else
            {
                //Retuner index.cshtml view hvor username og amount er null da brugeren ikke er logget ind
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