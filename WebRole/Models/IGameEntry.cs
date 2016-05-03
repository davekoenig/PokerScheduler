using WebRole.Data;

namespace WebRole.Models
{
    interface IGameEntry
    {
        string GameId { get; set; }

        string PlayerId { get; set; }

        int BuyIn { get; set; }

        int CashOut { get; set; }

        Status PlayerStatus { get; set; }
    }
}
