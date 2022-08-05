using AutoMapper;
using Bot.BusinessLogic.Helper.Mapper;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common.Dto;
using Bot.Models.Data;
using Bot.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.BusinessLogic.Services.Implementations
{
	public class MovieService: IMovieService
    {
        private readonly IMapper _mapper;
        public MovieService()
        {
            var mapperConfiguration = new MapperConfiguration(x =>
            {
                x.AddProfile<MappingProfile>();
            });
            mapperConfiguration.AssertConfigurationIsValid();
            _mapper = mapperConfiguration.CreateMapper();
        }
		public MovieDto ChoiceMovie()
        {
            var movie = new Movie();
            Random random = new Random();
            using (ApplicationContext context = new ApplicationContext())
            {
                int value = random.Next(1, context.Movies.AsNoTracking().ToList().Count-1);
                movie = context.Movies.AsNoTracking().FirstOrDefault(m => m.Id == value);
            }
            MovieDto movieDto = _mapper.Map<MovieDto>(movie);
            return movieDto;
        }

        public IEnumerable<MovieDto> GetSimilar(string genre)
        {
            List<Movie> movies = new List<Movie>();
            using (ApplicationContext context = new ApplicationContext())
            {
                movies = context.Movies.Where(m => EF.Functions.Like(m.Genre, $"%{genre}%")).AsNoTracking().ToList();
            }
            List<MovieDto> moviesDto = _mapper.Map<List<MovieDto>>(movies);
            return moviesDto;
        }
    }
}

