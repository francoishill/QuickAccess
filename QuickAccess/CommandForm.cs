using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

public partial class CommandForm : Form
{
	private Size SizeBeforeAddingPredefinedList;
	private Point LocationBeforeAddingAndCentering;

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

	public void AddControl(string label, Control control, Color labelColor)
	{
		if (tableLayoutPanel1.Controls.Count > 0) tableLayoutPanel1.RowCount++;
		tableLayoutPanel1.Controls.Add(new Label() { Text = label, Margin = new Padding(3, 5, 3, 0) });
		tableLayoutPanel1.Controls.Add(control);
	}

	private void CommandForm_ResizeBegin(object sender, EventArgs e)
	{
		this.SuspendLayout();
	}

	private void CommandForm_ResizeEnd(object sender, EventArgs e)
	{
		this.ResumeLayout();
	}

	//Do not rename this member (FreezeEvent_Activated) as OverlayForm searches for it by name
	public bool FreezeEvent_Activated { get; set; }
	private void CommandForm_Activated(object sender, EventArgs e)
	{
		if (!FreezeEvent_Activated)
			this.Opacity = 1;
		//SizeBeforeAddingPredefinedList = this.Size;
		//LocationBeforeAddingAndCentering = this.Location;
		//if (!FreezeEvent_Activated)
		//{
		//  Size newSize= new Size(this.Size.Width + 100, this.Size.Height + 100);
		//  if (this.Size != newSize) this.Size = newSize;
		//  //Rectangle workingArea = Screen.FromHandle(this.Handle).WorkingArea;
		//  //this.Location = new Point((workingArea.Width - this.Width) / 2, (workingArea.Height - this.Height) / 2);
		//  this.Opacity = 1;
		//}
	}

	private void CommandForm_Deactivate(object sender, EventArgs e)
	{
		if (!FreezeEvent_Activated)
			this.Opacity = 0.75F;
		FreezeEvent_Activated = false;
		////if (!FreezeEvent_Activated)
		////{
		//  this.Size = SizeBeforeAddingPredefinedList;
		//  //this.Location = LocationBeforeAddingAndCentering;
		//  this.Opacity = 0.75F;
		////}
		//FreezeEvent_Activated = false;
	}
}