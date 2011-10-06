using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace QuickAccess
{
	public partial class Form1 : Form
	{
		//private static string ThisAppName = "QuickAccess";
		private double origOpacity;
		private bool ScrollTextHistoryOnUpDownKeys = false;

		MouseHooks.MouseHook mouseHook;
		OverlayForm overlayForm = new OverlayForm();

		public Form1()
		{
			InitializeComponent();

			textBox1.Tag = new List<string>();
			InlineCommands.PopulateCommandList();

			/*if (!System.Diagnostics.Debugger.IsAttached && Environment.GetCommandLineArgs()[0].ToUpper().Contains("Apps\\2.0".ToUpper()))
			{
				Microsoft.Win32.RegistryKey regkeyRUN = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
				regkeyRUN.SetValue(ThisAppName, "\"" + System.Windows.Forms.Application.ExecutablePath + "\"", Microsoft.Win32.RegistryValueKind.String);
			}*/

			origOpacity = this.Opacity;
			this.Opacity = 0;

			notifyIcon1.ContextMenu = contextMenu_TrayIcon;

			mouseHook = new MouseHooks.MouseHook();
			//mouseHook.MouseGestureEvent += (o, gest) => { if (gest.MouseGesture == Win32Api.MouseGestures.RL) UserMessages.ShowErrorMessage("Message"); };
			mouseHook.MouseMoveEvent += delegate
			{
				if (MousePosition.X < 5)
				{
					if (overlayForm.IsDisposed) overlayForm = new OverlayForm();
					if (!overlayForm.Visible)
					{
						//foreach (string s in s.s)
						foreach (string key in InlineCommands.CommandList.Keys)
						{
							CommandForm tmpCommandForm = new CommandForm(key);
							overlayForm.ListOfChildForms.Add(tmpCommandForm);

							InlineCommands.CommandDetails commandDetails = InlineCommands.CommandList[key];

							if (commandDetails.CommandHasArguments() || commandDetails.CommandHasPredefinedArguments())
							{
								if (commandDetails.CommandHasArguments())
								{

									foreach (InlineCommands.CommandDetails.CommandArgumentClass arg in commandDetails.commandArguments)
									{
										tmpCommandForm.AddControl(arg.ArgumentName, new TextBox() { ForeColor = arg.Required ? Color.Red : Color.Green });
									}
								}

								//if (commandDetails.CommandHasPredefinedArguments())
								//  foreach (string predefinedArguments in InlineCommands.CommandList[key].commandPredefinedArguments)
								//  {
								//    tmpCommandForm.AddControl(new TextBox() { Text = predefinedArguments });
								//    //MenuItem subcommanditem = new MenuItem(predefinedArguments.Substring(key.Length + 1)) { Tag = predefinedArguments };
								//    //subcommanditem.AutoSize = true;
								//    //subcommanditem.DropDownDirection = defaultDropDirection;
								//    //subcommanditem.Click += delegate
								//    //{
								//    //  PerformCommandNow(subcommanditem.Tag.ToString(), false, true);
								//    //};
								//    //subcommanditem.DropDownOpened += delegate
								//    //{
								//    //  PerformCommandNow(subcommanditem.Tag.ToString(), false, true);
								//    //};
								//    //commanditem.MenuItems.Add(subcommanditem);//.DropDownItems.Add(subcommanditem);
								//  }

								//for (int i = 0; i < commanditem.MenuItems.Count; i++)//.DropDownItems.Count; i++)
								//{
								//  MenuItem tmpsubcommandItem = commanditem.MenuItems[i];//.DropDownItems[i] as ToolStripMenuItem;
								//  if (commandDetails.CommandHasArguments())
								//  {
								//    foreach (Commands.CommandDetails.CommandArgumentClass arg in Commands.CommandList[key].commandArguments)
								//    {
								//      //ToolStripTextBox subcommandTextboxitem = new ToolStripTextBox();
								//      MenuItem subcommandTextboxitem = new MenuItem();
								//      subcommandTextboxitem.Tag = tmpsubcommandItem;
								//      //subcommandTextboxitem.BackColor = arg.Required ? Color.LightGreen : Color.LightGray;
								//      subcommandTextboxitem.Click += (sender1, e1) =>
								//      {
								//        //if (e1.KeyCode == Keys.Enter)
								//        //{
								//        if (subcommandTextboxitem.Tag is MenuItem)
								//          {
								//            //ToolStripMenuItem tmpCommandMenuItem = (subcommandTextboxitem.OwnerItem as ToolStripMenuItem).OwnerItem as ToolStripMenuItem;
								//            MenuItem tmpCommandMenuItem = subcommandTextboxitem.Tag as MenuItem;
								//            //ToolStripMenuItem tmpArgumentsOwner = subcommandTextboxitem.OwnerItem as ToolStripMenuItem;

								//            bool EmptyRequiredArguments = false;
								//            Commands.CommandDetails commdet = (Commands.CommandDetails)commanditem.Tag;
								//            for (int j = 0; j < commdet.commandArguments.Count; j++)
								//              if (commdet.commandArguments[j].Required && tmpArgumentsOwner.DropDownItems[j].Text.Length == 0)
								//              {
								//                EmptyRequiredArguments = true;
								//                MessageBox.Show("Please complete all required textboxes (green), textbox " + (j + 1).ToString() + " is empty");
								//              }

								//            if (!EmptyRequiredArguments)
								//            {
								//              string concatString = "";
								//              foreach (ToolStripItem ti1 in tmpArgumentsOwner.DropDownItems)
								//              {
								//                if (ti1 is ToolStripTextBox)
								//                  concatString += (concatString.Length > 0 ? ";" : "") + ((ToolStripTextBox)ti1).Text;
								//              }
								//              PerformCommandNow(((Commands.CommandDetails)commanditem.Tag).commandName + " " + concatString, false, true);
								//            }
								//            //MessageBox.Show((subcommandTextboxitem.OwnerItem as ToolStripMenuItem).Text);
								//          }
								//        //}
								//        //else if (e1.KeyCode == Keys.Left)
								//        //{
								//        //  if (subcommandTextboxitem.Text.Length == 0 || subcommandTextboxitem.SelectionStart == 0)
								//        //    if (subcommandTextboxitem.OwnerItem is ToolStripMenuItem)
								//        //      (subcommandTextboxitem.OwnerItem as ToolStripMenuItem).HideDropDown();
								//        //}
								//      };
								//      tmpsubcommandItem.MenuItems.Add(subcommandTextboxitem);//.DropDownItems.Add(subcommandTextboxitem);
								//    }

								//    if (i > 0)
								//    {
								//      string[] splittedArgs = tmpsubcommandItem.Tag.ToString().Substring(key.Length + 1).Split(';');
								//      for (int k = 0; k < splittedArgs.Length; k++)
								//        tmpsubcommandItem.DropDownItems[k].Text = splittedArgs[k];
								//    }
								//  }
								//}
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
						overlayForm.Show();
					}
				}
			};
			mouseHook.Start();
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
			this.ShowInTaskbar = true;
			if (!Win32Api.RegisterHotKey(this.Handle, Win32Api.Hotkey1, Win32Api.MOD_CONTROL, (int)Keys.Q)) UserMessages.ShowErrorMessage("QuickAccess could not register hotkey Ctrl + Q");
			if (!Win32Api.RegisterHotKey(this.Handle, Win32Api.Hotkey2, Win32Api.MOD_CONTROL + Win32Api.MOD_SHIFT, (int)Keys.Q)) UserMessages.ShowErrorMessage("QuickAccess could not register hotkey Ctrl + Shift + Q");
			label1.Text = InlineCommands.AvailableActionList;
			SetAutocompleteActionList();
			//InitializeHooks(false, true);
			this.Hide();
			this.Opacity = origOpacity;
		}

		private Point MousePositionBeforePopup = new Point(-1, -1);
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == Win32Api.WM_HOTKEY)
			{
				if (m.WParam == new IntPtr(Win32Api.Hotkey1))
					ToggleWindowActivation();
				if (m.WParam == new IntPtr(Win32Api.Hotkey2))
				{
					MousePositionBeforePopup = MousePosition;
					Cursor.Position = new Point(0, 0);
					MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
					mi.Invoke(notifyIcon1, null);
					//notifyIcon1.ContextMenu.Show(this, new Point(0, 0));
					//ShowWindow(notifyIcon1.ContextMenu.Handle, 5);
					//SetWindowPos((int)notifyIcon1.ContextMenu.Handle, HWND_TOPMOST, 0, 0, 100, 20, 0x0010);

					////contextMenu_TrayIcon.Show(null, new Point(0, 0));//Screen.PrimaryScreen.WorkingArea.Right, Screen.PrimaryScreen.WorkingArea.Bottom));//MousePosition);
					//this.Activate();
					////contextMenu_TrayIcon.Focus();
					//if (contextMenu_TrayIcon.MenuItems.Count > 0)
					//{
					//  //DONE TODO: The following line actually dows nothing
					//  //contextMenu_TrayIcon.ShowDropDown();//.DropDownItems[0].Select();
					//  contextMenu_TrayIcon.MenuItems[0].PerformSelect();//.DropDownItems[0].Select();
					//}
					//else
					//  menuItem_Commands.PerformSelect();
					//  //commandsToolStripMenuItem.sel.Select();
				}
			}
			base.WndProc(ref m);
		}

		private bool IsAltDown()
		{
			return ((ModifierKeys & Keys.Alt) == Keys.Alt);
		}

		private bool IsControlDown()
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
				if (textBox1.Text.Length > 0) textBox1.Text = "";
				else this.Hide();
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

		private void ToggleWindowActivation()
		{
			if (Win32Api.GetForegroundWindow() != this.Handle)
			{
				WindowsInterop.ShowAndActivateForm(this);
			}
			else this.Hide();
		}

		void SetAutocompleteActionList()
		{
			if (textBox1.AutoCompleteCustomSource != InlineCommands.AutoCompleteAllactionList)
			{
				textBox1.AutoCompleteCustomSource = InlineCommands.AutoCompleteAllactionList;
			}
		}

		void SetAutoCompleteForAction(string action)
		{
			InlineCommands.CommandDetails commDetails = InlineCommands.CommandList[action];
			if (textBox1.AutoCompleteCustomSource != commDetails.commandPredefinedArguments && textBox1.TextLength > 2)
			{
				textBox1.AutoCompleteCustomSource = commDetails.commandPredefinedArguments;
			}
		}

		private int ScrollLinesCtrlUpDown = 1;
		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				PerformCommandNow(textBox1.Text);
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
				if (textBox1.TextLength == 0)
				{
					List<string> historyList = textBox1.Tag as List<string>;
					if (historyList.Count > 0)
					{
						ScrollTextHistoryOnUpDownKeys = true;
						textBox1.Text = historyList[e.KeyCode == Keys.Down ? 0 : historyList.Count - 1];
					}
				}
				else if (ScrollTextHistoryOnUpDownKeys)
				{
					List<string> historyList = textBox1.Tag as List<string>;
					if (historyList.Count > 0)
					{
						if (e.KeyCode == Keys.Down && historyList.IndexOf(textBox1.Text) < historyList.Count - 1)
							textBox1.Text = historyList[historyList.IndexOf(textBox1.Text) + 1];
						else if (e.KeyCode == Keys.Up && historyList.IndexOf(textBox1.Text) > 0)
							textBox1.Text = historyList[historyList.IndexOf(textBox1.Text) - 1];
					}
				}
				//if (textBox1.AutoCompleteSource.
			}
		}

		private void PerformCommandNow(string text, bool ClearCommandTextboxOnSuccess = true, bool HideAfterSuccess = false)
		{
			string errorMsg;
			InlineCommands.CommandDetails command = InlineCommands.GetCommandDetailsFromTextboxText(text);
			if (command == null && text.Contains(' '))
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
				(textBox1.Tag as List<string>).Add(text);
				command.PerformCommand(this.textBox1, this.textBox_Messages);
				if (ClearCommandTextboxOnSuccess) textBox1.Text = "";
				if (HideAfterSuccess) this.Hide();
			}
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
			string tmpkey = "987654321abcde";
			if (textBox1.Text.ToLower().IndexOf(' ') != -1)
				tmpkey = textBox1.Text.ToLower().Substring(0, textBox1.Text.ToLower().IndexOf(' '));
			if (InlineCommands.CommandList.ContainsKey(tmpkey))
			{
				label1.Text = InlineCommands.CommandList[tmpkey].UserLabel;

				string textboxArgsString = textBox1.Text.Substring(textBox1.Text.IndexOf(' ') + 1);

				string[] textBoxArgsSplitted = textboxArgsString.Split(InlineCommands.CommandDetails.ArgumentSeparator);
				string lastetextboxArg = textBoxArgsSplitted[textBoxArgsSplitted.Length - 1];
				if (textboxArgsString.EndsWith(@"\") && lastetextboxArg.Contains(@":\"))
				{
					if (Directory.Exists(lastetextboxArg))
					{
						//appendLogTextbox("Dir");
						InlineCommands.CommandDetails commDetails = InlineCommands.CommandList[tmpkey];
						if (commDetails.commandArguments != null && commDetails.commandArguments.Count > textBoxArgsSplitted.Length - 1)
							if (commDetails.commandArguments[textBoxArgsSplitted.Length - 1].PathAutocomplete == InlineCommands.CommandDetails.PathAutocompleteEnum.Both
								|| commDetails.commandArguments[textBoxArgsSplitted.Length - 1].PathAutocomplete == InlineCommands.CommandDetails.PathAutocompleteEnum.Directories
								|| commDetails.commandArguments[textBoxArgsSplitted.Length - 1].PathAutocomplete == InlineCommands.CommandDetails.PathAutocompleteEnum.Files)
							{
								string textboxText = textBox1.Text;
								string argswithoutlast = textboxArgsString.Substring(0, textboxArgsString.Length - lastetextboxArg.Length);
								if (commDetails.commandArguments[textBoxArgsSplitted.Length - 1].PathAutocomplete == InlineCommands.CommandDetails.PathAutocompleteEnum.Directories
									|| commDetails.commandArguments[textBoxArgsSplitted.Length - 1].PathAutocomplete == InlineCommands.CommandDetails.PathAutocompleteEnum.Both)
								foreach (string dir in Directory.GetDirectories(lastetextboxArg))
									if (!textBox1.AutoCompleteCustomSource.Contains(tmpkey + " " + argswithoutlast + dir))
										textBox1.AutoCompleteCustomSource.Add(tmpkey + " " + argswithoutlast + dir);

								if (commDetails.commandArguments[textBoxArgsSplitted.Length - 1].PathAutocomplete == InlineCommands.CommandDetails.PathAutocompleteEnum.Files
									|| commDetails.commandArguments[textBoxArgsSplitted.Length - 1].PathAutocomplete == InlineCommands.CommandDetails.PathAutocompleteEnum.Both)
									foreach (string file in Directory.GetFiles(lastetextboxArg))
										if (!textBox1.AutoCompleteCustomSource.Contains(tmpkey + " " + argswithoutlast + file))
											textBox1.AutoCompleteCustomSource.Add(tmpkey + " " + argswithoutlast + file);
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
				else if (tmpkey == "kill" && textboxArgsString.Length >= 2)
				{
					ThreadingInterop.PerformVoidFunctionSeperateThread(() =>
						{
							ThreadingInterop.ClearTextboxAutocompleteCustomSource(textBox1);
							//textBox1.AutoCompleteCustomSource.Clear();
							//textBox_Messages.Invoke(new ClearTextboxAutocompleteDelegate(ClearTextboxAutocomplete), new object[] { });
							Process[] procs = Process.GetProcesses();
							foreach (Process proc in procs)
								if (proc.ProcessName.ToLower().StartsWith(textboxArgsString.ToLower()))
									//textBox_Messages.Invoke(new AddTextboxAutocompleteDelegate(AddTextboxAutocomplete), new object[] { tmpkey + " " + proc.ProcessName });
									//textBox1.AutoCompleteCustomSource.Add(tmpkey + " " + proc.ProcessName);
									ThreadingInterop.AddTextboxAutocompleteCustomSource(textBox1, tmpkey + " " + proc.ProcessName);
						});
				}
				else SetAutoCompleteForAction(tmpkey);
			}
			else
			{
				label1.Text = InlineCommands.AvailableActionList;
				if (textBox1.Text.Length == 0) SetAutocompleteActionList();
			}
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
			Win32Api.UnregisterHotKey(this.Handle, Win32Api.Hotkey1);
		}

		private void Form1_VisibleChanged(object sender, EventArgs e)
		{
			notifyIcon1.Visible = !this.Visible;
			if (notifyIcon1.Visible) textBox1.Focus();
		}

		private void Form1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right) this.Hide();
		}

		private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
				WindowsInterop.ShowAndActivateForm(this);
		}

		private void PopulateCommandsMenuItem()
		{
			//ToolStripDropDownDirection defaultDropDirection = ToolStripDropDownDirection.Right;//Left;
			//menuItem_Commands.MenuItems.Clear();//.DropDownItems.Clear();
			//contextMenu_TrayIcon.DropDownDirection = defaultDropDirection;
			if (menuItem_Commands.MenuItems.Count == 0 && InlineCommands.CommandList != null)
				foreach (string key in InlineCommands.CommandList.Keys)
				{
					InlineCommands.CommandDetails commandDetails = InlineCommands.CommandList[key];

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
							foreach (string predefinedArguments in InlineCommands.CommandList[key].commandPredefinedArguments)
							{
								MenuItem subcommanditem = new MenuItem(predefinedArguments.Substring(key.Length + 1)) { Tag = predefinedArguments };
								//subcommanditem.AutoSize = true;
								//subcommanditem.DropDownDirection = defaultDropDirection;
								subcommanditem.Click += delegate
								{
									PerformCommandNow(subcommanditem.Tag.ToString(), false, true);
								};
								/*subcommanditem.DropDownOpened += delegate
								{
									PerformCommandNow(subcommanditem.Tag.ToString(), false, true);
								};*/
								commanditem.MenuItems.Add(subcommanditem);//.DropDownItems.Add(subcommanditem);
							}

						/*for (int i = 0; i < commanditem.MenuItems.Count; i++)//.DropDownItems.Count; i++)
						{
							MenuItem tmpsubcommandItem = commanditem.MenuItems[i];//.DropDownItems[i] as ToolStripMenuItem;
							if (commandDetails.CommandHasArguments())
							{
								foreach (Commands.CommandDetails.CommandArgumentClass arg in Commands.CommandList[key].commandArguments)
								{
									//ToolStripTextBox subcommandTextboxitem = new ToolStripTextBox();
									MenuItem subcommandTextboxitem = new MenuItem();
									subcommandTextboxitem.Tag = tmpsubcommandItem;
									//subcommandTextboxitem.BackColor = arg.Required ? Color.LightGreen : Color.LightGray;
									subcommandTextboxitem.Click += (sender1, e1) =>
									{
										//if (e1.KeyCode == Keys.Enter)
										//{
										if (subcommandTextboxitem.Tag is MenuItem)
											{
												//ToolStripMenuItem tmpCommandMenuItem = (subcommandTextboxitem.OwnerItem as ToolStripMenuItem).OwnerItem as ToolStripMenuItem;
												MenuItem tmpCommandMenuItem = subcommandTextboxitem.Tag as MenuItem;
												//ToolStripMenuItem tmpArgumentsOwner = subcommandTextboxitem.OwnerItem as ToolStripMenuItem;

												bool EmptyRequiredArguments = false;
												Commands.CommandDetails commdet = (Commands.CommandDetails)commanditem.Tag;
												for (int j = 0; j < commdet.commandArguments.Count; j++)
													if (commdet.commandArguments[j].Required && tmpArgumentsOwner.DropDownItems[j].Text.Length == 0)
													{
														EmptyRequiredArguments = true;
														MessageBox.Show("Please complete all required textboxes (green), textbox " + (j + 1).ToString() + " is empty");
													}

												if (!EmptyRequiredArguments)
												{
													string concatString = "";
													foreach (ToolStripItem ti1 in tmpArgumentsOwner.DropDownItems)
													{
														if (ti1 is ToolStripTextBox)
															concatString += (concatString.Length > 0 ? ";" : "") + ((ToolStripTextBox)ti1).Text;
													}
													PerformCommandNow(((Commands.CommandDetails)commanditem.Tag).commandName + " " + concatString, false, true);
												}
												//MessageBox.Show((subcommandTextboxitem.OwnerItem as ToolStripMenuItem).Text);
											}
										//}
										//else if (e1.KeyCode == Keys.Left)
										//{
										//  if (subcommandTextboxitem.Text.Length == 0 || subcommandTextboxitem.SelectionStart == 0)
										//    if (subcommandTextboxitem.OwnerItem is ToolStripMenuItem)
										//      (subcommandTextboxitem.OwnerItem as ToolStripMenuItem).HideDropDown();
										//}
									};
									tmpsubcommandItem.MenuItems.Add(subcommandTextboxitem);//.DropDownItems.Add(subcommandTextboxitem);
								}

								if (i > 0)
								{
									string[] splittedArgs = tmpsubcommandItem.Tag.ToString().Substring(key.Length + 1).Split(';');
									for (int k = 0; k < splittedArgs.Length; k++)
										tmpsubcommandItem.DropDownItems[k].Text = splittedArgs[k];
								}
							}
						}*/
					}
					else
					{
						commanditem.Click += delegate
						{
							UserMessages.ShowInfoMessage("No subcommands");
						};
					}
					menuItem_Commands.MenuItems.Add(commanditem);//.DropDownItems.Add(commanditem);
				}
		}

		private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
		{
			WindowsInterop.ShowAndActivateForm(this);
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

			//this.Activate();
			//contextMenu_TrayIcon.Focus();
			/*if (menuItem_Commands.MenuItems.Count > 0)
			{
				//DONE TODO: The following line actually dows nothing
				//contextMenu_TrayIcon.ShowDropDown();//.DropDownItems[0].Select();

				menuItem_Commands.MenuItems[0].PerformSelect();//.DropDownItems[0].Select();
			}
			else*/
				//menuItem_Commands.PerformClick();//.PerformSelect();
		}

		private void menuItem2_Click(object sender, EventArgs e)
		{
			CommandForm cf = new CommandForm("tmp123");
			cf.AddControl("tmp1", new TextBox());
			cf.AddControl("tmp2", new TextBox());
			cf.AddControl("tmp3", new TextBox());
			cf.AddControl("tmp4", new TextBox());
			cf.Show();
		}
	}
}