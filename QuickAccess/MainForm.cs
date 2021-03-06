﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
//using Microsoft.WindowsAPICodePack.Taskbar;
using System.Windows.Forms.Integration;
using System.Threading;
//using UnhandledExceptions;
using PropertyInterceptor;
using DynamicDLLsInterop;
//using InterfaceForQuickAccessPlugin;
using ICommandWithHandler = InlineCommandToolkit.InlineCommands.ICommandWithHandler;//InlineCommands.CommandsManagerClass.ICommandWithHandler;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;
using SharedClasses;
using System.Collections.ObjectModel;
using System.Windows.Media;
using QuickAccessPluginCreator;
using System.Net;
using System.Web.UI;
//using QuickAccess.TestEmguCV;
//using Emgu.CV;
//using Emgu.CV.Structure;
//using Emgu.CV.UI;

namespace QuickAccess
{
	public partial class MainForm : Form
	{
		private CommandsWindow commandsWindow;
		//private tmpCommandsWindow tmpCommandsWindow1;

		//private static string ThisAppName = "QuickAccess";
		private double origOpacity;
		private bool ScrollTextHistoryOnUpDownKeys = false;

		//MouseHooks.MouseHook mouseHook;
		//OverlayForm overlayForm = new OverlayForm();
		//OverlayWindow overlayWindow = new OverlayWindow();

		//Color LabelColorRequiredArgument = Color.Green;
		//Color LabelColorOptionalArgument = SystemColors.WindowText;
		System.Windows.Media.Color LabelColorRequiredArgument = System.Windows.Media.Colors.White;
		System.Windows.Media.Color LabelColorOptionalArgument = System.Windows.Media.Colors.Black;

		Icon originalTrayIcon = null;
		public static string CurrentVersionString = null;
		public static string ErrorMessageIfCannotCheckVersion = null;

		public MainForm()
		{
			////DynamicDLLs.InvokeDllMethodGetReturnObject(@"D:\Francois\Dev\VSprojects\TestDynamicDllLoadingInQuickAccess\TestDynamicDllLoadingInQuickAccess\bin\Debug\TestDynamicDllLoadingInQuickAccess.dll", "TestClass", "ShowMessage", null);
			//object obj = DynamicDLLs.InvokeDllMethodGetReturnObject(@"D:\Francois\Dev\VSprojects\TestDynamicDllLoadingInQuickAccess\TestDynamicDllLoadingInQuickAccess\bin\Debug\TestDynamicDllLoadingInQuickAccess.dll", "TestClass", "ShowMessage", null);
			//if (obj == null) UserMessages.ShowWarningMessage("NULL object returned from dynamic dll");
			//else UserMessages.ShowInfoMessage("Object returned from dynamic dll is of type = " + obj.GetType().ToString() + Environment.NewLine +
			//	(obj is Color ? "Color = " + ((Color)obj).ToKnownColor().ToString() : ""));

			//AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			InitializeComponent();

			System.Windows.Forms.Timer timerVersionString = new System.Windows.Forms.Timer();
			timerVersionString.Interval = 1000;
			timerVersionString.Tick +=
			(s, e) =>
			{
				if (CurrentVersionString != null && commandsWindow != null)
				{
					commandsWindow.Title += " (up to date version " + CurrentVersionString + ")";
					System.Windows.Forms.Timer t = s as System.Windows.Forms.Timer;
					t.Stop();
					t.Dispose();
					t = null;
				}
				else if (ErrorMessageIfCannotCheckVersion != null && commandsWindow != null)
				{
					commandsWindow.Title += " (" + ErrorMessageIfCannotCheckVersion + ")";
					System.Windows.Forms.Timer t = s as System.Windows.Forms.Timer;
					t.Stop();
					t.Dispose();
					t = null;
				}
			};
			timerVersionString.Start();

			originalTrayIcon = notifyIcon1.Icon;

			//textBoxCommand.Tag = new List<string>();
			comboboxCommand.Tag = new List<string>();
			//InlineCommands.InlineCommands.PopulateCommandList();

			origOpacity = this.Opacity;
			this.Opacity = 0;

			notifyIcon1.ContextMenu = contextMenu_TrayIcon;

			//mouseHook = new MouseHooks.MouseHook();
			////mouseHook.MouseGestureEvent += (o, gest) => { if (gest.MouseGesture == Win32Api.MouseGestures.RL) UserMessages.ShowErrorMessage("Message"); };
			//mouseHook.MouseMoveEvent += delegate
			//{
			//  if (MousePosition.X < 5 && MousePosition.Y < Screen.FromPoint(new Point(0, 0)).WorkingArea.Bottom - 50)
			//  {
			//    //ShowOverlayCommandWindows();
			//    //ShowOverlayRibbon();
			//  }
			//};
			//if (!Debugger.IsAttached)
			//  mouseHook.Start();
			//else
			//  notifyIcon1.ShowBalloonTip(3000, "Mousehook", "Mousehook not started due to debugging mode", ToolTipIcon.Info);

			//overlayWindow.IsVisibleChanged += delegate
			//{
			//	if (overlayWindow.IsVisible) overlayRibbon.Hide();
			//	else overlayRibbon.Show();
			//};
			//ShowOverlayCommandWindows(true);

			//textFeedback += (tag, evtargs) => { Logging.appendLogTextbox_OfPassedTextbox(textBox_Messages, evtargs.FeedbackText); };
			//progressChanged += (tag, evtargs) => { UpdateProgress(evtargs.CurrentValue, evtargs.MaximumValue, evtargs.BytesPerSecond); };

			//SubversionInterop.CheckStatusAllVisualStudio2010Projects();

			//SharedClassesSettings.EnsureAllSharedClassesSettingsNotNullCreateDefault();

			//VisualStudioInteropSettings.UriProtocol? up = VisualStudioInteropSettings.Instance.UriProtocolForVsPublishing;
			//string s = VisualStudioInteropSettings.Instance.BaseUri;
			//VisualStudioInteropSettings.Instance.BaseUri = "sdfsd";
			//int? i = NetworkInteropSettings.Instance.ServerSocket_ReceiveBufferSize;
			//bool? b = NetworkInteropSettings.Instance.ServerSocket_NoDelay;
			//VisualStudioInteropSettings.Instance.OnPropertySet
			//string p = VisualStudioInteropSettings.Instance.FtpPassword;
			//p = VisualStudioInteropSettings.Instance.FtpPassword;
			//string f = VisualStudioInteropSettings.Instance.FtpUsername;
			//VisualStudioInteropSettings.UriProtocol? up = VisualStudioInteropSettings.Instance.UriProtocolForVsPublishing;

			//MessageBox.Show("FtpPassword = " + GlobalSettings.VisualStudioInteropSettings.Instance.FtpPassword);

			UserMessages.iconForMessages = this.Icon;//IconsInterop.IconToImageSource(this.Icon);

			DeleteAllDropboxConfilctsOfVsProjects();

			//StartMonitoringForSourceCodeVersionedFileChanges();
			//StartAppsmonitorWebUi();

			//UserMessages.Confirm("Hallo");
			//System.Windows.Window w = UserMessages.GetTopmostForm();
			//w.Close();
			//w = null;

			//if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "InlineCommandToolkit.dll"))
			//	DynamicDLLs.LoadPlugin(AppDomain.CurrentDomain.BaseDirectory + "InlineCommandToolkit.dll");
			//if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "System.Windows.Controls.Input.Toolkit.dll"))
			//	DynamicDLLs.LoadPlugin(AppDomain.CurrentDomain.BaseDirectory + "System.Windows.Controls.Input.Toolkit.dll");

			//inlineCommandsWindowWPF = new InlineCommandsWindowWPF(this);
			//inlineCommandsWindowWPF.Closed += delegate { this.Close(); };
			//tmpCommandsWindow1 = new tmpCommandsWindow();
			//tmpCommandsWindow1.Closed += delegate { this.Close(); };

			//MessageBox.Show(AppDomain.MonitoringIsEnabled.ToString());

			//int a = 1;
			//double i = 1 / (1 - a);

			//StaticPropertyInterceptor.RunMethodAndBypassUserPromptForGetMethods(
			//	delegate
			//	{
			//EnsureAllSettingsAreNotNull();

			//GenericSettings.EnsureAllSettingsAreInitialized();

			//GenericSettings.ShowAndEditAllSettings();
			//GenericSettings.ShowAndEditAllSettings();


			//});

			//string s = InputBoxWPF.Prompt("Hallo");
			//string p = InputBoxWPF.Prompt("Please enter password", IsPassword: true);
			//MessageBox.Show("Password was = " + p);

			//DynamicDLLsInterop.DynamicDLLs.LoadPluginsInDirectory(@"D:\Francois\Dev\VSprojects\QuickAccessPlugins\ShowMessagePlugin\bin\Debug");
			//DynamicDLLsInterop.DynamicDLLs.LoadPluginsInDirectory(@"D:\Francois\Dev\VSprojects\QuickAccessPlugins\ShowNotificationPlugin\bin\Debug");
			//DynamicDLLsInterop.DynamicDLLs.LoadPluginsInDirectory(@"D:\Francois\Dev\VSprojects\QuickAccessPlugins\RunCommandPlugin\bin\Debug");
			//DynamicDLLsInterop.DynamicDLLs.LoadPluginsInDirectory(@"D:\Francois\Dev\VSprojects\QuickAccessPlugins\GoogleSearchCommandPlugin\bin\Debug");
			//DynamicDLLsInterop.DynamicDLLs.LoadPluginsInDirectory(@"D:\Francois\Dev\VSprojects\QuickAccessPlugins\bin\Debug", 5000);

			//foreach (IQuickAccessPluginInterface qai in DynamicDLLs.PluginList)
			//	if (qai.GetType().GetInterface(typeof(ICommandWithHandler).Name) != null)
			//	{
			//		OverrideToStringClass comm = (OverrideToStringClass)qai.GetType().GetConstructor(new Type[0]).Invoke(new object[0]);
			//		MessageBox.Show(comm.DisplayName);
			//	}
			//MessageBox.Show((qai as ICommandWithHandler).CommandName);

			//foreach (IQuickAccessPluginInterface plugin in DynamicDLLsInterop.DynamicDLLs.PluginList)
			//	plugin.Rundefault();
			//SharedClasses.DebugInterop.Assert(1 != 1, "1==1");

			//OverlayGestures og = new OverlayGestures();
			//og.ShowDialog();

			//TextFeedbackEventArgs.RaiseTextFeedbackEvent_Ifnotnull(null, inlineCommandsWindowWPF.textFeedbackEvent, "Loading plugins...", TextFeedbackType.Subtle);
			//System.Windows.Forms.Application.DoEvents();
			//if (!AppDomain.CurrentDomain.BaseDirectory.ToLower().Contains(@"QuickAccess\QuickAccess\bin".ToLower()))
			//	DynamicDLLs.LoadPluginsInDirectory(System.AppDomain.CurrentDomain.BaseDirectory + @"Plugins");
			//else
			//	foreach (string pluginProjectBaseDir in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\\Visual Studio 2010\Projects\QuickAccess", "*Plugin"))
			//		DynamicDLLs.LoadPluginsInDirectory(pluginProjectBaseDir + @"\bin\Release");
			//TextFeedbackEventArgs.RaiseTextFeedbackEvent_Ifnotnull(null, textFeedbackEvent, "Done loading plugins.", TextFeedbackType.Noteworthy);

			//inlineCommandsWindowWPF.GetInlineCommandsUserControl().LoadPlugins();
		}

