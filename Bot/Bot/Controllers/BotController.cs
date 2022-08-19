using Bot.BusinessLogic.Helper;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common.Dto;
using Bot.Helper.Handler;
using Bot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Controllers
{
    public class BotController
    {
        private readonly IMovieService movieService;
        private readonly IButtonService buttonService;
        private readonly IContentService contentService;

        private int _numberFilm { get; set; }
        private List<MovieDto> _movies { get; set; }

        private MessageHendler messageHendler;

        public BotController(IMovieService movieService, IButtonService buttonService , IContentService contentService)
        {
            this.movieService = movieService;
            this.buttonService = buttonService;
            _movies = new List<MovieDto>();
            messageHendler = new MessageHendler(movieService, buttonService, contentService);
        }
        public async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await messageHendler.HandleMessage(botClient, update.Message);
                return;
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery(botClient, update.CallbackQuery);
                return;
            }
        }
        
        public async Task HandleCallbackQuery(ITelegramBotClient botClient,
            CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data.StartsWith("similar"))
            {
                if (_numberFilm == 0)
                {
                    if (View.Genre == null)
                    {
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Для начала, необходимо выбрать фильм");
                        return;
                    }
                    _movies = movieService.GetSimilar(View.Genre).ToList();
                    await View.ShowMovie(_movies[_numberFilm], callbackQuery.Message.Chat.Id, botClient, buttonService.Buttons(_movies[_numberFilm].Link));
                }
                if (_numberFilm >= _movies.Count)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Похожих фильмов больше нет");
                    _numberFilm = 0;
                    return;
                }
                else if (_numberFilm > 0)
                    await View.ShowMovie(_movies[_numberFilm], callbackQuery.Message.Chat.Id, botClient, buttonService.Buttons(_movies[_numberFilm].Link));

                _numberFilm++;
                return;
            }
            if (callbackQuery.Data.StartsWith("next"))
            {
                var movie = movieService.ChoiceMovie();
                await View.ShowMovie(movie, callbackQuery.Message.Chat.Id, botClient, buttonService.Buttons(movie.Link));
                _numberFilm = 0;
                return;
            }
            if (callbackQuery.Data.StartsWith("Далее"))
            {
                var movie = movieService.ChoiceMovie("","");
                try
                {
                    await View.ShowMovie(movie, callbackQuery.Message.Chat.Id, botClient, buttonService.Button(movie.Link));
                }
                catch (Exception) { await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Фильмов с таким критерием больше нет"); }
                return;
            }
            await botClient.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"You choose with data: {callbackQuery.Data}");
            return;
        }
    }
}

