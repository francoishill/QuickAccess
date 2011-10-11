using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;

	/// <summary>
	/// Interaction logic for tmpUserControl.xaml
	/// </summary>
public partial class CommandUserControl : UserControl
{
	public UIElement currentFocusedElement = null;

	public CommandUserControl(string CommandTitle)
	{
		InitializeComponent();

		labelTitle.Content = CommandTitle;
	}

	public void AddControl(string label, System.Windows.Controls.Control control, Color labelColor)
	{
		Label labelControl = new Label() { Content = label, Margin = new Thickness(3, 5, 3, 0), MinWidth = 50, Foreground = Brushes.White };

		AddRowToGrid();

		gridTable.Children.Add(labelControl);
		Grid.SetColumn(labelControl, 0);
		Grid.SetRow(labelControl, gridTable.RowDefinitions.Count - 1);

		gridTable.Children.Add(control);
		Grid.SetColumn(control, 1);
		Grid.SetRow(control, gridTable.RowDefinitions.Count - 1);
	}

	private void AddRowToGrid()
	{
		gridTable.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
	}

	public void AddTreeviewItem(object itemToAdd)
	{
		if (tmpTreeview == null)
		{
			tmpTreeview = new TreeView() { MinWidth = 100, MinHeight = 20, MaxHeight = 40, VerticalAlignment = System.Windows.VerticalAlignment.Top };
			gridTable.Children.Add(tmpTreeview);
			Grid.SetColumn(tmpTreeview, 2);
			if (gridTable.RowDefinitions.Count == 0) AddRowToGrid();
			Grid.SetRow(tmpTreeview, 0);
			Grid.SetRowSpan(tmpTreeview, 1000);
		}
		tmpTreeview.Items.Add(itemToAdd);
	}
	TreeView tmpTreeview = null;

	private void border_Closebutton_MouseUp(object sender, MouseButtonEventArgs e)
	{
		//this.Opacity = System.Windows.Visibility.Collapsed;
	}

	private void storyboardFadeout_Completed(object sender, EventArgs e)
	{
		//this.Visibility = System.Windows.Visibility.Collapsed;
		this.LayoutTransform = new ScaleTransform(0.1, 0.1);
	}

	private void mainGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		
	}

	private void parentUsercontrol_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		this.Focus();
		if (this.Tag != null && this.Tag is OverlayWindow.OverlayChildManager && (this.Tag as OverlayWindow.OverlayChildManager).FirstUIelementToFocus != null)
		{
			if (currentFocusedElement == null)
			{
				(this.Tag as OverlayWindow.OverlayChildManager).FirstUIelementToFocus.Focus();
				currentFocusedElement = (this.Tag as OverlayWindow.OverlayChildManager).FirstUIelementToFocus;
			}
			else
			{
				currentFocusedElement.Focus();
			}
		}
	}

	//private void mainGrid_GotFocus(object sender, RoutedEventArgs e)
	//{
	//  System.Windows.Forms.MessageBox.Show("Test");
	//  if (this.Tag != null && this.Tag is Control)
	//    (this.Tag as Control).Focus();
	//}
}