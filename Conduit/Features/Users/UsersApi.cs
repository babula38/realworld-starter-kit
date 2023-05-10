using Conduit.Grains;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.Features.Users;

public static class UsersApi
{
    public static RouteGroupBuilder MapUsers(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/users");

        group.WithTags("Users");

        group.MapPost("/", async (IGrainFactory grainFactory, [FromBody] UserRequest request) =>
        {
            IUserGrain grain = grainFactory.GetGrain<IUserGrain>(request.Email);

            User user = await grain.CreateUser(request);

            return Results.Ok(new UserApiResponse(user));
        }).Produces<UserApiResponse>();

        return group;
    }
}