using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace CN2.Core.DataStructures
{
	/// <summary>
	/// Представляет набор типов атрибутов.
	/// </summary>
	public class AttributeTypeSet
	{
		/// <summary>
		/// Список типов предсказывающих атрибутов.
		/// </summary>
		private List<AttributeType> _predictiveAttributeTypeses;
		/// <summary>
		/// Тип решающего атрибута.
		/// </summary>
		private AttributeType _decisiveAttributeType;

		/// <summary>
		/// Возвращает список предсказывающих атрибутов.
		/// </summary>
		public List<AttributeType> PredictiveAttributeTypes { get { return _predictiveAttributeTypeses; } }
		/// <summary>
		/// Возвращает тип предсказывающего атрибута.
		/// </summary>
		public AttributeType DecisiveAttributeType { get { return _decisiveAttributeType; } }

		public AttributeTypeSet(IEnumerable<AttributeType> predictiveAttributeTypes, AttributeType decisiveAttributeType)
		{
			_predictiveAttributeTypeses = new List<AttributeType>(predictiveAttributeTypes);
			_decisiveAttributeType = decisiveAttributeType;
		}

	    public List<AttributeType> GetAttributeTypes()
	    {
	        return new List<AttributeType>(_predictiveAttributeTypeses) {_decisiveAttributeType};
	    }

	    public bool IsValid()
	    {
	        if (!_decisiveAttributeType.IsValid())
	        {
	            return false;
	        }
	        foreach (var predictiveAttributeType in _predictiveAttributeTypeses)
	        {
	            if (!predictiveAttributeType.IsValid())
	            {
	                return false;
	            }
	        }
	        return true;
	    }
	}
}
