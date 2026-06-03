using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace FT_ProgramWPF.Elements
{
	public class RichTextBoxBehavior
	{
		// Map to associate the Collection with the specific RichTextBox instance
		private static readonly Dictionary<ObservableCollection<string>, RichTextBox> _map
			= new Dictionary<ObservableCollection<string>, RichTextBox>();

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.RegisterAttached(
				"ItemsSource",
				typeof(ObservableCollection<string>),
				typeof(RichTextBoxBehavior),
				new PropertyMetadata(null, OnItemsSourceChanged));

		public static void SetItemsSource(DependencyObject element, ObservableCollection<string> value) =>
			element.SetValue(ItemsSourceProperty, value);

		public static ObservableCollection<string> GetItemsSource(DependencyObject element) =>
			(ObservableCollection<string>)element.GetValue(ItemsSourceProperty);

		private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is RichTextBox richTextBox)
			{
				// Cleanup old collection
				if (e.OldValue is ObservableCollection<string> oldCol)
				{
					oldCol.CollectionChanged -= CollectionChanged;
					_map.Remove(oldCol);
				}

				// Setup new collection
				if (e.NewValue is ObservableCollection<string> newCol)
				{
					_map[newCol] = richTextBox;
					newCol.CollectionChanged += CollectionChanged;

					// Initial load
					richTextBox.Document.Blocks.Clear();
					foreach (var item in newCol)
					{
						// custom
						richTextBox.Document.Blocks.Add(new Paragraph(new Run(item))
						{
							Margin = new Thickness(0),
							FontSize = 18
						});
					}
				}
			}
		}

		private static void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (sender is ObservableCollection<string> collection &&
				_map.TryGetValue(collection, out RichTextBox rtb))
			{
				if (e.Action == NotifyCollectionChangedAction.Add)
				{
					foreach (string item in e.NewItems)
					{
						// custom
						rtb.Document.Blocks.Add(new Paragraph(new Run(item))
						{
							Margin = new Thickness(0),
							FontSize = 18
						});
					}
					rtb.ScrollToEnd(); // Auto-scroll
				}
				else if (e.Action == NotifyCollectionChangedAction.Reset)
				{
					rtb.Document.Blocks.Clear();
				}
			}
		}
	}
}
