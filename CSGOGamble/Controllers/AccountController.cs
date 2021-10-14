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

    

    /// <summary>  
    /// Account controller class.    
    /// </summary>  
    public class AccountController : Controller
    {
        #region Private Properties    
        /// <summary>  
        /// Database Store property.    
        /// </summary>  
        private CsgoBettingEntities1 databaseManager = new CsgoBettingEntities1();
        #endregion
        #region Default Constructor    
        /// <summary>  
        /// Initializes a new instance of the <see cref="AccountController" /> class.    
        /// </summary>  
        public AccountController()
        {
        }
        #endregion
        #region Login methods    

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginSteam(string provider, string returnUrl)
        {
            try
            {
                return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { returnUrl = returnUrl }));

            } catch (Exception e)
            {
                throw e;
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await authenticationManager.GetExternalLoginInfoAsync();
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            } else if(loginInfo.ExternalIdentity.IsAuthenticated)
            {

                var user = this.databaseManager.users.SingleOrDefault(u => u.username == loginInfo.DefaultUserName && u.steam == loginInfo.Login.ProviderKey);
                if(user != null)
                {
                    this.SignInUser(user.ID.ToString(), false);
                    return this.RedirectToAction("Index", "Home");
                }
                else
                {
                    user = this.databaseManager.users.Add(new user { username = loginInfo.DefaultUserName, steam = loginInfo.Login.ProviderKey, amount = 0 });
                    // Login In.    
                    this.SignInUser(user.ID.ToString(), false);
                    // Info.    
                    this.databaseManager.SaveChanges();
                    return this.RedirectToAction("Index", "Home");
                }

            }
            return this.RedirectToAction("Login");
        }
        /// <summary>  
        /// GET: /Account/Login    
        /// </summary>  
        /// <param name="returnUrl">Return URL parameter</param>  
        /// <returns>Return login view</returns>  
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            try
            {
                // Setting.    
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign Out.    
                authenticationManager.SignOut();
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
            // Info.    
            return this.RedirectToAction("Home", "Index");
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
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                // Sign In.    
                authenticationManager.SignIn(new AuthenticationProperties() {IsPersistent = isPersistent }, claimIdenties);
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
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