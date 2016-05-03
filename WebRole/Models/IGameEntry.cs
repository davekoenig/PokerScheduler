using WebRole.Data;

namespace WebRole.Models
{
    public interface IGameEntry : IDataModel
    {
        string GameId { get; set; }

        string PlayerId { get; set; }

        int BuyIn { get; set; }

        int CashOut { get; set; }

        Status PlayerStatus { get; set; }
    }
}
