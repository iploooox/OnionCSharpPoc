using System.ComponentModel.DataAnnotations.Schema;

namespace OnionCSharpPoc.Movies;

[Table("Movie")]
public class MovieModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Director { get; set; }
    public int ReleaseYear { get; set; }
}