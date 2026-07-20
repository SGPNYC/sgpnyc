var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Serve sub-page index.html files for /page and /page/ requests
app.Use(async (ctx, next) =>
{
    var pages = new[] { "abt", "services", "products", "success", "partners", "new-page", "blog", "games" };
    var segment = ctx.Request.Path.Value?.Trim('/');
    if (pages.Contains(segment))
    {
        var file = Path.Combine(app.Environment.WebRootPath, segment, "index.html");
        if (File.Exists(file))
        {
            ctx.Response.ContentType = "text/html; charset=utf-8";
            await ctx.Response.SendFileAsync(file);
            return;
        }
    }
    await next();
});

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapFallbackToFile("index.html");

app.Run();
