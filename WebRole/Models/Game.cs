using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WebRole.Models
{
    public class Game : TableEntity, IDataModel, IGame
    {
        public DateTime StartTime { get; set; }
        
        public string RoomName { get; set; }

        public string GameId { get; private set; }

        private HashSet<IPlayer> _players = new HashSet<IPlayer>();

        public IReadOnlyCollection<IPlayer> Players
        {
            get
            {
                return _players.ToList().AsReadOnly();
            }
        }

        private Game(DateTime startTime, string room)
        {
            const string format = "yyyyMMddHHmm";

            GameId = StartTime.ToString(format);

            RowKey = GameId;

            PartitionKey = StartTime.ToString(format);
        }

        public static Game Create(DateTime startTime, string room)
        {
            var game = new Game(startTime, room);

            return game;
        }


        public void AddPlayer(IPlayer player)
        {
            _players.Add(player);
        }

        public void RemovePlayer(IPlayer player)
        {
            _players.Remove(player);
        }
    }
}