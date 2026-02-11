using System.Web;
using System.Web.Mvc;

namespace ONnegha_Mefisto_Theatre_Company
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
