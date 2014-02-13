using System;
using System.Text;
using System.IO;
using System.Linq;
using NGrams;

namespace NGrams
{
        class MainClass
        {
                /// <summary>
                /// The entry point of the program, where the program control starts and ends.
                /// </summary>
                /// <param name='args'>
                /// Параметры командной строки. Запускается так: "ngrams ИМЯ_ФАЙЛА [длина N-грам]"
                /// </param>
                public static void Main (string[] args)
                {
                        string fileName;
                        int ngramLength;
                        
                        NgramGetter ngrams;
                        
                        if (args.Any()) {
                                Console.Write(args[0]);
                                fileName = extendHome(args[0]);
                        }
                        else
                                fileName = extendHome(Console.ReadLine());
                        
                        if (args.Length > 1 && int.TryParse(args[1], out ngramLength)) {
                                ngrams = new NgramGetter(fileName, ngramLength);
                        }
                        else
                                ngrams = new NgramGetter(fileName);
                 
                        var ngramCollection = ngrams.NGramProbability;
                 
                        foreach (var ngram in ngramCollection.OrderBy(x=> x.Value)) {
                                Console.WriteLine(String.Format("{0}\t{1}", 
                                                         ngram.Key,
                                                         ngram.Value));
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
                                System.Diagnostics.Debug.WriteLine(@"I smell a penguin! Or is it an imp?...");
                                return input.Replace("~", System.Environment.GetEnvironmentVariable("$HOME"));
                        }
                        return input;
                }
        }
}
