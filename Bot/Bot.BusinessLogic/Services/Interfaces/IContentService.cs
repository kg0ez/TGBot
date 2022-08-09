using System;
namespace Bot.BusinessLogic.Services.Interfaces
{
	public interface IContentService
	{
		List<string> CountryList { get; }
		string Release { get; }
		List<string> GenreList { get; }
	}
}

