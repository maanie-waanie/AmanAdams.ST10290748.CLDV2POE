using CLDV2POE.Services;
using Microsoft.AspNetCore.Hosting;


namespace CLDV2POE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //CreateHostBuilder(args).Build().Run();

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddHttpClient();
            builder.Services.AddControllersWithViews();

            //builder.Services.AddHttpClient<BlobService>();

            builder.Services.AddScoped<TableService>();
            builder.Services.AddScoped<BlobService>();

            // Register your custom services
            //builder.Services.AddSingleton<BlobService>();
            //builder.Services.AddSingleton<TableService>();
            //builder.Services.AddSingleton<QueueService>();
            //builder.Services.AddSingleton<FileService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Program>();
            });
    }
}
