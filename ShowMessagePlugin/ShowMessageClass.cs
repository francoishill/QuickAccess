using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InlineCommandToolkit;
//using InterfaceForQuickAccessPlugin;

namespace ShowMessagePlugin
{
	public class ShowMessageClass : IQuickAccessPluginInterface
	{
		public void Rundefault()
		{
			MessageBox.Show("Test");
		}
	}
}
