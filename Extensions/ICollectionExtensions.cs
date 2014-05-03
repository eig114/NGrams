using System;
using System.Collections.Generic;

namespace NGrams.Extensions
{
    public static class ICollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> toAdd){
            foreach (var addition in toAdd){
                collection.Add(addition);
            }
        }
    }
}

