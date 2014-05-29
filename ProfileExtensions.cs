using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGrams.Extensions;

namespace NGrams
{
    public static class ProfileExtensions
    {

        public static decimal GetDistanceWithNormal(this Profile p1, Profile p2, Profile normal){
             Dictionary<string,decimal> distanceVector = normal.NGramProbability.Select(
                                ngram =>
            {
                string ngramString = ngram.Key;

                decimal freq;
                decimal freq1 = p1.NGramProbability.TryGetValue(ngramString, out freq)
                                        ? freq
                                        : 0;
                decimal freq2 = p2.NGramProbability.TryGetValue(ngramString, out freq)
                                        ? freq
                                        : 0;
                return new KeyValuePair<string,decimal>(ngramString, GetDistance(freq1, freq2, ngram.Value));
            }).ToDictionary(x => x.Key, y => y.Value);

            return distanceVector.Sum(x=> x.Value);
        }


        public static decimal GetDistanceWithoutNormal(this Profile p1, Profile p2){
            return 0;
        }

        public static decimal GetSimpleDistance(this Profile p1, Profile p2){
            return p1.NGramRawOccurencies.Count(x => p2.NGramRawOccurencies.ContainsKey(x.Key));
        }

        public static IDictionary<Profile,decimal> GetDistancesWithNormal(
            this Profile p1,
            IEnumerable<Profile> other,
            Profile normal)
        {
            Dictionary<Profile,decimal> result = new Dictionary<Profile, decimal>();
            decimal sum=0;
            foreach(var p2 in other){
                decimal distance = p1.GetDistanceWithNormal(p2,normal);
                sum+=distance;
                result.Add(p2,distance);
            }

            result = result
                .Select(x=> new KeyValuePair<Profile,decimal>(x.Key, x.Value/sum))
                .ToDictionary(x=> x.Key, y=> y.Value);

            return result;
        }

        public static IDictionary<Profile,decimal> GetSimpleDistances(
            this Profile p1,
            IEnumerable<Profile> other)
        {
            Dictionary<Profile,decimal> result = new Dictionary<Profile, decimal>();
            decimal sum=0;
            foreach(var p2 in other){
                decimal distance = p1.GetSimpleDistance(p2);
                sum+=distance;
                result.Add(p2,distance);
            }

            result = result
                .Select(x=> new KeyValuePair<Profile,decimal>(x.Key, x.Value/sum))
                .ToDictionary(x=> x.Key, y=> y.Value);

            return result;
        }

        public static IDictionary<Profile,decimal> GetDistancesWithoutNormal(
            this Profile p1,
            IEnumerable<Profile> other)
        {
            Dictionary<Profile,decimal> result = new Dictionary<Profile, decimal>();
            decimal sum=0;
            foreach(var p2 in other){
                decimal distance = p1.GetDistanceWithoutNormal(p2);
                sum+=distance;
                result.Add(p2,distance);
            }

            result = result
                .Select(x=> new KeyValuePair<Profile,decimal>(x.Key, x.Value/sum))
                .ToDictionary(x=> x.Key, y=> y.Value);

            return result;
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
        private static decimal GetDistance (
                        decimal freq1,
                        decimal freq2,
                        decimal freqNorm)
        {
            decimal sum = freq1 + freq2;
                        
            decimal x = sum != 0 
                                ? 2 * (freq1 - freq2) / sum
                                : 0;
            x *= x;
                        
            sum = freq1 + freqNorm;
            decimal y = sum != 0 
                                ? 2 * (freq1 - freqNorm) / (freq1 + freqNorm)
                                : 0;
                        
            return x * y;
        }
    }
}

