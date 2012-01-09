using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
using InterfaceForQuickAccessPlugin;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;

namespace ExploreCommandPlugin
{
	public class ExploreCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "explore"; } }
		public override string DisplayName { get { return "Explore a Folder"; } }
		public override string Description { get { return "Explore a folder"; } }
		public override string ArgumentsExample { get { return @"c:\windows"; } }

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
				{
					new ObservableCollection<string>() { @"c:\", @"c:\Program Files" }
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
				errorMessage = "Only one argument allowed for Explore command";
			else if (Index == 0 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "First argument of Explore command may not be null/empty/whitespaces only";
			else if (Index == 0 && !Directory.Exists(argumentValue))
				errorMessage = "First argument of Explore command must be existing directory";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			errorMessage = "";
			if (arguments.Length != 1) errorMessage = "Exactly one argument required for Explore command";
			//else if (!(arguments[0] is string)) errorMessage = "First argument of Explore command must be of type string";
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
				if (!Directory.Exists(arguments[0])) throw new DirectoryNotFoundException("Directory not found: " + arguments[0]);
				System.Diagnostics.Process.Start(arguments[0]);
				//Process.Start("explorer", "/select, \"" + argumentString + "\"");
				errorMessage = "";
				return true;
			}
			catch (Exception exc)
			{
				//UserMessages.ShowWarningMessage("Cannot explore: " + arguments[0] + Environment.NewLine + exc.Message);
				errorMessage = "Cannot explore: " + arguments[0] + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
				{
					new CommandArgument("", "folder", new ObservableCollection<string>() { @"c:\", @"c:\Program Files" })
				};
		public override CommandArgument[] AvailableArguments { get { return availableArguments; } set { availableArguments = value; } }

		//private KeyAndValuePair[] availableArgumentAndDescriptionsPair = new KeyAndValuePair[]
		//{
		//	new KeyAndValuePair("", "folder")
		//};
		//public override KeyAndValuePair[] AvailableArgumentAndDescriptionsPair { get { return availableArgumentAndDescriptionsPair; } set { availableArgumentAndDescriptionsPair = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments { get { return 1; } }
	}
}
