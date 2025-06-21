﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Movies.Api.Auth
{
    public class ApiKeyAuthFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        public ApiKeyAuthFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName,out var extractedValue))
            {
                context.Result = new UnauthorizedObjectResult("Api key missing!");
                return;
            }

            var apiKey = _configuration["ApiKey"];

            if (extractedValue != apiKey)
            {
                context.Result = new UnauthorizedObjectResult("Invalid api Key!");
                return;
            }

           
        }
    }
}
