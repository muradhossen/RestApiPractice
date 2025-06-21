using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Movies.Api.Auth
{
    public class AdminAuthRequirment : IAuthorizationHandler, IAuthorizationRequirement
    {
        private readonly string _apiKey;

        public AdminAuthRequirment(string apiKey)
        {
            this._apiKey = apiKey;
        }
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (context.User.HasClaim(AuthConstants.AdminClaimName, "true"))
            {
                context.Succeed(this);
                return Task.CompletedTask;
            }


            var httpContext = context.Resource as HttpContext;
            if (httpContext is null)
            {
                return Task.CompletedTask;
            }

            if (!httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedValue))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            
            if (extractedValue != _apiKey)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(this);
            return Task.CompletedTask;
        }
    }
}
