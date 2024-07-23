using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryProjectAPI.DTO.Books;

public class BooksDTO
{
    [JsonPropertyName("title")]
    [Required]
    public string Title { get; set; } = null!;
    
    [JsonPropertyName("bookCoverUrl")]
    public string BookCoverUrl { get; set; } = null!;

    [JsonPropertyName("description")] 
    public string Description { get; set; } = null!;

    [JsonPropertyName("author")] 
    public string Author { get; set; } = null!;

    [JsonPropertyName("publisher")] 
    public string Publisher { get; set; } = null!;

    [JsonPropertyName("available")] 
    public bool Available { get; set; }
    
    [JsonPropertyName("daysUntilAvailable")]
    public int DaysUntilAvailable { get; set; }
    
    [JsonPropertyName("publicationDate")]
    public DateTime? PublicationDate { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; } = null!;
    
    [JsonPropertyName("isbn")]
    public int Isbn { get; set; }
    
    [JsonPropertyName("pageCount")]
    public int PageCount { get; set; }

    [JsonPropertyName("reviewCount")]
    public int ReviewCount { get; set; }

    [JsonPropertyName("reviewScore")]
    public float ReviewScore { get; set; }

    public static BooksDTO BookToBookDto(Models.Books books)
    {
        return new BooksDTO
        {
            Title = books.Title,
            BookCoverUrl = books.BookCoverUrl,
            Description = books.Description,
            Author = books.Author,
            Publisher = books.Publisher,
            Available = books.Available,
            DaysUntilAvailable = books.DaysUntilAvailable,
            PublicationDate = books.PublicationDate,
            Category = books.Category,
            Isbn = books.Isbn,
            PageCount = books.PageCount,
            ReviewCount = books.Reviews.Count,
            ReviewScore = GetAverageHelper(books)
        };
    }

    private static int GetAverageHelper(Models.Books books)
    {
        var cummulativeScore = books.Reviews.Aggregate(0, (prev, current) => prev + current.Score);
        if (books.Reviews.Count == 0)
        {
            return 0;
        }
        return cummulativeScore / books.Reviews.Count;
    }
}