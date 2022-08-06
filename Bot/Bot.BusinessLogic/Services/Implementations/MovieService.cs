using AutoMapper;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common.Dto;
using Bot.Models.Data;
using Bot.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.BusinessLogic.Services.Implementations
{
	public class MovieService: IMovieService
    {
        public IMapper Mapper { get; set; }

        public MovieDto ChoiceMovie()
        {
            var movie = new Movie();
            Random random = new Random();
            using (ApplicationContext context = new ApplicationContext())
            {
                int value = random.Next(1, context.Movies.AsNoTracking().ToList().Count-1);
                movie = context.Movies.AsNoTracking().FirstOrDefault(m => m.Id == value);
            }
            MovieDto movieDto = Mapper.Map<MovieDto>(movie);
            return movieDto;
        }

        public IEnumerable<MovieDto> GetSimilar(string genre)
        {
            List<Movie> movies = new List<Movie>();
            using (ApplicationContext context = new ApplicationContext())
            {
                movies = context.Movies.Where(m => EF.Functions.Like(m.Genre, $"%{genre}%")).AsNoTracking().ToList();
            }
            List<MovieDto> moviesDto = Mapper.Map<List<MovieDto>>(movies);
            return moviesDto;
        }
    }
}

