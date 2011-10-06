using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuickAccess
{
	public partial class CommandForm : Form
	{
		public CommandForm(string WindowTitle)
		{
			InitializeComponent();
			tableLayoutPanel1.Controls.Clear();
			this.Text = WindowTitle;
		}

		private void tableLayoutPanel1_ControlAdded(object sender, ControlEventArgs e)
		{
			//try { tableLayoutPanel1.RowStyles[tableLayoutPanel1.GetRow(e.Control)] = new RowStyle(SizeType.AutoSize); }
			//catch { }
		}

		public void AddControl(string label, Control control)
		{
			if (tableLayoutPanel1.Controls.Count > 0) tableLayoutPanel1.RowCount++;
			tableLayoutPanel1.Controls.Add(new Label() { Text = label, Margin = new Padding(3, 5, 3, 0) });
			tableLayoutPanel1.Controls.Add(control);
		}
	}
}
