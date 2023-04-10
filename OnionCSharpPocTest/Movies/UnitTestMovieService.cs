using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using Moq;
using OnionCSharpPoc.Movies;

namespace OnionCSharpPocTest.Movies;

public class UnitTestMovieService
{
    private readonly Mock<IMovieRepository> _mockRepository = new();
    private readonly Mock<ILogger<MovieService>> _mockLogger = new();
    private readonly MovieValidator _validator = new();
    private readonly MovieService _service;

    public UnitTestMovieService()
    {
        _service = new MovieService(_mockRepository.Object, _validator, _mockLogger.Object);
    }

    [Fact]
    public void GetAll_ReturnsListOfMovies()
    {
        // Arrange
        var movies = new List<MovieModel>
        {
            new() { Id = 1, Title = "Movie 1", Director = "Director 1", ReleaseYear = 2020 },
            new() { Id = 2, Title = "Movie 2", Director = "Director 2", ReleaseYear = 2021 }
        };
        _mockRepository.Setup(r => r.GetAll()).Returns(movies.Select(x => x.ToMovieEntity()));

        // Act
        var result = _service.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Select(m => m.Id).Should().BeEquivalentTo(new[] { 1, 2 });
        result.Select(m => m.Title).Should().BeEquivalentTo("Movie 1", "Movie 2");
        result.Select(m => m.Director).Should().BeEquivalentTo("Director 1", "Director 2");
        result.Select(m => m.ReleaseYear).Should().BeEquivalentTo(new[] { 2020, 2021 });
    }

    [Fact]
    public void GetById_WithValidId_ReturnsMovieEntity()
    {
        // Arrange
        var id = 1;
        var movieModel = new MovieModel { Id = id, Title = "Movie 1", Director = "Director 1", ReleaseYear = 2020 };
        var expectedMovieEntity = new MovieEntity(id, "Movie 1", "Director 1", 2020);
        _mockRepository.Setup(r => r.GetById(id)).Returns(movieModel.ToMovieEntity);

        // Act
        var result = _service.GetById(id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedMovieEntity);
    }

    [Fact]
    public void GetById_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var id = 1;
        _mockRepository.Setup(r => r.GetById(id)).Returns((MovieEntity)null);

        // Act
        var result = _service.GetById(id);

