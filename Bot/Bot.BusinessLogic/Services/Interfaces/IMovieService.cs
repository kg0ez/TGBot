using Bot.Common.Dto;

namespace Bot.BusinessLogic.Services.Interfaces
{
	public interface IMovieService
	{
		MovieDto ChoiceMovie();
		IEnumerable<MovieDto> GetSimilar(string genre);
	}
}

