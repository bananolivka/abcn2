using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using Microsoft.Win32;
using System.IO;

namespace CN2.UC.LearningExamplesView
{
	/// <summary>
	/// Interaction logic for ExamplesView.xaml
	/// </summary>
	public partial class LearningExamplesView : UserControl
	{
		/// <summary>
		/// Набор типов атрибутов.
		/// </summary>
		private AttributeTypeSet _attributeTypeSet;
		/// <summary>
		/// Список примеров в представлении для элемента управления.
		/// </summary>
		private List<LearningExamplesDataGridItem> _items;

		public LearningExamplesView()
		{
			InitializeComponent();

            numericUpDownCoversCount.OnValueChanged += numericUpDownCoversCount_OnValueChanged;
        }

		public void SetAttributeTypes(AttributeTypeSet attributeTypesSet)
		{
			_attributeTypeSet = attributeTypesSet;

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
					TextBinding = new Binding("PredictiveAttributeValues[" + i + "]")
				};
				dataGrid.Columns.Add(predictiveAttributeTypeColumn);
			}

			DataGridComboBoxColumn decisiveAttributeValueColumn = new DataGridComboBoxColumn()
			{
				Header = _attributeTypeSet.DecisiveAttributeType.Name,
				Width = 100,
				ItemsSource = _attributeTypeSet.DecisiveAttributeType.Values,
				TextBinding = new Binding("DecisiveAttributeValue"),
				CellStyle =
					new Style(typeof (DataGridCell)) {Setters = {new Setter() {Property = ForegroundProperty, Value = Brushes.Blue}}}
			};
			dataGrid.Columns.Add(decisiveAttributeValueColumn);

			_items= new List<LearningExamplesDataGridItem>();
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

				if (ofdResult != null && ofdResult == true)
				{
					XDocument examplesXDocument = XDocument.Load(ofd.FileName);
					foreach (var examplesXElement in examplesXDocument.Elements())
					{
						if (examplesXElement.Name.ToString().Equals(SerializationData.ExamplesNode))
						{
							foreach (var exampleXElement in examplesXElement.Elements())
							{
								if (exampleXElement.Name.ToString().Equals(SerializationData.ExampleNode))
								{
									LearningExamplesDataGridItem item = new LearningExamplesDataGridItem();

									string[] precisiveValues = new string[_attributeTypeSet.PredictiveAttributeTypes.Count];
									string decisiveValue = string.Empty;

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
										int j = -2;
										string value = string.Empty;

										foreach (var exampleAttributeXAttribute in exampleAttributeXElement.Attributes())
										{
											switch (exampleAttributeXAttribute.Name.ToString())
											{
												case SerializationData.ExampleAttributeTypeName:
													{
														if (_attributeTypeSet.DecisiveAttributeType.Name.Equals(exampleAttributeXAttribute.Value))
														{
															j = -1;
														}
														else
														{
															for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i++)
															{
																if (_attributeTypeSet.PredictiveAttributeTypes[i].Name.Equals(exampleAttributeXAttribute.Value))
																{
																	j = i;
																	break;
																}
															}
														}

														if (j == -2)
														{
															OnErrorOccured(this,
																@"Тип атрибута """ + exampleAttributeXAttribute.Value +
																@""" не определён в текущем наборе типов атрибутов.");
															isNeedContinue = true;
															break;
														}
													}
													break;

												case SerializationData.ExampleAttrinuteValue:
													{
														value = exampleAttributeXAttribute.Value;
													}
													break;
											}

											if (isNeedContinue)
											{
												break;
											}
										}

										//todo проверить наличие значения у типа данных и если всё хорошо

										if (j == -1)
										{
											decisiveValue = value;
										}
										else
										{
											precisiveValues[j] = value;
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
										item.PredictiveAttributeValues.Insert(i, precisiveValues[i]);
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

				if (sfdResult != null && sfdResult == true)
				{
					XElement examplesXElement = new XElement(SerializationData.ExamplesNode);
					foreach (var example in _items)
					{
						XElement exampleXElement = new XElement(SerializationData.ExampleNode, new XAttribute(SerializationData.IsUse, example.IsUse));
						for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i ++)
						{
							exampleXElement.Add(new XElement(SerializationData.ExampleAttributeNode,
								new XAttribute(SerializationData.ExampleAttributeTypeName, _attributeTypeSet.PredictiveAttributeTypes[i].Name),
								new XAttribute(SerializationData.ExampleAttrinuteValue, example.PredictiveAttributeValues[i])));
						}
						exampleXElement.Add(new XElement(SerializationData.ExampleAttributeNode,
							new XAttribute(SerializationData.ExampleAttributeTypeName, _attributeTypeSet.DecisiveAttributeType.Name),
							new XAttribute(SerializationData.ExampleAttrinuteValue, example.DecisiveAttributeValue)));
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
		public List<LearnableExample> GetExamples()
		{
            List<LearnableExample> examples = new List<LearnableExample>();

			foreach (var item in _items)
			{
				if (!item.IsUse)
				{
					continue;
				}

				List<AttributeValue> predictiveAttributes = new List<AttributeValue>();
				AttributeValue decisionAttribute = null;
				
				for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i ++)
				{
					{
						predictiveAttributes.Add(new AttributeValue(_attributeTypeSet.PredictiveAttributeTypes[i], item.PredictiveAttributeValues[i]));
					}
				}
				examples.Add(new LearnableExample(predictiveAttributes, new AttributeValue(_attributeTypeSet.DecisiveAttributeType, item.DecisiveAttributeValue)));
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
            //for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i++)
            //{
            //    _items.Last().PredictiveAttributeValues.Insert(i, string.Empty);
            //}
            //_items.Last().DecisiveAttributeValue = string.Empty;

            for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i++)
            {
                _items.Last().PredictiveAttributeValues.Insert(i, _attributeTypeSet.PredictiveAttributeTypes[i].Values.First());
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

		private void dataGrid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			dataGrid.CommitEdit();
		}

		private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
            LearningExamplesDataGridItem item = (LearningExamplesDataGridItem)e.Row.Item;
            if (item.IsValid)
            {
                e.Row.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            else
            {
                e.Row.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
        }

		private void buttonLearn_Click(object sender, RoutedEventArgs e)
		{
			if (OnLearn != null)
			{
				OnLearn(this, null);
			}
		}

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
