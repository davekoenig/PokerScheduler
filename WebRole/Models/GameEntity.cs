using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WebRole.Models
{
    public class GameEntity : TableEntity, IGame
    {
        public DateTime StartTime { get; set; }
        
        public string RoomName { get; set; }

        public string Id { get; private set; }

        private HashSet<IPlayer> _players = new HashSet<IPlayer>();

        public IReadOnlyCollection<IPlayer> Players
        {
            get
            {
                return _players.ToList().AsReadOnly();
            }
        }

        private GameEntity(DateTime startTime, string room)
        {
            const string format = "yyyyMMddHHmm";

            Id = StartTime.ToString(format);

            RowKey = Id;

            PartitionKey = StartTime.ToString(format);
        }

        public static GameEntity Create(DateTime startTime, string room)
        {
            var game = new GameEntity(startTime, room);

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