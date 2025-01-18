using Business.Exceptions;
using Business.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CliChat.Filters
{
    public class RateLimitFilterAttribute : ActionFilterAttribute, IFilterMetadata
    {
        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new ClientSideException(System.Net.HttpStatusCode.BadRequest, "What the fuck is with your ip", "IPAddress");
            }

            // Check if IP is rate-limited or banned
            if (RateLimitingService.IsRateLimitedAsync(ipAddress))
            {
                throw new TooManyRequestsException();
            }

            // Record the request if not rate-limited
            RateLimitingService.RecordRequest(ipAddress);

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
