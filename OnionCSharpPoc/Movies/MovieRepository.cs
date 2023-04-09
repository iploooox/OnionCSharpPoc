using OnionCSharpPoc.Infrastructure;

namespace OnionCSharpPoc.Movies;

    public interface IMovieRepository
    {
        IEnumerable<MovieEntity> GetAll();
        MovieEntity? GetById(int id);
        bool Add(MovieEntity movieEntity);
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
        return movieModels.Select(movieModel => MovieMapper.ToMovieEntity(movieModel));
    }

    public MovieEntity? GetById(int id)
    {
        MovieModel? movieModel = _context.Movies.FirstOrDefault(m => m.Id == id);
        return movieModel != null ? MovieMapper.ToMovieEntity(movieModel) : null;
    }

    public bool Add(MovieEntity movieEntity)
    {
        MovieModel movieModel = MovieMapper.ToMovieModel(movieEntity);
        _context.Movies.Add(movieModel);
        _context.SaveChanges();
        return true;
    }

    public void Update(MovieEntity movieEntity)
    {
        MovieModel movieModel = MovieMapper.ToMovieModel(movieEntity);
        _context.Movies.Update(movieModel);
        _context.SaveChanges();
    }

    public bool Delete(MovieEntity movieEntity)
    {
        MovieModel movieModel = MovieMapper.ToMovieModel(movieEntity);
        _context.Movies.Remove(movieModel);
        _context.SaveChanges();
        return true;
    }
}
