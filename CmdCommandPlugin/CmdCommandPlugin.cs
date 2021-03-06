﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
//using InterfaceForQuickAccessPlugin;
using SharedClasses;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;

namespace CmdCommandPlugin
{
	public class CmdCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "cmd"; } }
		public override string DisplayName { get { return "Command Prompt"; } }
		public override string Description { get { return "Open a folder in Command Prompt"; } }
		public override string ArgumentsExample { get { return @"c:\windows\system32"; } }

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
				{
					new ObservableCollection<string>() { @"c:\windows", @"c:\windows\system32" }
				};
		public override ObservableCollection<string>[] PredefinedArgumentsList { get { return predefinedArgumentsList; } }

		private readonly Dictionary<string, string>[] argumentsReplaceKeyValuePair =
				{
				};
		public override Dictionary<string, string>[] ArgumentsReplaceKeyValuePair { get { return argumentsReplaceKeyValuePair; } }

		public override bool PreValidateArgument(out string errorMessage, int index, string argumentValue)
		{
			errorMessage = "";
			if (index < argumentsReplaceKeyValuePair.Length && argumentsReplaceKeyValuePair[index].ContainsKey(argumentValue))
				argumentValue = argumentsReplaceKeyValuePair[index][argumentValue];
			if (index != 0)
				errorMessage = "Only one argument allowed for Cmd command";
			else if (index == 0 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "First argument of Cmd command may not be null/empty/whitespaces only";
			else if (index == 0 && !Directory.Exists(argumentValue))
				errorMessage = "First argument of Cmd command must be existing directory";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			errorMessage = "";
			if (arguments.Length != 1) errorMessage = "Exactly one argument required for Cmd command";
			//else if (!(arguments[0] is string)) errorMessage = "First argument of Explore command must be of type string";
			else if (!PreValidateArgument(out errorMessage, 0, arguments[0]))
				errorMessage = errorMessage + "";
			else return true;
			return false;
		}

		public override bool PerformCommand(out string errorMessage, TextFeedbackEventHandler textFeedbackEvent = null, ProgressChangedEventHandler progressChangedEvent = null, params string[] arguments)
		{
			try
			{
				WindowsInterop.StartCommandPromptOrVScommandPrompt(this, arguments[0], false, textFeedbackEvent);
				errorMessage = "";
				return true;
			}
			catch (Exception exc)
			{
				errorMessage = "Cannot open Cmd: " + arguments[0] + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
				{
					new CommandArgument("", "folder", new ObservableCollection<string>() { @"c:\Windows", @"c:\Windows\System32" })
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
