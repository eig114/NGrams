using System;
using System.Collections.Generic;

namespace NGrams.Extensions
{
        public static class IDictionaryExtensions
        {
                public static void AddOrUpdate<TKey, TValue> (this IDictionary<TKey,TValue> dict,
                                                     TKey key,
                                                     TValue newValue,
                                                     Func<TKey, TValue, TValue> updater)
                {
                        if (dict.ContainsKey (key)) {
                                dict [key] = updater (key, dict [key]);
                        }
                        else {
                                dict.Add (key, newValue);
                        }
                }
        }
}

