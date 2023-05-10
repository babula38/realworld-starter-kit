using Conduit.Features.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Host.UseOrleans(siloBuilder =>
{
    _ = siloBuilder.UseLocalhostClustering();
    _ = siloBuilder.AddMemoryGrainStorage("conduit");

    _ = siloBuilder.UseDashboard(x => x.HostSelf = true);
});


//Add services
WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Map("/dashboard", x => x.UseOrleansDashboard());

app.MapUsers();
app.MapUser();
app.MapProfile();

app.Run();
