namespace NGrams.DistanceCalculation
{
	using System.Collections.Generic;

	using NGrams.Profiles;

	public static class DistanceCalculator
	{
		public static double GetDistance<TCriteria, TDistance>(this IProfile<TCriteria> p1, IProfile<TCriteria> p2) where  TDistance:IDistance,new()
		{
			var distance = new TDistance();
			return distance.GetDistance(p1, p2);
		}

		public static IDictionary<IProfile<TCriteria>, double> GetAbsoluteDistances<TCriteria, TDistance>(
			this IProfile<TCriteria> p1,
			IEnumerable<IProfile<TCriteria>> other) where TDistance : IDistance, new()
		{
			var distance = new TDistance();
			return distance.GetAbsoluteDistances(p1, other);
		}

		public static IDictionary<IProfile<TCriteria>, double> GetRelativeDistances<TCriteria, TDistance>(
			this IProfile<TCriteria> p1,
			IEnumerable<IProfile<TCriteria>> other) where TDistance : IDistance, new()
		{
			var distance = new TDistance();
			return distance.GetRelativeDistances(p1, other);
		}
	}
}
