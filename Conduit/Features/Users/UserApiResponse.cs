using Conduit.Grains;

namespace Conduit.Features.Users;

public class UserApiResponse
{
    public User User { get; }
    public UserApiResponse(User user)
    {
        User = user;
    }
}
public class UserApiRequest
{
    public User User { get; set; }
}
