using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CN2.Core.DataStructures
{
	public enum Types
	{
		Boolean,
		Integer,
		Float,
		String
	}

	/// <summary>
	/// Представляет тип атрибута.
	/// </summary>
	[DebuggerDisplay("{ToString()}")]
	public class AttributeType
	{
		protected string _name;
		protected Types _types;
		protected List<string> _values;

		public string Name { get { return _name; } set { _name = value; } }
		public Types Types { get { return _types; } set { _types = value; } }
		public List<string> Values { get { return _values; } }

		public AttributeType() { }

		public AttributeType(string name, Types types)
		{
			_name = name;
			_types = types;
			_values = new List<string>();
		}

		public AttributeType(string name, Types types, IEnumerable<string> values)
		{
			_name = name;
			_types = types;
			_values = new List<string>();
			foreach (var value in values)
			{
				bool retBool;
				int retInt;
				double retDouble;

				if (_types == Types.Boolean && Boolean.TryParse(value, out retBool) ||
					_types == Types.Integer && Int32.TryParse(value, out retInt) ||
					_types == Types.Float && Double.TryParse(value, out retDouble) || _types == Types.String)
				{
					_values.Add(value);
				}
				else
				{
					throw new ArgumentException();
				}
			}
		}

		public AttributeType(AttributeType sample)
		{
			_name = sample.Name;
			_types = sample.Types;
			_values = sample.Values;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private bool IsValid(string value)
		{
			switch (_types)
			{
				case Types.Boolean:
					{
						bool retBool;
						return Boolean.TryParse(value, out retBool);
					}
					break;

				case Types.Integer:
					{
						int retInt;
						return Int32.TryParse(value, out retInt);
					}
					break;

				case Types.Float:
					{
						double retFloat;
						return Double.TryParse(value, out retFloat);
					}
					break;

				case Types.String:
					{
						return !value.Equals(string.Empty);
					}
					break;

				default:
					{
						return false;
					}
					break;
			}
		}

		/// <summary>
		/// Проверяет наличие значения среди множества значений типа атрибута.
		/// </summary>
		/// <param name="value">Проверяемое значние.</param>
		/// <returns><c>true</c>, если среди значений типа атрибута есть проверяемое значение; иначе - <c>false</c>.</returns>
		public bool HasValue(string value)
		{
			return IsValid(value) && _values.Contains(value);
		}

		/// <summary>
		/// Добавляет
		/// </summary>
		/// <param name="value"></param>
		public void Add(string value)
		{
			if (!HasValue(value))
			{
				_values.Add(value);
			}
		}

		/// <summary>
		/// Удаляет значение из множества значений типа атрибута.
		/// </summary>
		/// <param name="value">Удаляемое значение.</param>
		public void Remove(string value)
		{
			_values.Remove(value);
		}

		/// <summary>
		/// Производит сброс множества значений типа атрибута.
		/// </summary>
		public void Clear()
		{
			_values.Clear();
		}

	    public bool IsValid()
	    {
	        if (_name.Equals(string.Empty) || _values.Count == 0)
	        {
	            return false;
	        }
	        foreach (var value in _values)
	        {
	            if (!IsValid(value))
	            {
	                return false;
	            }
	        }
	        return true;
	    }

		/// <summary>
		/// С УЧЁТОМ ПОРЯДКА!!!
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			AttributeType otc = obj as AttributeType;
			if (otc == null || !_name.Equals(otc.Name) || _types != otc.Types || _values.Count != otc.Values.Count)
			{
				return false;
			}
			for (int i = 0; i < _values.Count; i++)
			{
				if (!_values[i].Equals(otc.Values[i]))
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
			string values = string.Empty;
			for (int i = 0; i < _values.Count; i++)
			{
				if (i == 0)
				{
					values += "(";
				}
				if (i == _values.Count - 1)
				{
					values += _values[i] + ")";
				}
				else
				{
					values += _values[i] + "; ";
				}
			}
			return "Name = " + _name + " Type = " + _types + " Values = " + values;
		}
	}
}
