using AutoMapper;
using Bot.BusinessLogic.Helper;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common.Dto;
using Bot.Models.Data;
using Bot.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.BusinessLogic.Services.Implementations
{
	public class MovieService : Setting, IMovieService
    {
        public IMapper Mapper { get; set; }
        public IContentService ContentService { get; set; }
        public bool ChoiceCategory { get; set; }
        private List<MovieDto> moviesDto = new List<MovieDto>();
        private int _count = default;

        public MovieDto ChoiceMovie()
        {
            var movie = new Movie();
            
            using (ApplicationContext context = new ApplicationContext())
            {
                int count = context.Movies.AsNoTracking().ToList().Count - 1;
                if (count <1)
                    return null;
                if (_count == default)
                    _count = context.Movies.AsNoTracking().ToList().Count - 1;
                int value = GenerateNumber(_count);
                
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
                        movies = Search(value, context, action);
                    else if (action == "genre")
                        movies = Search(value,context,action);
                    else if (action == "release")
                        movies = SearchRelease(value,context);
                }
                moviesDto = Mapper.Map<List<MovieDto>>(movies);
                ChoiceCategory = true;
            }
            if (moviesDto.Count == 1)
                return moviesDto[0];
            if (moviesDto.Count<1)
                return null;
            try
            {
                var number = GenerateNumber(moviesDto.Count);
                return moviesDto[number];
            }
            catch (Exception) { return null; }
        }
        
        public IEnumerable<MovieDto> GetSimilar(string genre)
        {
            List<Movie> movies = new List<Movie>();

            using (ApplicationContext context = new ApplicationContext())
            {
                if (string.IsNullOrWhiteSpace(genre))
                    return null;
                IQueryable<Movie> query = context.Movies;
                query = query.Where(x => x.Genre.Contains(genre));
                movies = query.AsNoTracking().ToList();
            }

            List<MovieDto> moviesDto = Mapper.Map<List<MovieDto>>(movies);
            return moviesDto;
        }

    }
}

