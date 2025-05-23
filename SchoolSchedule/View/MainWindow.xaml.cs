﻿using SchoolSchedule.ViewModel;
using System;
using System.Collections.Generic;
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

namespace SchoolSchedule.View
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			// Для публикации
			//this.DataContext = new MainViewModel();
			(this.DataContext as MainViewModel).MainWindow = this;

			#if !DEBUG
			SplashScreen splashScreen = new SplashScreen();
			splashScreen.ShowDialog();
			#endif
		}

		private void CloseMenu_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
