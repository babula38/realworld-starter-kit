using Conduit.Grains;

namespace Conduit.Features.Users;

public static class ArticleAPI
{
    public static RouteGroupBuilder MapArticle(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder group = routes.MapGroup("/api/articles");
        _ = group.WithTags("articles");

        _ = group.MapPost("/", async (IGrainFactory grainFactory, IHttpContextAccessor contextAccessor, CreateArticle request) =>
                {
                    IArticleGrain grain = grainFactory.GetGrain<IArticleGrain>(request.Title);//TODO:Make this to use slug
                    Article article = await grain.CreateArticle(request);

                    return Results.Ok(new { Article = article });
                });

        return group;
    }
}