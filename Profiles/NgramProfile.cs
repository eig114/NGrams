using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGrams.Extensions;
using System.Text.RegularExpressions;

namespace NGrams.Profiles
{
 
    /// <summary>
    ///       Профиль автора.
    /// </summary>
    public class NgramProfile:IProfile
    {
        // по умолчанию принимаем все буквенные символы Юникода
        private const string DefaultMatchPattern = @"^\p{L}+$";
        //private const string DefaultMatchPattern = @"^\p{L}|\s+$";
        private const int DefaultNGramLength = 3;
        private Guid id;

        private List<string> _fileNames;

        public NgramProfile (int ngramLength, string matchPattern, string authorName)
        {
            MatchPattern = matchPattern;
            N = ngramLength;
            AuthorName=authorName;

            _fileNames = new List<string>();
            RawOccurencies = new Dictionary<string,int>();
            Probability = new Dictionary<string,decimal>();

            id = Guid.NewGuid();
        }

        public NgramProfile (int ngramLength, string authorName)
                 :this(ngramLength,DefaultMatchPattern,authorName)
        {
        }

        public NgramProfile (string matchPattern, string authorName)
                 :this(DefaultNGramLength,matchPattern,authorName)
        {
        }

        public NgramProfile (string authorName)
                 :this(DefaultNGramLength,DefaultMatchPattern,authorName)
        {
        }

        /// <summary>
        ///      Получает имена файлов, обработанных для получения N-грамм
        /// </summary>
        public IEnumerable<string> FileNames {
            get { return _fileNames;}
        }

        public string AuthorName{get; private set;}

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
        public IDictionary<string,int> RawOccurencies { get; private set; }

        /// <summary>
        ///     Получает частоту использования N-грамм
        /// </summary>
        public IDictionary<string,decimal> Probability { get; private set; }

        /// <summary>
        ///     Учитывает в профиле текст из файла <paramref name="fileName"/>
        /// </summary>
        /// <param name='fileName'> Имя файла. </param>
        public void AddFile(string fileName){
            Dictionary<string,int> ngrams = ParseFile(fileName);

            foreach(var ngram in ngrams){
                RawOccurencies.AddOrUpdate(
                    ngram.Key,
                    ngram.Value,
                    (key,oldValue)=> oldValue+ngram.Value );
            }

            RecountProbabilities();
        }

        /// <summary>
        ///     Пересчитывает частоты использования N-грам в профиле.
        /// </summary>
        private void RecountProbabilities(){
            int totalChars = RawOccurencies.Sum(x => x.Value);
            Probability = RawOccurencies
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

        public override bool Equals (object obj)
        {
            NgramProfile other = obj as NgramProfile;

            if (other != null){
                return other.id.Equals(this.id);
            }
            return false;
        }

        public override int GetHashCode ()
        {
            return id.GetHashCode ();
        }
    }
}
