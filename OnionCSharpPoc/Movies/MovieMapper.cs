namespace OnionCSharpPoc.Movies;

public static class MovieMapper
{
    public static MovieEntity ToMovieEntity(this MovieModel movieModel)
    {
        return new MovieEntity(
            id: movieModel.Id,
            title: movieModel.Title,
            director: movieModel.Director,
            releaseYear: movieModel.ReleaseYear
        );
    }

    public static MovieModel ToMovieModel(this MovieEntity movieEntity)
    {
        return new MovieModel
        {
            Id = movieEntity.Id,
            Title = movieEntity.Title,
            Director = movieEntity.Director,
            ReleaseYear = movieEntity.ReleaseYear
        };
    }
}