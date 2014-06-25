using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NGrams.Profiles;
using NGrams.Extensions;
using NGrams.DistanceCalculation;
using System.Diagnostics;

namespace NGrams
{
    class MainClass
    {
        private const int DefaultNgramLength = 3;

        public static void Main(string[] args){
            Console.WriteLine("Определение по длине слов.");
            WordLengthMain(args);

            Console.WriteLine("Определение по Nграмам.");
            NgramMain(args);
        }

        public static void WordLengthMain(string[] args){
            var profiles = new Dictionary<string,WordLengthProfile>();
            WordLengthProfile unknownText=null;

            foreach (string arg in args) {
                if (arg.StartsWith("--target=")){
                    string fileName=arg.Substring(9);
                    unknownText=new WordLengthProfile("unknown");
                    unknownText.AddFile(fileName);
                }
                else {
                    string fileName = extendHome(arg);
                    Debug.WriteLine(fileName);

                    var newProfile=new WordLengthProfile(fileName);
                    newProfile.AddFile(fileName);
                    profiles.Add(fileName, newProfile);
                }
            }
            if (unknownText == null){
                Console.WriteLine("Укажите известный текст параметром --target=<имя>");
                return;
            }

            WordLengthProfile normalProfile = new WordLengthProfile("normal");
            foreach(var profile in profiles){
                normalProfile.AddFile(profile.Key);
            }

            Console.WriteLine("Нормированные расстояния");
            var others = profiles.Select(x=> x.Value).ToArray();
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

        public static void NgramMain (string[] args)
        {
            int ngramLength = DefaultNgramLength;

            var profiles = new Dictionary<string,NgramProfile>();
            NgramProfile unknownText=null;

            foreach (string arg in args) {
                if (arg.StartsWith("--n=")) {
                    if (!Int32.TryParse(arg.Substring(4), out ngramLength)) {
                        Console.WriteLine("Параметр n должен быть целым числом.");
                        ngramLength = DefaultNgramLength;
                    }
                }
                else if (arg.StartsWith("--target=")){
                    string fileName=arg.Substring(9);
                    unknownText=new NgramProfile(ngramLength, "unknown");
                    unknownText.AddFile(fileName);
                }
                else {
                    string fileName = extendHome(arg);
                    Debug.WriteLine(fileName);

                    NgramProfile newProfile=new NgramProfile(ngramLength, fileName);
                    newProfile.AddFile(fileName);
                    profiles.Add(fileName, newProfile);
                }
            }
            if (unknownText == null){
                Console.WriteLine("Укажите известный текст параметром --target=<имя>");
                return;
            }

            NgramProfile normalProfile = new NgramProfile(ngramLength, "normal");
            foreach(var profile in profiles){
                normalProfile.AddFile(profile.Key);
            }

            Console.WriteLine("Нормированные расстояния");
            var others = profiles.Select(x=> x.Value).ToArray();
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
