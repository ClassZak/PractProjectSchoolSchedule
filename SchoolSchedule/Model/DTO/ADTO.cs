using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public abstract class ADTO<T> : INotifyDataErrorInfo
	where T : class, new()
	{
		private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public bool HasErrors => _errors.Any();

		public IEnumerable GetErrors(string propertyName)
		{
			if (_errors.ContainsKey(propertyName))
				return _errors[propertyName];
			else return new List<string>();
		}

		protected void AddError(string propertyName, string error)
		{
			if (!_errors.ContainsKey(propertyName))
				_errors[propertyName] = new List<string>();

			_errors[propertyName].Add(error);
			OnErrorsChanged(propertyName);
		}

		protected void ClearErrors(string propertyName)
		{
			_errors.Remove(propertyName);
			OnErrorsChanged(propertyName);
		}

		protected void OnErrorsChanged(string propertyName)
		{
			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
		}






		public T ModelRef { get; set; } = new T();
		public int DTOId { get; set; } = 0;
		protected abstract void LoadAllLabels(ref Model.SchoolScheduleEntities dataBase);

		public bool ExistsInDataBase()
		{
			return DTOId != 0;
		}
		public abstract bool HasReferenceOfNotExistingObject();
	}
}
