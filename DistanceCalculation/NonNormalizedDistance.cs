using System;
using NGrams.Profiles;

namespace NGrams.DistanceCalculation
{
	/// <summary>
	///		Ненормализованное расстояние abs( sum( (2*(xi-yi)) / (xi+yi) ) )
	/// </summary>
	public class NonNormalizedDistance :DistanceBase
    {
		public override double GetDistance<TCriteria>(IProfile<TCriteria> profile1, IProfile<TCriteria> profile2)
		{
			var criteries = MergeCriteries(profile1,profile2);

			decimal sum = 0;
			foreach (var ngram in criteries)
			{
				decimal freq;
				decimal freq1 = profile1.Probability.TryGetValue(ngram, out freq)
										? freq
										: 0;
				decimal freq2 = profile2.Probability.TryGetValue(ngram, out freq)
										? freq
										: 0;

				decimal x = (freq1 + freq2) != 0
									? 2 * (freq1 - freq2) / (freq1 + freq2)
									: 0;
				sum+= Math.Abs(x * x);
			}

			return (double)sum;
		}
    }
}

