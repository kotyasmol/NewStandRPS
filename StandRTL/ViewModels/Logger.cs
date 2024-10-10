using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Serilog;
using Serilog.Events;

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
        private bool _useDarkTheme = false; // Переключение темы 

        // Serilog Logger
        private Serilog.Core.Logger _serilogLogger;

        public Logger(ObservableCollection<LogEntry> logMessages, string logFilePath)
        {
            _logMessages = logMessages ?? new ObservableCollection<LogEntry>();

            // Инициализация Serilog
            _serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Минимальный уровень логирования
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7, fileSizeLimitBytes: 10_000_000) // Логирование в файл с ежедневной ротацией
                .CreateLogger();
        }

        public void Log(string message, LogLevel level)
        {
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            string logEntry = $"{timestamp}  {message}";

            switch (level)
            {
                case LogLevel.Error:
                    _serilogLogger.Error(message);
                    break;
                case LogLevel.Info:
                    _serilogLogger.Information(message);
                    break;
                case LogLevel.Warning:
                    _serilogLogger.Warning(message);
                    break;
                case LogLevel.Debug:
                    _serilogLogger.Debug(message);
                    break;
                case LogLevel.Critical:
                    _serilogLogger.Fatal(message);
                    break;
                case LogLevel.Success:
                    _serilogLogger.Information(message); // Serilog не имеет "Success", используем Information
                    break;
                default:
                    _serilogLogger.Information(message);
                    break;
            }

            SolidColorBrush color = GetLogColor(level);

            // Добавляем сообщение в коллекцию для отображения в GUI
            Application.Current.Dispatcher.Invoke(() =>
            {
                _logMessages.Add(new LogEntry
                {
                    Message = logEntry,
                    Color = color
                });
            });
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
