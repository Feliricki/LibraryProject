using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProjectAPI.Models;

// This table may go unused
[Table("Reviews")]
public class Reviews
{
    [Key]
    [Required]
    public int Id { get; set; }

    [ForeignKey(nameof(Books))]
    public int BookId { get; set; }

    public Books? Books { get; set; } = null!;

    //[ForeignKey(nameof(User))]
    //public string UserId { get; set; } = null!;

    //public ApplicationUser? User { get; set; }

    [Required]
    public int Review { get; set; }
}