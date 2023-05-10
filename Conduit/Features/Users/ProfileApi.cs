
using Conduit.Grains;

namespace Conduit.Features.Users;

public static class ProfileApi
{
    public static RouteGroupBuilder MapProfile(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/profile");
        group.WithTags("profile");

        group.MapGet("/{emailToFollow}", async (IGrainFactory grainFactory, string emailToFollow) =>
        {
            string currentUserEmail = "";

            IFollowGrain followGrain = grainFactory.GetGrain<IFollowGrain>(currentUserEmail);
            List<Follow> followsOfLoggedInUser = await followGrain.GetFollows();

            bool isFollowing = followsOfLoggedInUser.Any(x => string.Equals(x.Email, emailToFollow, StringComparison.OrdinalIgnoreCase));

            IUserGrain userGrain = grainFactory.GetGrain<IUserGrain>(emailToFollow);
            User toFollowUser = await userGrain.GetUser();

            return Results.Ok(new ProfileResponse
            {
                Profile = new ProfileData
                {
                    UserName = toFollowUser.UserName,
                    Bio = toFollowUser.Bio,
                    Img = toFollowUser.Img,
                    Following = isFollowing,
                }
            });
        }).Produces<ProfileResponse>();

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