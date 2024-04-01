using MongoDB.Driver;
using StackOverflow.Infrastructure;
using StackOverflow.Infrastructure.Clients;

var builder = WebApplication.CreateBuilder(args);
var client = new MongoClient("mongodb://localhost:27017");
var database = client.GetDatabase("StackOverflow");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IClientFactory, ClientFactory>();
builder.Services.AddSingleton<MongoDbContext>(options => new MongoDbContext(database));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

MongoDbInitializer.Seed(app).Wait();

app.Run();
