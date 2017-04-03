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
using Microsoft.Win32;

namespace CN2.UC.Log
{
	/// <summary>
	/// Interaction logic for Log.xaml
	/// </summary>
	public partial class Log : UserControl
	{
		public Log()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Очищает содержимое журнала.
		/// </summary>
		public void Clear()
		{
			logTextBlock.Text = string.Empty;
		}

		public string CorrectMessage(string message)
		{
			return message.EndsWith(Environment.NewLine) ? message : message + Environment.NewLine;
		}

		private void WriteColor(string message, Brush brush)
		{
			logTextBlock.Dispatcher.Invoke(
				new Action(
					() => logTextBlock.Inlines.Add(new Run( /*DateTime.Now + ": " + */CorrectMessage(message)) {Foreground = brush})));
			scrollViewer.Dispatcher.Invoke(new Action(() => scrollViewer.ScrollToBottom()));
		}

		public void Write(string message)
		{
			WriteColor(message, Brushes.Black);
		}

		public void WriteOfftop(string message)
		{
			WriteColor(message, Brushes.Gray);
		}

		public void WriteError(string message)
		{
			WriteColor(message, Brushes.Red);
		}

		public void WriteSuccess(string message)
		{
			WriteColor(message, Brushes.Green);
		}

		public void WriteWarning(string message)
		{
			WriteColor(message, Brushes.YellowGreen);
		}

		public void WriteSection(string message)
		{
			WriteColor(message, Brushes.Blue);
		}

	    public void WriteSubSection1(string message)
	    {
	        WriteColor(message, Brushes.CornflowerBlue);
	    }

        public void WriteSubSection2(string message)
        {
            WriteColor(message, Brushes.DeepSkyBlue);
        }

        private void buttonSaveLog_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				SaveFileDialog sfd = new SaveFileDialog()
				{
					Title = SerializationData.SaveLogFileDialogTitle,
					Filter = SerializationData.LogFileDialogFilter,
					FileName = "log" + DateTime.Now.ToFileTime()
				};

				Nullable<bool> sfdResult = sfd.ShowDialog();

				if (sfdResult != null && sfdResult == true)
				{
					using (StreamWriter logFile = new StreamWriter(sfd.FileName))
					{
						logFile.Write(logTextBlock.Text);
					}
				}
			}
			catch (Exception ex)
			{
				WriteError(ex.Message);
			}
		}
	}
}
