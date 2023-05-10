using System.Security.Claims;
using Conduit.Grains;

namespace Conduit.Features.Users;

public static class UserApi
{
    public static RouteGroupBuilder MapUser(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/user");
        group.WithTags("Users");

        //TODO: Need to work on as the API contract is not taking any input
        // group.MapGet("/", async (IGrainFactory grainFactory, IHttpContextAccessor contextAccessor) =>
        group.MapGet("/{email}", async (IGrainFactory grainFactory, string email) =>
        {
            // Claim? email = contextAccessor
            //     .HttpContext?
            //     .User?
            //     .Claims?
            //     .FirstOrDefault(x => x.Type == ClaimTypes.Email);

            IUserGrain grain = grainFactory.GetGrain<IUserGrain>(email);

            User user = await grain.GetUser();

            return Results.Ok(new UserApiResponse(user));
        }).Produces<UserApiResponse>();

        group.MapPut("/", async (IGrainFactory grainFactory, UserApiRequest request) =>
        {
            // Claim? email = contextAccessor
            //     .HttpContext?
            //     .User?
            //     .Claims?
            //     .FirstOrDefault(x => x.Type == ClaimTypes.Email);

            IUserGrain grain = grainFactory.GetGrain<IUserGrain>(request.User.Email);

            User user = await grain.UpdateUser(request.User);

            return Results.Ok(new UserApiResponse(user));
        }).Produces<UserApiResponse>();

        return group;
    }
}