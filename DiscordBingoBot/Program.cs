using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBingoBot.Commands;
using DiscordBingoBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBingoBot
{
    public class Program
    {
        private DiscordSocketClient _client;
        private IConfigurationRoot _config;
        private CommandHandler _commandHandler;
        private CommandService _commandService;
        private ILogger _logger;


        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _config = GetConfig();
            _client = new DiscordSocketClient();
            _logger = new Logger();
            _client.Log += _logger.Log;
            _commandService = new CommandService();

            _commandHandler = new CommandHandler(BuildServiceProvider(_logger), _client, _commandService, _config);
            await _commandHandler.InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, _config["DiscordBotToken"]);
            var helpCommand = _commandService.Commands.FirstOrDefault(c => c.Name == "help");
            if (helpCommand != null)
            {
                await _client.SetActivityAsync(new Game(_config["DiscordBotPrefix"] + helpCommand.Name, ActivityType.Watching));
            }
            
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public IServiceProvider BuildServiceProvider(ILogger logger) => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_config)
            .AddSingleton(_commandService)
            .AddSingleton<CommandHandler>()
            .AddSingleton<IBingoService,BingoService>()
            .AddSingleton<ICsvReader,CsvReader>()
            .AddSingleton(logger)
            .BuildServiceProvider();

        private IConfigurationRoot GetConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appSettings.json.secret", optional: false, reloadOnChange: true);
            return builder.Build();
        }

    }
}
