using OneCampus.Api.Models;
using OneCampus.Domain.Services;
using System.IdentityModel.Tokens.Jwt;

namespace OneCampus.Api.Middlewares;

public class UserAuthMiddleware
{
    private readonly RequestDelegate _next;

    private readonly IUsersService _usersService;

    public UserAuthMiddleware(RequestDelegate next, IUsersService usersService)
    {
        _next = next;
        _usersService = usersService.ThrowIfNull().Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
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

        var user = await _usersService.FindAsync(guid);
        if (user == null)
        {
            user = await _usersService.CreateAsync(guid, username, email);
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
}
