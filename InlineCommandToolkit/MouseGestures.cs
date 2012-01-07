using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseGesturePlugins
{
	public interface IMouseGesture
	{
		string GestureString { get; }
		string ThisCommandName { get; }
		bool PerformCommandAfterGesture(out string ErrorMessage);
	}
}
