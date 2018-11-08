using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SupFile2.Startup))]
namespace SupFile2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
