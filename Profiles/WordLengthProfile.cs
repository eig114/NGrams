using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using NGrams.Extensions;
using System.Text.RegularExpressions;

namespace NGrams.Profiles
{
    public class WordLengthProfile:IProfile<int>
    {
        // по умолчанию принимаем все буквенные символы Юникода
        private const string DefaultMatchPattern = @"\p{L}+";

        private const char DEFAULT_SEPARATOR = ' ';

        private readonly char[] _separators;

        public WordLengthProfile (string authorName, char[] separators)
        {
            _separators = separators;
            AuthorName = authorName;
            RawOccurencies = new Dictionary<int,int>();
            Probability = new Dictionary<int,decimal>();
        }

        public WordLengthProfile(string authorName): this(authorName, new []{DEFAULT_SEPARATOR})
        {

        }

        public void AddFile (string fileName)
        {
            Dictionary<int,int> wordLengths = ParseFile(fileName);

            foreach(var length in wordLengths){
                RawOccurencies.AddOrUpdate(
                    length.Key,
                    length.Value,
                    (key,oldValue)=> oldValue+length.Value );
            }

            RecountProbabilities();
        }

        public string AuthorName {get; private set;}

        public IDictionary<int, int> RawOccurencies {get; private set;}

        public IDictionary<int, decimal> Probability {get; private set;}

        private Dictionary<int,int> ParseFile (string fileName)
        {
            Dictionary<int,int> wordLengths = new Dictionary<int, int>();

            using (StreamReader reader = new StreamReader(fileName,Encoding.UTF8)) {
                while (!reader.EndOfStream && reader.BaseStream.CanRead) {
                    var words =reader
                            .ReadLine()
                            .Split(_separators);
                    // todo Учитывать перенос

                    
                    var filteredWords = words
                            .Select(x=> concatGroups(Regex.Match(x,DefaultMatchPattern).Groups))
                            .Where(x=> x.Length>0);
                    foreach (var word in filteredWords){
                        wordLengths.AddOrUpdate(word.Length, 1, (key,val)=>val+1);
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

        private void RecountProbabilities(){
            int totalChars = RawOccurencies.Sum(x => x.Value);
            Probability = RawOccurencies
                                .Select(x => new {
                                        Key = x.Key,
                                        Value = Decimal.Divide(x.Value, totalChars)})
                                 .ToDictionary(x => x.Key, y => y.Value);
        }
    }
}

