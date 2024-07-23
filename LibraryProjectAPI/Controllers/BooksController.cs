using System.ComponentModel.DataAnnotations;
using LibraryProjectAPI.Constants;
using LibraryProjectAPI.DTO;
using LibraryProjectAPI.DTO.Books;
using LibraryProjectAPI.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryProjectAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private UserManager<ApplicationUser> _userManager;

        public BooksController(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }


        [HttpPost(Name = "LeaveReview")]
        [Authorize(Roles = $"{ApplicationRoles.Librarian}, {ApplicationRoles.User}")]
        public async Task<IActionResult> LeaveReview(
            int isbn,
            [Range(1, 5, ErrorMessage = "Value is not within the acceptable range.")]
            int review,
            string userName)
        {
            var book = await _dbContext.Books
                .Include(x => x.Reviews)
                .FirstOrDefaultAsync(b => b.Isbn == isbn);

            if (book is null)
            {
                return BadRequest("Invalid book requested.");
            }

            var findByUsername = await _userManager.FindByNameAsync(userName);
            var findByEmail = await _userManager.FindByEmailAsync(userName);

            var foundUser = findByUsername ?? findByEmail;

            if (foundUser is null)
            {
                return BadRequest("User does not exists.");
            }

            // TODO: Finish this method
            // See if there's already a score for this book from the user
            bool prevScore = book.Reviews.Any(b => b.UserId == foundUser.Id);
            if (prevScore)
            {
                var prevReview = book.Reviews.FirstOrDefault(b => b.UserId == foundUser.Id);
                prevReview!.Score = review;
                prevReview = foundUser.Reviews.FirstOrDefault(u => u.UserId == foundUser.Id);
                prevReview!.Score = review;

                return Ok(await _dbContext.SaveChangesAsync());
            }

            var newReview = new Reviews
            {
                Score = review
            };

            // WARNING: The users table needs to be checked to see if changes are being saved.
            foundUser.Reviews.Add(newReview);
            book.Reviews.Add(newReview);

            var changes = await _dbContext.SaveChangesAsync();

            return Ok(changes);
        }

        private async Task<long?> GetAverageReview(int isbn)
        {
            var book = await _dbContext.Books
                .AsNoTracking()
                .Include(x => x.Reviews)
                .FirstOrDefaultAsync(b => b.Isbn == isbn);

            if (book is null)
            {
                return null;
            }

            var reviews = book.Reviews;
            if (reviews.Count == 0)
            {
                return 0;
            }

            long cummulativeScore = reviews.Aggregate(0, (prev, current) => prev + current.Score);
            return cummulativeScore / reviews.Count;
        }

        [HttpPost(Name = "SetCheckout")]
        [Authorize(Roles = $"{ApplicationRoles.Librarian}, {ApplicationRoles.User}")]
        public async Task<IActionResult> SetCheckout(
            int isbn,
            bool checkout
            )
        {
            try
            {
                var book = await _dbContext.Books.Where(book => book.Isbn == isbn).FirstAsync();
                if (book is null)
                {
                    return BadRequest("Book was not found");
                }
                if (checkout && book.Available == false)
                {
                    return BadRequest("Book is already checked out");
                }
                // Checking out a book
                if (checkout)
                {
                    book.Available = false;
                    book.DaysUntilAvailable = 5;
                    _dbContext.Books.Update(book);
                    return Ok(await _dbContext.SaveChangesAsync());
                }
                // Returning a book (only librarians can do this)
                if (!User.IsInRole(ApplicationRoles.Librarian))
                {
                    return Unauthorized("User is not authorized.");
                }

                book.Available = true;
                book.DaysUntilAvailable = 0;
                _dbContext.Books.Update(book);
                var changes = await _dbContext.SaveChangesAsync();
                return Ok(changes);


            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost(Name = "AddBook")]
        [Authorize(Roles = ApplicationRoles.Librarian)]
        public async Task<IActionResult> AddBook(
            [FromBody] BooksDTO book)
        {
            try
            {
                var newBook = book.Adapt<Books>();
                var count = await _dbContext.Books.CountAsync();
                newBook.Isbn = count + 1;

                _dbContext.Books.Add(newBook);
                var changes = await _dbContext.SaveChangesAsync();
                return Ok(changes);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        // TODO: Consider some data validation here
        [HttpPost(Name = "EditBook")]
        [Authorize(Roles = ApplicationRoles.Librarian)]
        public async Task<IActionResult> EditBook(
            string title,
            string description,
            int isbn)
        {
            try
            {
                var source = await _dbContext.Books.Where(book => book.Isbn == isbn).FirstAsync();
                if (source is null)
                {
                    return BadRequest("Book was not found");
                }
                source.Title = title;
                source.Description = description;
                _dbContext.Books.Update(source);
                var changes = await _dbContext.SaveChangesAsync();
                return Ok(changes);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete(Name = "RemoveBook")]
        [Authorize(Roles = ApplicationRoles.Librarian)]
        public async Task<IActionResult> RemoveBook(int isbn)
        {
            try
            {
                var source = await _dbContext.Books.Where(book => book.Isbn == isbn).FirstAsync();
                if (source is null)
                {
                    return BadRequest("Book was not found.");
                }
                _dbContext.Books.Remove(source);
                var changes = await _dbContext.SaveChangesAsync();
                return Ok(changes);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet(Name = "GetFeaturedBooks")]
        public async Task<ActionResult<List<BooksDTO>>> GetFeaturedBooks(
            [Range(1, 100)] int numFeatured = 10)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state");
                }

                var source = _dbContext.Books
                    .AsNoTracking()
                    .Select(book => BooksDTO.BookToBookDto(book))
                    .OrderBy(b => Guid.NewGuid())
                    .Take(numFeatured);

                return Ok(await source.ToListAsync());

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private static BooksDTO GetReviewHelper(Books book)
        {
            //var dto = book.Adapt<BooksDTO>(); 
            var dto = new BooksDTO
            {
                Title = book.Title,
                BookCoverUrl = book.BookCoverUrl,
                Description = book.Description,
                Author = book.Author,
                Publisher = book.Publisher,
                Available = book.Available,
                DaysUntilAvailable = book.DaysUntilAvailable,
                PublicationDate = book.PublicationDate,
                Category = book.Category,
                Isbn = book.Isbn,
                PageCount = book.PageCount,
                ReviewCount = book.Reviews.Count
            };

            if (book.Reviews.Count == 0)
            {
                // Prevent divide by zero errors
                dto.ReviewScore = 0;
                return dto;
            }
            var cumulativeScore = book.Reviews.Sum(b => b.Score);
            dto.ReviewScore = cumulativeScore / book.Reviews.Count;
            return dto;
        }


        [HttpGet(Name = "GetBook")]
        public async Task<ActionResult<BooksDTO>> GetBook(int isbn)
        {
            try
            {
                var book = await _dbContext.Books
                    .AsNoTracking()
                    .Include(b => b.Reviews)
                    .FirstOrDefaultAsync(book => book.Isbn == isbn);

                if (book is null) throw new Exception("No Book Found");
                var dto = GetReviewHelper(book);
                return Ok(dto) ?? throw new Exception("No Book Found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet(Name = "GetBooks")]
        public async Task<ActionResult<ApiResult<BooksDTO>>> GetBooks(
            int pageIndex = 0,
            [Range(1, 50)] int pageSize = 10,
            string? sortColumn = "Title",
            string? sortOrder = "ASC",
            string? filterColumn = null,
            string? filterQuery = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state");
                }

                IQueryable<BooksDTO> source = _dbContext.Books
                    .AsNoTracking()
                    .ProjectToType<BooksDTO>();

                var ret = await ApiResult<BooksDTO>.CreateAsync(
                    source,
                    pageIndex,
                    pageSize,
                    sortColumn,
                    sortOrder,
                    filterColumn,
                    filterQuery);


                var isbns = ret.Data.Select(b => b.Isbn).ToHashSet();
                Dictionary<int, Books> booksList = await _dbContext.Books
                    .AsNoTracking()
                    .Include(b => b.Reviews)
                    .Where(b => isbns.Contains(b.Isbn))
                    .ToDictionaryAsync(b => b.Isbn);

                // Modify the return value to include review information such as the reviewCount and the reviewScore
                foreach (var bookDto in ret.Data)
                {
                    if (booksList.TryGetValue(bookDto.Isbn, out var foundBook))
                    {
                        int count = foundBook.Reviews.Count;
                        float sum = foundBook.Reviews.Sum(b => b.Score);
                        if (count == 0)
                        {
                            bookDto.ReviewCount = 0;
                            bookDto.ReviewScore = 0;
                            continue;
                        }

                        bookDto.ReviewCount = count;
                        bookDto.ReviewScore = sum / count;
                    }
                }

                return Ok(ret);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
