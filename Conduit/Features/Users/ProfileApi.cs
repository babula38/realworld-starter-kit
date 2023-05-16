
using Conduit.Grains;

namespace Conduit.Features.Users;

public static class ProfileApi
{
    public static RouteGroupBuilder MapProfile(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder group = routes.MapGroup("/api/profile");
        _ = group.WithTags("profile");

        _ = group.MapGet("/{emailToFollow}", async (IGrainFactory grainFactory, IHttpContextAccessor contextAccessor, string emailToFollow) =>
        {
            string currentUserEmail = contextAccessor.GetUserEmail();

            IFollowGrain currentUserFollowGrain = grainFactory.GetGrain<IFollowGrain>(currentUserEmail);
            List<Follow> followsOfLoggedInUser = await currentUserFollowGrain.GetFollowers();

            bool isFollowing = followsOfLoggedInUser.Any(x => string.Equals(x.UserName, emailToFollow, StringComparison.OrdinalIgnoreCase));

            IUserGrain userToGrain = grainFactory.GetGrain<IUserGrain>(emailToFollow);
            User userToFollow = await userToGrain.GetUser();

            return Results.Ok(new ProfileResponse
            {
                Profile = new ProfileData
                {
                    UserName = userToFollow.UserName,
                    Bio = userToFollow.Bio,
                    Img = userToFollow.Img,
                    Following = isFollowing,
                }
            });
        }).Produces<ProfileResponse>();

        _ = group.MapGet("/test/{key}",
        async (IGrainFactory grainFactory, string key) =>
        {
            var profileGrain = grainFactory.GetGrain<IProfileGrain>(key);
            User user = await profileGrain.GetUser();

            return Results.Ok(user);
        });

        return group;
    }

    public record ProfileResponse
    {
        public ProfileData Profile { get; set; }
    }

    public record ProfileData
    {
        public string UserName { get; set; }
        public string Bio { get; set; }
        public string Img { get; set; }
        public bool Following { get; set; }
    }
}