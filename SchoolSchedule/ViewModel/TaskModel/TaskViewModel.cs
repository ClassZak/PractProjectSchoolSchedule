﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SchoolSchedule.ViewModel.TaskModel
{
	internal class TaskViewModel : ABaseViewModel
	{
		static readonly Dictionary<ETaskStatus, string> TASK_STATUS_DICIONARY = new Dictionary<ETaskStatus, string>
		{
			{ ETaskStatus.Unknown,"" },
			{ ETaskStatus.Completed,"Выполнена" },
			{ ETaskStatus.InProgress,"В процессе" },
			{ ETaskStatus.Failed,"Завершена с ошибкой" },
		};


		public ETaskStatus _taskStatus=ETaskStatus.Completed;
		public ETaskStatus ETaskStatus{ get { return _taskStatus; } set { SetPropertyChanged(ref _taskStatus, value); } }
		public void SetTaskStatus(ETaskStatus taskStatus)
		{
			this._taskStatus = taskStatus;
			if (taskStatus != ETaskStatus.Failed)
				ErrorMessage = string.Empty;
		}
		public string TaskStatus
		{ 
			get { return TASK_STATUS_DICIONARY[_taskStatus]; } 
			set 
			{
				for (int i = 1; i < TASK_STATUS_DICIONARY.Count; ++i)
					if (value == TASK_STATUS_DICIONARY.ElementAt(i).Value)
						_taskStatus = TASK_STATUS_DICIONARY.ElementAt(i).Key;
			} 
		}



		string _taskName;
		public string TaskName{ get { return _taskName; } set { SetPropertyChanged(ref _taskName, value); } }



		public string _errorMessage = null;
		public string ErrorMessage{ get { return _errorMessage; } set { SetPropertyChanged(ref _errorMessage, value); } }




		public TaskViewModel() { }
		public TaskViewModel(string taskName, ETaskStatus taskStatus) : this()
		{
			TaskName = taskName;
			_taskStatus = taskStatus;
		}
	}
}
