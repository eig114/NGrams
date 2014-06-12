using System;
using System.Collections.Generic;

namespace NGrams.Profiles
{
    public interface IProfile
    {
        string AuthorName{get;}

        /// <summary>
        ///      Получает N-граммы и их количество во всех обработанных текстах.
        /// </summary>
        IDictionary<string,int> RawOccurencies { get; }

        /// <summary>
        ///     Получает частоту использования N-грамм
        /// </summary>
        IDictionary<string,decimal> Probability { get; }

        /// <summary>
        ///     Учитывает в профиле текст из файла <paramref name="fileName"/>
        /// </summary>
        /// <param name='fileName'> Имя файла. </param>
        void AddFile(string fileName);

    }
}

