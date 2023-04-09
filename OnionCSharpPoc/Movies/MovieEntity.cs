namespace OnionCSharpPoc.Movies;

public class MovieEntity
{
    public int Id { get; }
    public string Title { get; }
    public string Director { get; }
    public int ReleaseYear { get; }

    public MovieEntity(int id, string title, string director, int releaseYear)
    {
        Id = id;
        Title = title;
        Director = director;
        ReleaseYear = releaseYear;
    }
}