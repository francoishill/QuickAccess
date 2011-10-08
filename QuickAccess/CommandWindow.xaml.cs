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

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
public partial class MainWindow : Window
{
	public MainWindow(string WindowTitle)
	{
		InitializeComponent();
		this.Title = WindowTitle;
		labelTitle.Content = WindowTitle;
	}

	public void AddControl(string label, System.Windows.Controls.Control control, Color labelColor)
	{
		Label labelControl = new Label() { Content = label, Margin = new Thickness(3, 5, 3, 0), MinWidth = 50, Foreground = Brushes.White };
		
		gridTable.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

		gridTable.Children.Add(labelControl);
		Grid.SetColumn(labelControl, 0);
		Grid.SetRow(labelControl, gridTable.RowDefinitions.Count - 1);
		
		gridTable.Children.Add(control);
		Grid.SetColumn(control, 1);
		Grid.SetRow(control, gridTable.RowDefinitions.Count - 1);
		return;
		//StackPanel horizontalStackPanel = new StackPanel();
		//horizontalStackPanel.Orientation = Orientation.Horizontal;
		//horizontalStackPanel.Children.Add(new Label() { Content = label, Margin = new Thickness(3, 5, 3, 0) });
		//horizontalStackPanel.Children.Add(control);
		//verticalStackPanel.Children.Add(horizontalStackPanel);

		//if (tableLayoutPanel1.Controls.Count > 0) tableLayoutPanel1.RowCount++;
		//tableLayoutPanel1.Controls.Add(new Label() { Text = label, Margin = new Padding(3, 5, 3, 0) });
		//tableLayoutPanel1.Controls.Add(control);
	}

	private void Window_Activated(object sender, EventArgs e)
	{
		this.Opacity = 1;
	}

	private void Window_Deactivated(object sender, EventArgs e)
	{
		this.Opacity = 0.75F;
	}

	private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		DragMove();
	}
}