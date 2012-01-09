using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
using InterfaceForQuickAccessPlugin;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;

namespace WebCommandPlugin
{
	public class WebCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "web"; } }
		public override string DisplayName { get { return "Open Web URL"; } }
		public override string Description { get { return "Open a web URL"; } }
		public override string ArgumentsExample { get { return "google.com"; } }

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
				{
					new ObservableCollection<string>() { "google.com", "firepuma.com", "fjh.dyndns.org" }
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
				errorMessage = "Only one argument required for Web command";
			else if (Index == 0 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "First argument of Web command may not be null/empty/whitespaces only";
			else if (Index == 0 && !argumentValue.Contains('.') && !argumentValue.ToLower().Contains("localhost"))
				errorMessage = "First argument of Web command must contain a '.' or be localhost.";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			errorMessage = "";
			if (arguments.Length != 1) errorMessage = "Exactly one argument required for Web command";
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
				if (!arguments[0].StartsWith("http://") && !arguments[0].StartsWith("https://") && !arguments[0].StartsWith("www."))
					arguments[0] = "http://" + arguments[0];
				System.Diagnostics.Process.Start(arguments[0]);
				errorMessage = "";
				return true;
			}
			catch (Exception exc)
			{
				//UserMessages.ShowWarningMessage("Cannot google search: " + arguments[0] + Environment.NewLine + exc.Message);
				errorMessage = "Cannot open web url: " + arguments[0] + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
				{
					new CommandArgument("", "url", new ObservableCollection<string>() { "google.com", "firepuma.com", "fjh.dyndns.org" })
				};
		public override CommandArgument[] AvailableArguments { get { return availableArguments; } set { availableArguments = value; } }

		//private KeyAndValuePair[] availableArgumentAndDescriptionsPair = new KeyAndValuePair[]
		//{
		//	new KeyAndValuePair("", "url")
		//};
		//public override KeyAndValuePair[] AvailableArgumentAndDescriptionsPair { get { return availableArgumentAndDescriptionsPair; } set { availableArgumentAndDescriptionsPair = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments { get { return 1; } }
	}
}
