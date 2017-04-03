using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CN2.UC
{
	/// <summary>
	/// Представляет данные, необходимые для сериализации и десериализации объектов.
	/// </summary>
	public class SerializationData
	{
		public const string SaveAttributeTypesFileDialogTitle = "Сохранение типов атрибутов";
		public const string LoadAttributeTypesFileDialogTitle = "Загрузка типов атрибутов";
		public const string SaveExamplesFileDialogTitle = "Сохранение примеров";
		public const string LoadExamplesFileDialogTitle = "Загрузка примеров";
		public const string FileDialogFilter = "XML документ (*.xml)|*.xml";
		public const string SaveLogFileDialogTitle = "Сохранение журнала";
		public const string LogFileDialogFilter = "Текстовый файл (*.txt)|*.txt";

		public const string IsUse = "isUse";

		#region наименования узлов и атрибутов элеменотов файла типов атрибутов

		public const string AttributeTypesNode = "attrinuteTypes";
		public const string AttributeTypeNode = "attributeType";

		public const string AttributeTypeName = "name";
		public const string AttrinuteTypeType = "type";
		public const string AttributeTypeValues = "values";

		#endregion наименования узлов и атрибутов элеменотов файла типов атрибутов

		#region наименования узлов и атрибутов элементов файла примеров

		public const string ExamplesNode = "examples";
		public const string ExampleNode = "example";
		public const string ExampleAttributeNode = "attribute";

		public const string ExampleAttributeTypeName = "typeName";
		public const string ExampleAttrinuteValue = "value";
		public const string ExampleAttributeIsDecisive = "isDecisive";
	    public const string ExampleAttributeIsBecause = "isBecause";
	    public const string ExampleAttributeIsDespite = "isDespite";

	    #endregion наименования узлов и атрибутов элементов файла примеров
	}
}
