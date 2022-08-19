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
        private readonly IMovieService _movieService;
        private readonly IButtonService _buttonService;
        private readonly IContentService _contentService;

        private bool _genre { get; set; }
        private bool _release { get; set; }
        private bool _country { get; set; }

        public MessageHendler(IMovieService movieService, IButtonService buttonService,IContentService contentService)
        {
            _movieService = movieService;
            _buttonService = buttonService;
            _contentService = contentService;
        }

        public async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "Назад")
            {
                _genre = _country = _release = false;
                _movieService.ChoiceCategory = false;
                ReplyKeyboardMarkup keyboard = _buttonService.MenuButton();
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите параметр", replyMarkup: keyboard);
                return;
            }

            if (_country || _genre || _release)
            {
                string action = string.Empty;
                if (_country)
                    action = "country";
                else if (_genre)
                    action = "genre";
                else if (_release)
                    action = "release";
                _genre = _country = _release = false;
                var movie = _movieService.ChoiceMovie(message.Text, action);
                if (movie == null)
                    return;
                await View.ShowMovie(movie, message.Chat.Id, botClient, _buttonService.Button(movie.Link));
                return;
            }

            if (message.Text == "/start")
            {
                ReplyKeyboardMarkup keyboard = _buttonService.MenuButton(new KeyboardButton[] { "Начать" });
                await botClient.SendTextMessageAsync(message.Chat.Id, "Телеграм бот о фильмах", replyMarkup: keyboard);
                return;
            }

            if (message.Text == "Начать" || message.Text == "В начало")
            {
                ReplyKeyboardMarkup keyboard = _buttonService.MenuButton(new KeyboardButton[] { "Случайный фильм", "Подбор по параметрам" });
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите раздел", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Случайный фильм")
            {
                var movie = _movieService.ChoiceMovie();
                await View.ShowMovie(movie, message.Chat.Id, botClient, _buttonService.Buttons(movie.Link));
                return;
            }
            if (message.Text == "Подбор по параметрам")
            {
                ReplyKeyboardMarkup keyboard = _buttonService.MenuButton();
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите параметр", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Страна")
            {
                ReplyKeyboardMarkup keyboard = _buttonService.MenuButton(new KeyboardButton[] { "Назад" });
                string country = string.Empty;

                for (int i = 0; i < _contentService.CountryList.Count; i++)
                    country += "• " + $"<i>{_contentService.CountryList[i]}</i>" + $" (/ct{i})"+ Environment.NewLine;

                await botClient.SendTextMessageAsync(message.Chat.Id, "<b>Выберите страну используя тег:</b>" + Environment.NewLine + country,parseMode:Telegram.Bot.Types.Enums.ParseMode.Html,replyMarkup: keyboard);
                _country = true;
                return;
            }
            if (message.Text == "Жанр")
            {
                ReplyKeyboardMarkup keyboard = _buttonService.MenuButton(new KeyboardButton[] { "Назад" });
                string genre = string.Empty;
                
                for (int i = 0; i < _contentService.GenreList.Count; i++)
                    genre += "• " + $"<i>{_contentService.GenreList[i]}</i>" + $" (/gn{i})" +Environment.NewLine;

                await botClient.SendTextMessageAsync(message.Chat.Id, "<b>Выберите жанр используя тег:</b>"+Environment.NewLine+ genre,parseMode:Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: keyboard);
                _genre = true;
                return;
            }
            if (message.Text == "Год")
            {
                ReplyKeyboardMarkup keyboard = _buttonService.MenuButton(new KeyboardButton[] { "Назад" });
                string release = string.Empty;
                foreach (var item in _contentService.ReleaseList)
                    release += "• " + item+Environment.NewLine;

                await botClient.SendTextMessageAsync(message.Chat.Id, "<b>Выберите диапазон используя тег:</b>"+Environment.NewLine+release, replyMarkup: keyboard,parseMode:Telegram.Bot.Types.Enums.ParseMode.Html);
                _release = true;
                return;
            }
            await botClient.SendTextMessageAsync(message.Chat.Id, $"Команда: "+message.Text+"не найдена");
        }
    }
}

