using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGrams.Profiles;
using NGrams.Extensions;

namespace NGrams
{
    public static class ProfileExtensions
    {

        public static decimal GetDistanceWithNormal(this IProfile p1, IProfile p2, IProfile normal){
            decimal sum=0;
            foreach(var ngram in normal.Probability){
                string ngramString = ngram.Key;

                decimal freq;
                decimal freq1 = p1.Probability.TryGetValue(ngramString, out freq)
                                        ? freq
                                        : 0;
                decimal freq2 = p2.Probability.TryGetValue(ngramString, out freq)
                                        ? freq
                                        : 0;
                sum+=GetNormalizedDistance(freq1, freq2, ngram.Value);
            }
            return sum;

        }


        public static decimal GetDistanceWithoutNormal(this IProfile p1, IProfile p2){
            var ngrams = p1.Probability.Keys.Union(p2.Probability.Keys);

            decimal sum=0;
            foreach(var ngram in ngrams){
                decimal freq;
                decimal freq1 = p1.Probability.TryGetValue(ngram, out freq)
                                        ? freq
                                        : 0;
                decimal freq2 = p2.Probability.TryGetValue(ngram, out freq)
                                        ? freq
                                        : 0;
                sum+=GetDistance(freq1, freq2);
            }

            return sum;
        }

        public static decimal GetSimpleDistance(this IProfile p1, IProfile p2){
            return  p1.RawOccurencies.Sum(x => {
                int p2Occurencies=0;
                if (p2.RawOccurencies.TryGetValue(x.Key,out p2Occurencies)){
                    return p2Occurencies;
                }
                return 0;
            });
        }

        public static IDictionary<IProfile,decimal> GetSimpleDistances(
            this IProfile p1,
            IEnumerable<IProfile> other)
        {
            Dictionary<IProfile,decimal> result = new Dictionary<IProfile, decimal>();
            decimal sum=0;
            foreach(var p2 in other){
                decimal distance = p1.GetSimpleDistance(p2);
                sum+=distance;
                result.Add(p2,distance);
            }

            if (sum ==0){
                result = result
                .Select(x=> new KeyValuePair<IProfile,decimal>(x.Key, 0))
                .ToDictionary(x=> x.Key, y=> y.Value);
            }
            else{
                result = result
                .Select(x=> new KeyValuePair<IProfile,decimal>(x.Key, x.Value/sum))
                .ToDictionary(x=> x.Key, y=> y.Value);
            }

            return result;
        }

        public static IDictionary<IProfile,decimal> GetDistancesWithNormal(
            this IProfile p1,
            IEnumerable<IProfile> other,
            IProfile normal)
        {
            Dictionary<IProfile,decimal> result = new Dictionary<IProfile, decimal>();
            decimal sum=0;
            foreach(var p2 in other){
                decimal distance = p1.GetDistanceWithNormal(p2,normal);
                sum+=distance;
                result.Add(p2,distance);
            }

            decimal sum2 = result.Sum(x=> sum-x.Value);
            result = result
                .Select(x=> new KeyValuePair<IProfile,decimal>(x.Key, (sum-x.Value) /sum2)) ///sum))
                .ToDictionary(x=> x.Key, y=> y.Value);

            return result;
        }

        public static IDictionary<IProfile,decimal> GetDistancesWithoutNormal(
            this IProfile p1,
            IEnumerable<IProfile> other)
        {
            Dictionary<IProfile,decimal> result = new Dictionary<IProfile, decimal>();
            decimal sum=0;
            foreach(var p2 in other){
                decimal distance = p1.GetDistanceWithoutNormal(p2);
                sum+=distance;
                result.Add(p2,distance);
            }

            decimal sum2 = result.Sum(x=> sum-x.Value);
            result = result
                .Select(x=> new KeyValuePair<IProfile,decimal>(x.Key, (sum-x.Value) /sum2))///sum))
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
        private static decimal GetNormalizedDistance (
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
                        
            return Math.Abs(x * y);
        }

        /// <summary>
        /// Получает расстояние между двумя частотами Н-грам
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
        private static decimal GetDistance (
                        decimal freq1,
                        decimal freq2)
        {
            decimal sum = freq1 + freq2;
                        
            decimal x = sum != 0 
                                ? 2 * (freq1 - freq2) / sum
                                : 0;
            return Math.Abs(x*x);
        }
    }
}

