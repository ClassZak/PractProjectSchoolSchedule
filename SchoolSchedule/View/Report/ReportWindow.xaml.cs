using SchoolSchedule.Model;
using SchoolSchedule.ViewModel;
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
using System.Windows.Shapes;

namespace SchoolSchedule.View.Report
{
	/// <summary>
	/// Логика взаимодействия для ReportWindow.xaml
	/// </summary>
	public partial class ReportWindow : Window
	{
		public ReportWindow()
		{
			InitializeComponent();
		}
		public ReportWindow(MainViewModel mainViewModel) : this()
		{
			DataContext=new ViewModel.Report.ReportMainViewModel(mainViewModel);
		}
		public ReportWindow(List<Group> groups, List<Teacher> teachers, List<BellScheduleType> bellScheduleTypes) : this()
		{
			DataContext=new ViewModel.Report.ReportMainViewModel(groups, teachers, bellScheduleTypes);
		}
	}
}
