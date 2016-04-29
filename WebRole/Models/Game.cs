using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WebRole.Models
{
    internal class Game : TableEntity
    {
        internal DateTime StartTime { get; set; }

        internal DateTime EndTime { get; set; }

        internal string RoomName { get; set; }

        internal string GameId { get; private set; }

        private HashSet<Player> _players = new HashSet<Player>();

        internal ReadOnlyCollection<Player> Players
        {
            get
            {
                return _players.ToList().AsReadOnly();
            }
        }

        internal Game()
        {
            GameId = Guid.NewGuid().ToString();

            RowKey = GameId;

            PartitionKey = StartTime.ToLongDateString();
        }

        internal void AddPlayer(Player player)
        {
            _players.Add(player);
        }

        internal void RemovePlayer(Player player)
        {
            _players.Remove(player);
        }
    }
}