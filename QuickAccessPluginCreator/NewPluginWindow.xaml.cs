using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
		}

		private static NewPluginWindow tmpWindow;
		private const string TemplateInitialPart = "QuickAccessPluginCreator.Templates.";
		public static void ShowAndSavePlugin()
		{
			string newCommandName = UserMessages.Prompt("Please enter the command name");

			if (newCommandName == null)
				return;

			newCommandName += newCommandName.EndsWith("Plugin", StringComparison.InvariantCultureIgnoreCase) ? "" : "Plugin";

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
				Dictionary<string, string> fullReplacementTokens = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

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
					foreach (string line in lines)
					{
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

					//List<string> keyList = fullReplacementTokens.Keys.ToList();
					//for (int i = 0; i < keyList.Count; i++)
					//{
					//	string keyreplace = keyList[i];
					//	string answer = null;
					//	if (keyreplace.Substring(2, keyreplace.Length - 4).ToLower() == "CommandName".ToLower())
					//		answer = newCommandName;
					//	else
					//		answer = UserMessages.Prompt("Please enter " + keyreplace.Substring(2, keyreplace.Length - 4));
					//	i, f (answer == null)
					//		continue;
					//	fullReplacementTokens[keyreplace] = answer;
					//}

					//for (int i = 0; i < newLines.Count; i++)
					//	foreach (string replacement in fullReplacementTokens.Keys)
					//		newLines[i] = newLines[i].Replace(replacement, fullReplacementTokens[replacement]);

					//File.WriteAllLines(fullpathOfFile, newLines);
					//if (fullpathOfFile.ToLower().EndsWith(".csproj"))
					//	tmpCsProjFilepath = fullpathOfFile;
				}

				if (tmpWindow == null)
					tmpWindow = new NewPluginWindow();
				if (fullReplacementTokens.ContainsKey("[{CommandName}]"))
					fullReplacementTokens["[{CommandName}]"] = newCommandName;
				tmpWindow.propertyGrid1.SelectedObject = GenerateObjectWithReplaceValues(ref fullReplacementTokens, new List<string> { "CommandName" });
				if (tmpWindow.ShowDialog() == true)
				{
					foreach (PropertyInfo pi in tmpWindow.propertyGrid1.SelectedObject.GetType().GetProperties())
						fullReplacementTokens["[{" + pi.Name + "}]"] = pi.GetValue(tmpWindow.propertyGrid1.SelectedObject).ToString();
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
						fullpathOfFile = fullpathOfFile.Replace("NewCommandPluginTemplate", newCommandName);

						string[] lines = File.ReadAllLines(fullpathOfFile);
						List<string> newLines = new List<string>();
						foreach (string lin in lines)
						{
							string line = lin.Replace("NewCommandPluginTemplate", newCommandName);
							newLines.Add(line);
						}

						for (int i = 0; i < newLines.Count; i++)
							foreach (string replacement in fullReplacementTokens.Keys)
								newLines[i] = newLines[i].Replace(replacement, fullReplacementTokens[replacement]);

						File.WriteAllLines(fullpathOfFile, newLines);
						if (fullpathOfFile.ToLower().EndsWith(".csproj"))
							tmpCsProjFilepath = fullpathOfFile;
					}
				}
				tmpWindow = null;

				if (tmpCsProjFilepath != null && File.Exists(tmpCsProjFilepath))
				{
					///
					///
					//TODO: Add here to check if successfully built, then ask user if plugin must be placed in QuickAccess\Plugins in the programfiles folder.
					///
					///
					string newVersionString = VisualStudioInterop.BuildVsProjectReturnNewversionString(
						newCommandName,
						tmpCsProjFilepath,
						"",
						false,
						VisualStudioInterop.BuildType.Rebuild,
						VisualStudioInterop.ProjectConfiguration.Release,
						VisualStudioInterop.PlatformTarget.x86,
						false,
						null);

					if (newVersionString != null)
					{
						string quickAccessProjectDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Visual Studio 2010\Projects\QuickAccess";
						if (Directory.Exists(quickAccessProjectDir)
							&& (UserMessages.Confirm("The plugin was successfully created and built." + Environment.NewLine
							+ "The following directory is found on this machine, do you want to move the source files to this directory (directory = " + quickAccessProjectDir + "?", DefaultYesButton: true)))
						{
							string newDir = quickAccessProjectDir + "\\" + newCommandName;
							Directory.Move(tempDir + key, newDir);
							tmpCsProjFilepath = newDir + "\\" + Path.GetFileName(tmpCsProjFilepath);
						}
						else
						{
							string newDir = UserMessages.ChooseDirectory("Please choose directory into which source folder will be moved", Directory.Exists(quickAccessProjectDir) ? quickAccessProjectDir : null);
							if (newDir != null)
							{
								if (newDir[newDir.Length - 1] != '\\') newDir += "\\";
								newDir += newCommandName;
								Directory.Move(tempDir + key, newDir);
								tmpCsProjFilepath = newDir + "\\" + Path.GetFileName(tmpCsProjFilepath);
							}
						}
						//if (UserMessages.Confirm("New plugin () successfully built, 
					}

					System.Diagnostics.Process.Start("explorer", "/select, " + tmpCsProjFilepath);
				}
			}

			//UserMessages.ShowInfoMessage(templates.ToString());
			//foreach (string reso in myResources)
			//	if (reso.ToLower().EndsWith(HtmlFileName.ToLower()))
			//	{
			//	}
		}

		public static object GenerateObjectWithReplaceValues(ref Dictionary<string, string> ReplaceValueDictionary, List<string> ignoreValues)
		{
			List<DynamicCodeInvoking.DynamicTypeBuilder.Property> properties = new List<DynamicCodeInvoking.DynamicTypeBuilder.Property>();
			foreach (string key in ReplaceValueDictionary.Keys)
				if (!ignoreValues.Contains(key.Substring(2, key.Length - 4), StringComparer.InvariantCultureIgnoreCase))
				properties.Add(new DynamicCodeInvoking.DynamicTypeBuilder.Property(key.Substring(2, key.Length - 4), typeof(string)));

			return DynamicCodeInvoking.DynamicTypeBuilder.CreateNewObject(
				"ReplaceValues",
				properties);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
	}
}
