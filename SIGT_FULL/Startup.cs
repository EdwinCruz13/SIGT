using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SIGT_FULL.Startup))]
namespace SIGT_FULL
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
            app.MapSignalR(); //can take path also see overloads...
        }

    }
}
