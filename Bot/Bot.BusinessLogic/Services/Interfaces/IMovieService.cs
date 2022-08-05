using AutoMapper;
using Bot.Common.Dto;

namespace Bot.BusinessLogic.Services.Interfaces
{
	public interface IMovieService
	{
		IMapper Mapper { get; set; }
		MovieDto ChoiceMovie();
		IEnumerable<MovieDto> GetSimilar(string genre);
	}
}

