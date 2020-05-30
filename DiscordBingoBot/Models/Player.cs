﻿using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace DiscordBingoBot.Models
{
    public class Player
    {
        public string Name { get; set; }
        public string NickName { get; set; }
        public Grid Grid { get; set; }

        public Player(string name, string nickName)
        {
            NickName = nickName;
            Name = name;
        }
    }
}
