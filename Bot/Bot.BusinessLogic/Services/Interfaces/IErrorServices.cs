using System;
using Telegram.Bot;

namespace Bot.BusinessLogic.Services.Interfaces
{
	public interface IErrorServices
	{
		Task HandleError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken);
	}
}

