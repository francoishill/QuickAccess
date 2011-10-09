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
using System.Reflection;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
public partial class OverlayWindow : Window
{
	public class OverlayChildManager
	{
		public bool AllowShow;
		public bool EventAttached;
		public bool WindowAlreadyPositioned;

		public OverlayChildManager(bool AllowShowIn, bool EventAttachedIn, bool WindowAlreadyPositionedIn)
		{
			AllowShow = AllowShowIn;
			EventAttached = EventAttachedIn;
			WindowAlreadyPositioned = WindowAlreadyPositionedIn;
		}
	}

	public List<Window> ListOfChildWindows = new List<Window>();

	public OverlayWindow()
	{
		InitializeComponent();
		System.Windows.Forms.Application.EnableVisualStyles();
		System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
	}

	private void overlayWindow_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Left)
			this.Close();//.Hide();
	}

	private void overlayWindow_Loaded(object sender, RoutedEventArgs e)
	{
		//System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
		//timer.Interval = 1;
		//timer.Tick += delegate
		//{
		//  timer.Stop();
		//  timer = null;
		//  SetupAllChildWindows();
		//};
		//timer.Start();
	}

	public void SetupAllChildWindows()
	{
		foreach (Window window in ListOfChildWindows)
		{
			if (window.Tag == null)
				window.Tag = new OverlayChildManager(true, false, false);

			window.WindowStyle = WindowStyle.None;
			if (!window.AllowsTransparency) window.AllowsTransparency = true;
			//window.MaximizeBox = false;
			//window.MinimizeBox = false;
			window.ShowInTaskbar = false;
			//form.ShowIcon = false;

			if (!IsEventsAdded(window))//form))
			{
				//AddMouseDownEventToControlandSubcontrols(form);
				AddClosingEventToWindow(window);//form);
				AddMouseLeftButtonDownEventToWindow(window);
				//AddMouseRightButtonDownEventToWindow(window);
				AddMouseWheelEventToWindow(window);
				//AddKeydownEventToControlandSubcontrols(form);
				AddKeydownEventToWindowAndChildren(window);

				MarkformEventsAdded(window);
			}
			//form.TopMost = true;
			window.Topmost = true;
			if (MayFormBeShow(window))
			{
				window.WindowStartupLocation = WindowStartupLocation.Manual;
				//window.Opacity = 0.75F;
				window.Show();
				//if (propertyFreezeEvent_Activated != null) propertyFreezeEvent_Activated.SetValue(form, false, null);
			}
		}

		System.Windows.Forms.Application.DoEvents();
		AutoLayoutOfForms();
	}

	private void AddMouseLeftButtonDownEventToWindow(Window window)
	{
		window.MouseLeftButtonDown += (s, closeargs) =>
		{
			(s as Window).DragMove();
		};
	}

	private void AddMouseWheelEventToWindow(Window window)
	{
		window.MouseWheel += (s, evtargs) =>
		{
			if (evtargs.Delta > 0) ActivatePreviousWindowInChildList(FindWindowOfControl(s as System.Windows.Controls.Control));
			else ActivateNextWindowInChildList(FindWindowOfControl(s as System.Windows.Controls.Control));
		};
	}

	private bool IsWindowAlreadyPositioned(Window window)//Form form)
	{
		if (window == null) return false;
		if (!(window.Tag is OverlayChildManager)) return false;
		return (window.Tag as OverlayChildManager).WindowAlreadyPositioned;
	}

	private void MarkfWindowAsAlreadyPositioned(Window window)// Form form)
	{
		if (window == null) return;
		if (!(window.Tag is OverlayChildManager)) return;
		(window.Tag as OverlayChildManager).WindowAlreadyPositioned = true;
	}

	private void AutoLayoutOfForms()
	{
		int leftGap = 20;
		int topGap = 10;

		int NextLeftPos = leftGap;
		int MaxHeightInRow = 0;
		int NextTopPos = topGap;

		System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;

		foreach (Window window in ListOfChildWindows)
		{
			//window.WindowStartupLocation = WindowStartupLocation.Manual;
			if (NextLeftPos + window.Width + leftGap >= workingArea.Right)
			{
				NextTopPos += MaxHeightInRow + topGap;
				NextLeftPos = leftGap;
				MaxHeightInRow = 0;
			}

			if (!IsWindowAlreadyPositioned(window))
			{
				window.Left = NextLeftPos;
				window.Top = NextTopPos;
				MarkfWindowAsAlreadyPositioned(window);
			}
			NextLeftPos += (int)window.Width + leftGap;
			if (window.Height > MaxHeightInRow) MaxHeightInRow = (int)window.Height;

			//PositionBeforeActivated = new Point(this.Left, this.Top);
			PropertyInfo propertyPositionBeforeActivated = window.GetType().GetProperty("PositionBeforeActivated");
			if (propertyPositionBeforeActivated != null) propertyPositionBeforeActivated.SetValue(window, new System.Windows.Point(window.Left, window.Top), null);

			PropertyInfo propertyFreezeEvent_Activated = window.GetType().GetProperty("AllowedToAnimationLocation");
			if (propertyFreezeEvent_Activated != null) propertyFreezeEvent_Activated.SetValue(window, true, null);
		}
	}

	private void AddKeydownEventToWindowAndChildren(Window window)
	{
		window.KeyDown += new System.Windows.Input.KeyEventHandler(control_KeyDown1);
		foreach (object o in LogicalTreeHelper.GetChildren(window))
			if (o is System.Windows.Controls.Control)
			{
				(o as System.Windows.Controls.Control).KeyUp += new System.Windows.Input.KeyEventHandler(control_KeyDown1);
			}
	}

	void control_KeyDown1(object sender, System.Windows.Input.KeyEventArgs e)
	{
		if (e.Key == Key.Escape)
		{
			this.Activate();
		}

		//if (e.Key == System.Windows.Input.Key.Tab && ModifierKeys == System.Windows.Forms.Keys.None)
		//{
		//  TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
		//  UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;
		//  if (keyboardFocus != null)
		//    keyboardFocus.MoveFocus(tRequest);
		//}
		if (e.Key == System.Windows.Input.Key.Tab && System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
		{
			e.Handled = true;
			ActivateNextWindowInChildList(FindWindowOfControl(sender as System.Windows.Controls.Control));
		}
		//else if (e.KeyCode == Keys.Tab && (ModifierKeys & (Keys.Control | Keys.Shift)) == (Keys.Control | Keys.Shift))
		else if (e.Key == System.Windows.Input.Key.Tab && (System.Windows.Forms.Control.ModifierKeys & (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)) == (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift))
		{
			e.Handled = true;
			ActivatePreviousWindowInChildList(FindWindowOfControl(sender as System.Windows.Controls.Control));
		}
	}

	private void ActivatePreviousWindowInChildList(Window window)
	{
		if (ListOfChildWindows != null && ListOfChildWindows.IndexOf(window) != -1)
		{
			int currentActiveFormIndex = ListOfChildWindows.IndexOf(window);
			int newIndexToActivate = currentActiveFormIndex == 0 ? ListOfChildWindows.Count - 1 : currentActiveFormIndex - 1;
			ListOfChildWindows[newIndexToActivate].Activate();
		}
		else if (ListOfChildWindows != null)
			ListOfChildWindows[0].Activate();
	}

	private void ActivateNextWindowInChildList(Window window)
	{
		if (ListOfChildWindows != null && ListOfChildWindows.IndexOf(window) != -1)
		{
			int currentActiveFormIndex = ListOfChildWindows.IndexOf(window);
			int newIndexToActivate = currentActiveFormIndex == ListOfChildWindows.Count - 1 ? 0 : currentActiveFormIndex + 1;
			ListOfChildWindows[newIndexToActivate].Activate();
		}
		else if (ListOfChildWindows != null)
			ListOfChildWindows[0].Activate();
	}

	private Window FindWindowOfControl(System.Windows.Controls.Control control)
	{
		System.Windows.Controls.Control tmpControl = control;
		while (!(tmpControl is Window))
			tmpControl = (System.Windows.Controls.Control)tmpControl.Parent;
		return tmpControl as Window;
	}

	private void AddClosingEventToWindow(System.Windows.Window window)//Form form)
	{
		//form.FormClosing += (s, closeargs) =>
		window.Closing += (s, closeargs) =>
		{
			System.Windows.Forms.Form thisForm = s as System.Windows.Forms.Form;
			//if (closeargs.CloseReason == CloseReason.UserClosing)
			{
				closeargs.Cancel = true;
				thisForm.Hide();
				SetFormAllowShow(thisForm, false);
				//UserMessages.ShowMessage("Userclose");
			}
		};
	}

	private void SetFormAllowShow(System.Windows.Forms.Form form, bool allowShowValue)
	{
		if (form == null) return;
		if (!(form.Tag is OverlayChildManager)) return;
		(form.Tag as OverlayChildManager).AllowShow = allowShowValue;
	}

	private bool MayFormBeShow(System.Windows.Window window)// Form form)
	{
		if (window == null) return true;
		if (!(window.Tag is OverlayChildManager)) return true;
		return (window.Tag as OverlayChildManager).AllowShow;
	}

	private bool IsEventsAdded(System.Windows.Window window)//Form form)
	{
		if (window == null) return false;
		if (!(window.Tag is OverlayChildManager)) return false;
		return (window.Tag as OverlayChildManager).EventAttached;
	}

	private void MarkformEventsAdded(System.Windows.Window window)// Form form)
	{
		if (window == null) return;
		if (!(window.Tag is OverlayChildManager)) return;
		(window.Tag as OverlayChildManager).EventAttached = true;
	}

	public bool PreventClosing = true;
	private void overlayWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		if (PreventClosing) e.Cancel = true;
		this.Hide();
		foreach (System.Windows.Window window in ListOfChildWindows)
			if (window != null)
			{
				window.Owner = null;
				window.Hide();
			}
	}

	private void overlayWindow_Activated(object sender, EventArgs e)
	{
		this.Topmost = true;
		this.Topmost = false;
	}

	private void overlayWindow_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Escape)
			this.Close();
		else if (e.Key == Key.Tab && System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
		{
			if (ListOfChildWindows != null && ListOfChildWindows.Count > 0)
				ListOfChildWindows[0].Activate();
		}
		else if (e.Key == Key.Tab && (System.Windows.Forms.Control.ModifierKeys & (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)) == (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift))
		{
			if (ListOfChildWindows != null && ListOfChildWindows.Count > 0)
				ListOfChildWindows[ListOfChildWindows.Count - 1].Activate();
		}
	}
}