using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickAccess
{
	public partial class OverlayGesturesForm : Form
	{
		private bool IsGestureBusy = false;

		public OverlayGesturesForm()
		{
			InitializeComponent();
		}

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

		private void mouseGestures1_BeginGestureEvent(object sender, EventArgs e)
		{
			IsGestureBusy = true;
		}

		private void mouseGestures1_EndGestureEvent(object sender, EventArgs e)
		{
			IsGestureBusy = false;
			this.Refresh();
			this.Close();
		}

		private void mouseGestures1_Gesture(object sender, MouseGestures.MouseGestureEventArgs e)
		{
			this.Refresh();
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

		private void mouseGestures1_MouseMove(object sender, EventArgs e)
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
	}
}
