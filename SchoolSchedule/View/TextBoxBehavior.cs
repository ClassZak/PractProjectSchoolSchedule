using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace SchoolSchedule.View
{
	public static class TextBoxBehavior
	{
		public static bool GetMoveCursorToEndOnFocus(DependencyObject obj)
		{
			return (bool)obj.GetValue(MoveCursorToEndOnFocusProperty);
		}

		public static void SetMoveCursorToEndOnFocus(DependencyObject obj, bool value)
		{
			obj.SetValue(MoveCursorToEndOnFocusProperty, value);
		}

		public static readonly DependencyProperty MoveCursorToEndOnFocusProperty =
		DependencyProperty.RegisterAttached
		(
			"MoveCursorToEndOnFocus",
			typeof(bool),
			typeof(TextBoxBehavior),
			new PropertyMetadata(false, OnMoveCursorToEndOnFocusChanged)
		);

		private static void OnMoveCursorToEndOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TextBox textBox)
				textBox.GotFocus += (sender, args) =>
				{
					textBox.CaretIndex = textBox.Text.Length;
				};
		}
	}
}
