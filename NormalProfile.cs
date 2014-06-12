using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NGrams
{
	/// <summary>
	/// The normal profile.
	/// </summary>
	public class NormalProfile : IProfile
	{
		public int N { get; private set; }

		public string MatchPattern { get; private set; }

		public IDictionary<string, int> NGramRawOccurencies { get; private set; }

		public IDictionary<string, decimal> NGramProbability { get; private set; }
	}
}
