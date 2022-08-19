using System;
namespace Bot.BusinessLogic.Services.Interfaces
{
	public interface IContentService
	{
		List<string> CountryList { get; }
		List<string> ReleaseList { get; }
		List<string> GenreList { get; }
	}
}

