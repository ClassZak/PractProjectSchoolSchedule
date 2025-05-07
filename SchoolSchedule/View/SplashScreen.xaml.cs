using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
	/// Логика взаимодействия для SplashScreen.xaml
	/// </summary>
	public partial class SplashScreen : Window
	{
		public SplashScreen()
		{
			InitializeComponent();

			Thread thread = new Thread(() =>
			{
				Thread.Sleep(1000);
				App.Current.Dispatcher.Invoke(new Action(() => { Close(); }));
			});
			thread.Start();
		}
	}
}
