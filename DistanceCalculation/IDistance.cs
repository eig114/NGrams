using System.Collections.Generic;
using NGrams.Profiles;

namespace NGrams.DistanceCalculation
{
	/// <summary>
	///		Интерфейс расстояния между профилями
	/// </summary>
	public interface IDistance
	{
		double GetDistance<TCriteria>(IProfile<TCriteria> profile1, IProfile<TCriteria> profile2);

		IDictionary<IProfile<TCriteria>, double> GetAbsoluteDistances<TCriteria>(
			IProfile<TCriteria> p1,
			IEnumerable<IProfile<TCriteria>> other);

		IDictionary<IProfile<TCriteria>, double> GetRelativeDistances<TCriteria>(
			IProfile<TCriteria> p1,
			IEnumerable<IProfile<TCriteria>> other);
	}
}
