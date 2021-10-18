using CSGOGamble.Betting;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using System.Globalization;
using System.Threading.Tasks;

[assembly: OwinStartupAttribute(typeof(CSGOGamble.Startup))]
namespace CSGOGamble
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            var idProvider = new CustomUserIdProvider();
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);
            ConfigureAuth(app);
            app.MapSignalR();
            mainFunctionality bettingObject = new mainFunctionality();
            Task.Run(() => { bettingObject.Start(); });
        }
    }
}
