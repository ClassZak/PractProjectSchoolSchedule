﻿using SchoolSchedule.Model.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SchoolSchedule.View
{
	/// <summary>
	/// Логика взаимодействия для GroupAddWindow.xaml
	/// </summary>
	public partial class SubjectAddWindow : Window
	{
		private SubjectAddWindow()
		{
			InitializeComponent();
		}

		public bool dialogResult = false;

		public Model.Subject NewGroup { get; set; }

		private List<DTOSubject> _subjects=new List<DTOSubject>();
		public SubjectAddWindow(ICollection<DTOSubject> list):this()
		{
			foreach(var el in list)
				_subjects.Add(el);
		}

		private void Button_Click_Cancel(object sender, RoutedEventArgs e)
		{
			dialogResult = false;

			this.Close();
		}
		private void Button_Click_Add(object sender, RoutedEventArgs e)
		{
			byte year;
			bool parsed= byte.TryParse(YearTextBox.Text, out year);
			if (!parsed)
			{
				MessageBox.Show("Неудалось получить число при вводе", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (year<1 || year >11)
			{
				MessageBox.Show("Год обучения должен быть от 1 до 11 включительно", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			string nameText=GroupTextBox.Text.ToUpper();
			if(string.IsNullOrEmpty(nameText))
			{
				MessageBox.Show("Введите букву для группы", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if(nameText.Length!=1)
			{
				MessageBox.Show("Введите только одну букву для группы", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if(nameText[0]<'А' ||  nameText[0]>'Е')
			{
				MessageBox.Show("Введите только одну букву для группы от \'А до \'Е", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			NewGroup = new Model.Subject
			{
				Name = nameText
			};
			

			dialogResult = true;
			Close();
		}
		private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Разрешить byte
			var textBox = (TextBox)sender;
			string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
			e.Handled = !byte.TryParse(newText, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
		}
		private void RussianTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Разрешить только русские буквы, пробелы и дефис
			var regex = new Regex(@"^[\p{IsCyrillic}\s\-]+$");
			e.Handled = !regex.IsMatch(e.Text);
		}
	}
}
