using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
//TODO: Facedetection disabled for now
//using Emgu.CV;
//using Emgu.CV.Structure;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Win32;
using SharedClasses;

//This line is supposed to prevent dissasembly with ILDASM, but it does not prevent dotPeek
[assembly: SuppressIldasmAttribute()]

namespace QuickAccess
{
	static class Program
	{
		const string UrlHandlerArgument = "handleurl";
		const string UrlHandlerUriStart = "quickaccess";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			AssociateFacesFileExtensionInRegistry();
			AssociateUrlProtocolHandler();
			AutoUpdatingForm.CheckForUpdates(
				exitApplicationAction: delegate { Application.Exit(); },
				ActionIfUptoDate_Versionstring: (versionstring) => { Form1.CurrentVersionString = versionstring; });
			SingleInstanceApplication.Run(NewInstanceHandler);
		}

		public static void NewInstanceHandler(object sender, StartupNextInstanceEventArgs e)
		{
			e.BringToForeground = false;			

			Form1 form1 = null;
			var app = sender as SingleInstanceApplication;
			if (app != null)
				form1 = app.GetMainForm() as Form1;

			ProcessCommandLineArguments(form1, e.CommandLine.ToArray(), false);
		}

		public static void ProcessCommandLineArguments(Form1 mainForm, string[] args, bool IsFirstInstance)
		{
			if (args.Length == 3)
			{
				if (args[1] == null)
					return;

				if (args[1].Equals("extract", StringComparison.InvariantCultureIgnoreCase))
				{
					if (File.Exists(args[2]))
					{
						string saveToDir = Path.GetDirectoryName(args[2]) + "\\ExtractedFaces";
						if (!Directory.Exists(saveToDir))
							Directory.CreateDirectory(saveToDir);

						//TODO: Facedetection disabled for now
						//int counter = 1;
						//Dictionary<string, List<Image<Gray, byte>>> tmpList = new Dictionary<string, List<Image<Gray, byte>>>();
						//FaceDetectionInterop.ExtractFacesFromFile(FaceDetectionInterop.Passphrase, FaceDetectionInterop.Salt, ref tmpList, args[2]);
						//foreach (string name in tmpList.Keys)
						//	foreach (Image<Gray, byte> faceimage in tmpList[name])
						//	{
						//		string personFullDir = saveToDir + "\\" + name;
						//		if (!Directory.Exists(personFullDir))
						//			Directory.CreateDirectory(personFullDir);
						//		faceimage.ToBitmap().Save(personFullDir + "\\Face" + " (" + counter++ + ").bmp");
						//	}
					}
				}
				else if (args[1].Equals(UrlHandlerArgument, StringComparison.InvariantCultureIgnoreCase))
				{
					bool commandRecognized = false;
					string expectedStartString = UrlHandlerUriStart + ":";
					if (args[2].StartsWith(expectedStartString, StringComparison.InvariantCultureIgnoreCase))
					{
						string commandString = args[2].Substring(expectedStartString.Length);
						commandString = Uri.UnescapeDataString(commandString);
						if (commandString == "show")
						{
							commandRecognized = true;
							if (mainForm != null && !IsFirstInstance)
								mainForm.ShowAndActivateMainWindow();
						}
						else if (commandString.StartsWith("selectfile/", StringComparison.InvariantCultureIgnoreCase))
						{
							commandRecognized = true;
							Process.Start("explorer", "/select, \"" + commandString.Substring("selectfile/".Length) + "\"");
						}
					}

					if (!commandRecognized)
						UserMessages.ShowWarningMessage("QuickAccess cannot process unknown uri command: " + args[2]);
				}
			}
		}

		public class SingleInstanceApplication : WindowsFormsApplicationBase
		{
			private SingleInstanceApplication()
			{
				base.IsSingleInstance = true;
			}

			public Form GetMainForm() { return MainForm; }

			public static void Run(StartupNextInstanceEventHandler startupHandler)
			{
				using (Mutex mutex = new Mutex(false, "QuickAccess-{6EBAC5AC-BCF2-4263-A82C-F189930AEA30}"))
				{
					SingleInstanceApplication app = new SingleInstanceApplication();
					app.StartupNextInstance += startupHandler;

					if (mutex.WaitOne(0, true))
					{
						app.MainForm = new Form1();
						ProcessCommandLineArguments(app.MainForm as Form1, Environment.GetCommandLineArgs(), true);
					}
					app.Run(Environment.GetCommandLineArgs());
				}
			}
		}

		private static void AssociateFacesFileExtensionInRegistry()
		{
			RegistryInterop.AddCommandToFileTypeHandlerAndAddExstensionListToHandler(
				new List<string>() { ".faces" },
				"Faces file",
				"File containing faces (used for face detection)",
				@"%SystemRoot%\System32\shell32.dll,170",
				"ExtractFaces",
				"\"" + Environment.GetCommandLineArgs()[0] + "\"" + " \"" + "extract" + "\" " + "\"%1\"",//In format QuickAccess.exe extract "filepath..\to..\extra\face.faces"
				"Extract faces here",
				"\"" + Environment.GetCommandLineArgs()[0] + "\"");
		}

		private static void AssociateUrlProtocolHandler()
		{
			var classesRootKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryInterop.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
			var quickaccessKey = classesRootKey.CreateSubKey(UrlHandlerUriStart);
			quickaccessKey.SetValue(null, "URL:QuickAccess Protocol");
			quickaccessKey.SetValue("URL Protocol", "");
			var shellSubkey = quickaccessKey.CreateSubKey("shell");
			var openSubkey = shellSubkey.CreateSubKey("open");
			var commandSubkey = openSubkey.CreateSubKey("command");
			commandSubkey.SetValue(null, "\"" + Environment.GetCommandLineArgs()[0] + "\" " + UrlHandlerArgument + " \"%1\"");
		}
	}
}
