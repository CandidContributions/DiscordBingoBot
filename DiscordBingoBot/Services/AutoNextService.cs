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

namespace DiscordBingoBot.Services
{
    public class AutoNextService : IAutoNextService
    {
        private readonly IBingoService _bingoService;
        private readonly IConfigurationService _configurationService;
        private readonly ILogger _logger;
        private SocketCommandContext _context;
        private Random _random = new Random();
        public bool Paused { get; private set; }

        public AutoNextService(IBingoService bingoService, IConfigurationService configurationService,
            ILogger logger)
        {
            _bingoService = bingoService;
            _configurationService = configurationService;
            _logger = logger;
        }

        public async Task Start(object context)
        {
            var discordContext = context as SocketCommandContext;
            if (discordContext == null)
            {
                return;
            }
            _context = discordContext;
            Paused = false;
            await ScheduleNext().ConfigureAwait(false);
        }

        public void Pause()
        {
            Paused = true;
        }

        private async Task ExecuteNext()
        {
            // check if we are not paused
            if (Paused)
            {
                return;
            }

            var next = await _bingoService.NextItem().ConfigureAwait(false);
            if (next.Result)
            {
                var reply = string.Format(await _configurationService.GetPhrase(PhraseKeys.Next.Success).ConfigureAwait(false), next.Info);
                    await _context.Message.Channel.SendMessageAsync(reply);
                // queue the next
                await ScheduleNext().ConfigureAwait(false);
            }
            else
            {
                await _context.User.SendMessageAsync("Can't call a new item: " + next.Info);
            }
        }

        private async Task ScheduleNext()
        {
            var config = await _configurationService.GetConfiguration().ConfigureAwait(true);

#pragma warning disable 4014
            // No need to await, as it will run on another thread anyway and we want the scheduling to return asap
            Task.Delay(CalculateDelay(config.AutoRoundSettings) * 1000).ContinueWith(t => ExecuteNext());
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
