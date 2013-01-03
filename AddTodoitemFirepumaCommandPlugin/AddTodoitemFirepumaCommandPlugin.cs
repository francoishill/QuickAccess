using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
//using InterfaceForQuickAccessPlugin;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;
using SharedClasses;

namespace AddTodoitemFirepumaCommandPlugin
{
	public class AddTodoitemFirepumaCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "addtodo"; } }
		public override string DisplayName { get { return "Add Todo Item"; } }
		public override string Description { get { return "Add todo item to firepuma"; } }
		public override string ArgumentsExample { get { return "13;30;Reminder;Buy milk => (MinutesFromNow, Autosnooze, Name, Description)"; } }

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
				{
					new ObservableCollection<string>() { "5", "30", "60" },
					new ObservableCollection<string>() { "15", "30", "60" },
					new ObservableCollection<string>() { "Shop" }
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
			if (Index >= 4)
				errorMessage = "More than 4 arguments not allowed for Add todo command (minutesfromnow, autosnooze, name, desc)";
			else if (Index == 0 && !InlineCommandToolkit.InlineCommands.CanParseToInt(argumentValue))
				errorMessage = "First argument (minutesfromnow) of Add todo command must be of type int";
			else if (Index == 1 && !InlineCommandToolkit.InlineCommands.CanParseToInt(argumentValue))
				errorMessage = "Second argument (autosnooze) of Add todo command must be of type int";
			else if (Index == 2 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "Third argument (name) of Add todo command may not be null/empty/whitespaces only";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			//minutes, autosnooze, name, desc
			errorMessage = "";
			if (arguments.Length < 3) errorMessage = "At least 3 arguments required for Add todo command (minutesfromnow, autosnooze, name, desc)";
			else if (arguments.Length > 4) errorMessage = "More than 4 arguments not allowed for Add todo command (minutesfromnow, autosnooze, name, desc)";
			else if (!PreValidateArgument(out errorMessage, 0, arguments[0]))
				errorMessage = errorMessage + "";
			else if (!PreValidateArgument(out errorMessage, 1, arguments[1]))
				errorMessage = errorMessage + "";
			else if (!PreValidateArgument(out errorMessage, 2, arguments[2]))
				errorMessage = errorMessage + "";
			else return true;
			return false;
		}

		public override bool PerformCommand(out string errorMessage, TextFeedbackEventHandler textFeedbackEvent = null, ProgressChangedEventHandler progressChangedEvent = null, params string[] arguments)
		{
			//if (!ValidateArguments(out errorMessage, arguments)) return false;
			try
			{
				SharedClasses.PhpInterop.AddTodoItemFirepuma(
					this,
					PhpInterop.ServerAddress,
					PhpInterop.doWorkAddress,
					PhpInterop.Username,
					PhpInterop.Password,
					"QuickAccess",
					"Quick todo",
					arguments[2],
					arguments.Length > 3 ? arguments[3] : "",
					false,
					DateTime.Now.AddMinutes(Convert.ToInt32(arguments[0])),
					DateTime.Now,
					0,
					false,
					Convert.ToInt32(arguments[1]),
					textFeedbackEvent);
				errorMessage = "";
				return true;
			}
			catch (Exception exc)
			{
				//UserMessages.ShowWarningMessage("Cannot add todo item: " + Environment.NewLine + exc.Message);
				errorMessage = "Cannot add todo item: " + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
				{
					new CommandArgument("", "minutes", new ObservableCollection<string>() { "5", "30", "60" }),
					new CommandArgument("", "autosnooze", new ObservableCollection<string>() { "15", "30", "60" }),
					new CommandArgument("", "name", new ObservableCollection<string>() { "Shop" }),
					new CommandArgument("", "description", new ObservableCollection<string>())
				};
		public override CommandArgument[] AvailableArguments { get { return availableArguments; } set { availableArguments = value; } }

		//private KeyAndValuePair[] availableArgumentAndDescriptionsPair = new KeyAndValuePair[]
		//{
		//	new KeyAndValuePair("", "minutes"),
		//	new KeyAndValuePair("", "autosnooze"),
		//	new KeyAndValuePair("", "name"),
		//	new KeyAndValuePair("", "description")
		//};
		//public override KeyAndValuePair[] AvailableArgumentAndDescriptionsPair { get { return availableArgumentAndDescriptionsPair; } set { availableArgumentAndDescriptionsPair = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments { get { return 4; } }
	}
}
