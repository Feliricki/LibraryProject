using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryProjectAPI.Models;

[Table("Books")]
[Index(nameof(Title), nameof(Author), nameof(Available))]
public class Books
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    [Required] 
    public string Description { get; set; } = null!;

    [Required]
    public string BookCoverUrl { get; set; } = null!;

    [Required]
    public string Publisher { get; set; } = null!;

    [Required]
    public string Author { get; set; } = null!;

    // NOTE: If the book is available then the number of days of remaining must be 0
    // else if the book is not available and there are days remaining then the book is borrowed 
    // else if the book is not available and there are no days remaining then the book is overdue
    [Required]
    public bool Available { get; set; } = true;
    [Required]
    public int DaysUntilAvailable { get; set; } = 0;
    [Required]
    public DateTime? PublicationDate { get; set; } = null;
    [Required]
    public string Category { get; set; } = null!;
    [Required]
    public int Isbn { get; set; }
    [Required]
    public int PageCount { get; set; }

    // NOTE: The review serve as the parent of the relationship
    public ICollection<Reviews>? Reviews { get; set; } = null!;

    //public ICollection<ApplicationUser>? ApplicationUsers { get; set; } = null!;
}