using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CN2.Core.DataStructures
{
	/// <summary>
	/// Представляет атрибут.
	/// </summary>
	public class AttributeValue: IExpressionMember
	{
		private AttributeType _type;
		private string _value;

		public AttributeType Type { get { return _type; } }
		public string Value { get { return _value; } }

		public AttributeValue(AttributeType type, string value)
		{
			if (type.HasValue(value))
			{
				_type = type;
				_value = value;
			}
			else
			{
				throw new ArgumentException();
			}
		}

		//todo переделать с проверкой типа значения у attribuyteType и без tryPharse
		//public bool GetBoolValue()
		//{
		//	bool boolRet;
		//	if (Boolean.TryParse(_value, out boolRet))
		//	{
		//		return boolRet;
		//	}

		//	throw new Exception();
		//}


		//public int GetIntValue()
		//{
		//	int intRet;
		//	if (Int32.TryParse(_value, out intRet))
		//	{
		//		return intRet;
		//	}

		//	throw new Exception();
		//}

		//public double GetDoubleValue()
		//{
		//	double floatRet;
		//	if (Double.TryParse(_value, out floatRet))
		//	{
		//		return floatRet;
		//	}

		//	throw new Exception();
		//}

		#region перегрузка object

		public override bool Equals(object obj)
		{
			AttributeValue otc = obj as AttributeValue;
			return otc != null && (_type.Equals(otc.Type) && _value.Equals(otc.Value));
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return "type=" + _type.Name + "; value=" + _value;
			//return "Attribute type = [" + _type + "] Value = " + _value;
		}

		#endregion перегрузка object

		#region реализация интерфейса IExpressionMember

		public string GetValueString()
		{
			return "[" + _type.Name + " = " + _value + "]";

			//return _type.Type == Types.Boolean || _type.Type == Types.Integer || _type.Type == Types.Float
			//	? "[" + _type.Name + "]" + _value
			//	: _value;

			//return _type.Type == Types.Boolean
			//	? Convert.ToBoolean(_value) ? _type.Name : "!" + _type.Name
			//	: _type.Type == Types.Integer || _type.Type == Types.Float ? "[" + _type.Name + "]" + _value : _value;
		}

		public bool IsCover(LearnableExample example)
		{
			foreach (var attribute in example.PredictiveAttributes)
			{
				if (_type.Equals(attribute.Type))
				{
					return _value.Equals(attribute.Value);
				}
			}
			return false;
		}

	    //public bool Equals(IExpressionMember expressionMember)
	    //{
	    //    var otherAttributeValue = expressionMember as AttributeValue;
	    //    return otherAttributeValue == null || !_type.Equals(otherAttributeValue.Type) ||
	    //           !_value.Equals(otherAttributeValue.Value);
	    //}

		#endregion реализация интерфейса IExpressionMember
	}
}
