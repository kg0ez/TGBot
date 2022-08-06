using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common.Dto;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Controllers
{
    public class BotController
    {
        private readonly IMovieService movieService;
        private readonly IButtonService buttonService;

        private string _genre { get; set; }
        private int _numberFilm { get; set; }
        private List<MovieDto> _movies { get; set; }

        public BotController(IMovieService movieService, IButtonService buttonService)
        {
            this.movieService = movieService;
            this.buttonService = buttonService;
            _movies = new List<MovieDto>();
        }
        public async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandleMessage(botClient, update.Message);
                return;
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery(botClient, update.CallbackQuery);
                return;
            }
        }
        public async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "/start")
            {
                ReplyKeyboardMarkup keyboard = buttonService.MenuButton();
                await botClient.SendTextMessageAsync(message.Chat.Id, "Телеграм бот о фильмах", replyMarkup: keyboard);
                return;
            }

            if (message.Text == "Начать")
            {
                var movie = movieService.ChoiceMovie();
                await GenerateMovie(movie, message.Chat.Id, botClient);
                return;
            }
            await botClient.SendTextMessageAsync(message.Chat.Id, $"You said:\n{message.Text}");
        }
        public async Task HandleCallbackQuery(ITelegramBotClient botClient,
            CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data.StartsWith("similar"))
            {
                if (_numberFilm == 0)
                {
                    _movies = movieService.GetSimilar(_genre).ToList();
                    await GenerateMovie(_movies[_numberFilm], callbackQuery.Message.Chat.Id, botClient);
                }
                if (_numberFilm >= _movies.Count)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Похожих фильмов больше нет");
                    _numberFilm = 0;
                    return;
                }
                else if (_numberFilm > 0)
                    await GenerateMovie(_movies[_numberFilm], callbackQuery.Message.Chat.Id, botClient);

                _numberFilm++;
                return;
            }
            if (callbackQuery.Data.StartsWith("next"))
            {
                var movie = movieService.ChoiceMovie();
                await GenerateMovie(movie, callbackQuery.Message.Chat.Id, botClient);
                _numberFilm = 0;
                return;
            }
            await botClient.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"You choose with data: {callbackQuery.Data}");
            return;
        }
        public async Task GenerateMovie(MovieDto movie, long id, ITelegramBotClient botClient)
        {
            InlineKeyboardMarkup keyboard = buttonService.Buttons(movie.Link);
            await botClient.SendPhotoAsync(
                        chatId: id,
                        photo: $"{movie.LinkPoster}",
                        $"{movie.Title.Replace("смотреть онлайн", "")}" + Environment.NewLine +
                        $"Жанр: {movie.Genre}" + Environment.NewLine +
                        $"Год: {movie.Release}" + Environment.NewLine +
                        $"Страна: {movie.Country}" + Environment.NewLine +
                        $"Сюжет: {movie.Sutitle.Substring(0, Math.Min(500, movie.Sutitle.Length))}",
                        replyMarkup: keyboard);
            _genre = movie.Genre.Split(' ').First();
            return;
        }
    }
}

