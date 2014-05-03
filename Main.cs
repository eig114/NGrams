using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NGrams;
using NGrams.Extensions;

namespace NGrams
{
    class MainClass
    {
        private const int DefaultNgramLength = 3;
        private const int DefaultTopN = 3;
                
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// Параметры командной строки. Запускается так: "ngrams [имя файла или параметр]*"
        /// параметры: --n=VALUE - длина н-граммы, --top=VALUE кол-во отображаемых н-грам на файл
        /// </param>
        public static void Main (string[] args)
        {
            int ngramLength = DefaultNgramLength;
            int topN = DefaultTopN;
                        
            List<Profile> profiles = new List<Profile>();

            foreach (string arg in args) {
                if (arg.StartsWith("--n=")) {
                    if (!Int32.TryParse(arg.Substring(4), out ngramLength)) {
                        Console.WriteLine("Параметр n должен быть целым числом.");
                        ngramLength = DefaultNgramLength;
                    }
                }
                else if (arg.StartsWith("--top=")) {
                    if (!Int32.TryParse(arg.Substring(6), out topN)) {
                        Console.WriteLine("Количество отображаемых n-грам должно быть целым числом.");
                        topN = DefaultTopN;
                    }
                }
                else {
                    string fileName = extendHome(arg);
                    Console.WriteLine(fileName);

                    Profile newProfile=new Profile(ngramLength);
                    newProfile.AddFile(fileName);
                    profiles.Add(newProfile);
                }
            }

            foreach(var profile in profiles){
                var topOccurencies = profile.NGramRawOccurencies.OrderByDescending(x=> x.Value).Take(topN);

                foreach (var ngram in topOccurencies){
                    Console.WriteLine(ngram);
                }
            }
                        

        }
                               
        /// <summary>
        ///     Разворачивает символ '~' в путь к домашней директории на UNIX'ах
        /// </summary>
        /// <returns>
        /// The home.
        /// </returns>
        /// <param name='input'>
        /// Input.
        /// </param>
        private static string extendHome (string input)
        {
            OperatingSystem os = Environment.OSVersion;

            if (os.Platform == PlatformID.Unix) {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(@"I smell a penguin! Or is it an imp?...");
#endif
                return input.Replace("~", System.Environment.GetEnvironmentVariable("$HOME"));
            }
            return input;
        }
    }
}
