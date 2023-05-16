
using Conduit;
using Conduit.Grains;

public static class ProfileApi
{
    public static RouteGroupBuilder MapFollow(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder group = routes.MapGroup("/api/profile");
        _ = group.WithTags("profile");

        _ = group.MapGet("/{userName}/follow", async (IGrainFactory grainFactory, IHttpContextAccessor contextAccessor, string userName) =>
        {
            string currentUserName = contextAccessor.GetUserName();
            string currentUserEmail = contextAccessor.GetUserEmail();

            IFollowGrain currentUserFollowGrain = grainFactory.GetGrain<IFollowGrain>(currentUserName);
            List<Follow> followsOfLoggedInUser = await currentUserFollowGrain.Follow(userName);

            bool isFollowing = followsOfLoggedInUser.Any(x => string.Equals(x.UserName, userName, StringComparison.OrdinalIgnoreCase));

            IUserGrain userToGrain = grainFactory.GetGrain<IUserGrain>(currentUserEmail);
            User userToFollow = await userToGrain.GetUser();

            return new
            {

            };
        });

        _ = group.MapDelete("/{userName}/follow", async (IGrainFactory grainFactory, IHttpContextAccessor contextAccessor, string userName) =>
        {
            string currentUserName = contextAccessor.GetUserName();
            string currentUserEmail = contextAccessor.GetUserEmail();

            IFollowGrain currentUserFollowGrain = grainFactory.GetGrain<IFollowGrain>(currentUserName);
            List<Follow> followsOfLoggedInUser = await currentUserFollowGrain.UnFollow(userName);

            bool isFollowing = followsOfLoggedInUser.Any(x => string.Equals(x.UserName, userName, StringComparison.OrdinalIgnoreCase));

            IUserGrain userToGrain = grainFactory.GetGrain<IUserGrain>(currentUserEmail);
            User userToFollow = await userToGrain.GetUser();
        });

        return group;
    }
}