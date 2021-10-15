using CSGOGamble.Betting;
using Microsoft.Owin;
using Owin;
using System.Threading.Tasks;

[assembly: OwinStartupAttribute(typeof(CSGOGamble.Startup))]
namespace CSGOGamble
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
            mainFunctionality bettingObject = new mainFunctionality();
            Task.Run(() => { bettingObject.Start(); });
        }
    }
}
