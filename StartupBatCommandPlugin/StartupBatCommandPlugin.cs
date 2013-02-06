using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
//using InterfaceForQuickAccessPlugin;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;

namespace StartupBatCommandPlugin
{
	public class StartubBatCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "startupbat"; } }
		public override string DisplayName { get { return "Startup Batch File"; } }
		public override string Description { get { return "Startup batch file"; } }
		public override string ArgumentsExample { get { return "getline outlook.exe"; } }

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
				{
					new ObservableCollection<string>() { "open", "getall", "getline", "comment", "uncomment" },
				};
		public override ObservableCollection<string>[] PredefinedArgumentsList { get { return predefinedArgumentsList; } }

		private readonly Dictionary<string, string>[] argumentsReplaceKeyValuePair =
				{
				};
		public override Dictionary<string, string>[] ArgumentsReplaceKeyValuePair { get { return argumentsReplaceKeyValuePair; } }

		private bool IsStartubBatCommand(string command)
		{
			string commandLowercase = command.ToLower().Trim();
			return
				commandLowercase == "open" ||
				commandLowercase == "getall" ||
				commandLowercase == "getline" ||
				commandLowercase == "comment" ||
				commandLowercase == "uncomment";
		}

		public override bool PreValidateArgument(out string errorMessage, int index, string argumentValue)
		{
			errorMessage = "";
			if (index < argumentsReplaceKeyValuePair.Length && argumentsReplaceKeyValuePair[index].ContainsKey(argumentValue))
				argumentValue = argumentsReplaceKeyValuePair[index][argumentValue];
			if (index >= 2)
				errorMessage = "More than 2 arguments not allowed for Startub bat command";
			else if (index == 0 && !IsStartubBatCommand(argumentValue))
				errorMessage = "First argument of Startup bat command is invalid, must be one of the predefined commands: " + argumentValue;
			else if (index == 1 && (new string[] { "open", "getall" }).Contains(CurrentArguments[0].CurrentValue))
				errorMessage = "No additional arguments allowed for '" + CurrentArguments[0].CurrentValue + "'";
			else if (index == 1 && (new string[] { "comment", "uncomment" }).Contains(CurrentArguments[0].CurrentValue) && !InlineCommandToolkit.InlineCommands.CanParseToInt(argumentValue))
				errorMessage = "Second argument of Startup bat command (" + CurrentArguments[0].CurrentValue + ") must be a valid integer";
			else if (index == 1 && "getline" == CurrentArguments[0].CurrentValue && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "Second argument of Startup bat command (" + CurrentArguments[0].CurrentValue + ") may not be null/empty/whitespaces";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			//minutes, autosnooze, name, desc
			errorMessage = "";
			if (arguments.Length < 1) errorMessage = "At least 1 argument is required for Startub bat command";
			else if (arguments.Length > 2) errorMessage = "More than 2 arguments not allowed for Startup bat command";
			else if (!PreValidateArgument(out errorMessage, 0, arguments[0]))
				errorMessage = errorMessage + "";
			else if (arguments.Length > 1 && !PreValidateArgument(out errorMessage, 1, arguments[1]))
				errorMessage = errorMessage + "";
			else return true;
			return false;
		}

		public override bool PerformCommand(out string errorMessage, TextFeedbackEventHandler textFeedbackEvent = null, ProgressChangedEventHandler progressChangedEvent = null, params string[] arguments)
		{
			try
			{
				string filePath = @"C:\Francois\Other\Startup\work Startup.bat";
				StartupbatInterop.PerformStartupbatCommand(this, filePath, arguments[0] + (arguments.Length > 1 ? " " + arguments[1] : ""), textFeedbackEvent);
				errorMessage = "";
				return true;
			}
			catch (Exception exc)
			{
				//UserMessages.ShowWarningMessage("Cannot add todo item: " + Environment.NewLine + exc.Message);
				errorMessage = "Cannot perform startup bat command: " + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
				{
					new CommandArgument("", "sub-command", new ObservableCollection<string>() { "open", "getall", "getline", "comment", "uncomment" }),
					new CommandArgument("", "parameter", new ObservableCollection<string>())
				};
		public override CommandArgument[] AvailableArguments { get { return availableArguments; } set { availableArguments = value; } }

		//private KeyAndValuePair[] availableArgumentAndDescriptionsPair = new KeyAndValuePair[]
		//{
		//	new KeyAndValuePair("", "sub-command"),
		//	new KeyAndValuePair("", "parameter")
		//};
		//public override KeyAndValuePair[] AvailableArgumentAndDescriptionsPair { get { return availableArgumentAndDescriptionsPair; } set { availableArgumentAndDescriptionsPair = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments
		{
			get
			{
				if (currentArguments.Count > 0 && (new string[] { "getline", "comment", "uncomment" }).Contains(currentArguments[0].CurrentValue))
					return 2;
				else return 1;
			}
		}
	}
}
