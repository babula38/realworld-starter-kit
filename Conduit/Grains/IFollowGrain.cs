using Orleans.Runtime;

namespace Conduit.Grains;

public interface IFollowGrain : IGrainWithStringKey
{
    Task<List<Follow>> GetFollowers();
    Task<List<Follow>> Follow(string userName);
    Task<List<Follow>> UnFollow(string userName);
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

    public async Task<List<Follow>> Follow(string userName)
    {
        _follows.State.Add(new Follow { UserName = userName });

        await _follows.WriteStateAsync();

        return _follows.State;
    }

    public Task<List<Follow>> GetFollowers()
            => Task.FromResult(_follows.State);
    public async Task<List<Follow>> UnFollow(string userName)
    {
        _follows.State.Remove(new Follow { UserName = userName });

        await _follows.WriteStateAsync();

        return _follows.State;
    }
}

[GenerateSerializer]
public record Follow
{
    [Id(0)]
    public string UserName { get; set; }
}