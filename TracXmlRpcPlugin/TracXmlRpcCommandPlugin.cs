using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
//using InterfaceForQuickAccessPlugin;
using SharedClasses;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;

namespace TracXmlRpcPlugin
{
	public class TracXmlRpcCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "tracxmlrpc"; } }
		public override string DisplayName { get { return "Trac Xml Rpc"; } }
		public override string Description { get { return "Get information about Trac projects"; } }
		public override string ArgumentsExample { get { return @"QuickAccess"; } }

		private enum SubCommandsEnum { GetFieldLabels, GetTicketIDs, GetListOfMethods, ChangeLogs, GetFieldValuesOfTicket, GetAllTicketDescriptionsAndTypes }

		private static List<string> subCommandList;
		private static List<string> SubCommandList { get { if (subCommandList == null) subCommandList = EnumsInterop.GetStringListOfEnumNames(typeof(SubCommandsEnum)); return subCommandList; } }
		private readonly static string[] projectNameList = new string[] { "QuickAccess", "MonitorSystem", "SharedClasses" };
		private readonly static string[] TicketIds = new string[] { "1" };

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
		{
			new ObservableCollection<string>(SubCommandList),
			new ObservableCollection<string>(projectNameList)
		};
		public override ObservableCollection<string>[] PredefinedArgumentsList { get { return predefinedArgumentsList; } }

		private readonly Dictionary<string, string>[] argumentsReplaceKeyValuePair =
		{
		};
		public override Dictionary<string, string>[] ArgumentsReplaceKeyValuePair { get { return argumentsReplaceKeyValuePair; } }

		public override bool PreValidateArgument(out string errorMessage, int index, string argumentValue)
		{
			errorMessage = "";
			if (index < argumentsReplaceKeyValuePair.Length && argumentsReplaceKeyValuePair[index].ContainsKey(argumentValue))
				argumentValue = argumentsReplaceKeyValuePair[index][argumentValue];
			if (index >= 3)
				errorMessage = "More than 3 arguments not allowed for Trac XmlRpc command (sub-command, project)";
			else if (index == 0 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "First argument (sub-command) of Trac XmlRpc command may not be null/empty/whitespaces";
			else if (index == 0 && !(predefinedArgumentsList[0].ToArray()).Contains(argumentValue))
				errorMessage = "First argument of Trac XmlRpc command is an invalid sub-command";
			else if (index == 1 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "Second argument (project) of Trac XmlRpc command may not be null/empty/whitespaces";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			errorMessage = "";

			if (arguments.Length < 2) errorMessage = "At least 2 arguments required for Trac XmlRpc command (sub-command, project, ticketid)";
			else if (arguments.Length > 3) errorMessage = "More than 3 arguments not allowed for Trac XmlRpc command (sub-command, project, ticketid)";
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
				if (string.Equals(arguments[0], SubCommandsEnum.GetFieldLabels.ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					//DONE: Warning, if the password userprompt pops up it crashes because the commands are run on separate threads
					List<string> tmplist = TracXmlRpcInterop.GetFieldLables(TracXmlRpcInterop.ChangeLogs.GetTracXmlRpcUrlForApplication(arguments[1]));
					int tmpcounter = 1;
					foreach (string s in tmplist)
						TextFeedbackEventArgs.RaiseTextFeedbackEvent_Ifnotnull(
							this,
							textFeedbackEvent,
							"getfieldlables " + tmpcounter++ + ": " + s,
							TextFeedbackType.Noteworthy);
				}
				else if (string.Equals(arguments[0], SubCommandsEnum.GetTicketIDs.ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					int[] ids = TracXmlRpcInterop.GetOpenTicketIds(
						err => TextFeedbackEventArgs.RaiseSimple(textFeedbackEvent, err, TextFeedbackType.Error),
						TracXmlRpcInterop.ChangeLogs.GetTracXmlRpcUrlForApplication(arguments[1]));
					int tmpcounter = 1;
					foreach (int i in ids)
						TextFeedbackEventArgs.RaiseTextFeedbackEvent_Ifnotnull(
							this,
							textFeedbackEvent,
							"getticketids " + tmpcounter++ + ": " + i.ToString(),
							TextFeedbackType.Noteworthy);
				}
				else if (string.Equals(arguments[0], SubCommandsEnum.GetListOfMethods.ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					string[] arr = TracXmlRpcInterop.GetListOfMethods(TracXmlRpcInterop.ChangeLogs.GetTracXmlRpcUrlForApplication(arguments[1]));
					int tmpcounter = 1;
					foreach (string s in arr)
						TextFeedbackEventArgs.RaiseTextFeedbackEvent_Ifnotnull(
							this,
							textFeedbackEvent,
							"getlistofmethods " + tmpcounter++ + ": " + s.ToString(),
							TextFeedbackType.Noteworthy);
				}
				else if (string.Equals(arguments[0], SubCommandsEnum.ChangeLogs.ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					int tmpticketid;
					if (!int.TryParse(arguments[2], out tmpticketid))
					{
						errorMessage = "Argument 3 of Trac XmlRpc must be a valid integer.";
						return false;
					}
					else
					{
						List<TracXmlRpcInterop.ChangeLogStruct> tmplist = TracXmlRpcInterop.GetChangeLogs(tmpticketid, TracXmlRpcInterop.ChangeLogs.GetTracXmlRpcUrlForApplication(arguments[1]));
						int tmpcounter = 1;
						foreach (TracXmlRpcInterop.ChangeLogStruct cl in tmplist)
							if (cl.Field == "comment" && !string.IsNullOrWhiteSpace(cl.NewValue))
								TextFeedbackEventArgs.RaiseTextFeedbackEvent_Ifnotnull(
									this,
									textFeedbackEvent,
									"changelogs " + tmpcounter++ + ": " + "Ticket #" + tmpticketid + ", " + cl.Field + ": " + cl.NewValue,
									TextFeedbackType.Noteworthy);
					}
				}
				else if (string.Equals(arguments[0], SubCommandsEnum.GetFieldValuesOfTicket.ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					int tmpticketid;
					if (!int.TryParse(arguments[2], out tmpticketid))
					{
						errorMessage = "Argument 3 of Trac XmlRpc must be a valid integer.";
						return false;
					}
					else
					{
						Dictionary<string, object> fieldvalues = TracXmlRpcInterop.GetFieldValuesOfTicket(tmpticketid, TracXmlRpcInterop.ChangeLogs.GetTracXmlRpcUrlForApplication(arguments[1]));
						int tmpcounter = 1;
						foreach (string key in fieldvalues.Keys)
							TextFeedbackEventArgs.RaiseTextFeedbackEvent_Ifnotnull(
								this,
								textFeedbackEvent,
								"getfieldvaluesofticket " + tmpcounter++ + ": " + "Ticket #" + tmpticketid + ", " + key + ": " + fieldvalues[key].ToString(),
								TextFeedbackType.Noteworthy);
					}
				}
				else if (string.Equals(arguments[0], SubCommandsEnum.GetAllTicketDescriptionsAndTypes.ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					Dictionary<int, TracXmlRpcInterop.TracTicketDetails> ticketDescriptionsAndTypes = TracXmlRpcInterop.GetAllClosedTicketDescriptionsAndTypes(TracXmlRpcInterop.ChangeLogs.GetTracXmlRpcUrlForApplication(arguments[1]));
					int tmpcounter = 1;
					foreach (int i in ticketDescriptionsAndTypes.Keys)
						TextFeedbackEventArgs.RaiseTextFeedbackEvent_Ifnotnull(
							this,
							textFeedbackEvent,
							"getallticketdescriptions " + tmpcounter++ + ", #" + i + " (" + ticketDescriptionsAndTypes[i].TicketType.ToString() + "): " + ticketDescriptionsAndTypes[i].Description,
							TextFeedbackType.Noteworthy);
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
				new CommandArgument("", "sub-command", new ObservableCollection<string>(SubCommandList)),
				new CommandArgument("", "project name", new ObservableCollection<string>(projectNameList)),
				new CommandArgument("", "ticket id", new ObservableCollection<string>(TicketIds))
			};
		public override CommandArgument[] AvailableArguments { get { return availableArguments; } set { availableArguments = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments
		{
			get
			{
				if (currentArguments.Count > 0 && (new string[] 
					{
						SubCommandsEnum.ChangeLogs.ToString().ToLower(),
						SubCommandsEnum.GetFieldValuesOfTicket.ToString().ToLower() 
					}).Contains(currentArguments[0].CurrentValue.ToLower()))
					return 3;
				else return 2;
			}
		}
	}
}
