namespace InlineCommandToolkit
{
	partial class EnterStringAndListOfEnumsForm
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
			this.textBoxString = new System.Windows.Forms.TextBox();
			this.comboBoxEnumPickerToAdd = new System.Windows.Forms.ComboBox();
			this.buttonAccept = new System.Windows.Forms.Button();
			this.treeViewListOfEnums = new System.Windows.Forms.TreeView();
			this.buttonAddSelectedEnum = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelMessage = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textBoxString
			// 
			this.textBoxString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxString.Location = new System.Drawing.Point(13, 74);
			this.textBoxString.Name = "textBoxString";
			this.textBoxString.Size = new System.Drawing.Size(313, 20);
			this.textBoxString.TabIndex = 0;
			// 
			// comboBoxEnumPickerToAdd
			// 
			this.comboBoxEnumPickerToAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxEnumPickerToAdd.FormattingEnabled = true;
			this.comboBoxEnumPickerToAdd.Location = new System.Drawing.Point(12, 126);
			this.comboBoxEnumPickerToAdd.Name = "comboBoxEnumPickerToAdd";
			this.comboBoxEnumPickerToAdd.Size = new System.Drawing.Size(254, 21);
			this.comboBoxEnumPickerToAdd.TabIndex = 1;
			// 
			// buttonAccept
			// 
			this.buttonAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAccept.Location = new System.Drawing.Point(11, 291);
			this.buttonAccept.Name = "buttonAccept";
			this.buttonAccept.Size = new System.Drawing.Size(49, 23);
			this.buttonAccept.TabIndex = 3;
			this.buttonAccept.Text = "A&ccept";
			this.buttonAccept.UseVisualStyleBackColor = true;
			this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
			// 
			// treeViewListOfEnums
			// 
			this.treeViewListOfEnums.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.treeViewListOfEnums.HideSelection = false;
			this.treeViewListOfEnums.Indent = 15;
			this.treeViewListOfEnums.Location = new System.Drawing.Point(11, 153);
			this.treeViewListOfEnums.Name = "treeViewListOfEnums";
			this.treeViewListOfEnums.ShowLines = false;
			this.treeViewListOfEnums.ShowPlusMinus = false;
			this.treeViewListOfEnums.ShowRootLines = false;
			this.treeViewListOfEnums.Size = new System.Drawing.Size(254, 119);
			this.treeViewListOfEnums.TabIndex = 4;
			this.treeViewListOfEnums.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeViewListOfEnums_KeyDown);
			// 
			// buttonAddSelectedEnum
			// 
			this.buttonAddSelectedEnum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAddSelectedEnum.Location = new System.Drawing.Point(272, 126);
			this.buttonAddSelectedEnum.Name = "buttonAddSelectedEnum";
			this.buttonAddSelectedEnum.Size = new System.Drawing.Size(35, 23);
			this.buttonAddSelectedEnum.TabIndex = 5;
			this.buttonAddSelectedEnum.Text = "A&dd";
			this.buttonAddSelectedEnum.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.buttonAddSelectedEnum.UseVisualStyleBackColor = true;
			this.buttonAddSelectedEnum.Click += new System.EventHandler(this.buttonAddSelectedEnum_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.Color.Gray;
			this.label1.Location = new System.Drawing.Point(10, 52);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Enter string here:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.Color.Gray;
			this.label2.Location = new System.Drawing.Point(13, 107);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(128, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Add to list or leave empty:";
			// 
			// labelMessage
			// 
			this.labelMessage.AutoEllipsis = true;
			this.labelMessage.AutoSize = true;
			this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelMessage.ForeColor = System.Drawing.Color.Green;
			this.labelMessage.Location = new System.Drawing.Point(11, 13);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(65, 17);
			this.labelMessage.TabIndex = 8;
			this.labelMessage.Text = "Message";
			// 
			// EnterStringAndListOfEnumsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(353, 326);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonAddSelectedEnum);
			this.Controls.Add(this.treeViewListOfEnums);
			this.Controls.Add(this.buttonAccept);
			this.Controls.Add(this.comboBoxEnumPickerToAdd);
			this.Controls.Add(this.textBoxString);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EnterStringAndListOfEnumsForm";
			this.Text = "String and item list";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxString;
		private System.Windows.Forms.Button buttonAccept;
		private System.Windows.Forms.TreeView treeViewListOfEnums;
		private System.Windows.Forms.Button buttonAddSelectedEnum;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.ComboBox comboBoxEnumPickerToAdd;
		public System.Windows.Forms.Label labelMessage;
	}
}