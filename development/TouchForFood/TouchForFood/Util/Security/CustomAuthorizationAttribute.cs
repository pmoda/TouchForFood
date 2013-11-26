using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TouchForFood.Util.Security
{
    [Serializable]
    [Flags]
    // Enum to define the different roles available
    // Works as a bit-shifting operation to the value on the left-hand side, in binary.
    // Allows us to perform comparisons to allow different roles into different controllers
    public enum SiteRoles
    {
        Admin = 1 << 0, // 0001, shifted 0 times to the left, is 0001, which is 1 in decimal
        Customer = 1 << 1, // 0001, shifted 1 time to the left, is 0010, which is 2 in decimal
        Restaurant = 1 << 2, // 0001, shifted 2 times to the left, is 0100, which is 4 in decimal 
        Developer = 1 << 3 // 0001, shifted 3 times to the left, is 1000, which is 8 in decimal
    }

    // Custom authorization class that allows us to finely tune the access permissions to 
    // different controller actions
    // http://schotime.net/blog/index.php/2009/02/17/custom-authorization-with-aspnet-mvc/
    public class CustomAuthorizationAttribute : AuthorizeAttribute
    {
        // the "new" must be used here because we are hiding
        // the Roles property on the underlying class
        public new SiteRoles Roles;

        //private bool FailedRolesAuth = false; // Goes with the commented out function below

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            string[] users = Users.Split(',');

            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            SiteRoles role = (SiteRoles)(-1);
            
            // Use cookie information (from the integrated FormsAuthentication) to fetch the username and be logged in
            // through the session as well
            if (httpContext.Session["role"] == null)
            {
                httpContext.Session["role"] = Util.User.UserUtil.getAuthenticatedUser(httpContext).user_role;
                role = (SiteRoles)httpContext.Session["role"];
            }
            else
            {
                role = (SiteRoles)httpContext.Session["role"];
            }

            if (Roles != 0 && ((Roles & role) != role))
                return false;

            return true;
        }

        /* Commented out for now. We might wanna use a custom page as an end-all solution if you try to access
           something you're not supposed to
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (FailedRolesAuth)
            {
                filterContext.Result = new ViewResult { ViewName = “NotAuth” };
            }
        }*/
    }




}