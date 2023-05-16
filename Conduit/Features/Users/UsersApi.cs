using Conduit.Grains;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.Features.Users;

public static class UsersApi
{
    public static RouteGroupBuilder MapUsers(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder group = routes.MapGroup("/api/users");
        _ = group.WithTags("Users");

        _ = group.MapPost("/login", async (IGrainFactory grainFactory, [FromBody] UserRequest request) =>
        {
            IUserGrain grain = grainFactory.GetGrain<IUserGrain>(request.Email);

            User user = await grain.GetUser();

            bool isAuthenticated = user.Email == request.Email && user.Password == request.Password;

            return Results.Ok(isAuthenticated ? new UserApiResponse(user) : null);
        }).Produces<UserApiResponse>();

        return group;
    }

    public record UserLoginRequest
    {
        public UserLoginData User { get; set; }
    }

    public record UserLoginData
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}