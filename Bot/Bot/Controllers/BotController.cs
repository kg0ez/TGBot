﻿using System;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Models.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Controllers
{
	public class BotController
	{
        IMovieService movieService;
        IErrorService errorService;
        IButtonService buttonService;
        public BotController(IMovieService movieService, IErrorService errorService, IButtonService buttonService)
		{
            this.movieService = movieService;
            this.errorService = errorService;
            this.buttonService = buttonService;
		}
        string genre { get; set; }
        int similarFilm { get; set; }
        List<Movie> movies { get; set; }
        async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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

        async Task HandleMessage(ITelegramBotClient botClient, Message message)
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

        async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data.StartsWith("similar"))
            {
                if (similarFilm == 0)
                {
                    movies = movieService.GetSimilar(genre).ToList();
                    await GenerateMovie(movies[similarFilm], callbackQuery.Message.Chat.Id, botClient);
                }
                if (similarFilm >= movies.Count)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Похожих фильмов больше нет");
                    similarFilm = 0;
                    return;
                }
                else if (similarFilm > 0)
                    await GenerateMovie(movies[similarFilm], callbackQuery.Message.Chat.Id, botClient);

                similarFilm++;
                return;
            }
            if (callbackQuery.Data.StartsWith("next"))
            {
                var movie = movieService.ChoiceMovie();
                await GenerateMovie(movie, callbackQuery.Message.Chat.Id, botClient);
                similarFilm = 0;
                return;
            }
            await botClient.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"You choose with data: {callbackQuery.Data}");
            return;
        }

        async Task GenerateMovie(Movie movie, long id, ITelegramBotClient botClient)
        {
            genre = movie.Genre.Split(' ').First();

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
                        //cancellationToken: cts.Token);
            return;
        }
    }
}

