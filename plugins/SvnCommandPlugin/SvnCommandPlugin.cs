using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
using InterfaceForQuickAccessPlugin;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;

namespace SvnCommandPlugin
{
	public class SvnCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "svn"; } }
		public override string DisplayName { get { return "Svn"; } }
		public override string Description { get { return "Perform svn command(s) on a folder"; } }
		public override string ArgumentsExample { get { return @"commit c:\dev86\myproject1;Bug fixed where it automatically..."; } }

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
				{
					new ObservableCollection<string>() { "commit", "update", "status", "statuslocal" },
					new ObservableCollection<string>() { "all", "QuickAccess", "SharedClasses", "TestingSharedClasses" },
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
			if (Index >= 3)
				errorMessage = "More than 3 arguments not allowed for Svn command (sub-command, folder, description)";
			else if (Index == 0 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "First argument (sub-command) of Svn command may not be null/empty/whitespaces";
			else if (Index == 0 && !(predefinedArgumentsList[0].ToArray()).Contains(argumentValue))
				errorMessage = "First argument of Svn command is an invalid sub-command";
			else if (Index == 1 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "Second argument (folder) of Svn command may not be null/empty/whitespaces";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			//minutes, autosnooze, name, desc
			errorMessage = "";
			if (arguments.Length < 2) errorMessage = "At least 2 arguments required for Svn command (sub-command, folder, description)";
			else if (arguments.Length > 3) errorMessage = "More than 3 arguments not allowed for Svn command (sub-command, folder, description)";
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
				SvnInterop.SvnCommand svnCommand;
				if (Enum.TryParse<SvnInterop.SvnCommand>(arguments[0], true, out svnCommand))
				{
					SvnInterop.PerformSvn(
						this,
						arguments[1] + (arguments[0] == "commit" ? ";" + arguments[2] : ""),
					 svnCommand,
					 textFeedbackEvent);
					errorMessage = "";
					return true;
				}
				else
				{
					errorMessage = "Invalid svn command = " + arguments[0];
					return false;
				}
			}
			catch (Exception exc)
			{
				errorMessage = "Cannot perform Svn command: " + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
				{
					new CommandArgument("", "sub-command", new ObservableCollection<string>() { "commit", "update", "status", "statuslocal" }),
					new CommandArgument("", "folder/path", new ObservableCollection<string>() { "all", "QuickAccess", "SharedClasses", "TestingSharedClasses"}),
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
