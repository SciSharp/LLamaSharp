using LLama.Web.Common;
using LLama.Web.Hubs;
using LLama.Web.Services;
using System.Text.Json.Serialization;

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

            // Load LLamaOptions
            builder.Services.AddOptions<LLamaOptions>()
                  .PostConfigure(options => options.Initialize())
                  .BindConfiguration(nameof(LLamaOptions));

            // Services DI
            builder.Services.AddHostedService<LoaderService>();
            builder.Services.AddSingleton<IModelService, ModelService>();
            builder.Services.AddSingleton<IModelSessionService, ModelSessionService>();


            var configuration = builder.Configuration.GetSection(nameof(LLamaOptions)).Get<LLamaOptions>();
            if (configuration.AppType == AppType.Web)
            {
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
            else if (configuration.AppType == AppType.WebApi)
            {

                // Add Controllers
                builder.Services.AddControllers().AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

                // Add Swagger/OpenAPI https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options => options.UseInlineDefinitionsForEnums());

                var app = builder.Build();

                app.UseSwagger();
                app.UseSwaggerUI(options => {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });

                app.UseAuthorization();
                app.MapControllers();
                app.Run();
            }
        }
    }
}