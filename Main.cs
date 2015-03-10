using System;
using System.Linq;
using System.Collections.Generic;
using NGrams.Profiles;
using NGrams.DistanceCalculation;
using System.Diagnostics;

namespace NGrams
{
  /// <summary>
  ///     Синтаксис Ngrams [--option=<csv> ...] --target=<fileName> files ...
  ///     Опции:
  ///         --criteries - критерии(Ngram, WordLength),
  ///         --distances - ф-ии расстояния(E,M,KS,NN,WH)
  /// </summary>
  class MainClass
  {
    public static void Main(string[] args){
      string targetName;
      if (GetTargetName(args, out targetName))
        {
          var distances = GetDistances(args);

          if (!distances.Any()){
            distances = new HashSet<string>{"E"}; // по умолчанию - Евклидово расстояние
          }

          var criteries = GetCriteries(args);

          if (!criteries.Any()){
            criteries = new HashSet<string>{"NGram"}; // по умолчанию - критерий Nграм
          }

          foreach(var criteria in criteries)
            {
              switch(criteria)
                {
                case "NGram":
                  Console.WriteLine("Определение по Nграмам.");
                  ProfileTest<NgramProfile, string>(args, distances, targetName);
                  break;
                case "WordLength":
                  Console.WriteLine("Определение по длине слов.");
                  ProfileTest<WordLengthProfile, int>(args, distances, targetName);
                  break;

                }
            }
        }
      else
        {
          Console.WriteLine("Укажите неизвестный текст параметром --target=<имя>");
        }
    }

    private static bool GetTargetName(string[] consoleArgs, out string target)
    {
      string paramName="--target=";
      target = consoleArgs.FirstOrDefault(x => x.StartsWith(paramName));
      if (target == default(string))
        {   
          return false;
        }

      target = target.Substring(paramName.Length);
      return true;
    }

    private static ISet<string> GetDistances(string[] consoleArgs)
    {
      string paramName="--distances=";

      var result = new HashSet<string>();

      string distanceString = consoleArgs.FirstOrDefault(x=> x.StartsWith(paramName));
      if (distanceString!=default(string))
        {
          var distances = distanceString.Substring(paramName.Length).Split(',');

          foreach(var d in distances)
            {
              result.Add(d);
            }
        }

      return result;
    }

    private static ISet<string> GetCriteries(string[] consoleArgs)
    {
      string paramName="--criteries=";

      var result = new HashSet<string>();

      string distanceString = consoleArgs.FirstOrDefault(x=> x.StartsWith(paramName));
      if (distanceString!=default(string))
        {
          var distances = distanceString.Substring(paramName.Length).Split(',');

          foreach(var d in distances)
            {
              result.Add(d);
            }
        }

      return result;
    }

    private static void ProfileTest<TProfile, TCriteria>(string[] args, ISet<string> distances, string targetFileName) where TProfile : class, IProfile<TCriteria>
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

                                                                                                                                          foreach(var distance in distances)
                                                                                                                                            {
                                                                                                                                              switch(distance){
                                                                                                                                              case "E":
                                                                                                                                                Print<TCriteria,EuclideanDistance>("Евклид",unknownText,others);
                                                                                                                                                break;
                                                                                                                                              case "M":
                                                                                                                                                Print<TCriteria,MatusitaDistance>("Матусита",unknownText,others);
                                                                                                                                                break;
                                                                                                                                              case "KS":
                                                                                                                                                Print<TCriteria,KolmogorovSmirnovDistance>("Колмогоров-Смирнов",unknownText,others);
                                                                                                                                                break;
                                                                                                                                              case "WH":
                                                                                                                                                Print<TCriteria,WaveHedgesDistance>("Wavehedges",unknownText,others);
                                                                                                                                                break;
                                                                                                                                              case "NN":
                                                                                                                                                Print<TCriteria,NonNormalizedDistance>("Ненормализ.",unknownText,others);
                                                                                                                                                break;
                                                                                                                                              }
                                                                                                                                            }

    }

    private static void Print<TCriteria, TDistance>(string distanceName, IProfile<TCriteria> unknown, IEnumerable<IProfile<TCriteria>> others)
      where  TDistance:IDistance,new()
        {
          Console.WriteLine(
                            "прошло {0} секунд",
                            MeasureAction(() => PrintDistances(distanceName, DistanceCalculator.GetRelativeDistances<TCriteria, TDistance>, unknown, others)));
        }

    private static TimeSpan MeasureAction(Action action)
    {
      Stopwatch watch = new Stopwatch();
      watch.Start();
      action();
      watch.Stop();

      return watch.Elapsed;
    }

    private static void PrintDistances<TCriteria>(
                                                  string distanceName,
                                                  Func<IProfile<TCriteria>, IEnumerable<IProfile<TCriteria>>, IDictionary<IProfile<TCriteria>, double>> distanceFunc,
                                                  IProfile<TCriteria> p1,
                                                  IEnumerable<IProfile<TCriteria>> p2)
    {
      Console.WriteLine(distanceName);
      var d = distanceFunc(p1,p2).OrderByDescending(x => x.Value);
      double prev = d.First().Value;
      foreach (var distance in d)
        {
          Console.WriteLine("{0} - {1}, delta={2}", distance.Key.AuthorName, distance.Value, distance.Value - prev);
        }

      Console.WriteLine();
    }

    private static void PrintDistances<TCriteria>(
                                                  string distanceName,
                                                  Func<IProfile<TCriteria>, IEnumerable<IProfile<TCriteria>>, IProfile<TCriteria>, IDictionary<IProfile<TCriteria>, double>> distanceFunc,
                                                  IProfile<TCriteria> p1,
                                                  IEnumerable<IProfile<TCriteria>> p2,
                                                  IProfile<TCriteria> normal )
    {
      Console.WriteLine(distanceName);
      var d = distanceFunc(p1, p2,normal).OrderByDescending(x => x.Value);
      double prev = d.First().Value;
      foreach (var distance in d)
        {
          Console.WriteLine("{0} - {1}, delta={2}", distance.Key.AuthorName, distance.Value, distance.Value - prev);
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
