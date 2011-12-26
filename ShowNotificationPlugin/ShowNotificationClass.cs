using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InterfaceForQuickAccessPlugin;

namespace ShowNotificationPlugin
{
	public class ShowNotificationClass : IQuickAccessPluginInterface
	{
		public void Rundefault()
		{
			CustomBalloonTipwpf.ShowCustomBalloonTip(
				"Plugin title",
				"Plugin message",
				2000,
				CustomBalloonTipwpf.IconTypes.Shield,
				OnClickCallback: delegate { MessageBox.Show("Callback was clicked"); },
				Scaling: 2);
		}
	}
}
