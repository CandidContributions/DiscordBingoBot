using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BingoCore.Constants;
using BingoCore.Models.BingoConfiguration;
using BingoCore.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBingoBot.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace DiscordBingoBot.Services
{
    public class AutoNextService : IAutoNextService
    {
        private readonly IBingoService _bingoService;
        private readonly IConfigurationService _configurationService;
        private readonly ILogger _logger;
        private Random _random = new Random();
        private readonly Dictionary<string, bool> _gameIsPaused = new Dictionary<string, bool>();


        public AutoNextService(IBingoService bingoService, IConfigurationService configurationService,
            ILogger logger)
        {
            _bingoService = bingoService;
            _configurationService = configurationService;
            _logger = logger;
        }

        public async Task<bool> Start(object context)
        {
            var discordContext = context as SocketCommandContext;
            if (discordContext == null)
            {
                return false;
            }

            var gameIdentifier = discordContext.GetChannelGuildIdentifier();
            if (_gameIsPaused.ContainsKey(gameIdentifier) == false)
            {
                _gameIsPaused.Add(gameIdentifier,false);
            }
            await ScheduleNext(discordContext).ConfigureAwait(false);
            return true;
        }

        public bool IsPaused(object context)
        {
            var discordContext = context as SocketCommandContext;
            if (discordContext == null)
            {
                return false;
            }

            var gameIdentifier = discordContext.GetChannelGuildIdentifier();
            if (_gameIsPaused.ContainsKey(gameIdentifier) == false)
            {
                return false;
            }

            return _gameIsPaused[gameIdentifier];
        }

        public bool Pause(object context)
        {
            var discordContext = context as SocketCommandContext;
            if (discordContext == null)
            {
                return false;
            }
            var gameIdentifier = discordContext.GetChannelGuildIdentifier();
            if (_gameIsPaused.ContainsKey(gameIdentifier) == false)
            {
                return false;
            }

            _gameIsPaused[gameIdentifier] = true;
            return true;
        }

        public bool Stop(object context)
        {
            var discordContext = context as SocketCommandContext;
            if (discordContext == null)
            {
                return false;
            }
            var gameIdentifier = discordContext.GetChannelGuildIdentifier();
            if (_gameIsPaused.ContainsKey(gameIdentifier) == false)
            {
                return false;
            }

            _gameIsPaused.Remove(gameIdentifier);
            return true;
        }

        private async Task ExecuteNext(SocketCommandContext context)
        {
            var gameIdentifier = context.GetChannelGuildIdentifier();
            // check if we are not paused
            if (_gameIsPaused.ContainsKey(gameIdentifier) == false || _gameIsPaused[gameIdentifier])
            {
                return;
            }

            var bingoGame = _bingoService.GetGame(context.GetChannelGuildIdentifier());

            var next = await bingoGame.NextItem().ConfigureAwait(false);
            if (next.Result)
            {
                var reply = string.Format(await _configurationService.GetPhrase(PhraseKeys.Next.Success).ConfigureAwait(false), next.Info);
                    await context.Message.Channel.SendMessageAsync(reply);
                // queue the next
                await ScheduleNext(context).ConfigureAwait(false);
            }
            else
            {
                await context.User.SendMessageAsync("Can't call a new item: " + next.Info);
            }
        }

        private async Task ScheduleNext(SocketCommandContext context)
        {
            var config = await _configurationService.GetConfiguration().ConfigureAwait(true);

#pragma warning disable 4014
            // No need to await, as it will run on another thread anyway and we want the scheduling to return asap
            Task.Delay(CalculateDelay(config.AutoRoundSettings) * 1000).ContinueWith(t => ExecuteNext(context));
#pragma warning restore 4014
        }

        // todo put into a skewedRandomizer so we can unit test this
        private int CalculateDelay(AutoRoundSettings settings)
        {
            // get a random number between min and max
            var randomDelay = _random.Next(settings.MinimumTimeout, settings.MaximumTimeout + 1);

            // get the difference between random and preferred;
            var difference = (randomDelay - settings.PreferredTimeout) * -1;

            // add the skewFactor
            var delay = randomDelay + (difference * settings.PreferredTimeoutSkewPercentage / 100);
            return delay;
        }
    }
}
