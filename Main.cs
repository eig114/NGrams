using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NGrams;
using NGrams.Extensions;
using System.Diagnostics;

namespace NGrams
{
    class MainClass
    {
        private const int DefaultNgramLength = 3;
        private const int DefaultTopN = 10;

//        public static void Main (string[] args){
//            //ngrams --target=1 ./2 ./3
//            Main1(new[]{"--target=1", "2", "3", "4"});
//        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// Параметры командной строки. Запускается так: "ngrams [имя файла или параметр]*"
        /// параметры: --n=VALUE - длина н-граммы, --top=VALUE кол-во отображаемых н-грам на файл
        /// --target=TARGETNAME - имя файла с неизвестным текстом
        /// </param>
        public static void Main (string[] args)
        {
            int ngramLength = DefaultNgramLength;
            int topN = DefaultTopN;
                        
            var profiles = new Dictionary<string,Profile>();
            Profile unknownText=null;

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
                else if (arg.StartsWith("--target=")){
                    string fileName=arg.Substring(9);
                    unknownText=new Profile(ngramLength, "unknown");
                    unknownText.AddFile(fileName);
                }
                else {
                    string fileName = extendHome(arg);
                    Debug.WriteLine(fileName);

                    Profile newProfile=new Profile(ngramLength, fileName);
                    newProfile.AddFile(fileName);
                    profiles.Add(fileName, newProfile);
                }
            }
            if (unknownText == null){
                Console.WriteLine("Укажите известный текст параметром --target=<имя>");
                return;
            }

            Profile normalProfile = new Profile(ngramLength, "normal");
            foreach(var profile in profiles){
                normalProfile.AddFile(profile.Key);
            }

//            foreach(var x in profiles){
//                PrintTop(topN, x.Value);
//            }
//            PrintTop(topN, unknownText);

            Console.WriteLine("Нормированные расстояния");
            var others = profiles.Select(x=> x.Value);
            var d1 = unknownText.GetDistancesWithNormal(others,normalProfile).OrderByDescending(x=> x.Value);
            foreach(var distance in d1){
                Console.WriteLine(String.Format("{0} - {1}", distance.Key.AuthorName, distance.Value));
            }

            Console.WriteLine("Ненормированные расстояния");
            var d2 = unknownText.GetDistancesWithoutNormal(others).OrderByDescending(x=> x.Value);
            foreach(var distance in d2){
                Console.WriteLine(String.Format("{0} - {1}", distance.Key.AuthorName, distance.Value));
            }

            Console.WriteLine("Простые суммы");
            var d3 = unknownText.GetSimpleDistances(others).OrderByDescending(x=> x.Value);
            foreach(var distance in d3){
                Console.WriteLine(String.Format("{0} - {1}", distance.Key.AuthorName, distance.Value));
            }
        }

        private static void PrintTop(int topN, Profile profile){
            Console.WriteLine(profile.AuthorName);
            foreach(var x in profile.NGramProbability.Take(topN)){
                Console.WriteLine(x);
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
                Debug.WriteLine(@"I smell a penguin! Or is it an imp?...");
                return input.Replace("~", System.Environment.GetEnvironmentVariable("$HOME"));
            }
            return input;
        }
    }
}
