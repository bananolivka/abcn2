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
	/// Представляет пример.
	/// </summary>
	[DebuggerDisplay("{ToString()}")]
	public class LearnableExample
	{
		/// <summary>
		/// Список предсказывающих атрибутов примера.
		/// </summary>
		protected List<AttributeValue> _predictiveAttributes;
		/// <summary>
		/// Решающий атрибут примера.
		/// </summary>
		protected AttributeValue _decisiveAttribute;

		/// <summary>
		/// Возвращает список предсказывающих атрибутов примера.
		/// </summary>
		public List<AttributeValue> PredictiveAttributes { get { return _predictiveAttributes; } /*set { _predictiveAttributes = value; }*/ }
		/// <summary>
		/// Возвращает решающий атрибут примера.
		/// </summary>
		public AttributeValue DecisiveAttribute { get { return _decisiveAttribute; } /*set { _decisiveAttribute = value; }*/ }

		/// <summary>
		/// Возвращает список всех атрибутов примера.
		/// </summary>
		public List<AttributeValue> AllAttributes { get { return new List<AttributeValue>(_predictiveAttributes) {_decisiveAttribute}; } } 

		public LearnableExample(IEnumerable<AttributeValue> attributes, AttributeValue decisiveAttribute)
		{
			_predictiveAttributes = new List<AttributeValue>(attributes);
			_decisiveAttribute = decisiveAttribute;
		}

		public LearnableExample(LearnableExample sample)
		{
			_predictiveAttributes = new List<AttributeValue>(sample.PredictiveAttributes);
			_decisiveAttribute = sample.DecisiveAttribute;
		}

		public override bool Equals(object obj)
		{
			LearnableExample otc = obj as LearnableExample;
			return otc != null && (_predictiveAttributes.Equals(otc.PredictiveAttributes) && _decisiveAttribute.Equals(otc.DecisiveAttribute));
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			string attributes = string.Empty;
			for (int i = 0; i < _predictiveAttributes.Count; i ++)
			{
				if (i == 0)
				{
					attributes += "(";
				}
				if (i == _predictiveAttributes.Count - 1)
				{
					attributes += _predictiveAttributes[i] + ")";
				}
				else
				{
					attributes += _predictiveAttributes[i] + "; ";
				}
			}
			return "Attributes = " + attributes + " Result = " + _decisiveAttribute;
		}

	    public string GetString()
	    {
	        string example = "(";
	        for (int i = 0; i < _predictiveAttributes.Count; i++)
	        {
	            example += "[" + _predictiveAttributes[i].Type.Name + " = " + _predictiveAttributes[i].Value + "]; ";
	        }
	        example += " решающий атрибут: [" + _decisiveAttribute.Type.Name + " = " + _decisiveAttribute.Value + "])";
	        return example;
	    }
	}
}
