using Bot.Services.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Services.Implementations
{
	public class ButtonService: IButtonService
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
                    InlineKeyboardButton.WithCallbackData("Далее", "next"),
                    InlineKeyboardButton.WithCallbackData("Похожий фильм", "similar"),
                }

            });
            return keyboard;
        }

        public InlineKeyboardMarkup Button(string movie)
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
                    InlineKeyboardButton.WithCallbackData("Далее", "Далее"),
                }

            });
            return keyboard;
        }

        public ReplyKeyboardMarkup MenuButton(KeyboardButton[] keyboardButtons)
        {
            ReplyKeyboardMarkup keyboard = new(
            keyboardButtons)
            {
                ResizeKeyboard = true
            };
            return keyboard;
        }

        public ReplyKeyboardMarkup MenuButton()
        {
            ReplyKeyboardMarkup keyboard = new(new[]
            {
                new KeyboardButton[] {  "Жанр", "Страна", "Год"},
                new KeyboardButton[] {"В начало" }
            })
            {
                ResizeKeyboard = true
            };
            return keyboard;
        }
    }
}

