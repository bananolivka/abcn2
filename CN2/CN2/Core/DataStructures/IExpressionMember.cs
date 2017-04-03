namespace CN2.Core.DataStructures
{
	public interface IExpressionMember
	{
		/// <summary>
		/// Возвращает строковое представление члена выражения.
		/// </summary>
		/// <returns></returns>
		string GetValueString();

		/// <summary>
		/// Проверяет, покрывает ли член выражения пример.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		bool IsCover(LearnableExample example);
	}
}
