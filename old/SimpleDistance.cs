using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGrams.Profiles;
using NGrams.Extensions;

namespace NGrams.DistanceCalculation
{
    public static class SimpleDistance
    {
        public static decimal GetSimpleDistance<Criteria>(this IProfile<Criteria> p1, IProfile<Criteria> p2){
            return  p1.RawOccurencies.Sum(x => {
                int p2Occurencies=0;
                if (p2.RawOccurencies.TryGetValue(x.Key,out p2Occurencies)){
                    return p2Occurencies;
                }
                return 0;
            });
        }

        public static IDictionary<IProfile<Criteria>,decimal> GetSimpleDistances<Criteria>(
            this IProfile<Criteria> p1,
            IEnumerable<IProfile<Criteria>> other)
        {
            Dictionary<IProfile<Criteria>,decimal> result = new Dictionary<IProfile<Criteria>, decimal>();
            decimal sum=0;
            foreach(var p2 in other){
                decimal distance = p1.GetSimpleDistance(p2);
                sum+=distance;
                result.Add(p2,distance);
            }

            if (sum ==0){
                result = result
                .Select(x=> new KeyValuePair<IProfile<Criteria>,decimal>(x.Key, 0))
                .ToDictionary(x=> x.Key, y=> y.Value);
            }
            else{
                result = result
                .Select(x=> new KeyValuePair<IProfile<Criteria>,decimal>(x.Key, x.Value/sum))
                .ToDictionary(x=> x.Key, y=> y.Value);
            }

            return result;
        }
    }
}

