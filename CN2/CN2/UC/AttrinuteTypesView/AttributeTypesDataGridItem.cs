using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Xml;
using CN2.Core.DataStructures;
using System.Xml.Serialization;

namespace CN2.UC.AttrinuteTypesView
{
	public class AttributeTypesDataGridItem
	{
		private const char Separator = ';';

		public bool IsUse { get; set; }

		public string Name { get; set; }

		public Types Type { get; set; }

		public string Values { get; set; }

		public AttributeTypesDataGridItem()
		{
			IsUse = true;
		}

		public AttributeTypesDataGridItem(bool isUse, string name, Types type, string values)
		{
			IsUse = isUse;
			Name = name;
			Type = type;
			Values = values;
		}

		public bool IsValid()
		{
			if (Name == null || Name.Equals(string.Empty) || Values == null || Values.Equals(string.Empty))
			{
				return false;
			}

			foreach (var stringValue in Values.Split(Separator))
			{
				switch (Type)
				{
					case Types.Boolean:
					{
						bool boolValue;
						if (!Boolean.TryParse(stringValue, out boolValue))
						{
							return false;
						}
					}
						break;

					case Types.Integer:
					{
						int intValue;
						if (!Int32.TryParse(stringValue, out intValue))
						{
							return false;
						}
					}
						break;

					case Types.Float:
					{
						double floatValue;
						if (!Double.TryParse(stringValue, out floatValue))
						{
							return false;
						}
					}
						break;
				}
			}
			return true;
		}
	}
}
