using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;
using InlineCommands;
using System.Windows.Forms.Integration;
using System.Threading;
using UnhandledExceptions;

namespace QuickAccess
{
	//TODO: In C# press Ctrl + K, Ctrl + H to add an item to the Task List (choose Shorcuts from the dropdown options).
	public partial class Form1 : Form
	{
		private InlineCommandsWindowWPF inlineCommandsWindowWPF;
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

		Socket listeningSocket;
		TextFeedbackEventHandler textFeedback;
		ProgressChangedEventHandler progressChanged;

		public Form1()
		{
			InitializeComponent();
			if (IsApplicationArestartedInstance())
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
			}

			ApplicationRecoveryAndRestart.RegisterApplicationRecoveryAndRestart(delegate
			{
				//TODO: Application Restart and Recovery is there but no use so far?
				//File.WriteAllText(@"C:\Francois\Crash reports\tmpQuickAccess.log", "Application crashed, more details not incorporated yet." + DateTime.Now.ToString());
				//using (StreamWriter sw = new StreamWriter())
				ApplicationRecoveryAndRestart.WriteCrashReportFile("QuickAccess", "Application crashed, more details not incorporated yet.");
			},
			delegate
			{
				labelRecoveryAndRestartSafe.Visible = true;
				notifyIcon1.ShowBalloonTip(3000, "Recovery and Restart", "QuickAccess is now Recovery and Restart Safe", ToolTipIcon.Info);
			});

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
			ShowOverlayRibbon();

			//overlayWindow.IsVisibleChanged += delegate
			//{
			//	if (overlayWindow.IsVisible) overlayRibbon.Hide();
			//	else overlayRibbon.Show();
			//};
			//ShowOverlayCommandWindows(true);

			textFeedback += (snder, evtargs) => { Logging.appendLogTextbox_OfPassedTextbox(textBox_Messages, evtargs.FeedbackText); };
			progressChanged += (snder, evtargs) => { UpdateProgress(evtargs.CurrentValue, evtargs.MaximumValue, evtargs.BytesPerSecond); };

			//TODO: Check out this command SvnInterop.CheckStatusAllVisualStudio2010Projects()
			//SvnInterop.CheckStatusAllVisualStudio2010Projects();

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

			foreach (Type type in typeof(GlobalSettings).GetNestedTypes(BindingFlags.Public))
				if (!type.IsAbstract && type.BaseType == typeof(GenericSettings))
				{
					PropertyInfo[] staticProperties = type.GetProperties(BindingFlags.Static | BindingFlags.Public);
					foreach (PropertyInfo spi in staticProperties)
						if (type == spi.PropertyType)
						{
							//MessageBox.Show("Static = " + spi.Name + ", of type = " + spi.PropertyType.Name);
							PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
							foreach (PropertyInfo pi in properties)
								pi.GetValue(spi.GetValue(null));
								//MessageBox.Show(pi.Name + " = " + pi.GetValue(spi.GetValue(null)).ToString());
						}
				}

			//MessageBox.Show("FtpPassword = " + GlobalSettings.VisualStudioInteropSettings.Instance.FtpPassword);

			UserMessages.iconForMessages = this.Icon;

			inlineCommandsWindowWPF = new InlineCommandsWindowWPF();
			inlineCommandsWindowWPF.Closed += delegate { this.Close(); };
			//tmpCommandsWindow1 = new tmpCommandsWindow();
			//tmpCommandsWindow1.Closed += delegate { this.Close(); };

			//TODO: Check uit AppDomain.MonitoringIsEnabled
			//MessageBox.Show(AppDomain.MonitoringIsEnabled.ToString());

			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			//int a = 1;
			//double i = 1 / (1 - a);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (!(e.ExceptionObject is Exception)) return;
			//UserMessages.ShowErrorMessage("An error unhandled by the application has occurred, application shutting down: " + exc.Message + Environment.NewLine + exc.StackTrace);
			//UnhandledExceptionsWindow uew = new UnhandledExceptionsWindow(e.ExceptionObject as Exception);
			UnhandledExceptionsWindow.ShowUnHandledException(e.ExceptionObject as Exception);
			this.Close();
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
			this.ShowInTaskbar = true;
			if (!Win32Api.RegisterHotKey(this.Handle, Win32Api.Hotkey1, Win32Api.MOD_CONTROL, (int)Keys.Q)) UserMessages.ShowErrorMessage("QuickAccess could not register hotkey Ctrl + Q");
			//if (!Win32Api.RegisterHotKey(this.Handle, Win32Api.Hotkey2, Win32Api.MOD_CONTROL + Win32Api.MOD_SHIFT, (int)Keys.Q)) UserMessages.ShowErrorMessage("QuickAccess could not register hotkey Ctrl + Shift + Q");
			//label1.Text = InlineCommands.InlineCommands.AvailableActionList;
			//SetAutocompleteActionList();
			//InitializeHooks(false, true);
			this.Hide();
			this.Opacity = origOpacity;

