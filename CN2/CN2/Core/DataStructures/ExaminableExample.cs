using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CN2.Core.DataStructures
{
	/// <summary>
	/// 
	/// </summary>
	[DebuggerDisplay("{ToString()}")]
	public class ExaminableExample : LearnableExample
	{
		private AttributeValue _examinedAttribute;

		public AttributeValue ExaminedAttribute
		{
			get { return _examinedAttribute; }
			set { SetValue(value); }
		}

		public ExaminableExample(IEnumerable<AttributeValue> attributes, AttributeValue decisiveAttribute, AttributeValue value = null)
			: base(attributes, decisiveAttribute)
		{
			SetValue(value);
		}

		public ExaminableExample(LearnableExample example, AttributeValue value = null) : base(example)
		{
			SetValue(value);
		}

		public ExaminableExample(ExaminableExample sample) : base(sample)
		{
			SetValue(sample.ExaminedAttribute);
		}

		private void SetValue(AttributeValue value)
		{
			if (value == null)
			{
				_examinedAttribute = null;
				return;
			}

			if (_decisiveAttribute.Type.Equals(value.Type))
			{
				_examinedAttribute = value;
			}
			else
			{
				throw new ArgumentException("Тип не соответствует типу решающего атрибута.");
			}
		}

		public override bool Equals(object obj)
		{
			ExaminableExample otc = obj as ExaminableExample;
			return otc != null && base.Equals(otc) && (_examinedAttribute.Equals(otc.ExaminedAttribute));
		}

		public override string ToString()
		{
			return base.ToString() + "; результат экзамена" + (_examinedAttribute == null ? " не определён" : (": " + _examinedAttribute.Value));
		}
	}
}
