using System.ComponentModel.DataAnnotations;
using LibraryProjectAPI.Constants;
using LibraryProjectAPI.DTO;
using LibraryProjectAPI.DTO.Books;
using LibraryProjectAPI.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
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
        public BooksController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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


            } catch (Exception)
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

            } catch (Exception)
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

        [HttpDelete(Name="RemoveBook")]
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
                var changes = _dbContext.SaveChangesAsync();
                return Ok(changes);

            } catch (Exception)
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

            } catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet(Name = "GetBook")]
        public async Task<ActionResult<BooksDTO>> GetBook(int isbn)
        {
            try
            {
                var book = await _dbContext.Books
                    .AsNoTracking()
                    .Where(book => book.Isbn == isbn)
                    .ProjectToType<BooksDTO>()
                    .ToListAsync();

                return Ok(book.First()) ?? throw new Exception("No Book Found");
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

                return await ApiResult<BooksDTO>.CreateAsync(
                    source,
                    pageIndex,
                    pageSize,
                    sortColumn,
                    sortOrder,
                    filterColumn,
                    filterQuery);
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
