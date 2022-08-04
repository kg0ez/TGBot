using Bot.Models.Data;
using Bot.Models.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var botClient = new TelegramBotClient("5414080253:AAFOrTfZx3QULazPNGakMhV7gWrmiPhCukg");

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }
};

botClient.StartReceiving(
    HandleUpdatesAsync,
    HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token);

var me = await botClient.GetMeAsync();

Console.WriteLine(me.Username);
Console.ReadLine();

cts.Cancel();

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
        await botClient.SendTextMessageAsync(message.Chat.Id, "Choose commands: /inline | /keyboard");
        return;
    }

    if (message.Text == "/keyboard")
    {
        ReplyKeyboardMarkup keyboard = new(
            new KeyboardButton[] {"Start"}
        )
        //ReplyKeyboardMarkup keyboard = new(new[]
        //{
        //    new KeyboardButton[] {"Start"},
        //})
        {
            ResizeKeyboard = true
        };
        await botClient.SendTextMessageAsync(message.Chat.Id, "Choose:", replyMarkup: keyboard);
        return;
    }

    if (message.Text == "/inline")
    {
        InlineKeyboardMarkup keyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Similar Movie", "similar_movie"),
                InlineKeyboardButton.WithCallbackData("Next Movie", "next_movie"),
            }
        });
        await botClient.SendTextMessageAsync(message.Chat.Id, "Choose inline:", replyMarkup: keyboard);
        return;
    }

    await botClient.SendTextMessageAsync(message.Chat.Id, $"You said:\n{message.Text}");
}

async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    if (callbackQuery.Data.StartsWith("similar"))
    {
        await botClient.SendPhotoAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    photo: "https://avatars.mds.yandex.net/get-kinopoisk-image/1599028/4b27e219-a8a5-4d85-9874-57d6016e0837/600x900",
                    $"Вы хотите купить?");
        return;
    }
    if (callbackQuery.Data.StartsWith("next"))
    {
        var movie = new Movie();
        Random random = new Random();
        int value = random.Next(0, 19);
        using(ApplicationContext context = new ApplicationContext())
        {
            movie = context.Movies.AsNoTracking().FirstOrDefault(m => m.Id == value);
        }
        InlineKeyboardMarkup keyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl(
                text: "Ссылка на фильм",
                url: $"{movie.Link}"
                )
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Похожий фильм", "similar"),
                InlineKeyboardButton.WithCallbackData("Следующий фильм", "next"),
            }

        });
        
        await botClient.SendPhotoAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    photo: $"{movie.LinkPoster}",
                    $"{movie.Title}"+Environment.NewLine+
                    $"Жанр: {movie.Genre}"+ Environment.NewLine+
                    $"Год: {movie.Release}" + Environment.NewLine +
                    $"Страна: {movie.Country}" + Environment.NewLine +
                    $"Сюжет: {movie.Sutitle}",
                    replyMarkup: keyboard,
                    cancellationToken: cts.Token);
        return;

    }
    await botClient.SendTextMessageAsync(
        callbackQuery.Message.Chat.Id,
        $"You choose with data: {callbackQuery.Data}");
    return;
}

async Task GenerateMovie(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    var movie = new Movie();
    Random random = new Random();
    int value = random.Next(0, 19);
    using (ApplicationContext context = new ApplicationContext())
    {
        movie = context.Movies.AsNoTracking().FirstOrDefault(m => m.Id == value);
    }
    InlineKeyboardMarkup keyboard = new(new[]
    {
            new[]
            {
                InlineKeyboardButton.WithUrl(
                text: "Ссылка на фильм",
                url: $"{movie.Link}"
                )
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Похожий фильм", "similar"),
                InlineKeyboardButton.WithCallbackData("Следующий фильм", "next"),
            }

        });

    await botClient.SendPhotoAsync(
                chatId: callbackQuery.Message.Chat.Id,
                photo: $"{movie.LinkPoster}",
                $"{movie.Title}" + Environment.NewLine +
                $"Жанр: {movie.Genre}" + Environment.NewLine +
                $"Год: {movie.Release}" + Environment.NewLine +
                $"Страна: {movie.Country}" + Environment.NewLine +
                $"Сюжет: {movie.Sutitle}",
                replyMarkup: keyboard,
                cancellationToken: cts.Token);
    return;
}

Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Ошибка телеграм АПИ:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

//InlineKeyboardMarkup inlineKeyboard = new(new[]
//{

//    InlineKeyboardButton.WithUrl(
//    text: "Link to the Repository",
//    url: "https://github.com/TelegramBots/Telegram.Bot"
//    )
//});
//await botClient.SendTextMessageAsync(
//    chatId: callbackQuery.Message.Chat.Id,
//    text: "A message with an inline keyboard markup",
//    replyMarkup: keyboard,
//    cancellationToken: cts.Token);
                    //photo: "https://avatars.mds.yandex.net/get-kinopoisk-image/1599028/4b27e219-a8a5-4d85-9874-57d6016e0837/600x900",
