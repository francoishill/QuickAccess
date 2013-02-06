using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
//using InterfaceForQuickAccessPlugin;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;
using SharedClasses;

namespace KillCommandPlugin
{
	public class KillCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "kill"; } }
		public override string DisplayName { get { return "Kill a Process"; } }
		public override string Description { get { return "Kills a process"; } }
		public override string ArgumentsExample { get { return "notepad"; } }

		private ObservableCollection<string> LastProcessList;
		private readonly ObservableCollection<string>[] predefinedArgumentsList =
				{
					new ObservableCollection<string>() { }//Empty collection because populated on demand
				};
		public override ObservableCollection<string>[] PredefinedArgumentsList
		{
			get
			{
				if (LastProcessList == null) LastProcessList = new ObservableCollection<string>();
				List<string> tmpList = new List<string>();
				Process[] processes = System.Diagnostics.Process.GetProcesses();
				foreach (Process proc in processes)
					tmpList.Add(proc.ProcessName);
				tmpList.Sort();
				for (int i = LastProcessList.Count - 1; i >= 0; i--)
					if (!tmpList.Contains(LastProcessList[i]))
						LastProcessList.RemoveAt(i);
				foreach (string item in tmpList)
					if (!LastProcessList.Contains(item))
						LastProcessList.Add(item);
				predefinedArgumentsList[0] = LastProcessList;
				return predefinedArgumentsList;
			}
		}

		private readonly Dictionary<string, string>[] argumentsReplaceKeyValuePair =
				{
				};
		public override Dictionary<string, string>[] ArgumentsReplaceKeyValuePair { get { return argumentsReplaceKeyValuePair; } }

		public override bool PreValidateArgument(out string errorMessage, int index, string argumentValue)
		{
			errorMessage = "";
			if (index < argumentsReplaceKeyValuePair.Length && argumentsReplaceKeyValuePair[index].ContainsKey(argumentValue))
				argumentValue = argumentsReplaceKeyValuePair[index][argumentValue];
			if (index != 0)
				errorMessage = "Only one argument required for Kill command";
			else if (index == 0 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "First argument of Kill command may not be null/empty/whitespaces";
			else if (index == 0 && !GetPredefinedArgumentsList(index).Contains(argumentValue))
				errorMessage = "Process not found in running list: " + argumentValue;
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			errorMessage = "";
			if (arguments.Length != 1) errorMessage = "Exactly one argument required for Kill command";
			else if (!PreValidateArgument(out errorMessage, 0, arguments[0]))
				errorMessage = errorMessage + "";
			else return true;
			return false;
		}

		public override bool PerformCommand(out string errorMessage, TextFeedbackEventHandler textFeedbackEvent = null, ProgressChangedEventHandler progressChangedEvent = null, params string[] arguments)
		{
			try
			{
				errorMessage = "";
				string processName = arguments[0];
				Process[] processes = Process.GetProcessesByName(processName);
				if (processes.Length > 1) errorMessage = "More than one process found, cannot kill";
				else if (processes.Length == 0) errorMessage = "Cannot find process with name ";
				else
				{
					if (UserMessages.Confirm("Confirm to kill process '" + processes[0].ProcessName + "'"))
					{
						processes[0].Kill();
						TextFeedbackEventArgs.RaiseTextFeedbackEvent_Ifnotnull(this, textFeedbackEvent, "Process killed: " + processName, TextFeedbackType.Noteworthy);
						errorMessage = "";

						//Logging.appendLogTextbox_OfPassedTextbox(messagesTextbox, "Process killed: " + processName);
					}
					else errorMessage = "User cancelled to kill process";
					return true;
				}
				return false;
			}
			catch (Exception exc)
			{
				errorMessage = "Cannot kill process: " + arguments[0] + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
				{
					new CommandArgument("", "process name", new ObservableCollection<string>())
				};
		public override CommandArgument[] AvailableArguments { get { return availableArguments; } set { availableArguments = value; } }

		//private KeyAndValuePair[] availableArgumentAndDescriptionsPair = new KeyAndValuePair[]
		//{
		//	new KeyAndValuePair("", "process name")
		//};
		//public override KeyAndValuePair[] AvailableArgumentAndDescriptionsPair { get { return availableArgumentAndDescriptionsPair; } set { availableArgumentAndDescriptionsPair = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments { get { return 1; } }
	}
}
