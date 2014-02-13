using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGrams.Extensions;

namespace NGrams
{
	
	/// <summary>
	/// 	Ngram getter.
	/// </summary>
	public class NgramGetter
	{
		private const string DefaultCharsToStrip = "\n\t";
		private const int DefaultNGramLength = 3;
		
		private string _charsToStrip;
		
		private string _fileName;
		
		private int _ngramLength;
		
		public NgramGetter (string fileName, int ngramLength, string charsToStrip)
		{
			_charsToStrip = charsToStrip;
			_ngramLength = ngramLength;
			_fileName = fileName;
			ParseFile(fileName,ngramLength);
		}
		
		public NgramGetter (string fileName, int ngramLength)
			:this(fileName,ngramLength,DefaultCharsToStrip)
		{
		}
		
		public NgramGetter (string fileName)
			:this(fileName,DefaultNGramLength,DefaultCharsToStrip)
		{
		}
		
		/// <summary>
		/// 	Имя файла, с которым ассоциирован получатель N-грамм
		/// </summary>
		public string FileName 
		{
			get {return _fileName;}
			private set
			{
				_fileName = value;
				ParseFile(_fileName,N);
			}
		}
		
		/// <summary>
		/// 	Получает кол-во символов в каждой N-грамме
		/// </summary>
		public int N 
		{
			get {return _ngramLength;} 
			private set
			{
				_ngramLength = value;
				ParseFile(_fileName,_ngramLength);
			}
		}
		
		public string CharsToStrip
		{
			get{return _charsToStrip;}
			private set
			{
				_charsToStrip = value;
				ParseFile(_fileName,_ngramLength);
			}
		}
		
		/// <summary>
		/// 	Получает N-граммы, выделенные при последней обработке файла,
		/// 	и их количество в тексте.
		/// </summary>
		public IDictionary<string,int> NGrams {get; private set;}
		
		/// <summary>
		/// 	Проходит по файлу, запоминая найденные N-граммы.
		/// </summary>
		/// <param name='ngramLength'>
		/// Ngram Length
		/// </param>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		private void ParseFile(string fileName, int ngramLength){
			Dictionary<string,int> ngrams = new Dictionary<string, int>();
			
			Decoder UTF8decoder = Encoding.UTF8.GetDecoder();
			
			using(StreamReader reader = File.OpenText(fileName))
			{
				byte []fileBytes = new byte[reader.BaseStream.Length];
				while (!reader.EndOfStream && reader.BaseStream.CanRead)
					ngrams.AddOrUpdate(GetNextNGram(reader, ngramLength),
					                   1,
					                   (key,val)=>val+1);
			}
			
			NGrams = ngrams;
		}
		
		/// <summary>
		/// 	Получает следующую N-грамму из потока, 
		/// 	читаемого <paramref name='reader'/>
		/// </summary>
		/// <returns>
		/// 	Следующая N-грамму из потока. Если файл закончился раньше, 
		/// 	чем N было достигнуто, возвращается неполная N-грамма.
		/// </returns>
		/// <param name='reader'>
		/// 	Reader.
		/// </param>
		/// <param name='ngramSize'>
		/// 	Ngram size.
		/// </param>
		/// <param name='decoder'>
		/// 	Decoder
		/// </param>
		private string GetNextNGram(StreamReader reader, int ngramSize, Decoder decoder){
			StringBuilder tmpBuilder = new StringBuilder();
			int readResult;
			while ((readResult = reader.BaseStream()) != -1 &&
			       tmpBuilder.Length < ngramSize)
			{
				char c = (char) readResult;
				if (_charsToStrip.Contains(c))
				{
					tmpBuilder.Append(c);
				}
			}
			
			return tmpBuilder.ToString();
		}
		
		
	}
}

