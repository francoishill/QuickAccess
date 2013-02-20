using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickAccess
{
	public class RunningApps
	{
		public string AppName;
		public string AppPath;
		public int Count;
		public RunningApps(string AppName, string AppPath, int Count)
		{
			this.AppName = AppName;
			this.AppPath = AppPath;
			this.Count = Count;
		}
	}
}