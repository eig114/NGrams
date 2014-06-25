using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGrams.Profiles;
using NGrams.Extensions;

namespace NGrams.DistanceCalculation
{
    public static class NonNormalizedDistance
    {
        public static decimal GetDistanceWithoutNormal<Criteria>(this IProfile<Criteria> p1, IProfile<Criteria> p2){
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

        public static IDictionary<IProfile<Criteria>,decimal> GetDistancesWithoutNormal<Criteria>(
            this IProfile<Criteria> p1,
            IEnumerable<IProfile<Criteria>> other)
        {
            Dictionary<IProfile<Criteria>,decimal> result = new Dictionary<IProfile<Criteria>, decimal>();
            decimal sum=0;
            foreach(var p2 in other){
                decimal distance = p1.GetDistanceWithoutNormal(p2);
                sum+=distance;
                result.Add(p2,distance);
            }

            decimal sum2 = result.Sum(x=> sum-x.Value);
            result = result
                .Select(x=> new KeyValuePair<IProfile<Criteria>,decimal>(x.Key, (sum-x.Value) /sum2))///sum))
                .ToDictionary(x=> x.Key, y=> y.Value);

            return result;
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

