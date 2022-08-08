using Bot.BusinessLogic.Helper;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common.Dto;
using Bot.Helper.Handler;
using Bot.Services.Interfaces;
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

        private int _numberFilm { get; set; }
        private List<MovieDto> _movies { get; set; }

        private MessageHendler messageHendler;

        public BotController(IMovieService movieService, IButtonService buttonService)
        {
            this.movieService = movieService;
            this.buttonService = buttonService;
            _movies = new List<MovieDto>();
            messageHendler = new MessageHendler(movieService, buttonService);
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
        //public async Task HandleMessage(ITelegramBotClient botClient, Message message)
        //{
        //    if (message.Text == "Назад")
        //    {
        //        genre = country = release = false;
        //        movieService.ChoiceCategory = false;
        //        ReplyKeyboardMarkup keyboard = buttonService.MenuButton();
        //        await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите параметр", replyMarkup: keyboard);
        //        return;
        //    }

        //    if (country)
        //    {
        //        var movie = movieService.ChoiceMovie(message.Text, "country");
        //        await ShowMovie(movie, message.Chat.Id, botClient, buttonService.Button(movie.Link));
        //        return;
        //    }
        //    if (genre)
        //    {
        //        var movie = movieService.ChoiceMovie(message.Text, "genre");
        //        await ShowMovie(movie, message.Chat.Id, botClient, buttonService.Button(movie.Link));
        //        return;
        //    }
        //    if (release)
        //    {

        //    }

        //    if (message.Text == "/start")
        //    {
        //        ReplyKeyboardMarkup keyboard = buttonService.MenuButton(new KeyboardButton[] { "Начать" });
        //        await botClient.SendTextMessageAsync(message.Chat.Id, "Телеграм бот о фильмах", replyMarkup: keyboard);
        //        return;
        //    }

        //    if (message.Text == "Начать" || message.Text == "В начало")
        //    {
        //        ReplyKeyboardMarkup keyboard = buttonService.MenuButton(new KeyboardButton[] { "Случайный фильм", "Подбор по параметрам" });
        //        await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите раздел", replyMarkup: keyboard);
        //        return;
        //    }
        //    if (message.Text == "Случайный фильм")
        //    {
                
        //        var movie = movieService.ChoiceMovie();
        //        await ShowMovie(movie, message.Chat.Id, botClient, buttonService.Buttons(movie.Link));
        //        return;
        //    }
        //    if (message.Text == "Подбор по параметрам")
        //    {
        //        ReplyKeyboardMarkup keyboard = buttonService.MenuButton();
        //        await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите параметр", replyMarkup: keyboard);
        //        return;
        //    }
        //    if (message.Text == "Страна")
        //    {
        //        ReplyKeyboardMarkup keyboard = buttonService.MenuButton(new KeyboardButton[] { "Назад" });
        //        await botClient.SendTextMessageAsync(message.Chat.Id, "Вернуться", replyMarkup: keyboard);
        //        string country = string.Empty;
        //        for (int i = 0; i < buttonService.CountryList.Count-1; i+=2)
        //            country += "•" + buttonService.CountryList[i] + "     •" + buttonService.CountryList[i+1] + Environment.NewLine;
                
        //        await botClient.SendTextMessageAsync(message.Chat.Id, country);
        //        this.country = true;
        //        return;
        //    }
        //    if (message.Text == "Жанр")
        //    {
        //        ReplyKeyboardMarkup keyboard = buttonService.MenuButton(new KeyboardButton[] { "Назад" });
        //        await botClient.SendTextMessageAsync(message.Chat.Id, "Вернуться", replyMarkup: keyboard);
        //        string genre = string.Empty;
        //        foreach (var item in buttonService.GenreList)
        //            genre += "•" + item + Environment.NewLine;
        //        await botClient.SendTextMessageAsync(message.Chat.Id, genre);
        //        this.genre = true;
        //        return;
        //    }
            

        //    await botClient.SendTextMessageAsync(message.Chat.Id, $"You said:\n{message.Text}");
        //}
        public async Task HandleCallbackQuery(ITelegramBotClient botClient,
            CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data.StartsWith("similar"))
            {
                if (_numberFilm == 0)
                {
                    if (View.Genre == null)
                    {
                        List<string> s = new List<string> { "sdfs", "sdfsd" };

                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"/{string.Join(" ",s)}",parseMode: ParseMode.Html);
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
                await View.ShowMovie(movie, callbackQuery.Message.Chat.Id, botClient, buttonService.Button(movie.Link));
                return;
            }
            await botClient.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"You choose with data: {callbackQuery.Data}");
            return;
        }
        //public async Task GenerateMovie(MovieDto movie, long id, ITelegramBotClient botClient)
        //{
        //    await ShowMovie(movie,id,botClient,buttonService.Buttons(movie.Link));

        //}
        //public async Task Sort(MovieDto movie, long id, ITelegramBotClient botClient)
        //{
        //    await ShowMovie(movie, id, botClient, buttonService.Button(movie.Link));
        //}

        //public async Task ShowMovie(MovieDto movie, long id, ITelegramBotClient botClient, InlineKeyboardMarkup keyboard)
        //{
        //    await botClient.SendPhotoAsync(
        //                chatId: id,
        //                photo: $"{movie.LinkPoster}",
        //                $"{movie.Title.Replace("смотреть онлайн", "")}" + Environment.NewLine +
        //                $"Рейтинг: {movie.Rating}" + Environment.NewLine +
        //                $"Жанр: {movie.Genre}" + Environment.NewLine +
        //                $"Год: {movie.Release}" + Environment.NewLine +
        //                $"Страна: {movie.Country}" + Environment.NewLine +
        //                $"Сюжет: {movie.Sutitle.Replace("\n", " ").Substring(0, Math.Min(600, movie.Sutitle.Length))}...",
        //                replyMarkup: keyboard);
        //    _genre = movie.Genre.Split(' ').First();
        //    return;
        //}
    }
}

