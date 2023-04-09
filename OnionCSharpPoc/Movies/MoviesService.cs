using FluentValidation;

namespace OnionCSharpPoc.Movies;

public interface IMovieService
{
    IReadOnlyCollection<MovieEntity> GetAll();
    MovieEntity? GetById(int id);
    bool Add(MovieEntity movieEntity);
    MovieEntity? Update(MovieEntity movieEntity);
    bool Delete(MovieEntity movieEntity);
}

public class MovieService : IMovieService
{
    private readonly IMovieRepository _repository;
    private readonly IValidator<MovieEntity> _validator;
    private readonly ILogger<MovieService> _logger;

    public MovieService(IMovieRepository repository, IValidator<MovieEntity> validator, ILogger<MovieService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public IReadOnlyCollection<MovieEntity> GetAll()
    {
        return _repository.GetAll()
            .Select(m => new MovieEntity(m.Id, m.Title, m.Director, m.ReleaseYear))
            .ToList()
            .AsReadOnly();
    }

    public MovieEntity? GetById(int id)
    {
        MovieEntity? movie = _repository.GetById(id);
        if (movie == null)
        {
            _logger.LogWarning("Trying to access movie that does not exist with id: {id}", id);
            return null;
        }

        return movie;
    }

    public bool Add(MovieEntity movieEntity)
    {
        _validator.ValidateAndThrow(movieEntity);
        bool result = _repository.Add(movieEntity);

        if (!result)
        {
            _logger.LogWarning("Failed to add movie");
        }
        
        return result;
    }

    public MovieEntity? Update(MovieEntity movieEntity)
    {
        _validator.ValidateAndThrow(movieEntity);

        MovieEntity? movieModel = _repository.GetById(movieEntity.Id);
        if (movieModel == null)
        {
            _logger.LogWarning("Trying to update movie that does not exist with id: {id}", movieEntity.Id);
            return null;
        }

        _repository.Update(movieModel);

        return movieEntity;
    }

    public bool Delete(MovieEntity movieEntity)
    {
        bool result = _repository.Delete(movieEntity);

        if (!result)
        {
            _logger.LogWarning("Failed to delete movie with id: {id}", movieEntity.Id);
        }

        return result;
    }
}