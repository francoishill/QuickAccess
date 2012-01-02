using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
using InterfaceForQuickAccessPlugin;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;

namespace TracXmlRpcPlugin
{
	public class TracXmlRpcCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "tracxmlrpc"; } }
		public override string DisplayName { get { return "TracXmlRpc"; } }
		public override string Description { get { return "Get information about Trac projects"; } }
		public override string ArgumentsExample { get { return @"QuickAccess"; } }

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
		{
			new ObservableCollection<string>() { "getfieldlables", "getticketids", "getlistofmethods" },
			new ObservableCollection<string>() { "QuickAccess", "MonitorSystem", "SharedClasses" }
		};
		public override ObservableCollection<string>[] PredefinedArgumentsList { get { return predefinedArgumentsList; } }

		private readonly Dictionary<string, string>[] argumentsReplaceKeyValuePair =
		{
		};
		public override Dictionary<string, string>[] ArgumentsReplaceKeyValuePair { get { return argumentsReplaceKeyValuePair; } }

		public override bool PreValidateArgument(out string errorMessage, int Index, string argumentValue)
		{
			errorMessage = "";
			if (Index < argumentsReplaceKeyValuePair.Length && argumentsReplaceKeyValuePair[Index].ContainsKey(argumentValue))
				argumentValue = argumentsReplaceKeyValuePair[Index][argumentValue];
			if (Index >= 2)
				errorMessage = "More than 2 arguments not allowed for Trac XmlRpc command (sub-command, project)";
			else if (Index == 0 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "First argument (sub-command) of Trac XmlRpc command may not be null/empty/whitespaces";
			else if (Index == 0 && !(predefinedArgumentsList[0].ToArray()).Contains(argumentValue))
				errorMessage = "First argument of Trac XmlRpc command is an invalid sub-command";
			else if (Index == 1 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "Second argument (project) of Trac XmlRpc command may not be null/empty/whitespaces";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			errorMessage = "";
			if (arguments.Length != 2) errorMessage = "Exactly 2 arguments required for Trac XmlRpc command (sub-command, project)";
			else if (!PreValidateArgument(out errorMessage, 0, arguments[0]))
				errorMessage = errorMessage + "";
			else if (!PreValidateArgument(out errorMessage, 1, arguments[1]))
				errorMessage = errorMessage + "";
			else return true;
			return false;
		}

		public override bool PerformCommand(out string errorMessage, TextFeedbackEventHandler textFeedbackEvent = null, ProgressChangedEventHandler progressChangedEvent = null, params string[] arguments)
		{
			try
			{
				if (arguments[0] == "getfieldlables")
				{
					ThreadingInterop.PerformVoidFunctionSeperateThread(() =>
					{
						List<string> tmplist = TracXmlRpcInterop.GetFieldLables(VisualStudioInterop.GetTracXmlRpcHttpPathFromProjectName(arguments[1]));
						int tmpcounter = 1;
						foreach (string s in tmplist)
							TextFeedbackEventArgs.RaiseTextFeedbackEvent_Ifnotnull(
								this,
								textFeedbackEvent,
								"getfieldlables " + tmpcounter++ + ": " + s,
								TextFeedbackType.Noteworthy);
					},
					ThreadName: "getfieldlabels");
				}
				else if (arguments[0] == "getticketids")
				{
					ThreadingInterop.PerformVoidFunctionSeperateThread(() =>
					{
						int[] ids = TracXmlRpcInterop.GetTicketIds(VisualStudioInterop.GetTracXmlRpcHttpPathFromProjectName(arguments[1]));
						int tmpcounter = 1;
						foreach (int i in ids)
							TextFeedbackEventArgs.RaiseTextFeedbackEvent_Ifnotnull(
								this,
								textFeedbackEvent,
								"getticketids " + tmpcounter++ + ": " + i.ToString(),
								TextFeedbackType.Noteworthy);
					},
					ThreadName: "getticketids");
				}
				else if (arguments[0] == "getlistofmethods")
				{
					ThreadingInterop.PerformVoidFunctionSeperateThread(() =>
					{
						string[] arr = TracXmlRpcInterop.GetListOfMethods(VisualStudioInterop.GetTracXmlRpcHttpPathFromProjectName(arguments[1]));
						int tmpcounter = 1;
						foreach (string s in arr)
							TextFeedbackEventArgs.RaiseTextFeedbackEvent_Ifnotnull(
								this,
								textFeedbackEvent,
								"getlistofmethods " + tmpcounter++ + ": " + s.ToString(),
								TextFeedbackType.Noteworthy);
					},
					ThreadName: "getlistofmethods");
				}
				else
				{
					errorMessage = "Invalid sub-command for Trac XmlRpc command: " + arguments[0];
					return false;
				}
				errorMessage = "";
				return true;
			}
			catch (Exception exc)
			{
				errorMessage = "Cannot perform Trac XmlRpc command: " + arguments[0] + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
			{
				new CommandArgument("", "sub-command", new ObservableCollection<string>() { "getfieldlables", "getticketids", "getlistofmethods" }),
				new CommandArgument("", "project name", new ObservableCollection<string>() { "QuickAccess", "MonitorSystem", "SharedClasses" })
			};
		public override CommandArgument[] AvailableArguments { get { return availableArguments; } set { availableArguments = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments { get { return 2; } }
	}
}
