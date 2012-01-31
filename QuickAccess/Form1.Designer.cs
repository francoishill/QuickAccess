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
			this.label1 = new System.Windows.Forms.Label();
			this.textBox_Messages = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.contextMenu_TrayIcon = new System.Windows.Forms.ContextMenu();
			this.menuItem_Exit = new System.Windows.Forms.MenuItem();
			this.menuItem_Commands = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.comboboxCommand = new System.Windows.Forms.ComboBox();
			this.buttonTestCrash = new System.Windows.Forms.Button();
			this.labelRecoveryAndRestartSafe = new System.Windows.Forms.Label();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
			this.notifyIcon1.Text = "Press Ctrl + Q";
			this.notifyIcon1.Visible = true;
			this.notifyIcon1.BalloonTipClicked += new System.EventHandler(this.notifyIcon1_BalloonTipClicked);
			this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
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
			this.textBox_Messages.Size = new System.Drawing.Size(652, 69);
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
			this.label2.Location = new System.Drawing.Point(553, 158);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(132, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Right-click minimizes to tray";
			// 
			// contextMenu_TrayIcon
			// 
			this.contextMenu_TrayIcon.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_Exit,
            this.menuItem_Commands,
            this.menuItem1,
            this.menuItem3,
            this.menuItem4,
            this.menuItem5,
            this.menuItem6});
			this.contextMenu_TrayIcon.Popup += new System.EventHandler(this.contextMenu_TrayIcon_Popup);
			// 
			// menuItem_Exit
			// 
			this.menuItem_Exit.Index = 0;
			this.menuItem_Exit.Text = "E&xit";
			this.menuItem_Exit.Click += new System.EventHandler(this.menuItem_Exit_Click);
			// 
			// menuItem_Commands
			// 
			this.menuItem_Commands.DefaultItem = true;
			this.menuItem_Commands.Index = 1;
			this.menuItem_Commands.Text = "&Commands";
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2});
			this.menuItem1.Text = "Test";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "Test commandform";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 3;
			this.menuItem3.Text = "&WebResults";
			this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 4;
			this.menuItem4.Text = "Show &ten notifications";
			this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 5;
			this.menuItem5.Text = "Create &plugin";
			this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
			// 
			// comboboxCommand
			// 
			this.comboboxCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboboxCommand.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.comboboxCommand.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.comboboxCommand.FormattingEnabled = true;
			this.comboboxCommand.Items.AddRange(new object[] {
            "Edit1",
            "Edit2",
            "Edit3",
            "Francois"});
			this.comboboxCommand.Location = new System.Drawing.Point(12, 129);
			this.comboboxCommand.Name = "comboboxCommand";
			this.comboboxCommand.Size = new System.Drawing.Size(508, 32);
			this.comboboxCommand.TabIndex = 4;
			this.comboboxCommand.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			this.comboboxCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
			this.comboboxCommand.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form1_PreviewKeyDown);
			// 
			// buttonTestCrash
			// 
			this.buttonTestCrash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonTestCrash.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonTestCrash.Location = new System.Drawing.Point(589, 10);
			this.buttonTestCrash.Name = "buttonTestCrash";
			this.buttonTestCrash.Size = new System.Drawing.Size(75, 23);
			this.buttonTestCrash.TabIndex = 5;
			this.buttonTestCrash.Text = "Test crash";
			this.buttonTestCrash.UseVisualStyleBackColor = true;
			this.buttonTestCrash.Visible = false;
			this.buttonTestCrash.Click += new System.EventHandler(this.button1_Click);
			// 
			// labelRecoveryAndRestartSafe
			// 
			this.labelRecoveryAndRestartSafe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelRecoveryAndRestartSafe.AutoSize = true;
			this.labelRecoveryAndRestartSafe.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelRecoveryAndRestartSafe.ForeColor = System.Drawing.Color.Green;
			this.labelRecoveryAndRestartSafe.Location = new System.Drawing.Point(553, 144);
			this.labelRecoveryAndRestartSafe.Name = "labelRecoveryAndRestartSafe";
			this.labelRecoveryAndRestartSafe.Size = new System.Drawing.Size(133, 13);
			this.labelRecoveryAndRestartSafe.TabIndex = 6;
			this.labelRecoveryAndRestartSafe.Text = "Recovery and Restart Safe";
			this.labelRecoveryAndRestartSafe.Visible = false;
			// 
			// progressBar1
			// 
			this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar1.Location = new System.Drawing.Point(556, 126);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(118, 15);
			this.progressBar1.TabIndex = 7;
			this.progressBar1.Visible = false;
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 6;
			this.menuItem6.Text = "Train &face detection";
			this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(51)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(686, 173);
			this.ControlBox = false;
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.labelRecoveryAndRestartSafe);
			this.Controls.Add(this.buttonTestCrash);
			this.Controls.Add(this.comboboxCommand);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox_Messages);
			this.Controls.Add(this.label1);
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
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Shown += new System.EventHandler(this.Form1_Shown);
			this.VisibleChanged += new System.EventHandler(this.Form1_VisibleChanged);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
			this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form1_PreviewKeyDown);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

				public System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Label label1;
				public System.Windows.Forms.TextBox textBox_Messages;
				private System.Windows.Forms.Label label2;
				private System.Windows.Forms.ContextMenu contextMenu_TrayIcon;
				private System.Windows.Forms.MenuItem menuItem_Exit;
				private System.Windows.Forms.MenuItem menuItem_Commands;
				private System.Windows.Forms.MenuItem menuItem1;
				private System.Windows.Forms.MenuItem menuItem2;
				public System.Windows.Forms.ComboBox comboboxCommand;
				private System.Windows.Forms.Button buttonTestCrash;
				private System.Windows.Forms.Label labelRecoveryAndRestartSafe;
				private System.Windows.Forms.ProgressBar progressBar1;
				private System.Windows.Forms.MenuItem menuItem3;
				private System.Windows.Forms.MenuItem menuItem4;
				private System.Windows.Forms.MenuItem menuItem5;
				private System.Windows.Forms.MenuItem menuItem6;
    }
}

