using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using CN2.Core.DataStructures;
using CN2.UC.LearningExamplesView;
using Microsoft.Win32;

namespace CN2.UC.ArguedLearningExamplesView
{
	/// <summary>
	/// Interaction logic for ArguedLearningExamplesView.xaml
	/// </summary>
	public partial class ArguedLearningExamplesView : UserControl
	{
		/// <summary>
		/// Набор типов атрибутов.
		/// </summary>
		private AttributeTypeSet _attributeTypeSet;
		/// <summary>
		/// Список аргументированных примеров в представлении для элемента управления.
		/// </summary>
		private List<ArguedLearningExamplesDataGridItem> _items;

		public ArguedLearningExamplesView()
		{
			InitializeComponent();

		    numericUpDownCoversCount.OnValueChanged += numericUpDownCoversCount_OnValueChanged;
		}

		public void SetAttributeTypes(AttributeTypeSet attributeTypesSet)
		{
            _attributeTypeSet = attributeTypesSet;

            //List<CheckBox> becauseExpressionItems = new List<CheckBox>();
            //List<CheckBox> despiteExpressionItems = new List<CheckBox>();

            dataGrid.Columns.Clear();

            DataGridCheckBoxColumn isUseColumn = new DataGridCheckBoxColumn()
            {
                Header = "Использовать",
                Width = 30,
                Binding = new Binding("IsUse")
            };
            dataGrid.Columns.Add(isUseColumn);

            for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i++)
            {
                DataGridComboBoxColumn predictiveAttributeTypeColumn = new DataGridComboBoxColumn()
                {
                    Header = _attributeTypeSet.PredictiveAttributeTypes[i].Name,
                    Width = 100,
                    ItemsSource = _attributeTypeSet.PredictiveAttributeTypes[i].Values,
                    TextBinding = new Binding("PredictiveAttributeValues[" + i + "].Value"),

                };
                dataGrid.Columns.Add(predictiveAttributeTypeColumn);

                DataGridCheckBoxColumn becauseExpressionColumn = new DataGridCheckBoxColumn()
                {
                    Header = "Потому что",
                    Width = 30,
                    Binding =
                        new Binding("PredictiveAttributeValues[" + i + "].IsBecauseExpression")
                        {
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        }
                };
                dataGrid.Columns.Add(becauseExpressionColumn);

                DataGridCheckBoxColumn despiteExpressionColumn = new DataGridCheckBoxColumn()
                {
                    Header = "Несмотря на",
                    Width = 30,
                    Binding =
                        new Binding("PredictiveAttributeValues[" + i + "].IsDespiteExpression")
                        {
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        }
                };
                dataGrid.Columns.Add(despiteExpressionColumn);

                //   CheckBox becauseCheckBox = new CheckBox();
                //   becauseCheckBox.SetBinding(ToggleButton.ContentProperty,
                //       new Binding("PredictiveAttributeValues[" + i + "].Value")
                //       {
                //           Mode = BindingMode.TwoWay,
                //           UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                //       });
                //becauseCheckBox.SetBinding(ToggleButton.IsCheckedProperty,
                //             "PredictiveAttributeValues[" + i + "].IsBecauseExpression");
                //         becauseExpressionItems.Add(becauseCheckBox);

                //         CheckBox despiteCheckBox = new CheckBox();
                //   despiteCheckBox.SetBinding(ToggleButton.ContentProperty,
                //       new Binding("PredictiveAttributeValues[" + i + "].Value")
                //       {
                //           Mode = BindingMode.TwoWay,
                //           UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                //       });
                //   despiteCheckBox.SetBinding(ToggleButton.IsCheckedProperty,
                //       new Binding("IsDespiteExpression") {ElementName = "PredictiveAttributeValues[" + i + "]"});
                //         despiteExpressionItems.Add(despiteCheckBox);
            }

            DataGridComboBoxColumn decisiveAttributeValueColumn = new DataGridComboBoxColumn()
            {
                Header = _attributeTypeSet.DecisiveAttributeType.Name,
                Width = 100,
                ItemsSource = _attributeTypeSet.DecisiveAttributeType.Values,
                TextBinding = new Binding("DecisiveAttributeValue"),
                CellStyle =
                    new Style(typeof(DataGridCell)) { Setters = { new Setter() { Property = ForegroundProperty, Value = Brushes.Blue } } }
            };
            dataGrid.Columns.Add(decisiveAttributeValueColumn);

