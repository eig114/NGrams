using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGrams.Extensions;
using System.Text.RegularExpressions;

namespace NGrams
{
 
        /// <summary>
        ///      Ngram getter.
        /// </summary>
        public class NgramGetter
        {
                private const string DefaultMatchPattern = @"^\p{L}+$";
                private const int DefaultNGramLength = 3;
                
                private string _matchPattern;
                private string _fileName;
                private int _ngramLength;
         
                public NgramGetter (string fileName, int ngramLength, string matchPattern)
                {
                        _matchPattern = matchPattern;
                        _ngramLength = ngramLength;
                        _fileName = fileName;
                        ParseFile(fileName, ngramLength);
                }
         
                public NgramGetter (string fileName, int ngramLength)
                 :this(fileName,ngramLength,DefaultMatchPattern)
                {
                }
                
                public NgramGetter (string fileName, string matchPattern)
                 :this(fileName,DefaultNGramLength,matchPattern)
                {
                }
         
                public NgramGetter (string fileName)
                 :this(fileName,DefaultNGramLength,DefaultMatchPattern)
                {
                }
         
                /// <summary>
                ///      Имя файла, с которым ассоциирован получатель N-грамм
                /// </summary>
                public string FileName {
                        get { return _fileName;}
                        private set {
                                _fileName = value;
                                ParseFile(_fileName, N);
                        }
                }
         
                /// <summary>
                ///      Получает кол-во символов в каждой N-грамме
                /// </summary>
                public int N {
                        get { return _ngramLength;} 
                        private set {
                                _ngramLength = value;
                                ParseFile(_fileName, _ngramLength);
                        }
                }
         
                /// <summary>
                ///     Получает паттерн для фильтрации текста
                /// </summary>
                /// <value>
                /// The chars to strip.
                /// </value>
                public string MatchPattern {
                        get{ return _matchPattern;}
                        private set {
                                _matchPattern = value;
                                ParseFile(_fileName, _ngramLength);
                        }
                }
         
                /// <summary>
                ///      Получает N-граммы, выделенные при последней обработке файла,
                ///      и их количество в тексте.
                /// </summary>
                public IDictionary<string,int> NGramRawOccurencies { get; private set; }
                
                public IDictionary<string,decimal> NGramProbability { get; private set; }
                         
                /// <summary>
                ///      Проходит по файлу, запоминая найденные N-граммы.
                /// </summary>
                /// <param name='ngramLength'>
                /// Ngram Length
                /// </param>
                /// <param name='fileName'>
                /// File name.
                /// </param>
                private void ParseFile (string fileName, int ngramLength)
                {
                        Dictionary<string,int> ngrams = new Dictionary<string, int>();
                 
                        using (StreamReader reader = new StreamReader(fileName,Encoding.UTF8)) {
                                while (!reader.EndOfStream && reader.BaseStream.CanRead)
                                        ngrams.AddOrUpdate(GetNextNGram(reader, ngramLength),
                                                    1,
                                                    (key,val) => val + 1);
                        }
                 
                        NGramRawOccurencies = ngrams;
                        int totalChars = NGramRawOccurencies.Sum(x => x.Value);
                        NGramProbability = NGramRawOccurencies
                                .Select(x => new {
                                        Key = x.Key,
                                        Value = Decimal.Divide(x.Value, totalChars)})
                                 .ToDictionary(x => x.Key, y => y.Value);
                }
         
                /// <summary>
                ///      Получает следующую N-грамму из потока, 
                ///      читаемого <paramref name='reader'/>
                /// </summary>
                /// <returns>
                ///      Следующая N-грамму из потока. Если файл закончился раньше, 
                ///      чем N было достигнуто, возвращается неполная N-грамма.
                /// </returns>
                /// <param name='reader'>
                ///      Reader.
                /// </param>
                /// <param name='ngramSize'>
                ///      Ngram size.
                /// </param>
                private string GetNextNGram (StreamReader reader, int ngramSize)
                {
                        StringBuilder tmpBuilder = new StringBuilder();
                        int readResult;
                        while ((readResult = reader.Read()) != -1 && // BaseStream.CanRead &&
                        tmpBuilder.Length < ngramSize) {
                                char c = (char)readResult;
                                //bool ismatch = Regex.IsMatch(c.ToString(), @"^\p{L}+$");
                                //if (!_charsToStrip.Contains(c)) {
                                if (Regex.IsMatch(c.ToString(), _matchPattern)) {
                                        tmpBuilder.Append(Char.ToUpper(c));
                                }
                        }
                 
                        return tmpBuilder.ToString();
                }
         
         
        }
}

