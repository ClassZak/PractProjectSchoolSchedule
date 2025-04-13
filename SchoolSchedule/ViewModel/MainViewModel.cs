﻿using SchoolSchedule.ViewModel.Comands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SchoolSchedule.ViewModel
{
	public class MainViewModel : ABaseViewModel
	{
		bool [] _changedTables=new bool [8];
		public MainViewModel()
		{
			_groups			= new ObservableCollection<Model.Group>			(new List<Model.Group>			());
			_lessons		= new ObservableCollection<Model.DTO.DTOLesson>		(new List<Model.DTO.DTOLesson>			());
			_schedules		= new ObservableCollection<Model.DTO.DTOSchedule>		(new List<Model.DTO.DTOSchedule>		());
			_students		= new ObservableCollection<Model.DTO.DTOStudent>		(new List<Model.DTO.DTOStudent>		());
			_subjects		= new ObservableCollection<Model.Subject>		(new List<Model.Subject>		());
			_teachers		= new ObservableCollection<Model.DTO.DTOTeacher>		(new List<Model.DTO.DTOTeacher>		());
			_teacherPhones	= new ObservableCollection<Model.DTO.DTOTeacherPhone>	(new List<Model.DTO.DTOTeacherPhone>	());
			LoadData();
		}

		private void LoadData() 
		{
			try
			{
				using (var dataBase = new Model.SchoolScheduleEntities())
				{
					App.Current.Dispatcher.Invoke(() =>
					{
						_groups.Clear();
						_lessons.Clear();
						_schedules.Clear();
						_students.Clear();
						_subjects.Clear();
						_teachers.Clear();
						_teacherPhones.Clear();

						_classesTeachers.Clear();
						_specialityTeachers.Clear();
					});

					foreach (var el in dataBase.SpecialityTeacher.ToList())
						App.Current.Dispatcher.Invoke(() => { _specialityTeachers.Add(el); });
					foreach (var el in dataBase.ClassTeacher.ToList())
						App.Current.Dispatcher.Invoke(() => { _classesTeachers.Add(el); });

					foreach (var el in dataBase.Group.ToList())
						App.Current.Dispatcher.Invoke(() => { Groups.Add(el); });
					foreach (var el in dataBase.Lesson.ToList())
						App.Current.Dispatcher.Invoke(() => { Lessons.Add(new Model.DTO.DTOLesson (el)); });
					foreach (var el in dataBase.Student.ToList())
						App.Current.Dispatcher.Invoke(() => { Students.Add(new Model.DTO.DTOStudent(el)); });
					foreach (var el in dataBase.Subject.ToList())
						App.Current.Dispatcher.Invoke(() => { Subjects.Add(el); });
					foreach (var el in dataBase.Teacher.ToList())
						App.Current.Dispatcher.Invoke(() => { Teachers.Add(new Model.DTO.DTOTeacher(el)); });
					foreach (var el in dataBase.TeacherPhone.ToList())
						App.Current.Dispatcher.Invoke(() => { TeacherPhones.Add(new Model.DTO.DTOTeacherPhone(el)); });
					foreach (var el in dataBase.Schedule.ToList())
						App.Current.Dispatcher.Invoke(() => { Schedules.Add(new Model.DTO.DTOSchedule(el)); });
				}
			}
			// Для того, чтобы не было ошибки в xaml
			catch (System.InvalidOperationException)
			{ }
			catch (System.Data.EntityException ex)
			{
				Task.Run(() =>
				{ 
					MessageBox.Show
					(
						ex.InnerException != null ? ex.InnerException.Message : ex.Message, 
						"Ошибка базы данных",
						MessageBoxButton.OK,
						MessageBoxImage.Stop
					);
				});
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message,"Ошибка базы данных",MessageBoxButton.OK,MessageBoxImage.Stop);
			}
		}
		private async void LoadDataAsync()
		{
			await Task.Run(() =>
			{
				LoadData();
			});
		}

		protected void UpdateList<T>(ObservableCollection<T> collection) where T : class
		{
			try
			{
				using (var dataBase = new Model.SchoolScheduleEntities())
				{
					List<T> elements = dataBase.Set<T>().ToList();

					App.Current.Dispatcher.Invoke(() => { collection.Clear(); });
					foreach (var el in elements)
						App.Current.Dispatcher.Invoke(() => { collection.Add(el); });
				}
			}
			catch(Exception ex)
			{	
				MessageBox.Show(ex.Message,"Ошибка базы данных",MessageBoxButton.OK,MessageBoxImage.Stop);
			}
		}

		protected FieldInfo FindTargetField(Type targetType)
		{
			FieldInfo field = null;
			foreach(var type in this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
				if(IsObservableCollection(type.FieldType,out Type elementType) && elementType==targetType)
				{
					field = type;
					break;
				}

			return field;
		}

		// Тип аргумента: Type
		RelayCommand _updateData;
		public RelayCommand UpdateDataAsync
		{
			get
			{
				return _updateData ??
				(_updateData = new RelayCommand
				(
					param =>
					{
						if (!(param is Type targetType))
						{
							LoadDataAsync();
							return;
						}

						Task.Run(() =>
						{
							FieldInfo targetField = FindTargetField(targetType);
							if (targetField == null)
								throw new ArgumentException("Коллекция для указанного типа не найдена");

							var collection = (IList)targetField.GetValue(this);
							var updateMethod =
							GetType().GetMethod
							(
								nameof(UpdateList), BindingFlags.NonPublic | BindingFlags.Instance
							);
							var genericMethod = updateMethod.MakeGenericMethod(targetType);
							genericMethod.Invoke(this, new object[] { collection });
						});
					}
				));
			}
		}
		private RelayCommand _deleteCommand;
		public RelayCommand DeleteCommand
		{
			get
			{
				return _deleteCommand ??
				(_deleteCommand = new RelayCommand
				(
					param =>
					{
						DataGrid dataGridRef = param as DataGrid;
						IEnumerable collectionRef = dataGridRef.ItemsSource;
						if (dataGridRef == null)
							throw new ArgumentException("Неверный аргумент для обновления таблицы с удаляемыми значениями");

						Task.Run(() =>
						{
							if (!IsObservableCollection(collectionRef.GetType(), out Type paramType))
								throw new ArgumentException("Неверный тип аргумента для обновления таблицы с удаляемыми значениями");

							FieldInfo targetField = FindTargetField(paramType);

							if (targetField == null)
								throw new ArgumentException("Не найдена коллекция для обновления");

							List<object> selectedItems = new List<object>();
							App.Current.Dispatcher.Invoke(() => 
							{ 
								foreach(var el in dataGridRef.SelectedItems)
									selectedItems.Add(el);
							});

							var collection = (IList)(targetField.GetValue(this));

							foreach (var el in selectedItems)
								App.Current.Dispatcher.Invoke(() =>{ collection.Remove(el); });
						});
					}
				));
			}
		}

		/// <summary>
		/// Проверка на принадлежность шаблону ObservableCollection<T>, и возвращает T тип из шаблона
		/// </summary>
		/// <param name="type"></param>
		/// <param name="elementType"></param>
		/// <returns></returns>
		protected bool IsObservableCollection(Type type, out Type elementType)
		{
			elementType = null;

			if (type.IsGenericType &&
				type.GetGenericTypeDefinition() == typeof(ObservableCollection<>))
			{
				elementType = type.GetGenericArguments()[0];
				return true;
			}

			return false;
		}


		private ObservableCollection<Model.Group> _groups;
		public ObservableCollection<Model.Group> Groups
		{ get {return _groups;}set {SetPropertyChanged(ref _groups, value);} }

		private ObservableCollection<Model.DTO.DTOLesson> _lessons;
		public ObservableCollection<Model.DTO.DTOLesson> Lessons
		{ get { return _lessons; } set { SetPropertyChanged(ref _lessons, value); } }

		private ObservableCollection<Model.DTO.DTOSchedule> _schedules;
		public ObservableCollection<Model.DTO.DTOSchedule> Schedules
		{ get { return _schedules; } set { SetPropertyChanged(ref _schedules, value); } }

		private ObservableCollection<Model.DTO.DTOStudent> _students;
		public ObservableCollection<Model.DTO.DTOStudent> Students
		{ get { return _students; } set { SetPropertyChanged(ref _students, value); } }

		private ObservableCollection<Model.Subject> _subjects;
		public ObservableCollection<Model.Subject> Subjects
		{ get { return _subjects; } set { SetPropertyChanged(ref _subjects, value); } }

		private ObservableCollection<Model.DTO.DTOTeacher> _teachers;
		public ObservableCollection<Model.DTO.DTOTeacher> Teachers
		{ get { return _teachers; } set { SetPropertyChanged(ref _teachers, value); } }

		private ObservableCollection<Model.DTO.DTOTeacherPhone> _teacherPhones;
		public ObservableCollection<Model.DTO.DTOTeacherPhone> TeacherPhones
		{ get { return _teacherPhones; } set { SetPropertyChanged(ref _teacherPhones, value); } }




		private List<Model.SpecialityTeacher> _specialityTeachers=new List<Model.SpecialityTeacher>();
		private List<Model.ClassTeacher> _classesTeachers=new List<Model.ClassTeacher>();
	}
}
