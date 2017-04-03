using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using CN2.Core.DataStructures;
using CN2.UC.LearningExamplesView;
using Brush = System.Drawing.Brush;
using Brushes = System.Drawing.Brushes;

namespace CN2.UC.ExamineExamolesView
{
    public class ExamineExamplesDataGridItem: IValidatable
    {
        public bool IsUse { get; set; }
        public List<string> PredictiveAttributeValues { get; set; }
        public string DecisiveAttributeValue { get; set; }
        public string ExaminedAttributeValue { get; set; }
        public string ProductionRule { get; set; }

        public SolidColorBrush ExaminedAttributeValueColor
        {
            get
            {
                return DecisiveAttributeValue == null || ExaminedAttributeValue == null
                    ? new SolidColorBrush(Colors.Black)
                    : DecisiveAttributeValue.Equals(ExaminedAttributeValue) ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            }
            set { }
        }

        public ExamineExamplesDataGridItem()
        {
            IsUse = true;
            PredictiveAttributeValues = new List<string>();
        }

        public bool IsValid
        {
            get { return PredictiveAttributeValues.Count > 0 && !DecisiveAttributeValue.Equals(string.Empty) && !ExaminedAttributeValue.Equals(string.Empty); }
        }

        public ExamineExamplesDataGridItem(ExaminableExample examinedExample) : this()
        {
            //todo цикл должен быть по Attributes, а установка Result должна производиться отдельно
            foreach (var attributeValue in examinedExample.AllAttributes)
            {
                PredictiveAttributeValues.Add(attributeValue.Value);
            }
            DecisiveAttributeValue = examinedExample.DecisiveAttribute.Value;
            ExaminedAttributeValue = examinedExample.ExaminedAttribute.Value;
            ProductionRule = string.Empty;
            //ArguedExample = string.Empty;
        }

        //public ExamineExamplesDataGridItem(ExaminableExample examinedExample, ProductionRule productionRule, ArguedLearnableExample arguedExample) : this()
        public ExamineExamplesDataGridItem(ExaminableExample examinedExample, ProductionRule productionRule) : this()
        {
            foreach (var attributeValue in examinedExample.AllAttributes)
            {
                PredictiveAttributeValues.Add(attributeValue.Value);
            }
            DecisiveAttributeValue = examinedExample.DecisiveAttribute.Value;
            ExaminedAttributeValue = examinedExample.ExaminedAttribute.Value;
            ProductionRule = productionRule.GetStringValue();
            //ArguedExample = arguedExample.BecauseExpression.GetValueString();
        }
    }
}
