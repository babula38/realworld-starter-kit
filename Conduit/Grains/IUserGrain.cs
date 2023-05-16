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
    private readonly IGrainFactory _grainFactory;

    public UserGrain(
        ILogger<UserGrain> logger
        , [PersistentState(stateName: "user", storageName: "conduit")] IPersistentState<User> user
        , IGrainFactory grainFactory
        )
    {
        _logger = logger;
        _user = user;
        _grainFactory = grainFactory;
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
            Password = request.Password,
        };
        await _user.WriteStateAsync();

        _logger.LogInformation($"UserName:-{request.UserName},Email:-{request.Email},Token:-{_user.State.Token}");

        IProfileGrain profileGrain = _grainFactory.GetGrain<IProfileGrain>(_user.State.UserName);
        await profileGrain.CreateProfile(this.GetPrimaryKeyString());

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
    [Id(5)]
    public string Password { get; set; }
}