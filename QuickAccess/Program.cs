using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using SharedClasses;

//This line is supposed to prevent dissasembly with ILDASM, but it does not prevent dotPeek
[assembly: SuppressIldasmAttribute()]

namespace QuickAccess
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			AssociateFacesFileExtensionInRegistry();

			string[] args = Environment.GetCommandLineArgs();

			if (args.Length == 3)
			{
				if (args[1] == null)
					return;

				if (args[1].ToLower() == "extract")
				{
					if (File.Exists(args[2]))
					{
						string saveToDir = Path.GetDirectoryName(args[2]) + "\\ExtractedFaces";
						if (!Directory.Exists(saveToDir))
							Directory.CreateDirectory(saveToDir);

						int counter = 1;
						Dictionary<string, List<Image<Gray, byte>>> tmpList = new Dictionary<string, List<Image<Gray, byte>>>();
						FaceDetectionInterop.ExtractFacesFromFile(FaceDetectionInterop.Passphrase, FaceDetectionInterop.Salt, ref tmpList, args[2]);
						foreach (string name in tmpList.Keys)
							foreach (Image<Gray, byte> faceimage in tmpList[name])
							{
								string personFullDir = saveToDir + "\\" + name;
								if (!Directory.Exists(personFullDir))
									Directory.CreateDirectory(personFullDir);
								faceimage.ToBitmap().Save(personFullDir + "\\Face" + " (" + counter++ + ").bmp");
							}
					}
				}
			}
			else if (args.Length == 1)
			{
				//QuickAccess-{6EBAC5AC-BCF2-4263-A82C-F189930AEA30}
				//Mutex.

				using (Mutex mutex = new Mutex(false, "QuickAccess-{6EBAC5AC-BCF2-4263-A82C-F189930AEA30}"))
				{
					if (!mutex.WaitOne(0, true))
					{
						Application.EnableVisualStyles();
						MessageBox.Show("Another instances of QuickAccess is already running.", "Only one instance allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else
					{
						Application.EnableVisualStyles();
						Application.SetCompatibleTextRenderingDefault(false);

						if (SharedClasses.FaceDetectionInterop.CheckFaceDetectionDllsExistInCurrentExeDir(true)
							|| UserMessages.Confirm("Due to missing DLLs, application will not be able to do online publishing, continue withouth this support?"))
							Application.Run(new Form1());
					}
				}

				//Application.EnableVisualStyles();
				//Application.SetCompatibleTextRenderingDefault(false);
				//Application.Run(new Form1());
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
	}
}
