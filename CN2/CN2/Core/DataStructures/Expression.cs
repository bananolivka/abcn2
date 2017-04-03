using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace CN2.Core.DataStructures
{
	/// <summary>
	/// Определяет типы операций выражений.
	/// </summary>
	public enum Operation
	{
		/// <summary>
		/// Коньюнкция. (conjunction)
		/// </summary>
		Con,
		/// <summary>
		/// Дизъюнкция. (disjunction)
		/// </summary>
		Dis,
		/// <summary>
		/// Отрицание.
		/// </summary>
		Neg
	};

	/// <summary>
	/// Представляет логическое выражение.
	/// </summary>
	[DebuggerDisplay("{ToString()}")]
	public class Expression: IExpressionMember
	{
		/// <summary>
		/// Тип операции между элементами выражения.
		/// </summary>
		private Operation _operation;
		/// <summary>
		/// 
		/// </summary>
		private List<IExpressionMember> _members;

		/// <summary>
		/// Возвращает тип операции между элементами выражения.
		/// </summary>
		public Operation Operation { get { return _operation; } }
		/// <summary>
		/// 
		/// </summary>
		public List<IExpressionMember> Members { get { return _members; } }

		public Expression(Operation operation)
		{
			_operation = operation;
			_members = new List<IExpressionMember>();
		}

		public Expression(Operation operation, IExpressionMember member)
		{
			_operation = operation;
			_members = new List<IExpressionMember>() {member};
		}

		public Expression(Operation operation, IEnumerable<IExpressionMember> members)
		{
			_operation = operation;
			_members = new List<IExpressionMember>(members);
		}

		/// <summary>
		/// Добавляет выражение в список выражений.
		/// </summary>
		/// <param name="member"></param>
		public void AddMember(IExpressionMember member)
		{
			if (_operation == Operation.Neg)
			{
				throw new Exception("Unable to add an member.");
			}

			_members.Add(member);
		}

		#region перегрузка object

		public override bool Equals(object obj)
		{
			Expression otc = obj as Expression;
			if (otc == null || _operation != otc.Operation || _members.Count != otc.Members.Count)
			{
				return false;
			}
			for (int i = 0; i < _members.Count; i ++)
			{
				if (!otc.Members.Contains(_members[i]))
				{
					return false;
				}
			}
			for (int i = 0; i < otc.Members.Count; i ++)
			{
				if (!_members.Contains(otc.Members[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return GetValueString();
		}

		#endregion перегрузка object

		#region реализация интерфейса IExpressionMember

		public string GetValueString()
		{
			string op = string.Empty;
			switch (_operation)
			{
				case Operation.Con:
					{
						op = " & ";
					}
					break;

				case Operation.Dis:
					{
						op = " V ";
					}
					break;

				case Operation.Neg:
					{
						if (_members.Count == 1)
						{
							return "! " + _members[0].GetValueString();
						}
						else
						{
							throw new Exception();
						}
					}
					break;

				default:
					{
						throw new NotImplementedException();
					}
					break;
			}
			if (_members.Count == 0)
			{
				return "Operator: " + op + ". No members.";
			}
			string expr = string.Empty;
			for (int i = 0; i < _members.Count; i++)
			{
				//bool isMemberExoression = (_members[i] as Expression) == null;
				if (i == 0)
				{
					expr += "(";
				}
				if (i == _members.Count - 1)
				{
					expr += _members[i].GetValueString() + ")";
				}
				else
				{
					expr += _members[i].GetValueString() + op;
				}
			}
			return expr;
		}

		public bool IsCover(LearnableExample example)
		{
			if (_members.Count == 0)
			{
				return true;
			}
			switch (_operation)
			{
				case Operation.Neg:
				{
					return !_members[0].IsCover(example);
				}
					break;

				case Operation.Con:
				{
					foreach (var member in _members)
					{
						if (!member.IsCover(example))
						{
							return false;
						}
					}
					return true;
				}
					break;

				case Operation.Dis:
				{
					foreach (var member in _members)
					{
						if (member.IsCover(example))
						{
							return true;
						}
						return false;
					}
				}
					break;
			}
			return false;
		}

	    //public bool Equals(IExpressionMember expressionMember)
	    //{
	    //    var otherExpression = expressionMember as Expression;
	    //    if (otherExpression == null || _operation != otherExpression.Operation || _members.Count != otherExpression.Members.Count)
	    //    {
	    //        return false;
	    //    }

	    //    foreach (var member in _members)
	    //    {
	    //        if (!otherExpression.Members.Contains(member))
	    //        {
	    //            return false;
	    //        }
	    //    }
	    //    foreach (var member in otherExpression.Members)
	    //    {
	    //        if (!_members.Contains(member))
	    //        {
	    //            return false;
	    //        }
	    //    }

	    //    return true;
	    //}

		#endregion реализация интерфейса IExpressionMember
	}
}
