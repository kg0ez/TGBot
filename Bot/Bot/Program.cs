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
            .AddSingleton<IContentService,ContentService>()
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
var contentServices = serviceProvider.GetService<IContentService>();

movieService.Mapper = mapper;
movieService.ContentService = contentServices;
var botController = new BotController(movieService, buttonServices,contentServices);

var botClient = new TelegramBotClient("5575229159:AAFpnpbKTZGyiBQ8k7XQ_fu1-GYbvdhmh7E");

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
