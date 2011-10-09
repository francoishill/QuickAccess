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
using System.Windows.Interop;
using System.Windows.Media.Animation;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
public partial class MainWindow : Window
{
	public MainWindow(string WindowTitle)
	{
		InitializeComponent();
		System.Windows.Forms.Application.EnableVisualStyles();
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

	//private Point PositionBeforeActivated;
	private void Window_Activated(object sender, EventArgs e)
	{
		//this.FormFadein.BeginAnimation(this, this.FormFadeAnimation.);
		//this.Opacity = 1;
		//PositionBeforeActivated = new Point(this.Left, this.Top);
		mainBorder.LayoutTransform = new ScaleTransform(1.5, 1.5);
		//System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;//System.Windows.Forms.Screen..FromPoint();//.FromHandle(new WindowInteropHelper(Application.Current.MainWindow).Handle).WorkingArea;
		//this.Left = workingArea.Left + (workingArea.Width - this.Width) / 2;
		//this.Top = workingArea.Top + (workingArea.Height - this.Height) / 2;

		AnimateWindowActivation();
	}

	Storyboard storyBoard = new Storyboard();
	private void AnimateWindowActivation()
	{
		//this.RegisterName(mainBorder.Name, mainBorder);

		//DoubleAnimation opacityDoubleAnimation = new DoubleAnimation();

		//opacityDoubleAnimation.From = 0;
		//opacityDoubleAnimation.To = 0.1;
		//opacityDoubleAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 1500));
		//opacityDoubleAnimation.AutoReverse = false;
		//opacityDoubleAnimation.RepeatBehavior = new RepeatBehavior(1);

		
		//storyBoard.Children.Add(opacityDoubleAnimation);
		//Storyboard.SetTargetName(opacityDoubleAnimation, mainBorder.Name);
		//Storyboard.SetTargetProperty(opacityDoubleAnimation, new PropertyPath(Border.OpacityProperty));
		//storyBoard.Begin(this);
	}

	private void Window_Deactivated(object sender, EventArgs e)
	{
		mainBorder.LayoutTransform = new ScaleTransform(1, 1);
		//this.Left = PositionBeforeActivated.X;
		//this.Top = PositionBeforeActivated.Y;
		//this.Opacity = 0.75F; 
	}
}