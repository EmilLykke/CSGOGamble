using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSGOGamble.Controllers
{
    public class ApiController : Controller
    {
        // GET: api'
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bet()
        {
            return View();
        }
    }
}