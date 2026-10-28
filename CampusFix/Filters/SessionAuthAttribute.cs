using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CampusFix.Filters;

/// <summary>
/// Simple session based authorization filter.
/// Usage: [SessionAuth] or [SessionAuth(Role = "Admin")]
/// </summary>
public class SessionAuthAttribute : ActionFilterAttribute
{
    public string? Role { get; set; }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;

        if (session.GetInt32("UserId") == null)
        {
            context.Result = new RedirectToActionResult("Login", "Account",
                new { returnUrl = context.HttpContext.Request.Path.ToString() });
            return;
        }

        if (Role != null && session.GetString("Role") != Role)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
        }
    }
}
