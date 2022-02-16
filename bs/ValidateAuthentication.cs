using Microsoft.AspNetCore.Authentication;

namespace bs
{
    internal class ValidateAuthentication : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.User.Identity.IsAuthenticated)
                await next(context);
            else
                await context.ChallengeAsync();
        }
    }
}
