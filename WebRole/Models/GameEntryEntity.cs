﻿using Microsoft.WindowsAzure.Storage.Table;
using WebRole.Data;

namespace WebRole.Models
{
    public class GameEntryEntity : TableEntity, IGameEntry
    {
        public string Id => $"{GameId}-{PlayerId}";

        public string GameId { get; set; }

        public string PlayerId { get; set; }

        public int BuyIn { get; set; }

        public int CashOut { get; set; }

        public Status PlayerStatus { get; set; } 

        public GameEntryEntity()
        {
            this.PartitionKey = this.GameId;
            this.RowKey = this.PlayerId;
        }

        private GameEntryEntity(string gameId, string playerId) : this()
        {
            this.GameId = gameId;
            this.PlayerId = playerId;
        }

        public static GameEntryEntity Create(string gameId, string playerId)
        {
            var gameEntry = new GameEntryEntity(gameId, playerId);
            
            return gameEntry;
        }
    }
}