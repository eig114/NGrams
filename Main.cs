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
				Console.WriteLine("Укажите известный текст параметром --target=<имя>");
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

	    private static void ProfileTest<TProfile, TCriteria>(string[] args, string targetFileName) where TProfile : class,IProfile<TCriteria>
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

			Console.WriteLine("Нормированные расстояния");
			var others = profiles.Select(x => x.Value).ToArray();
			var d1 = unknownText.GetDistancesWithNormal(others, normalProfile).OrderByDescending(x => x.Value);
			foreach (var distance in d1)
			{
				Console.WriteLine("{0} - {1}", distance.Key.AuthorName, distance.Value);
			}

			Console.WriteLine("Ненормированные расстояния");
			var d2 = unknownText.GetDistancesWithoutNormal(others).OrderByDescending(x => x.Value);
			foreach (var distance in d2)
			{
				Console.WriteLine("{0} - {1}", distance.Key.AuthorName, distance.Value);
			}

			Console.WriteLine("Простые суммы");
			var d3 = unknownText.GetSimpleDistances(others).OrderByDescending(x => x.Value);
			foreach (var distance in d3)
			{
				Console.WriteLine("{0} - {1}", distance.Key.AuthorName, distance.Value);
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
