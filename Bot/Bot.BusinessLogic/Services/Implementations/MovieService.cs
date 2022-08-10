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
        public IContentService ContentService { get; set; }
        public bool ChoiceCategory { get; set; }
        List<MovieDto> moviesDto = new List<MovieDto>();
        private List<int> MovieNumbers { get; set; } = new List<int>();
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
        private List<Movie> Search(string value, ApplicationContext context,string action)
        {
            string numberString = string.Empty;
            for (int i = 3; i < value.Length; i++)
                numberString += value[i];
            try
            {
                int number = Convert.ToInt32(numberString);
                IQueryable<Movie> query = context.Movies;
                if (action != "genre" && action != "country")
                    return null;
                if (action == "genre")
                    query = query.Where(m => m.Genre.Contains(ContentService.GenreList[number]));
                else if(action == "country")
                    query = query.Where(m => m.Country.Contains(ContentService.CountryList[number]));
                return query.AsNoTracking().ToList();
            }
            catch (Exception) {return null;}
        }
        private List<Movie> SearchRelease(string value,ApplicationContext context)
        {
            int startRelease = default, finishRelease=default;
            if (value.Substring(0,1)!="/")
                return null;
            if (value=="/1980")
            {
                startRelease = 1910;
                finishRelease = 1980;
            }
            else if(value == "/1990" || value == "/2000")
            {
                finishRelease = Convert.ToInt32(value.Substring(1));
                startRelease = finishRelease - 10;
            }
            else if (value == "/2005" || value == "/2010" || value == "/2015" || value == "/2020")
            {
                finishRelease = Convert.ToInt32(value.Substring(1));
                startRelease = finishRelease - 5;
            }
            else if (value == "/2022")
            {
                finishRelease = Convert.ToInt32(value.Substring(1));
                startRelease = finishRelease - 2;
            }
            if (startRelease == default)
                return null;
            IQueryable<Movie> query = context.Movies;
            query = query.Where(m => Convert.ToInt32(m.Release) > startRelease && Convert.ToInt32(m.Release) <= finishRelease);
            return query.AsNoTracking().ToList();
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

        private int GenerateNumber(int length)
        {
            Random random = new Random();
            int value, block=default;
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
                if (block > 12)
                {    MovieNumbers = new List<int>();
                    return 1000; }
                block++;
            }
            return value;
        }
    }
}

