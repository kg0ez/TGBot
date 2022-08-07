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
            
            using (ApplicationContext context = new ApplicationContext())
            {
                int value = GenerateNumber(context.Movies.AsNoTracking().ToList().Count - 1);
                
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
        private List<int> MovieNumbers { get; set; } = new List<int>();

        private int GenerateNumber(int length)
        {
            Random random = new Random();
            int value;
            if (MovieNumbers.Count > 30)
                MovieNumbers = new List<int>();
            while (true)
            {
                bool content = false;
                value = random.Next(1, length);
                foreach (var number in MovieNumbers)
                {
                    if (number == value)
                    {
                        content = true;
                        break;
                    }
                }
                if (!content)
                {
                    MovieNumbers.Add(value);
                    break;
                }
            }
            return value;
        }
    }
}

