using System;

namespace NGrams.Profiles
{
	using System.Reflection;

	public static class ProfileFactory
	{
		/// <summary>
		///		Получает новый профиль <typeparamref name="TProfile"/> по имени автора.
		/// </summary>
		/// <typeparam name="TProfile">Тип профиля</typeparam>
		/// <typeparam name="TCriteria">Тип критерия профиля</typeparam>
		/// <param name="author">Имя автора</param>
		/// <returns>Новый экземпляр <typeparamref name="TProfile"/></returns>
		public static TProfile GetProfile<TProfile, TCriteria>(string author) where TProfile : IProfile<TCriteria>
		{
			// потом надо чем-то заменить, потому что ЭТО нехорошо.
			var constructor = typeof(TProfile).GetConstructor(new[] { typeof(string) });
			if (constructor == default(ConstructorInfo))
			{
				throw new InvalidOperationException("Профиль не содержит конструктор от имени автора");
			}

			return (TProfile)constructor.Invoke(new object[] { author });
		}
	}
}
