using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
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


        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _config = GetConfig();
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _commandService = new CommandService();

            _commandHandler = new CommandHandler(BuildServiceProvider(), _client, new CommandService(), _config);
            await _commandHandler.InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, _config["DiscordBotToken"]);
            await _client.SetActivityAsync(new Game("!bingo help", ActivityType.Watching));
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_config)
            .AddSingleton(_commandService)
            .AddSingleton<CommandHandler>()
            .BuildServiceProvider();

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private IConfigurationRoot GetConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

    }
}
