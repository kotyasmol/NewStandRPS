using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace NewStandRPS.ViewModels
{
    public class Logger
    {
        private ListView _logListView; // ListView для отображения логов
        private string _logFilePath = "log.txt"; // Путь к файлу лога
        private bool _useDarkTheme = false; // Переключение темы 

        public Logger(ListView logListView)
        {
            _logListView = logListView;
        }

        // Метод для логирования событий
        public void Log(string message, LogLevel level)
        {
            string color = GetLogColor(level);
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            string logEntry = $"{timestamp} {message}";

            // Запись в файл
            WriteLogToFile(logEntry);

            // Добавление логов в ListView
            Application.Current.Dispatcher.Invoke(() =>
            {
                AddLogToListView(logEntry, color, level == LogLevel.S); // Форматирование для ListView
            });
        }

        // Запись лога в файл
        private void WriteLogToFile(string logEntry)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(_logFilePath, true))
                {
                    sw.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка записи лога: {ex.Message}");
            }
        }

        // Добавление лога в ListView с цветом текста
        private void AddLogToListView(string logEntry, string color, bool isBold)
        {
            var listItem = new ListBoxItem
            {
                Content = logEntry,
                Foreground = (Brush)new BrushConverter().ConvertFromString(color),
                FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal
            };

            _logListView.Items.Add(listItem);

            // Автопрокрутка вниз
            if (_logListView.Items.Count > 0)
            {
                _logListView.ScrollIntoView(_logListView.Items[_logListView.Items.Count - 1]);
            }
        }

        // Выбор цвета логов в зависимости от уровня
        private string GetLogColor(LogLevel level)
        {
            if (!_useDarkTheme)
            {
                return level switch
                {
                    LogLevel.E => "#ffffff", // Белый
                    LogLevel.I => "#b4b4b4", // Серый
                    LogLevel.W => "#ffffff", // Белый
                    LogLevel.D => "#b4b4b4", // Серый
                    LogLevel.C => "#b4b4b4", // Серый
                    LogLevel.S => "#ffffff", // Белый (для жирного текста)
                    _ => "#b4b4b4",          // По умолчанию серый
                };
            }
            else
            {
                return level switch
                {
                    LogLevel.E => "#ff0000", // Красный
                    LogLevel.I => "#000000", // Черный
                    LogLevel.W => "#ff0000", // Красный
                    LogLevel.D => "#000000", // Черный
                    LogLevel.C => "#0000CD", // Синий
                    LogLevel.S => "#000000", // Черный (для жирного текста)
                    _ => "#000000",          // По умолчанию черный
                };
            }
        }
    }

    // Уровни логов
    public enum LogLevel
    {
        E, // Error
        I, // Info
        W, // Warning
        D, // Debug
        C, // Critical
        S  // Success
    }
}
