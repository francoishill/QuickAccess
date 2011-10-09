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
		labelTitle.Content = WindowTitle.ToUpper();
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

	public Point PositionBeforeActivated { get; set; }
	private void Window_Activated(object sender, EventArgs e)
	{
		//this.FormFadein.BeginAnimation(this, this.FormFadeAnimation.);
		//this.Opacity = 1;

		//mainBorder.LayoutTransform = new ScaleTransform(1.5, 1.5);

		//System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;//System.Windows.Forms.Screen..FromPoint();//.FromHandle(new WindowInteropHelper(Application.Current.MainWindow).Handle).WorkingArea;
		//this.Left = workingArea.Left + (workingArea.Width - this.Width) / 2;
		//this.Top = workingArea.Top + (workingArea.Height - this.Height) / 2;

		if (AllowedToAnimationLocation) AnimateWindowActivation();
	}

	public bool AllowedToAnimationLocation { get; set; }// = false;
	Storyboard storyBoardActivated = new Storyboard();
	private void AnimateWindowActivation()
	{
		//this.RegisterName(mainWindow.Name, mainWindow);

		if (storyBoardActivated.Children.Count == 0)
		{
			const double ScaleFactor = 2.5;
			double durationSeconds = 500;

			System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;

			double newTopPosition = workingArea.Top + (workingArea.Height - this.Height * ScaleFactor) / 2;
			DoubleAnimation windowTopAnimation = new DoubleAnimation(newTopPosition, new Duration(TimeSpan.FromMilliseconds(durationSeconds)));
			windowTopAnimation.AutoReverse = false;
			windowTopAnimation.RepeatBehavior = new RepeatBehavior(1);

			double newLeftPosition = workingArea.Left + (workingArea.Width - this.Width * ScaleFactor) / 2;
			DoubleAnimation windowLeftAnimation = new DoubleAnimation(newLeftPosition, new Duration(TimeSpan.FromMilliseconds(durationSeconds)));
			windowLeftAnimation.AutoReverse = false;
			windowLeftAnimation.RepeatBehavior = new RepeatBehavior(1);

			DoubleAnimation windowScaleXanimation = new DoubleAnimation(ScaleFactor, new Duration(TimeSpan.FromMilliseconds(durationSeconds / 2)));
			windowScaleXanimation.AutoReverse = false;
			windowScaleXanimation.RepeatBehavior = new RepeatBehavior(1);

			DoubleAnimation windowScaleYanimation = new DoubleAnimation(ScaleFactor, new Duration(TimeSpan.FromMilliseconds(durationSeconds / 2)));
			windowScaleYanimation.AutoReverse = false;
			windowScaleYanimation.RepeatBehavior = new RepeatBehavior(1);

			storyBoardActivated.Children.Add(windowTopAnimation);
			storyBoardActivated.Children.Add(windowLeftAnimation);
			storyBoardActivated.Children.Add(windowScaleXanimation);
			storyBoardActivated.Children.Add(windowScaleYanimation);
			Storyboard.SetTargetName(windowTopAnimation, mainWindow.Name);
			Storyboard.SetTargetName(windowLeftAnimation, mainWindow.Name);
			Storyboard.SetTargetName(windowScaleXanimation, mainBorder.Name);
			Storyboard.SetTargetName(windowScaleYanimation, mainBorder.Name);
			Storyboard.SetTargetProperty(windowTopAnimation, new PropertyPath(Window.TopProperty));
			Storyboard.SetTargetProperty(windowLeftAnimation, new PropertyPath(Window.LeftProperty));
			Storyboard.SetTargetProperty(windowScaleXanimation, (PropertyPath)new PropertyPathConverter().ConvertFromString("(FrameworkElement.LayoutTransform).(ScaleTransform.ScaleX)"));
			Storyboard.SetTargetProperty(windowScaleYanimation, (PropertyPath)new PropertyPathConverter().ConvertFromString("(FrameworkElement.LayoutTransform).(ScaleTransform.ScaleY)"));
		}
		storyBoardActivated.Begin(this);
	}

	Storyboard storyBoardDeactivated = new Storyboard();
	private void Window_Deactivated(object sender, EventArgs e)
	{
		//mainBorder.LayoutTransform = new ScaleTransform(1, 1);

		if (AllowedToAnimationLocation) ReverseAnimateWindowActivation();

		//this.Left = PositionBeforeActivated.X;
		//this.Top = PositionBeforeActivated.Y;
		//this.Opacity = 0.75F; 
	}

	private void ReverseAnimateWindowActivation()
	{
		if (storyBoardDeactivated.Children.Count == 0)
		{
			double durationSeconds = 100;

			DoubleAnimation windowTopAnimation = new DoubleAnimation(PositionBeforeActivated.Y, new Duration(TimeSpan.FromMilliseconds(durationSeconds)));
			windowTopAnimation.AutoReverse = false;
			windowTopAnimation.RepeatBehavior = new RepeatBehavior(1);

			DoubleAnimation windowLeftAnimation = new DoubleAnimation(PositionBeforeActivated.X, new Duration(TimeSpan.FromMilliseconds(durationSeconds)));
			windowLeftAnimation.AutoReverse = false;
			windowLeftAnimation.RepeatBehavior = new RepeatBehavior(1);

			DoubleAnimation windowScaleXanimation = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(durationSeconds)));
			windowScaleXanimation.AutoReverse = false;
			windowScaleXanimation.RepeatBehavior = new RepeatBehavior(1);

			DoubleAnimation windowScaleYanimation = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(durationSeconds)));
			windowScaleYanimation.AutoReverse = false;
			windowScaleYanimation.RepeatBehavior = new RepeatBehavior(1);

			storyBoardDeactivated.Children.Add(windowTopAnimation);
			storyBoardDeactivated.Children.Add(windowLeftAnimation);
			storyBoardDeactivated.Children.Add(windowScaleXanimation);
			storyBoardDeactivated.Children.Add(windowScaleYanimation);
			Storyboard.SetTargetName(windowTopAnimation, mainWindow.Name);
			Storyboard.SetTargetName(windowLeftAnimation, mainWindow.Name);
			Storyboard.SetTargetName(windowScaleXanimation, mainBorder.Name);
			Storyboard.SetTargetName(windowScaleYanimation, mainBorder.Name);
			Storyboard.SetTargetProperty(windowTopAnimation, new PropertyPath(Window.TopProperty));
			Storyboard.SetTargetProperty(windowLeftAnimation, new PropertyPath(Window.LeftProperty));
			Storyboard.SetTargetProperty(windowScaleXanimation, (PropertyPath)new PropertyPathConverter().ConvertFromString("(FrameworkElement.LayoutTransform).(ScaleTransform.ScaleX)"));
			Storyboard.SetTargetProperty(windowScaleYanimation, (PropertyPath)new PropertyPathConverter().ConvertFromString("(FrameworkElement.LayoutTransform).(ScaleTransform.ScaleY)"));
		}
		storyBoardDeactivated.Begin(this);
	}

	private void mainWindow_Loaded(object sender, RoutedEventArgs e)
	{
		mainBorder.LayoutTransform = new ScaleTransform(1, 1);
	}

	//class WindowRec : Window
	//{
	//  public static DependencyProperty WindowRect { get; set; }
	//}
}