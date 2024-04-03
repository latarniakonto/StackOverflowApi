using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using StackOverflow.Infrastructure.Clients;
using Microsoft.Extensions.DependencyInjection;
using StackOverflow.Infrastructure.Authorization;
using StackOverflow.Infrastructure.Authentication;

namespace StackOverflow.Infrastructure;

public static class MongoDbInitializer
{
    public static async Task SeedAsync(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            MongoDbContext dbContext = serviceScope.ServiceProvider.GetService<MongoDbContext>();

            if(dbContext == null || dbContext.Tags == null)
                throw new InvalidDataException("MongoDbContext service is missing");

            if (await dbContext.Tags.EstimatedDocumentCountAsync() >= 1000) return;

            await dbContext.Tags.DeleteManyAsync(FilterDefinition<Services.Tag>.Empty);

            ITagsClient client = serviceScope.ServiceProvider.GetService<ITagsClient>();

            List<Services.ResponseTag> tags = await client.GetDataAsync();
            int totalTagsCount = tags.Sum(t => t.Count);
            
            foreach(Services.ResponseTag tag in tags)
            {
                Services.Tag dbTag = new Services.Tag()
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = tag.Name,
                    Count = tag.Count,
                    Weight = (float)tag.Count / totalTagsCount,
                };
                await dbContext.Tags.InsertOneAsync(dbTag);
            }
        }
    }

    public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<TagRole>>();

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new TagRole(UserRoles.Admin));
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new TagRole(UserRoles.User));

            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<TagUser>>();
            string adminUserEmail = "admin@tag.com";

            var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
            if(adminUser == null)
            {
                var newAdminUser = new TagUser()
                {
                    UserName = "admin-tag",
                    Email = adminUserEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newAdminUser, "Password@1234?");
                await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
            }

            string appUserEmail = "user@tag.com";

            var appUser = await userManager.FindByEmailAsync(appUserEmail);
            if (appUser == null)
            {
                var newAppUser = new TagUser()
                {
                    UserName = "user-tag",
                    Email = appUserEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newAppUser, "Password@1234?");
                await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
            }
        }
    }
}
