using Microsoft.AspNetCore.Mvc;

namespace OnionCSharpPoc.Movies
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var movies = _movieService.GetAll();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var movie = _movieService.GetById(id);
            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpPost]
        public IActionResult Add(MovieEntity movieEntity)
        {
            bool result = _movieService.Add(movieEntity);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, MovieEntity movieEntity)
        {
            if (id != movieEntity.Id)
            {
                return BadRequest();
            }

            var result = _movieService.Update(movieEntity);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var movieEntity = _movieService.GetById(id);
            if (movieEntity == null)
            {
                return NotFound();
            }

            bool result = _movieService.Delete(movieEntity);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
