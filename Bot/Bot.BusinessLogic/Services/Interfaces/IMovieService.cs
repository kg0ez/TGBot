using System;
using Bot.Models.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.BusinessLogic.Services.Interfaces
{
	public interface IMovieService
	{
		Movie ChoiceMovie();
		IEnumerable<Movie> GetSimilar(string genre);
	}
}

