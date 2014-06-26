using System;
using System.Linq;

using NGrams.Profiles;

namespace NGrams.DistanceCalculation
{
	/// <summary>
	///		Расстояние Колмогорова-Смирнова sum( xi-yi ) / 2
	/// </summary>
	public class KolmogorovSmirnovDistance : DistanceBase
	{
		public override double GetDistance<TCriteria>(IProfile<TCriteria> profile1, IProfile<TCriteria> profile2)
		{
			var criteries = this.MergeCriteries(profile1, profile2);

			decimal distance = criteries.Sum(criteria =>
				Math.Abs(profile2.GetCriteriaOccurencyFrequency(criteria) - profile1.GetCriteriaOccurencyFrequency(criteria)));

			return (double)(distance / 2);
		}
	}
}
