using AutoMapper;
using Bot.Common.Dto;

namespace Bot.BusinessLogic.Services.Interfaces
{
	public interface IMovieService
	{
		IMapper Mapper { get; set; }
		IContentService ContentService { get; set; }
		bool ChoiceCategory { get; set; }

		MovieDto ChoiceMovie();
		MovieDto ChoiceMovie(string value, string action);
		IEnumerable<MovieDto> GetSimilar(string genre);
	}
}

