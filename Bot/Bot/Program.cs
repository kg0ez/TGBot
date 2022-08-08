using AutoMapper;
using Bot.BusinessLogic.Helper.Mapper;
using Bot.BusinessLogic.Services.Implementations;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Controllers;
using Bot.Services.Interfaces;
using Bot.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;


var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IButtonService, ButtonService>()
            .AddSingleton<IErrorService,ErrorService>()
            .AddSingleton<IMovieService, MovieService>()
            .BuildServiceProvider();
var mapperConfiguration = new MapperConfiguration(x =>
{
    x.AddProfile<MappingProfile>();
});
mapperConfiguration.AssertConfigurationIsValid();
IMapper mapper = mapperConfiguration.CreateMapper();

var movieService = serviceProvider.GetService<IMovieService>();
var errorServices = serviceProvider.GetService<IErrorService>();
var buttonServices = serviceProvider.GetService<IButtonService>();

movieService.Mapper = mapper;
var botController = new BotController(movieService, buttonServices);

var botClient = new TelegramBotClient("5346358438:AAHfncUZIXOuvKBz8YsDvypbzoCKuDR6s7k");

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }
};

botClient.StartReceiving(
    botController.HandleUpdatesAsync,
    errorServices.HandleError,
    receiverOptions,
    cancellationToken: cts.Token);

var me = await botClient.GetMeAsync();
Console.WriteLine(me.Username + " is working");
Console.ReadLine();

cts.Cancel();