            //DataGridComboBoxColumn becauseExpressionColumn = new DataGridComboBoxColumn()
            //{
            //    Header = "Потому что",
            //          ItemsSource = becauseExpressionItems
            //};
            //      dataGrid.Columns.Add(becauseExpressionColumn);

            //DataGridComboBoxColumn despiteExpressionColumn = new DataGridComboBoxColumn()
            //{
            //    Header = "Несмотря на",
            //    ItemsSource = despiteExpressionItems
            //};
            //      dataGrid.Columns.Add(despiteExpressionColumn);

            _items = new List<ArguedLearningExamplesDataGridItem>();
            dataGrid.ItemsSource = _items;
        }



		#region средства для сериализации и десериализации списка примеров

		private void buttonLoadExamples_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    Filter = SerializationData.FileDialogFilter,
                    Title = SerializationData.LoadExamplesFileDialogTitle
                };

                Nullable<bool> ofdResult = ofd.ShowDialog();

                if (ofdResult == true)
                {
                    var attributeTypes = _attributeTypeSet.GetAttributeTypes();

                    XDocument examplesXDocument = XDocument.Load(ofd.FileName);
                    foreach (var examplesXElement in examplesXDocument.Elements())
                    {
                        if (examplesXElement.Name.ToString().Equals(SerializationData.ExamplesNode))
                        {
                            foreach (var exampleXElement in examplesXElement.Elements())
                            {
                                if (exampleXElement.Name.ToString().Equals(SerializationData.ExampleNode))
                                {
                                    ArguedLearningExamplesDataGridItem item = new ArguedLearningExamplesDataGridItem();

                                    AttributeType[] predictiveValuesType = new AttributeType[_attributeTypeSet.PredictiveAttributeTypes.Count];
                                    string[] predictiveValues = new string[_attributeTypeSet.PredictiveAttributeTypes.Count];
                                    bool[] predictiveValuesIsDecisive = new bool[_attributeTypeSet.PredictiveAttributeTypes.Count];
                                    bool[] predictiveValuesIsBecause = new bool[_attributeTypeSet.PredictiveAttributeTypes.Count];
                                    bool[] predictiveValuesIsDespite = new bool[_attributeTypeSet.PredictiveAttributeTypes.Count];
                                    AttributeType decisiveValueType = null;
                                    string decisiveValue = string.Empty;
                                    bool decisiveValuieIsDecisive = false;
                                    bool decisiveValuieIsBecause = false;
                                    bool decisiveValuieIsDespite = false;


                                    // признак того, что следует перейти к следующзему примеру
                                    bool isNeedContinue = false;

                                    foreach (var exampleXAttribute in exampleXElement.Attributes())
                                    {
                                        if (exampleXAttribute.Name.ToString().Equals(SerializationData.IsUse))
                                        {
                                            bool isUse;
                                            if (Boolean.TryParse(exampleXAttribute.Value, out isUse))
                                            {
                                                item.IsUse = isUse;
                                            }
                                            else
                                            {
                                                if (OnErrorOccured != null)
                                                {
                                                    OnErrorOccured(this, @"Недопустимое значение атрибута ""Использовать"".");
                                                    isNeedContinue = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (isNeedContinue)
                                    {
                                        continue;
                                    }

                                    foreach (var exampleAttributeXElement in exampleXElement.Elements())
                                    {
                                        AttributeType type = null;
                                        string value = string.Empty;
                                        bool isDecisive = false;
                                        bool isBecause = false;
                                        bool isDespite = false;

                                        int predictiveAttributeIndex = -1;

                                        foreach (var exampleAttributeXAttribute in exampleAttributeXElement.Attributes())
                                        {
                                            switch (exampleAttributeXAttribute.Name.ToString())
                                            {
                                                case SerializationData.ExampleAttributeTypeName:
                                                    {
                                                        if (exampleAttributeXAttribute.Value.Equals(_attributeTypeSet.DecisiveAttributeType.Name))
                                                        {
                                                            type = _attributeTypeSet.DecisiveAttributeType;
                                                        }
                                                        else
                                                        {
                                                            for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i++)
                                                            {
                                                                var attributeType = _attributeTypeSet.PredictiveAttributeTypes[i];
                                                                if (exampleAttributeXAttribute.Value.Equals(attributeType.Name))
                                                                {
                                                                    type = attributeType;
                                                                    predictiveAttributeIndex = i;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    break;

                                                case SerializationData.ExampleAttrinuteValue:
                                                    {
                                                        value = exampleAttributeXAttribute.Value;
                                                    }
                                                    break;

                                                case SerializationData.ExampleAttributeIsDecisive:
                                                    {
                                                        Boolean.TryParse(exampleAttributeXAttribute.Value, out isDecisive);
                                                    }
                                                    break;

                                                case SerializationData.ExampleAttributeIsBecause:
                                                    {
                                                        Boolean.TryParse(exampleAttributeXAttribute.Value, out isBecause);
                                                    }
                                                    break;

                                                case SerializationData.ExampleAttributeIsDespite:
                                                    {
                                                        Boolean.TryParse(exampleAttributeXAttribute.Value, out isDespite);
                                                    }
                                                    break;
                                            }

                                            if (isNeedContinue)
                                            {
                                                break;
                                            }
                                        }

                                        //todo проверить наличие значения у типа данных и если всё хорошо

                                        if (isDecisive)
                                        {
                                            decisiveValueType = type;
                                            decisiveValue = value;
                                            decisiveValuieIsDecisive = isDecisive;
                                            decisiveValuieIsBecause = isBecause;
                                            decisiveValuieIsDespite = isDespite;
                                        }
                                        else
                                        {
                                            predictiveValuesType[predictiveAttributeIndex] = type;
                                            predictiveValues[predictiveAttributeIndex] = value;
                                            predictiveValuesIsDecisive[predictiveAttributeIndex] = isDecisive;
                                            predictiveValuesIsBecause[predictiveAttributeIndex] = isBecause;
                                            predictiveValuesIsDespite[predictiveAttributeIndex] = isDespite;
                                        }

                                        if (isNeedContinue)
                                        {
                                            break;
                                        }
                                    }

                                    if (isNeedContinue)
                                    {
                                        continue;
                                    }

                                    for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i++)
                                    {
                                        item.PredictiveAttributeValues.Insert(i,
                                            new PredictiveAttributeValue(predictiveValues[i], predictiveValuesIsBecause[i],
                                                predictiveValuesIsDespite[i]));
                                    }
                                    item.DecisiveAttributeValue = decisiveValue;
                                    _items.Add(item);
                                }
                            }
                        }
                    }

                    dataGrid.Items.Refresh();

                    if (OnFileLoaded != null)
                    {
                        OnFileLoaded(sender, "Примеры загружены из файла " + ofd.FileName + ".");
                    }
                }
            }
            catch (Exception ex)
            {
                if (OnErrorOccured != null)
                {
                    OnErrorOccured(this, ex.Message);
                }
            }
        }

        private void buttonSaveExamples_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				SaveFileDialog sfd = new SaveFileDialog()
				{
					Title = SerializationData.SaveExamplesFileDialogTitle,
					Filter = SerializationData.FileDialogFilter,
					FileName = "examples"
				};

				Nullable<bool> sfdResult = sfd.ShowDialog();

				if (sfdResult == true)
				{
					XElement examplesXElement = new XElement(SerializationData.ExamplesNode);
					foreach (var example in _items)
					{
						XElement exampleXElement = new XElement(SerializationData.ExampleNode, new XAttribute(SerializationData.IsUse, example.IsUse));
						for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i++)
						{
						    exampleXElement.Add(new XElement(SerializationData.ExampleAttributeNode,
						        new XAttribute(SerializationData.ExampleAttributeTypeName,
						            _attributeTypeSet.PredictiveAttributeTypes[i].Name),
                                new XAttribute(SerializationData.ExampleAttrinuteValue, example.PredictiveAttributeValues[i].Value),
                                new XAttribute(SerializationData.ExampleAttributeIsDecisive, false),
                                new XAttribute(SerializationData.ExampleAttributeIsBecause, example.PredictiveAttributeValues[i].IsBecauseExpression),
                                new XAttribute(SerializationData.ExampleAttributeIsDespite, example.PredictiveAttributeValues[i].IsDespiteExpression)));
						}
					    exampleXElement.Add(new XElement(SerializationData.ExampleAttributeNode,
					        new XAttribute(SerializationData.ExampleAttributeTypeName, _attributeTypeSet.DecisiveAttributeType.Name),
					        new XAttribute(SerializationData.ExampleAttrinuteValue, example.DecisiveAttributeValue),
					        new XAttribute(SerializationData.ExampleAttributeIsDecisive, true),
					        new XAttribute(SerializationData.ExampleAttributeIsBecause, false),
					        new XAttribute(SerializationData.ExampleAttributeIsDespite, false)));
						examplesXElement.Add(exampleXElement);
					}

					new XDocument(examplesXElement).Save(sfd.FileName);

					if (OnFileSaved != null)
					{
						OnFileSaved(sender, "Примеры сохранены в файл " + sfd.FileName + ".");
					}
				}
			}
			catch (Exception ex)
			{
				if (OnErrorOccured != null)
				{
					OnErrorOccured(this, ex.Message);
				}
			}
		}

		#endregion средства для сериализации и десериализации списка примеров

		/// <summary>
		/// Возвращает список обучающих примеров.
		/// </summary>
		/// <returns></returns>
		public List<ArguedLearnableExample> GetExamples()
		{
            List<ArguedLearnableExample> examples = new List<ArguedLearnableExample>();

			foreach (var item in _items)
			{
				if (!item.IsUse)
				{
					continue;
				}

				List<AttributeValue> predictiveAttributes = new List<AttributeValue>();
                List<IExpressionMember> becauseExpressionValues = new List<IExpressionMember>();
                List<IExpressionMember> despiteExpressionValues = new List<IExpressionMember>();

				for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i++)
				{
				    var attributeValue = new AttributeValue(_attributeTypeSet.PredictiveAttributeTypes[i],
				        item.PredictiveAttributeValues[i].Value);

                    predictiveAttributes.Add(attributeValue);

				    if (item.PredictiveAttributeValues[i].IsBecauseExpression)
				    {
				        becauseExpressionValues.Add(attributeValue);
				    }
				    if (item.PredictiveAttributeValues[i].IsDespiteExpression)
				    {
				        despiteExpressionValues.Add(attributeValue);
				    }
				}
			    examples.Add(new ArguedLearnableExample(predictiveAttributes,
			        new AttributeValue(_attributeTypeSet.DecisiveAttributeType, item.DecisiveAttributeValue),
			        new Core.DataStructures.Expression(Operation.Con, becauseExpressionValues),
			        despiteExpressionValues.Count == 0
			            ? null
			            : new Core.DataStructures.Expression(Operation.Con, despiteExpressionValues)));
			}

			return examples;
		}

		/// <summary>
		/// Возвращает STARSIZE.
		/// </summary>
		/// <returns>STARSIZE.</returns>
		public int GetStarSize()
		{
			return numericUpDownStarSize.Value;
		}

		/// <summary>
		/// Возвращает HEAPSIZE.
		/// </summary>
		/// <returns>HEAPSIZE.</returns>
		public int GetHeapSize()
		{
			return numericUpDownHeapSize.Value;
		}

		/// <summary>
		/// Возвращает количество проходов алгоритма.
		/// </summary>
		/// <returns>Количество проходов алгоритма.</returns>
		public int GetCoversCount()
		{
			return numericUpDownCoversCount.Value;
		}

		#region обработчики событий элемента управления dataGrid

		private void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
		{
			for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i++)
			{
			    _items.Last()
			        .PredictiveAttributeValues.Insert(i,
			            new PredictiveAttributeValue(_attributeTypeSet.PredictiveAttributeTypes[i].Values.First(), false, false));
            }
			_items.Last().DecisiveAttributeValue = _attributeTypeSet.DecisiveAttributeType.Values.First();

		}

		private void dataGrid_GotFocus(object sender, RoutedEventArgs e)
		{
			if (e.OriginalSource.GetType() == typeof(DataGridCell))
			{
				dataGrid.BeginEdit(e);
			}
		}

		private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
            ArguedLearningExamplesDataGridItem item = (ArguedLearningExamplesDataGridItem)e.Row.Item;
            if (item.IsValid)
            {
                e.Row.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            else
            {
                e.Row.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }

      //      int columnIndex = dataGrid.Columns.IndexOf(e.Column);
      //      for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i ++)
		    //{
		    //    if (e.Column.Header.Equals("Потому что"))
		    //    {
		            
		    //    }
		    //}
		    


		    //foreach (var arguedLearningExamplesDataGridItem in _items)
		    //{
		    //    if (item.Equals(arguedLearningExamplesDataGridItem))
		    //    {
		    //        continue;
		    //    }

            
		    //}
        }

		private void buttonLearn_Click(object sender, RoutedEventArgs e)
		{
            OnLearn?.Invoke(this, null);
        }

		#region средства для установки аргументации

		/// <summary>
		/// Признак активного режима установки 
		/// </summary>
		private bool _isSettingBecauseArgumentMode;
		private bool _isSettingDespiteArgumentMode;

		private List<AttributeValue> _becauseExpressionAttributeValues;
		private List<AttributeValue> _despiteExpressionAttributeValues;

		private void setBecauseExpressionButton_Click(object sender, RoutedEventArgs e)
		{
			if (_isSettingDespiteArgumentMode)
			{
				FlushDespiteExpression();
			}

			if (_isSettingBecauseArgumentMode)
			{
				FlushBecauseExpression();
			}
			else
			{
				_isSettingBecauseArgumentMode = true;

				_becauseExpressionAttributeValues = new List<AttributeValue>();
			}
		}

		private void setDespiteExpressionButton_Click(object sender, RoutedEventArgs e)
		{
			if (_isSettingBecauseArgumentMode)
			{
				FlushBecauseExpression();
			}

			if (_isSettingDespiteArgumentMode)
			{
				FlushDespiteExpression();
			}
			else
			{
				_isSettingDespiteArgumentMode = true;
			}
		}

		private void FlushBecauseExpression()
		{
			_isSettingBecauseArgumentMode = false;

			_becauseExpressionAttributeValues = null;
		}

		private void FlushDespiteExpression()
		{
			_isSettingDespiteArgumentMode = false;

			_despiteExpressionAttributeValues = null;
		}

		private void dataGridCell_MouseDown(object sender, MouseButtonEventArgs e)
		{
			
		}

		private void dataGrid_MouseUp(object sender, MouseButtonEventArgs e)
		{
			#region определление индекса и значения

			int columnIndex = 0;
			string value = string.Empty;

			DependencyObject dep = (DependencyObject) e.OriginalSource;

			while (dep != null && !(dep is DataGridCell) && !(dep is DataGridColumnHeader))
			{
				dep = VisualTreeHelper.GetParent(dep);
			}

			if (dep == null)
			{
				return;
			}

			if (dep is DataGridCell)
			{
				DataGridCell cell = dep as DataGridCell;
				columnIndex = cell.Column.DisplayIndex;
			}

			if (dep is DataGridColumnHeader)
			{
				DataGridColumnHeader header = dep as DataGridColumnHeader;
				columnIndex = header.DisplayIndex;
			}

			if (columnIndex <= 2 || columnIndex > 2 + _attributeTypeSet.PredictiveAttributeTypes.Count)
			{
				return;
			}

			#endregion определление индекса и значения

			if (_isSettingBecauseArgumentMode && _isSettingDespiteArgumentMode)
			{
				if (OnErrorOccured != null)
				{
					OnErrorOccured(this, "Ошибка ввода аргументации примеров.");
				}

				_isSettingBecauseArgumentMode = false;
				_isSettingDespiteArgumentMode = false;
			}

			if (_isSettingBecauseArgumentMode)
			{
				_becauseExpressionAttributeValues.Add(new AttributeValue(_attributeTypeSet.PredictiveAttributeTypes[columnIndex], value));
			}

			if (_isSettingDespiteArgumentMode)
			{
				_despiteExpressionAttributeValues.Add(new AttributeValue(_attributeTypeSet.PredictiveAttributeTypes[columnIndex], value));
			}
		}

		#endregion средства для установки аргументации

        private void numericUpDownCoversCount_OnValueChanged(object sender, int value)
	    {
            if (OnCoversCountChanged != null)
            {
                OnCoversCountChanged(this, numericUpDownCoversCount.Value);
            }
	    }

        #endregion обработчики событий элемента управления dataGrid

        #region события

        public delegate void ErrorOccuredHandler(object sender, string message);
		public event ErrorOccuredHandler OnErrorOccured;

		public delegate void FileSavedHandler(object sender, string message);
		public event FileSavedHandler OnFileSaved;

		public delegate void FileLoadedHandler(object sender, string message);
		public event FileLoadedHandler OnFileLoaded;

		public delegate void LearnHandler(object sender, EventArgs e);
		public event LearnHandler OnLearn;

	    public delegate void CoversCountChangedHandler(object sender, int coversCount);
        public event CoversCountChangedHandler OnCoversCountChanged;

	    #endregion события
	}
}
