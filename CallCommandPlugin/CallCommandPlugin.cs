using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
//using InterfaceForQuickAccessPlugin;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;

namespace CallCommandPlugin
{
	public class CallCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "call"; } }
		public override string DisplayName { get { return "Call"; } }
		public override string Description { get { return "Shows the phone number of a contact"; } }
		public override string ArgumentsExample { get { return "yolwork"; } }

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
				{
					new ObservableCollection<string>(NameAndNumberDictionary.Keys)
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
				errorMessage = "Only one argument required for Call command";
			else if (Index == 0 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "First argument of Call command may not be null/empty/whitespaces";
			else if (Index == 0 && !NameAndNumberDictionary.ContainsKey(argumentValue))
				errorMessage = "Name not found in contact list: " + argumentValue;
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			errorMessage = "";
			if (arguments.Length != 1) errorMessage = "Exactly one argument required for Call command";
			else if (!PreValidateArgument(out errorMessage, 0, arguments[0]))
				errorMessage = errorMessage + "";
			else return true;
			return false;
		}

		public override bool PerformCommand(out string errorMessage, TextFeedbackEventHandler textFeedbackEvent = null, ProgressChangedEventHandler progressChangedEvent = null, params string[] arguments)
		{
			try
			{
				TextFeedbackEventArgs.RaiseTextFeedbackEvent_Ifnotnull(this, textFeedbackEvent, NameAndNumberDictionary[arguments[0]], TextFeedbackType.Noteworthy);
				errorMessage = "";
				return true;
			}
			catch (Exception exc)
			{
				errorMessage = "Cannot find name in dictionary: " + arguments[0] + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private static Dictionary<string, string> NameAndNumberDictionary = new Dictionary<string, string>()
				{
					{ "yolwork", "Yolande work: (021) 853 3564" },
					{ "imqs", "IMQS office: 021-880 2712 / 880 1632" },
					{ "kerry", "Kerry extension: 107" },
					{ "adrian", "Adrian extension: 106" },
					{ "deon",   "Deon extension: 121" },
					{ "johann", "Johann extension: 119" },
					{ "wesley", "Wesley extension: 111" },
					{ "honda",  "Honda Tygervalley: 021 910 8300" }
				};

		private CommandArgument[] availableArguments = new CommandArgument[]
				{
					new CommandArgument("", "name", new ObservableCollection<string>(NameAndNumberDictionary.Keys))
				};
		public override CommandArgument[] AvailableArguments { get { return availableArguments; } set { availableArguments = value; } }

		//private KeyAndValuePair[] availableArgumentAndDescriptionsPair = new KeyAndValuePair[]
		//{
		//	new KeyAndValuePair("", "name")
		//};
		//public override KeyAndValuePair[] AvailableArgumentAndDescriptionsPair { get { return availableArgumentAndDescriptionsPair; } set { availableArgumentAndDescriptionsPair = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments { get { return 1; } }
	}
}