using LLama.Web.Common;
using LLama.Web.Hubs;
using LLama.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
    builder.Configuration.AddJsonFile("appSettings.Local.json");
}

builder.Services.AddSignalR();
builder.Logging.ClearProviders();
builder.Services.AddLogging((loggingBuilder) => loggingBuilder.SetMinimumLevel(LogLevel.Trace).AddConsole());

// Load InteractiveOptions
builder.Services.AddOptions<LLamaOptions>()
    .BindConfiguration(nameof(LLamaOptions));

// Services DI
builder.Services.AddHostedService<ModelLoaderService>();
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