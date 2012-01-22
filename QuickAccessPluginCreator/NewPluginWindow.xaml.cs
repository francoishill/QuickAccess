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
//using System.Windows.Shapes;

namespace QuickAccessPluginCreator
{
	/// <summary>
	/// Interaction logic for NewPluginWindow.xaml
	/// </summary>
	public partial class NewPluginWindow : Window
	{
		public NewPluginWindow()
		{
			InitializeComponent();


			comboBoxPluginType.Items.Clear();
			comboBoxPluginType.Items.Add(typeof(InlineCommandToolkit.InlineCommands.ICommandWithHandler));
			comboBoxPluginType.Items.Add(typeof(InlineCommandToolkit.IMouseGesture));

			tmp();
		}

		private const string TemplateInitialPart = "QuickAccessPluginCreator.Templates";
		private void tmp()
		{
			Dictionary<string, List<string>> templates = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);

			string tempDir = Path.GetTempPath() + "NewPlugins";

			System.Reflection.Assembly objAssembly = System.Reflection.Assembly.GetExecutingAssembly();
			string[] myResources = objAssembly.GetManifestResourceNames();
			foreach (string s in myResources)
				if (s.ToLower().StartsWith(TemplateInitialPart.ToLower()))
				{
					string pathWithoutInitialPart = s.Substring(TemplateInitialPart.Length + 1);
					string folderName = pathWithoutInitialPart.Split('.')[0];
					if (!templates.ContainsKey(folderName))
						templates.Add(folderName, new List<string>());
					templates[folderName].Add(s);
				}

			//foreach (string key in templates.Keys)
			//{
			//	foreach (string file in templates[key])
			//	{
			//		Stream stream = objAssembly.GetManifestResourceStream(file);
			//		int length = (int)stream.Length;
			//		byte[] bytesOfPublishHtmlTemplateDLL = new byte[length];
			//		stream.Read(bytesOfPublishHtmlTemplateDLL, 0, length);
			//		stream.Close();
			//		FileStream fileStream = new FileStream(tempFilename, FileMode.Create);
			//		fileStream.Write(bytesOfPublishHtmlTemplateDLL, 0, length);
			//		fileStream.Close();
			//		string textOfFile = File.ReadAllText(tempFilename);
			//	}
			//}

			//UserMessages.ShowInfoMessage(templates.ToString());
			//foreach (string reso in myResources)
			//	if (reso.ToLower().EndsWith(HtmlFileName.ToLower()))
			//	{
			//	}
		}
	}
}
