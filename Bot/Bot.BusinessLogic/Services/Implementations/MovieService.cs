using System;
using AutoMapper;
using Bot.BusinessLogic.Helper.Mapper;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Models.Data;
using Bot.Models.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.BusinessLogic.Services.Implementations
{
	public class MovieService: IMovieService
    {
		public Movie ChoiceMovie()
        {
            var movie = new Movie();
            Random random = new Random();
            int value = random.Next(1, 19);
            using (ApplicationContext context = new ApplicationContext())
            {
                movie = context.Movies.AsNoTracking().FirstOrDefault(m => m.Id == value);
            }
            return movie!;
        }

        public IEnumerable<Movie> GetSimilar(string genre)
        {
            List<Movie> movies = new List<Movie>();
            using (ApplicationContext context = new ApplicationContext())
            {
                movies = context.Movies.Where(m => EF.Functions.Like(m.Genre, $"%{genre}%")).AsNoTracking().ToList();
            }
            return movies;
        }
    }
}

