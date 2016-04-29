using Microsoft.WindowsAzure.Storage.Table;
using WebRole.Data;

namespace WebRole.Models
{
    public class GameEntry : TableEntity
    {
        public string GameId { get; set; }

        public string PlayerId { get; set; }

        public int BuyIn { get; set; }

        public int CashOut { get; set; }

        public Status PlayerStatus { get; set; } 

        private GameEntry(string gameId, string playerId)
        {
            this.GameId = gameId;
            this.PlayerId = playerId;

            this.PartitionKey = this.GameId;
            this.RowKey = this.PlayerId;
        }

        public static GameEntry Create(string gameId, string playerId)
        {
            var gameEntry = new GameEntry(gameId, playerId);
            
            return gameEntry;
        }
    }
}