using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Services.Interfaces
{
	public interface IButtonService
	{
		InlineKeyboardMarkup Buttons(string movie);
		ReplyKeyboardMarkup MenuButton(KeyboardButton[] keyboardButtons);
		InlineKeyboardMarkup Button(string movie);
		ReplyKeyboardMarkup MenuButton();
	}
}

