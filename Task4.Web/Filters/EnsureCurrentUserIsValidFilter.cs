using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Task4.Web.Services;

namespace Task4.Web.Filters
{
    public sealed class EnsureCurrentUserIsValidFilter(
        CurrentUserGuard guard) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (IsAnonymousAllowed(context))
            {
                await next();
                return;
            }

            if (!IsAuthenticated(context))
            {
                await next();
                return;
            }

            if (await guard.IsValidAsync(
                    context.HttpContext.User,
                    context.HttpContext.RequestAborted))
            {
                await next();
                return;
            }

            await RejectInvalidUserAsync(context);
        }

        private static bool IsAuthenticated(ActionExecutingContext context)
            => context.HttpContext.User.Identity?.IsAuthenticated == true;

        private static bool IsAnonymousAllowed(ActionExecutingContext context)
            => context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null;

        private static async Task RejectInvalidUserAsync(ActionExecutingContext context)
        {
            await context.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            context.Result = new RedirectToActionResult(
                "Login",
                "Auth",
                null);
        }
    }
}