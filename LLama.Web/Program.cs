using LLama.Web.Common;
using LLama.Web.Hubs;
using LLama.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mvcBuilder = builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
    builder.Configuration.AddJsonFile("appsettings.Local.json", true);
}

builder.Services.AddSignalR();
builder.Logging.ClearProviders();
builder.Services.AddLogging((loggingBuilder) => loggingBuilder.SetMinimumLevel(LogLevel.Trace).AddConsole());
builder.Services.AddHttpClient(nameof(ModelDownloadService), client => client.Timeout = Timeout.InfiniteTimeSpan);

// Load interactive options.
builder.Services.AddOptions<LLamaOptions>()
    .BindConfiguration(nameof(LLamaOptions));

// Register DI services.
builder.Services.AddHostedService<ModelLoaderService>();
builder.Services.AddSingleton<IModelDownloadService, ModelDownloadService>();
builder.Services.AddSingleton<IAttachmentService, AttachmentService>();
builder.Services.AddSingleton<IModelService, ModelService>();
builder.Services.AddSingleton<IModelSessionService, ModelSessionService>();

var app = builder.Build();

app.Logger.LogInformation("Model storage path: {Path}", app.Services.GetRequiredService<IModelDownloadService>().ModelsRoot);
app.Logger.LogInformation("Download staging path: {Path}", app.Services.GetRequiredService<IModelDownloadService>().DownloadsRoot);
app.Logger.LogInformation("Uploads path: {Path}", app.Services.GetRequiredService<IAttachmentService>().UploadsRoot);

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
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapHub<SessionConnectionHub>(nameof(SessionConnectionHub));

app.Run();
