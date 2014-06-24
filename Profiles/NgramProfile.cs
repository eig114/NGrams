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
    ///       Профиль автора, учитывающий количество символьных сочетаний в качестве критерия
    /// </summary>
    public class NgramProfile:ProfileBase<string>
    {
        // по умолчанию принимаем все буквенные символы Юникода
        private const string DefaultMatchPattern = @"^\p{L}+$";
        //private const string DefaultMatchPattern = @"^\p{L}|\s+$";
        private const int DefaultNGramLength = 3;

        public NgramProfile (int ngramLength, string matchPattern, string authorName)
			:base(authorName)
        {
            MatchPattern = matchPattern;
            N = ngramLength;
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
        ///      Получает кол-во символов в каждой N-грамме
        /// </summary>
        public int N { get; private set; }

        /// <summary>
        ///     Получает паттерн для фильтрации текста
        /// </summary>
        public string MatchPattern { get; private set; }

        /// <summary>
        ///      Проходит по файлу, запоминая найденные N-граммы.
        /// </summary>
        /// <returns> Словарь "N-грамма -- количество упоминаний" </returns>
        /// <param name='fileName'> Имя файла</param>
        protected override IDictionary<string,int> ParseFile (string fileName)
        {
            var ngrams = new Dictionary<string, int>();
            var charQueue = new Queue<char>(N);

            using (StreamReader reader = new StreamReader(fileName,Encoding.UTF8)) {
                while (!reader.EndOfStream && reader.BaseStream.CanRead) {
	                FillQueue(charQueue, reader);
	                ngrams.AddOrUpdate(new string(charQueue.ToArray()), 1, (key,val) => val + 1);
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
                    queue.Enqueue(c);
                }
            }
        }
    }
}
