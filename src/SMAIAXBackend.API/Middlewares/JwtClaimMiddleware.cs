using System.IdentityModel.Tokens.Jwt;

namespace SMAIAXBackend.API.Middlewares;

public class JwtClaimMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers.Authorization.FirstOrDefault();

        if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authorizationHeader["Bearer ".Length..].Trim();

            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);
                
            var nameIdentifierClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

            if (nameIdentifierClaim != null)
            {
                context.Items["UserId"] = nameIdentifierClaim.Value;
            }
        }
        
        await next(context);
    }
}