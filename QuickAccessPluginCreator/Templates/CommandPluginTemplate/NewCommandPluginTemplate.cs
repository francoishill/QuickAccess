using System;
using InlineCommandToolkit;

namespace NewCommandPluginTemplate
{
	public class NewCommandPluginTemplate : InlineCommands.OverrideToStringClass, IQuickAccessPluginInterface
	{
		//The command name must be a name without spaced (preferably all small caps).
		public override string CommandName { get { throw new NotImplementedException(); } }

		//The display name may be a more "human friendly" name
		public override string DisplayName { get { throw new NotImplementedException(); } }

		//A short description of what the command does
		public override string Description { get { throw new NotImplementedException(); } }

		//An example of arguments
		public override string ArgumentsExample { get { throw new NotImplementedException(); } }

		//This method is the validation that happens before a specific argument is "accepted"
		public override bool PreValidateArgument(out string errorMessage, int Index, string argumentValue)
		{
			throw new NotImplementedException();
		}

		//This method is the method that validate ALL arguments
		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			throw new NotImplementedException();
		}

		//This is where the command actually get performed (after successful validation)
		public override bool PerformCommand(out string errorMessage, TextFeedbackEventHandler textFeedbackEvent = null, ProgressChangedEventHandler progressChangedEvent = null, params string[] arguments)
		{
			throw new NotImplementedException();
		}

		//This lists all the available arguments (with their "auto-complete" list). The ArgumentCountFor..(later defined) will determine which of these available arguments are shown
		public override CommandArgument[] AvailableArguments
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		//This is an array of lists to define the "autocomplete" strings
		public override System.Collections.ObjectModel.ObservableCollection<string>[] PredefinedArgumentsList
		{
			get { throw new NotImplementedException(); }
		}

		//This is an array of lists to define replacements for each argument
		public override System.Collections.Generic.Dictionary<string, string>[] ArgumentsReplaceKeyValuePair
		{
			get { throw new NotImplementedException(); }
		}

		//This must return the argument count visible to the user (may vary depending what is already in the currentArguments).
		public override int ArgumentCountForCurrentPopulatedArguments
		{
			get { throw new NotImplementedException(); }
		}
	}
}
