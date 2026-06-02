using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FT_ProgramWPF.Model
{
	public static class AutoScrollBehavior
	{
		public static readonly DependencyProperty AutoScrollProperty =
			DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(AutoScrollBehavior), new PropertyMetadata(false, OnAutoScrollChanged));

		public static void SetAutoScroll(DependencyObject obj, bool value) => obj.SetValue(AutoScrollProperty, value);
		public static bool GetAutoScroll(DependencyObject obj) => (bool)obj.GetValue(AutoScrollProperty);

		private static void OnAutoScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ItemsControl itemsControl && (bool)e.NewValue)
			{
				itemsControl.Loaded += (s, args) =>
				{
					// Ensure we are listening to the correct collection
					if (itemsControl.ItemsSource is INotifyCollectionChanged collection)
					{
						collection.CollectionChanged += (sender, eventArgs) =>
						{
							if (eventArgs.Action == NotifyCollectionChangedAction.Add && itemsControl.Items.Count > 0)
							{
								// Critical: Use Dispatcher to wait for UI rendering
								itemsControl.Dispatcher.BeginInvoke(new Action(() =>
								{
									var lastItem = itemsControl.Items[^1];
									//itemsControl.ScrollIntoView(lastItem);
									itemsControl.BringIntoView();
								}), System.Windows.Threading.DispatcherPriority.Loaded);
							}
						};
					}
				};
			}
		}
	}
}
