using ABCretail_CLVD.Services;
using ABCretail_CLVD.Models;


namespace ABCretail_CLVD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Access the configuration object
            var configuration = builder.Configuration;

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Register BlobService with configuration
            builder.Services.AddSingleton<BlobStorageService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new BlobStorageService(connectionString); // BlobService constructor should take the connection string
            });

            // Register TableStorageService for CustomerEntity with configuration
            builder.Services.AddSingleton<TableStorageService<CustomerEntity>>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new TableStorageService<CustomerEntity>(connectionString, "customerinfo"); // Use "customerinfo" for Table Name
            });

            // Register QueueService with configuration
            builder.Services.AddSingleton<QueueStorageService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage"); // Use the connection string from appsettings.json
                return new QueueStorageService(connectionString, "processing"); // Use "processing" as the queue name
            });

            // Register AzureFileShareService with configuration
            builder.Services.AddSingleton<FileStorageService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new FileStorageService(connectionString, "contracts"); // This is where the file share name goes
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
