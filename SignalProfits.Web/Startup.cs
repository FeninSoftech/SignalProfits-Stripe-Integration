using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BrickandCherry.Web.Startup))]
namespace BrickandCherry.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
