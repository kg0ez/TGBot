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
        public bool ChoiceCategory { get; set; }
        List<MovieDto> moviesDto = new List<MovieDto>();
        private List<int> MovieNumbers { get; set; } = new List<int>();


        public MovieDto ChoiceMovie()
        {
            var movie = new Movie();
            
            using (ApplicationContext context = new ApplicationContext())
            {
                int count = context.Movies.AsNoTracking().ToList().Count - 1;
                if (count <1)
                    return null;
                int value = GenerateNumber(context.Movies.AsNoTracking().ToList().Count - 1);
                
                movie = context.Movies.AsNoTracking().FirstOrDefault(m => m.Id == value);
            }
            MovieDto movieDto = Mapper.Map<MovieDto>(movie);
            return movieDto;
        }

        public MovieDto ChoiceMovie(string value, string action)
        {
            if (moviesDto.Count==0 || !ChoiceCategory)
            {
                List<Movie> movies = new List<Movie>();
                using (ApplicationContext context = new ApplicationContext())
                {
                    if (action == "country")
                        movies = context.Movies.Where(m => m.Country.Contains(value)).AsNoTracking().ToList();
                    else if (action == "genre")
                        movies = context.Movies.Where(m => m.Genre.Contains(value)).AsNoTracking().ToList();
                    else if (action == "release")
                        movies = context.Movies.Where(m => m.Release.Contains(value)).AsNoTracking().ToList();
                }
                moviesDto = Mapper.Map<List<MovieDto>>(movies);
                ChoiceCategory = true;
            }
            if (moviesDto.Count<1)
                return null;
            var number = GenerateNumber(moviesDto.Count);
            return moviesDto[number];
        }

        public IEnumerable<MovieDto> GetSimilar(string genre)
        {
            List<Movie> movies = new List<Movie>();

            using (ApplicationContext context = new ApplicationContext())
                movies = context.Movies.Where(m => m.Genre.Contains(genre)).AsNoTracking().ToList();

            List<MovieDto> moviesDto = Mapper.Map<List<MovieDto>>(movies);
            return moviesDto;
        }

        private int GenerateNumber(int length)
        {
            Random random = new Random();
            int value;
            if (MovieNumbers.Count > length-1)
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

