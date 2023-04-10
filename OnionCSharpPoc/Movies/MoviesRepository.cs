using LanguageExt.Common;
using OnionCSharpPoc.Infrastructure;

namespace OnionCSharpPoc.Movies;

    public interface IMovieRepository
    {
        IEnumerable<MovieEntity> GetAll();
        MovieEntity? GetById(int id);
        bool Add(MovieEntity movieEntity);
        Task<Result<MovieEntity>> AddAsync(MovieEntity movieEntity);
        void Update(MovieEntity movieEntity);
        bool Delete(MovieEntity movieEntity);
    }

public class MovieRepository : IMovieRepository
{
    private readonly DataContext _context;

    public MovieRepository(DataContext context)
    {
        _context = context;
    }

    public IEnumerable<MovieEntity> GetAll()
    {
        List<MovieModel> movieModels = _context.Movies.ToList();
        return movieModels.Select(movieModel => movieModel.ToMovieEntity());
    }

    public MovieEntity? GetById(int id)
    {
        MovieModel? movieModel = _context.Movies.FirstOrDefault(m => m.Id == id);
        return movieModel?.ToMovieEntity();
    }

    public bool Add(MovieEntity movieEntity)
    {
        MovieModel movieModel = movieEntity.ToMovieModel();
        _context.Movies.Add(movieModel);
        _context.SaveChanges();
        return true;
    }

    public async Task<Result<MovieEntity>> AddAsync(MovieEntity movieEntity)
    {
        MovieModel movieModel = movieEntity.ToMovieModel();
        await _context.Movies.AddAsync(movieModel);
        await _context.SaveChangesAsync();
        return movieModel.ToMovieEntity();
    }

    public void Update(MovieEntity movieEntity)
    {
        MovieModel movieModel = movieEntity.ToMovieModel();
        _context.Movies.Update(movieModel);
        _context.SaveChanges();
    }

    public bool Delete(MovieEntity movieEntity)
    {
        MovieModel movieModel = movieEntity.ToMovieModel();
        _context.Movies.Remove(movieModel);
        _context.SaveChanges();
        return true;
    }
}
