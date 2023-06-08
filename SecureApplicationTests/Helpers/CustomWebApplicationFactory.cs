using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SecureApplicationTests.Helpers
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Replace the existing database context with an in-memory database context
                services.AddDbContext<RepositoryContext>(options =>
                    options.UseInMemoryDatabase("MyDatabase"));

                // Remove the existing RepositoryContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(RepositoryContext));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add a new instance of RepositoryContext as a singleton
                services.AddSingleton<RepositoryContext>(sp =>
                    InMemorydataContext.Inmemory());  // Create an existing in-memory database context
            });
        }
    }

}