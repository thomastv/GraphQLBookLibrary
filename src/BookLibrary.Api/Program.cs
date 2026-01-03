using BookLibrary.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Entity Framework Core with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add GraphQL Server with Hot Chocolate
builder.Services
    .AddGraphQLServer()
    .AddApiTypes()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddMutationConventions()
    .AddInMemorySubscriptions()
    .RegisterDbContextFactory<AppDbContext>()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = builder.Environment.IsDevelopment());

var app = builder.Build();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync();
    await SeedData.InitializeAsync(context);
}

// Configure HTTP request pipeline
app.UseWebSockets();
app.MapGraphQL();

// Redirect root to GraphQL endpoint
app.MapGet("/", () => Results.Redirect("/graphql"));

app.Run();

// Make Program class accessible for testing
public partial class Program { }
