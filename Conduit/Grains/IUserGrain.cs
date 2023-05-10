using Orleans.Runtime;

namespace Conduit.Grains;

public interface IUserGrain : IGrainWithStringKey
{
    Task<User> CreateUser(UserRequest request);
    Task<User> GetUser();
    Task<User> UpdateUser(User user);
}

public class UserGrain : Grain, IUserGrain
{
    private readonly IPersistentState<User> _user;
    private readonly ILogger<UserGrain> _logger;
    public UserGrain(
        ILogger<UserGrain> logger,
        [PersistentState(stateName: "user", storageName: "conduit")] IPersistentState<User> user
    )
    {
        _logger = logger;
        _user = user;
    }

    public async Task<User> CreateUser(UserRequest request)
    {
        _user.State = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            Token = Guid.NewGuid().ToString(),
            Bio = "",
            Img = "",
        };
        await _user.WriteStateAsync();

        _logger.LogInformation($"UserName:-{request.UserName},Email:-{request.Email},Token:-{_user.State.Token}");

        return _user.State;
    }

    public Task<User> GetUser()
            => Task.FromResult(_user.State);

    public async Task<User> UpdateUser(User user)
    {
        _user.State = user;

        await _user.WriteStateAsync();

        return _user.State;
    }
}

[GenerateSerializer]
public record UserRequest
{
    [Id(0)]
    public string UserName { get; set; }
    [Id(1)]
    public string Email { get; set; }
    [Id(2)]
    public string Password { get; set; }
}

[GenerateSerializer]
public record User
{
    [Id(0)]
    public string Email { get; set; }
    [Id(1)]
    public string Token { get; set; }
    [Id(2)]
    public string UserName { get; set; }
    [Id(3)]
    public string Bio { get; set; }
    [Id(4)]
    public string Img { get; set; }
}