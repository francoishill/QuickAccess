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
public partial class OverlayRibbon : Window
{
	public event EventHandler MouseClickedRequestToOpenOverlayWindow;

	public OverlayRibbon()
	{
		InitializeComponent();

		//stretchableGrid.RenderTransform = new ScaleTransform(0.1, 1);
		//mainBorder.RenderTransform = new ScaleTransform(0.1, 0.1);
	}

	private void mainWindow_LocationChanged(object sender, EventArgs e)
	{
		System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point(0, 0)).WorkingArea;
		if (this.Left != 0) this.Left = 0;
		if (this.Top + this.ActualHeight > workingArea.Bottom) this.Top = workingArea.Bottom - this.ActualHeight;
	}

	private void mainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (QuickAccess.Form1.IsControlDown()) this.DragMove();
		else CallEvent_MouseClickedRequestToOpenOverlayWindow();
	}
	
	private void CallEvent_MouseClickedRequestToOpenOverlayWindow()
	{
		if (MouseClickedRequestToOpenOverlayWindow != null) MouseClickedRequestToOpenOverlayWindow(this, new EventArgs());
	}

	private void mainWindow_DragEnter(object sender, DragEventArgs e)
	{
		CallEvent_MouseClickedRequestToOpenOverlayWindow();
	}

	//TODO: Read up a bit more on MeasureOverride and ArrangeOverride, see following line for website
	//http://www.dotnetfunda.com/articles/article900-wpf-tutorial--layoutpanelscontainers--layout-transformation-2-.aspx
	//protected override Size MeasureOverride(Size availableSize)
	//{
	//  return base.MeasureOverride(availableSize);
	//}
	//protected override Size ArrangeOverride(Size arrangeBounds)
	//{
	//  return base.ArrangeOverride(arrangeBounds);
	//}
}