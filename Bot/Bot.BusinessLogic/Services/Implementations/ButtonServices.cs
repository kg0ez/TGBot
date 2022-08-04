using System;
using Bot.BusinessLogic.Services.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.BusinessLogic.Services.Implementations
{
	public class ButtonServices: IButtonServices
	{
        public InlineKeyboardMarkup Buttons(string movie)
        {
            InlineKeyboardMarkup keyboard = new(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithUrl(
                    text: "Ссылка на фильм",
                    url: $"{movie}"
                    )
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Похожий фильм", "similar"),
                    InlineKeyboardButton.WithCallbackData("Далее", "next"),
                }

            });
            return keyboard;
        }

        public ReplyKeyboardMarkup MenuButton()
        {
            ReplyKeyboardMarkup keyboard = new(
            new KeyboardButton[] { "Начать" })
            {
                ResizeKeyboard = true
            };
            return keyboard;
        }
    }
}

