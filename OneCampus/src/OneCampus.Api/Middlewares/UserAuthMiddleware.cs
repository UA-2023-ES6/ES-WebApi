using OneCampus.Api.Models;
using OneCampus.Domain.Services;
using System.IdentityModel.Tokens.Jwt;

namespace OneCampus.Api.Middlewares;

public class UserAuthMiddleware
{
    private const string HealthCheckPath = "/healthz";

    private readonly RequestDelegate _next;

    public UserAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUsersService usersService, DebugDataService debugDataService)
    {
        usersService.ThrowIfNull();

        if(IsHealthCheck(context.Request))
        {
            await _next(context);
            
            return;
        }

        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (token == null)
        {
            throw new ArgumentException("token not found in Authorization header. Please provide token");
        }
        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtSecurityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
        var claims = jwtSecurityToken?.Claims;

        var id = claims?.FirstOrDefault(x => x.Type == "cognito:username")?.Value;
        var email = claims?.FirstOrDefault(x => x.Type == "email")?.Value;
        var username = claims?.FirstOrDefault(x => x.Type == "custom:username")?.Value;

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("id, email and username cannot be null");
        }

        if (!Guid.TryParse(id, out var guid))
        {
            throw new ArgumentException("invalid id");
        }

        var user = await usersService.FindAsync(guid);
        if (user == null)
        {
            user = await usersService.CreateAsync(guid, username, email);

            await debugDataService.AddUserToDefaultInstitution(guid);
        }

        if (user.Username != username || user.Email != email)
        {
            throw new ArgumentException("invalid user");
        }

        var userInfo = context.RequestServices.GetRequiredService<UserInfo>();
        userInfo.Id = user.Id;
        userInfo.UserName = user.Username;
        userInfo.Email = user.Email;

        await _next(context);
    }

    private bool IsHealthCheck(HttpRequest request)
    {
        request.ThrowIfNull();

        return request.Path == HealthCheckPath;
    }
}
