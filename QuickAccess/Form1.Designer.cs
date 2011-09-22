namespace QuickAccess
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
					this.components = new System.ComponentModel.Container();
					System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
					this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
					this.contextMenuStrip_TrayIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
					this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
					this.textBox1 = new System.Windows.Forms.TextBox();
					this.label1 = new System.Windows.Forms.Label();
					this.textBox_Messages = new System.Windows.Forms.TextBox();
					this.label2 = new System.Windows.Forms.Label();
					this.contextMenuStrip_TrayIcon.SuspendLayout();
					this.SuspendLayout();
					// 
					// notifyIcon1
					// 
					this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip_TrayIcon;
					this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
					this.notifyIcon1.Text = "Press Ctrl + Q";
					this.notifyIcon1.Visible = true;
					this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
					// 
					// contextMenuStrip_TrayIcon
					// 
					this.contextMenuStrip_TrayIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
					this.contextMenuStrip_TrayIcon.Name = "contextMenuStrip_TrayIcon";
					this.contextMenuStrip_TrayIcon.Size = new System.Drawing.Size(153, 48);
					// 
					// exitToolStripMenuItem
					// 
					this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
					this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
					this.exitToolStripMenuItem.Text = "E&xit";
					this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
					// 
					// textBox1
					// 
					this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
											| System.Windows.Forms.AnchorStyles.Right)));
					this.textBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
					this.textBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
					this.textBox1.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
					this.textBox1.Location = new System.Drawing.Point(14, 157);
					this.textBox1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
					this.textBox1.Name = "textBox1";
					this.textBox1.Size = new System.Drawing.Size(425, 30);
					this.textBox1.TabIndex = 0;
					this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
					this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
					this.textBox1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form1_PreviewKeyDown);
					// 
					// label1
					// 
					this.label1.AutoSize = true;
					this.label1.Font = new System.Drawing.Font("Comic Sans MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
					this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
					this.label1.Location = new System.Drawing.Point(10, 16);
					this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
					this.label1.Name = "label1";
					this.label1.Size = new System.Drawing.Size(39, 17);
					this.label1.TabIndex = 2;
					this.label1.Text = "label1";
					// 
					// textBox_Messages
					// 
					this.textBox_Messages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
											| System.Windows.Forms.AnchorStyles.Left)
											| System.Windows.Forms.AnchorStyles.Right)));
					this.textBox_Messages.BackColor = System.Drawing.SystemColors.InactiveCaption;
					this.textBox_Messages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
					this.textBox_Messages.Location = new System.Drawing.Point(12, 45);
					this.textBox_Messages.Multiline = true;
					this.textBox_Messages.Name = "textBox_Messages";
					this.textBox_Messages.ScrollBars = System.Windows.Forms.ScrollBars.Both;
					this.textBox_Messages.Size = new System.Drawing.Size(585, 103);
					this.textBox_Messages.TabIndex = 1;
					this.textBox_Messages.WordWrap = false;
					this.textBox_Messages.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form1_PreviewKeyDown);
					// 
					// label2
					// 
					this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
					this.label2.AutoSize = true;
					this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
					this.label2.ForeColor = System.Drawing.SystemColors.AppWorkspace;
					this.label2.Location = new System.Drawing.Point(467, 190);
					this.label2.Name = "label2";
					this.label2.Size = new System.Drawing.Size(132, 13);
					this.label2.TabIndex = 3;
					this.label2.Text = "Right-click minimizes to tray";
					// 
					// Form1
					// 
					this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
					this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
					this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(51)))), ((int)(((byte)(68)))));
					this.ClientSize = new System.Drawing.Size(611, 207);
					this.ControlBox = false;
					this.Controls.Add(this.label2);
					this.Controls.Add(this.textBox_Messages);
					this.Controls.Add(this.label1);
					this.Controls.Add(this.textBox1);
					this.DoubleBuffered = true;
					this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
					this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
					this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
					this.KeyPreview = true;
					this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
					this.MaximizeBox = false;
					this.MinimizeBox = false;
					this.MinimumSize = new System.Drawing.Size(500, 180);
					this.Name = "Form1";
					this.Opacity = 0.99D;
					this.ShowInTaskbar = false;
					this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
					this.Text = "Quick Access";
					this.TopMost = true;
					this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
					this.Shown += new System.EventHandler(this.Form1_Shown);
					this.VisibleChanged += new System.EventHandler(this.Form1_VisibleChanged);
					this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
					this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
					this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form1_PreviewKeyDown);
					this.contextMenuStrip_TrayIcon.ResumeLayout(false);
					this.ResumeLayout(false);
					this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        public System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
				public System.Windows.Forms.TextBox textBox_Messages;
				private System.Windows.Forms.Label label2;
				private System.Windows.Forms.ContextMenuStrip contextMenuStrip_TrayIcon;
				private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}

