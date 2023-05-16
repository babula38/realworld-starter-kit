using Conduit.Grains;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.Features.Users;

public static class UserApi
{
    public static RouteGroupBuilder MapUser(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder group = routes.MapGroup("/api/user");
        _ = group.WithTags("Users");

        //POST /api/user
        _ = group.MapPost("/", async (IGrainFactory grainFactory, [FromBody] UserRequest request) =>
        {
            IUserGrain grain = grainFactory.GetGrain<IUserGrain>(request.Email);

            User user = await grain.CreateUser(request);

            return Results.Ok(new UserApiResponse(user));
        }).Produces<UserApiResponse>();

        // GET api/user
        _ = group.MapGet("/", async (IGrainFactory grainFactory, IHttpContextAccessor contextAccessor) =>
        {
            string email = contextAccessor.GetUserEmail();

            IUserGrain grain = grainFactory.GetGrain<IUserGrain>(email);

            User user = await grain.GetUser();

            return Results.Ok(new UserApiResponse(user));
        }).Produces<UserApiResponse>();

        // PUT api/user
        _ = group.MapPut("/", async (IGrainFactory grainFactory, UserApiRequest request) =>
        {
            IUserGrain grain = grainFactory.GetGrain<IUserGrain>(request.User.Email);

            User user = await grain.UpdateUser(request.User);

            return Results.Ok(new UserApiResponse(user));
        }).Produces<UserApiResponse>();

        return group;
    }
}