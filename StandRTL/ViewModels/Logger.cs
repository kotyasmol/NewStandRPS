using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace NewStandRPS.ViewModels
{
    public class Logger
    {
        private ObservableCollection<string> _logMessages;
        private string _logFilePath;
        private bool _useDarkTheme = false;

        public Logger(ObservableCollection<string> logMessages, string logFilePath)
        {
            _logMessages = logMessages ?? new ObservableCollection<string>();  // Инициализация коллекции, если не передана
            _logFilePath = logFilePath;
        }

        // Метод для логирования с уровнями логирования
        public void Log(string message, LogLevel level)
        {
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            string logEntry = $"{timestamp} [{level}] {message}";

            // Запись в файл
            WriteLogToFile(logEntry);

            // Добавляем в GUI через Dispatcher.Invoke (для многопоточности)
            Application.Current.Dispatcher.Invoke(() =>
            {
                string coloredLogEntry = ApplyColorToLogEntry(logEntry, level);
                _logMessages.Add(coloredLogEntry);
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
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Ошибка записи лога: {ex.Message}");
                });
            }
        }

        // Применение цвета к логам в зависимости от уровня
        private string ApplyColorToLogEntry(string logEntry, LogLevel level)
        {
            string color = GetLogColor(level);
            return $"<span style=\"color:{color}\">{logEntry}</span>";
        }

        // Выбор цвета для логов
        private string GetLogColor(LogLevel level)
        {
            if (!_useDarkTheme)
            {
                return level switch
                {
                    LogLevel.Error => "#ff0000",   // Красный для ошибок
                    LogLevel.Info => "#000000",    // Черный для информации
                    LogLevel.Warning => "#FFA500", // Оранжевый для предупреждений
                    LogLevel.Debug => "#808080",   // Серый для отладки
                    LogLevel.Critical => "#FF0000",// Ярко-красный для критических ошибок
                    LogLevel.Success => "#008000", // Зеленый для успеха
                    _ => "#000000",                // Черный по умолчанию
                };
            }
            else
            {
                return level switch
                {
                    LogLevel.Error => "#ff0000",   // Красный для ошибок
                    LogLevel.Info => "#ffffff",    // Белый для информации в темной теме
                    LogLevel.Warning => "#FFFF00", // Желтый для предупреждений
                    LogLevel.Debug => "#ffffff",   // Белый для отладки в темной теме
                    LogLevel.Critical => "#FF0000",// Ярко-красный для критических ошибок
                    LogLevel.Success => "#00FF00", // Зеленый для успеха в темной теме
                    _ => "#ffffff",                // Белый по умолчанию для темной темы
                };
            }
        }
    }

    public enum LogLevel
    {
        Error,    // Ошибка
        Info,     // Информация
        Warning,  // Предупреждение
        Debug,    // Отладка
        Critical, // Критическая ошибка
        Success   // Успех
    }
}
