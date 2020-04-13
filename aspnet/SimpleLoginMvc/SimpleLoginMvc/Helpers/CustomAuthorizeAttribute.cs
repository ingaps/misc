using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleLoginMvc.Helpers
{


    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedroles;
        public CustomAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            if (HttpContext.Current.Session["UserName"] != null)
            {

                if (this.allowedroles.Length == 1)
                {
                    if (HttpContext.Current.Session["UserRole"].ToString() == allowedroles[0])
                    {
                        authorize = true;
                    }
                }
                else if (this.allowedroles.Length == 2)
                {
                    foreach (string role in allowedroles)
                    {
                        if (HttpContext.Current.Session["UserRole"].ToString() == role)
                        {
                            authorize = true;
                            break;
                        }
                    }
                }
                /*  else if (this.allowedroles.Length > 2)
                  {
                      foreach (string role in allowedroles)
                      {
                          //if there were more roles
                          //compare to roles stored in Session[]
                          //if ok authorize = true
                          //not in use now so not implemeted, if necesarry, can be implemeted     
                          break;
                      }
                  }   */
            }

            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}
