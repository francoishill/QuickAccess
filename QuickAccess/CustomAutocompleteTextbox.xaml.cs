using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuickAccess
{
	/// <summary>
	/// Interaction logic for CustomAutocompleteTextbox.xaml
	/// </summary>
	public partial class CustomAutocompleteTextbox : UserControl, INotifyPropertyChanged
	{
		public event RoutedEventHandler SelectionChanged;
		private bool EventAddedSelectionChanged = false;

		public CustomAutocompleteTextbox()
		{
			InitializeComponent();
			if (!EventAddedSelectionChanged)
			{ myACB.SelectionChanged += delegate { if (SelectionChanged != null) SelectionChanged(this, new RoutedEventArgs()); }; }
		}

		public bool IsDropDownOpen { get { return myACB.IsDropDownOpen; } set { myACB.IsDropDownOpen = value; } }
		public object SelectedItem { get { return myACB.SelectedItem; } set { myACB.SelectedItem = value; } }
		
		public static DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(CustomAutocompleteTextbox));
		public IEnumerable ItemsSource
		{
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); NotifyPropertyChanged("ItemsSource"); }
		}

		public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CustomAutocompleteTextbox));
		public string Text { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); NotifyPropertyChanged("Text"); } }

		private void ClearTextboxTextButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			myACB.SelectedItem = null;
			myACB.Text = "";
			myACB.Focus();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(info));
		}
	}
}
