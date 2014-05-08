using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WilcoCon.Startup))]
namespace WilcoCon
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
