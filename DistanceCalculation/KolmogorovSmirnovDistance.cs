using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NGrams.DistanceCalculation
{
	using NGrams.Profiles;

	public static class KolmogorovSmirnovDistance
	{
		public static decimal GetKolmogorovSmirnovDistance<TCriteria>(this IProfile<TCriteria> profile1, IProfile<TCriteria> profile2)
		{
			var criteries = profile1.Probability.Keys.Union(profile2.Probability.Keys);

			decimal distance = criteries.Sum(criteria => 
				Math.Abs(profile2.GetCriteriaOccurencyFrequency(criteria) - profile1.GetCriteriaOccurencyFrequency(criteria)));

			return distance/2;
		}

		public static IDictionary<IProfile<TCriteria>, double> GetKolmogorovSmirnovDistances<TCriteria>(
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
