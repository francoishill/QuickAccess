using System;
using System.Windows;

namespace SharedClasses
{
	public enum CommandPerformedType { EmbeddedButton_MouseLeftButtonDown, EmbeddedButton_MouseRightButtonDown,
	ClearTextboxTextButton_MouseLeftButtonDown, AutoCompleteActualTextBox_PreviewKeyDown, ArgumentText_DragOver, ArgumentText_Drop, ArgumentText_GotFocus, ArgumentText_SelectionChanged }
	public delegate void CommandPerformedEventHandler(object sender, CommandPerformedEventArgs e);
	public class CommandPerformedEventArgs : EventArgs
	{
		public CommandPerformedType CommandPerformed;
		public EventArgs eventArgs;
		public CommandPerformedEventArgs(CommandPerformedType CommandPerformed, EventArgs eventArgs)
		{
			this.CommandPerformed = CommandPerformed;
			this.eventArgs = eventArgs;
		}
	}

	partial class GeneralResourceDictionary : ResourceDictionary
	{
		public static event CommandPerformedEventHandler CommandPerformedEvent;

		public GeneralResourceDictionary()
		{
			InitializeComponent();
		}

		private void RaiseCommandPerformedEvent(object sender, EventArgs eventArgs, CommandPerformedType CommandPerformed)
		{
			if (CommandPerformedEvent != null)
				CommandPerformedEvent(sender, new CommandPerformedEventArgs(CommandPerformed, eventArgs));
		}

		private void EmbeddedButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			RaiseCommandPerformedEvent(sender, e, CommandPerformedType.EmbeddedButton_MouseLeftButtonDown);
		}

		private void EmbeddedButton_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			RaiseCommandPerformedEvent(sender, e, CommandPerformedType.EmbeddedButton_MouseRightButtonDown);
		}

		private void ClearTextboxTextButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			RaiseCommandPerformedEvent(sender, e, CommandPerformedType.ClearTextboxTextButton_MouseLeftButtonDown);
		}

		private void AutoCompleteActualTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			RaiseCommandPerformedEvent(sender, e, CommandPerformedType.AutoCompleteActualTextBox_PreviewKeyDown);
		}

		private void ArgumentText_DragOver(object sender, DragEventArgs e)
		{
			RaiseCommandPerformedEvent(sender, e, CommandPerformedType.ArgumentText_DragOver);
		}

		private void ArgumentText_Drop(object sender, DragEventArgs e)
		{
			RaiseCommandPerformedEvent(sender, e, CommandPerformedType.ArgumentText_Drop);
		}

		private void ArgumentText_GotFocus(object sender, RoutedEventArgs e)
		{
			RaiseCommandPerformedEvent(sender, e, CommandPerformedType.ArgumentText_GotFocus);
		}

		private void ArgumentText_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			RaiseCommandPerformedEvent(sender, e, CommandPerformedType.ArgumentText_SelectionChanged);
		}
	}
}