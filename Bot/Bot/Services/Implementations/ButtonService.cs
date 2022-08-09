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
        public List<string> GenreList { get; } = new List<string> {
            "Боевик","Приключение","Мультфильм","Комедия", "Криминал",
            "Документальный", "Драма", "Семейный","Фэнтези","История","Ужасы",
            "Музыка","Детектив","Мелодрама","Фантастика","Триллер","Военный","Вестерн"};

        public List<string> CountryList { get; } = new List<string> {
            "Австралия", "Великобритания", "Франция", "Мексика", "США", "Южная Корея",
            "Гонконг", "Китай", "Марокко", "Аргентина", "Испания", "Уругвай",
            "Беларусь", "Великобритания", "Ирландия","Италия", "Люксембург", "Канада",
            "Македония", "Болгария", "Бразилия", "Германия", "Дания", "Япония",
            "Норвегия", "Швеция", "Нидерланды", "Румыния", "Польша", "Чехия", "Иран",
            "Индия", "Перу", "Чили", "Россия", "Кипр", "Киргизия", "Колумбия",
            "Мексика", "Украина", "СССР", "Узбекистан", "Финляндия"};

        public string Release { get; } = "<i>1910-1980</i> (/1980)" + Environment.NewLine + "<i>1980-1990</i> (/1990)" + Environment.NewLine + "<i>1990-2000</i> (/2000)" +
                    Environment.NewLine + "<i>2000-2005</i> (/2005)" + Environment.NewLine + "<i>2005-2010</i> (/2010)" + Environment.NewLine + "<i>2010-2015</i> (/2015)" +
                    Environment.NewLine + "<i>2015-2020</i> (/2020)" + Environment.NewLine + "<i>2020-2022</i> (/2022)";
    }
}

