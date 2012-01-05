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
	public partial class OverlayGestures : Window
	{
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

			this.mouseGestures1 = new MouseGestures.MouseGestures(true);//this.components);

			mouseGestures1.BeginGestureEvent += delegate
			{
				IsGestureBusy = true;
			};
			mouseGestures1.MouseMove += delegate
			{
				if (IsGestureBusy)
				{
					System.Drawing.Point pt = System.Windows.Forms.Cursor.Position; // Get the mouse cursor in screen coordinates 
					using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
					{
						g.FillEllipse(System.Drawing.Brushes.Black, pt.X, pt.Y, 5, 5);
					}
				}
			};
			mouseGestures1.EndGestureEvent += delegate
			{
				IsGestureBusy = false;
				//this.Refresh();
				this.UpdateLayout();
				this.Close();
			};
			mouseGestures1.Gesture += (sndr, evtargs) =>
			{
				this.UpdateLayout();
				this.Close();
				switch (evtargs.Gesture.ToString())
				{
					case "URDLU":
						Clipboard.SetText("bokbokkie");
						RunMethodAfterMilliseconds(delegate { Clipboard.Clear(); }, 7000);
						break;
					default: System.Windows.Forms.MessageBox.Show("Unknown gesture: " + evtargs.Gesture.ToString()); break;
				}
			};
		}
	}
}
