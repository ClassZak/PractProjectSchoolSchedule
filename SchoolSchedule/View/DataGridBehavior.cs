using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace SchoolSchedule.View
{
	public static class DataGridBehavior
	{
		public static readonly DependencyProperty SelectedItemsProperty =
			DependencyProperty.RegisterAttached("SelectedItems", typeof(IList), typeof(DataGridBehavior),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsChanged));

		public static IList GetSelectedItems(DependencyObject obj) => (IList)obj.GetValue(SelectedItemsProperty);
		public static void SetSelectedItems(DependencyObject obj, IList value) => obj.SetValue(SelectedItemsProperty, value);

		private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is DataGrid dataGrid)
			{
				dataGrid.SelectionChanged -= DataGrid_SelectionChanged;
				if (e.NewValue != null)
				{
					dataGrid.SelectionChanged += DataGrid_SelectionChanged;
				}
			}
		}

		private static void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var dataGrid = (DataGrid)sender;
			var selectedItems = GetSelectedItems(dataGrid);
			if (selectedItems == null) return;

			foreach (var item in e.RemovedItems)
				selectedItems.Remove(item);
			foreach (var item in e.AddedItems)
				if (!selectedItems.Contains(item))
					selectedItems.Add(item);
		}
	}
}
