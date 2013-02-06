using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
//using InterfaceForQuickAccessPlugin;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;

namespace SubversionCommandPlugin
{
	public class SubversionCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "subversion"; } }
		public override string DisplayName { get { return "Subversion"; } }
		public override string Description { get { return "Perform subversion command(s) on a folder"; } }
		public override string ArgumentsExample { get { return @"commit c:\dev86\myproject1;Bug fixed where it automatically..."; } }

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
		{
			new ObservableCollection<string>() { "commit", "update", "status", "statuslocal" },
			new ObservableCollection<string>() { "all", "QuickAccess", "SharedClasses", "PublishOwnApps", "TestingSharedClasses" },
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
				errorMessage = "More than 3 arguments not allowed for Subversion command (sub-command, folder, description)";
			else if (index == 0 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "First argument (sub-command) of Subversion command may not be null/empty/whitespaces";
			else if (index == 0 && !(predefinedArgumentsList[0].ToArray()).Contains(argumentValue))
				errorMessage = "First argument of Subversion command is an invalid sub-command";
			else if (index == 1 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "Second argument (folder) of Subversion command may not be null/empty/whitespaces";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			//minutes, autosnooze, name, desc
			errorMessage = "";
			if (arguments.Length < 2) errorMessage = "At least 2 arguments required for Subversion command (sub-command, folder, description)";
			else if (arguments.Length > 3) errorMessage = "More than 3 arguments not allowed for Subversion command (sub-command, folder, description)";
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
				SubversionInterop.SubversionCommand subversionCommand;
				if (Enum.TryParse<SubversionInterop.SubversionCommand>(arguments[0], true, out subversionCommand))
				{
					SubversionInterop.PerformSubversionCommand(
						this,
						arguments[1] + (arguments[0] == "commit" ? ";" + arguments[2] : ""),
					 subversionCommand,
					 textFeedbackEvent);
					errorMessage = "";
					return true;
				}
				else
				{
					errorMessage = "Invalid subversion command = " + arguments[0];
					return false;
				}
			}
			catch (Exception exc)
			{
				errorMessage = "Cannot perform Subversion command: " + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
				{
					new CommandArgument("", "sub-command", new ObservableCollection<string>() { "commit", "update", "status", "statuslocal" }),
					new CommandArgument("", "folder/path", new ObservableCollection<string>() { "all", "QuickAccess", "SharedClasses", "PublishOwnApps", "TestingSharedClasses"}),
					new CommandArgument("", "description", new ObservableCollection<string>())
				};
		public override CommandArgument[] AvailableArguments { get { return availableArguments; } set { availableArguments = value; } }

		//private KeyAndValuePair[] availableArgumentAndDescriptionsPair = new KeyAndValuePair[]
		//{
		//	new KeyAndValuePair("", "sub-command"),
		//	new KeyAndValuePair("", "Folder/path"),
		//	new KeyAndValuePair("", "Description")
		//};
		//public override KeyAndValuePair[] AvailableArgumentAndDescriptionsPair { get { return availableArgumentAndDescriptionsPair; } set { availableArgumentAndDescriptionsPair = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments
		{
			get
			{
				if (currentArguments.Count > 0 && (new string[] { "commit" }).Contains(currentArguments[0].CurrentValue))
					return 3;
				else return 2;
			}
		}
	}
}
