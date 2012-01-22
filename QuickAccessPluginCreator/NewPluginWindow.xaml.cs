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

		private const string TemplateInitialPart = "QuickAccessPluginCreator.Templates.";
		private void tmp()
		{
			string newCommandName = UserMessages.Prompt("Please enter the command name");

			if (newCommandName == null)
				return;

			Dictionary<string, List<string>> templates = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);

			string tempDir = Path.GetTempPath() + "NewPlugins\\";

			System.Reflection.Assembly objAssembly = System.Reflection.Assembly.GetExecutingAssembly();
			string[] myResources = objAssembly.GetManifestResourceNames();
			foreach (string s in myResources)
				if (s.ToLower().StartsWith(TemplateInitialPart.ToLower()))
				{
					string pathWithoutInitialPart = s.Substring(TemplateInitialPart.Length);
					string folderName = pathWithoutInitialPart.Split('.')[0];
					if (!templates.ContainsKey(folderName))
						templates.Add(folderName, new List<string>());
					templates[folderName].Add(s);
				}

			foreach (string key in templates.Keys)
			{
				foreach (string file in templates[key])
				{
					string fileRelativePath = file.Substring(TemplateInitialPart.Length);
					int lastDotPos = fileRelativePath.LastIndexOf('.');
					fileRelativePath = fileRelativePath.Replace('.', '\\');
					string fileRelativeLastDotPutBackAgain =
						fileRelativePath.Substring(0, lastDotPos) +
						"." +
						fileRelativePath.Substring(lastDotPos + 1);

					string fullpathOfFile = tempDir + fileRelativeLastDotPutBackAgain;
					string fileDir = Path.GetDirectoryName(fullpathOfFile);
					if (Directory.Exists(fileDir))
					{
						try
						{
							Array.ForEach(Directory.GetFiles(fileDir, "*.*", SearchOption.AllDirectories), path => File.Delete(path));
							Directory.Delete(fileDir, true);
						}
						catch (Exception exc)
						{
							UserMessages.ShowWarningMessage(exc.Message);
						}
					}
				}
			}

			foreach (string key in templates.Keys)
			{
				string tmpCsProjFilepath = null;
				//0				  10				20				30				40				50				60				70				80
				//01234567890123456789012345678901234567890123456789012345678901234567890123456789012
				//QuickAccessPluginCreator.Templates.CommandPluginTemplate.Properties.AssemblyInfo.cs
				foreach (string file in templates[key])
				{
					string fileRelativePath = file.Substring(TemplateInitialPart.Length);//CommandPluginTemplate.Properties.AssemblyInfo.cs
					int lastDotPos = fileRelativePath.LastIndexOf('.');//80
					fileRelativePath = fileRelativePath.Replace('.', '\\');
					string fileRelativeLastDotPutBackAgain =
						fileRelativePath.Substring(0, lastDotPos) +
						"." +
						fileRelativePath.Substring(lastDotPos + 1);

					string fullpathOfFile = tempDir + fileRelativeLastDotPutBackAgain;
					string fileDir = Path.GetDirectoryName(fullpathOfFile);
					//Directories are deleted in the previous loop (to ensure only the new files are there, no old ones)
					Directory.CreateDirectory(Path.GetDirectoryName(fullpathOfFile));

					Stream stream = objAssembly.GetManifestResourceStream(file);
					int length = (int)stream.Length;
					byte[] bytesOfPublishHtmlTemplateDLL = new byte[length];
					stream.Read(bytesOfPublishHtmlTemplateDLL, 0, length);
					stream.Close();

					fullpathOfFile = fullpathOfFile.Replace("NewCommandPluginTemplate", newCommandName);

					FileStream fileStream = new FileStream(fullpathOfFile, FileMode.Create);
					fileStream.Write(bytesOfPublishHtmlTemplateDLL, 0, length);
					fileStream.Close();
					//string textOfFile = File.ReadAllText(fullpathOfFile);
					//textOfFile = textOfFile.Replace("NewCommandPluginTemplate", newCommandName);

					string[] lines = File.ReadAllLines(fullpathOfFile);
					List<string> newLines = new List<string>();
					Dictionary<string, string> fullReplacementTokens = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
					foreach (string lin in lines)
					{
						string line = lin.Replace("NewCommandPluginTemplate", newCommandName);
						newLines.Add(line);

						if (line.Contains("[{") && line.Contains("}]"))
						{
							string tmpline = line;
							while (tmpline.Contains("[{") && tmpline.Contains("}]"))
							{
								int startIndex = tmpline.IndexOf("[{");
								int endIndex = tmpline.IndexOf("}]");
								string fullReplacementToken = tmpline.Substring(startIndex, endIndex + 2 - startIndex);
								if (!fullReplacementTokens.ContainsKey(fullReplacementToken))
									fullReplacementTokens.Add(fullReplacementToken, null);
								tmpline = tmpline.Substring(endIndex + 2);
							}
						}
					}

					List<string> keyList = fullReplacementTokens.Keys.ToList();
					for (int i = 0; i < keyList.Count; i++)
					{
						string keyreplace = keyList[i];
						string answer = null;
						if (keyreplace.Substring(2, keyreplace.Length - 4).ToLower() == "CommandName".ToLower())
							answer = newCommandName;
						else
							answer = UserMessages.Prompt("Please enter " + keyreplace.Substring(2, keyreplace.Length - 4));
						if (answer == null)
							continue;
						fullReplacementTokens[keyreplace] = answer;
					}

					for (int i = 0; i < newLines.Count; i++)
						foreach (string replacement in fullReplacementTokens.Keys)
							newLines[i] = newLines[i].Replace(replacement, fullReplacementTokens[replacement]);

					File.WriteAllLines(fullpathOfFile, newLines);
					if (fullpathOfFile.ToLower().EndsWith(".csproj"))
						tmpCsProjFilepath = fullpathOfFile;
				}
				if (tmpCsProjFilepath != null && File.Exists(tmpCsProjFilepath))
				{
					VisualStudioInterop.BuildVsProjectReturnNewversionString(
						newCommandName,
						tmpCsProjFilepath,
						"",
						false,
						VisualStudioInterop.BuildType.Rebuild,
						VisualStudioInterop.ProjectConfiguration.Release,
						VisualStudioInterop.PlatformTarget.x86,
						false,
						null);
					System.Diagnostics.Process.Start("explorer", "/select, " + tmpCsProjFilepath);
				}
			}

			//UserMessages.ShowInfoMessage(templates.ToString());
			//foreach (string reso in myResources)
			//	if (reso.ToLower().EndsWith(HtmlFileName.ToLower()))
			//	{
			//	}
		}
	}
}
