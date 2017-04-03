using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CN2.Core.Algorithms;
using CN2.Core.DataStructures;
using CN2.UC.ExamineExamolesView;

namespace WpfApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private List<CN2.Core.Algorithms.CN2> _cn2Iqs;
        private List<CN2.Core.Algorithms.CN2> _arguedCN2Iqs;

	    private bool _isRandom;

        public MainWindow()
		{
			InitializeComponent();

			cn2AttributeTypesView.OnErrorOccured += ErrorOccured;
            cn2AttributeTypesView.OnFileSaved += FileSavedHandler;
            cn2AttributeTypesView.OnFileLoaded += FileLoadedHandler;
            cn2AttributeTypesView.OnAttributeTypesChanged += cn2AttributeTypesView_AttributeTypesChangedHandler;

			cn2LearningExamplesView.OnErrorOccured += ErrorOccured;
            cn2LearningExamplesView.OnFileSaved += FileSavedHandler;
            cn2LearningExamplesView.OnFileLoaded += FileLoadedHandler;
            cn2LearningExamplesView.OnLearn += cn2LearningExamplesView_LearnHandler;
            cn2LearningExamplesView.OnCoversCountChanged += cn2LearningExamplesView_OnCoversCountChangedHandler;


            cn2ExamineExamplesView.OnErrorOccured += ErrorOccured;
            cn2ExamineExamplesView.OnFileSaved += FileSavedHandler;
            cn2ExamineExamplesView.OnFileLoaded += FileLoadedHandler;
            cn2ExamineExamplesView.OnExamine += cn2ExamineExamplesView_ExamineHandler;

			_cn2Iqs = new List<CN2.Core.Algorithms.CN2>();

            arguedCN2AttributeTypesView.OnErrorOccured += ArguedErrorOccured;
            arguedCN2AttributeTypesView.OnFileSaved += ArguedFileSavedHandler;
            arguedCN2AttributeTypesView.OnFileLoaded += ArguedFileLoadedHandler;
            arguedCN2AttributeTypesView.OnAttributeTypesChanged += arguedCN2AttributeTypesView_AttributeTypesChangedHandler;

            arguedCN2LearningExamplesView.OnErrorOccured += ArguedErrorOccured;
            arguedCN2LearningExamplesView.OnFileSaved += ArguedFileSavedHandler;
            arguedCN2LearningExamplesView.OnFileLoaded += ArguedFileLoadedHandler;
            arguedCN2LearningExamplesView.OnLearn += arguedCN2LearningExamplesView_LearnHandler;
            arguedCN2LearningExamplesView.OnCoversCountChanged += arguedCN2LearningExamplesView_OnCoversCountChangedHandler;

            arguedCN2ExamineExamplesView.OnErrorOccured += ArguedErrorOccured;
            arguedCN2ExamineExamplesView.OnFileSaved += ArguedFileSavedHandler;
            arguedCN2ExamineExamplesView.OnFileLoaded += ArguedFileLoadedHandler;
            arguedCN2ExamineExamplesView.OnExamine += arguedCN2ExamineExamplesView_ExamineHandler;

            _arguedCN2Iqs = new List<CN2.Core.Algorithms.CN2>();

            _isRandom = true;
            mainWindow.ToolTip = "наугад";
            arguedCn2Label.ToolTip = "наугад";
        }

        #region обработчики событий

        private void ErrorOccured(object sender, string message)
        {
            log.WriteError(message);
        }

        private void ArguedErrorOccured(object sender, string message)
        {
            arguedLog.WriteError(message);
        }

        private void FileSavedHandler(object sender, string message)
        {
            log.Write(message);
        }

        private void ArguedFileSavedHandler(object sender, string message)
        {
            arguedLog.Write(message);
        }

        private void FileLoadedHandler(object sender, string message)
        {
            log.Write(message);
        }

        private void ArguedFileLoadedHandler(object sender, string message)
        {
            arguedLog.Write(message);
        }

        private void cn2AttributeTypesView_AttributeTypesChangedHandler(object sender)
		{
            try
            {
                AttributeTypeSet attributeTypeSet = cn2AttributeTypesView.GetTypeSet();
                cn2LearningExamplesView.SetAttributeTypes(attributeTypeSet);
                cn2LearningExamplesTabItem.IsEnabled = attributeTypeSet.IsValid();
                cn2ExamineExamplesView.SetAttributeTypes(attributeTypeSet);
            }
            catch (Exception exception)
            {
                cn2LearningExamplesTabItem.IsEnabled = false;
                ErrorOccured(this, exception.Message);
            }
		}

        private void arguedCN2AttributeTypesView_AttributeTypesChangedHandler(object sender)
        {
            try
            {
                AttributeTypeSet attributeTypeSet = arguedCN2AttributeTypesView.GetTypeSet();
                arguedCN2LearningExamplesView.SetAttributeTypes(attributeTypeSet);
                arguedCN2LearningExamplesTabItem.IsEnabled = attributeTypeSet.IsValid();
                arguedCN2ExamineExamplesView.SetArguedAttributeTypes(attributeTypeSet);
            }
            catch (Exception exception)
            {
                arguedCN2LearningExamplesTabItem.IsEnabled = false;
                ArguedErrorOccured(this, exception.Message);
            }
        }

	    private void cn2LearningExamplesView_LearnHandler(object sender, EventArgs e)
	    {
	        try
	        {
	            _cn2Iqs.Clear();

	            int coversCount = cn2LearningExamplesView.GetCoversCount();
	            log.WriteSection("Обучение по алгоритму CN2 с построением " + coversCount + " наборов правил.");
	            var attributeTypeSet = cn2AttributeTypesView.GetTypeSet();
	            log.Write("Решающий атрибут: " + attributeTypeSet.DecisiveAttributeType + ".");
	            log.Write("Предсказывающие атрибуты: ");
	            for (int i = 0; i < attributeTypeSet.PredictiveAttributeTypes.Count; i++)
	            {
	                log.Write(i + 1 + ". " + attributeTypeSet.PredictiveAttributeTypes[i] + ".");
	            }
	            for (int i = 0; i < coversCount; i++)
	            {
	                log.WriteSection("Определение " + (i + 1).ToString() + " набора правил.");

	                CN2.Core.Algorithms.CN2 iq = new CN2.Core.Algorithms.CN2();
	                iq.Learn(cn2LearningExamplesView.GetExamples(), cn2LearningExamplesView.GetStarSize(),
	                    cn2LearningExamplesView.GetHeapSize(), _isRandom);
	                _cn2Iqs.Add(iq);

	                for (int j = 0; j < iq.Cover.Count; j++)
	                {
	                    log.Write(j + 1 + ". Если " + iq.Cover[j].Condition.GetValueString() + ", то " +
	                              iq.Cover[j].Result.GetValueString() + ".");
	                }

	                log.WriteSuccess("Набор продукционных правил " + (i + 1).ToString() + " определён.");
	            }

	            List<Tuple<ProductionRule, int>> productionRulesChart = new List<Tuple<ProductionRule, int>>();
	            foreach (var iq in _cn2Iqs)
	            {
	                foreach (var productionRule in iq.Cover)
	                {
	                    bool isFounded = false;
	                    for (int i = 0; i < productionRulesChart.Count; i++)
	                    {
	                        if (productionRule.Equals(productionRulesChart[i].Item1))
	                        {
	                            productionRulesChart[i] = new Tuple<ProductionRule, int>(productionRulesChart[i].Item1,
	                                productionRulesChart[i].Item2 + 1);
	                            isFounded = true;
	                        }
	                    }
	                    if (!isFounded)
	                    {
	                        productionRulesChart.Add(new Tuple<ProductionRule, int>(productionRule, 1));
	                    }
	                }
	            }

	            for (int i = 0; i < productionRulesChart.Count; i++)
	            {
	                for (int j = i + 1; j < productionRulesChart.Count; j++)
	                {
	                    if (productionRulesChart[i].Item2 < productionRulesChart[j].Item2)
	                    {
	                        var buffer = productionRulesChart[i];
	                        productionRulesChart[i] = productionRulesChart[j];
	                        productionRulesChart[j] = buffer;
	                    }
	                }
	            }

	            log.WriteSection("Сводка правил.");
	            for (int i = 0; i < productionRulesChart.Count; i++)
	            {
	                log.Write((i + 1).ToString() + ". " + productionRulesChart[i].Item1 + ". Встречается " +
	                          productionRulesChart[i].Item2 + " раз.");
	            }
	            log.WriteSuccess("Обучение произведено.");

                cn2ExamineExamplesView.SetCoversCount(cn2LearningExamplesView.GetCoversCount());
                cn2ExamineExamplesTabItem.IsEnabled = true;
	        }
	        catch (Exception exception)
	        {
                cn2ExamineExamplesTabItem.IsEnabled = false;
                ErrorOccured(this, exception.Message);
	        }
	    }

	    private void cn2LearningExamplesView_OnCoversCountChangedHandler(object sender, int coversCount)
	    {
	        cn2ExamineExamplesView.SetCoversCount(coversCount);
	    }

        private void arguedCN2LearningExamplesView_LearnHandler(object sender, EventArgs e)
        {
            try
            {
                _arguedCN2Iqs.Clear();

                int coversCount = arguedCN2LearningExamplesView.GetCoversCount();
                arguedLog.WriteSection("Обучение по алгоритму CN2 с аргументированными примерами с построением " + coversCount +
                                 " наборов правил.");
                var attributeTypeSet = arguedCN2AttributeTypesView.GetTypeSet();
                arguedLog.Write("Решающий атрибут: " + attributeTypeSet.DecisiveAttributeType + ".");
                arguedLog.Write("Предсказывающие атрибуты: ");
                for (int i = 0; i < attributeTypeSet.PredictiveAttributeTypes.Count; i++)
                {
                    arguedLog.Write(i + 1 + ". " + attributeTypeSet.PredictiveAttributeTypes[i] + ".");
                }
                for (int i = 0; i < coversCount; i++)
                {
                    arguedLog.WriteSubSection1("Определение " + (i + 1).ToString() + " набора правил.");

                    CN2.Core.Algorithms.CN2 iq = new CN2.Core.Algorithms.CN2();
                    iq.Learn(arguedCN2LearningExamplesView.GetExamples(), arguedCN2LearningExamplesView.GetStarSize(),
                        arguedCN2LearningExamplesView.GetHeapSize(), _isRandom);
                    _arguedCN2Iqs.Add(iq);
                     
                    for (int j = 0; j < iq.Cover.Count; j++)
                    {
                        arguedLog.Write(j + 1 + ". Если " + iq.Cover[j].Condition.GetValueString() + ", то " +
                                  iq.Cover[j].Result.GetValueString() + ".");
                    }

                    arguedLog.WriteSubSection2("Аргументированные правила:");
                    for (int j = 0; j < iq.ArguedCover.Count; j ++)
                    {
                        arguedLog.Write(j + 1 + ". Если " + iq.ArguedCover[j].Item1.Condition.GetValueString() + ", то " +
                                  iq.ArguedCover[j].Item1.Result.GetValueString() + " потому что: " +
                                  iq.ArguedCover[j].Item2.BecauseExpression + ".");
                    }

                    arguedLog.WriteSuccess("Набор продукционных правил " + (i + 1).ToString() + " определён.");
                }

                List<Tuple<ProductionRule, int>> productionRulesChart = new List<Tuple<ProductionRule, int>>();
                foreach (var iq in _arguedCN2Iqs)
                {
                    foreach (var productionRule in iq.Cover)
                    {
                        bool isFounded = false;
                        for (int i = 0; i < productionRulesChart.Count; i++)
                        {
                            if (productionRule.Equals(productionRulesChart[i].Item1))
                            {
                                productionRulesChart[i] = new Tuple<ProductionRule, int>(productionRulesChart[i].Item1,
                                    productionRulesChart[i].Item2 + 1);
                                isFounded = true;
                            }
                        }
                        if (!isFounded)
                        {
                            productionRulesChart.Add(new Tuple<ProductionRule, int>(productionRule, 1));
                        }
                    }
                }

                for (int i = 0; i < productionRulesChart.Count; i++)
                {
                    for (int j = i + 1; j < productionRulesChart.Count; j++)
                    {
                        if (productionRulesChart[i].Item2 < productionRulesChart[j].Item2)
                        {
                            var buffer = productionRulesChart[i];
                            productionRulesChart[i] = productionRulesChart[j];
                            productionRulesChart[j] = buffer;
                        }
                    }
                }

                arguedLog.WriteSection("Сводка правил.");
                for (int i = 0; i < productionRulesChart.Count; i++)
                {
                    arguedLog.Write((i + 1).ToString() + ". " + productionRulesChart[i].Item1 + ". Встречается " +
                              productionRulesChart[i].Item2 +
                              (productionRulesChart[i].Item2 >= 2 && productionRulesChart[i].Item2 <= 4
                                  ? " раза."
                                  : " раз."));
                }
                arguedLog.WriteSuccess("Обучение произведено.");

                arguedCN2ExamineExamplesView.SetCoversCount(arguedCN2LearningExamplesView.GetCoversCount());
                arguedCB2ExamineExamplesTabItem.IsEnabled = true;
            }
            catch (Exception exception)
            {
                arguedCB2ExamineExamplesTabItem.IsEnabled = false;
                ArguedErrorOccured(this, exception.Message);
            }
        }

        private void arguedCN2LearningExamplesView_OnCoversCountChangedHandler(object sender, int coversCount)
        {
            arguedCN2ExamineExamplesView.SetCoversCount(coversCount);
        }

        private void cn2ExamineExamplesView_ExamineHandler(object sender, EventArgs e)
		{
		    try
		    {
                log.WriteSection("Экзамен по алгоритму CN2 для " + cn2ExamineExamplesView.CoverId + " набора продукционных правил.");

                var iq = _cn2Iqs[cn2ExamineExamplesView.CoverId];

                List<ExaminableExample> examineResults = iq.Examine(cn2ExamineExamplesView.GetExamples()).Select(example=>example.Item1).ToList();
		        cn2ExamineExamplesView.SetExaminedExamples(examineResults);

                log.Write("Процент правильно рассчитанных примеров: " + iq.PCRE + ".");
                log.WriteSuccess("Экзамен проведён.");
            }
            catch (Exception exception)
            {
                ErrorOccured(this, exception.Message);
            }
        }

        private void arguedCN2ExamineExamplesView_ExamineHandler(object sender, EventArgs e)
        {
            try
            {
                arguedLog.WriteSection("Экзамен по алгоритму CN2 с аргументацией примеров для "+ cn2ExamineExamplesView.CoverId + " набора продукционных правил.");

                var arguedIq = _arguedCN2Iqs[arguedCN2ExamineExamplesView.CoverId];

                List<Tuple<ExaminableExample, ProductionRule>> examineResults = arguedIq.Examine(arguedCN2ExamineExamplesView.GetExamples());
                arguedCN2ExamineExamplesView.SetExaminedExamples(examineResults);

                arguedLog.Write("Процент правильно рассчитанных примеров: " + arguedIq.PCRE + ".");
                arguedLog.WriteSuccess("Экзамен проведён.");
            }
            catch (Exception exception)
            {
                ArguedErrorOccured(this, exception.Message);
            }
        }

        #endregion обработчики событий

        private void label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_isRandom)
            {
                _isRandom = false;
                cn2Label.ToolTip = "по очереди";
                arguedCn2Label.ToolTip = "по очереди";
            }
            else
            {
                _isRandom = true;
                cn2Label.ToolTip = "наугад";
                arguedCn2Label.ToolTip = "наугад";
            }
        }
    }
}
