using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole.Models
{
    internal class Player : TableEntity, IEquatable<Player>
    {
        internal string FirstName { get; set; }
        internal string LastName { get; set; }

        internal string Alias { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Player other = obj as Player;
            if (other == null)
            {
                return false;
            }

            return (other.FirstName == FirstName) && (other.LastName == LastName);
        }

        public bool Equals(Player other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals((object)other);
        }

        public static bool operator==(Player left, Player right)
        {
            return left.Equals(right);
        }

        public static bool operator!=(Player left, Player right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return FirstName.GetHashCode() ^ LastName.GetHashCode() * 37;
        }
    }
}