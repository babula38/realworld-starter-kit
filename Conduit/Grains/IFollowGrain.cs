using Orleans.Runtime;

namespace Conduit.Grains;

public interface IFollowGrain : IGrainWithStringKey
{
    Task<List<Follow>> GetFollows();
    Task<List<Follow>> Follow(string mailToFollow);
}

public class FollowGrain : Grain, IFollowGrain
{
    private readonly IPersistentState<List<Follow>> _follows;

    public FollowGrain(
        [PersistentState(stateName: "follow", storageName: "conduit")] IPersistentState<List<Follow>> follows
    )
    {
        _follows = follows;
    }

    public async Task<List<Follow>> Follow(string mailToFollow)
    {
        _follows.State.Add(new Follow { Email = mailToFollow });

        await _follows.WriteStateAsync();

        return _follows.State;
    }

    public Task<List<Follow>> GetFollows()
            => Task.FromResult(_follows.State);
}

[GenerateSerializer]
public record Follow
{
    [Id(0)]
    public string Email { get; set; }
}