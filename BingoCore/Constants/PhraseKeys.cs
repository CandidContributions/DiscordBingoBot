using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBingoBot.Constants
{
    public static class PhraseKeys
    {
        public static class Start
        {
            public const string AlreadyActive = "Start.AlreadyActive";
        }

        public static class Register
        {
            public const string InvalidName = "Register.InvalidName";
            public const string NoActiveGame = "Register.NoActiveGame";
            public const string ActiveRound = "Register.ActiveRound";
            public const string AlreadyRegistered = "Register.AlreadyRegistered";
        }

        public static class DeRegister
        {
            public const string InvalidName = "DeRegister.InvalidName";
            public const string NotRegistered = "DeRegister.NotRegistered";
        }

        public static class StartRound
        {
            public const string NoActiveGame = "StartRound.NoActiveGame";
            public const string ActiveRound = "StartRound.ActiveRound";
            public const string NotEnoughPlayers = "StartRound.NotEnoughPlayers";
        }

        public static class Next
        {
            public const string NoActiveGame = "Next.NoActiveGame";
            public const string NoActiveRound = "Next.NoActiveRound";
            public const string NoRemainingItems = "Next.NoRemainingItems";
            public const string Success = "Next.Success";
        }

        public static class Bingo
        {
            public const string NotRegistered = "Bingo.NotRegistered";
            public const string Mistake = "Bingo.Mistake";
        }

        public static class Stop
        {
            public const string NoActive = "Bingo.NoActive";
        }
    }
}
