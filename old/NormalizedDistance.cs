using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGrams.Profiles;
using NGrams.Extensions;

namespace NGrams.DistanceCalculation
{
    public static class NormalizedDistance
    {
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

        public static IDictionary<IProfile<Criteria>,decimal> GetDistancesWithNormal<Criteria>(
            this IProfile<Criteria> p1,
            IEnumerable<IProfile<Criteria>> other,
            IProfile<Criteria> normal)
        {
            Dictionary<IProfile<Criteria>,decimal> result = new Dictionary<IProfile<Criteria>, decimal>();
            decimal sum=0;
            foreach(var p2 in other){
                decimal distance = p1.GetDistanceWithNormal(p2,normal);
                sum+=distance;
                result.Add(p2,distance);
            }

            decimal sum2 = result.Sum(x=> sum-x.Value);
            result = result
                .Select(x=> new KeyValuePair<IProfile<Criteria>,decimal>(x.Key, (sum-x.Value) /sum2)) ///sum))
                .ToDictionary(x=> x.Key, y=> y.Value);

            return result;
        }

        public static decimal GetDistanceWithNormal<Criteria>(this IProfile<Criteria> p1, IProfile<Criteria> p2, IProfile<Criteria> normal){
            decimal sum=0;
            foreach(var ngram in normal.Probability){
                Criteria ngramString = ngram.Key;

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

    }
}

