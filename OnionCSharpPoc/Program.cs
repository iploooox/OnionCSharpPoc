using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OnionCSharpPoc.Infrastructure;
using OnionCSharpPoc.Movies;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add middlewares
builder.Services.AddSingleton<ExceptionMiddleware>();

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite("Filename=moviedatabase.db"));

// Add other services to the DI container
builder.Services.AddTransient<IMovieRepository, MovieRepository>();
builder.Services.AddTransient<IMovieService, MovieService>();
builder.Services.AddTransient<IValidator<MovieEntity>, MovieValidator>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

#region CreateDbInMemory
using (var scope = app.Services.CreateScope())
{
    // Get an instance of the DataContext from the DI container
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

    // Ensure that the database schema is created before the tests run
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}
#endregion

app.Run();