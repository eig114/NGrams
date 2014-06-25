using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGrams.Profiles;
using NGrams.Extensions;
namespace NGrams.DistanceCalculation
{
    /// <summary>
    /// d = sqrt(sum((sqrt(xi) - sqrt(yi))^2))
    /// </summary>
    public static class MatsuitaDistance
    {
        public static double GetMatsuitaDistance<Criteria>(this IProfile<Criteria> profile1, IProfile<Criteria> profile2){
            var criteries = profile1.RawOccurencies.Keys.Union(profile2.RawOccurencies.Keys);

            double sum=0;

            foreach(var criteria in criteries){
                double sqrt1 = Math.Sqrt((double)profile1.GetCriteriaOccurencyFrequency(criteria));
                double sqrt2 = Math.Sqrt((double)profile2.GetCriteriaOccurencyFrequency(criteria));
                sum+=Math.Pow(sqrt1 - sqrt2, 2);
            }

            return Math.Sqrt(sum);
        }
    }
}

