using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NewWebsite.Filters;

public class SessionAuthorizeAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var username = context.HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
        }
    }
}
