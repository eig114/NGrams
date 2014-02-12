using System;
using System.IO;

namespace NGrams
{
	
	/// <summary>
	/// 	Ngram getter.
	/// </summary>
	public class NgramGetter
	{
		private StreamReader _fileReader;
		
		public NgramGetter ()
		{
			_fileReader=null;
		}
				
		public void OpenFile(string fileName){
			_fileReader = File.OpenText(fileName);
		}
	}
}

