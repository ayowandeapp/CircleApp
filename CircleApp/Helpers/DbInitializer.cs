using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Data;
using CircleApp.Helpers.Constants;
using CircleApp.Models;
using Microsoft.AspNetCore.Identity;

namespace CircleApp.Helpers
{
    public static class DbInitializer
    {
        public static async Task SeedUserAndRolesAsync(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                foreach (var rolesName in AppRoles.All)
                {
                    if (!await roleManager.RoleExistsAsync(rolesName))
                    {
                        await roleManager.CreateAsync(new IdentityRole<int>(rolesName));
                    }
                }
            }
            //users with roles
            if (!userManager.Users.Any(n => !string.IsNullOrEmpty(n.Email)))
            {
                var userPassword = "Coding@1234?";
                var newUser = new User()
                {
                    UserName = "Ayo.wande",
                    Email = "ayo@hotmail.com",
                    Fullname = "Ayowande Oluwatosin",
                    ProfilePictureUrl = "https://img-b.udemycdn.com/user/200_H/16004620_10db_5.jpg",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(newUser, userPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(newUser, AppRoles.User);
                    
                
                var newAdmin = new User()
                {
                    UserName = "admin.admin",
                    Email = "admin@homail.com",
                    Fullname = "Ervis Admin",
                    ProfilePictureUrl = "https://img-b.udemycdn.com/user/200_H/16004620_10db_5.jpg",
                    EmailConfirmed = true
                };

                var resultNewAdmin = await userManager.CreateAsync(newAdmin, userPassword);
                if (resultNewAdmin.Succeeded)
                    await userManager.AddToRoleAsync(newAdmin, AppRoles.Admin);
            }

        }
        public static async Task Seed(AppDbContext appDbContext)
        {

            // if (!appDbContext.Users.Any() && !appDbContext.Posts.Any())
            // {
            //     User newUser = new()
            //     {
            //         Fullname = "Ayowande Oluwatosin",
            //         ProfilePictureUrl = "https://img-b.udemycdn.com/user/200_H/16004620_10db_5.jpg"
            //     };
            //     await appDbContext.Users.AddAsync(newUser);
            //     await appDbContext.SaveChangesAsync();

            //     var newPostWithoutImage = new Post()
            //     {
            //         Content = "This is going to be our first post which is being loaded from the database and it has been created using our test user.",
            //         ImageUrl = "",
            //         NrOfReports = 0,
            //         DateCreated = DateTime.UtcNow,
            //         DateUpdated = DateTime.UtcNow,

            //         UserId = newUser.Id
            //     };

            //     var newPostWithImage = new Post()
            //     {
            //         Content = "This is going to be our first post which is being loaded from the database and it has been created using our test user. This post has an image",
            //         ImageUrl = "https://unsplash.com/photos/foggy-mountain-summit-1Z2niiBPg5A",
            //         NrOfReports = 0,
            //         DateCreated = DateTime.UtcNow,
            //         DateUpdated = DateTime.UtcNow,

            //         UserId = newUser.Id
            //     };

            //     await appDbContext.Posts.AddRangeAsync(newPostWithoutImage, newPostWithImage);
            //     await appDbContext.SaveChangesAsync();


            // }

        }
        
    }
}