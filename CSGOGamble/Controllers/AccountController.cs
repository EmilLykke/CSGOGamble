//-----------------------------------------------------------------------    
// <copyright file="AccountController.cs" company="None">  
//   Copyright (c) Allow to distribute this code.    
// </copyright>  
// <author>Asma Khalid</author>  
//-----------------------------------------------------------------------    
namespace CSGOGamble.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Claims;
    using System.Web;
    using System.Web.Mvc;
    using CSGOGamble;
    using CSGOGamble.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security;
    using System.Threading.Tasks;

    

    //Denne controller håndtere alle logins. Den håndtere alle kald til /Account
    public class AccountController : Controller
    {
        #region Private Properties    
        
        //Opret en ny entity af vores database
        private CsgoBettingEntities1 databaseManager = new CsgoBettingEntities1();

        public AccountController() {}
        #endregion

        #region Login methods    

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //Dette actionResult håndtere kaldt til /Account/LoginSteam over post. Den tager to værdier, en provider og returnUrl. Provideren vil altid være steam.
        public ActionResult LoginSteam(string provider, string returnUrl)
        {
            //Vi prøver at retunere et nyt ChallengeResult som redirecter brugeren til steam med de korrekte informaitoner. Denne redirect er håndteret af et library kaldet OwinProviders
            try
            {
                return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { returnUrl = returnUrl }));

            } catch (Exception e)
            {
                throw e;
            }
        }

        [AllowAnonymous]
        //Når brugeren bliver sendt tilbage til hjemmesiden fra steam af kommer de til Account/ExternalLoginCallback med get.
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            //Vi kalder funktionen authenticcationManager.GetExternalLoginInfoAsync. authenticationManager er blot en variable hvor get retunere HttpContext.GetOwinContext().Authentication, som indeholder authenticering af en httprequest
            //Ved hjælp af funktionen GetExternalLoginInfoAsync får vi den info som steam har sendt med tilbage. Owin, det library vi bruger, sørger for verificering af datatene med steams server
            var loginInfo = await authenticationManager.GetExternalLoginInfoAsync();
            //Vi tjekker lige om brugeren allerede er logget ind, så sender vi dem bare tilbage til Index
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            } 
            //Ellers begynder vi at logge dem ind.
            else if(loginInfo.ExternalIdentity.IsAuthenticated)
            {
                //Vi ser om dataabasen allerede indeholder en bruger med det brugernavn
                var user = this.databaseManager.users.SingleOrDefault(u => u.username == loginInfo.DefaultUserName && u.steam == loginInfo.Login.ProviderKey);
                if(user != null)
                {
                    this.SignInUser(user.ID.ToString(), false);
                    return this.RedirectToAction("Index", "Home");
                }
                else
                {
                    user = this.databaseManager.users.Add(new users { username = loginInfo.DefaultUserName, steam = loginInfo.Login.ProviderKey, amount = 500 });
                    // Login In.    
                    this.SignInUser(user.ID.ToString(), false);
                    // Info.    
                    this.databaseManager.SaveChanges();
                    return this.RedirectToAction("Index", "Home");
                }

            }
            return this.RedirectToAction("Login");
        }
        
        #endregion
        #region Helpers    
        #region Sign In method.    
        /// <summary>  
        /// Sign In User method.    
        /// </summary>  
        /// <param name="username">Username parameter.</param>  
        /// <param name="isPersistent">Is persistent parameter.</param>  
        /// 
        private const string XsrfKey = "asdijasoidjwasdnacsaoiqw00fda";

        private IAuthenticationManager authenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private void SignInUser(string id, bool isPersistent)
        {
            // Initialization.    
            var claims = new List<Claim>();
            try
            {
                // Setting    
                claims.Add(new Claim(ClaimTypes.Name, id));
                claims.Add(new Claim("ActiveBet", "false"));

                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                // Sign In.    
                authenticationManager.SignIn(new AuthenticationProperties() {IsPersistent = isPersistent }, claimIdenties);
                Session["ActiveBet"] = false;
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            authenticationManager.SignOut();
            return this.RedirectToAction("Index", "Home");
        }
        #endregion
        #region Redirect to local method.    
        /// <summary>  
        /// Redirect to local method.    
        /// </summary>  
        /// <param name="returnUrl">Return URL parameter.</param>  
        /// <returns>Return redirection action</returns>  
        private ActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                // Verification.    
                if (Url.IsLocalUrl(returnUrl))
                {
                    // Info.    
                    return this.Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
            // Info.    
            return this.RedirectToAction("Index", "Home");
        }
        #endregion
        #endregion

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {

            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if(UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }

                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }

        }
    }
}