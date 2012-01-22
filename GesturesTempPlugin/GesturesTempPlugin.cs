using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineCommandToolkit;
//using InterfaceForQuickAccessPlugin;
using SharedClasses;

namespace MouseGesturePlugins
{
	public class GesturesTemp1 : IMouseGesture//, IQuickAccessPluginInterface
	{
		public string GestureString { get { return "UR"; } }

		public string ThisCommandName { get { return "Temp1"; } }

		public bool PerformCommandAfterGesture(out string ErrorMessage)
		{
			try
			{
				System.Diagnostics.Process.Start(@"c:\francois");
				ErrorMessage = null;
				return true;
			}
			catch (Exception exc)
			{
				ErrorMessage = exc.Message;
				return false;
			}
		}
	}

	public class GesturesTemp2 : IMouseGesture//, IQuickAccessPluginInterface
	{
		public string GestureString { get { return "UL"; } }

		public string ThisCommandName { get { return "Temp2"; } }

		public bool PerformCommandAfterGesture(out string ErrorMessage)
		{
			try
			{
				System.Diagnostics.Process.Start(@"c:\windows");
				ErrorMessage = null;
				return true;
			}
			catch (Exception exc)
			{
				ErrorMessage = exc.Message;
				return false;
			}
		}
	}

	public class GesturesTemp3 : IMouseGesture//, IQuickAccessPluginInterface
	{
		public string GestureString { get { return "DR"; } }

		public string ThisCommandName { get { return "Temp3"; } }

		public bool PerformCommandAfterGesture(out string ErrorMessage)
		{
			try
			{
				string clipboardText = "francoishill11@gmail.com";
				int durationTextWillBeInClipboard = 5000;
				UserMessages.ShowMessage("Press ok and then the clipboard will be set to \"" + clipboardText + "\" and removed after " + durationTextWillBeInClipboard + " milliseconds");
				System.Windows.Forms.Clipboard.SetText(clipboardText);
				OverlayGesturesForm.RunMethodAfterMilliseconds(delegate { System.Windows.Forms.Clipboard.Clear(); }, durationTextWillBeInClipboard);
				ErrorMessage = null;
				return true;
			}
			catch (Exception exc)
			{
				ErrorMessage = exc.Message;
				return false;
			}
		}
	}
}
