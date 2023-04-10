using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnionCSharpPoc.Infrastructure;
using OnionCSharpPoc.Movies;

namespace OnionCSharpPocTest.Movies;

[Collection("Non-parallel tests")]
public class UnitTestMovieRepository
{
    private readonly DataContext _context;
    private readonly IMovieRepository _repository;

    public UnitTestMovieRepository()
    {
        // Configure the DI container with the DataContext using an in-memory SQLite database
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddDbContext<DataContext>(options => options.UseSqlite("Filename=moviedatabase.db"))
            .BuildServiceProvider();

        // Get an instance of the DataContext from the DI container
        _context = serviceProvider.GetRequiredService<DataContext>();

        // Ensure that the database schema is created before the tests run
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _repository = new MovieRepository(_context);
    }

    [Fact]
    public void GetAll_ShouldReturnAllMovies()
    {
        // Arrange
        _context.Movies.ExecuteDelete();
        MovieModel movie1 = new MovieModel { Title = "Movie 1", Director = "Director 1", ReleaseYear = 2020 };
        MovieModel movie2 = new MovieModel { Title = "Movie 2", Director = "Director 2", ReleaseYear = 2021 };
        _context.Movies.AddRange(movie1, movie2);
        _context.SaveChanges();

        // Act
        List<MovieEntity> result = _repository.GetAll().ToList();

        // Assert
        MovieEntity movie1Entity = movie1.ToMovieEntity();
        MovieEntity movie2Entity = movie2.ToMovieEntity();
        
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Title == movie1Entity.Title);
        result.Should().Contain(x => x.Title == movie2Entity.Title);
    }

    [Fact]
    public void GetById_ShouldReturnMovieById()
    {
        // Arrange
        MovieModel movie1 = new MovieModel { Title = "Movie 1", Director = "Director 1", ReleaseYear = 2020 };
        MovieModel movie2 = new MovieModel { Title = "Movie 2", Director = "Director 2", ReleaseYear = 2021 };
        _context.Movies.AddRange(movie1, movie2);
        _context.SaveChanges();

        // Act
        MovieEntity? result = _repository.GetById(movie2.Id);

        // Assert
        result.Should().BeEquivalentTo(movie2);
    }

    [Fact]
    public void Add_ShouldAddNewMovie()
    {
        // Arrange
        MovieModel movie = new MovieModel { Title = "New Movie", Director = "New Director", ReleaseYear = 2022 };

        // Act
        _repository.Add(movie.ToMovieEntity());

        // Assert
        _context.Movies.Should().Contain(x => x.Title == movie.Title);
    }
    
    [Fact]
    public async Task AddAsync_ShouldAddNewMovieAsync()
    {
        // Arrange
        var movieEntity = new MovieEntity(1, "Movie 1", "Director 1", 2020);

        // Act
        var result = await _repository.AddAsync(movieEntity);

        // Assert
        var expectedMovie = new MovieEntity(1, "Movie 1", "Director 1", 2020);
        result.IfSucc(x => x.Should().BeEquivalentTo(expectedMovie));
        _context.Movies.Should().ContainEquivalentOf(expectedMovie.ToMovieModel());
    }

    [Fact]
    public void Update_ShouldUpdateExistingMovie()
    {
        // Arrange
        MovieModel movie = new MovieModel { Id = 99999, Title = "Movie", Director = "Director", ReleaseYear = 2020 };
        _context.Movies.Add(movie);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();

        MovieModel updatedMovie = new MovieModel { Id = movie.Id, Title = "Updated Movie", Director = "Updated Director", ReleaseYear = 2021 };

        // Act
        _repository.Update(updatedMovie.ToMovieEntity());

        // Assert
        MovieEntity expectedMovie = updatedMovie.ToMovieEntity();
        MovieEntity? actualMovie = _context.Movies.FirstOrDefault(m => m.Id == updatedMovie.Id)?.ToMovieEntity();

        actualMovie.Should().NotBeNull();
        actualMovie.Should().BeEquivalentTo(expectedMovie);
    }

    [Fact]
    public void Delete_ShouldDeleteExistingMovie()
    {
        // Arrange
        MovieModel movie = new MovieModel { Title = "Movie", Director = "Director", ReleaseYear = 2020 };
        _context.Movies.Add(movie);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();

        // Act
        _repository.Delete(movie.ToMovieEntity());

        // Assert
        _context.Movies.Should().NotContain(movie);
    }
}