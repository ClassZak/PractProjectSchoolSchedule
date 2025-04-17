using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace SchoolSchedule.View
{
	public static class ListViewBehavior
	{
		public static readonly DependencyProperty SelectedItemsProperty =
			DependencyProperty.RegisterAttached("SelectedItems", typeof(IList), typeof(ListViewBehavior),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsChanged));

		public static IList GetSelectedItems(DependencyObject obj) => (IList)obj.GetValue(SelectedItemsProperty);
		public static void SetSelectedItems(DependencyObject obj, IList value) => obj.SetValue(SelectedItemsProperty, value);

		private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ListView listView)
			{
				listView.SelectionChanged -= ListView_SelectionChanged;

				if (e.NewValue != null)
				{
					listView.SelectionChanged += ListView_SelectionChanged;

					// Инициализация при первом присоединении
					if (e.NewValue is IList collection && listView.SelectedItems.Count == 0)
					{
						foreach (var item in collection)
							listView.SelectedItems.Add(item);
					}
				}
			}
		}

		private static void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var listView = (ListView)sender;
			var selectedItems = GetSelectedItems(listView);
			if (selectedItems == null) return;

			try
			{
				// Удаляем старые элементы
				foreach (var item in e.RemovedItems)
					if (selectedItems.Contains(item))
						selectedItems.Remove(item);

				// Добавляем новые элементы
				foreach (var item in e.AddedItems)
					if (!selectedItems.Contains(item))
						selectedItems.Add(item);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error syncing SelectedItems: {ex.Message}");
			}
		}
	}
}
