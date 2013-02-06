using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using InlineCommandToolkit;
using SharedClasses;
//using InterfaceForQuickAccessPlugin;
using OverrideToStringClass = InlineCommandToolkit.InlineCommands.OverrideToStringClass;

namespace PublishCommandPlugin
{
	public class PublishCommand : OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "publish"; } }
		public override string DisplayName { get { return "Publish VS Project"; } }
		public override string Description { get { return "Perform publish command(s) on a folder"; } }
		public override string ArgumentsExample { get { return @"localvs c:\dev86\myproject1"; } }

		private readonly ObservableCollection<string>[] _predefinedArgumentsList =
		{
			new ObservableCollection<string>() { "localvs", "onlinevs" },
			new ObservableCollection<string>() { "QuickAccess", "MonitorSystem", "PublishOwnApps" },
		};
		public override ObservableCollection<string>[] PredefinedArgumentsList { get { return _predefinedArgumentsList; } }

		private readonly Dictionary<string, string>[] _argumentsReplaceKeyValuePair =
		{
		};
		public override Dictionary<string, string>[] ArgumentsReplaceKeyValuePair { get { return _argumentsReplaceKeyValuePair; } }

		public override bool PreValidateArgument(out string errorMessage, int index, string argumentValue)
		{
			errorMessage = "";
			if (index < _argumentsReplaceKeyValuePair.Length && _argumentsReplaceKeyValuePair[index].ContainsKey(argumentValue))
				argumentValue = _argumentsReplaceKeyValuePair[index][argumentValue];
			if (index >= 2)
				errorMessage = "More than 2 arguments not allowed for Publish command (sub-command, folder/project)";
			else if (index == 0 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "First argument (sub-command) of Publish command may not be null/empty/whitespaces";
			else if (index == 0 && !(_predefinedArgumentsList[0].ToArray()).Contains(argumentValue))
				errorMessage = "First argument of Publish command is an invalid sub-command";
			else if (index == 1 && string.IsNullOrWhiteSpace(argumentValue))
				errorMessage = "Second argument (folder) of Publish command may not be null/empty/whitespaces";
			else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			//minutes, autosnooze, name, desc
			errorMessage = "";
			if (arguments.Length != 2) errorMessage = "Exactly 2 arguments required for Publish command (sub-command, folder/project)";
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
				bool? hasPlugins = null;
				bool? automaticallyUpdateRevision = null;
				bool? writeIntoRegistryForWindowsAutostartup = null;

				hasPlugins = UserMessages.ConfirmNullable("Does the application have plugins (in a Plugins subfolder of the binaries)?", DefaultYesButton: true);
				if (hasPlugins != null)
					automaticallyUpdateRevision = UserMessages.ConfirmNullable("Update the revision also?");
				if (hasPlugins != null && automaticallyUpdateRevision != null)
					writeIntoRegistryForWindowsAutostartup = UserMessages.ConfirmNullable("Auto startup with windows (written into registry)?", DefaultYesButton: true);

				if (hasPlugins != null && automaticallyUpdateRevision != null && writeIntoRegistryForWindowsAutostartup != null)
				{
					if (arguments[0] == "localvs")
					{
						string tmpNoUseVersionStr;
						string tmpNoUseSetupPath;
						DateTime publishedDate;
						PublishInterop.PerformPublish(
							//VisualStudioInterop.PerformPublish(
							//textfeedbackSenderObject: this,
							arguments[1],
							/*_64Only: false,*/
							(bool)hasPlugins, (bool)automaticallyUpdateRevision, true, (bool)writeIntoRegistryForWindowsAutostartup, false, out tmpNoUseVersionStr, out tmpNoUseSetupPath, out publishedDate, (mes, msgtype) =>
							{
								if (textFeedbackEvent == null) return;
								var tmpFeedbackType = TextFeedbackType.Subtle;
								switch (msgtype)
								{
									case FeedbackMessageTypes.Success:
										tmpFeedbackType = TextFeedbackType.Success;
										break;
									case FeedbackMessageTypes.Error:
										tmpFeedbackType = TextFeedbackType.Error;
										break;
									case FeedbackMessageTypes.Warning:
										tmpFeedbackType = TextFeedbackType.Noteworthy;
										break;
									case FeedbackMessageTypes.Status:
										tmpFeedbackType = TextFeedbackType.Subtle;
										break;
								}
								textFeedbackEvent(null, new TextFeedbackEventArgs(mes, tmpFeedbackType));
							},
							(progperc) =>
							{
								if (progressChangedEvent != null)
									progressChangedEvent(null, new ProgressChangedEventArgs(progperc, 100));
							});
					}
					else if (arguments[0] == "onlinevs")
					{
						string tmpNoUseVersionStr;
						string tmpNoUseSetupPath;
						DateTime publishedDate;
						PublishInterop.PerformPublishOnline(
							//VisualStudioInterop.PerformPublishOnline(
							//textfeedbackSenderObject: this,
							arguments[1], false, (bool)hasPlugins, (bool)automaticallyUpdateRevision, true, (bool)writeIntoRegistryForWindowsAutostartup, false, false, out tmpNoUseVersionStr, out tmpNoUseSetupPath, out publishedDate, (mes, msgtype) =>
							{
								if (textFeedbackEvent == null) return;
								var tmpFeedbackType = TextFeedbackType.Subtle;
								switch (msgtype)
								{
									case FeedbackMessageTypes.Success:
										tmpFeedbackType = TextFeedbackType.Success;
										break;
									case FeedbackMessageTypes.Error:
										tmpFeedbackType = TextFeedbackType.Error;
										break;
									case FeedbackMessageTypes.Warning:
										tmpFeedbackType = TextFeedbackType.Noteworthy;
										break;
									case FeedbackMessageTypes.Status:
										tmpFeedbackType = TextFeedbackType.Subtle;
										break;
								}
								textFeedbackEvent(null, new TextFeedbackEventArgs(mes, tmpFeedbackType));
							},
							(progperc) =>
							{
								if (progressChangedEvent != null)
									progressChangedEvent(null, new ProgressChangedEventArgs(progperc, 100));
							});
						//textFeedbackEvent: textFeedbackEvent,
						//progressChanged: progressChangedEvent);
					}
					else
					{
						errorMessage = "Invalid sub-command for Publish: " + arguments[0];
						return false;
					}
					errorMessage = "";
					return true;
				}
				else
				{
					errorMessage = "User cancelled the operation";
					return false;
				}
			}
			catch (Exception exc)
			{
				errorMessage = "Cannot perform Publish command: " + Environment.NewLine + exc.Message;
				return false;
			}
		}

		private CommandArgument[] _availableArguments = new CommandArgument[]
		{
			new CommandArgument("", "sub-command", new ObservableCollection<string>() { "localvs", "onlinevs" }),
			new CommandArgument("", "folder/path", new ObservableCollection<string>() { "QuickAccess", "MonitorSystem","PublishOwnApps"})
		};
		public override CommandArgument[] AvailableArguments { get { return _availableArguments; } set { _availableArguments = value; } }

		//private KeyAndValuePair[] availableArgumentAndDescriptionsPair = new KeyAndValuePair[]
		//{
		//	new KeyAndValuePair("", "sub-command"),
		//	new KeyAndValuePair("", "Folder/Path"),
		//};
		//public override KeyAndValuePair[] AvailableArgumentAndDescriptionsPair { get { return availableArgumentAndDescriptionsPair; } set { availableArgumentAndDescriptionsPair = value; } }

		public override int ArgumentCountForCurrentPopulatedArguments { get { return 2; } }
	}
}
