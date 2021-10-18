using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Owin.Security.Providers.Steam;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using System;

namespace CSGOGamble
{
	public partial class Startup
	{
		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			// Enable the application to use a cookie to store information for the signed in user
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Account/Login")
			});
			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseSteamAuthentication(applicationKey: "9C9C2949AB53DCE4D84ED1B30DC2C19E");

        }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDataProtection()
			// This helps surviving a restart: a same app will find back its keys. Just ensure to create the folder.
			.PersistKeysToFileSystem(new DirectoryInfo("/Auth/Keys/"))
			// This helps surviving a site update: each app has its own store, building the site creates a new app
			.SetApplicationName("MyWebsite")
			.SetDefaultKeyLifetime(TimeSpan.FromDays(90));
		}
	}
}