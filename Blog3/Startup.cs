using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Blog3.Startup))]
namespace Blog3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
