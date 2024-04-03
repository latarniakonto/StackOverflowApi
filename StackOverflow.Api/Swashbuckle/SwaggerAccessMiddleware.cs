using Microsoft.AspNetCore.Identity;
using StackOverflow.Infrastructure.Authentication;
using StackOverflow.Infrastructure.Authorization;

namespace StackOverflow.Api.Swashbuckle
{
    public class SwaggerAccessMiddleware
    {
        private readonly RequestDelegate _next;

        public SwaggerAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, UserManager<TagUser> userManager)
        {
            if (context == null || userManager == null)
                throw new InvalidOperationException("HttpContext or UserManager is missing");

            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    var returnUrl = context.Request.Path + context.Request.QueryString;
                    var accessDeniedUrl = $"/Account/AccessDenied?ReturnUrl={Uri.EscapeDataString(returnUrl)}";
                    context.Response.Redirect(accessDeniedUrl);
                    return;
                }

                var user = await userManager.GetUserAsync(context.User);
                if (user == null || !await userManager.IsInRoleAsync(user, UserRoles.Admin))
                {
                    var returnUrl = context.Request.Path + context.Request.QueryString;
                    var accessDeniedUrl = $"/Account/AccessDenied?ReturnUrl={Uri.EscapeDataString(returnUrl)}";
                    context.Response.Redirect(accessDeniedUrl);
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class SwaggerAccessMiddlewareExtensions
    {
        public static IApplicationBuilder UseSwaggerAccessMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<SwaggerAccessMiddleware>();
            return app;
        }
    }
}
