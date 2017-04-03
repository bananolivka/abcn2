using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CN2.Core.DataStructures;
using CN2.UC.LearningExamplesView;

namespace CN2.UC.ArguedLearningExamplesView
{
	/// <summary>
	/// Представляет аргументированный пример обучающей выборки для элемента управления LearningExamplesView.
	/// </summary>
	public class ArguedLearningExamplesDataGridItem:IValidatable
	{
        /// <summary>
		/// Возвращает и задаёт признак использования примера для элемента управления LearningExamplesView.
		/// </summary>
		public bool IsUse { get; set; }
        /// <summary>
        /// Возвращает и задаёт список значений предсказывающих атрибутов примера для элемента управления LearningExamplesView.
        /// </summary>
        public List<PredictiveAttributeValue> PredictiveAttributeValues { get; set; }
        /// <summary>
        /// Возвращает и задаёт значение решающего атрибута примера для элемента управления LearningExamplesView.
        /// </summary>
        public string DecisiveAttributeValue { get; set; }
        
        //public string BecauseExpression { get; set; }
        //public string DespiteExpression { get; set; }

        public bool IsValid { get
        {
            return
                (from value in PredictiveAttributeValues where value.IsBecauseExpression select value).ToList().Count >
                0;
        } }

        public ArguedLearningExamplesDataGridItem()
        {
            IsUse = true;
            PredictiveAttributeValues = new List<PredictiveAttributeValue>();
        }
	}
}
