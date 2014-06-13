using System;
using System.Collections.Generic;

namespace NGrams.Profiles
{
    /// <summary>
    ///     Интерфейс профиля с произвольным критерием.
    /// </summary>
    public interface IProfile<Criteria>
    {
        /// <summary>
        /// Получает имя автора
        /// </summary>
        string AuthorName{get;}

        /// <summary>
        ///      Получает значение критерия и его количество во всех обработанных текстах.
        /// </summary>
        IDictionary<Criteria,int> RawOccurencies { get; }

        /// <summary>
        ///     Получает частоту использования значения критерия
        /// </summary>
        IDictionary<Criteria,decimal> Probability { get; }

        /// <summary>
        ///     Учитывает в профиле текст из файла <paramref name="fileName"/>
        /// </summary>
        /// <param name='fileName'> Имя файла. </param>
        void AddFile(string fileName);

    }
}

