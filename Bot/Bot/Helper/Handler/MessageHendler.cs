using Bot.BusinessLogic.Helper;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common.Dto;
using Bot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
namespace Bot.Helper.Handler
{
	public class MessageHendler
	{
        private readonly IMovieService movieService;
        private readonly IButtonService buttonService;

        private bool genre { get; set; }
        private bool release { get; set; }
        private bool country { get; set; }

        public MessageHendler(IMovieService movieService, IButtonService buttonService)
        {
            this.movieService = movieService;
            this.buttonService = buttonService;
        }

        public async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "Назад")
            {
                genre = country = release = false;
                movieService.ChoiceCategory = false;
                ReplyKeyboardMarkup keyboard = buttonService.MenuButton();
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите параметр", replyMarkup: keyboard);
                return;
            }

            if (country)
            {
                var movie = movieService.ChoiceMovie(message.Text, "country");
                if (movie == null)
                    return;
                await View.ShowMovie(movie, message.Chat.Id, botClient, buttonService.Button(movie.Link));
                return;
            }
            if (genre)
            {
                var movie = movieService.ChoiceMovie(message.Text, "genre");
                await View.ShowMovie(movie, message.Chat.Id, botClient, buttonService.Button(movie.Link));
                return;
            }
            if (release)
            {

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
                await botClient.SendTextMessageAsync(message.Chat.Id, "Вернуться", replyMarkup: keyboard);
                string country = string.Empty;
                for (int i = 0; i < buttonService.CountryList.Count - 1; i += 2)
                    country += "• " + $"/{ buttonService.CountryList[i]}" +" "+"/ewrwe"+"    •" + buttonService.CountryList[i + 1] + Environment.NewLine;

                await botClient.SendTextMessageAsync(message.Chat.Id, country);
                this.country = true;
                return;
            }
            if (message.Text == "Жанр")
            {
                ReplyKeyboardMarkup keyboard = buttonService.MenuButton(new KeyboardButton[] { "Назад" });
                await botClient.SendTextMessageAsync(message.Chat.Id, "Вернуться", replyMarkup: keyboard);
                string genre = string.Empty;
                foreach (var item in buttonService.GenreList)
                    genre += "•"+" " +"/"+ item + Environment.NewLine;
                await botClient.SendTextMessageAsync(message.Chat.Id, genre);
                this.genre = true;
                return;
            }
            if (message.Text == "Год")
            {
                ReplyKeyboardMarkup keyboard = buttonService.MenuButton(new KeyboardButton[] { "Назад" });
                await botClient.SendTextMessageAsync(message.Chat.Id, "Вернуться", replyMarkup: keyboard);

                string genre = string.Empty;
                foreach (var item in buttonService.GenreList)
                    genre += "•" + item + Environment.NewLine;

                await botClient.SendTextMessageAsync(message.Chat.Id, genre);
                this.genre = true;
                return;
            }

            await botClient.SendTextMessageAsync(message.Chat.Id, $"You said:\n{message.Text}");
        }
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

