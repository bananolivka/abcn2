using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CN2.UC.ExamineExamolesView
{
	/// <summary>
	/// Interaction logic for ExamineExamplesView.xaml
	/// </summary>
	public partial class ExamineExamplesView : UserControl
	{
		/// <summary>
		/// Список типов атрибутов.
		/// </summary>
		private AttributeTypeSet _attributeTypeSet;
		/// <summary>
		/// <summary>
		/// Индекс типа решающего атрибута в списке типов атрибутов.
		/// </summary>
		private int _decisiveAttributeIndex;

        public int CoverId { get { return numericUpDownCoverIndex.Value - 1; } }

		public ExamineExamplesView()
		{
			InitializeComponent();
		}

		public void SetAttributeTypes(AttributeTypeSet attributeTypeSet)
		{
			List<ExamineExamplesDataGridItem> items = new List<ExamineExamplesDataGridItem>();
			_attributeTypeSet = attributeTypeSet;

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
				DataGridComboBoxColumn attributeTypeColumn = new DataGridComboBoxColumn()
				{
					Header = _attributeTypeSet.PredictiveAttributeTypes[i].Name,
					Width = 100,
					ItemsSource = _attributeTypeSet.PredictiveAttributeTypes[i].Values,
					TextBinding = new Binding("PredictiveAttributeValues[" + i + "]")
				};
				dataGrid.Columns.Add(attributeTypeColumn);
			}

			DataGridComboBoxColumn decisiveComboBoxColumn = new DataGridComboBoxColumn()
			{
				Header = _attributeTypeSet.DecisiveAttributeType.Name,
				Width = 100,
				ItemsSource = _attributeTypeSet.DecisiveAttributeType.Values,
				TextBinding = new Binding("DecisiveAttributeValue"),
				CellStyle =
					new Style(typeof (DataGridCell)) {Setters = {new Setter() {Property = ForegroundProperty, Value = Brushes.Blue}}}
			};
			dataGrid.Columns.Add(decisiveComboBoxColumn);

            DataGridTextColumn examinedColumn = new DataGridTextColumn()
            {
                Header = "Экзамен",
                Width = 100,
                Binding = new Binding("ExaminedAttributeValue"),
                IsReadOnly = true,
                CellStyle = new Style()
                {
                    TargetType = typeof(DataGridCell),
                    Setters =
                    {
                        new Setter(DataGridCell.ForegroundProperty, new Binding("ExaminedAttributeValueColor"))
                    }
                }
            };
            dataGrid.Columns.Add(examinedColumn);

            dataGrid.ItemsSource = items;
            //dataGrid.DataContext = items;
            dataGrid.Items.Refresh();
		}

	    public void SetArguedAttributeTypes(AttributeTypeSet attributeTypeSet)
	    {
	        SetAttributeTypes(attributeTypeSet);

            DataGridTextColumn productionRuleColumn = new DataGridTextColumn()
            {
                Header = "Причина",
                Width = 300,
                Binding = new Binding("ProductionRule"),
                IsReadOnly = true
            };
            dataGrid.Columns.Add(productionRuleColumn);
        }


        public void SetExaminedExamples(IEnumerable<ExaminableExample> examinedExamples)
		{
			List<ExamineExamplesDataGridItem> items = new List<ExamineExamplesDataGridItem>();
			foreach (var examinedExample in examinedExamples)
			{
				 items.Add(new ExamineExamplesDataGridItem(examinedExample));
			}
			dataGrid.ItemsSource = items;
			//dataGrid.Items.Refresh();
		}

	    public void SetExaminedExamples(IEnumerable<Tuple<ExaminableExample, ProductionRule>> examinedExamples)
	    {
            List<ExamineExamplesDataGridItem> items = new List<ExamineExamplesDataGridItem>();
            foreach (var examinedExample in examinedExamples)
            {
                items.Add(new ExamineExamplesDataGridItem(examinedExample.Item1, examinedExample.Item2));
            }
            dataGrid.ItemsSource = items;
        }

	    public void SetCoversCount(int coversCount)
	    {
	        numericUpDownCoverIndex.MinValue = 1;
	        numericUpDownCoverIndex.MaxValue = coversCount;
	        numericUpDownCoverIndex.Value = 1;
	    }

	    public void ResetCoversCount()
	    {
	        numericUpDownCoverIndex.MinValue = 0;
	        numericUpDownCoverIndex.MaxValue = 0;
	    }

		private void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
		{
			List<ExamineExamplesDataGridItem> items = new List<ExamineExamplesDataGridItem>(dataGrid.ItemsSource.Cast<ExamineExamplesDataGridItem>());
			items.Last().PredictiveAttributeValues = new List<string>();
			for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i++)
			{
				items.Last().PredictiveAttributeValues.Insert(i, string.Empty);
			}
			items.Last().DecisiveAttributeValue = string.Empty;
			items.Last().ExaminedAttributeValue = string.Empty;
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
					List<ExamineExamplesDataGridItem> items = new List<ExamineExamplesDataGridItem>();

					XDocument examplesXDocument = XDocument.Load(ofd.FileName);
					foreach (var examplesXElement in examplesXDocument.Elements())
					{
						if (examplesXElement.Name.ToString().Equals(SerializationData.ExamplesNode))
						{
							foreach (var exampleXElement in examplesXElement.Elements())
							{
								if (exampleXElement.Name.ToString().Equals(SerializationData.ExampleNode))
								{
									ExamineExamplesDataGridItem item = new ExamineExamplesDataGridItem();

									string[] predictiveValues = new string[_attributeTypeSet.PredictiveAttributeTypes.Count];
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
											predictiveValues[j] = value;
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
										item.PredictiveAttributeValues.Insert(i, predictiveValues[i]);
									}
									item.DecisiveAttributeValue = decisiveValue;
									item.ExaminedAttributeValue = string.Empty;
									items.Add(item);
								}
							}
						}
					}

					dataGrid.ItemsSource = items;

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
					foreach (var example in dataGrid.ItemsSource.Cast<ExamineExamplesDataGridItem>())
					{
						XElement exampleXElement = new XElement(SerializationData.ExampleNode, new XAttribute(SerializationData.IsUse, example.IsUse));
						for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i++)
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

		public List<ExaminableExample> GetExamples()
		{
			List<ExaminableExample> examples = new List<ExaminableExample>();

			foreach (var item in dataGrid.ItemsSource.Cast<ExamineExamplesDataGridItem>())
			{
				if (!item.IsUse)
				{
					continue;
				}

				List<AttributeValue> attributes = new List<AttributeValue>();
				for (int i = 0; i < _attributeTypeSet.PredictiveAttributeTypes.Count; i++)
				{
					attributes.Add(new AttributeValue(_attributeTypeSet.PredictiveAttributeTypes[i], item.PredictiveAttributeValues[i]));
				}
				examples.Add(new ExaminableExample(attributes,
					new AttributeValue(_attributeTypeSet.DecisiveAttributeType, item.DecisiveAttributeValue)));
			}

			return examples;
		}

		#region обработчики событий элемента управления dataGrid

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
            ExamineExamplesDataGridItem item = (ExamineExamplesDataGridItem)e.Row.Item;
            if (item.IsValid)
            {
                e.Row.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            else
            {
                e.Row.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
        }

		private void buttonExamine_Click(object sender, RoutedEventArgs e)
		{
			if (OnExamine != null)
			{
				OnExamine(this, null);
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

		public delegate void ExamineHandler(object sender, EventArgs e);
		public event ExamineHandler OnExamine;

		#endregion события
	}
}
