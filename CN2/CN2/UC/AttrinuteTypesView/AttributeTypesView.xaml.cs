using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CN2.Core.DataStructures;
using Microsoft.Win32;

namespace CN2.UC.AttrinuteTypesView
{
	/// <summary>
	/// Interaction logic for AttributeTypesView.xaml
	/// </summary>
	public partial class AttributeTypesView : UserControl
	{
		private List<AttributeTypesDataGridItem> _items;

		private CollectionView _itemsCV;
        
//	        get { return (from item in _items where item.IsUse && item.IsValid() select item).ToList().Count > 0; }

	    public AttributeTypesView()
		{
			InitializeComponent();

			_items = new List<AttributeTypesDataGridItem>();

			DataGridCheckBoxColumn isUseColumn = new DataGridCheckBoxColumn()
			{
				Header = "Использовать",
				Width = 30,
				Binding = new Binding("IsUse") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }
			};
			dataGrid.Columns.Add(isUseColumn);

			DataGridTextColumn nameColumn = new DataGridTextColumn()
			{
				Header = "Имя типа данных",
				Width = 100,
				Binding = new Binding("Name") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }
			};
			dataGrid.Columns.Add(nameColumn);

			DataGridComboBoxColumn typeColumn = new DataGridComboBoxColumn()
			{
				Header = "Тип данных значений",
				Width = 100,
				ItemsSource = Enum.GetValues(typeof (Types)).Cast<Types>(),
				SelectedItemBinding = new Binding("Type") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }
			};
			dataGrid.Columns.Add(typeColumn);

			DataGridTextColumn valuesColumn = new DataGridTextColumn()
			{
				Header = "Значения",
				Width = 300,
				Binding = new Binding("Values") {Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged}
			};
			dataGrid.Columns.Add(valuesColumn);

			dataGrid.ItemsSource = _items;

			_itemsCV = (CollectionView) CollectionViewSource.GetDefaultView(dataGrid.Items);
			((INotifyCollectionChanged) _itemsCV).CollectionChanged += OnItemsCVChanged;
		}

		#region средства для сериализации и десериализации списка типов атрибутов

		private void buttonLoadAttrinuteTypes_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				OpenFileDialog ofd = new OpenFileDialog()
				{
					Filter = SerializationData.FileDialogFilter,
					Title = SerializationData.LoadAttributeTypesFileDialogTitle
				};

				Nullable<bool> ofdResult = ofd.ShowDialog();

				if (ofdResult == true)
				{
                    XDocument attributeTypesXDocument = XDocument.Load(ofd.FileName);
					foreach (var attributeTypesXElement in attributeTypesXDocument.Elements())
					{
						if (attributeTypesXElement.Name.ToString().Equals(SerializationData.AttributeTypesNode))
						{
							foreach (var attributeTypeXElement in attributeTypesXElement.Elements())
							{
								if (attributeTypeXElement.Name.ToString().Equals(SerializationData.AttributeTypeNode))
								{
									AttributeTypesDataGridItem item = new AttributeTypesDataGridItem();
									foreach (var attributeTypeXAttribute in attributeTypeXElement.Attributes())
									{
										if (attributeTypeXAttribute.Name.ToString().Equals(SerializationData.IsUse))
										{
											item.IsUse = Convert.ToBoolean(attributeTypeXAttribute.Value);
										}
										if (attributeTypeXAttribute.Name.ToString().Equals(SerializationData.AttributeTypeName))
										{
											item.Name = attributeTypeXAttribute.Value;
										}
										if (attributeTypeXAttribute.Name.ToString().Equals(SerializationData.AttrinuteTypeType))
										{
											switch (attributeTypeXAttribute.Value)
											{
												case "Boolean":
													{
														item.Type = Types.Boolean;
													}
													break;

												case "Integer":
													{
														item.Type = Types.Integer;
													}
													break;

												case "Float":
													{
														item.Type = Types.Float;
													}
													break;

												case "String":
													{
														item.Type = Types.String;
													}
													break;

												default:
													{
														throw new Exception();
													}
													break;
											}
										}
										if (attributeTypeXAttribute.Name.ToString().Equals(SerializationData.AttributeTypeValues))
										{
											item.Values += attributeTypeXAttribute.Value;
										}
									}
									_items.Add(item);
									if (!item.IsValid())
									{
										//todo сделать получение строки по элементу
									}
								}
							}
						}
					}

					dataGrid.Items.Refresh();

					if (OnFileLoaded != null)
					{
						OnFileLoaded(sender, "Типы тарибутов загружены из файла " + ofd.FileName + ".");
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

		private void buttonSaveAttrinuteTypes_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				SaveFileDialog sfd = new SaveFileDialog()
				{
					Title = SerializationData.SaveAttributeTypesFileDialogTitle,
					Filter = SerializationData.FileDialogFilter,
					FileName = "attribute types"
				};

				Nullable<bool> sfdResult = sfd.ShowDialog();

				if (sfdResult == true)
				{
					XElement attributeTypesXElement = new XElement(SerializationData.AttributeTypesNode);
					foreach (var attributeType in _items)
					{
						attributeTypesXElement.Add(new XElement(SerializationData.AttributeTypeNode,
							new XAttribute(SerializationData.IsUse, attributeType.IsUse),
							new XAttribute(SerializationData.AttributeTypeName, attributeType.Name),
							new XAttribute(SerializationData.AttrinuteTypeType, attributeType.Type),
							new XAttribute(SerializationData.AttributeTypeValues, attributeType.Values)));
					}
					new XDocument(attributeTypesXElement).Save(sfd.FileName);

					if (OnFileSaved != null)
					{
						OnFileSaved(sender, "Типы тарибутов сохранены в файл " + sfd.FileName + ".");
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

		#endregion средства для сериализации и десериализации списка типов атрибутов

		/// <summary>
		/// Возвращает набор типов атрибутов.
		/// </summary>
		/// <returns>Набор типов атрибутов.</returns>
		public AttributeTypeSet GetTypeSet()
		{
			List<AttributeType> types = new List<AttributeType>();

			for (int i = 0; i < _items.Count - 1; i ++)
			{
				var item = _items[i];
				if (item.IsUse)
				{
					types.Add(new AttributeType(item.Name, item.Type, item.Values.Split(';')));
				}
			}

			return new AttributeTypeSet(types,
				new AttributeType(_items.Last().Name, _items.Last().Type, _items.Last().Values.Split(';')));
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
			AttributeTypesDataGridItem item = (AttributeTypesDataGridItem) e.Row.Item;
			if (item.IsValid())
			{
				e.Row.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
			}
			else
			{
				e.Row.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
			}
            OnAttributeTypesChanged?.Invoke(this);
        }

		#endregion обработчики событий элемента управления dataGrid

		private void OnItemsCVChanged(object sender, EventArgs e)
		{
			if (OnAttributeTypesChanged != null)
			{
				OnAttributeTypesChanged(this);
			}
		}

		#region события

		public delegate void ErrorOccuredHandler(object sender, string message);
		public event ErrorOccuredHandler OnErrorOccured;

		public delegate void FileSavedHandler(object sender, string message);
		public event FileSavedHandler OnFileSaved;

		public delegate void FileLoadedHandler(object sender, string message);
		public event FileLoadedHandler OnFileLoaded;

		public delegate void AttributeTypesChangedHandler(object sender);
		public event AttributeTypesChangedHandler OnAttributeTypesChanged;

		#endregion события
	}
}
