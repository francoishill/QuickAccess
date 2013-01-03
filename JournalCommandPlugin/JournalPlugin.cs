using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using InlineCommandToolkit;
using SharedClasses;

namespace JournalPluginCommandPlugin
{
	public class JournalPluginCommandPlugin : InlineCommands.OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "JournalPlugin"; } }
		public override string DisplayName { get { return "Journal Plugin"; } }
		public override string Description { get { return "Creates an online journal item (having a date, description and link)."; } }
		public override string ArgumentsExample { get { return "Today I discovered that my left pinky finger is slightly longer than my right."; } }

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
				{
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
			if (Index < 0 || Index > 1)
				errorMessage = "At least one and at most two arguments required for Journal command";
			else if (Index == 0 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "First argument Journal command may not be null/empty/whitespaces";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			//minutes, autosnooze, name, desc
			errorMessage = "";
			if (arguments.Length < 1 || arguments.Length > 2) errorMessage = "At least one and at most two arguments required for Journal command";
			else if (!PreValidateArgument(out errorMessage, 0, arguments[0]))
				errorMessage = errorMessage + "";
			//else if (arguments.Length == 2 && !PreValidateArgument(out errorMessage, 0, arguments[0]))
			//    errorMessage = errorMessage + "";
			else return true;
			return false;
		}

		public override bool PerformCommand(out string errorMessage, TextFeedbackEventHandler textFeedbackEvent = null, ProgressChangedEventHandler progressChangedEvent = null, params string[] arguments)
		{
			try
			{
				PhpInterop.AddJournalItemFirepuma(this, arguments[0], arguments[1], textFeedbackEvent);
				errorMessage = "";
				return true;
			}
			catch (Exception exc)
			{
				//UserMessages.ShowWarningMessage("Cannot add todo item: " + Environment.NewLine + exc.Message);
				errorMessage = "Cannot add journal item: " + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
				{
					new CommandArgument("", "Journal text", new ObservableCollection<string>()),
					new CommandArgument("", "Link", new ObservableCollection<string>())
				};
		public override CommandArgument[] AvailableArguments { get { return availableArguments; } set { availableArguments = value; } }

		//private KeyAndValuePair[] availableArgumentAndDescriptionsPair = new KeyAndValuePair[]
		//{
		//	new KeyAndValuePair("", "btw text")
		//};
		//public override KeyAndValuePair[] AvailableArgumentAndDescriptionsPair { get { return availableArgumentAndDescriptionsPair; } set { availableArgumentAndDescriptionsPair = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments { get { return 2; } }
	}
}
