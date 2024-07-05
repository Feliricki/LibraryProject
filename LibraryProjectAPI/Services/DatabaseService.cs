using LibraryProjectAPI.Models;

namespace LibraryProjectAPI.Services
{
    public class DatabaseService : IHostedService, IDisposable
    {
        readonly IServiceScopeFactory _scopeFactory;
        private Timer? _timer = null;

        public DatabaseService(
            IServiceScopeFactory scopeFactory
        )
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(UpdateEntries, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            await Task.CompletedTask;
        }

        // NOTE: Every day the entries in the database need to decrement their 
        // DaysUntilAvailable field
        private void UpdateEntries(object? ob)
        {
            try
            {
                Console.WriteLine("Updating entries in the database in Database worker.");
                var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                foreach (var book in dbContext.Books)
                {
                    if (book.DaysUntilAvailable <= 0)
                    {
                        continue;
                    }
                    book.DaysUntilAvailable -= 1;
                    Console.WriteLine($"Book with title {book.Title} is now available in {book.DaysUntilAvailable} days");
                }

                dbContext.SaveChanges();

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