        // Assert
        result.Should().BeNull();
        _mockLogger.VerifyLog(logger =>
            logger.LogWarning("Trying to access movie that does not exist with id: {id}", id));
    }

    [Fact]
    public void Add_ValidMovieEntity_ReturnsTrue()
    {
        // Arrange
        var movieEntity = new MovieEntity(1, "Movie 1", "Director 1", 2020);
        _mockRepository.Setup(r => r.Add(movieEntity)).Returns(true);

        // Act
        var result = _service.Add(movieEntity);

        // Assert
        result.Should().BeTrue();
        _mockLogger.VerifyNoOtherCalls();
    }

    [Fact]
    public void Add_InvalidMovieEntity_ThrowsValidationException()
    {
        // Arrange
        var movieEntity = new MovieEntity(1, "", "Director 1", 2020);

        // Act
        var action = new Action(() => _service.Add(movieEntity));

        // Assert
        action.Should().Throw<ValidationException>();
        _mockRepository.VerifyNoOtherCalls();
        _mockLogger.VerifyNoOtherCalls();
    }

    [Fact]
    public void Add_FailedToAdd_ReturnsFalse()
    {
        // Arrange
        var movieEntity = new MovieEntity(1, "Movie 1", "Director 1", 2020);
        _mockRepository.Setup(r => r.Add(movieEntity)).Returns(false);

        // Act
        var result = _service.Add(movieEntity);

        // Assert
        result.Should().BeFalse();
        _mockLogger.VerifyLog(logger => logger.LogWarning("Failed to add movie"));
    }

    [Fact]
    public void Add_Result_WithInvalidModel_ReturnsValidationErrors()
    {
        // Arrange
        var movieEntity = new MovieEntity(1, "", "", 0);

        // Act
        var result = new MovieService(_mockRepository.Object, _validator, _mockLogger.Object).AddWithResult(movieEntity);

        // Assert
        result.IsFaulted.Should().BeTrue();
        result.IfFail(exception =>
        {
            var errors = ((ValidationException)exception).Errors.ToList();
            errors.Should().HaveCount(3);
            errors[0].PropertyName.Should().Be("Title");
            errors[1].PropertyName.Should().Be("Director");
            errors[2].PropertyName.Should().Be("ReleaseYear");
        });
    }

    [Fact]
    public void Add_Result_WithValidModel_ReturnsSuccessResult()
    {
        // Arrange
        var movieEntity = new MovieEntity(1, "Movie 1", "Director 1", 2020);
        _mockRepository.Setup(r => r.Add(movieEntity)).Returns(true);

        // Act
        var result = new MovieService(_mockRepository.Object, _validator, _mockLogger.Object).AddWithResult(movieEntity);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IfSucc(value => value.Should().BeTrue());
    }

    [Fact]
    public void Add_Result_WithRepositoryFailure_ReturnsFailureResult()
    {
        // Arrange
        var movieEntity = new MovieEntity(1, "Movie 1", "Director 1", 2020);
        _mockRepository.Setup(r => r.Add(movieEntity)).Returns(false);

        // Act
        var result = new MovieService(_mockRepository.Object, _validator, _mockLogger.Object).AddWithResult(movieEntity);

        // Assert
        result.Match(x => x, exception => false).Should().BeFalse();
    }


    [Fact]
    public void Update_WithValidMovie_ReturnsUpdatedMovie()
    {
        // Arrange
        var movie = new MovieEntity(1, "The Matrix", "Lana Wachowski", 1999);
        var updatedMovie = new MovieEntity(1, "The Matrix: Resurrections", "Lana Wachowski", 2021);
        _mockRepository.Setup(r => r.GetById(movie.Id)).Returns(movie);
        _mockRepository.Setup(r => r.Update(movie));

        // Act
        var result = _service.Update(updatedMovie);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(updatedMovie);
    }

    [Fact]
    public void Update_WithInvalidMovie_ReturnsNull()
    {
        // Arrange
        var movie = new MovieEntity(1, "The Matrix", "Lana Wachowski", 1999);
        var updatedMovie = new MovieEntity(1, "The Matrix: Resurrections", "Lana Wachowski", 2021);
        _mockRepository.Setup(r => r.GetById(movie.Id)).Returns((MovieEntity)null);

        // Act
        var result = _service.Update(updatedMovie);

        // Assert
        result.Should().BeNull();
        _mockLogger.VerifyLog(logger =>
            logger.LogWarning("Trying to update movie that does not exist with id: {id}", movie.Id));
    }

    [Fact]
    public void Delete_ReturnsTrue_WhenRepositoryReturnsTrue()
    {
        // Arrange
        var movie = new MovieEntity(1, "Movie 1", "Director 1", 2021);
        _mockRepository.Setup(r => r.Delete(movie)).Returns(true);

        // Act
        var result = _service.Delete(movie);

        // Assert
        result.Should().BeTrue();
        _mockLogger.VerifyLog(logger => logger.LogWarning(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Delete_ReturnsFalse_WhenRepositoryReturnsFalse()
    {
        // Arrange
        var movie = new MovieEntity(1, "Movie 1", "Director 1", 2021);
        _mockRepository.Setup(r => r.Delete(movie)).Returns(false);

        // Act
        var result = _service.Delete(movie);

        // Assert
        result.Should().BeFalse();
        _mockLogger.VerifyLog(logger => logger.LogWarning("Failed to delete movie with id: {id}", movie.Id),
            Times.Once);
    }
    
    [Fact]
    public async Task AddResultAsync_Should_Return_SuccessResult_When_Model_Is_Valid()
    {
        // Arrange
        var movieEntity = new MovieEntity(1, "Movie 1", "Director 1", 2020);
        _mockRepository.Setup(x => x.AddAsync(movieEntity)).ReturnsAsync(new Result<MovieEntity>(movieEntity));

        // Act
        var result = await _service.AddResultAsync(movieEntity);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Match(movie =>
            {
                movie.Should().Be(movieEntity);
                return true;
            }, 
            _ => false).Should().BeTrue();
    }

    [Fact]
    public async Task AddResultAsync_Should_Return_ValidationErrors_When_Model_Is_Invalid()
    {
        // Arrange
        var movieEntity = new MovieEntity(1, "", "Director 1", 2020); // invalid model

        // Act
        var result = await _service.AddResultAsync(movieEntity);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Match(
            movie => true,
            error =>
            {
                error.Should().BeOfType<ValidationException>();
                var validationException = (ValidationException)error;
                validationException.Errors.Should().HaveCount(1);
                validationException.Errors.First().PropertyName.Should().Be("Title");
                validationException.Errors.First().ErrorMessage.Should().Be("'Title' must not be empty.");
                return false;
            }).Should().BeFalse();
    }

}