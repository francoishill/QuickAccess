using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
//using InterfaceForQuickAccessPlugin;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;

namespace GoogleSearchCommandPlugin
{
	public class GoogleSearchCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "google"; } }
		public override string DisplayName { get { return "Google Search"; } }
		public override string Description { get { return "Google search a word/phrase"; } }
		public override string ArgumentsExample { get { return "first man on the moon"; } }

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
			if (Index != 0)
				errorMessage = "Only one argument allowed for Google search command";
			else if (Index == 0 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "First argument of Google search command may not be null/empty/whitespaces only";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			errorMessage = "";
			if (arguments.Length != 1) errorMessage = "Exactly one argument required for Google search command";
			//else if (!(arguments[0] is string)) errorMessage = "First argument of Google search command must be of type string";
			else if (!PreValidateArgument(out errorMessage, 0, arguments[0]))
				errorMessage = errorMessage + "";
			else return true;
			return false;
		}

		public override bool PerformCommand(out string errorMessage, TextFeedbackEventHandler textFeedbackEvent = null, ProgressChangedEventHandler progressChangedEvent = null, params string[] arguments)
		{
			//if (!ValidateArguments(out errorMessage, arguments)) return false;
			try
			{
				System.Diagnostics.Process.Start("http://www.google.co.za/search?q=" + arguments[0]);
				errorMessage = "";
				return true;
			}
			catch (Exception exc)
			{
				//UserMessages.ShowWarningMessage("Cannot google search: " + arguments[0] + Environment.NewLine + exc.Message);
				errorMessage = "Cannot google search: " + arguments[0] + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
			{
				new CommandArgument("", "search phrase", new ObservableCollection<string>() { })
			};
		public override CommandArgument[] AvailableArguments { get { return availableArguments; } set { availableArguments = value; } }

		//private KeyAndValuePair[] availableArgumentAndDescriptionsPair = new KeyAndValuePair[]
		//{
		//	new KeyAndValuePair("", "search phrase")
		//};
		//public override KeyAndValuePair[] AvailableArgumentAndDescriptionsPair { get { return availableArgumentAndDescriptionsPair; } set { availableArgumentAndDescriptionsPair = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments { get { return 1; } }
	}
}
