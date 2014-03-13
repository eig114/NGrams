using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGrams.Extensions;
using System.Text.RegularExpressions;

namespace NGrams
{
        /// <summary>
        /// Класс, содержащий функции для получения расстояний между профилями.
        /// </summary>
        public static class Profiler
        {
                 /// <summary>
                /// Получает вектор средних частот  n-грам
                /// </summary>
                /// <returns>
                /// The normal vector.
                /// </returns>
                /// <param name='profiles'>
                /// Profiles.
                /// </param>
                public static IDictionary<string,decimal> GetNormalVector(IEnumerable<IDictionary<string,decimal>> profiles)
                {
                        IDictionary<string,Tuple<decimal,decimal>> ngramSumsAndCounts;
                        foreach (var profile in profiles) {
                                foreach (var ngram in profile) {
                                        ngramSumsAndCounts.AddOrUpdate (
                                                ngram.Key, 
                                                new Tuple<decimal,decimal> (ngram.Value, 1),
                                                (key,val) => new Tuple<decimal,decimal> (ngram.Value, val.Item1 + ngram.Value));
                                }
                        }
                        
                        return ngramSumsAndCounts.ToDictionary (x => x.Key, y => y.Value.Item1 / y.Value.Item2);
                }
                
                /// <summary>
                ///Получает вектор расстояний между двумя профилями, 
                /// также используя усредненный профиль.
                /// </summary>
                /// <returns>
                /// The distance vetor.
                /// </returns>
                /// <param name='profile1'>
                /// Profile1.
                /// </param>
                /// <param name='profile2'>
                /// Profile2.
                /// </param>
                /// <param name='normal'>
                /// Normal.
                /// </param>
                public static IDictionary<string,decimal> GetDistanceVector(
                        IDictionary<string,decimal> profile1, 
                        IDictionary<string,decimal> profile2, 
                        IDictionary<string,decimal> normal)
                {
                        Dictionary<string,decimal> distanceVector = normal.Select (
                                ngram => 
                        {
                                string ngramString = ngram.Key;
                                
                                decimal freq;
                                decimal freq1 = profile1.TryGetValue (ngramString, out freq)
                                        ? freq 
                                        : 0;
                                decimal freq2 = profile2.TryGetValue (ngramString, out freq)
                                        ? freq 
                                        : 0;
                                return new KeyValuePair<string,decimal> (ngramString, GetDistance (freq1, freq2, ngram.Value));
                        }).ToDictionary (x => x.Key, y => y.Value);
                        
                        return distanceVector;
                }      
                
                /// <summary>
                /// Получает расстояние между двумя частотами Н-грам, с 
                /// учетом нормальной по профилю
                /// </summary>
                /// <returns>
                /// The distance.
                /// </returns>
                /// <param name='frequency1'>
                /// Frequency1.
                /// </param>
                /// <param name='frequency2'>
                /// Frequency2.
                /// </param>
                /// <param name='frequencyNormal'>
                /// Frequency normal.
                /// </param>
                public static decimal GetDistance(
                        decimal freq1, 
                        decimal freq2, 
                        decimal freqNorm)
                {
                        // страшная формула.
                        decimal x = 2 * (freq1 - freq2) / (freq1 + freq2);
                        x *= x;
                        
                        decimal y = 2 * (freq1 - freqNorm) / (freq1 + freqNorm);
                        
                        return x * y;
                }
        }
}

