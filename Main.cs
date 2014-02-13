using System;
using System.Text;
using System.IO;
using NGrams;

namespace NGrams
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.Write("FileName:");
			
			string fileName = Console.ReadLine();
			
			using(StreamReader reader = File.OpenText(fileName))
			{
				Console.WriteLine(reader.ReadToEnd());
			}
			
			NgramGetter ngrams = new NgramGetter(fileName);
			
			var ngramCollection = ngrams.NGrams;
			
			foreach(var ngram in ngramCollection){
				Console.WriteLine(String.Format("{0}	{1} occurencies.", 
				                                ngram.Key,
				                                ngram.Value));
			}
		}
	}
}
