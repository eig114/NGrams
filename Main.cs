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
	    public static void Main(string[] args){
			string targetName;
	        if (GetTargetName(args, out targetName))
	        {
				Console.WriteLine("Определение по Nграмам.");
				ProfileTest<NgramProfile, string>(args,targetName);
				Console.WriteLine("Определение по длине слов.");
				ProfileTest<WordLengthProfile, int>(args, targetName);
	        }
	        else
	        {
				Console.WriteLine("Укажите неизвестный текст параметром --target=<имя>");
	        }
        }

	    private static bool GetTargetName(string[] consoleArgs, out string target)
	    {
			target = consoleArgs.FirstOrDefault(x => x.StartsWith("--target="));
			if (target == default(string))
		    {	
			    return false;
		    }

		    target = target.Substring(9);
		    return true;
	    }

	    private static void ProfileTest<TProfile, TCriteria>(string[] args, string targetFileName) where TProfile : class, IProfile<TCriteria>
		{
			var profiles = new Dictionary<string, TProfile>();
			TProfile unknownText = null;

			foreach (string arg in args)
			{
				if (!arg.StartsWith("--")) // игнорим параметры
				{
					string fileName = extendHome(arg);
					Debug.WriteLine(fileName);

					var newProfile = ProfileFactory.GetProfile<TProfile, TCriteria>(fileName);
					newProfile.AddFile(fileName);
					profiles.Add(fileName, newProfile);	
				}
			}

			unknownText = ProfileFactory.GetProfile<TProfile, TCriteria>("unknown");
			unknownText.AddFile(targetFileName);

			TProfile normalProfile = ProfileFactory.GetProfile<TProfile, TCriteria>("normal");
			foreach (var profile in profiles)
			{
				normalProfile.AddFile(profile.Key);
			}

			var others = profiles.Select(x => x.Value).ToArray();
			Console.WriteLine("прошло {0} секунд", MeasureAction(() => PrintDistances("Расстояние Матсуиты", MatsuitaDistance.GetMatsuitaDistances, unknownText, others)));
			Console.WriteLine("прошло {0} секунд", MeasureAction(() => PrintDistances("Ненормированное расстояние", NonNormalizedDistance.GetDistancesWithoutNormal, unknownText, others)));
			Console.WriteLine("прошло {0} секунд", MeasureAction(() => PrintDistances("Расстояние Колмогорова-Смирнова", KolmogorovSmirnovDistance.GetKolmogorovSmirnovDistances, unknownText, others)));
			Console.WriteLine("прошло {0} секунд", MeasureAction(() => PrintDistances("Нормированные расстояния", NormalizedDistance.GetDistancesWithNormal, unknownText, others, normalProfile)));
			Console.WriteLine("прошло {0} секунд", MeasureAction(() => PrintDistances("Простая сумма", SimpleDistance.GetSimpleDistances, unknownText, others)));
			//PrintDistances("Ненормированное расстояние", NonNormalizedDistance.GetDistancesWithoutNormal, unknownText, others);
			//PrintDistances("Расстояние Колмогорова-Смирнова", KolmogorovSmirnovDistance.GetKolmogorovSmirnovDistances, unknownText, others);
			//PrintDistances("Нормированные расстояния", NormalizedDistance.GetDistancesWithNormal, unknownText, others, normalProfile);
			//PrintDistances("Простая сумма", SimpleDistance.GetSimpleDistances, unknownText, others);
		}

	    private static TimeSpan MeasureAction(Action action)
	    {
		    Stopwatch watch = new Stopwatch();
			watch.Start();
		    action();
			watch.Stop();

		    return watch.Elapsed;
	    }

	    private static void PrintDistances<TCriteria, TDistanceType>(
		    string distanceName,
			Func<IProfile<TCriteria>, IEnumerable<IProfile<TCriteria>>, IDictionary<IProfile<TCriteria>, TDistanceType>> distanceFunc,
			IProfile<TCriteria> p1,
			IEnumerable<IProfile<TCriteria>> p2) 
	    {
		    Console.WriteLine(distanceName);
			var d = distanceFunc(p1,p2).OrderByDescending(x => x.Value);
			foreach (var distance in d)
			{
				Console.WriteLine("{0} - {1}", distance.Key.AuthorName, distance.Value);
			}

			Console.WriteLine();
	    }

		private static void PrintDistances<TCriteria, TDistanceType>(
			string distanceName,
			Func<IProfile<TCriteria>, IEnumerable<IProfile<TCriteria>>, IProfile<TCriteria>, IDictionary<IProfile<TCriteria>, TDistanceType>> distanceFunc,
			IProfile<TCriteria> p1,
			IEnumerable<IProfile<TCriteria>> p2,
			IProfile<TCriteria> normal )
		{
			Console.WriteLine(distanceName);
			var d = distanceFunc(p1, p2,normal).OrderByDescending(x => x.Value);
			foreach (var distance in d)
			{
				Console.WriteLine("{0} - {1}", distance.Key.AuthorName, distance.Value);
			}

			Console.WriteLine();
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
