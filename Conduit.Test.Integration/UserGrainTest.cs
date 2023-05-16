using Conduit.Grains;
using FluentAssertions;
using Orleans.TestingHost;
using Tynamix.ObjectFiller;

namespace Conduit.Test.Integration;

[Collection(ClusterCollection.Name)]
public class UserGrainTest
{
    private readonly TestCluster _cluster;

    public UserGrainTest(ClusterFixture fixture)
    {
        _cluster = fixture.Cluster;
    }
    private static Filler<UserRequest> RandomUserRequest()
    {
        var filler = new Filler<UserRequest>();
        var email = new EmailAddresses(".test").GetValue();

        filler.Setup()
            .OnProperty(request => request.Email).Use(email)
            .OnProperty(request => request.UserName).Use(email)
            .OnProperty(request => request.Password).Use(new MnemonicString(1, 12, 20));

        return filler;
    }

    [Fact]
    public async Task CreateUser()
    {
        //Given
        UserRequest randomRequest = RandomUserRequest().Create();

        UserRequest inputUserRequest = randomRequest;
        UserRequest expectedUserRequest = inputUserRequest;

        //When
        IUserGrain userGrain = _cluster.GrainFactory.GetGrain<IUserGrain>(inputUserRequest.Email);
        User actualUser = await userGrain.CreateUser(inputUserRequest);

        //then
        _ = actualUser.Email.Should().Be(expectedUserRequest.Email);
        _ = actualUser.UserName.Should().Be(expectedUserRequest.UserName);
        _ = actualUser.Password.Should().Be(expectedUserRequest.Password);
        _ = actualUser.Token.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateProfile()
    {
        //Given
        UserRequest randomRequest = RandomUserRequest().Create();

        UserRequest inputUserRequest = randomRequest;
        UserRequest expectedUserRequest = inputUserRequest;

        //When
        IUserGrain userGrain = _cluster.GrainFactory.GetGrain<IUserGrain>(inputUserRequest.Email);
        User createdUser = await userGrain.CreateUser(inputUserRequest);

        IProfileGrain profileGrain = _cluster.GrainFactory.GetGrain<IProfileGrain>(createdUser.UserName);
        // await profileGrain.CreateProfile(createdUser.Email);
        var actualUser = await profileGrain.GetUser();

        //then
        _ = actualUser.Email.Should().Be(createdUser.Email);
        _ = actualUser.UserName.Should().Be(createdUser.UserName);
        _ = actualUser.Password.Should().Be(createdUser.Password);
        _ = actualUser.Token.Should().BeEmpty();
    }
}