using LibraryProjectAPI.Constants;
using LibraryProjectAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bogus;

namespace LibraryProjectAPI.Services
{
    public class StartupService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        
        private readonly int NumberOfBookToGenerate = 500;
        public StartupService(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory
            ) 
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellation)
        {
            // NOTE: Traditional injection does not work in a singleton service.
            await SetUpDatabase();
            await SeedDatabase();
        }

        //private async Task CreateDatabase()
        //{
        //}

        private async Task SeedDatabase()
        {
            // NOTE: The isbn is unique for each book. This is used in place of the Id 
            // to differentiate between books
            var isbn = 0;
            var bookFaker = new Faker<Books>()
                .RuleFor(o => o.Title, f => f.Hacker.Phrase())
                .RuleFor(o => o.BookCoverUrl, f => f.Image.PicsumUrl(300, 500))
                .RuleFor(o => o.Description, f => f.Lorem.Sentence())
                .RuleFor(o => o.Publisher, f => f.Company.CompanyName())
                .RuleFor(o => o.Author, f => f.Name.FullName())
                .RuleFor(o => o.Available, f => true)
                .RuleFor(o => o.DaysUntilAvailable, f => 0)
                .RuleFor(o => o.PublicationDate, f => f.Date.Past())
                .RuleFor(o => o.Category, f => CategoryHelper.PickRandomCategory())
                .RuleFor(o => o.Isbn, f => isbn++)
                .RuleFor(o => o.PageCount, f => f.Random.Number(50, 1000));
                

            List<Books> newBooks = [];
            for (int i = 0; i < NumberOfBookToGenerate; i++)
            {
                newBooks.Add(bookFaker.Generate());
            }

            await using var scope = _scopeFactory.CreateAsyncScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (await dbContext.Books.AnyAsync())
            {
                Console.WriteLine("Database already contains data");
                return;
            }
            foreach (var book in newBooks)
            {
                dbContext.Books.Add(book);
            }
            await dbContext.SaveChangesAsync();

            Console.WriteLine("Populated the Books table with dummy data.");
        }

        // This method is in charge of setting up the initial migrations
        // using Books.cs and ApplicationDbContext as the entity.
        private async Task SetUpDatabase()
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            using RoleManager<IdentityRole> _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            using UserManager<ApplicationUser> _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await dbContext.Database.MigrateAsync();

            Console.WriteLine("Applied migrations to database.");

            int userCreated = 0;
            int rolesCreated = 0;

            // Instantiate the roles if they don't already exist
            if (!await _roleManager.RoleExistsAsync(ApplicationRoles.Librarian))
            {
                await _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Librarian));
                rolesCreated++;
            }

            if (!await _roleManager.RoleExistsAsync(ApplicationRoles.User))
            {
                await _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.User));
                rolesCreated++;
            }

            // Instantiate a default libarian
            if (await _userManager.FindByNameAsync(_configuration["DefaultUser:DefaultName"]!) == null)
            {
                var libarian = new ApplicationUser
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = _configuration["DefaultUser:DefaultName"],
                    Email = _configuration["DefaultUser:DefaultEmail"]
                };

                var identityResult = await _userManager.CreateAsync(libarian, _configuration["DefaultUser:DefaultPassword"]!);
                if (identityResult.Succeeded)
                {
                    userCreated++;
                }
                await _userManager.AddToRoleAsync(libarian, ApplicationRoles.Librarian);
            }
            Console.WriteLine($"Created {userCreated} new users and {rolesCreated} new roles.");
        }

        public Task StopAsync(CancellationToken cancellationToken) 
        {
            return Task.CompletedTask;
        }
    }
}
