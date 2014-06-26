using System;
using System.Linq;

using NGrams.Profiles;

namespace NGrams.DistanceCalculation
{
	/// <summary>
	///		Расстояние Матуситы sqrt( sum ( (sqrt(xi) - sqrt(yi))^2 ) )
	/// </summary>
	public class MatusitaDistance:DistanceBase
	{
		public override double GetDistance<TCriteria>(IProfile<TCriteria> profile1, IProfile<TCriteria> profile2)
		{
			var criteries = profile1.RawOccurencies.Keys.Union(profile2.RawOccurencies.Keys);

			double sum = 0;

			foreach (var criteria in criteries)
			{
				double sqrt1 = Math.Sqrt((double)profile1.GetCriteriaOccurencyFrequency(criteria));
				double sqrt2 = Math.Sqrt((double)profile2.GetCriteriaOccurencyFrequency(criteria));

				sum += Math.Pow(sqrt1 - sqrt2, 2);
			}

			return Math.Sqrt(sum);
		}
	}
}
