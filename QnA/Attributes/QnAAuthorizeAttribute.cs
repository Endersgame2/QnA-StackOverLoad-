using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QnA.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class QnAAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _role;
        public QnAAuthorizeAttribute(string role)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var roles = session?.GetString("Roles");
            var userId = session?.GetInt32("UserId");
            if (userId == null || userId == 0)
            {
                context.Result = new RedirectToActionResult("Login", "Account", "");
                return;
            }
            if (roles == null || !roles.Contains(_role))
            {
                context.Result = new RedirectToActionResult("Index", "Home", "");
                return;
            }
        }
    }
}