		private static byte[] GetFavicon()
		{
			const string filepath = @"C:\Francois\Dev\VSprojects\ApplicationManager\ApplicationManager\app.ico";
			return File.ReadAllBytes(filepath);
			//StreamReader streamReader = new StreamReader(filepath);
			//string responseString = streamReader.ReadToEnd();
			//return Encoding.UTF8.GetBytes(responseString);
		}

		private static void RenderTableStartWithColumns(HtmlTextWriter writer, IEnumerable<string> columnNames)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Table);

			writer.RenderBeginTag(HtmlTextWriterTag.Thead);
			foreach (var colname in columnNames)
			{
				writer.RenderBeginTag(HtmlTextWriterTag.Th);
				writer.Write(colname);
				writer.RenderEndTag();//Th
			}
			writer.RenderEndTag();//Thead
		}

		private static void RenderTableRow(HtmlTextWriter writer, IEnumerable<string> rowCells)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);

			foreach (var cell in rowCells)
			{
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				writer.Write(cell);
				writer.RenderEndTag();//Td
			}

			writer.RenderEndTag();//Tr
		}

		private static List<RunningApps> GetRunningApps()
		{
			List<RunningApps> runningApps = new List<RunningApps>();

			Dictionary<string, KeyValuePair<string, int>> pathAndNameWithCount = new Dictionary<string, KeyValuePair<string, int>>();
			var procs = Process.GetProcesses();
			foreach (var p in procs)
			{
				try
				{
					if (p.MainModule != null)
						if (!pathAndNameWithCount.ContainsKey(p.MainModule.FileName))
							pathAndNameWithCount.Add(p.MainModule.FileName, new KeyValuePair<string, int>(p.ProcessName, 1));
						else
							pathAndNameWithCount[p.MainModule.FileName] = new KeyValuePair<string, int>(
								pathAndNameWithCount[p.MainModule.FileName].Key,
								pathAndNameWithCount[p.MainModule.FileName].Value + 1);
				}
				catch { }
			}

			foreach (var path in pathAndNameWithCount.Keys)
				runningApps.Add(new RunningApps(pathAndNameWithCount[path].Key, path, pathAndNameWithCount[path].Value));

			runningApps = runningApps.OrderBy(ra => ra.AppName).ToList();
			return runningApps;
		}

		private static byte[] GetResponseHtml()
		{
			int todo;
			//Instead of doing this on http request, rather maybe do it on a timer, say every minute and cache the result
			//So then when we get the html response, there is no delay and also not simultaneous calls which calculates this
			//This will speed it up a lot

			// Initialize StringWriter instance.
			var stringWriter = new StringWriter();

			// Put HtmlTextWriter in using block because it needs to call Dispose.
			using (var writer = new HtmlTextWriter(stringWriter))
			{
				RenderTableStartWithColumns(writer, new List<string>()
				{
					"Application",
					"Path",
					"Status",
					"Process count"
				});

				writer.RenderBeginTag(HtmlTextWriterTag.Tbody);
				foreach (var app in GetRunningApps())
					RenderTableRow(writer, new List<string>()
					{
						app.AppName,
						app.AppPath,
						"Undeterminate",
						app.Count.ToString()
					});
				writer.RenderEndTag();//Tbody

				writer.RenderEndTag();//Table
			}

			return Encoding.UTF8.GetBytes(stringWriter.ToString());
		}

		private static void OnClientConnectionCallback(IAsyncResult result)
		{
			var listener = (HttpListener)result.AsyncState;
			// Call EndGetContext to complete the asynchronous operation.
			var context = listener.EndGetContext(result);
			var request = context.Request;
			var response = context.Response;

			byte[] bytesToReturn = null;
			if (request.RawUrl.Equals("/myicon.ico", StringComparison.InvariantCultureIgnoreCase))
			{
				response.ContentType = "image/x-icon";
				bytesToReturn = GetFavicon();
			}
			else
				bytesToReturn = GetResponseHtml();

			if (bytesToReturn == null) return;

			response.ContentLength64 = bytesToReturn.Length;
			response.StatusCode = 200;
			var output = response.OutputStream;
			output.Write(bytesToReturn, 0, bytesToReturn.Length);
			output.Close();
		}

		private static bool mustStop = false;
		private static void StartAppsmonitorWebUi()
		{
			ThreadingInterop.DoAction(
				delegate
				{
					const int portnum = 9099;
					var listener = new HttpListener();
					listener.Prefixes.Add(string.Format("http://+:{0}/", portnum));
					listener.Start();
					//actionOnStatus("Running server on port " + portnum);
					while (true)
					{
						//Each connection will be handled with ListenerCallback, then will wait again for new connection
						try
						{
							var result = listener.BeginGetContext(new AsyncCallback(OnClientConnectionCallback),
																listener);
							result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
							//Only wait a second then retry, so we dont hang forever
						}
						catch
						{
						}
						if (mustStop)
							break;
					}
				},
				false);
		}

		private static void DeleteAllDropboxConfilctsOfVsProjects()
		{
			var vsprojectsDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).TrimEnd('\\') + @"\Dropbox\Dev\VSprojects";
			if (Directory.Exists(vsprojectsDir))
				foreach (var f in Directory.GetFiles(vsprojectsDir, "* conflicted copy *", SearchOption.AllDirectories))
					if (f.IndexOf(@"\bin\", StringComparison.InvariantCultureIgnoreCase) != -1)
						File.Delete(f);
		}

		private FileSystemWatcher _csharpVersionedFilesMonitor = null;
		private FileSystemWatcher _delphiVersionedFilesMonitor = null;

		private const string CcSharpMonitoredDirectory = @"C:\Francois\Dev\VSprojects";
		private const string CDelphiMonitoredDirectory = @"C:\Programming";

		private readonly List<string> _changedVisualStudioProjectDirectories = new List<string>();
		private readonly List<string> _changedDelphiProjectDirectories = new List<string>();
		private readonly Dictionary<int, Process> _runningVisualStudioProcesses = new Dictionary<int, Process>();
		private readonly Dictionary<int, Process> _runningDelphiProcesses = new Dictionary<int, Process>();

		private void StartMonitoringForSourceCodeVersionedFileChanges()
		{
			if (Directory.Exists(CcSharpMonitoredDirectory))
			{
				_csharpVersionedFilesMonitor = new FileSystemWatcher(CcSharpMonitoredDirectory);
				_csharpVersionedFilesMonitor.EnableRaisingEvents = true;
				_csharpVersionedFilesMonitor.IncludeSubdirectories = true;
				_csharpVersionedFilesMonitor.Changed += (sn, ev) => { OnFileModifiedCheckIfCSharpVersioned(ev.FullPath); };
			}

			if (Directory.Exists(CDelphiMonitoredDirectory))
			{
				_delphiVersionedFilesMonitor = new FileSystemWatcher(CDelphiMonitoredDirectory);
				_delphiVersionedFilesMonitor.EnableRaisingEvents = true;
				_delphiVersionedFilesMonitor.IncludeSubdirectories = true;
				_delphiVersionedFilesMonitor.Changed += (sn, ev) => { OnFileModifiedCheckIfDelphiVersioned(ev.FullPath); };
			}

			//int removeline;
			PopulateRunningVisualStudioProcesses();
		}

		DateTime lastProcessCheckTime = DateTime.MinValue;
		private void PopulateRunningVisualStudioProcesses()
		{
			DateTime now = DateTime.Now;
			if (now.Subtract(lastProcessCheckTime).TotalSeconds < 10)
				return;//Do not do more often than every 10 seconds

			var allprocs = Process.GetProcessesByName("VCSExpress");
			if (allprocs.Length > 0)
			{
				for (int i = 0; i < allprocs.Length; i++)
				{
					try
					{
						int pid = allprocs[i].Id;
						if (_runningVisualStudioProcesses.ContainsKey(pid))
							continue;
						_runningVisualStudioProcesses.Add(pid, allprocs[i]);
						ThreadingInterop.PerformOneArgFunctionSeperateThread<Process>(
							(proc) =>
							{
								proc.WaitForExit();
								_runningVisualStudioProcesses.Remove(proc.Id);
								if (_runningVisualStudioProcesses.Count == 0)
									AllVisualStudioIDEsClosed_PromptUserIfSubversionChanges();
							},
							allprocs[i],
							false);
					}
					catch
					{
					}
				}
			}

			//Delphi
			allprocs = Process.GetProcessesByName("bds");
			if (allprocs.Length > 0)
			{
				for (int i = 0; i < allprocs.Length; i++)
				{
					try
					{
						int pid = allprocs[i].Id;
						if (_runningDelphiProcesses.ContainsKey(pid))
							continue;
						_runningDelphiProcesses.Add(pid, allprocs[i]);
						ThreadingInterop.PerformOneArgFunctionSeperateThread<Process>(
							(proc) =>
							{
								proc.WaitForExit();
								_runningDelphiProcesses.Remove(proc.Id);
								if (_runningDelphiProcesses.Count == 0)
									AllDelphiIDEsClosed_PromptUserIfSubversionChanges();
							},
							allprocs[i],
							false);
					}
					catch
					{
					}
				}
			}
		}

		private void OnFileModifiedCheckIfCSharpVersioned(string path)
		{
			string projectRootFolder = Path.Combine(
				CcSharpMonitoredDirectory,
				path.Substring(CcSharpMonitoredDirectory.Length + 1).TrimStart('\\').Split('\\')[0]);//Just the project folder name
			if (DirIsValidSvnPath(projectRootFolder))
			{
				if (!_changedVisualStudioProjectDirectories.Contains(projectRootFolder))
					_changedVisualStudioProjectDirectories.Add(projectRootFolder);
				PopulateRunningVisualStudioProcesses();
			}
		}

		private void OnFileModifiedCheckIfDelphiVersioned(string path)
		{
			string projectRootFolder = Path.Combine(
				CDelphiMonitoredDirectory,
				path.Substring(CDelphiMonitoredDirectory.Length + 1).TrimStart('\\').Split('\\')[0]);//Just the project folder name
			if (DirIsValidSvnPath(projectRootFolder))
			{
				if (!_changedDelphiProjectDirectories.Contains(projectRootFolder))
					_changedDelphiProjectDirectories.Add(projectRootFolder);
				PopulateRunningVisualStudioProcesses();
			}
		}

		private void AllVisualStudioIDEsClosed_PromptUserIfSubversionChanges()
		{
			UserMessages.ShowWarningMessage("The following subversion projects were modified (the last Visual Studio C# IDE was closed):"
				+ Environment.NewLine + Environment.NewLine
				+ string.Join(Environment.NewLine, _changedVisualStudioProjectDirectories));
		}

		private void AllDelphiIDEsClosed_PromptUserIfSubversionChanges()
		{
			UserMessages.ShowWarningMessage("The following subversion projects were modified (the last Delphi IDE was closed):"
				+ Environment.NewLine + Environment.NewLine
				+ string.Join(Environment.NewLine, _changedDelphiProjectDirectories));
		}

		private bool DirIsValidSvnPath(string dir)
		{
			if (!Directory.Exists(dir))
				return false;
			return Directory.Exists(System.IO.Path.Combine(dir, ".svn"));
		}

		/*private void StartPipeClient()
		{
			NamedPipesInterop.NamedPipeClient pipeclient = null;
			pipeclient = NamedPipesInterop.NamedPipeClient.StartNewPipeClient(
			ActionOnError: (e) => { Console.WriteLine("Error occured: " + e.GetException().Message); },
			ActionOnMessageReceived: (m) =>
			{
				if (m.MessageType == PipeMessageTypes.AcknowledgeClientRegistration)
					Console.WriteLine("Client successfully registered.");
				else
				{
					if (m.MessageType == PipeMessageTypes.Show)
					{
						//if (this.InvokeRequired)
						//	this.Invoke((Action)delegate { ShowAndActivateMainWindow(1); });
						//else
						ShowAndActivateMainWindow(1);
					}
					else if (m.MessageType == PipeMessageTypes.Hide)
					{
						//if (this.InvokeRequired)
						//	this.Invoke((Action)delegate { this.WindowState = FormWindowState.Minimized; });
						//else
						MainWindow.Dispatcher.Invoke(delegate { MainWindow.Hide(); });
					}
					else if (m.MessageType == PipeMessageTypes.Close)
					{
						if (pipeclient != null)
							pipeclient.ForceCancelRetryLoop = true;
						this.notifyIcon1.Visible = false;
						if (this.InvokeRequired)
							this.Invoke((Action)delegate { this.Close(); });
						else
							this.Close();
					}
				}
			});
		}*/

		//private void ShowForm()
		//{
		//	this.Show();
		//	if (this.WindowState == FormWindowState.Minimized)
		//		this.WindowState = FormWindowState.Normal;
		//	bool tmptopmost = this.TopMost;
		//	this.TopMost = true;
		//	Application.DoEvents();
		//	this.TopMost = tmptopmost;
		//	this.Activate();
		//}

		private void Form1_Load(object sender, EventArgs e)
		{
			ApplicationRecoveryAndRestart.RegisterForRecoveryAndRestart(
				delegate//On crash
				{
					this.Invoke((Action)delegate
					{
						notifyIcon1.Visible = false;
					});
					//File.WriteAllText(@"C:\Francois\Crash reports\tmpQuickAccess.log", "Application crashed, more details not incorporated yet." + DateTime.Now.ToString());
					//using (StreamWriter sw = new StreamWriter())
					//ApplicationRecoveryAndRestart.WriteCrashReportFile("QuickAccess");//, "Application crashed, more details not incorporated yet.");
				},
				delegate//On successfully restarted
				{
					//MessageBox.Show("Application successfully restarted from crash. No functionality incorporated yet.", "Restarted", MessageBoxButtons.OK, MessageBoxIcon.Information);
					if (Directory.Exists(ApplicationRecoveryAndRestart.CrashReportsDirectory))
					{
						MessageBox.Show("QuickAccess successfully restarted from crash. See Crash report.", "Restarted successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
						Process.Start("explorer", "\"" + ApplicationRecoveryAndRestart.CrashReportsDirectory + "\"");
					}
					else MessageBox.Show("QuickAccess successfully restarted from crash. Could not find Crash reports folder ("
						+ ApplicationRecoveryAndRestart.CrashReportsDirectory
						+ ").", "Successfully Restarted", MessageBoxButtons.OK, MessageBoxIcon.Information);
				},
				delegate
				{
					this.Invoke((Action)delegate
					{
						labelRecoveryAndRestartSafe.Visible = true;
						notifyIcon1.ShowBalloonTip(3000, "Recovery and Restart", "QuickAccess is now Recovery and Restart Safe", ToolTipIcon.Info);
					});
				});

			//StartPipeClient();
			WindowMessagesInterop.InitializeClientMessages();

			AddAllCurrentDomainAssembliesToLoadedList();

			commandsWindow = new CommandsWindow(this);
			CommandsWindow.actionForShowingAboutWindow = delegate
			{
				AboutWindow2.ShowAboutWindow(new System.Collections.ObjectModel.ObservableCollection<DisplayItem>()
				{
					new DisplayItem("Author", "Francois Hill"),
					new DisplayItem("Icon(s) obtained from", null)
				});
			};
			commandsWindow.Show();

			//Thread.Sleep(500);
			//int i = 1;
			//int j = 0;
			//var k = i / j;
			////throw new Exception("Hallo");

			commandsWindow.Hide();

			commandsWindow.MouseClickedRequestToOpenOverlayWindow += (snder, evtargs) =>
			{
				if (!evtargs.WasRightClick)
					ShowAndActivateMainWindow(evtargs.ScalingFactor);
				else
				{
					ShowOverlayGesturesWindow();
				}
			};
			commandsWindow.Closed += delegate { this.Close(); };
			commandsWindow.GetCommandsUsercontrol().CommandPropertyChangedEvent += new System.ComponentModel.PropertyChangedEventHandler(CommandsUsercontrol_OnCommandPropertyChanged);
			//bool MustStillAddOverlayToTRAYICON;

			ShowOverlayRibbonMain();

			//System.Windows.Forms.Application.DoEvents();
			//if (!AppDomain.CurrentDomain.BaseDirectory.ToLower().Contains(@"QuickAccess\QuickAccess\bin".ToLower()))
			//	DynamicDLLs.LoadPluginsInDirectory(System.AppDomain.CurrentDomain.BaseDirectory + @"Plugins");
			//else
			//	foreach (string pluginProjectBaseDir in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\\Visual Studio 2010\Projects\QuickAccess", "*Plugin"))
			//		DynamicDLLs.LoadPluginsInDirectory(pluginProjectBaseDir + @"\bin\Release");
			//CommandsUsercontrol.LoadAllPlugins();
		}

		void CommandsUsercontrol_OnCommandPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (string.Compare(e.PropertyName, "NumberUnreadMessages", true) == 0)
			{
				//notifyIcon1.Icon = DynamicDLLs.HasUnreadMessages ? IconsInterop.OverlayIconWithCircle(notifyIcon1.Icon, System.Drawing.Brushes.Red, new Point(0, 0), 17) : originalTrayIcon;
				notifyIcon1.Icon = DynamicDLLs.HasUnreadMessages ?
					IconsInterop.OverlayIconWithFourCircles(
						notifyIcon1.Icon,
						DynamicDLLs.HasUnreadErrorMessages ? System.Drawing.Brushes.Red : System.Drawing.Brushes.Transparent,
						DynamicDLLs.HasUnreadSuccessMessages ? System.Drawing.Brushes.Green : System.Drawing.Brushes.Transparent,
						DynamicDLLs.HasUnreadNoteworhyMessages ? System.Drawing.Brushes.Purple : System.Drawing.Brushes.Transparent,
						DynamicDLLs.HasUnreadSubtleMessages ? System.Drawing.Brushes.Gray : System.Drawing.Brushes.Transparent)
					: originalTrayIcon;
			}
		}

		private static void AddAllCurrentDomainAssembliesToLoadedList()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				try
				{
					string assemblyLocation = assemblies[i].Location;
					DynamicDLLs.AllSuccessfullyLoadedDllFiles.Add(assemblyLocation);
				}
				catch (NotSupportedException) { }
				catch (Exception exc)
				{
					UserMessages.ShowErrorMessage("Unable to find assembly location: " + exc.Message);
				}
			}
		}

		//private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		//{
		//    if (!(e.ExceptionObject is Exception)) return;
		//    //UserMessages.ShowErrorMessage("An error unhandled by the application has occurred, application shutting down: " + exc.Message + Environment.NewLine + exc.StackTrace);
		//    //UnhandledExceptionsWindow uew = new UnhandledExceptionsWindow(e.ExceptionObject as Exception);
		//    UnhandledExceptionsWindow.ShowUnHandledException(e.ExceptionObject as Exception);
		//    this.Close();
		//}

		private void Form1_Shown(object sender, EventArgs e)
		{
			this.ShowInTaskbar = true;
			if (!Win32Api.RegisterHotKey(this.Handle, Win32Api.Hotkey1, Win32Api.MOD_CONTROL + Win32Api.MOD_SHIFT, (int)Keys.Q)) UserMessages.ShowErrorMessage("QuickAccess could not register hotkey Ctrl + Q");
			//if (!Win32Api.RegisterHotKey(this.Handle, Win32Api.Hotkey2, Win32Api.MOD_CONTROL + Win32Api.MOD_SHIFT, (int)Keys.Q)) UserMessages.ShowErrorMessage("QuickAccess could not register hotkey Ctrl + Shift + Q");
			//label1.Text = InlineCommands.InlineCommands.AvailableActionList;
			//SetAutocompleteActionList();
			//InitializeHooks(false, true);
			this.Hide();
			this.Opacity = origOpacity;

			if (Environment.CommandLine.ToLower().Contains(@"documents\visual studio 2010\")) buttonTestCrash.Visible = true;
		}

		private Point MousePositionBeforePopup = new Point(-1, -1);
		protected override void WndProc(ref Message m)
		{
			WindowMessagesInterop.MessageTypes mt;
			WindowMessagesInterop.ClientHandleMessage(m.Msg, m.WParam, m.LParam, out mt);
			if (mt == WindowMessagesInterop.MessageTypes.Show)
				ShowAndActivateMainWindow(1);
			else if (mt == WindowMessagesInterop.MessageTypes.Hide)
				MainWindow.Hide();
			else if (mt == WindowMessagesInterop.MessageTypes.Close)
				this.Close();
			else if (m.Msg == Win32Api.WM_HOTKEY)
			{
				if (m.WParam == new IntPtr(Win32Api.Hotkey1))
					ToggleWindowActivation();
				//if (m.WParam == new IntPtr(Win32Api.Hotkey2))
				//	ShowOverlayCommandWindows();
			}
			base.WndProc(ref m);
		}

		/*private void UpdateProgress(int currentValue, int maximumValue, double bytesPerSecond = -1)
		{
			ThreadingInterop.UpdateGuiFromThread(this, delegate
			{
				if (this.Visible)
				{
					//if (currentValue > maximumValue) return;
					if (currentValue < 0 || maximumValue < 0) return;
					if (progressBar1.Maximum != maximumValue) progressBar1.Maximum = maximumValue;
					if (progressBar1.Value != currentValue) progressBar1.Value = currentValue;
					//if (bytesPerSecond != -1 && labelBytesPerSecond.Text != Math.Round(bytesPerSecond, 0).ToString()) labelBytesPerSecond.Text = Math.Round(bytesPerSecond, 0).ToString();
					if ((currentValue == 0 || currentValue == 100) && maximumValue == 100)
						progressBar1.Visible = false;
					else if (!progressBar1.Visible)
						progressBar1.Visible = true;
					if (TaskbarManager.IsPlatformSupported)
					{
						TaskbarManager.Instance.SetProgressValue(currentValue, maximumValue);
						if ((currentValue == 0 || currentValue == 100) && maximumValue == 100)
							TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
						else
							TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
					}
					Application.DoEvents();
				}
			});
		}*/

		private bool EventAddedMouseClickedRequestToOpenOverlayWindow = false;
		private double? OriginalWidthOfMainWindow = null;
		private double? OriginalHeightOfMainWindow = null;
		OverlayRibbon overlayRibbonMain = new OverlayRibbon();
		OverlayGesturesForm overlayGestures;// = new OverlayGestures();
		private void ShowOverlayRibbonMain()
		{
			Rectangle workingArea = Screen.FromPoint(new Point(0, 0)).WorkingArea;
			overlayRibbonMain.Top = workingArea.Top + (workingArea.Height - overlayRibbonMain.ActualHeight) / 2 + 100;
			overlayRibbonMain.Left = 0;
			//MessageBox.Show(overlayRibbon.Left + ", " + overlayRibbon.Top);
			if (!EventAddedMouseClickedRequestToOpenOverlayWindow)
			{
				overlayRibbonMain.MouseClickedRequestToOpenOverlayWindow += (sndr, evtargs) =>
				{
					if (!evtargs.WasRightClick)
					{
						if (MainWindow.Visibility != System.Windows.Visibility.Visible)
							ShowAndActivateMainWindow(evtargs.ScalingFactor);
						else
							MainWindow.Hide();
					}
					else
					{
						ShowOverlayGesturesWindow();
					}
				};
				//overlayRibbonMain.MouseRightButtonUp += (tag, evtargs) =>
				//{
				//	evtargs.Handled = true;
				//	overlayGestures = new OverlayGestures();
				//	//overlayGestures.Closed += delegate { overlayGestures = null; };
				//	overlayGestures.ShowDialog();
				//	overlayGestures = null;
				//};
				EventAddedMouseClickedRequestToOpenOverlayWindow = true;
			}
			overlayRibbonMain.Show();
		}

		private void ShowOverlayGesturesWindow()
		{
			if (overlayGestures != null && overlayGestures.IsDisposed)
				overlayGestures = null;
			if (overlayGestures == null)
			{
				overlayGestures = new OverlayGesturesForm();
				overlayGestures.FormClosed += (snder, ea) =>
				{
					Form thisForm = snder as Form;
					if (!thisForm.IsDisposed)
						thisForm.Dispose();
					thisForm = null;
				};
			}
			////overlayGestures.Closed += delegate { overlayGestures = null; };
			overlayGestures.ShowDialog();
			//overlayGestures = null;
			//WindowsInterop.ShowAndActivateWindow(overlayGestures);
		}

		//OverlayRibbon overlayRibbonMain = new OverlayRibbon();

		/*private void ShowOverlayCommandWindows(bool JustCreateDoNotShow = false)
		{
			//if (overlayWindow == null) overlayWindow = new OverlayWindow();
			if (overlayWindow.Visibility != System.Windows.Visibility.Visible)
			{
				int tmpcounter = 0;
				foreach (string key in InlineCommands.InlineCommands.CommandList.Keys)
				{
					tmpcounter++;
					if (InlineCommands.InlineCommands.CommandList[key].commandUsercontrol != null)//.commandForm != null)
					{
						if (!overlayWindow.ListOfCommandUsercontrols.Contains(InlineCommands.InlineCommands.CommandList[key].commandUsercontrol))
							overlayWindow.ListOfCommandUsercontrols.Add(InlineCommands.InlineCommands.CommandList[key].commandUsercontrol);
					}
					else
					{
						CommandUserControl tmpCommandUsercontrol = new CommandUserControl(key);
						tmpCommandUsercontrol.labelShortcutKeyNumber.Content = (tmpcounter).ToString();

						InlineCommands.InlineCommands.CommandList[key].commandUsercontrol = tmpCommandUsercontrol;
						if (!overlayWindow.ListOfCommandUsercontrols.Contains(tmpCommandUsercontrol))
							overlayWindow.ListOfCommandUsercontrols.Add(tmpCommandUsercontrol);

						InlineCommands.InlineCommands.CommandDetails commandDetails = InlineCommands.InlineCommands.CommandList[key];

						tmpCommandUsercontrol.RemoveAndHideControls();

						if (commandDetails.CommandHasPredefinedArguments()) tmpCommandUsercontrol.ExpandTreeviewInput();
						else if (commandDetails.CommandHasArguments()) tmpCommandUsercontrol.ExpandCustomInputs();

						bool SetUsercontrolTagAsFirstTextbox = false;//Set the tag of the CommandUserControl as the first textbox to easily set focus to it
						if (commandDetails.CommandHasArguments() || commandDetails.CommandHasPredefinedArguments())
						{
							if (commandDetails.CommandHasArguments())
							{
								foreach (InlineCommands.InlineCommands.CommandDetails.CommandArgumentClass arg in commandDetails.commandArguments)
								{
									System.Windows.Controls.TextBox textBox = new System.Windows.Controls.TextBox()
									{
										//ForeColor = arg.Required ? Color.Red : Color.Green,
										Tag = commandDetails,
										MinWidth = 80,
										Margin = new System.Windows.Thickness(5),
										VerticalAlignment = System.Windows.VerticalAlignment.Top,
										UseLayoutRounding = true,
										IsUndoEnabled = true,
										IsReadOnlyCaretVisible = true,
										IsReadOnly = false
									};

									if (commandDetails.CommandHasPredefinedArguments())
									{
										//textBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
										//textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
										//textBox.AutoCompleteCustomSource.Clear();
										//foreach (string s in commandDetails.commandPredefinedArguments)
										//  textBox.AutoCompleteCustomSource.Add(s.Substring(s.IndexOf(' ') + 1));
									}

									textBox.GotFocus += (send, evtargs) => { (send as System.Windows.Controls.TextBox).Background = System.Windows.Media.Brushes.LightBlue; };
									textBox.LostFocus += (send, evtargs) => { (send as System.Windows.Controls.TextBox).Background = System.Windows.Media.Brushes.White; };
									textBox.KeyDown += (txtbox, evt) =>
									{
										//if (evt.KeyCode == Keys.Enter)
										if (evt.Key == System.Windows.Input.Key.Enter)
										{
											bool EmptyRequiredArguments = false;
											InlineCommands.InlineCommands.CommandDetails thisCommandDetails = (txtbox as System.Windows.Controls.TextBox).Tag as InlineCommands.InlineCommands.CommandDetails;
											foreach (InlineCommands.InlineCommands.CommandDetails.CommandArgumentClass argument in (thisCommandDetails).commandArguments)
											{
												if (argument.Required && argument.textBox.Text.Trim().Length == 0)
												{
													EmptyRequiredArguments = true;
													UserMessages.ShowWarningMessage("Please complete all required textboxes (green), empty textbox for " + argument.ArgumentName);
												}
											}

											if (!EmptyRequiredArguments)
											{
												string concatString = "";
												foreach (InlineCommands.InlineCommands.CommandDetails.CommandArgumentClass argument in thisCommandDetails.commandArguments)
													concatString += (concatString.Length > 0 ? ";" : "") + argument.textBox.Text;
												//UserMessages.ShowInfoMessage(commandDetails.commandName + " " + concatString);
												overlayWindow.Close();
												PerformCommandNow(commandDetails.commandName + " " + concatString, false, true);
											}
										}
									};

									arg.textBox = textBox;
									//tmpCommandForm.AddControl(arg.ArgumentName, textBox, arg.Required ? Color.Green : SystemColors.WindowText);
									//tmpCommandForm.AddControl(arg.ArgumentName, textBox, arg.Required ? LabelColorOptionalArgument : LabelColorRequiredArgument);

									tmpCommandUsercontrol.AddControl(arg.ArgumentName, textBox, arg.Required ? LabelColorOptionalArgument : LabelColorRequiredArgument);

									if (!SetUsercontrolTagAsFirstTextbox && !commandDetails.CommandHasPredefinedArguments())
									{
										tmpCommandUsercontrol.Tag = new OverlayWindow.OverlayChildManager(true, false, false, textBox);
										tmpCommandUsercontrol.currentFocusedElement = textBox;
										SetUsercontrolTagAsFirstTextbox = true;
									}
								}
							}

							if (commandDetails.CommandHasPredefinedArguments())
							{
								foreach (string predefinedCommands in commandDetails.commandPredefinedArguments)
								{
									System.Windows.Controls.TreeViewItem treeviewItem = new System.Windows.Controls.TreeViewItem()
									{
										Header = predefinedCommands.Substring(predefinedCommands.IndexOf(' ') + 1),//stackPanel,
										Tag = new object[] { predefinedCommands, commandDetails },//predefined string, commandDetails
										Margin = new System.Windows.Thickness(0,0,20,0)
									};
									treeviewItem.KeyDown += (send, evtargs) =>
									{
										if (evtargs.Key == System.Windows.Input.Key.Enter)
										{
											string predefinedArg = ((send as System.Windows.Controls.TreeViewItem).Tag as object[])[0].ToString();
											if (overlayWindow != null)// && !overlayWindow.IsDisposed)
												overlayWindow.Close();
											PerformCommandNow(predefinedArg, false, false);
										}
									};
									treeviewItem.MouseDoubleClick += (send, evtargs) =>
									{
										string predefinedArg = ((send as System.Windows.Controls.TreeViewItem).Tag as object[])[0].ToString();
										if (overlayWindow != null)// && !overlayWindow.IsDisposed)
											overlayWindow.Close();
										PerformCommandNow(predefinedArg, false, false);
									};
									treeviewItem.MouseRightButtonUp += (send, evtargs) =>
									{
										InlineCommands.InlineCommands.CommandDetails thisCommandDetails = ((send as System.Windows.Controls.TreeViewItem).Tag as object[])[1] as InlineCommands.InlineCommands.CommandDetails;
										string thisPredefinedArguments = ((send as System.Windows.Controls.TreeViewItem).Tag as object[])[0].ToString();
										//Something not working right here (see the MAIL command), does not concat the arguments into one string.
										//foreach (string s in thisPredefinedArguments.Substring(thisCommandDetails.commandName.Length + 1).Split(InlineCommands.InlineCommands.CommandDetails.ArgumentSeparator))
										//  MessageBox.Show(s;)
									};
									//treeviewItem.PreviewDragEnter += (tag, evtargs) => { evtargs.Handled = true; };
									//treeviewItem.PreviewDragOver += (tag, evtargs) => { evtargs.Handled = true; };
									//treeviewItem.PreviewDrop += (tag, evtargs) => { evtargs.Handled = true; };

									tmpCommandUsercontrol.AddTreeviewItem(treeviewItem);

									if (!SetUsercontrolTagAsFirstTextbox)
									{
										tmpCommandUsercontrol.Tag = new OverlayWindow.OverlayChildManager(true, false, false, treeviewItem);
										tmpCommandUsercontrol.currentFocusedElement = treeviewItem;
										SetUsercontrolTagAsFirstTextbox = true;
									}
								}
							}
						}
						else
						{
							//commanditem.Click += delegate
							//{
							//  UserMessages.ShowInfoMessage("No subcommands");
							//};
						}
						//menuItem_Commands.MenuItems.Add(commanditem);//.DropDownItems.Add(commanditem);
					}
				}
				//overlayWindow.Loaded += delegate { overlayWindow.SetupAllChildWindows(); };
				overlayWindow.Show();
				if (JustCreateDoNotShow) overlayWindow.Close();
				overlayWindow.SetupAllChildWindows();
				overlayWindow.AddEventsToAllChildUsercontrols();
				if (overlayWindow.currentActiveUsercontrol != null)
				{
					//overlayWindow.SetFocusToNewUsercontrol(overlayWindow.currentActiveUsercontrol);
					//overlayWindow.currentActiveUsercontrol.ActivateControl();
					//overlayWindow.currentActiveUsercontrol.currentFocusedElement.Focus();
					overlayWindow.SetFocusToNewUsercontrol(overlayWindow.currentActiveUsercontrol);
					overlayWindow.currentActiveUsercontrol.currentFocusedElement.Focus();
					Console.WriteLine("overlayWindow.currentActiveUsercontrol != null");
				}
				else
				{
					overlayWindow.SetFocusToNewUsercontrol(overlayWindow.ListOfCommandUsercontrols[0]);
					//Console.WriteLine("overlayWindow.currentActiveUsercontrol == null");
				}
			}
		}*/

		private bool IsAltDown()
		{
			return ((ModifierKeys & Keys.Alt) == Keys.Alt);
		}

		public static bool IsControlDown()
		{
			return ((ModifierKeys & Keys.Control) == Keys.Control);
		}

		private bool IsAltShiftDown()
		{
			return (ModifierKeys & (Keys.Alt | Keys.Shift)) == (Keys.Alt | Keys.Shift);
		}

		private bool IsControlShiftAltDown()
		{
			return (ModifierKeys & (Keys.Control | Keys.Shift | Keys.Alt)) == (Keys.Control | Keys.Shift | Keys.Alt);
		}

		private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			int CtrlMoveInterval = 100;
			if (IsAltShiftDown()) CtrlMoveInterval = 10;

			if (e.KeyCode == Keys.Escape)
			{
				if (comboboxCommand.Text.Length > 0) comboboxCommand.Text = "";
				else this.Hide();
				//if (textBoxCommand.Text.Length > 0) textBoxCommand.Text = "";
				//else this.Hide();
			}
			else if (IsControlShiftAltDown())
			{
				int ResizeInterval = 40;
				if (e.KeyCode == Keys.Down)
				{
					if (this.Bottom + ResizeInterval < Screen.FromHandle(this.Handle).WorkingArea.Bottom)
						this.Height += ResizeInterval;
					else this.Height += Screen.FromHandle(this.Handle).WorkingArea.Bottom - this.Bottom;
				}
				if (e.KeyCode == Keys.Up)
				{
					if (this.Height - ResizeInterval > this.MinimumSize.Height)
						this.Height -= ResizeInterval;
					else this.Height = this.MinimumSize.Height;
				}
				if (e.KeyCode == Keys.Left)
				{
					if (this.Width - ResizeInterval > this.MinimumSize.Width)
						this.Width -= ResizeInterval;
					else this.Width = this.MinimumSize.Width;
				}
				if (e.KeyCode == Keys.Right)
				{
					if (this.Right + ResizeInterval < Screen.FromHandle(this.Handle).WorkingArea.Right)
						this.Width += ResizeInterval;
					else this.Width += Screen.FromHandle(this.Handle).WorkingArea.Right - this.Right;
				}
			}
			else if (e.KeyCode == Keys.Down && (IsAltDown() || IsAltShiftDown()))
				MoveWindowDownByInterval(CtrlMoveInterval);
			else if (e.KeyCode == Keys.Up && (IsAltDown() || IsAltShiftDown()))
				MoveWindowUpByInterval(CtrlMoveInterval);
			else if (e.KeyCode == Keys.Left && (IsAltDown() || IsAltShiftDown()))
				MoveWindowLeftByInterval(CtrlMoveInterval);
			else if (e.KeyCode == Keys.Right && (IsAltDown() || IsAltShiftDown()))
				MoveWindowRightByInterval(CtrlMoveInterval);
		}

		private void MoveWindowRightByInterval(int CtrlMoveInterval)
		{
			if (this.Location.X + CtrlMoveInterval <= Screen.GetWorkingArea(this.Location).Right - this.Width)
				this.Location = new Point(this.Location.X + CtrlMoveInterval, this.Location.Y);
			else
				this.Location = new Point(Screen.GetWorkingArea(this.Location).Right - this.Width, this.Location.Y);
		}

		private void MoveWindowLeftByInterval(int CtrlMoveInterval)
		{
			if (this.Location.X - CtrlMoveInterval >= 0)
				this.Location = new Point(this.Location.X - CtrlMoveInterval, this.Location.Y);
			else
				this.Location = new Point(0, this.Location.Y);
		}

		private void MoveWindowUpByInterval(int CtrlMoveInterval)
		{
			if (this.Location.Y - CtrlMoveInterval >= 0)
				this.Location = new Point(this.Location.X, this.Location.Y - CtrlMoveInterval);
			else
				this.Location = new Point(this.Location.X, 0);
		}

		private void MoveWindowDownByInterval(int CtrlMoveInterval)
		{
			if (this.Location.Y + CtrlMoveInterval <= Screen.GetWorkingArea(this.Location).Bottom - this.Height)
				this.Location = new Point(this.Location.X, this.Location.Y + CtrlMoveInterval);
			else
				this.Location = new Point(this.Location.X, Screen.GetWorkingArea(this.Location).Bottom - this.Height);
		}

		System.Windows.Window MainWindow
		{
			get
			{
				return
					commandsWindow;
				//tmpCommandsWindow1;
				//this;
			}
		}
		private void ToggleWindowActivation()
		{
			//if (Win32Api.GetForegroundWindow() != this.Handle)
			//	WindowsInterop.ShowAndActivateForm(this);
			//else this.Hide();
			if (Win32Api.GetForegroundWindow() != (new System.Windows.Interop.WindowInteropHelper(MainWindow)).Handle)
				ShowAndActivateMainWindow(1);
			else
				MainWindow.Hide();
			//if (Win32Api.GetForegroundWindow() != (new System.Windows.Interop.WindowInteropHelper(tmpCommandsWindow1)).Handle)
			//	WindowsInterop.ShowAndActivateWindow(tmpCommandsWindow1);
			//else
			//	tmpCommandsWindow1.Hide();
		}

		//void SetAutocompleteActionList()
		//{
		//	if (comboboxCommand.AutoCompleteCustomSource != InlineCommands.InlineCommands.AutoCompleteAllactionList)
		//	{
		//		comboboxCommand.AutoCompleteCustomSource = InlineCommands.InlineCommands.AutoCompleteAllactionList;
		//	}
		//}

		//void SetAutoCompleteForAction(string action)
		//{
		//	InlineCommands.InlineCommands.CommandDetails commDetails = InlineCommands.InlineCommands.CommandList[action];
		//	if (comboboxCommand.AutoCompleteCustomSource != commDetails.commandPredefinedArguments && action.Length > 2)
		//	{
		//		comboboxCommand.AutoCompleteCustomSource = commDetails.commandPredefinedArguments;
		//	}
		//}

		private int ScrollLinesCtrlUpDown = 1;
		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				PerformCommandNow(comboboxCommand.Text);
			}
			else if (IsControlDown() && e.KeyCode == Keys.Down)
			{
				for (int i = 0; i < ScrollLinesCtrlUpDown; i++)
					Win32Api.SendMessage(textBox_Messages.Handle, Win32Api.WM_VSCROLL, (IntPtr)Win32Api.SB_LINEDOWN, IntPtr.Zero);
			}
			else if (IsControlDown() && e.KeyCode == Keys.Up)
			{
				for (int i = 0; i < ScrollLinesCtrlUpDown; i++)
					Win32Api.SendMessage(textBox_Messages.Handle, Win32Api.WM_VSCROLL, (IntPtr)Win32Api.SB_LINEUP, IntPtr.Zero);
			}
			else if ((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) && ModifierKeys == Keys.None)
			{
				if (comboboxCommand.Text.Length == 0)
				{
					List<string> historyList = comboboxCommand.Tag as List<string>;
					if (historyList.Count > 0)
					{
						ScrollTextHistoryOnUpDownKeys = true;
						comboboxCommand.Text = historyList[e.KeyCode == Keys.Down ? 0 : historyList.Count - 1];
					}
				}
				else if (ScrollTextHistoryOnUpDownKeys)
				{
					List<string> historyList = comboboxCommand.Tag as List<string>;
					if (historyList.Count > 0)
					{
						if (e.KeyCode == Keys.Down && historyList.IndexOf(comboboxCommand.Text) < historyList.Count - 1)
							comboboxCommand.Text = historyList[historyList.IndexOf(comboboxCommand.Text) + 1];
						else if (e.KeyCode == Keys.Up && historyList.IndexOf(comboboxCommand.Text) > 0)
							comboboxCommand.Text = historyList[historyList.IndexOf(comboboxCommand.Text) - 1];
					}
				}
				//if (textBox1.AutoCompleteSource.
			}
		}

		private void PerformCommandNow(string text, bool ClearCommandTextboxOnSuccess = true, bool HideAfterSuccess = false)
		{
			/*string errorMsg;
			InlineCommands.InlineCommands.CommandDetails command = InlineCommands.InlineCommands.GetCommandDetailsFromTextboxText(text);
			if (command == null)
			{
				appendLogTextbox("Command not recognized: " + text);
				//Win32Api.SendMessage(comboboxCommand.Handle, Win32Api.CB_SHOWDROPDOWN, 1, 0);
			}
			else if (text.Trim().Length == 0)
				appendLogTextbox("No command entered.");
			else if (command == null && text.Contains(' '))
				appendLogTextbox("Command not regonized: " + text);
			else if (command != null && text.Contains(' ') && text.Length < command.commandName.Length + 2
				&& command.CommandHasRequiredArguments())
				appendLogTextbox("No arguments for command: " + command.commandName);
			else if (!command.TextHasAllRequiredArguments(text))
				appendLogTextbox("Not all required arguments found");
			else if (!command.TextValidateArguments(text, out errorMsg))
				appendLogTextbox("Error: " + errorMsg);
			else
			{
				ScrollTextHistoryOnUpDownKeys = false;
				appendLogTextbox("");
				appendLogTextbox("Performing command: " + text);
				(comboboxCommand.Tag as List<string>).Add(text);
				Logging.staticNotifyIcon = notifyIcon1;
				command.PerformCommand(text, this.comboboxCommand, textFeedback, progressChanged);
				if (ClearCommandTextboxOnSuccess) comboboxCommand.Text = "";
				if (HideAfterSuccess) this.Hide();
			}*/
		}

		//UserActivityHook actHook;
		private void InitializeHooks(bool InstallMouseHook, bool InstallKeyboardHook)
		{
			//actHook = new UserActivityHook(InstallMouseHook, InstallKeyboardHook);
			//actHook.OnMouseActivity += new UserActivityHook.MoreMouseEventHandler(actHook_OnMouseActivity);
			//actHook.KeyDown += new KeyEventHandler(actHook_KeyDown);
			//actHook.KeyPress += new KeyPressEventHandler(actHook_KeyPress);
			//actHook.KeyUp += new KeyEventHandler(actHook_KeyUp);
		}

		//void actHook_OnMouseActivity(object sender, UserActivityHook.MoreMouseEventArgs e)
		//{
		//    switch (e.Button.Button)
		//    {
		//        case MouseButtons.Left:
		//            break;
		//        case MouseButtons.Middle:
		//            break;
		//        case MouseButtons.None:
		//            break;
		//        case MouseButtons.Right:
		//            break;
		//        case MouseButtons.XButton1:
		//            break;
		//        case MouseButtons.XButton2:
		//            break;
		//        default:
		//            break;
		//    }
		//}

		//void actHook_KeyDown(object sender, KeyEventArgs e)
		//{

		//}

		//void actHook_KeyPress(object sender, KeyPressEventArgs e)
		//{
		//}

		//void actHook_KeyUp(object sender, KeyEventArgs e)
		//{
		//    if (ModifierKeys == Keys.Control)
		//    {
		//    }
		//}

		private void updateStatusText(string str)
		{
			label1.Text = str;
		}

		private void appendLogTextbox(string str, bool mustUpdateStatusText = false)
		{
			//label1.Text = str;
			textBox_Messages.Text = str + (textBox_Messages.Text.Length > 0 ? Environment.NewLine : "") + textBox_Messages.Text;
			if (mustUpdateStatusText) updateStatusText(str);
			Application.DoEvents();
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			/*string tmpkey = null;
			if (comboboxCommand.Text.ToLower().IndexOf(' ') != -1)
				tmpkey = comboboxCommand.Text.ToLower().Substring(0, comboboxCommand.Text.ToLower().IndexOf(' '));
			if (tmpkey != null && InlineCommands.InlineCommands.CommandList != null && InlineCommands.InlineCommands.CommandList.ContainsKey(tmpkey))
			{
				label1.Text = InlineCommands.InlineCommands.CommandList[tmpkey].UserLabel;

				string textboxArgsString =  comboboxCommand.Text.Length >= comboboxCommand.Text.IndexOf(' ') + 2 ? comboboxCommand.Text.Substring(comboboxCommand.Text.IndexOf(' ') + 1) : "";

				string[] textBoxArgsSplitted = textboxArgsString.Split(InlineCommands.InlineCommands.CommandDetails.ArgumentSeparator);
				string lastetextboxArg = textBoxArgsSplitted.Length > 0 ? textBoxArgsSplitted[textBoxArgsSplitted.Length - 1] : "";
				if (textboxArgsString.EndsWith(@"\") && lastetextboxArg.Contains(@":\"))
				{
					if (Directory.Exists(lastetextboxArg))
					{
						//appendLogTextbox("Dir");
						InlineCommands.InlineCommands.CommandDetails commDetails = InlineCommands.InlineCommands.CommandList[tmpkey];
						if (commDetails.commandArguments != null && commDetails.commandArguments.Count > textBoxArgsSplitted.Length - 1)
							if (commDetails.commandArguments[textBoxArgsSplitted.Length - 1].PathAutocomplete == InlineCommands.InlineCommands.CommandDetails.PathAutocompleteEnum.Both
								|| commDetails.commandArguments[textBoxArgsSplitted.Length - 1].PathAutocomplete == InlineCommands.InlineCommands.CommandDetails.PathAutocompleteEnum.Directories
								|| commDetails.commandArguments[textBoxArgsSplitted.Length - 1].PathAutocomplete == InlineCommands.InlineCommands.CommandDetails.PathAutocompleteEnum.Files)
							{
								string textboxText = comboboxCommand.Text;
								string argswithoutlast = textboxArgsString.Substring(0, textboxArgsString.Length - lastetextboxArg.Length);
								if (commDetails.commandArguments[textBoxArgsSplitted.Length - 1].PathAutocomplete == InlineCommands.InlineCommands.CommandDetails.PathAutocompleteEnum.Directories
									|| commDetails.commandArguments[textBoxArgsSplitted.Length - 1].PathAutocomplete == InlineCommands.InlineCommands.CommandDetails.PathAutocompleteEnum.Both)
									foreach (string dir in Directory.GetDirectories(lastetextboxArg))
										if (!comboboxCommand.AutoCompleteCustomSource.Contains(tmpkey + " " + argswithoutlast + dir))
											comboboxCommand.AutoCompleteCustomSource.Add(tmpkey + " " + argswithoutlast + dir);

								if (commDetails.commandArguments[textBoxArgsSplitted.Length - 1].PathAutocomplete == InlineCommands.InlineCommands.CommandDetails.PathAutocompleteEnum.Files
									|| commDetails.commandArguments[textBoxArgsSplitted.Length - 1].PathAutocomplete == InlineCommands.InlineCommands.CommandDetails.PathAutocompleteEnum.Both)
									foreach (string file in Directory.GetFiles(lastetextboxArg))
										if (!comboboxCommand.AutoCompleteCustomSource.Contains(tmpkey + " " + argswithoutlast + file))
											comboboxCommand.AutoCompleteCustomSource.Add(tmpkey + " " + argswithoutlast + file);
							}


						//commDetails.commandArguments[0]
					}
					//string[] textBoxArgsSplitted = textboxArgsString.Split(Commands.CommandDetails.ArgumentSeparator);
					//foreach (string textboxArg in textBoxArgsSplitted)
					//  if (textboxArg.Contains(@":\") && textboxArg.EndsWith(@"\"))
					//  {
					//    if (Directory.Exists(textboxArg)) appendLogTextbox("Dir");
					//    foreach (Commands.CommandDetails.CommandArgumentClass commArg in commDetails.commandArguments)
					//    {

					//    }
					//  }
				}
				//else if (textboxArgsString.EndsWith(@","))// && lastetextboxArg.Contains(@":\"))
				//{
				//}
				else if (tmpkey == "kill" && textboxArgsString.Length >= 2)
				{
					ThreadingInterop.PerformVoidFunctionSeperateThread(() =>
						{
							ThreadingInterop.ClearTextboxAutocompleteCustomSource(comboboxCommand);
							//textBox1.AutoCompleteCustomSource.Clear();
							//textBox_Messages.Invoke(new ClearTextboxAutocompleteDelegate(ClearTextboxAutocomplete), new object[] { });
							Process[] procs = Process.GetProcesses();
							foreach (Process proc in procs)
								if (proc.ProcessName.ToLower().StartsWith(textboxArgsString.ToLower()))
									//textBox_Messages.Invoke(new AddTextboxAutocompleteDelegate(AddTextboxAutocomplete), new object[] { tmpkey + " " + proc.ProcessName });
									//textBox1.AutoCompleteCustomSource.Add(tmpkey + " " + proc.ProcessName);
									ThreadingInterop.AddTextboxAutocompleteCustomSource(comboboxCommand, tmpkey + " " + proc.ProcessName);
						});
				}
				else
				{
					SetAutoCompleteForAction(tmpkey);
					//Win32Api.SendMessage(comboboxCommand.Handle, Win32Api.CB_SHOWDROPDOWN, 1, 0);
				}
			}
			else
			{
				label1.Text = InlineCommands.InlineCommands.AvailableActionList;
				if (comboboxCommand.Text.Length == 0) SetAutocompleteActionList();
			}*/
		}

		/*public void ClearTextboxAutocomplete()
		{
			textBox1.AutoCompleteCustomSource.Clear();
		}

		public void AddTextboxAutocomplete(string strToAdd)
		{
			textBox1.AutoCompleteCustomSource.Add(strToAdd);
		}

		public delegate void AddTextboxAutocompleteDelegate(string text);
		public delegate void ClearTextboxAutocompleteDelegate();*/

		private void Form1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Win32Api.ReleaseCapture();
				Win32Api.SendMessage(Handle, Win32Api.WM_NCLBUTTONDOWN, new IntPtr(Win32Api.HT_CAPTION), IntPtr.Zero);
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			ApplicationRecoveryAndRestart.UnregisterForRecoveryAndRestart();
			Win32Api.UnregisterHotKey(this.Handle, Win32Api.Hotkey1);
			//overlayWindow.PreventClosing = false;
			//overlayWindow.Close();

			CustomBalloonTipwpf.AllowToClose = true;
			if (CustomBalloonTipwpf.StaticInstance != null)
				CustomBalloonTipwpf.StaticInstance.Close();
		}

		private void Form1_VisibleChanged(object sender, EventArgs e)
		{
			notifyIcon1.Visible = !this.Visible;
			if (notifyIcon1.Visible) comboboxCommand.Focus();
		}

		private void Form1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right) this.Hide();
		}

		public void ShowAndActivateMainWindow(double ScalingFactor = 1)
		{
			//ShowOverlayCommandWindows();
			MainWindow.Dispatcher.BeginInvoke((Action)delegate
			{
				if (OriginalWidthOfMainWindow == null) OriginalWidthOfMainWindow = MainWindow.Width;//.ActualWidth;
				if (OriginalHeightOfMainWindow == null) OriginalHeightOfMainWindow = MainWindow.Height;//.ActualHeight;
				MainWindow.Width = (double)OriginalWidthOfMainWindow;
				MainWindow.Height = (double)OriginalHeightOfMainWindow;
				(MainWindow.Content as System.Windows.FrameworkElement).LayoutTransform
					= new ScaleTransform(ScalingFactor, ScalingFactor);
				MainWindow.Width *= ScalingFactor > 1.8 ? 1.8 : ScalingFactor;
				MainWindow.Height *= ScalingFactor > 1.2 ? 1.2 : ScalingFactor;

				if (ScalingFactor <= 1)
				{
					MainWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
					Rectangle workingArea1 = Screen.GetWorkingArea(new Point((int)MainWindow.Left, (int)MainWindow.Top));
					MainWindow.Left = workingArea1.Left + (workingArea1.Width - MainWindow.Width) / 2;
					MainWindow.Top = workingArea1.Top + (workingArea1.Height - MainWindow.Height) / 2;
				}
				else
				{
					MainWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
					MainWindow.Left = 0;
					MainWindow.Top = 0;
				}

				WPFHelper.ShowAndActivateWindow(MainWindow);
			});
		}
		private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if (MainWindow == null)
					return;
				if (MainWindow.Visibility != System.Windows.Visibility.Visible)
					ShowAndActivateMainWindow(1);
				else
					MainWindow.Hide();
			}
		}

		private void PopulateCommandsMenuItem()
		{
			//ToolStripDropDownDirection defaultDropDirection = ToolStripDropDownDirection.Right;//Left;
			//menuItem_Commands.MenuItems.Clear();//.DropDownItems.Clear();
			//contextMenu_TrayIcon.DropDownDirection = defaultDropDirection;
			/*if (menuItem_Commands.MenuItems.Count == 0 && InlineCommands.InlineCommands.CommandList != null)
				foreach (string key in InlineCommands.InlineCommands.CommandList.Keys)
				{
					InlineCommands.InlineCommands.CommandDetails commandDetails = InlineCommands.InlineCommands.CommandList[key];

					MenuItem commanditem = new MenuItem(key) { Tag = commandDetails };

					commanditem.Click += delegate
					{
						UserMessages.ShowMessage("HallO");
					};

					//commanditem.DropDownDirection = defaultDropDirection;

					if (commandDetails.CommandHasArguments() || commandDetails.CommandHasPredefinedArguments())
					{
						//MenuItem customArguments = new MenuItem("_Custom arguments");
						////customArguments.AutoSize = true;
						////customArguments.DropDownDirection = defaultDropDirection;
						//commanditem.MenuItems.Add(customArguments);//.DropDownItems.Add(customArguments);

						if (commandDetails.CommandHasPredefinedArguments())
							foreach (string predefinedArguments in InlineCommands.InlineCommands.CommandList[key].commandPredefinedArguments)
							{
								MenuItem subcommanditem = new MenuItem(predefinedArguments.Substring(key.Length + 1)) { Tag = predefinedArguments };
								//subcommanditem.AutoSize = true;
								//subcommanditem.DropDownDirection = defaultDropDirection;
								subcommanditem.Click += delegate
								{
									PerformCommandNow(subcommanditem.Tag.ToString(), false, true);
								};
								//subcommanditem.DropDownOpened += delegate
								//{
								//	PerformCommandNow(subcommanditem.Tag.ToString(), false, true);
								//};
								commanditem.MenuItems.Add(subcommanditem);//.DropDownItems.Add(subcommanditem);
							}
					}
					else
					{
						commanditem.Click += delegate
						{
							UserMessages.ShowInfoMessage("No subcommands");
						};
					}
					menuItem_Commands.MenuItems.Add(commanditem);//.DropDownItems.Add(commanditem);
				}*/
		}

		private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
		{
			//WindowsInterop.ShowAndActivateForm(this);
			ShowAndActivateMainWindow(1);
		}

		private void menuItem_Exit_Click(object sender, EventArgs e)
		{
			mustStop = true;
			this.Close();
		}

		private void contextMenu_TrayIcon_Popup(object sender, EventArgs e)
		{
			if (MousePositionBeforePopup.X != -1 && MousePositionBeforePopup.Y != -1)
				Cursor.Position = MousePositionBeforePopup;
			MousePositionBeforePopup = new Point(-1, -1);
			PopulateCommandsMenuItem();
		}

		private void menuItem2_Click(object sender, EventArgs e)
		{
			//CommandWindow mw = new CommandWindow("tmp123");
			//mw.AddControl("tmp1", new System.Windows.Controls.TextBox(), System.Windows.Media.Colors.Black);
			//mw.AddControl("tmp2", new System.Windows.Controls.TextBox(), System.Windows.Media.Colors.Red);
			//mw.AddControl("tmp3", new System.Windows.Controls.TextBox(), System.Windows.Media.Colors.Green);
			//mw.AddControl("tmp4", new System.Windows.Controls.TextBox(), System.Windows.Media.Colors.Blue);
			//mw.Show();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			OwnUnhandledExceptionHandler.TestCrash(true, msg => UserMessages.Confirm(msg));
		}

		private WebResultsWindow webResultsWindow;
		private void menuItem3_Click(object sender, EventArgs e)
		{
			//MessageBox.Show("Loading wind results for strand");
			//MessageBox.Show(html);

			if (webResultsWindow == null) webResultsWindow = new WebResultsWindow();
			webResultsWindow.ShowDialog();
			webResultsWindow = null;
		}

		private void menuItem4_Click(object sender, EventArgs e)
		{
			for (int i = 1; i <= 10; i++)
				CustomBalloonTipwpf.ShowCustomBalloonTip(
					"Title " + i,
					"Message " + i,
					TimeSpan.FromSeconds(2),
					CustomBalloonTipwpf.IconTypes.Information,
					(snder) => { if (snder is CustomBalloonTipwpf.CustomBalloonTipClass) MessageBox.Show("Clicked on: " + (snder as CustomBalloonTipwpf.CustomBalloonTipClass).Message); },
					Scaling: ((double)i) / (double)2);
		}

		//NewPluginWindow newPluginWindow;
		private void menuItem5_Click(object sender, EventArgs e)
		{
			//if (newPluginWindow == null)
			//	newPluginWindow = new NewPluginWindow();
			//WindowsInterop.ShowAndActivateWindow(newPluginWindow);
			NewPluginWindow.ShowAndSavePlugin();
		}

		private void menuItem6_Click(object sender, EventArgs e)
		{
			//Facedetection disabled for now
			//FaceTrainingForm.ShowFacetraining();
		}

		private void menuItem8_Click(object sender, EventArgs e)
		{
			//Facedetection disabled for now
			//List<string> embeddedImagePaths = VisualStudioInterop.GetAllEmbeddedResourcesReturnFilePaths(
			//	x =>
			//		x.ToLower().Contains(".TestEmguCV.".ToLower())
			//		&& x.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase), true);

			//if (embeddedImagePaths == null || embeddedImagePaths.Count == 0)
			//	return;

			////string ChosenImage = UserMessages.PickItem<string>(embeddedImagePaths, "Please choose the image to use for pedestrian detection", null);
			//string ChosenImage = (string)PickItemWPF.PickItem(typeof(string), embeddedImagePaths.ToArray(), "Please choose the image to use for pedestrian detection", null);

			//if (ChosenImage != null)
			//{
			//	Image<Bgr, byte> imageOut;
			//	bool usedGPU;
			//	long elapsedMilliseconds;
			//	string errorMsg;
			//	if (!PedestrianDetection.DetectInImage(
			//		//@"C:\Emgu\emgucv-windows-x86 2.3.0.1416\Emgu.CV.Example\PedestrianDetection\2pedestrian.png",
			//		ChosenImage,
			//		out imageOut,
			//		out usedGPU,
			//		out elapsedMilliseconds,
			//		out errorMsg))
			//		UserMessages.ShowWarningMessage("Could not do pedestrian detection: " + errorMsg);
			//	else
			//	{
			//		ThreadingInterop.PerformVoidFunctionSeperateThread(() =>
			//		{
			//			ImageViewer.Show(
			//				imageOut,
			//				String.Format("Pedestrain detection using {0} in {1} milliseconds.",
			//				   usedGPU ? "GPU" : "CPU",
			//				   elapsedMilliseconds));
			//		});
			//	}
			//}
		}
	}
}