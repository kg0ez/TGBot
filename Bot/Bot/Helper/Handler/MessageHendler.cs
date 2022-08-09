using Bot.BusinessLogic.Helper;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
namespace Bot.Helper.Handler
{
	public class MessageHendler
	{
        private readonly IMovieService movieService;
        private readonly IButtonService buttonService;
        private readonly IContentService contentService;

        private bool _genre { get; set; }
        private bool _release { get; set; }
        private bool _country { get; set; }

        public MessageHendler(IMovieService movieService, IButtonService buttonService,IContentService contentService)
        {
            this.movieService = movieService;
            this.buttonService = buttonService;
            this.contentService = contentService;
        }

        public async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "Назад")
            {
                _genre = _country = _release = false;
                movieService.ChoiceCategory = false;
                ReplyKeyboardMarkup keyboard = buttonService.MenuButton();
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите параметр", replyMarkup: keyboard);
                return;
            }

            if (_country)
            {
                var movie = movieService.ChoiceMovie(message.Text, "country");
                if (movie == null)
                    return;
                await View.ShowMovie(movie, message.Chat.Id, botClient, buttonService.Button(movie.Link));
                return;
            }
            if (_genre)
            {
                var movie = movieService.ChoiceMovie(message.Text, "genre");
                if (movie == null)
                    return;
                await View.ShowMovie(movie, message.Chat.Id, botClient, buttonService.Button(movie.Link));
                return;
            }
            if (_release)
            {
                var movie = movieService.ChoiceMovie(message.Text, "release");
                if (movie == null)
                    return;
                await View.ShowMovie(movie, message.Chat.Id, botClient, buttonService.Button(movie.Link));
                return;
                int startValue, finishvalue;
                if (message.Text == "/1980")
                {
                    startValue = 1910; 
                }
            }

            if (message.Text == "/start")
            {
                ReplyKeyboardMarkup keyboard = buttonService.MenuButton(new KeyboardButton[] { "Начать" });
                await botClient.SendTextMessageAsync(message.Chat.Id, "Телеграм бот о фильмах", replyMarkup: keyboard);
                return;
            }

            if (message.Text == "Начать" || message.Text == "В начало")
            {
                ReplyKeyboardMarkup keyboard = buttonService.MenuButton(new KeyboardButton[] { "Случайный фильм", "Подбор по параметрам" });
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите раздел", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Случайный фильм")
            {

                var movie = movieService.ChoiceMovie();
                await View.ShowMovie(movie, message.Chat.Id, botClient, buttonService.Buttons(movie.Link));
                return;
            }
            if (message.Text == "Подбор по параметрам")
            {
                ReplyKeyboardMarkup keyboard = buttonService.MenuButton();
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите параметр", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Страна")
            {
                ReplyKeyboardMarkup keyboard = buttonService.MenuButton(new KeyboardButton[] { "Назад" });
                string country = string.Empty;

                for (int i = 0; i < contentService.CountryList.Count; i++)
                    country += "• " + $"<i>{contentService.CountryList[i]}</i>" + $" (/ct{i})"+ Environment.NewLine;

                await botClient.SendTextMessageAsync(message.Chat.Id, "<b>Выберите страну используя тег:</b>" + Environment.NewLine + country,parseMode:Telegram.Bot.Types.Enums.ParseMode.Html,replyMarkup: keyboard);
                this._country = true;
                return;
            }
            if (message.Text == "Жанр")
            {
                ReplyKeyboardMarkup keyboard = buttonService.MenuButton(new KeyboardButton[] { "Назад" });
                string genre = string.Empty;
                
                for (int i = 0; i < contentService.GenreList.Count; i++)
                    genre += "• " + $"<i>{contentService.GenreList[i]}</i>" + $" (/gn{i})" +Environment.NewLine;

                await botClient.SendTextMessageAsync(message.Chat.Id, "<b>Выберите жанр используя тег:</b>"+Environment.NewLine+ genre,parseMode:Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: keyboard);
                this._genre = true;
                return;
            }
            if (message.Text == "Год")
            {
                ReplyKeyboardMarkup keyboard = buttonService.MenuButton(new KeyboardButton[] { "Назад" });
                string release = string.Empty;
                foreach (var item in contentService.ReleaseList)
                    release += "• " + item+Environment.NewLine;

                await botClient.SendTextMessageAsync(message.Chat.Id, "<b>Выберите диапазон используя тег:</b>"+Environment.NewLine+release, replyMarkup: keyboard,parseMode:Telegram.Bot.Types.Enums.ParseMode.Html);
                this._release = true;
                return;
            }

            await botClient.SendTextMessageAsync(message.Chat.Id, $"You said:\n{message.Text}");
        }
    }
}

