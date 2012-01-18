using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace InlineCommandToolkit
{
	public partial class EnterStringAndListOfEnumsForm : Form
	{
		public EnterStringAndListOfEnumsForm()
		{
			InitializeComponent();

			treeViewListOfEnums.HandleCreated += delegate
			{
				StylingInterop.SetTreeviewVistaStyle(treeViewListOfEnums);
			};
		}

		private void buttonAccept_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(textBoxString.Text))
				UserMessages.ShowWarningMessage("Please enter text into the textbox");
			else if (treeViewListOfEnums.Nodes.Count == 0 && !UserMessages.Confirm("Leave the list empty"))
			{ }
			else
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void buttonAddSelectedEnum_Click(object sender, EventArgs e)
		{
			if (comboBoxEnumPickerToAdd.SelectedIndex == -1)
			{
				UserMessages.ShowWarningMessage("Please select an item to add");
				return;
			}
			if (treeViewListOfEnums.Nodes.Count == 0 || treeViewListOfEnums.Nodes[treeViewListOfEnums.Nodes.Count - 1].Tag != comboBoxEnumPickerToAdd.SelectedItem)
				treeViewListOfEnums.Nodes.Add(new TreeNode(comboBoxEnumPickerToAdd.SelectedItem.ToString()) { Tag = comboBoxEnumPickerToAdd.SelectedItem });
			else
				UserMessages.ShowWarningMessage("Item to be added may not be the same as the last item in the list.");
		}

		private void treeViewListOfEnums_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
				if (treeViewListOfEnums.SelectedNode != null)
					treeViewListOfEnums.Nodes.Remove(treeViewListOfEnums.SelectedNode);
		}

		public static bool EnterStringAndListOfEnums<T>(string Message, out string OutString, out List<T> ListOfEnums, IWin32Window owner = null)
		{
			if (!(typeof(T).IsEnum))
			{
				UserMessages.ShowErrorMessage("Type of item is not an enum: " + typeof(T).Name + ", cannot perform EnterStringAndListOfEnums");
				OutString = null;
				ListOfEnums = null;
				return false;
			}
			EnterStringAndListOfEnumsForm tmpForm = new EnterStringAndListOfEnumsForm();
			tmpForm.labelMessage.Text = Message;
			foreach (T item in Enum.GetValues(typeof(T)))
				tmpForm.comboBoxEnumPickerToAdd.Items.Add(item);
			if (tmpForm.ShowDialog(owner) == DialogResult.OK)
			{
				OutString = tmpForm.textBoxString.Text;
				if (tmpForm.treeViewListOfEnums.Nodes.Count == 0)
					ListOfEnums = null;
				else
				{
					ListOfEnums = new List<T>();
					foreach (TreeNode node in tmpForm.treeViewListOfEnums.Nodes)
						ListOfEnums.Add((T)node.Tag);
				}
				return true;
			}
			else
			{
				OutString = null;
				ListOfEnums = null;
				return false;
			}
		}
	}
}
