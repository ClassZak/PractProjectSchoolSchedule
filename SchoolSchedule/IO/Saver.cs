using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using System.Windows;

namespace SchoolSchedule.IO
{
	public static class Saver
	{
		public static async Task SaveToFileAsync<T>
		(
			IEnumerable<T> collection,
			string filePath,
			string delimiter = "\t"
		)
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection));

			if (string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException("Пустой путь файла", nameof(filePath));

			var properties = typeof(T)
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => p.CanRead)
				.ToArray();

			if (properties.Length == 0)
			{
				throw new InvalidOperationException(
					"У класса нет свойств для сохранения " + typeof(T).Name);
			}
			try
			{
				using (var writer = new StreamWriter(filePath))
				{
					//// Запись заголовков
					//var headers = string.Join(delimiter, properties.Select(p => p.Name));
					//await writer.WriteLineAsync(headers).ConfigureAwait(false);

					// Запись данных
					foreach (var item in collection)
					{
						var values = properties.Select(p => p.GetValue(item)?.ToString() ?? string.Empty);

						var line = string.Join(delimiter, values);
						await writer.WriteLineAsync(line).ConfigureAwait(false);
					}
				}
			}
			catch (ArgumentException ex)
			{
				MessageBox.Show($"Некорректный путь: {ex.Message}", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Stop);
			}
			catch (PathTooLongException ex)
			{
				MessageBox.Show($"Путь слишком длинный: {ex.Message}", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Stop);
			}
			catch (DirectoryNotFoundException ex)
			{
				MessageBox.Show($"Директория не найдена: {ex.Message}", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Stop);
			}
			catch (UnauthorizedAccessException ex)
			{
				MessageBox.Show($"Доступ запрещен: {ex.Message}", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Stop);
			}
			catch (NotSupportedException ex)
			{
				MessageBox.Show($"Неподдерживаемый путь: {ex.Message}", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Stop);
			}
			catch (IOException ex)
			{
				MessageBox.Show($"Ошибка ввода-вывода: {ex.Message}", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Stop);
			}
			catch (SecurityException ex)
			{
				MessageBox.Show($"Ошибка безопасности: {ex.Message}", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Stop);
			}
		}
	}
}
