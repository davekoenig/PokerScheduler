using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace WebRole.Models
{
    public class PlayerEntity : TableEntity, IEquatable<IPlayer>, IPlayer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        private PlayerEntity(string first, string last, string email)
        {
            this.FirstName = first;
            this.LastName = last;
            this.Email = email;

            this.PartitionKey = FullName;
            this.RowKey = Email;
        }

        public static PlayerEntity Create(string first, string last, string email)
        {
            return new PlayerEntity(first, last, email);
        }



        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            IPlayer other = obj as IPlayer;
            if (other == null)
            {
                return false;
            }

            return (other.FirstName == FirstName) && (other.LastName == LastName);
        }

        public bool Equals(IPlayer other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals((object)other);
        }

        public static bool operator==(PlayerEntity left, PlayerEntity right)
        {
            return left.Equals(right);
        }

        public static bool operator!=(PlayerEntity left, PlayerEntity right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return FirstName.GetHashCode() ^ LastName.GetHashCode() * 37;
        }
    }
}