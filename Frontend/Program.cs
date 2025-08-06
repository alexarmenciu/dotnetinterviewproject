using Frontend.Components;


var builder = WebApplication.CreateBuilder(args);
// Register HttpClient and TaskApiService with configuration
builder.Services.AddScoped(sp => new HttpClient());
builder.Services.AddScoped<Frontend.Services.TaskApiService>(sp =>
    new Frontend.Services.TaskApiService(
        sp.GetRequiredService<HttpClient>(),
        sp.GetRequiredService<IConfiguration>()
    )
);

// Register SignalR service
builder.Services.AddScoped<Frontend.Services.TaskSignalRService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
