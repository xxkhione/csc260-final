using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Security;

namespace VideoGameLibrary
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            builder.Services.AddHttpContextAccessor();


            builder.Services.AddHttpClient("UserAuthenticationMicroservice", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["UserAuthenticationMicroservice:BaseUrl"]);
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            });

            builder.Services.AddHttpClient("UserDataMicroservice", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["UserDataMicroservice:BaseUrl"]);
            });
            builder.Services.AddHttpClient("ExternalGamesAPI", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ExternalGamesAPI:BaseUrl"]);
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                });

            var app = builder.Build();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            app.MapRazorPages();
            app.Run();
        }
    }
}
