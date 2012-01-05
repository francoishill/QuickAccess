using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuickAccess
{
	/// <summary>
	/// Interaction logic for OverlayGestures.xaml
	/// </summary>
	public partial class OverlayGestures : Window, System.ComponentModel.IContainer
	{
		private System.ComponentModel.IContainer components = null;
		private MouseGestures.MouseGestures mouseGestures1;

		private bool IsGestureBusy = false;

		private void RunMethodAfterMilliseconds(Action method, int milliseconds)
		{
			System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
			timer.Interval = milliseconds;
			timer.Tick += delegate
			{
				timer.Dispose();
				timer = null;
				method();
			};
			timer.Start();
		}

		public OverlayGestures()
		{
			InitializeComponent();
			this.components = new System.ComponentModel.Container();





			//TODO: SHould figure out how to use an IContainer for the following call (already tried to implement the interface in this window
			this.mouseGestures1 = new MouseGestures.MouseGestures(true);//this);






			//mouseGestures1.BeginGestureEvent -= new EventHandler(mouseGestures1_BeginGestureEvent);
			//mouseGestures1.MouseMove -= new EventHandler(mouseGestures1_MouseMove);
			//mouseGestures1.EndGestureEvent -= new EventHandler(mouseGestures1_EndGestureEvent);
			//mouseGestures1.Gesture -= new MouseGestures.MouseGestures.GestureHandler(mouseGestures1_Gesture);

			mouseGestures1.BeginGestureEvent += new EventHandler(mouseGestures1_BeginGestureEvent);
			mouseGestures1.MouseMove += new EventHandler(mouseGestures1_MouseMove);
			mouseGestures1.EndGestureEvent += new EventHandler(mouseGestures1_EndGestureEvent);
			mouseGestures1.Gesture += new MouseGestures.MouseGestures.GestureHandler(mouseGestures1_Gesture);

			//mouseGestures1.BeginGestureEvent += delegate
			//{
			//	IsGestureBusy = true;
			//};
			//mouseGestures1.MouseMove += delegate
			//{
			//	if (IsGestureBusy)
			//	{
			//		System.Drawing.Point pt = System.Windows.Forms.Cursor.Position; // Get the mouse cursor in screen coordinates 
			//		using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
			//		{
			//			g.FillEllipse(System.Drawing.Brushes.Black, pt.X, pt.Y, 5, 5);
			//		}
			//	}
			//};
			//mouseGestures1.EndGestureEvent += delegate
			//{
			//	IsGestureBusy = false;
			//	//this.Refresh();
			//	this.UpdateLayout();
			//	this.Close();
			//};
			//mouseGestures1.Gesture += (sndr, evtargs) =>
			//{
			//	this.UpdateLayout();
			//	this.Close();

			//	if (evtargs.Gesture.ToString() == "URDLU")
			//	{
			//		Clipboard.SetText("bokbokkie");
			//		RunMethodAfterMilliseconds(delegate { Clipboard.Clear(); }, 7000);
			//	}
			//	else if (GlobalSettings.MouseGesturesSettings.Instance.GetGesturesWithMessagesDictionary().ContainsKey(evtargs.Gesture.ToString()))
			//		UserMessages.ShowInfoMessage(GlobalSettings.MouseGesturesSettings.Instance.GetGesturesWithMessagesDictionary()[evtargs.Gesture.ToString()], "Gesture message");
			//	else
			//		System.Windows.Forms.MessageBox.Show("Unknown gesture: " + evtargs.Gesture.ToString());
			//};
		}

		void mouseGestures1_Gesture(object sender, MouseGestures.MouseGestureEventArgs e)
		{
			this.UpdateLayout();
			this.Close();

			if (e.Gesture.ToString() == "URDLU")
			{
				Clipboard.SetText("bokbokkie");
				RunMethodAfterMilliseconds(delegate { Clipboard.Clear(); }, 7000);
			}
			else if (GlobalSettings.MouseGesturesSettings.Instance.GetGesturesWithMessagesDictionary().ContainsKey(e.Gesture.ToString()))
				UserMessages.ShowInfoMessage(GlobalSettings.MouseGesturesSettings.Instance.GetGesturesWithMessagesDictionary()[e.Gesture.ToString()], "Gesture message");
			else
				System.Windows.Forms.MessageBox.Show("Unknown gesture: " + e.Gesture.ToString());
		}

		void mouseGestures1_EndGestureEvent(object sender, EventArgs e)
		{
			IsGestureBusy = false;
			//this.Refresh();
			this.UpdateLayout();
			this.Close();
		}

		void mouseGestures1_MouseMove(object sender, EventArgs e)
		{
			if (IsGestureBusy)
			{
				System.Drawing.Point pt = System.Windows.Forms.Cursor.Position; // Get the mouse cursor in screen coordinates 
				using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
				{
					g.FillEllipse(System.Drawing.Brushes.Black, pt.X, pt.Y, 5, 5);
				}
			}
		}

		void mouseGestures1_BeginGestureEvent(object sender, EventArgs e)
		{
			IsGestureBusy = true;
		}

		public void Add(System.ComponentModel.IComponent component, string name)
		{
			//throw new NotImplementedException();
		}

		public void Add(System.ComponentModel.IComponent component)
		{
			//Components
			//throw new NotImplementedException();
		}

		public System.ComponentModel.ComponentCollection Components
		{
			get;
			set;
			//get { throw new NotImplementedException(); }
		}

		public void Remove(System.ComponentModel.IComponent component)
		{
			//throw new NotImplementedException();
		}

		public void Dispose()
		{
			//throw new NotImplementedException();
		}
	}
}
