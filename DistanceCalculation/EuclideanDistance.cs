using System;

using NGrams.Profiles;

namespace NGrams.DistanceCalculation
{
	/// <summary>
	///		Евклидово расстояние sqrt( sum ((xi-yi)^2) )
	/// </summary>
	public class EuclideanDistance : DistanceBase
	{
		public override double GetDistance<TCriteria>(IProfile<TCriteria> profile1, IProfile<TCriteria> profile2)
		{
			var criteries = this.MergeCriteries(profile1, profile2);

			double sum = 0;
			foreach (var ngram in criteries)
			{
				decimal freq;
				decimal freq1 = profile1.Probability.TryGetValue(ngram, out freq)
										? freq
										: 0;
				decimal freq2 = profile2.Probability.TryGetValue(ngram, out freq)
										? freq
										: 0;
				double dif = (double)(freq1 - freq2);

				sum += dif * dif;
			}

			return Math.Sqrt(sum);
		}
	}
}
