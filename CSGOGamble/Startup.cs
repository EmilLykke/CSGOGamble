using CSGOGamble.Betting;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CSGOGamble.Startup))]
namespace CSGOGamble
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
            //mainFunctionality bettingObject = new mainFunctionality();
            //bettingObject.Start();
        }
    }
}
