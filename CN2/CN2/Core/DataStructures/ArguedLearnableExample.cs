using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CN2.Core.DataStructures
{
	/// <summary>
	/// Представляет аргументированный обучающий пример
	/// </summary>
	[DebuggerDisplay("{ToString()")]
	public class ArguedLearnableExample: LearnableExample
	{
		/// <summary>
		/// Выражение "потому что" объекта аргументации примера.
		/// </summary>
		private Expression _becauseExpression;
		/// <summary>
		/// Выражение "несмотря на" объекта аргументации примера.
		/// </summary>
		private Expression _despiteExpression;

		/// <summary>
		/// Возвращает выражение "потому что" объекта аргументации примера.
		/// </summary>
		public Expression BecauseExpression { get { return _becauseExpression; } }
		/// <summary>
		/// Возвращает выражение "несмотря на" объекта аргументации примера.
		/// </summary>
		public Expression DespiteExpression { get { return _despiteExpression; } }
		/// <summary>
		/// Возвращает признак того, что объект аргументации примера имеет часть "несмотря на".
		/// </summary>
		public bool HasDespitePart { get { return _despiteExpression != null; } }

		public ArguedLearnableExample(IEnumerable<AttributeValue> attributes, AttributeValue decisiveAttribute,
            Expression becauseExpression, Expression despiteExpression) : base(attributes, decisiveAttribute)
		{
			_becauseExpression = becauseExpression;
			_despiteExpression = despiteExpression;
		}

		public ArguedLearnableExample(ArguedLearnableExample sample) : base(sample)
		{
			_becauseExpression = sample.BecauseExpression;
			_despiteExpression = sample.DespiteExpression;
		}

		public override bool Equals(object obj)
		{
			ArguedLearnableExample otc = obj as ArguedLearnableExample;
			return otc != null &&
			       (base.Equals(otc) && _becauseExpression.Equals(otc.BecauseExpression) &&
			        (HasDespitePart && otc.HasDespitePart && _despiteExpression.Equals(otc.DespiteExpression)));
		}

		public override string ToString()
		{
			return base.ToString() + "; потому что: " + _becauseExpression + (HasDespitePart ? string.Empty : "; несмотря на: " + _despiteExpression);
		}
	}
}
