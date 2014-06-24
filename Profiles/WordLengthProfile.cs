using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using NGrams.Extensions;
using System.Text.RegularExpressions;

namespace NGrams.Profiles
{
    public class WordLengthProfile:ProfileBase<int>
    {
        // по умолчанию принимаем все буквенные символы Юникода
        private const string DefaultMatchPattern = @"\p{L}+";

        private const char DEFAULT_SEPARATOR = ' ';

        private readonly char[] _separators;

        public WordLengthProfile (string authorName, char[] separators)
			:base(authorName)
        {
            _separators = separators;
        }

        public WordLengthProfile(string authorName): this(authorName, new []{DEFAULT_SEPARATOR})
        {}

        protected override IDictionary<int,int> ParseFile (string fileName)
        {
            var wordLengths = new Dictionary<int, int>();

            using (StreamReader reader = new StreamReader(fileName,Encoding.UTF8)) {
                while (!reader.EndOfStream && reader.BaseStream.CanRead)
                {
	                var readLine = reader.ReadLine();
	                if (readLine != null)
	                {
		                var words =readLine
			                .Split(this._separators);
		                // todo Учитывать перенос

		                var filteredWords = words
			                .Select(x=> this.concatGroups(Regex.Match(x,DefaultMatchPattern).Groups))
			                .Where(x=> x.Length>0);
		                foreach (var word in filteredWords){
			                wordLengths.AddOrUpdate(word.Length, 1, (key,val)=>val+1);
		                }
	                }
                }
            }

            return wordLengths;
        }

        private string concatGroups(GroupCollection groupCollection){
            StringBuilder builder = new StringBuilder();
            foreach(var g in groupCollection){
                builder.Append(g);
            }
            return builder.ToString();
        }
    }
}