			if (Environment.CommandLine.ToLower().Contains(@"documents\visual studio 2010\")) buttonTestCrash.Visible = true;

			ThreadingInterop.PerformVoidFunctionSeperateThread(() =>
			{
				//TODO: There is some issue with the second time a file is sent (on the server side).
				NetworkInterop.StartServer_FileStream(
					out listeningSocket,
					this,
					TextFeedbackEvent: textFeedback,
					ProgressChangedEvent: progressChanged);
			}, false);

			//XmlRpcInterop.SampleServer();
			XmlRpcInterop.StartDynamicCodeInvokingServer_XmlRpc();
		}

		private Point MousePositionBeforePopup = new Point(-1, -1);
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == Win32Api.WM_HOTKEY)
			{
				if (m.WParam == new IntPtr(Win32Api.Hotkey1))
					ToggleWindowActivation();
				//if (m.WParam == new IntPtr(Win32Api.Hotkey2))
				//	ShowOverlayCommandWindows();
			}
			base.WndProc(ref m);
		}

		private void UpdateProgress(int currentValue, int maximumValue, double bytesPerSecond = -1)
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
		}

		private bool IsApplicationArestartedInstance()
		{
			return System.Environment.GetCommandLineArgs().Length > 1 && System.Environment.GetCommandLineArgs()[1] == "/restart";
		}

		OverlayRibbon overlayRibbon = new OverlayRibbon();
		private void ShowOverlayRibbon()
		{
			Rectangle workingArea = Screen.FromPoint(new Point(0, 0)).WorkingArea;
			overlayRibbon.Top = workingArea.Top + (workingArea.Height - overlayRibbon.ActualHeight) / 2 + 100;
			overlayRibbon.Left = 0;
			//MessageBox.Show(overlayRibbon.Left + ", " + overlayRibbon.Top);
			overlayRibbon.MouseClickedRequestToOpenOverlayWindow += delegate
			{
				//ShowOverlayCommandWindows();
				ShowAndActivateMainWindow();
			};
			overlayRibbon.Show();
		}

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
										//TODO: Something not working right here (see the MAIL command), does not concat the arguments into one string.
										//foreach (string s in thisPredefinedArguments.Substring(thisCommandDetails.commandName.Length + 1).Split(InlineCommands.InlineCommands.CommandDetails.ArgumentSeparator))
										//  MessageBox.Show(s;)
									};
									//treeviewItem.PreviewDragEnter += (snder, evtargs) => { evtargs.Handled = true; };
									//treeviewItem.PreviewDragOver += (snder, evtargs) => { evtargs.Handled = true; };
									//treeviewItem.PreviewDrop += (snder, evtargs) => { evtargs.Handled = true; };

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
					inlineCommandsWindowWPF;
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
				ShowAndActivateMainWindow();
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
			//TODO: Need to add comma separated values to textbox also working with autocomplete for each i.e. svnupdate MonitorSystem,QuickAccess,SharedClasses
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

		UserActivityHook actHook;
		private void InitializeHooks(bool InstallMouseHook, bool InstallKeyboardHook)
		{
			actHook = new UserActivityHook(InstallMouseHook, InstallKeyboardHook);
			actHook.OnMouseActivity += new MouseEventHandler(actHook_OnMouseActivity);
			actHook.KeyDown += new KeyEventHandler(actHook_KeyDown);
			actHook.KeyPress += new KeyPressEventHandler(actHook_KeyPress);
			actHook.KeyUp += new KeyEventHandler(actHook_KeyUp);
		}

		void actHook_OnMouseActivity(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Left:
					break;
				case MouseButtons.Middle:
					break;
				case MouseButtons.None:
					break;
				case MouseButtons.Right:
					break;
				case MouseButtons.XButton1:
					break;
				case MouseButtons.XButton2:
					break;
				default:
					break;
			}
		}

		void actHook_KeyDown(object sender, KeyEventArgs e)
		{

		}

		void actHook_KeyPress(object sender, KeyPressEventArgs e)
		{
		}

		void actHook_KeyUp(object sender, KeyEventArgs e)
		{
			if (ModifierKeys == Keys.Control)
			{
			}
		}

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
			//TODO: Should check out the debugging tools for Windows, Run the "Global Flags" and in the "Kernel Flags" tab enable items "Enable heap tail checking", "Enable heap free checking", "Enable page heap"
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
					//TODO: Should eventually (on next commented lines) add autocomplete for COMMAseparated arguments like svnupdate MonitorSystem,QuickAccess
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
					//TODO: Must still adapt combobox to use the Dropdown, instead of its built in autocomplete
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
			ApplicationRecoveryAndRestart.UnregisterApplicationRecoveryAndRestart();
			Win32Api.UnregisterHotKey(this.Handle, Win32Api.Hotkey1);
			//overlayWindow.PreventClosing = false;
			//overlayWindow.Close();
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

		private void ShowAndActivateMainWindow()
		{
			WindowsInterop.ShowAndActivateWindow(MainWindow);
		}
		private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
				ShowAndActivateMainWindow();
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
			ShowAndActivateMainWindow();
		}

		private void menuItem_Exit_Click(object sender, EventArgs e)
		{
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
			CommandWindow mw = new CommandWindow("tmp123");
			mw.AddControl("tmp1", new System.Windows.Controls.TextBox(), System.Windows.Media.Colors.Black);
			mw.AddControl("tmp2", new System.Windows.Controls.TextBox(), System.Windows.Media.Colors.Red);
			mw.AddControl("tmp3", new System.Windows.Controls.TextBox(), System.Windows.Media.Colors.Green);
			mw.AddControl("tmp4", new System.Windows.Controls.TextBox(), System.Windows.Media.Colors.Blue);
			mw.Show();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			ApplicationRecoveryAndRestart.TestCrash(true);
		}
	}
}