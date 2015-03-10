using System;
using NGrams.Profiles;

namespace NGrams.DistanceCalculation
{
    /// <summary>
    /// d = sum( 1 - min(xi, yi)/max(xi, yi) )
    /// </summary>
    public class WaveHedgesDistance:DistanceBase
    {
        public override double GetDistance<TCriteria> (IProfile<TCriteria> profile1, IProfile<TCriteria> profile2)
        {
            var criteries = MergeCriteries(profile1,profile2);
            double sum=0;
            foreach(var criteria in criteries){
                double min  = (double)Math.Min(
                    profile1.GetCriteriaOccurencyFrequency(criteria),
                    profile2.GetCriteriaOccurencyFrequency(criteria));

                double max = (double)Math.Max(
                    profile1.GetCriteriaOccurencyFrequency(criteria),
                    profile2.GetCriteriaOccurencyFrequency(criteria));

                sum+=(1- min/max);
            }

            return sum;
        }
    }
}

