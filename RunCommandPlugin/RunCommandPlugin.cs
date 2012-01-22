using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
//using InterfaceForQuickAccessPlugin;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;

namespace RunCommandPlugin
{
	public class RunCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "run"; } }
		public override string DisplayName { get { return "Run"; } }
		public override string Description { get { return "Run any file/folder"; } }
		public override string ArgumentsExample { get { return "outlook"; } }

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
			{
				new ObservableCollection<string>() { "cmd", "outlook" }
			};
		public override ObservableCollection<string>[] PredefinedArgumentsList { get { return predefinedArgumentsList; } }

		private readonly Dictionary<string, string>[] argumentsReplaceKeyValuePair =
			{
				new Dictionary<string, string>() { { "cmd", "cmd1" }, { "outlook", "outlook1" } }
			};
		public override Dictionary<string, string>[] ArgumentsReplaceKeyValuePair { get { return argumentsReplaceKeyValuePair; } }

		public override bool PreValidateArgument(out string errorMessage, int Index, string argumentValue)
		{
			errorMessage = "";
			if (Index < argumentsReplaceKeyValuePair.Length && argumentsReplaceKeyValuePair[Index].ContainsKey(argumentValue))
				argumentValue = argumentsReplaceKeyValuePair[Index][argumentValue];
			if (Index != 0)
				errorMessage = "Only one argument allowed for Run command";
			else if (Index == 0 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "First argument of run command may not be null/empty/whitespaces only";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			errorMessage = "";
			if (arguments.Length != 1) errorMessage = "Exactly one argument required for Run command";
			//else if (!(arguments[0] is string)) errorMessage = "First argument of run command must be of type string";
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
				System.Diagnostics.Process.Start(arguments[0]);
				errorMessage = "";
				return true;
			}
			catch (Exception exc)
			{
				//UserMessages.ShowWarningMessage("Cannot run: " + arguments [0] + Environment.NewLine + exc.Message);
				errorMessage = "Cannot run: " + arguments[0] + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
			{
				new CommandArgument("", "parameter", new ObservableCollection<string>() { "cmd", "outlook" })
			};
		public override CommandArgument[] AvailableArguments { get { return availableArguments; } set { availableArguments = value; } }

		//private KeyAndValuePair[] availableArgumentAndDescriptionsPair = new KeyAndValuePair[]
		//{
		//	new KeyAndValuePair("", "parameter")
		//};
		//public override KeyAndValuePair[] AvailableArgumentAndDescriptionsPair { get { return availableArgumentAndDescriptionsPair; } set { availableArgumentAndDescriptionsPair = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments { get { return 1; } }
	}
}
