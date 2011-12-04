using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InlineCommands;
using ICommandWithHandler = InlineCommands.TempNewCommandsManagerClass.ICommandWithHandler;

namespace QuickAccess
{
	/// <summary>
	/// Interaction logic for tmpCommandsUsercontrol.xaml
	/// </summary>
	public partial class tmpCommandsUsercontrol : UserControl
	{
		public tmpCommandsUsercontrol()
		{
			InitializeComponent();

			//SetActiveCommand(null);
			ActiveCommand = null;
			//AutoCompleteBox
		}

		public void InitializeCommands()
		{
			//List<ICommandWithHandler> tmplist = TempNewCommandsManagerClass.ListOfInitializedCommandInterfaces;
			//customAutocompleteTextbox1.ItemsSource = new ObservableCollection<string>();
			//foreach (ICommandWithHandler comm in tmplist)
			//	treeView_CommandList.Items.Add(comm);
			//ActiveCommand.DataContext = TempNewCommandsManagerClass.ListOfInitializedCommandInterfaces[0];
			//customAutocompleteTextbox1.DataContext = TempNewCommandsManagerClass.ListOfInitializedCommandInterfaces[0];
			//customAutocompleteTextbox1.UpdateLayout();
			customAutocompleteTextbox1.ItemsSource = TempNewCommandsManagerClass.ListOfInitializedCommandInterfaces;
			//SetActiveCommand(new TempNewCommandsManagerClass.RunCommand());//TempNewCommandsManagerClass.ListOfInitializedCommandInterfaces[12]);
		}

		//private void SetActiveCommand(ICommandWithHandler command)
		public ICommandWithHandler ActiveCommand
		{
			get { return this.DataContext as ICommandWithHandler; }
			set
			{
				this.DataContext = value;
				this.UpdateLayout();
				customAutocompleteTextbox1.myACB.IsEnabled = value == null;

				ActiveCommandNameBorder.Visibility =
					ArgumentsListbox.Visibility =
					value == null ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
			}
		}

		//ContextMenu cm = new ContextMenu() { Visibility = Visibility.Collapsed };
		private void AutoCompleteActualTextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			//(sender as TextBox).ContextMenu = GetActualTextBoxOfAutocompleteControl().Text == "" ? cm : null;
			//if (GetActualTextBoxOfAutocompleteControl().Text == "") e.Handled = true;
			//if (textBox_CommandLine.ItemsSource == null) e.Handled = true;
		}

		private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Tab && ActiveCommand == null)
			{
				foreach (ICommandWithHandler tmpcomm in TempNewCommandsManagerClass.ListOfInitializedCommandInterfaces)
					if (tmpcomm.CommandName.ToLower() == customAutocompleteTextbox1.Text.ToLower() || tmpcomm.DisplayName.ToLower() == customAutocompleteTextbox1.Text.ToLower())
					{
						ActiveCommand = tmpcomm;
						customAutocompleteTextbox1.IsDropDownOpen = false;
						customAutocompleteTextbox1.SelectedItem = null;
						customAutocompleteTextbox1.Text = "";
						break;
					}
				//int ItemIndex = (customAutocompleteTextbox1.ItemsSource as IList<ICommandWithHandler>).IndexOf(customAutocompleteTextbox1.Text);
				//if (ItemIndex != -1)
				//	ActiveCommand = (customAutocompleteTextbox1.ItemsSource as IList<string>)[ItemIndex] as ICommandWithHandler;
			}
			else if (e.Key == Key.Escape)
			{
				ActiveCommand = null;
			}
		}
	}
}
