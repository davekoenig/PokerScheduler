﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRole.Models
{
    public interface IGame
    {
        DateTime StartTime { get; set; }

        string RoomName { get; set; }

        string GameId { get; }

        IReadOnlyCollection<IPlayer> Players { get; }

        void AddPlayer(IPlayer player);

        void RemovePlayer(IPlayer player);
    }
}