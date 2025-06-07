using System.Security.Claims;
using CircleApp.Data;
using CircleApp.Helpers;
using CircleApp.Models;
using CircleApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
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

//Identity Configuration
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    //setting password
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Authentication/Login";
    options.AccessDeniedPath = "/Authentication/AccessDenied";
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    // .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Auth:Google:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"] ?? "";
        options.CallbackPath = "/signin-google";


        // Save tokens
        options.SaveTokens = true;
    })
    .AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["Auth:GitHub:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["Auth:GitHub:ClientSecret"] ?? "";
        options.CallbackPath = "/signin-github";
        
        // Request email scope
        options.Scope.Add("user:email");
        
        options.Scope.Add("read:user");

        options.SaveTokens = true;
        
    })
    ;
//Authentication
// builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


var app = builder.Build();

//Seeed database with initial data using Dependency Injection (DI) scoping
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>(); //resolve this service
    await dbContext.Database.MigrateAsync(); //run pending migrations
    await DbInitializer.Seed(dbContext);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

    await DbInitializer.SeedUserAndRolesAsync(userManager, roleManager);

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

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
