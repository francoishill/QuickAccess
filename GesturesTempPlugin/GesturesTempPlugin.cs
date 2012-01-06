using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceForQuickAccessPlugin;

namespace MouseGesturePlugins
{
	public class GesturesTemp : IMouseGestures, IQuickAccessPluginInterface
	{
		public string GestureString { get { return "UR"; } }

		public string ThisCommandName { get { return "Temp"; } }

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
}
