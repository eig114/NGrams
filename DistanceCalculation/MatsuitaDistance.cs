using System;
using System.Collections.Generic;
using System.Linq;
using NGrams.Profiles;

namespace NGrams.DistanceCalculation
{
    /// <summary>
    /// d = sqrt(sum((sqrt(xi) - sqrt(yi))^2))
    /// Может быть весьма медленным 
    /// </summary>
    public static class MatsuitaDistance
    {
        public static double GetMatsuitaDistance<TCriteria>(this IProfile<TCriteria> profile1, IProfile<TCriteria> profile2){
            var criteries = profile1.RawOccurencies.Keys.Union(profile2.RawOccurencies.Keys);

            double sum=0;

            foreach(var criteria in criteries){
                double sqrt1 = Math.Sqrt((double)profile1.GetCriteriaOccurencyFrequency(criteria));
                double sqrt2 = Math.Sqrt((double)profile2.GetCriteriaOccurencyFrequency(criteria));
                sum+=Math.Pow(sqrt1 - sqrt2, 2);
            }

            return Math.Sqrt(sum);
        }

		public static IDictionary<IProfile<TCriteria>, double> GetMatsuitaDistances<TCriteria>(
			this IProfile<TCriteria> p1,
			IEnumerable<IProfile<TCriteria>> other)
		{
			var result = new Dictionary<IProfile<TCriteria>, double>();
			double sum = 0;
			foreach (var p2 in other)
			{
				double distance = p1.GetMatsuitaDistance(p2);
				sum += distance;
				result.Add(p2, distance);
			}

			double sum2 = result.Sum(x => sum - x.Value);
			result = result
				.Select(x => new KeyValuePair<IProfile<TCriteria>, double>(x.Key, (sum - x.Value) / sum2))
				.ToDictionary(x => x.Key, y => y.Value);

			return result;
		}
    }
}

