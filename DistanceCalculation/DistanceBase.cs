using System.Collections.Generic;
using System.Linq;

using NGrams.Profiles;

namespace NGrams.DistanceCalculation
{
	/// <summary>
	///		Базовый класс для функций расстояния. содержит расширения для определения ближайшего соседа из нескольких вариантов
	/// </summary>
	public abstract class DistanceBase:IDistance
	{
		protected TCriteria[] MergeCriteries<TCriteria>(IProfile<TCriteria> p1, IProfile<TCriteria> p2)
		{
			return p1.RawOccurencies.Keys.Union(p2.RawOccurencies.Keys).ToArray();
		}

		public abstract double GetDistance<TCriteria>(IProfile<TCriteria> profile1, IProfile<TCriteria> profile2);

		public IDictionary<IProfile<TCriteria>, double> GetAbsoluteDistances<TCriteria>(
			IProfile<TCriteria> p1,
			IEnumerable<IProfile<TCriteria>> other)
		{
			var result = new Dictionary<IProfile<TCriteria>, double>();
			double sum = 0;
			foreach (var p2 in other)
			{
				double distance = this.GetDistance(p1, p2);
				sum += distance;
				result.Add(p2, distance);
			}

			return result;
		}

		public IDictionary<IProfile<TCriteria>, double> GetRelativeDistances<TCriteria>(
			IProfile<TCriteria> p1,
			IEnumerable<IProfile<TCriteria>> other)
		{
			var result = this.GetAbsoluteDistances(p1, other);
			var sum = result.Sum(x => x.Value);

			double sum2 = result.Sum(x => sum - x.Value);
			result = result
				.Select(x => new KeyValuePair<IProfile<TCriteria>, double>(x.Key, (sum - x.Value) / sum2))
				.ToDictionary(x => x.Key, y => y.Value);

			return result;
		}
	}
}
