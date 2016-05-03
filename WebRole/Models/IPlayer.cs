namespace WebRole.Models
{
    public interface IPlayer : IDataModel
    {
        string FirstName { get; set; }

        string LastName { get; set; }

        string Email { get; set; }
    }
}
