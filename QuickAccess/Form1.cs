using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace QuickAccess
{
	public partial class Form1 : Form
	{
		private static string ThisAppName = "QuickAccess";
		//private const string ServerAddress = "http://localhost";
		//private const string ServerAddress = "https://fjh.co.za";
		private const string ServerAddress = "http://firepuma.com";
		private const string doWorkAddress = ServerAddress + "/desktopapp";
		private static string Username = "f";
		private static string Password = "f";

		private const string AvailableActionList = "Type tasks: todo, run, mail, explore, web, google, kill, startupbat, call, cmd, btw, svncommit, etc";

		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;

		[DllImportAttribute("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();

		/// <summary>The GetForegroundWindow function returns a handle to the foreground window.</summary>
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		private double origOpacity;

		const int WM_HOTKEY = 786;
		[DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		const uint MOD_ALT = 1;
		const uint MOD_CONTROL = 2;
		const uint MOD_SHIFT = 4;
		const uint MOD_WIN = 8;
		const int Hotkey1 = 500;
		const int Hotkey2 = 501;

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		// Activate or minimize a window
		[DllImportAttribute("User32.DLL")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		private const int SW_SHOW = 5;
		private const int SW_MINIMIZE = 6;
		private const int SW_RESTORE = 9;

		// For Windows Mobile, replace user32.dll with coredll.dll
		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
		static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

		public class Commands
		{
			public static Dictionary<string, CommandDetails> CommandList = new Dictionary<string, CommandDetails>();
			public static AutoCompleteStringCollection AutoCompleteAllactionList;

			private static void AddToCommandList(string commandNameIn, List<string> commandPredefinedArgumentsIn, string UserLabelIn, List<CommandDetails.CommandArgumentClass> commandArgumentsIn, CommandDetails.PerformCommandTypeEnum PerformCommandTypeIn)
			{
				bool requiredFoundAfterOptional = false;
				if (commandArgumentsIn != null)
				{
					bool optionalFound = false;
					foreach (CommandDetails.CommandArgumentClass ca in commandArgumentsIn)
					{
						if (!ca.Required) optionalFound = true;
						if (optionalFound && ca.Required)
							requiredFoundAfterOptional = true;
					}
				}
				if (!requiredFoundAfterOptional)
				{
					CommandList.Add(commandNameIn.ToLower(), new CommandDetails(commandNameIn, commandPredefinedArgumentsIn, UserLabelIn, commandArgumentsIn, PerformCommandTypeIn));
					RepopulateAutoCompleteAllactionList();
				}
				else MessageBox.Show("Cannot have required parameter after optional: " + commandNameIn, "Error in argument list", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			public static void PopulateCommandList()
			{
				AddToCommandList("todo",
					new List<string>()
					{
						"120;15;Buy milk;Buy milk after work"
					},
					"todo MinutesFromNow;Autosnooze;Item name;Description",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("MinutesFromNow", true, CommandDetails.TypeArg.Int, null),
						new CommandDetails.CommandArgumentClass("AutosnoozeInterval", true, CommandDetails.TypeArg.Int, null),
						new CommandDetails.CommandArgumentClass("ItemName", true, CommandDetails.TypeArg.Text, null),
						new CommandDetails.CommandArgumentClass("ItemDescription", false, CommandDetails.TypeArg.Text, null)
					},
					CommandDetails.PerformCommandTypeEnum.AddTodoitemFirepuma);

				AddToCommandList("run",
					new List<string>()
					{
						"canary",
						"chrome",
			      "delphi2010",
			      "delphi2007",
			      "phpstorm",
			      "sqlitespy"
					},
					"run chrome/canary/delphi2010/delphi2007/phpstorm",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("TokenOrPath", true, CommandDetails.TypeArg.Text,
							new Dictionary<string,string>()
							{
								{ "chrome", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\Application\chrome.exe" },
								{ "canary", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome SxS\Application\chrome.exe" },
								{ "delphi2007", @"C:\Program Files (x86)\CodeGear\RAD Studio\5.0\bin\bds.exe" },
								{ "delphi2010", @"C:\Program Files (x86)\Embarcadero\RAD Studio\7.0\bin\bds.exe" },
								{ "phpstorm", @"C:\Program Files (x86)\JetBrains\PhpStorm 2.1.4\bin\PhpStorm.exe" },
								{ "sqlitespy", @"C:\Francois\Programs\SQLiteSpy_1.9.1\SQLiteSpy.exe" }
							})
					},
					CommandDetails.PerformCommandTypeEnum.CheckFileExistRun_ElseTryRun);

				AddToCommandList("mail",
					new List<string>()
					{
						"fhill@gls.co.za;",
						"francoishill11@gmail.com;"
					},
					"mail to;subject;body",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("Toaddress", true, CommandDetails.TypeArg.Text, null),
						new CommandDetails.CommandArgumentClass("Subject", true, CommandDetails.TypeArg.Text, null),
						new CommandDetails.CommandArgumentClass("Body", false, CommandDetails.TypeArg.Text, null)
					},
					CommandDetails.PerformCommandTypeEnum.CreateNewOutlookMessage);

				AddToCommandList("explore", null, "explore franother/prog/docs/folderpath",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("TokenOrPath", true, CommandDetails.TypeArg.Text,
							new Dictionary<string,string>()
							{
								{ "franother", @"c:\francois\other" },
								{ "prog", @"c:\programming" },
								{ "docs", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) }
							})
					},
					CommandDetails.PerformCommandTypeEnum.CheckDirectoryExistRun_ElseTryRun);

				AddToCommandList("web", null, "web google.com",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("TokenOrUrl", true, CommandDetails.TypeArg.Text, null)
					},
					CommandDetails.PerformCommandTypeEnum.WebOpenUrl);

				AddToCommandList("google", null, "google search on google",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("StringToSearchInGoogle", true, CommandDetails.TypeArg.Text, null)
					},
					CommandDetails.PerformCommandTypeEnum.WebSearchGoogle);

				AddToCommandList("kill", null, "kill processNameToKill",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("TokenOrProcessname", true, CommandDetails.TypeArg.Text, null)
					},
					CommandDetails.PerformCommandTypeEnum.KillProcess);

				AddToCommandList("startupbat",
					new List<string>()
					{
						"open",
						"getall",
						"getline xxx",
						"comment #",
						"uncomment #"
					},
					"startupbat open/getall/getline xxx/comment #/uncomment #",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("Command", true, CommandDetails.TypeArg.Text, null),
						new CommandDetails.CommandArgumentClass("ArgumentForCommand", false, CommandDetails.TypeArg.Text, null)
					},
					CommandDetails.PerformCommandTypeEnum.StartupBat);

				AddToCommandList("call",
					new List<string>()
					{
						"yolwork",
						"imqs",
						"kerry",
						"deon",
						"johann",
						"wesley",
						"honda"
					},
					"call yolwork/imqs/kerry/deon/johann/wesley/honda",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("Token", true, CommandDetails.TypeArg.Text,
							new Dictionary<string,string>()
							{
								{ "yolwork", "Yolande work: (021) 853 3564" },
								{ "imqs", "IMQS office: 021-880 2712 / 880 1632" },
								{ "kerry", "Kerry extension: 107" },
								{ "adrian", "Adrian extension: 106" },
								{ "deon",   "Deon extension: 121" },
								{ "johann", "Johann extension: 119" },
								{ "wesley", "Wesley extension: 111" },
								{ "honda",  "Honda Tygervalley: 021 910 8300" }
							})
					},
					CommandDetails.PerformCommandTypeEnum.Call);

				AddToCommandList("cmd",
					new List<string>()
					{
						"firepuma"
					},
					"cmd firepuma/folderpath",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("TokenOrPath", true, CommandDetails.TypeArg.Text,
							new Dictionary<string,string>()
							{
								{ "firepuma", @"c:\francois\websites\firepuma" }
							})
					},
					CommandDetails.PerformCommandTypeEnum.Cmd);

				AddToCommandList("vscmd",
					new List<string>()
					{
						"albionx86"
					},
					"vscmd albionx86/folderpath",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("TokenOrPath", true, CommandDetails.TypeArg.Text,
							new Dictionary<string,string>()
							{
								{ "albionx86", @"c:\devx86\Albion" }
							})
					},
					CommandDetails.PerformCommandTypeEnum.VsCmd);

				AddToCommandList("btw", null, "btw text",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("TextToUploadToBtw", true, CommandDetails.TypeArg.Text, null)
					},
					CommandDetails.PerformCommandTypeEnum.Btw);

				AddToCommandList("svncommit",
					new List<string>()
					{
						"quickaccess;",
						"wadiso5;",
						"glscore_programming;",
						"glscore_srmsdata;",
						"delphichromiumembedded;",
						"glsreports_cssjs;",
						"glsreports_xmlsql;"
					},
					"svncommit User32stuff;Log message",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("VsProjectName", true, CommandDetails.TypeArg.Text,
							new Dictionary<string,string>()
							{
								{ "wadiso5", @"C:\Programming\Wadiso5\W5Source" },
								{ "glscore_programming", @"C:\Programming\GLSCore" },
								{ "glscore_srmsdata", @"C:\Data\Delphi\GLSCore" },
								{ "delphichromiumembedded", @"C:\Users\francois.GLS\Documents\RAD Studio\Projects\TestChrome_working_svn" },
								{ "glsreports_cssjs", @"C:\ProgramData\GLS\Wadiso\Reports" },
								{ "glsreports_xmlsql", @"C:\ProgramData\GLS\ReportSQLqueries" }
							}),
						new CommandDetails.CommandArgumentClass("LogMessage", true, CommandDetails.TypeArg.Text, null)
					},
					CommandDetails.PerformCommandTypeEnum.Svncommit);

				AddToCommandList("svnupdate",
					new List<string>()
					{
						"quickaccess",
						"wadiso5",
						"glscore_programming",
						"glscore_srmsdata",
						"delphichromiumembedded",
						"glsreports_cssjs",
						"glsreports_xmlsql"
					},
					"svnupdate User32stuff",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("VsProjectName", true, CommandDetails.TypeArg.Text,
							new Dictionary<string,string>()
							{
								{ "wadiso5", @"C:\Programming\Wadiso5\W5Source" },
								{ "glscore_programming", @"C:\Programming\GLSCore" },
								{ "glscore_srmsdata", @"C:\Data\Delphi\GLSCore" },
								{ "delphichromiumembedded", @"C:\Users\francois.GLS\Documents\RAD Studio\Projects\TestChrome_working_svn" },
								{ "glsreports_cssjs", @"C:\ProgramData\GLS\Wadiso\Reports" },
								{ "glsreports_xmlsql", @"C:\ProgramData\GLS\ReportSQLqueries" }
							})
					},
					CommandDetails.PerformCommandTypeEnum.Svnupdate);

				AddToCommandList("svnstatusboth",
					new List<string>()
					{
						"quickaccess",
						"wadiso5",
						"glscore_programming",
						"glscore_srmsdata",
						"delphichromiumembedded",
						"glsreports_cssjs",
						"glsreports_xmlsql"
					},
					"svnstatusboth User32stuff",
					new List<CommandDetails.CommandArgumentClass>()
					{
						new CommandDetails.CommandArgumentClass("VsProjectName", true, CommandDetails.TypeArg.Text,
							new Dictionary<string,string>()
							{
								{ "wadiso5", @"C:\Programming\Wadiso5\W5Source" },
								{ "glscore_programming", @"C:\Programming\GLSCore" },
								{ "glscore_srmsdata", @"C:\Data\Delphi\GLSCore" },
								{ "delphichromiumembedded", @"C:\Users\francois.GLS\Documents\RAD Studio\Projects\TestChrome_working_svn" },
								{ "glsreports_cssjs", @"C:\ProgramData\GLS\Wadiso\Reports" },
								{ "glsreports_xmlsql", @"C:\ProgramData\GLS\ReportSQLqueries" }
							})
					},
					CommandDetails.PerformCommandTypeEnum.Svnstatus);
			}

			public static CommandDetails GetCommandDetailsFromTextboxText(string TextboxTextIn)
			{
				if (CommandList != null && TextboxTextIn.Contains(' '))
				{
					string tmpkey = TextboxTextIn.Substring(0, TextboxTextIn.IndexOf(' ')).ToLower();
					if (CommandList.ContainsKey(tmpkey))
						return CommandList[tmpkey];
				}
				else if (CommandList != null)
				{
					string tmpkey = TextboxTextIn.ToLower();
					if (CommandList.ContainsKey(tmpkey))
						return CommandList[tmpkey];
				}
				return null;
			}

			private static void RepopulateAutoCompleteAllactionList()
			{
				if (AutoCompleteAllactionList != null) AutoCompleteAllactionList.Clear();
				AutoCompleteAllactionList = new AutoCompleteStringCollection();
				foreach (string key in CommandList.Keys)
					AutoCompleteAllactionList.Add(CommandList[key].commandName);
			}

			public class CommandDetails
			{
				public enum TypeArg { Int, Text };
				public enum PerformCommandTypeEnum {
					CheckFileExistRun_ElseTryRun,
					CheckDirectoryExistRun_ElseTryRun,
					AddTodoitemFirepuma,
					CreateNewOutlookMessage,
					WebOpenUrl,
					WebSearchGoogle,
					KillProcess,
					StartupBat,
					Call,
					Cmd,
					VsCmd,
					Btw,
					Svncommit,
					Svnupdate,
					Svnstatus,
					Undefined };
				private const char ArgumentSeparator = ';';

				public string commandName;
				public AutoCompleteStringCollection commandPredefinedArguments;
				public string UserLabel;
				public List<CommandArgumentClass> commandArguments;
				public PerformCommandTypeEnum PerformCommandType;
				public CommandDetails(string commandNameIn, List<string> commandPredefinedArgumentsIn, string UserLabelIn, List<CommandArgumentClass> commandArgumentsIn, PerformCommandTypeEnum PerformCommandTypeIn)
				{
					commandName = commandNameIn;
					commandPredefinedArguments = new AutoCompleteStringCollection();
					if (commandPredefinedArgumentsIn != null)
						foreach (string arg in commandPredefinedArgumentsIn)
							commandPredefinedArguments.Add(commandNameIn + " " + arg);
					UserLabel = UserLabelIn;
					commandArguments = commandArgumentsIn;
					PerformCommandType = PerformCommandTypeIn;
				}

				public void PerformCommand(Form1 form1, string TextboxTextIn)
				{
					TextBox messagesTextbox = form1.textBox_Messages;
					string argStr = TextboxTextIn.Substring(TextboxTextIn.IndexOf(' ') + 1);

					switch (PerformCommandType)
					{
						case PerformCommandTypeEnum.CheckFileExistRun_ElseTryRun: case PerformCommandTypeEnum.CheckDirectoryExistRun_ElseTryRun:
							if (commandArguments.Count > 1) MessageBox.Show("More than one command argument not yet incorporated");
							else
							{
								string exepath = argStr;
								if (commandArguments[0].TokenWithReplaceStringPair != null && commandArguments[0].TokenWithReplaceStringPair.ContainsKey(exepath))
									exepath = commandArguments[0].TokenWithReplaceStringPair[exepath];
								if (
									(File.Exists(exepath) && PerformCommandType == PerformCommandTypeEnum.CheckFileExistRun_ElseTryRun) ||
									(Directory.Exists(exepath) && PerformCommandType == PerformCommandTypeEnum.CheckDirectoryExistRun_ElseTryRun))
									System.Diagnostics.Process.Start(exepath);
								else
								{
									try
									{
										System.Diagnostics.Process.Start(exepath);
									}
									catch (Exception exc) { Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, exc.Message); }
								}
							}
							break;

						case PerformCommandTypeEnum.AddTodoitemFirepuma:
							PhpInterop.AddTodoItemFirepuma(
								messagesTextbox,
								ServerAddress,
								doWorkAddress,
								Username,
								Password,
							 "QuickAccess",
							 "Quick todo",
							 argStr.Split(';')[2],
							 argStr.Split(';')[3],
							 false,
							 DateTime.Now.AddMinutes(Convert.ToInt32(argStr.Split(';')[0])),
							 DateTime.Now,
							 0,
							 false,
							 Convert.ToInt32(argStr.Split(';')[1]));
							break;

						case PerformCommandTypeEnum.CreateNewOutlookMessage:
							MicrosoftOfficeInterop.CreateNewOutlookMessage(
								form1,
										argStr.Split(';')[0],
										argStr.Split(';')[1],
										argStr.Split(';').Length >= 3 ? argStr.Split(';')[2] : "");
							break;

						case PerformCommandTypeEnum.WebOpenUrl:
							string url = argStr;
							if (!url.StartsWith("http://") && !url.StartsWith("https://") && !url.StartsWith("www."))
								url = "http://" + url;
							System.Diagnostics.Process.Start(url);
							break;

						case PerformCommandTypeEnum.WebSearchGoogle:
							System.Diagnostics.Process.Start("http://www.google.co.za/search?q=" + argStr);
							break;

						case PerformCommandTypeEnum.KillProcess:
							string processName = argStr;
							Process[] processes = Process.GetProcessesByName(processName);
							if (processes.Length > 1) Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "More than one process found, cannot kill");
							else if (processes.Length == 0) Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Cannot find process with name " + processName);
							else
							{
								processes[0].Kill();
								Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Process killed: " + processName);
							}
							break;

						case PerformCommandTypeEnum.StartupBat:
							string filePath = @"C:\Francois\Other\Startup\work Startup.bat";
							string comm = argStr;
							//getall/getline 'xxx'/comment #/uncomment #
							StartupbatInterop.PerformStartupbatCommand(messagesTextbox, filePath, comm);
							break;

						case PerformCommandTypeEnum.Call:
							if (commandArguments[0].TokenWithReplaceStringPair != null && commandArguments[0].TokenWithReplaceStringPair.ContainsKey(argStr))
								Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, commandArguments[0].TokenWithReplaceStringPair[argStr]);
							else Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Call command not recognized: " + argStr);
							break;

						case PerformCommandTypeEnum.Cmd: case PerformCommandTypeEnum.VsCmd:
							string cmdpath = argStr;
							if (commandArguments[0].TokenWithReplaceStringPair != null && commandArguments[0].TokenWithReplaceStringPair.ContainsKey(cmdpath))
								cmdpath = commandArguments[0].TokenWithReplaceStringPair[cmdpath];

							WindowsInterop.StartCommandPromptOrVScommandPrompt(messagesTextbox, cmdpath, PerformCommandType == PerformCommandTypeEnum.VsCmd);
							break;

						case PerformCommandTypeEnum.Btw:
							PhpInterop.AddBtwTextFirepuma(form1, messagesTextbox, argStr);
							break;

						case PerformCommandTypeEnum.Svncommit: case PerformCommandTypeEnum.Svnupdate: case PerformCommandTypeEnum.Svnstatus:
							string svnargs = argStr;// textBox1.Text.ToLower().Substring(10);
							//if (svncommitargs.Contains(' '))
							//{
							//string svncommand = svncommandwithargs.Substring(0, svncommandwithargs.IndexOf(' ')).ToLower();
							//string projnameAndlogmessage = svncommandwithargs.Substring(svncommandwithargs.IndexOf(' ') + 1);
							//if (svncommitargs.Contains(';'))//projnameAndlogmessage.Contains(';'))
							//{
							SvnInterop.SvnCommand svnCommand =
								PerformCommandType == PerformCommandTypeEnum.Svncommit ? SvnInterop.SvnCommand.Commit
								: PerformCommandType == PerformCommandTypeEnum.Svnupdate ? SvnInterop.SvnCommand.Update
								: PerformCommandType == PerformCommandTypeEnum.Svnstatus ? SvnInterop.SvnCommand.Status
								: SvnInterop.SvnCommand.Status;

							string svnprojname = svnargs.Contains(ArgumentSeparator) ? svnargs.Split(ArgumentSeparator)[0] : svnargs;
							string svnlogmessage = svnargs.Contains(ArgumentSeparator) ? svnargs.Split(ArgumentSeparator)[1] : "";
							if (commandArguments[0].TokenWithReplaceStringPair != null && commandArguments[0].TokenWithReplaceStringPair.ContainsKey(svnprojname))
								svnargs = commandArguments[0].TokenWithReplaceStringPair[svnprojname] + (svnargs.Contains(ArgumentSeparator) ? ArgumentSeparator + svnlogmessage : "");
							SvnInterop.PerformSvn(messagesTextbox, svnargs, svnCommand);
							//}
							//else appendLogTextbox_OfPassedTextbox(messagesTextbox, "Error: No semicolon. Command syntax is 'svncommit proj/othercommand projname;logmessage'");
							//}
							break;

						case PerformCommandTypeEnum.Undefined:
							MessageBox.Show("PerformCommandType is not defined");
							break;
						default:
							MessageBox.Show("PerformCommandType is not defined");
							break;
					}
				}

				public bool CommandHasRequiredArguments()
				{
					if (commandArguments == null) return false;
					if (commandArguments.Count == 0) return false;
					return commandArguments[0].Required;
				}

				public bool CommandHasArguments()
				{
					return commandArguments != null && commandArguments.Count > 0;
				}

				public bool CommandHasAutocompleteArguments()
				{
					return commandPredefinedArguments != null && commandPredefinedArguments.Count > 0;
				}

				public bool TextHasAllRequiredArguments(string TextboxTextIn)
				{
					if (!CommandHasRequiredArguments()) return true;
					else
					{
						int RequiredArgumentCount = 0;
						foreach (CommandArgumentClass ca in commandArguments)
							if (ca.Required)
								RequiredArgumentCount++;
						if (TextboxTextIn.Length == 0) return false;
						if (!TextboxTextIn.Contains(' ')) return false;
						string argStr = TextboxTextIn.Substring(TextboxTextIn.IndexOf(' ') + 1);
						if (argStr.Length == 0) return false;
						if (RequiredArgumentCount > 1 && !argStr.Contains(ArgumentSeparator)) return false;
						if (argStr.Split(ArgumentSeparator).Length < RequiredArgumentCount) return false;
					}
					return true;
				}

				private int GetArgumentCountFromString(string str)
				{
					return str.Split(ArgumentSeparator).Length;
				}

				public bool TextValidateArguments(string TextboxTextIn, out string Errormsg)
				{
					Errormsg = "";
					if (commandArguments == null) return true;
					string argStr = TextboxTextIn.Substring(TextboxTextIn.IndexOf(' ') + 1);
					int ArgCount = commandArguments.Count;
					if (GetArgumentCountFromString(argStr) > ArgCount) return false;
					string[] InputArguments = argStr.Split(ArgumentSeparator);
					int cnt = 0;
					foreach (string s in InputArguments)
					{
						int tmpint;
						CommandArgumentClass comm = commandArguments[cnt];
						switch (comm.TypeOfArgument)
						{
							case TypeArg.Int:
								if (comm.Required && !int.TryParse(s, out tmpint))
								{
									Errormsg = "Cannot convert argument to Integer: " + comm.ArgumentName;
									return false;
								}
								break;
							case TypeArg.Text:
								if (comm.Required && s.Length == 0)
								{
									Errormsg = "Argument may not be empty: " + comm.ArgumentName;
									return false;
								}
								break;
							default:
								break;
						}
						cnt++;
					}
					return true;
				}

				public class CommandArgumentsAndFunctionArguments
				{
					public CommandArgumentClass commandDetails;
					public Object FunctionArgumentObject;
					public CommandArgumentsAndFunctionArguments(CommandArgumentClass commandDetailsIn, Object FunctionArgumentObjectIn)
					{
						commandDetails = commandDetailsIn;
						FunctionArgumentObject = FunctionArgumentObjectIn;
					}
				}

				public class CommandArgumentClass
				{
					public delegate void functionDelegate(CommandArgumentsAndFunctionArguments args);

					public string ArgumentName;
					public bool Required;
					public TypeArg TypeOfArgument;
					public Dictionary<string, string> TokenWithReplaceStringPair;
					//public functionDelegate function;
					public CommandArgumentClass(string ArgumentNameIn, bool RequiredIn, TypeArg TypeOfArgumentIn, Dictionary<string, string> TokenWithReplaceStringPairIn)//, functionDelegate functionIn)
					{
						ArgumentName = ArgumentNameIn;
						Required = RequiredIn;
						TypeOfArgument = TypeOfArgumentIn;
						TokenWithReplaceStringPair = TokenWithReplaceStringPairIn;
						//function = functionIn;
					}
				}
			}
		}

		public Form1()
		{
			InitializeComponent();

			Commands.PopulateCommandList();

			if (!System.Diagnostics.Debugger.IsAttached && Environment.GetCommandLineArgs()[0].ToUpper().Contains("Apps\\2.0".ToUpper()))
			{
				Microsoft.Win32.RegistryKey regkeyRUN = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
				regkeyRUN.SetValue(ThisAppName, "\"" + System.Windows.Forms.Application.ExecutablePath + "\"", Microsoft.Win32.RegistryValueKind.String);
			}

			origOpacity = this.Opacity;
			this.Opacity = 0;
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
			this.ShowInTaskbar = true;
			if (!RegisterHotKey(this.Handle, Hotkey1, MOD_CONTROL, (int)Keys.Q)) MessageBox.Show("QuickAccess could not register hotkey Ctrl + Q");
			if (!RegisterHotKey(this.Handle, Hotkey2, MOD_CONTROL + MOD_SHIFT, (int)Keys.Q)) MessageBox.Show("QuickAccess could not register hotkey Ctrl + Shift + Q");
			label1.Text = AvailableActionList;
			SetAutocompleteActionList();
			//InitializeHooks(false, true);
			this.Hide();
			this.Opacity = origOpacity;
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_HOTKEY)
			{
				if (m.WParam == new IntPtr(Hotkey1))
					ToggleWindowActivation();
				if (m.WParam == new IntPtr(Hotkey2))
				{
					contextMenuStrip_TrayIcon.Show(new Point(0, 0));//Screen.PrimaryScreen.WorkingArea.Right, Screen.PrimaryScreen.WorkingArea.Bottom));//MousePosition);
					this.Activate();
					contextMenuStrip_TrayIcon.Focus();
					if (commandsToolStripMenuItem.HasDropDownItems)
					{
						//TODO: The following line actually dows nothing
						commandsToolStripMenuItem.ShowDropDown();//.DropDownItems[0].Select();
						commandsToolStripMenuItem.DropDownItems[0].Select();
					}
					else
						commandsToolStripMenuItem.Select();
				}
			}
			base.WndProc(ref m);
		}

		private bool IsAltDown()
		{
			return ((ModifierKeys & Keys.Alt) == Keys.Alt);
		}

		private bool IsAltShiftDown()
		{
			return (ModifierKeys & (Keys.Alt | Keys.Shift)) == (Keys.Alt | Keys.Shift);
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
			if (GetForegroundWindow() != this.Handle)
			{
				ShowAndActivateThisForm();
			}
			else this.Hide();
		}

		public void ShowAndActivateThisForm()
		{
			this.Visible = true;
			this.Activate();
			this.WindowState = FormWindowState.Normal;
		}

		void SetAutocompleteActionList()
		{
			if (textBox1.AutoCompleteCustomSource != Commands.AutoCompleteAllactionList)
			{
				textBox1.AutoCompleteCustomSource = Commands.AutoCompleteAllactionList;
			}
		}

		void SetAutoCompleteForAction(string action)
		{
			if (textBox1.AutoCompleteCustomSource != Commands.CommandList[action].commandPredefinedArguments && textBox1.TextLength > 2)//SelectionLength == 0)
			{
				textBox1.AutoCompleteCustomSource = Commands.CommandList[action].commandPredefinedArguments;
			}
		}

		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				PerformCommandNow(textBox1.Text);
			}
		}

		private void PerformCommandNow(string text, bool ClearCommandTextboxOnSuccess = true, bool HideAfterSuccess = false)
		{
			string errorMsg;
			Commands.CommandDetails command = Commands.GetCommandDetailsFromTextboxText(text);
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
				appendLogTextbox("");
				appendLogTextbox("Performing command: " + text);
				command.PerformCommand(this, text);
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
			string tmpkey = "987654321abcde";
			if (textBox1.Text.ToLower().IndexOf(' ') != -1)
				tmpkey = textBox1.Text.ToLower().Substring(0, textBox1.Text.ToLower().IndexOf(' '));
			if (Commands.CommandList.ContainsKey(tmpkey))
			{
				label1.Text = Commands.CommandList[tmpkey].UserLabel;
				SetAutoCompleteForAction(tmpkey);
			}
			else
			{
				label1.Text = AvailableActionList;
				if (textBox1.Text.Length == 0) SetAutocompleteActionList();
			}
		}

		private void Form1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			UnregisterHotKey(this.Handle, Hotkey1);
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

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
				ShowAndActivateThisForm();
		}

		private void contextMenuStrip_TrayIcon_Opening(object sender, CancelEventArgs e)
		{
			ToolStripDropDownDirection defaultDropDirection = ToolStripDropDownDirection.Right;//Left;

			commandsToolStripMenuItem.DropDownItems.Clear();
			commandsToolStripMenuItem.DropDownDirection = defaultDropDirection;
			if (Commands.CommandList != null)
				foreach (string key in Commands.CommandList.Keys)
				{
					Commands.CommandDetails commandDetails = Commands.CommandList[key];

					ToolStripMenuItem commanditem = new ToolStripMenuItem(key) { Tag = commandDetails };
					commanditem.DropDownDirection = defaultDropDirection;

					if (commandDetails.CommandHasArguments() || commandDetails.CommandHasAutocompleteArguments())
					{
						ToolStripMenuItem customArguments = new ToolStripMenuItem("_Custom arguments");
						customArguments.AutoSize = true;
						customArguments.DropDownDirection = defaultDropDirection;
						commanditem.DropDownItems.Add(customArguments);

						if (commandDetails.CommandHasAutocompleteArguments())
							foreach (string predefinedArguments in Commands.CommandList[key].commandPredefinedArguments)
							{
								ToolStripMenuItem subcommanditem = new ToolStripMenuItem(predefinedArguments.Substring(key.Length + 1)) { Tag = predefinedArguments };
								subcommanditem.AutoSize = true;
								subcommanditem.DropDownDirection = defaultDropDirection;
								subcommanditem.Click += delegate
								{
									PerformCommandNow(subcommanditem.Tag.ToString(), false, true);
								};
								/*subcommanditem.DropDownOpened += delegate
								{
									PerformCommandNow(subcommanditem.Tag.ToString(), false, true);
								};*/
								commanditem.DropDownItems.Add(subcommanditem);
							}

						for (int i = 0; i < commanditem.DropDownItems.Count; i++)
						{
							ToolStripMenuItem tmpsubcommandItem = commanditem.DropDownItems[i] as ToolStripMenuItem;
							if (commandDetails.CommandHasArguments())
							{
								foreach (Commands.CommandDetails.CommandArgumentClass arg in Commands.CommandList[key].commandArguments)
								{
									ToolStripTextBox subcommandTextboxitem = new ToolStripTextBox();
									subcommandTextboxitem.BackColor = arg.Required ? Color.LightGreen : Color.LightGray;
									subcommandTextboxitem.KeyDown += (sender1, e1) =>
									{
										if (e1.KeyCode == Keys.Enter)
										{
											if (subcommandTextboxitem.OwnerItem is ToolStripMenuItem
												&& (subcommandTextboxitem.OwnerItem as ToolStripMenuItem).OwnerItem is ToolStripMenuItem)
											{
												ToolStripMenuItem tmpCommandMenuItem = (subcommandTextboxitem.OwnerItem as ToolStripMenuItem).OwnerItem as ToolStripMenuItem;
												ToolStripMenuItem tmpArgumentsOwner = subcommandTextboxitem.OwnerItem as ToolStripMenuItem;

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
										}
										else if (e1.KeyCode == Keys.Left)
										{
											if (subcommandTextboxitem.Text.Length == 0 || subcommandTextboxitem.SelectionStart == 0)
												if (subcommandTextboxitem.OwnerItem is ToolStripMenuItem)
													(subcommandTextboxitem.OwnerItem as ToolStripMenuItem).HideDropDown();
										}
									};
									tmpsubcommandItem.DropDownItems.Add(subcommandTextboxitem);
								}

								if (i > 0)
								{
									string[] splittedArgs = tmpsubcommandItem.Tag.ToString().Substring(key.Length + 1).Split(';');
									for (int k = 0; k < splittedArgs.Length; k++)
										tmpsubcommandItem.DropDownItems[k].Text = splittedArgs[k];
								}
							}
						}						
					}
					else
					{
						commanditem.Click += delegate
						{
							MessageBox.Show("No subcommands");
						};
					}
					commandsToolStripMenuItem.DropDownItems.Add(commanditem);
				}
		}

		private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
		{
			ShowAndActivateThisForm();
		}
	}

	public class PhpInterop
	{
		public const string MySQLdateformat = "yyyy-MM-dd HH:mm:ss";

		public static string GetPrivateKey(TextBox messagesTextbox, string ServerAddress, string Username, string Password)
		{
			try
			{
				//toolStripStatusLabelCurrentStatus.Text = "Obtaining pvt key...";
				string tmpkey = null;

				ThreadingInterop.PerformVoidFunctionSeperateThread(() =>
				{
					tmpkey = PhpInterop.PostPHP(messagesTextbox, ServerAddress + "/generateprivatekey.php", "username=" + Username + "&password=" + Password);
				});

				string tmpSuccessKeyString = "Success: Key=";
				if (tmpkey != null && tmpkey.Length > 0 && tmpkey.ToUpper().StartsWith(tmpSuccessKeyString.ToUpper()))
				{
					tmpkey = tmpkey.Substring(tmpSuccessKeyString.Length).Replace("\n", "").Replace("\r", "");
					//toolStripStatusLabelCurrentStatus.Text = tmpkey;
				}
				return tmpkey;
			}
			catch (Exception exc)
			{
				Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Obtain private key exception: " + exc.Message);
				return null;
			}
		}

		/// <summary>
		/// Post data to php, maximum length of data is 8Mb
		/// </summary>
		/// <param name="url">The url of the php, do not include the ?</param>
		/// <param name="data">The data, i.e. "name=koos&surname=koekemoer". Note to not include the ?</param>
		/// <returns>Returns the data received from the php (usually the "echo" statements in the php.</returns>
		public static string PostPHP(TextBox messagesTextbox, string url, string data)
		{
			string vystup = "";
			ThreadingInterop.PerformVoidFunctionSeperateThread(() =>
				{
					try
					{
						data = data.Replace("+", "[|]");
						//Our postvars
						byte[] buffer = Encoding.ASCII.GetBytes(data);
						//Initialisation, we use localhost, change if appliable
						HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(url);
						//Our method is post, otherwise the buffer (postvars) would be useless
						WebReq.Method = "POST";
						//We use form contentType, for the postvars.
						WebReq.ContentType = "application/x-www-form-urlencoded";
						//The length of the buffer (postvars) is used as contentlength.
						WebReq.ContentLength = buffer.Length;
						//We open a stream for writing the postvars
						Stream PostData = WebReq.GetRequestStream();
						//Now we write, and afterwards, we close. Closing is always important!
						PostData.Write(buffer, 0, buffer.Length);
						PostData.Close();
						//Get the response handle, we have no true response yet!
						HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
						//Let's show some information about the response
						//System.Windows.Forms.MessageBox.Show(WebResp.StatusCode.ToString());
						//System.Windows.Forms.MessageBox.Show(WebResp.Server);

						//Now, we read the response (the string), and output it.
						Stream Answer = WebResp.GetResponseStream();
						StreamReader _Answer = new StreamReader(Answer);
						vystup = _Answer.ReadToEnd();

						//Congratulations, you just requested your first POST page, you
						//can now start logging into most login forms, with your application
						//Or other examples.
						string tmpresult = vystup.Trim() + "\n";
					}
					catch (Exception exc)
					{
						if (!exc.Message.ToUpper().StartsWith("The remote name could not be resolved:".ToUpper()))
							//LoggingClass.AddToLogList(UserMessages.MessageTypes.PostPHP, exc.Message);
							Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Post php: " + exc.Message);
						else //LoggingClass.AddToLogList(UserMessages.MessageTypes.PostPHPremotename, exc.Message);
							Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Post php remote name: " + exc.Message);
						//SysWinForms.MessageBox.Show("Error (092892): " + Environment.NewLine + exc.Message, "Exception error", SysWinForms.MessageBoxButtons.OK, SysWinForms.MessageBoxIcon.Error);
					}
				});
			return vystup;
		}

		public static void AddTodoItemFirepuma(TextBox messagesTextbox, string ServerAddress, string doWorkAddress, string Username, string Password, string Category, string Subcat, string Items, string Description, bool Completed, DateTime Due, DateTime Created, int RemindedCount, bool StopSnooze, int AutosnoozeInterval)
		{
			Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Adding new item, please wait...");
			bool successfulAdd = PerformDesktopAppDoTask(
				messagesTextbox,
					ServerAddress,
					doWorkAddress,
					Username,
					Password,
					"addtolist",
					new List<string>()
                {
                    Category,
                    Subcat,
                    Items,
                    Description,
                    Due.ToString(MySQLdateformat),
                    StopSnooze ? "1" : "0",
                    AutosnoozeInterval.ToString()
                },
					true,
					"1");
			if (successfulAdd)
			{
				Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Successfully added todo item.");
				//textBox1.Text = "";
			}
		}

		public static void AddBtwTextFirepuma(Form1 form1, TextBox messagesTextbox, string btwtext)
		{
			Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Sending btw text, please wait...");
			string responsestr  = PhpInterop.PostPHP(messagesTextbox, "http://firepuma.com/btw/directadd/f/" + PhpInterop.PhpEncryption.StringToHex(btwtext), "");

			Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, responsestr);
			if (responsestr.ToLower().StartsWith("success:"))
				form1.textBox1.Text = "";
			//textBox1.Text = "";
		}

		private static bool PerformDesktopAppDoTask(TextBox messagesTextbox, string ServerAddress, string doWorkAddress, string UsernameIn, string Password, string TaskName, List<string> ArgumentList, bool CheckForSpecificResult = false, string SuccessSpecificResult = "", bool MustWriteResultToLogsTextbox = false)
		{
			string result = GetResultOfPerformingDesktopAppDoTask(messagesTextbox, ServerAddress, doWorkAddress, UsernameIn, Password, TaskName, ArgumentList, MustWriteResultToLogsTextbox);
			if (CheckForSpecificResult && result == SuccessSpecificResult)
				return true;
			return false;
		}

		private static string GetResultOfPerformingDesktopAppDoTask(TextBox messagesTextbox, string ServerAddress,string doWorkAddress, string Username, string Password, string TaskName, List<string> ArgumentList, bool MustWriteResultToLogsTextbox = false)
		{
			string tmpkey = GetPrivateKey(messagesTextbox, ServerAddress, Username, Password);
			Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Obtained private key");

			if (tmpkey != null)
			{
				HttpWebRequest addrequest = null;
				HttpWebResponse addresponse = null;
				StreamReader input = null;

				try
				{
					if (Username != null && Username.Length > 0
																 && tmpkey != null && tmpkey.Length > 0)
					{
						string encryptedstring;
						string decryptedstring = "";
						bool mustreturn = false;
						ThreadingInterop.PerformVoidFunctionSeperateThread(() =>
						{
							string ArgumentListTabSeperated = "";
							foreach (string s in ArgumentList)
								ArgumentListTabSeperated += (ArgumentListTabSeperated.Length > 0 ? "\t" : "") + s;

							string tmpRequest = doWorkAddress + "/dotask/" +
									PhpInterop.PhpEncryption.SimpleTripleDesEncrypt(Username, "123456789abcdefghijklmno") + "/" +
									PhpInterop.PhpEncryption.SimpleTripleDesEncrypt(TaskName, tmpkey) + "/" +
									PhpInterop.PhpEncryption.SimpleTripleDesEncrypt(ArgumentListTabSeperated, tmpkey);
							addrequest = (HttpWebRequest)WebRequest.Create(tmpRequest);// + "/");
							//appendLogTextbox(addrequest.RequestUri.ToString());
							try
							{
								addresponse = (HttpWebResponse)addrequest.GetResponse();
								input = new StreamReader(addresponse.GetResponseStream());
								encryptedstring = input.ReadToEnd();
								//appendLogTextbox("Encrypted response: " + encryptedstring);

								decryptedstring = PhpInterop.PhpEncryption.SimpleTripleDesDecrypt(encryptedstring, tmpkey);
								//appendLogTextbox("Decrypted response: " + decryptedstring);
								decryptedstring = decryptedstring.Replace("\0", "").Trim();
								//MessageBox.Show(this, decryptedstring);
								if (MustWriteResultToLogsTextbox) Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Result for " + TaskName + ": " + decryptedstring);
								mustreturn = true;
							}
							catch (Exception exc) { MessageBox.Show("Exception:" + exc.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error); }
						});
						if (mustreturn) return decryptedstring;
					}
				}
				catch (Exception exc)
				{
					Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Obtain php: " + exc.Message);
				}
				finally
				{
					if (addresponse != null) addresponse.Close();
					if (input != null) input.Close();
				}
			}
			return null;
		}

		public class PhpEncryption
		{
			public static string SimpleTripleDesEncrypt(string Data, string keystring)
			{
				byte[] key = Encoding.ASCII.GetBytes(keystring);
				byte[] iv = Encoding.ASCII.GetBytes("password");
				byte[] data = Encoding.ASCII.GetBytes(Data);
				byte[] enc = new byte[0];
				TripleDES tdes = TripleDES.Create();
				tdes.IV = iv;
				tdes.Key = key;
				tdes.Mode = CipherMode.CBC;
				tdes.Padding = PaddingMode.Zeros;
				ICryptoTransform ict = tdes.CreateEncryptor();
				enc = ict.TransformFinalBlock(data, 0, data.Length);
				return ByteArrayToString(enc);
			}

			public static string SimpleTripleDesDecrypt(string Data, string keystring)
			{
				byte[] key = Encoding.ASCII.GetBytes(keystring);
				byte[] iv = Encoding.ASCII.GetBytes("password");
				byte[] data = StringToByteArray(Data);
				byte[] enc = new byte[0];
				TripleDES tdes = TripleDES.Create();
				tdes.IV = iv;
				tdes.Key = key;
				tdes.Mode = CipherMode.CBC;
				tdes.Padding = PaddingMode.Zeros;
				ICryptoTransform ict = tdes.CreateDecryptor();
				enc = ict.TransformFinalBlock(data, 0, data.Length);
				return Encoding.ASCII.GetString(enc);
			}

			public static string ByteArrayToString(byte[] ba)
			{
				string hex = BitConverter.ToString(ba);
				return hex.Replace("-", "");
			}

			public static string StringToHex(string stringIn)
			{
				return PhpEncryption.ByteArrayToString(Encoding.Default.GetBytes(stringIn));
			}

			public static byte[] StringToByteArray(String hex)
			{
				int NumberChars = hex.Length;
				byte[] bytes = new byte[NumberChars / 2];
				for (int i = 0; i < NumberChars; i += 2)
					bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
				return bytes;
			}

			public static string HexToString(string hexIn)
			{
				return Encoding.Default.GetString(StringToByteArray(hexIn));
			}
		}
	}

	public class Logging
	{
		public static void appendLogTextbox_OfPassedTextbox(TextBox messagesTextbox, string str)
		{
			//label1.Text = str;
			Control topParent = messagesTextbox;
			while (topParent != null && topParent != messagesTextbox.Parent) topParent = messagesTextbox.Parent;
			if (topParent is Form1)// && (topParent as Form1).Visible)
				(topParent as Form1).notifyIcon1.ShowBalloonTip(3000, "Message", str, ToolTipIcon.Info);
			messagesTextbox.Text = str + (messagesTextbox.Text.Length > 0 ? Environment.NewLine : "") + messagesTextbox.Text;
			Application.DoEvents();
		}
	}

	public class ThreadingInterop
	{
		public static void PerformVoidFunctionSeperateThread(MethodInvoker method)
		{
			System.Threading.Thread th = new System.Threading.Thread(() =>
			{
				method.Invoke();
			});
			th.Start();
			//th.Join();
			while (th.IsAlive) { Application.DoEvents(); }
		}
	}

	public class MicrosoftOfficeInterop
	{
		public static void CreateNewOutlookMessage(Form1 form1, string To, string Subject, string Body)
		{
			ThreadingInterop.PerformVoidFunctionSeperateThread(() =>
				{
					if (Process.GetProcessesByName("Outlook").Length == 0)
					{
						Logging.appendLogTextbox_OfPassedTextbox(form1.textBox_Messages, "Starting Outlook, please wait...");
						Process p = System.Diagnostics.Process.Start("Outlook");
					}

					// Creates a new Outlook Application Instance
					Outlook.Application objOutlook = new Outlook.Application();
					// Creating a new Outlook Message from the Outlook Application Instance
					Outlook.MailItem mic = (Outlook.MailItem)(objOutlook.CreateItem(Outlook.OlItemType.olMailItem));
					mic.To = To;
					mic.Subject = Subject;
					mic.Body = Body;
					form1.TopMost = false;
					mic.Display(true);
					form1.TopMost = true;
					form1.ShowAndActivateThisForm();
				});
		}
	}

	public class SvnInterop
	{
		public enum SvnCommand { Commit, Update, Status };

		public static void PerformSvn(TextBox messagesTextbox, string svnargs, SvnCommand svnCommand)
		{
			string projnameOrDir = svnargs.Split(';')[0];//projnameAndlogmessage.Split(';')[0];
			string logmessage = null;
			if (svnCommand == SvnCommand.Commit)
			{
				logmessage = svnargs.Split(';')[1];//projnameAndlogmessage.Split(';')[1];
				logmessage = logmessage.Replace("\\", "\\\\");
				logmessage = logmessage.Replace("\"", "\\\"");
			}
			try
			{
				string projDir =
					Directory.Exists(projnameOrDir) ? projnameOrDir :
					Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Visual Studio 2010\Projects\" + projnameOrDir;//"";
				string svnpath = "svn";
				if (Directory.Exists(projDir))
				{
					ThreadingInterop.PerformVoidFunctionSeperateThread(() =>
					{
						string processArguments =
											svnCommand ==
							SvnCommand.Commit ? "commit -m\"" + logmessage + "\" \"" + projDir + "\""
							: svnCommand == SvnCommand.Update ? "update \"" + projDir + "\""
							: svnCommand == SvnCommand.Status ? "status --show-updates \"" + projDir + "\""
							: "";

						ProcessStartInfo start = new ProcessStartInfo(svnpath, processArguments);//"commit -m\"" + logmessage + "\" \"" + projDir + "\"");
						start.UseShellExecute = false;
						start.CreateNoWindow = true;
						start.RedirectStandardOutput = true;
						start.RedirectStandardError = true;
						System.Diagnostics.Process svnproc = new Process();
						svnproc.OutputDataReceived += delegate(object sendingProcess, DataReceivedEventArgs outLine)
						{
							if (outLine.Data != null && outLine.Data.Trim().Length > 0) Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Svn output: " + outLine.Data);
							//else appendLogTextbox("Svn output empty");
						};
						svnproc.ErrorDataReceived += delegate(object sendingProcess, DataReceivedEventArgs outLine)
						{
							if (outLine.Data != null && outLine.Data.Trim().Length > 0) Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Svn error: " + outLine.Data);
							//else appendLogTextbox("Svn error empty");
						};
						svnproc.StartInfo = start;

						string performingPleasewaitMsg = 
							svnCommand == SvnCommand.Commit ? "Performing svn commit, please wait..."
							: svnCommand == SvnCommand.Update ? "Performing svn update, please wait..."
							: svnCommand == SvnCommand.Status ? "Check status of svn (local and server), please wait..."
							: "";
						if (svnproc.Start())
							Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, performingPleasewaitMsg);
						else Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Error: Could not start SVN process.");

						svnproc.BeginOutputReadLine();
						svnproc.BeginErrorReadLine();

						svnproc.WaitForExit();
					});
				}
				else Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Error: folder not found: " + projDir);
			}
			catch (Exception exc)
			{
				Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Exception on running svn: " + exc.Message);
			}
		}
	}

	public class StartupbatInterop
	{
		public static void PerformStartupbatCommand(TextBox messagesTextbox, string filePath, string comm)
		{
			if (!File.Exists(filePath))
				Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "File not found: " + filePath);
			else if (comm.StartsWith("open"))
			{
				System.Diagnostics.Process.Start("notepad", filePath);
			}
			else if (comm.StartsWith("getall"))
			{
				StreamReader sr = new StreamReader(filePath);
				string line = sr.ReadLine();
				int counter = 1;
				while (!sr.EndOfStream)
				{
					Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, (counter++) + ": " + line);
					line = sr.ReadLine();
				}
				sr.Close();
			}
			else if (comm.StartsWith("getline"))
			{
				if (comm.StartsWith("getline ") && comm.Length >= 9)
				{
					string searchstr = comm.Substring(8);//comm.Split('\'')[1];
					StreamReader sr = new StreamReader(filePath);
					string line = sr.ReadLine();
					int counter = 1;
					while (!sr.EndOfStream)
					{
						if (line.ToLower().Contains(searchstr)) Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, counter + ": " + line);
						counter++;
						line = sr.ReadLine();
					}
					sr.Close();
				}
				else Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Getline search string not defined (must be i.e. getline skype): " + comm);
			}
			else if (comm.StartsWith("comment"))
			{
				string linenumstr = comm.Substring(7).Trim();
				int linenum;
				if (!int.TryParse(linenumstr, out linenum)) Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Cannot obtain line number from: " + comm.Substring(7));
				else
				{
					Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Commenting line number " + linenum.ToString());
					List<string> tmpLines = new List<string>();
					StreamReader sr = new StreamReader(filePath);
					string line = sr.ReadLine();
					int counter = 1;
					while (!sr.EndOfStream)
					{
						if (counter == linenum && !line.Trim().StartsWith("::")) line = "::" + line;
						tmpLines.Add(line);
						counter++;
						line = sr.ReadLine();
					}
					sr.Close();
					StreamWriter sw = new StreamWriter(filePath);
					try
					{
						foreach (string s in tmpLines) sw.WriteLine(s);
					}
					finally { sw.Close(); }
					Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Successfully commented line number " + linenum.ToString());
				}
			}
			else if (comm.StartsWith("uncomment"))
			{
				string linenumstr = comm.Substring(9).Trim();
				int linenum;
				if (!int.TryParse(linenumstr, out linenum)) Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Cannot obtain line number from: " + comm.Substring(9));
				else
				{
					Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Unommenting line number " + linenum.ToString());
					List<string> tmpLines = new List<string>();
					StreamReader sr = new StreamReader(filePath);
					string line = sr.ReadLine();
					int counter = 1;
					while (!sr.EndOfStream)
					{
						if (counter == linenum && line.Trim().StartsWith("::")) line = line.Substring(2);
						tmpLines.Add(line);
						counter++;
						line = sr.ReadLine();
					}
					sr.Close();
					StreamWriter sw = new StreamWriter(filePath);
					try
					{
						foreach (string s in tmpLines) sw.WriteLine(s);
					}
					finally { sw.Close(); }
					Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Successfully uncommented line number " + linenum.ToString());
				}
			}
		}
	}

	public class WindowsInterop
	{
		public static void StartCommandPromptOrVScommandPrompt(TextBox messagesTextbox, string cmdpath, bool VisualStudioMode)
		{
			const string vsbatfile = @"c:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat";

			string processArgs = 
								VisualStudioMode ? @"/k """ + vsbatfile + @""" x86"
				: "";

			if (Directory.Exists(cmdpath))
			{
				if (!VisualStudioMode || (VisualStudioMode && File.Exists(vsbatfile)))
				{
					Process proc = new Process();
					proc.StartInfo = new ProcessStartInfo("cmd", processArgs);
					proc.StartInfo.WorkingDirectory = cmdpath;
					proc.Start();
				}
				else Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, @"Unable to start Visual Studio Command Prompt, cannot find file: """ + vsbatfile + @"""" + cmdpath);
			}
			else Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Folder does not exist, cannot start cmd: " + cmdpath);
		}
	}
}