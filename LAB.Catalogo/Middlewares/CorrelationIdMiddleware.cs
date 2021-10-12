using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace LAB.Catalogo.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string _correlationIdKey = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string correlationId = string.Empty;
            if (context.Request.Headers.TryGetValue(_correlationIdKey, out StringValues correlationIds))
            {
                correlationId = correlationIds.FirstOrDefault(x => x.Equals(correlationId));
            }
            else
            {
                correlationId = Guid.NewGuid().ToString();
            }
            context.Request.Headers.Add(_correlationIdKey, correlationId);
            await _next.Invoke(context);
        }
    }

    public static class CorrelationIdMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}