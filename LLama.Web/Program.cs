using LLama.Web.Common;
using LLama.Web.Hubs;
using LLama.Web.Services;

namespace LLama.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();

            // Load InteractiveOptions
            builder.Services.AddOptions<LLamaOptions>()
                .PostConfigure(x => x.Initialize())
                .BindConfiguration(nameof(LLamaOptions));

            // Services DI
            builder.Services.AddSingleton<IModelService, ModelService>();
            builder.Services.AddSingleton<IModelSessionService, ModelSessionService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapHub<SessionConnectionHub>(nameof(SessionConnectionHub));

            app.Run();
        }
    }
}