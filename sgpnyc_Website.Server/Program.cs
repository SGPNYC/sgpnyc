var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Serve index.html for any /segment or /segment/ that has one in wwwroot
app.Use(async (ctx, next) =>
{
    var segment = ctx.Request.Path.Value?.Trim('/');
    if (!string.IsNullOrEmpty(segment) && !segment.Contains('/'))
    {
        var wwwroot = app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
        var file = Path.Combine(wwwroot, segment, "index.html");
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
