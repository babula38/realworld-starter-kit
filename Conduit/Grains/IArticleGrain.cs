
using Orleans.Runtime;

namespace Conduit.Grains;

public interface IArticleGrain : IGrainWithStringKey
{
    Task<Article> CreateArticle(CreateArticle request);
}

public class ArticleGrain : Grain, IArticleGrain
{
    private readonly IPersistentState<Article> _article;
    private readonly ILogger<ArticleGrain> _logger;

    public ArticleGrain(
        [PersistentState(stateName: "article", storageName: "conduit")] IPersistentState<Article> article,
        ILogger<ArticleGrain> logger
    )
    {
        _article = article;
        _logger = logger;
    }

    public async Task<Article> CreateArticle(CreateArticle request)
    {
        var article = new Article
        {
            Slug = request.Title,//TODO: Need to convert it to Slug
            Title = request.Title,
            Body = request.Body,
            Description = request.Description,
            TagList = request.TagList,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AuthorId = request.UserId,
            Favorited = false,
            FavoritesCount = 0,
        };

        _article.State = article;
        await _article.WriteStateAsync();

        _logger.LogInformation("Article saved");

        return article;
    }
}

[GenerateSerializer]
public record Article
{
    [Id(0)]
    public string Title { get; set; }
    [Id(1)]
    public string Description { get; set; }
    [Id(2)]
    public string Body { get; set; }
    [Id(3)]
    public string[] TagList { get; set; }
    [Id(4)]
    public DateTime CreatedAt { get; set; }
    [Id(5)]
    public DateTime UpdatedAt { get; set; }
    [Id(6)]
    public bool Favorited { get; set; }
    [Id(7)]
    public int FavoritesCount { get; set; }
    [Id(8)]
    public int AuthorId { get; set; }
    [Id(9)]
    public string Slug { get; set; }
}

[GenerateSerializer]
public record CreateArticle
{
    [Id(0)]
    public string Title { get; set; }
    [Id(1)]
    public string Description { get; set; }
    [Id(2)]
    public string Body { get; set; }
    [Id(3)]
    public string[] TagList { get; set; }
    [Id(4)]
    public int UserId { get; set; }
}
