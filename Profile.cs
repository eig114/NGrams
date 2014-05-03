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
    ///       Профиль автора.
    /// </summary>
    public class Profile
    {
        // по умолчанию принимаем все буквенные символы Юникода
        private const string DefaultMatchPattern = @"^\p{L}+$";
        private const int DefaultNGramLength = 3;

        private List<string> _fileNames;

        public Profile (int ngramLength, string matchPattern)
        {
            MatchPattern = matchPattern;
            N = ngramLength;

            _fileNames = new List<string>();
            NGramRawOccurencies = new Dictionary<string,int>();
            NGramProbability = new Dictionary<string,decimal>();
        }

        public Profile (int ngramLength)
                 :this(ngramLength,DefaultMatchPattern)
        {
        }

        public Profile (string matchPattern)
                 :this(DefaultNGramLength,matchPattern)
        {
        }

        public Profile ()
                 :this(DefaultNGramLength,DefaultMatchPattern)
        {
        }

        /// <summary>
        ///      Получает имена файлов, обработанных для получения N-грамм
        /// </summary>
        public IEnumerable<string> FileNames {
            get { return _fileNames;}
        }

        /// <summary>
        ///      Получает кол-во символов в каждой N-грамме
        /// </summary>
        public int N { get; private set; }

        /// <summary>
        ///     Получает паттерн для фильтрации текста
        /// </summary>
        public string MatchPattern { get; private set; }

        /// <summary>
        ///      Получает N-граммы и их количество во всех обработанных текстах.
        /// </summary>
        public IDictionary<string,int> NGramRawOccurencies { get; private set; }

        /// <summary>
        ///     Получает частоту использования N-грамм
        /// </summary>
        public IDictionary<string,decimal> NGramProbability { get; private set; }

        /// <summary>
        ///     Учитывает в профиле текст из файла <paramref name="fileName"/>
        /// </summary>
        /// <param name='fileName'> Имя файла. </param>
        public void AddFile(string fileName){
            Dictionary<string,int> ngrams = ParseFile(fileName);

            NGramRawOccurencies.AddRange(ngrams);

            RecountProbabilities();
        }

        /// <summary>
        ///     Пересчитывает частоты использования N-грам в профиле.
        /// </summary>
        private void RecountProbabilities(){
            int totalChars = NGramRawOccurencies.Sum(x => x.Value);
            NGramProbability = NGramRawOccurencies
                                .Select(x => new {
                                        Key = x.Key,
                                        Value = Decimal.Divide(x.Value, totalChars)})
                                 .ToDictionary(x => x.Key, y => y.Value);
        }

        /// <summary>
        ///      Проходит по файлу, запоминая найденные N-граммы.
        /// </summary>
        /// <returns> Словарь "N-грамма -- количество упоминаний" </returns>
        /// <param name='fileName'> Имя файла</param>
        private Dictionary<string,int> ParseFile (string fileName)
        {
            Dictionary<string,int> ngrams = new Dictionary<string, int>();
            Queue<char> charQueue = new Queue<char>(N);

            using (StreamReader reader = new StreamReader(fileName,Encoding.UTF8)) {
                while (!reader.EndOfStream && reader.BaseStream.CanRead) {
                    FillQueue(charQueue, reader);
                    ngrams.AddOrUpdate(new string(charQueue.ToArray()),
                                                           1,
                                                           (key,val) => val + 1);
                    charQueue.Dequeue();
                                        
                }
            }

            return ngrams;
        }
                
        private void FillQueue (Queue<char> queue, StreamReader reader)
        {
            int readResult;
            while (queue.Count < N && (readResult = reader.Read()) != -1) {
                char c = (char)readResult;
                if (Regex.IsMatch(c.ToString(), MatchPattern)) {
                    queue.Enqueue(c);//Char.ToUpper(c));
                }
            }
        }
         
    }
}

