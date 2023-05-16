using Orleans.Runtime;

namespace Conduit.Grains;

public interface IProfileGrain : IGrainWithStringKey
{
    Task CreateProfile(string userId);
    Task<User> GetUser();
}

public class ProfileGrain : IProfileGrain
{
    private readonly IPersistentState<ProfileUserMap> _userId;
    private readonly ILogger<ProfileGrain> _logger;
    private readonly IGrainFactory _grainFactory;

    public ProfileGrain(
        ILogger<ProfileGrain> logger,
        [PersistentState(stateName: "profileUserMapping", storageName: "conduit")] IPersistentState<ProfileUserMap> userId,
        IGrainFactory grainFactory
        )
    {
        _userId = userId;
        _logger = logger;
        _grainFactory = grainFactory;
    }
    public async Task CreateProfile(string userId)
    {
        _userId.State = new ProfileUserMap { UserId = userId };

        await _userId.WriteStateAsync();

        return;
    }

    public async Task<User> GetUser()
    {
        IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>(_userId.State.UserId);
        User user = await userGrain.GetUser();

        return user;
    }

    [GenerateSerializer]
    public record ProfileUserMap
    {
        [Id(0)]
        public string UserId { get; set; }
    }
}