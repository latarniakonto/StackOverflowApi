using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using StackOverflow.Infrastructure;
using StackOverflow.Infrastructure.Authentication;
using StackOverflow.Infrastructure.Authorization;
using StackOverflow.Infrastructure.Clients;
using StackOverflow.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
var client = new MongoClient("mongodb://localhost:27017");
var database = client.GetDatabase("StackOverflow");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ITagsClient, TagsClient>();
builder.Services.AddSingleton<MongoDbContext>(options => new MongoDbContext(database));
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddIdentity<TagUser, TagRole>().AddMongoDbStores<TagUser, TagRole, ObjectId>
(
    "mongodb://localhost:27017",
    "StackOverflow"
).AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

MongoDbInitializer.SeedAsync(app).Wait();
MongoDbInitializer.SeedUsersAndRolesAsync(app).Wait();

app.Run();
