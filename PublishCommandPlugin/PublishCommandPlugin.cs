using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
using InterfaceForQuickAccessPlugin;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;

namespace PublishCommandPlugin
{
	public class PublishCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "publish"; } }
		public override string DisplayName { get { return "Publish VS Project"; } }
		public override string Description { get { return "Perform publish command(s) on a folder"; } }
		public override string ArgumentsExample { get { return @"localvs c:\dev86\myproject1"; } }

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
		{
			new ObservableCollection<string>() { "localvs", "onlinevs" },
			new ObservableCollection<string>() { "QuickAccess", "MonitorSystem" },
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
				errorMessage = "More than 2 arguments not allowed for Publish command (sub-command, folder/project)";
			else if (Index == 0 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "First argument (sub-command) of Publish command may not be null/empty/whitespaces";
			else if (Index == 0 && !(predefinedArgumentsList[0].ToArray()).Contains(argumentValue))
				errorMessage = "First argument of Publish command is an invalid sub-command";
			else if (Index == 1 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "Second argument (folder) of Publish command may not be null/empty/whitespaces";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			//minutes, autosnooze, name, desc
			errorMessage = "";
			if (arguments.Length != 2) errorMessage = "Exactly 2 arguments required for Publish command (sub-command, folder/project)";
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
				if (arguments[0] == "localvs")
				{
					string tmpNoUseVersionStr;
					VisualStudioInterop.PerformPublish(
						textfeedbackSenderObject: this,
						projName: arguments[1],
						versionString: out tmpNoUseVersionStr,
						HasPlugins: UserMessages.Confirm("Does the application have plugins (in a Plugins subfolder of the binaries)?"),
						AutomaticallyUpdateRevision: UserMessages.Confirm("Update the revision also?"),
						WriteIntoRegistryForWindowsAutostartup: UserMessages.Confirm("Auto startup with windows (written into registry)?"),
						textFeedbackEvent: textFeedbackEvent);
				}
				else if (arguments[0] == "onlinevs")
				{
					VisualStudioInterop.PerformPublishOnline(
							 textfeedbackSenderObject: this,
							 projName: arguments[1],
							 HasPlugins: UserMessages.Confirm("Does the application have plugins (in a Plugins subfolder of the binaries)?"),
							 AutomaticallyUpdateRevision: UserMessages.Confirm("Update the revision also?"),
							 WriteIntoRegistryForWindowsAutostartup: UserMessages.Confirm("Auto startup with windows (written into registry)?"),
							 textFeedbackEvent: textFeedbackEvent,
							 progressChanged: progressChangedEvent);
				}
				else
				{
					errorMessage = "Invalid sub-command for Publish: " + arguments[0];
					return false;
				}
				errorMessage = "";
				return true;
			}
			catch (Exception exc)
			{
				errorMessage = "Cannot perform Publish command: " + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
		{
			new CommandArgument("", "sub-command", new ObservableCollection<string>() { "localvs", "onlinevs" }),
			new CommandArgument("", "folder/path", new ObservableCollection<string>() { "QuickAccess", "MonitorSystem"})
		};
		public override CommandArgument[] AvailableArguments { get { return availableArguments; } set { availableArguments = value; } }

		//private KeyAndValuePair[] availableArgumentAndDescriptionsPair = new KeyAndValuePair[]
		//{
		//	new KeyAndValuePair("", "sub-command"),
		//	new KeyAndValuePair("", "Folder/Path"),
		//};
		//public override KeyAndValuePair[] AvailableArgumentAndDescriptionsPair { get { return availableArgumentAndDescriptionsPair; } set { availableArgumentAndDescriptionsPair = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments { get { return 2; } }
	}
}
