using Microsoft.AspNetCore.Identity;

namespace LibraryProjectAPI.Models;

public class ApplicationUser : IdentityUser
{
    // NOTE: Addtional properties can be added here
    // Adding more properties will result a new migrations being created

    // What relationship are needed to implement reviews?
    // Each user has a one to many relationship with reviews
    // Each book also a one to many relationship with reviews
    // In short changes needs to made to this and the book class to hold the instance of the review
    public ICollection<Reviews> Reviews { get; set; } = [];
}