using PhotoCollageWeb.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDependencyInjection(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddRazorPages();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapHub<PhotoCollageWeb.Server.Hubs.CollageHub>("/hubs/collage");
app.MapFallbackToFile("index.html");

app.Run();
