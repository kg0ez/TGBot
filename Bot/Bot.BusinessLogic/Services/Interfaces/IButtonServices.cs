using System;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.BusinessLogic.Services.Interfaces
{
	public interface IButtonServices
	{
		InlineKeyboardMarkup Buttons(string movie);
		ReplyKeyboardMarkup MenuButton();
	}
}

