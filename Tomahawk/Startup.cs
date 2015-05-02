using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Tomahawk.Startup))]
namespace Tomahawk
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
