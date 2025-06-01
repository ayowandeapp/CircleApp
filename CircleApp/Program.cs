using CircleApp.Data;
using CircleApp.Helpers;
using CircleApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Database configuration
string? dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(dbConnectionString));

//Service configuration
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IHashtagService, HashtagsService>();
builder.Services.AddScoped<IStoriesService, StoriesService>();
builder.Services.AddScoped<IFilesService, FilesService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

//Seeed database with initial data using Dependency Injection (DI) scoping
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>(); //resolve this service
    await dbContext.Database.MigrateAsync(); //run pending migrations
    await DbInitializer.Seed(dbContext);

}

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
