using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
using InterfaceForQuickAccessPlugin;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;

namespace MailCommandPlugin
{
	public class MailCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "mail"; } }
		public override string DisplayName { get { return "Mail"; } }
		public override string Description { get { return "Send an email"; } }
		public override string ArgumentsExample { get { return "billgates@microsoft.com;My subject;Hi Bill.\nHow have you been?"; } }

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
				{
					new ObservableCollection<string>() { "fhill@gls.co.za", "francoishill11@gmail.com" },
					//new ObservableCollection<string>() { "Hi there", "This is a subject" },
					//new ObservableCollection<string>() { "How have you been?", "This is the body" }
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
				errorMessage = "More than 3 arguments not allowed for Mail command (mail, subject, body)";
			else if (Index == 0 && !InlineCommandToolkit.InlineCommands.IsEmail(argumentValue))
				errorMessage = "First argument (to) of Mail command must be a valid email address";
			else if (Index == 1 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "Second argument (subject) of Mail command may not be null/empty/whitespaces only";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			errorMessage = "";
			if (arguments.Length < 2) errorMessage = "At least 2 arguments required for Mail command (mail, subject, body)";
			else if (arguments.Length > 3) errorMessage = "More than 3 arguments not allowed for Mail command (mail, subject, body)";
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
				MicrosoftOfficeInterop.CreateNewOutlookMessage(
					this,
					arguments[0],
					arguments[1],
					arguments.Length >= 3 ? arguments[2] : "",
					textFeedbackEvent);
				errorMessage = "";
				return true;
			}
			catch (Exception exc)
			{
				errorMessage = "Cannot send mail: " + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
				{
					new CommandArgument("", "to address", new ObservableCollection<string>() { "fhill@gls.co.za", "francoishill11@gmail.com" }),
					new CommandArgument("", "subject", new ObservableCollection<string>()),
					new CommandArgument("", "body", new ObservableCollection<string>())
				};
		public override CommandArgument[] AvailableArguments { get { return availableArguments; } set { availableArguments = value; } }

		//private KeyAndValuePair[] availableArgumentAndDescriptionsPair = new KeyAndValuePair[]
		//{
		//	new KeyAndValuePair("", "to address"),
		//	new KeyAndValuePair("", "subject"),
		//	new KeyAndValuePair("", "body"),
		//};
		//public override KeyAndValuePair[] AvailableArgumentAndDescriptionsPair { get { return availableArgumentAndDescriptionsPair; } set { availableArgumentAndDescriptionsPair = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments { get { return 3; } }
	}
}
