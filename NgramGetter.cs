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
         
        public NgramGetter (IEnumerable<string> fileNames, int ngramLength, string matchPattern)
        {
                        
        }
                
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
            Queue<char> charQueue = new Queue<char>(_ngramLength);
                        
            using (StreamReader reader = new StreamReader(fileName,Encoding.UTF8)) {
                while (!reader.EndOfStream && reader.BaseStream.CanRead) {
                    FillQueue(charQueue, reader);
                    ngrams.AddOrUpdate(new string(charQueue.ToArray()),
                                                           1,
                                                           (key,val) => val + 1);
                    charQueue.Dequeue();
                                        
                }
            }
                 
            NGramRawOccurencies = ngrams;
            int totalChars = NGramRawOccurencies.Sum(x => x.Value);
            NGramProbability = NGramRawOccurencies
                                .Select(x => new {
                                        Key = x.Key,
                                        Value = Decimal.Divide(x.Value, totalChars)})
                                 .ToDictionary(x => x.Key, y => y.Value);
        }
                
        private void FillQueue (Queue<char> queue, StreamReader reader)
        {
            int readResult;
            while (queue.Count < _ngramLength && (readResult = reader.Read()) != -1) {
                char c = (char)readResult;
                if (Regex.IsMatch(c.ToString(), _matchPattern)) {
                    queue.Enqueue(Char.ToUpper(c));
                }
            }
        }
         
    }
}

