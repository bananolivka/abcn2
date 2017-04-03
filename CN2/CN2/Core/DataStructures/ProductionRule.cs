using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CN2.Core.DataStructures
{
	/// <summary>
	/// Представляет продукционное правило.
	/// </summary>
	[DebuggerDisplay("{ToString()}")]
	public class ProductionRule
	{
		private IExpressionMember _condition;
		private AttributeValue _result;

		public IExpressionMember Condition { get { return _condition; } }
		public AttributeValue Result { get { return _result; } }
        public bool IsDefault { get; set; }

		public ProductionRule(IExpressionMember condition, AttributeValue result)
		{
            _condition = condition;
			_result = result;
		    IsDefault = false;
		}

		public override bool Equals(object obj)
		{
			ProductionRule otc = obj as ProductionRule;
			return otc != null && (_condition.Equals(otc.Condition) && _result.Equals(otc.Result));
		}

		public override string ToString()
		{
			return "Если " + _condition.GetValueString() + ", то " + _result.GetValueString();
		}

	    public string GetStringValue()
	    {
	        return (IsDefault ? "Правило по умолчанию: " : ("Если: " + _condition.GetValueString() + ", то: ")) + _result.GetValueString();
	    }
	}
}
