using System.Collections.ObjectModel;
using InlineCommandToolkit;

namespace [{CommandName}]CommandPlugin
{
	public class [{CommandName}]CommandPlugin : InlineCommands.OverrideToStringClass, IQuickAccessPluginInterface
	{
		//The command name must be a name without spaced (preferably all small caps).
		public override string CommandName { get { return "[{CommandName}]"; } }

		//The display name may be a more "human friendly" name
		public override string DisplayName { get { return "[{DisplayName}]"; } }

		//A short description of what the command does
		public override string Description { get { return "[{Description}]"; } }

		//An example of arguments
		public override string ArgumentsExample { get { return "[{ArgumentsExample}]"; } }

		//This method is the validation that happens before a specific argument is "accepted"
		public override bool PreValidateArgument(out string errorMessage, int Index, string argumentValue)
		{
			//TODO: This method must validate a specific argument
			errorMessage = "";
			return true;
		}

		//This method is the method that validate ALL arguments
		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			//TODO: This method must validate an array of arguments
			errorMessage = "";
			return true;
		}

		//This is where the command actually get performed (after successful validation)
		public override bool PerformCommand(out string errorMessage, TextFeedbackEventHandler textFeedbackEvent = null, ProgressChangedEventHandler progressChangedEvent = null, params string[] arguments)
		{
			//TODO: Perform the command here
			errorMessage = "";
			UserMessages.ShowMessage("This is where the command is performed");
			return true;
		}

		//This lists all the available arguments (with their "auto-complete" list). The ArgumentCountFor..(later defined) will determine which of these available arguments are shown
		private CommandArgument[] availableArguments = new CommandArgument[]
		{
			new CommandArgument("", "temp", new ObservableCollection<string>() { "runnow" })
		};
		public override CommandArgument[] AvailableArguments
		{
			get { return availableArguments; }
			set { availableArguments = value; }
		}

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
		{
			//TODO: This must return the "auto-complete" list for the arguments
			new ObservableCollection<string>() { "runnow" }
		};

		//This is an array of lists to define the "autocomplete" strings
		public override System.Collections.ObjectModel.ObservableCollection<string>[] PredefinedArgumentsList { get { return predefinedArgumentsList; } }

		private System.Collections.Generic.Dictionary<string, string>[] argumentsReplaceKeyValuePair =
		{
			//TODO: This must return the replace values like if progfiles must be replaced with "Program Files" for a specific argument
		};
		//This is an array of lists to define replacements for each argument
		public override System.Collections.Generic.Dictionary<string, string>[] ArgumentsReplaceKeyValuePair { get { return argumentsReplaceKeyValuePair; } }

		//This must return the argument count visible to the user (may vary depending what is already in the currentArguments).
		public override int ArgumentCountForCurrentPopulatedArguments
		{
			get
			{
				//TODO: The number of "visible" arguments, a statement can be used to determine this
				return 1;
			}
		}
	}
}