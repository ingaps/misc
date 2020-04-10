using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DecimalNumbersExampleMvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);           
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //############## added for decimal numbers validation ######################
            ModelBinders.Binders.Add(typeof(decimal), new ModelBinder.DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new ModelBinder.DecimalModelBinder());
            //##########################################################################
        }
    }
}
