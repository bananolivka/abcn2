using System;
using System.Collections.Generic;
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

namespace CN2.UC.NumericUpDown
{
	/// <summary>
	/// Interaction logic for NumericUpDown.xaml
	/// </summary>
	public partial class NumericUpDown : UserControl
	{
		/// <summary>
		/// Минимальное значение элемента управления.
		/// </summary>
		private int _minValue;
		/// <summary>
		/// Максимальное значение элемента управления.
		/// </summary>
		private int _maxValue;
		/// <summary>
		/// Шаг значений элемента управления.
		/// </summary>
		private int _step;

	    /// <summary>
	    /// Возвращает и задаёт минимальное значение элемента управления.
	    /// </summary>
	    public int MinValue
	    {
	        get { return _minValue; }
	        set
	        {
	            _minValue = value;
	            if (Value < _minValue)
	            {
	                Value = _minValue;
	            }
	        }
	    }

	    /// <summary>
	    /// Возвращает и задаёт максимальное значение элемента управления.
	    /// </summary>
	    public int MaxValue
	    {
	        get { return _maxValue; }
	        set
	        {
	            _maxValue = value;
	            if (Value > _maxValue)
	            {
	                Value = _maxValue;
	            }
	        }
	    }
	    /// <summary>
		/// Возвращает и задаёт шаг значений элемента управления.
		/// </summary>
		public int Step { get { return _step; } set { _step = value; } }
		/// <summary>
		/// Возвращает и задаёт значение элемента управления.
		/// </summary>
		public int Value
		{
			get
			{
				return Convert.ToInt32(textBoxValue.Text);
			}
			set
			{
				if (value <= _minValue)
				{
					textBoxValue.Text = _minValue.ToString();
					return;
				}
				if (value >= _maxValue)
				{
					textBoxValue.Text = _maxValue.ToString();
					return;
				}
				textBoxValue.Text = value.ToString();
			}
		}

		public NumericUpDown()
		{
			InitializeComponent();

			_minValue = 0;
			_maxValue = 100;
			_step = 1;
		}

        private void textBoxValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (OnValueChanged != null)
            {
                OnValueChanged(this, Value);
            }
        }

        private void buttonUp_Click(object sender, RoutedEventArgs e)
		{
			int incrementedValue = Value + _step;
			if (incrementedValue >= _maxValue)
			{
				Value = _maxValue;
			}
			else
			{
				Value = incrementedValue;
			}
		}

		private void buttonDown_Click(object sender, RoutedEventArgs e)
		{
			int decrementedValue = Value - _step;
			if (decrementedValue <= _minValue)
			{
				Value = _minValue;
			}
			else
			{
				Value = decrementedValue;
			}
		}

        public delegate void ValueChangedHandler(object sender, int value);
        public event ValueChangedHandler OnValueChanged;
    }
}
