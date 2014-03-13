using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NGrams;

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
                /// Параметры командной строки. Запускается так: "ngrams ИМЯ_ФАЙЛА [длина N-грам]"
                /// </param>
                public static void Main(string[] args)
                {
                        int ngramLength = DefaultNgramLength;
                        int topN = DefaultTopN;
                        
                        Dictionary<string,NgramGetter> ngramGetters = new Dictionary<string,NgramGetter>();
                        foreach (string arg in args) {
                                if (arg.StartsWith ("--n=")) {
                                        if (!Int32.TryParse (arg.Substring (4), out ngramLength)) {
                                                Console.WriteLine ("Параметр n должен быть целым числом.");
                                                ngramLength = DefaultNgramLength;
                                        }
                                }
                                else if (arg.StartsWith ("--top=")) {
                                                if (!Int32.TryParse (arg.Substring (6), out topN)) {
                                                        Console.WriteLine ("Количество отображаемых n-грам должно быть целым числом.");
                                                        topN = DefaultTopN;
                                                }
                                        }
                                        else {
                                                string fileName = extendHome (arg);
                                                Console.WriteLine (fileName);
                                                ngramGetters.Add (fileName, new NgramGetter (fileName, ngramLength));
                                        }
                        }
                        
                        foreach (var ngramInfo in ngramGetters) {
                                Console.Write (String.Format ("Top 3 for {0}:\n", ngramInfo.Key));
                                var top = ngramInfo.Value.NGramProbability.OrderByDescending (x => x.Value).Take (topN).ToArray ();
                                foreach (var ngram in top) {
                                        Console.WriteLine (ngram);
                                }
                        }
                        
                        /*if (args.Any ()) {
                                Console.Write (args [0]);
                                fileName = extendHome (args [0]);
                        }
                        else {
                                fileName = extendHome (Console.ReadLine ());
                        }
                        
                        if (args.Length > 1 && int.TryParse (args [1], out ngramLength)) {
                                ngrams = new NgramGetter (fileName, ngramLength);
                        }
                        else {
                                ngrams = new NgramGetter (fileName);
                        }
                 
                        var ngramCollection = ngrams.NGramProbability;
                 
                        foreach (var ngram in ngramCollection.OrderBy(x=> x.Value)) {
                                Console.WriteLine (String.Format ("{0}\t{1}", 
                                                         ngram.Key,
                                                         ngram.Value));
                        }*/
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
                private static string extendHome(string input)
                {
                        OperatingSystem os = Environment.OSVersion;
                        
                        if (os.Platform == PlatformID.Unix) {
                                System.Diagnostics.Debug.WriteLine (@"I smell a penguin! Or is it an imp?...");
                                return input.Replace ("~", System.Environment.GetEnvironmentVariable ("$HOME"));
                        }
                        return input;
                }
        }
}
