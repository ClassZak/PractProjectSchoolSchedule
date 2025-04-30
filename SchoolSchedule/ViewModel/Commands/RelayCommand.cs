using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SchoolSchedule.ViewModel.Commands
{
	public class RelayCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;
		private Action<object> _execute;
		private Func<object, bool> _canExecute;

		public RelayCommand(Action<object> action)
		{
			_execute = action;
		}
		public RelayCommand(Action<object> execute, Func<object, bool> canExecute) : this(execute)
		{
			_canExecute = canExecute;
		}


		public bool CanExecute(object parameter)
		{
			return _execute !=null && (parameter == null || _canExecute==null || _canExecute(parameter));
		}

		public void Execute(object parameter)
		{
			if(CanExecute(parameter))
				_execute(parameter);
		}
	}
}
