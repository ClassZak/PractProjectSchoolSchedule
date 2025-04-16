using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SchoolSchedule.View.Edit.EditPage
{
	public interface IEditPage<T> where T : class, new()
	{
		/// <summary>
		///	Проверяет, есть ли ошибка ввода и если есть, возвращает её строку
		/// </summary>
		/// <returns></returns>
		KeyValuePair<bool,string> CheckInputRules();
	}
}
