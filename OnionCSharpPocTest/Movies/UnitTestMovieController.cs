using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnionCSharpPoc.Movies;

namespace OnionCSharpPocTest.Movies;

public class UnitTestMovieController
{
    private readonly Mock<IMovieService> _mockService = new();
    private readonly MoviesController _controller;

    public UnitTestMovieController()
    {
        _controller = new MoviesController(_mockService.Object);
    }

    [Fact]
    public void GetAll_ReturnsOkResult_WithListOfMovies()
    {
        // Arrange
        var movie1 = new MovieEntity(1, "Movie 1", "Director 1", 2020);
        var movie2 = new MovieEntity(2, "Movie 2", "Director 2", 2021);
        var movies = new List<MovieEntity> { movie1, movie2 };

        _mockService.Setup(x => x.GetAll()).Returns(movies);

        // Act
        var result = _controller.GetAll();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result.As<OkObjectResult>();
        okResult.Value.Should().BeAssignableTo<IEnumerable<MovieEntity>>();
        var returnedMovies = okResult.Value.As<IEnumerable<MovieEntity>>().ToList();
        returnedMovies.Should().HaveCount(2);
        returnedMovies.Should().Contain(movie1);
        returnedMovies.Should().Contain(movie2);
    }

    [Fact]
    public void GetById_WithValidId_ReturnsOkObjectResult()
    {
        // Arrange
        var id = 1;
        var movie = new MovieEntity(id, "Movie 1", "Director 1", 2020);

        _mockService.Setup(s => s.GetById(id)).Returns(movie);

        // Act
        var result = _controller.GetById(id);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GetById_WithValidId_ReturnsMovieEntity()
    {
        // Arrange
        var id = 1;
        var movie = new MovieEntity(id, "Movie 1", "Director 1", 2020);

        _mockService.Setup(s => s.GetById(id)).Returns(movie);


        // Act
        var result = _controller.GetById(id) as OkObjectResult;
        var returnedMovie = result?.Value as MovieEntity;

        // Assert
        returnedMovie.Should().NotBeNull();
        returnedMovie?.Id.Should().Be(movie.Id);
        returnedMovie?.Title.Should().Be(movie.Title);
        returnedMovie?.Director.Should().Be(movie.Director);
        returnedMovie?.ReleaseYear.Should().Be(movie.ReleaseYear);
    }

    [Fact]
    public void GetById_WithInvalidId_ReturnsNotFoundResult()
    {
        // Arrange
        var id = 1;
        _mockService.Setup(s => s.GetById(id)).Returns((MovieEntity)null);

        // Act
        var result = _controller.GetById(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Add_WithValidModel_ReturnsOkResult()
    {
        // Arrange
        var movieEntity = new MovieEntity(1, "Movie 1", "Director 1", 2020);
        _mockService.Setup(s => s.Add(movieEntity)).Returns(true);

        // Act
        var result = _controller.Add(movieEntity);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public void Add_WithInvalidModel_ReturnsBadRequestResult()
    {
        // Arrange
        var movieEntity = new MovieEntity(1, "Movie 1", "Director 1", 2020);
        _mockService.Setup(s => s.Add(movieEntity)).Returns(false);

        // Act
        var result = _controller.Add(movieEntity);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }
    
    [Fact]
    public void Update_WithValidData_ReturnsOkObjectResult()
    {
        // Arrange
        var id = 1;
        var movieEntity = new MovieEntity(id, "Movie 1", "Director 1", 2020);
        var updatedMovieEntity = new MovieEntity(id, "Updated Movie 1", "Updated Director 1", 2021);

        _mockService.Setup(s => s.Update(movieEntity)).Returns(updatedMovieEntity);

        // Act
        var result = _controller.Update(id, movieEntity);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void Update_WithValidData_ReturnsUpdatedMovieEntity()
    {
        // Arrange
        var id = 1;
        var movieEntity = new MovieEntity(id, "Movie 1", "Director 1", 2020);
        var updatedMovieEntity = new MovieEntity(id, "Updated Movie 1", "Updated Director 1", 2021);

        _mockService.Setup(s => s.Update(movieEntity)).Returns(updatedMovieEntity);

        // Act
        var result = _controller.Update(id, movieEntity) as OkObjectResult;
        var returnedMovieEntity = result?.Value as MovieEntity;

        // Assert
        returnedMovieEntity.Should().NotBeNull();
        returnedMovieEntity?.Id.Should().Be(id);
        returnedMovieEntity?.Title.Should().Be(updatedMovieEntity.Title);
        returnedMovieEntity?.Director.Should().Be(updatedMovieEntity.Director);
        returnedMovieEntity?.ReleaseYear.Should().Be(updatedMovieEntity.ReleaseYear);
    }

    [Fact]
    public void Update_WithInvalidData_ReturnsBadRequestResult()
    {
        // Arrange
        var id = 1;
        var movieEntity = new MovieEntity(id, "Movie 1", "Director 1", 2020);

        // Act
        var result = _controller.Update(id + 1, movieEntity);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public void Update_WithNonexistentData_ReturnsNotFoundResult()
    {
        // Arrange
        var id = 1;
        var movieEntity = new MovieEntity(id, "Movie 1", "Director 1", 2020);

        _mockService.Setup(s => s.Update(movieEntity)).Returns((MovieEntity)null);

        // Act
        var result = _controller.Update(id, movieEntity);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }



    [Fact]
    public void Delete_WithValidId_ReturnsOkResult()
    {
        // Arrange
        int id = 1;
        _mockService.Setup(x => x.GetById(id)).Returns(new MovieEntity(id, "Movie 1", "Director 1", 2021));
        _mockService.Setup(x => x.Delete(It.IsAny<MovieEntity>())).Returns(true);

        // Act
        var result = _controller.Delete(id);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public void Delete_WithInvalidId_ReturnsNotFoundResult()
    {
        // Arrange
        int id = 1;
        _mockService.Setup(x => x.GetById(id)).Returns((MovieEntity)null);

        // Act
        var result = _controller.Delete(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Delete_WithValidIdButDeleteFails_ReturnsBadRequestResult()
    {
        // Arrange
        int id = 1;
        _mockService.Setup(x => x.GetById(id)).Returns(new MovieEntity(id, "Movie 1", "Director 1", 2021));
        _mockService.Setup(x => x.Delete(It.IsAny<MovieEntity>())).Returns(false);

        // Act
        var result = _controller.Delete(id);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }
}



