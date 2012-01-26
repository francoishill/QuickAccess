using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SharedClasses;

namespace QuickAccess
{
	/// <summary>
	/// Interaction logic for WebResultsWindow.xaml
	/// </summary>
	public partial class WebResultsWindow : Window
	{
		public WebResultsWindow()
		{
			InitializeComponent();

			webSourceControl1.LoadCompleted += delegate//.Navigated += delegate
			{
				listBox1.IsEnabled = true;
				if (listBox1.SelectedItem is TorrentSearchResult)
					(listBox1.SelectedItem as TorrentSearchResult).LoadingAnimationVisible = System.Windows.Visibility.Hidden;
			};
		}

		private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (listBox1.SelectedItem != null && listBox1.SelectedItem is TorrentSearchResult)
			{
				listBox1.IsEnabled = false;
				TorrentSearchResult result = (listBox1.SelectedItem as TorrentSearchResult);
				result.LoadingAnimationVisible = System.Windows.Visibility.Visible;
				try { webSourceControl1.LoadURL(result.Uri); }//.Navigate(result.Uri); }
				catch (Exception exc) { UserMessages.ShowErrorMessage(exc.Message); }
				
			}
		}

		private void textBoxSearchQuery_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				this.Effect = new BlurEffect() { Radius = 4 };
				this.IsEnabled = false;
				listBox1.ItemsSource = null;
				string searchQuery = textBoxSearchQuery.Text;
				ObservableCollection<TorrentSearchResult> results = ParseHtmlInterop.GetResultsForTorrentzSearch(searchQuery);
				if (results == null || results.Count == 0)
					UserMessages.ShowWarningMessage("No results for '" + searchQuery + "'");
				else listBox1.ItemsSource = results;
				this.Effect = null;
				this.IsEnabled = true;
			}
		}

		private void textBoxFilterResults_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (listBox1.ItemsSource == null || !(listBox1.ItemsSource is ObservableCollection<TorrentSearchResult>))
				return;
			foreach (TorrentSearchResult result in listBox1.ItemsSource as ObservableCollection<TorrentSearchResult>)
				if (string.IsNullOrWhiteSpace(textBoxFilterResults.Text))
					result.Visibility = System.Windows.Visibility.Visible;
				else if (!result.Name.ToLower().Contains(textBoxFilterResults.Text.ToLower()))
					result.Visibility = System.Windows.Visibility.Collapsed;
				else
					result.Visibility = System.Windows.Visibility.Visible;

			listBox1.UpdateLayout();
		}

		private void textBoxSearchQuery_TextChanged(object sender, TextChangedEventArgs e)
		{
			textBoxFilterResults.Clear();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			textBoxSearchQuery.Focus();
		}
	}
}
