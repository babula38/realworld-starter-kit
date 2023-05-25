using Conduit.Features.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans(siloBuilder =>
{
    _ = siloBuilder.UseLocalhostClustering();
    _ = siloBuilder.AddMemoryGrainStorage("conduit");

    _ = siloBuilder.UseDashboard(x => x.HostSelf = true);
});

//add Services IC
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//configure endpoints / middleware
WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Map("/dashboard", x => x.UseOrleansDashboard());

app.Use(async (context, next) =>
{
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        var email = context.Request.Headers["Authorization"].ToString()["Bearer ".Length..].Split("--")[0];
        var userName = context.Request.Headers["Authorization"].ToString()["Bearer ".Length..].Split("--")[1];

        context.Items["UserName"] = userName;
        context.Items["Email"] = email;
    }

    await next(context);
});

app.MapUsers();
app.MapUser();
app.MapProfile();
app.MapArticle();

app.Run();