using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OnionCSharpPoc.Infrastructure.Extensions;
using OnionCSharpPoc.Movies;

namespace OnionCSharpPoc.Benchmarks;

[SimpleJob(RunStrategy.ColdStart, iterationCount: 2)]
[MemoryDiagnoser(false)]
public class MoviesBenchmark
{
    private IMovieRepository _mockRepository;
    private ILogger<MovieService> _logger;
    private IValidator<MovieEntity> _validator;
    
    [Params(1000, 2500, 5000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // Configure the DI container with the DataContext using an in-memory SQLite database
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddTransient<IValidator<MovieEntity>, MovieValidator>()
            .AddLogging(builder => { builder.AddConsole(); })
            .BuildServiceProvider();

        _logger = serviceProvider.GetRequiredService<ILogger<MovieService>>();
        _validator = serviceProvider.GetRequiredService<IValidator<MovieEntity>>();
        _mockRepository = Substitute.For<IMovieRepository>();
    }

    [Benchmark]
    public bool Add_WithInvalidModel_ReturnsBadRequestResult_Count()
    {
        for (int i = 0; i < Count; i++)
        {
            var value = Add_WithInvalidModel_ReturnsBadRequestResult();
            
            // All the value jazz is here only for compiler to not optimize it away.
            if (value)
            {
                return value;
            }
        }

        return false;
    }
    
    [Benchmark]
    public bool Add_WithInvalidModel_ReturnsBadRequestResult_With_Result_Count()
    {
        for (int i = 0; i < Count; i++)
        {
            var value = Add_WithInvalidModel_ReturnsBadRequestResult_With_Result();

            // All the value jazz is here only for compiler to not optimize it away.
            if (value)
            {
                return value;
            }
        }

        return false;
    }

    [Benchmark]
    public async Task<MovieEntity?[]> Add_WithInvalidModel_ReturnsBadRequestResult_With_Result_And_Async_Count()
    {
        var tasks = new List<Task<MovieEntity?>>();
        for (var i = 0; i < Count; i++)
        {
            tasks.Add(
                Add_WithInvalidModel_ReturnsBadRequestResult_With_Result_And_Async()); // add the task for this movie entity to the list
        }

        // Wait for all tasks to complete and collect their results
        var results = await Task.WhenAll(tasks);
        
        // All the value jazz is here only for compiler to not optimize it away.
        return results;
    }

    private bool Add_WithInvalidModel_ReturnsBadRequestResult()
    {
        // Arrange
        var service = new MovieService(_mockRepository, _validator, _logger);

        var movieEntity = new MovieEntity(0, "", "", 0);
        _mockRepository.Add(movieEntity).Returns(true);

        // Act
        try
        {
            var result = service.Add(movieEntity);
            return result;
        }
        catch (ValidationException validationException)
        {
            // simulate middleware.
            var json = validationException.ToJsonString();
            _logger.LogWarning(json);
            return false;
        }
    }
    
    private bool Add_WithInvalidModel_ReturnsBadRequestResult_With_Result()
    {
        // Arrange
        var service = new MovieService(_mockRepository, _validator, _logger);

        var movieEntity = new MovieEntity(0, "", "", 0);
        _mockRepository.Add(movieEntity).Returns(true);

        // Act
        var result = service.AddWithResult(movieEntity);

        return result.Match(success => success, exception =>
        {
            if (exception is ValidationException validationException)
            {
                _logger.LogWarning(validationException.ToJsonString());
                return false;
            }
            
            _logger.LogError("kaboom 500");

            return false;
        });
    }
    
    private async Task<MovieEntity?> Add_WithInvalidModel_ReturnsBadRequestResult_With_Result_And_Async()
    {
        // Arrange
        var service = new MovieService(_mockRepository, _validator, _logger);

        var movieEntity = new MovieEntity(0, "", "", 0);
        _mockRepository.Add(movieEntity).Returns(true);

        // Act
        var result = await service.AddResultAsync(movieEntity);

        return result.Match(success => success, exception =>
        {
            if (exception is ValidationException validationException)
            {
                _logger.LogWarning(validationException.ToJsonString());
                return null;
            }
            
            _logger.LogError("kaboom 500");

            return null;
        });
    }
}