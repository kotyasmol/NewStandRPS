using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace NewStandRPS.ViewModels
{
    public class LogEntry
    {
        public string Message { get; set; }
        public SolidColorBrush Color { get; set; }
    }

    public class Logger
    {
        private ObservableCollection<LogEntry> _logMessages;
        private string _logFilePath;
        private bool _useDarkTheme = false; // Переключение темы 

        public Logger(ObservableCollection<LogEntry> logMessages, string logFilePath)
        {
            _logMessages = logMessages ?? new ObservableCollection<LogEntry>();
            _logFilePath = logFilePath;
        }

        public void Log(string message, LogLevel level)
        {
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            string logEntry = $"{timestamp} [{level}] {message}";

            // Запись в файл
            WriteLogToFile(logEntry);

            // Добавление лога в коллекцию с цветом
            SolidColorBrush color = GetLogColor(level);
            Application.Current.Dispatcher.Invoke(() =>
            {
                _logMessages.Add(new LogEntry
                {
                    Message = logEntry,
                    Color = color
                });
            });
        }

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

        private SolidColorBrush GetLogColor(LogLevel level)
        {
            if (_useDarkTheme)
            {
                return level switch
                {
                    LogLevel.Error => new SolidColorBrush(Colors.Red),   
                    LogLevel.Info => new SolidColorBrush(Colors.Black),  
                    LogLevel.Warning => new SolidColorBrush(Colors.Orange), 
                    LogLevel.Debug => new SolidColorBrush(Colors.Gray),  
                    LogLevel.Critical => new SolidColorBrush(Colors.DarkRed), 
                    LogLevel.Success => new SolidColorBrush(Colors.Green),  
                    _ => new SolidColorBrush(Colors.Black),           // По умолчанию черный
                };
            }
            else
            {
                return level switch
                {
                    LogLevel.Error => new SolidColorBrush(Colors.Red),   
                    LogLevel.Info => new SolidColorBrush(Colors.White),  
                    LogLevel.Warning => new SolidColorBrush(Colors.Yellow), 
                    LogLevel.Debug => new SolidColorBrush(Colors.White), 
                    LogLevel.Critical => new SolidColorBrush(Colors.DarkRed), 
                    LogLevel.Success => new SolidColorBrush(Colors.Lime), 
                    _ => new SolidColorBrush(Colors.White),            // По умолчанию белый для темной темы
                };
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
}
