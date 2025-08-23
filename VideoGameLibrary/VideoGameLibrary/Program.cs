using VideoGameLibrary.Data;
using VideoGameLibrary.Data.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

namespace VideoGameLibrary
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<VideoGameDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("VideoGameDBContext") ?? throw new InvalidOperationException("Connection string 'VideoGameDBContext' not found.")));
            builder.Services.AddDbContext<LoginContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("LoginContextConnection") ?? throw new InvalidOperationException("Connection string 'LoginContext' not found.")));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<LoginContext>();

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddTransient<IVideoGameDal, VideoGameDBDal>();
            builder.Services.AddHttpClient<VideoGameApiService>();
            builder.Services.AddScoped<VideoGameApiService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<VideoGameDBContext>();
                context.Database.EnsureCreated();
                DBInitializer.Initialize(context);
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
