using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRole.Models;

namespace WebRole.Data
{
    // BARF, this hacks sucks
    public static class StorageMapping
    {
        private static Dictionary<Type, string> _mappings = new Dictionary<Type, string>();

        static StorageMapping()
        {
            _mappings.Add(typeof(IGame), "Games");
            _mappings.Add(typeof(IGameEntry), "Entries");
            _mappings.Add(typeof(IPlayer), "Players");
        }

        public static string GetStorageLocation(Type type)
        {
            return _mappings[type];
        }

        public static IEnumerable<Type> GetSupportedStorageTypes()
        {
            return _mappings.Keys;
        }

    }
}