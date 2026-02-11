using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ONnegha_Mefisto_Theatre_Company.Startup))]
namespace ONnegha_Mefisto_Theatre_Company
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
