﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
//Facedetection disabled for now
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

		//private static bool alreadySetUnhandledExceptionHandler = false;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			//if (!alreadySetUnhandledExceptionHandler)
			//{
			//    alreadySetUnhandledExceptionHandler = true;
			//    //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);//Declares exception handler in Form1 constructor
			//    Application.ThreadException += (snder, evt) =>
			//    {
			//        ApplicationRecoveryAndRestart.UnhandledExceptions.Add(evt.Exception);
			//    };
			//}

			/*Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			AssociateFacesFileExtensionInRegistry();
			RegistryInterop.AssociateUrlProtocolHandler(UrlHandlerUriStart, "QuickAccess protocol", "\"" + Environment.GetCommandLineArgs()[0] + "\" " + UrlHandlerArgument + " \"%1\"");
			AutoUpdating.CheckForUpdates_ExceptionHandler(delegate
			{
				MainForm.CurrentVersionString = AutoUpdating.GetThisAppVersionString();
			});*/
			/*AutoUpdating.CheckForUpdates(
				//AutoUpdatingForm.CheckForUpdates(
				//    exitApplicationAction: () => Application.Exit(),
				ActionIfUptoDate_Versionstring: versionstring => Form1.CurrentVersionString = versionstring//,
				//ActionIfUnableToCheckForUpdates: errmsg => Form1.ErrorMessageIfCannotCheckVersion = errmsg);
				);*/

			Application.SetCompatibleTextRenderingDefault(false);//Must do it here before the next code section, otherwise it crashes
			SingleInstanceApplicationManager<MainForm>.CheckIfAlreadyRunningElseCreateNew(
				(onSecondInstanceArgs, onSecondInstanceMainFormFromFirstInstance) =>
				{
					ProcessCommandLineArguments(onSecondInstanceMainFormFromFirstInstance, onSecondInstanceArgs.CommandLineArgs, false);
				},
				(onFirstInstanceArgs, onAppStartFirstInstanceForm) =>
				{
					Application.EnableVisualStyles();
					AssociateFacesFileExtensionInRegistry();
					//RegistryInterop.AssociateUrlProtocolHandler(UrlHandlerUriStart, "QuickAccess protocol", "\"" + Environment.GetCommandLineArgs()[0] + "\" " + UrlHandlerArgument + " \"%1\"");
					AutoUpdating.CheckForUpdates_ExceptionHandler(delegate
					{
						MainForm.CurrentVersionString = AutoUpdating.GetThisAppVersionString();
					});
					Application.Run(onAppStartFirstInstanceForm);
				});

			//SingleInstanceApplication.Run(NewInstanceHandler);
		}

		public static void NewInstanceHandler(object sender, StartupNextInstanceEventArgs e)
		{
			e.BringToForeground = false;

			MainForm form1 = null;
			var app = sender as SingleInstanceApplication;
			if (app != null)
				form1 = app.GetMainForm() as MainForm;

			ProcessCommandLineArguments(form1, e.CommandLine.ToArray(), false);
		}

		public static void ProcessCommandLineArguments(MainForm mainForm, string[] args, bool IsFirstInstance)
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

						//Facedetection disabled for now
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
						app.MainForm = new MainForm();
						ProcessCommandLineArguments(app.MainForm as MainForm, Environment.GetCommandLineArgs(), true);
					}
					app.Run(Environment.GetCommandLineArgs());
				}
			}
		}

		private static void AssociateFacesFileExtensionInRegistry()
		{
			int followingCodeTakenOutNeedsRewrite;
			//RegistryInterop.AddCommandToFileTypeHandlerAndAddExstensionListToHandler(
			//    new List<string>() { ".faces" },
			//    "Faces file",
			//    "File containing faces (used for face detection)",
			//    @"%SystemRoot%\System32\shell32.dll,170",
			//    "ExtractFaces",
			//    "\"" + Environment.GetCommandLineArgs()[0] + "\"" + " \"" + "extract" + "\" " + "\"%1\"",//In format QuickAccess.exe extract "filepath..\to..\extra\face.faces"
			//    "Extract faces here",
			//    "\"" + Environment.GetCommandLineArgs()[0] + "\"",
			//    (err, title) => UserMessages.ShowErrorMessage(err, title));
		}
	}
}
