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
            user model;
            if(Request.IsAuthenticated)
            {
                string id = User.Identity.Name;
                int idint = Int32.Parse(id);
                model = this.databaseManager.users.SingleOrDefault(u => u.ID == idint);
                return View(model);
            } else
            {
                return View();
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