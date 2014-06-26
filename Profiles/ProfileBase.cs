using System;
using System.Collections.Generic;
using System.Linq;

namespace NGrams.Profiles
{
	using NGrams.Extensions;

	public abstract class ProfileBase<Criteria> : IProfile<Criteria>
	{
		private readonly List<string> _fileNames;
		
		private readonly Guid id;

		public ProfileBase(string authorName)
		{
			this._fileNames = new List<string>();
			
			id = Guid.NewGuid();
			
			AuthorName = authorName;
			
			RawOccurencies = new Dictionary<Criteria, int>();
			Probability = new Dictionary<Criteria, decimal>();
		}

		public string AuthorName { get; private set; }

		public IDictionary<Criteria, int> RawOccurencies { get; private set; }

		public int GetCriteriaOccurencyCount(Criteria c)
		{
			int count;
			return RawOccurencies.TryGetValue(c, out count) ? count : 0;
		}

		public IDictionary<Criteria, decimal> Probability { get; private set; }

		/// <summary>
		///      Получает имена файлов, обработанных для получения N-грамм
		/// </summary>
		public IEnumerable<string> FileNames
		{
			get { return this._fileNames; }
		}

		public decimal GetCriteriaOccurencyFrequency(Criteria c)
		{
			decimal freq;
			return Probability.TryGetValue(c, out freq) ? freq : 0;
		}

		/// <summary>
		///     Учитывает в профиле текст из файла <paramref name="fileName"/>
		/// </summary>
		/// <param name='fileName'> Имя файла. </param>
		public void AddFile(string fileName)
		{
			IDictionary<Criteria, int> ngrams = ParseFile(fileName);

			foreach (var ngram in ngrams)
			{
				// чтобы не использовать переменную foreach в анонимной ф-ии
				KeyValuePair<Criteria, int> ngramLocal = ngram;
				
				RawOccurencies.AddOrUpdate(
					ngram.Key,
					ngram.Value,
					(key, oldValue) => oldValue + ngramLocal.Value);
			}

			RecountProbabilities();
		}

		/// <summary>
		///     Пересчитывает частоты использования N-грам в профиле.
		/// </summary>
		private void RecountProbabilities()
		{
			int totalChars = RawOccurencies.Sum(x => x.Value);
			Probability = RawOccurencies
								.Select(x => new
								{
									Key = x.Key,
									Value = Decimal.Divide(x.Value, totalChars)
								})
								 .ToDictionary(x => x.Key, y => y.Value);
		}

		/// <summary>
		///      Проходит по файлу, запоминая найденные N-граммы.
		/// </summary>
		/// <returns> Словарь "N-грамма -- количество упоминаний" </returns>
		/// <param name='fileName'> Имя файла</param>
		protected abstract IDictionary<Criteria, int> ParseFile(string fileName);

		public override bool Equals(object obj)
		{
			NgramProfile other = obj as NgramProfile;

			if (other != null)
			{
				return other.id.Equals(this.id);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}
	}
}
