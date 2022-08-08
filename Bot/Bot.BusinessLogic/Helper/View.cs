using System;
using Bot.Common.Dto;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.BusinessLogic.Helper
{
	public class View
	{
        public static string Genre { get; set; }

        public static async Task ShowMovie(MovieDto movie, long id, ITelegramBotClient botClient, InlineKeyboardMarkup keyboard)
        {
            await botClient.SendPhotoAsync(
                        chatId: id,
                        photo: $"{movie.LinkPoster}",
                        $"{movie.Title.Replace("смотреть онлайн", "")}" + Environment.NewLine +
                        $"Рейтинг: {movie.Rating}" + Environment.NewLine +
                        $"Жанр: {movie.Genre}" + Environment.NewLine +
                        $"Год: {movie.Release}" + Environment.NewLine +
                        $"Страна: {movie.Country}" + Environment.NewLine +
                        $"Сюжет: {movie.Sutitle.Replace("\n", " ").Substring(0, Math.Min(600, movie.Sutitle.Length))}...",
                        replyMarkup: keyboard);
            Genre = movie.Genre.Split(' ').First();
            return;
        }
    }
}

