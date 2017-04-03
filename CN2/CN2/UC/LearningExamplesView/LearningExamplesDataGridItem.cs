using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CN2.Core.DataStructures;

namespace CN2.UC.LearningExamplesView
{
	/// <summary>
	/// Представляет пример обучающей выборки для элемента управления LearningExamplesView.
	/// </summary>
	public class LearningExamplesDataGridItem:IValidatable
	{
		/// <summary>
		/// Возвращает и задаёт признак использования примера для элемента управления LearningExamplesView.
		/// </summary>
		public bool IsUse { get; set; }
		/// <summary>
		/// Возвращает и задаёт список значений предсказывающих атрибутов примера для элемента управления LearningExamplesView.
		/// </summary>
		public List<string> PredictiveAttributeValues { get; set; }
		/// <summary>
		/// Возвращает и задаёт значение решающего атрибута примера для элемента управления LearningExamplesView.
		/// </summary>
		public string DecisiveAttributeValue { get; set; }

	    public bool IsValid
	    {
	        get { return PredictiveAttributeValues.Count > 0 && !DecisiveAttributeValue.Equals(string.Empty); }
	    }

	    public LearningExamplesDataGridItem()
		{
			IsUse = true;
			PredictiveAttributeValues = new List<string>();
		}
	}
}
