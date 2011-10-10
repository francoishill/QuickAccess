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

	//public List<Window> ListOfCommandUsercontrols = new List<Window>();
	public List<CommandUserControl> ListOfCommandUsercontrols = new List<CommandUserControl>();

	public OverlayWindow()
	{
		InitializeComponent();
		System.Windows.Forms.Application.EnableVisualStyles();
		System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
	}

	private Element currentElement = new Element(); 
	private void overlayWindow_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Left && currentElement.InputElement == null)
			this.Close();

		currentElement.InputElement = null;
		//if (this.currentElement == null || this.currentElement.InputElement == null) return;

		//this.currentElement.X = Mouse.GetPosition((IInputElement)sender).X;
		//this.currentElement.Y = Mouse.GetPosition((IInputElement)sender).Y;
		//// Ensure object receives all mouse events.
		//this.currentElement.InputElement.CaptureMouse();
	}

	private void overlayWindow_MouseUp(object sender, MouseButtonEventArgs e)
	{
		//if (this.currentElement == null || this.currentElement.InputElement == null) return;
		//if (this.currentElement.InputElement != null)
		//  this.currentElement.InputElement.ReleaseMouseCapture();
	}

	private void overlayWindow_MouseMove(object sender, MouseEventArgs e)
	{
		//if (this.currentElement == null || this.currentElement.InputElement == null) return;
		//// if mouse is down when its moving, then it's dragging current
		//if (e.LeftButton == MouseButtonState.Pressed)
		//  this.currentElement.IsDragging = true;

		//if (this.currentElement.IsDragging && currentElement.InputElement != null)
		//{
		//  // Retrieve the current position of the mouse.
		//  var newX = Mouse.GetPosition((IInputElement)sender).X;
		//  var newY = Mouse.GetPosition((IInputElement)sender).Y;
		//  // Reset the location of the object (add to sender's renderTransform
		//  // newPosition minus currentElement's position
		//  var rt = ((UIElement)this.currentElement.InputElement).RenderTransform;
		//  var offsetX = rt.Value.OffsetX;
		//  var offsetY = rt.Value.OffsetY;
		//  try
		//  {
		//    rt.SetValue(TranslateTransform.XProperty, offsetX + newX - currentElement.X);
		//    rt.SetValue(TranslateTransform.YProperty, offsetY + newY - currentElement.Y);
		//    // Update position of the mouse
		//    currentElement.X = newX;
		//    currentElement.Y = newY;
		//  }
		//  catch (Exception exc) { }
		//}
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
		foreach (CommandUserControl usercontrol in ListOfCommandUsercontrols)
		{
			if (usercontrol.Tag == null)
				usercontrol.Tag = new OverlayChildManager(true, false, false);

			//usercontrol.WindowStyle = WindowStyle.None;
			//if (!usercontrol.AllowsTransparency) usercontrol.AllowsTransparency = true;
			//window.MaximizeBox = false;
			//window.MinimizeBox = false;
			//usercontrol.ShowInTaskbar = false;
			//form.ShowIcon = false;

			if (!IsEventsAdded(usercontrol))//form))
			{
				//AddMouseDownEventToControlandSubcontrols(form);
				//AddClosingEventToWindow(usercontrol);//form);
				AddMouseLeftButtonDownEventToCommandUsercontrol(usercontrol);
				//AddMouseRightButtonDownEventToWindow(window);
				AddMouseWheelEventToWindow(usercontrol);
				//AddKeydownEventToControlandSubcontrols(form);
				AddKeydownEventToWindowAndChildren(usercontrol);

				MarkformEventsAdded(usercontrol);
			}

			//WpfDraggableCanvas draggableCanvas = new WpfDraggableCanvas();
			//draggableCanvas.
			//draggableCanvas.Children.Add(usercontrol);
			//wrapPanel_UserControls.Children.Add(draggableCanvas);
			if (wrapPanel_UserControls.Children.IndexOf(usercontrol) == -1)
				wrapPanel_UserControls.Children.Add(usercontrol);
			//this.mainGrid.Children.Add(usercontrol);

			//form.TopMost = true;
			//usercontrol.Topmost = true;
			usercontrol.Visibility = MayUsercontrolBeShown(usercontrol) ? Visibility.Visible : Visibility.Hidden;
		}

		System.Windows.Forms.Application.DoEvents();
		AutoLayoutOfForms();
	}

	private void AddMouseLeftButtonDownEventToCommandUsercontrol(CommandUserControl usercontrol)
	{
		usercontrol.MouseLeftButtonDown += (s, closeargs) =>
		{
			this.currentElement.InputElement = (IInputElement)s;
			//TODO: DragMove to be implemented for usercontrol because its not window anymore
			//(s as CommandUserControl).DragMove();
		};
	}

	private void AddMouseWheelEventToWindow(Control usercontrol)
	{
		usercontrol.MouseWheel += (s, evtargs) =>
		{
			if (evtargs.Delta > 0) ActivatePreviousWindowInChildList(FindCommandUsercontrolOfControl(s as System.Windows.Controls.Control));
			else ActivateNextWindowInChildList(FindCommandUsercontrolOfControl(s as System.Windows.Controls.Control));
		};
	}

	private bool IsWindowAlreadyPositioned(CommandUserControl usercontrol)//Form form)
	{
		if (usercontrol == null) return false;
		if (!(usercontrol.Tag is OverlayChildManager)) return false;
		return (usercontrol.Tag as OverlayChildManager).WindowAlreadyPositioned;
	}

	private void MarkfWindowAsAlreadyPositioned(CommandUserControl usercontrol)// Form form)
	{
		if (usercontrol == null) return;
		if (!(usercontrol.Tag is OverlayChildManager)) return;
		(usercontrol.Tag as OverlayChildManager).WindowAlreadyPositioned = true;
	}

	private void AutoLayoutOfForms()
	{
		int leftGap = 20;
		int topGap = 10;

		int NextLeftPos = leftGap;
		int MaxHeightInRow = 0;
		int NextTopPos = topGap;

		System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;

		foreach (CommandUserControl usercontrol in ListOfCommandUsercontrols)
		{
			//window.WindowStartupLocation = WindowStartupLocation.Manual;
			if (NextLeftPos + usercontrol.Width + leftGap >= workingArea.Right)
			{
				NextTopPos += MaxHeightInRow + topGap;
				NextLeftPos = leftGap;
				MaxHeightInRow = 0;
			}

			if (!IsWindowAlreadyPositioned(usercontrol))
			{
				//usercontrol.Left = NextLeftPos;
				//usercontrol.Top = NextTopPos;
				MarkfWindowAsAlreadyPositioned(usercontrol);
			}
			NextLeftPos += (int)usercontrol.Width + leftGap;
			if (usercontrol.Height > MaxHeightInRow) MaxHeightInRow = (int)usercontrol.Height;

			//PropertyInfo propertyPositionBeforeActivated = usercontrol.GetType().GetProperty("PositionBeforeActivated");
			//if (propertyPositionBeforeActivated != null) propertyPositionBeforeActivated.SetValue(usercontrol, new System.Windows.Point(usercontrol.Left, usercontrol.Top), null);

			//PropertyInfo propertyFreezeEvent_Activated = usercontrol.GetType().GetProperty("AllowedToAnimationLocation");
			//if (propertyFreezeEvent_Activated != null) propertyFreezeEvent_Activated.SetValue(usercontrol, true, null);
		}
	}

	private void AddKeydownEventToWindowAndChildren(Control usercontrol)
	{
		usercontrol.KeyDown += new System.Windows.Input.KeyEventHandler(control_KeyDown1);
		foreach (object o in LogicalTreeHelper.GetChildren(usercontrol))
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
			ActivateNextWindowInChildList(FindCommandUsercontrolOfControl(sender as CommandUserControl));
		}
		//else if (e.KeyCode == Keys.Tab && (ModifierKeys & (Keys.Control | Keys.Shift)) == (Keys.Control | Keys.Shift))
		else if (e.Key == System.Windows.Input.Key.Tab && (System.Windows.Forms.Control.ModifierKeys & (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)) == (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift))
		{
			e.Handled = true;
			ActivatePreviousWindowInChildList(FindCommandUsercontrolOfControl(sender as CommandUserControl));
		}
	}

	private void ActivatePreviousWindowInChildList(CommandUserControl usercontrol)
	{
		if (ListOfCommandUsercontrols != null && ListOfCommandUsercontrols.IndexOf(usercontrol) != -1)
		{
			int currentActiveFormIndex = ListOfCommandUsercontrols.IndexOf(usercontrol);
			int newIndexToActivate = currentActiveFormIndex == 0 ? ListOfCommandUsercontrols.Count - 1 : currentActiveFormIndex - 1;
			ListOfCommandUsercontrols[newIndexToActivate].Focus();
		}
		else if (ListOfCommandUsercontrols != null)
			ListOfCommandUsercontrols[0].Focus();
	}

	private void ActivateNextWindowInChildList(CommandUserControl usercontrol)
	{
		if (ListOfCommandUsercontrols != null && ListOfCommandUsercontrols.IndexOf(usercontrol) != -1)
		{
			int currentActiveFormIndex = ListOfCommandUsercontrols.IndexOf(usercontrol);
			int newIndexToActivate = currentActiveFormIndex == ListOfCommandUsercontrols.Count - 1 ? 0 : currentActiveFormIndex + 1;
			ListOfCommandUsercontrols[newIndexToActivate].Focus();
		}
		else if (ListOfCommandUsercontrols != null)
			ListOfCommandUsercontrols[0].Focus();
	}

	private CommandUserControl FindCommandUsercontrolOfControl(System.Windows.Controls.Control control)
	{
		System.Windows.Controls.Control tmpControl = control;
		while (!(tmpControl is CommandUserControl))
			tmpControl = (System.Windows.Controls.Control)tmpControl.Parent;
		return tmpControl as CommandUserControl;
	}

	private void AddClosingEventToWindow(Control usercontrol)//Form form)
	{
		//usercontrol.Closing += (s, closeargs) =>
		//{
		//  System.Windows.Forms.Form thisForm = s as System.Windows.Forms.Form;
		//  //if (closeargs.CloseReason == CloseReason.UserClosing)
		//  {
		//    closeargs.Cancel = true;
		//    thisForm.Hide();
		//    SetFormAllowShow(thisForm, false);
		//    //UserMessages.ShowMessage("Userclose");
		//  }
		//};
	}

	private void SetFormAllowShow(System.Windows.Forms.Form form, bool allowShowValue)
	{
		if (form == null) return;
		if (!(form.Tag is OverlayChildManager)) return;
		(form.Tag as OverlayChildManager).AllowShow = allowShowValue;
	}

	private bool MayUsercontrolBeShown(CommandUserControl usercontrol)// Form form)
	{
		if (usercontrol == null) return true;
		if (!(usercontrol.Tag is OverlayChildManager)) return true;
		return (usercontrol.Tag as OverlayChildManager).AllowShow;
	}

	private bool IsEventsAdded(Control usercontrol)//Form form)
	{
		if (usercontrol == null) return false;
		if (!(usercontrol.Tag is OverlayChildManager)) return false;
		return (usercontrol.Tag as OverlayChildManager).EventAttached;
	}

	private void MarkformEventsAdded(Control usercontrol)// Form form)
	{
		if (usercontrol == null) return;
		if (!(usercontrol.Tag is OverlayChildManager)) return;
		(usercontrol.Tag as OverlayChildManager).EventAttached = true;
	}

	public bool PreventClosing = true;
	private void overlayWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		if (PreventClosing) e.Cancel = true;
		this.Hide();
		foreach (CommandUserControl usercontrol in ListOfCommandUsercontrols)
			if (usercontrol != null)
			{
				//usercontrol.Owner = null;
				//usercontrol.Hide();
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
			if (ListOfCommandUsercontrols != null && ListOfCommandUsercontrols.Count > 0)
				ListOfCommandUsercontrols[0].Focus();
		}
		else if (e.Key == Key.Tab && (System.Windows.Forms.Control.ModifierKeys & (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)) == (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift))
		{
			if (ListOfCommandUsercontrols != null && ListOfCommandUsercontrols.Count > 0)
				ListOfCommandUsercontrols[ListOfCommandUsercontrols.Count - 1].Focus();
		}
	}

	public class Element
	{
		#region Fields
		bool isDragging = false;
		IInputElement inputElement = null;
		double x, y = 0;
		#endregion

		#region Constructor
		public Element() { }
		#endregion

		#region Properties
		public IInputElement InputElement
		{
			get { return this.inputElement; }
			set
			{
				this.inputElement = value;
				/* every time inputElement resets, the draggin stops (you actually don't even need to track it, but it made things easier in the begining, I'll change it next time I get to play with it. */
				this.isDragging = false;
			}
		}

		public double X
		{
			get { return this.x; }
			set { this.x = value; }
		}

		public double Y
		{
			get { return this.y; }
			set { this.y = value; }
		}

		public bool IsDragging
		{
			get { return this.isDragging; }
			set { this.isDragging = value; }
		}
		#endregion
	}
}