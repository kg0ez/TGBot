using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.BusinessLogic.Services.Interfaces
{
	public interface IButtonService
	{
		InlineKeyboardMarkup Buttons(string movie);
		ReplyKeyboardMarkup MenuButton();
	}
}

