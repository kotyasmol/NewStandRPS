using Microsoft.Win32;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.IO.Ports;
using Modbus.Device;
using Newtonsoft.Json;
using NewStandRPS.Models;
using System.IO;
using HandyControl.Tools.Command;
using HandyControl.Tools;
using Newtonsoft.Json.Linq;
using Stylet.Logging;

namespace NewStandRPS.ViewModels
{

    public class MainViewModel : Conductor<IScreen>.Collection.OneActive, IDisposable
    {


            private ModbusSerialMaster _modbusMaster;
            private SerialPort _serialPort;
            private Logger _logger;

            //public ICommand ConnectCommand { get; }
            private bool _isConnected;
            public bool IsConnected
            {
                get => _isConnected;
                private set
                {
                    if (_isConnected != value)
                    {
                        _isConnected = value;
                        OnPropertyChanged(nameof(IsConnected));
                    }
                }
            }


            public ICommand SelectJsonFileCommand { get; }
            public TestConfigModel Config { get; set; }
            private string _jsonFilePath;
            public string JsonFilePath
            {
                get => _jsonFilePath;
                set
                {
                    if (_jsonFilePath != value)
                    {
                        _jsonFilePath = value;
                        OnPropertyChanged(nameof(JsonFilePath));
                        LoadConfig(); // Загрузка конфигурации после выбора файла
                    }
                }
            }
            private void SelectJsonFile(object parameter)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    Title = "Выберите JSON файл конфигурации"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    JsonFilePath = openFileDialog.FileName;
                    Log($"Выбран файл: {JsonFilePath}");
                }
            }
            private void LoadConfig()
            {
                string jsonFilePath = JsonFilePath; // Используем путь, выбранный пользователем
                if (string.IsNullOrEmpty(jsonFilePath))
                {
                    Log("Путь к файлу конфигурации не задан. Пожалуйста, выберите файл конфигурации.");
                    return;
                }

                if (File.Exists(jsonFilePath))
                {
                    try
                    {
                        string jsonData = File.ReadAllText(jsonFilePath);
                        Config = JsonConvert.DeserializeObject<TestConfigModel>(jsonData);

                        if (Config == null)
                        {
                            Config = new TestConfigModel(); // Создаем пустой объект, чтобы избежать NullReferenceException
                            Log($"Файл конфигурации пуст или не может быть десериализован с использованием конфигурации по умолчанию.");
                        }
                        else
                        {
                            Log($"Конфигурация загружена для модели: {Config.ModelName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Config = new TestConfigModel(); // Создаем пустой объект в случае ошибки десериализации
                        Log($"Не удалось загрузить конфигурацию: {ex.Message}");
                    }
                }
                else
                {
                    Config = new TestConfigModel(); // Создаем пустой объект, если файл не найден
                    Log("Файл конфигурации не найден. Использование конфигурации по умолчанию.");
                }
            }


            public enum StartAddress // значения стенда слейв 2 
            {
                ACConnection = 1300,          // 1300 - Подключение AC (230V)
                LatrConnection,               // 1301 - Подключение ЛАТР
                ACBConnection,                // 1302 - Подключение АКБ (ИМИТАТОР АКБ)
                ACBPolarity,                  // 1303 - Полярность АКБ
                TemperatureSimulator,         // 1304 - Имитатор термодатчика (-40, -35, -30)
                AC_OKRelayState,              // RO 1305 - Состояние реле 1 (AC_OK) 
                RelayState,                   // RO 1306 - Состояние реле 2 (Relay)
                LoadSwitchKey,                // 1307 - Ключ подключения нагрузки
                ResistanceSetting,            // 1308 - Установка сопротивления для контроля тока зарядки, Ом (от 3,3 до 267)
                ACBVoltage,                   // RO 1309 - Напряжение на АКБ, mV
                ACBAmperage,                  // RO 1310 - Ток через АКБ, mA
                VPresenceAtEntrance,       // RO 1311 - Присутствие напряжения на входе RPS
                VPresenceAtExit,           // RO 1312 - Присутствие напряжения на выходе RPS
                Sensor1Temperature,           // RO 1313 - Температура датчика 1
                Sensor2Temperature,           // RO 1314 - Температура датчика 2
                CoolerControlKey,             // 1315 - Ключ управления вентиляторами
                FanOffTemperature,            // 1316 - Температура выключения вентиляторов
                FanOnTemperature,             // 1317 - Температура включения вентиляторов
                MaxRadiatorTemperature,       // 1318 - Установка максимальной температуры на радиаторе
                StatisticsReset               // 1319 - Очистка статистики
            }
            public enum StartAddressPlate // слейв 1
            {
                DeviceType = 1000,              // 1000 - Тип устройства
                HardwareVersion,                // 1001 - Аппаратная версия платы
                FirmwareVersion,                // 1002 - Версия прошивки
                PowerType,                      // 1003 - Тип питания (0 - АКБ / 1 - VAC)
                ACBVoltage,                     // 1004 - Напряжение на АКБ в mV
                ChargingVoltage,                // 1005 - Напряжение зарядки АКБ в mV
                ACBCurrent,                     // 1006 - Ток через АКБ в mA
                BoardTemperature,               // 1007 - Температура на плате в градусах
                BATLedStatus,                   // 1008 - Состояние светодиода BAT
                ACBConnectionSwitch,            // 1009 - Ключ подключения АКБ
                ChargingSwitch,                 // 1010 - Ключ включения зарядки
                OptoRelay,                      // 1011 - Оптореле - то самое сигма гигачад реле очень нужно  + супер важно 
                Unused_AC_OKOptocoupler,        // 1012 - Оптрон AC_OK (не используется)
                FullDischargeVoltage,           // 1013 - Напряжение полного отключения
                ACBLowVoltage,                  // 1014 - Низкое напряжение АКБ
                BatteryRunTimeEstimate,         // 1015 - Прогноз времени работы от АКБ
                TestPassFlag,                   // 1016 - Флаг прохождения тестирования
                BoardIdentifier,                // 1017 - Идентификатор платы
                LTC4151HealthFlag,              // 1018 - Флаг исправности LTC4151
                ACBVoltageADC,                  // 1019 - Напряжение АКБ (АЦП)
                ACBCurrentADC,                  // 1020 - Ток через АКБ (АЦП)
                TestMode                       // 1021 - Тестовый режим
            }
            private ushort ReadRegister(byte slaveID, ushort registerAddress)
            {
                ushort[] result = _modbusMaster.ReadHoldingRegisters(slaveID, registerAddress, 1);
                return result[0];

            }
            private void WriteRegister(byte slaveID, ushort registerAddress, int value)
            {
                try
                {
                    _modbusMaster.WriteSingleRegister(slaveID, registerAddress, (ushort)value);
                    Log($"Значение {value} успешно записано в регистр {registerAddress} для устройства с ID {slaveID}.");
                }
                catch (Exception ex)
                {
                    Log($"Ошибка при записи значения {value} в регистр {registerAddress} для устройства с ID {slaveID}: {ex.Message}");
                }
            }
            public void SetRpsPreheating(int value)
            {
                Log($"Установка эквивалента температуры: {value}");
                byte slaveID = 2;
                ushort registerAddress = (ushort)StartAddress.TemperatureSimulator; // 1304 имитатор термодатчика
                WriteRegister(slaveID, registerAddress, value);
            }


            public ICommand StartTestingCommand { get; }
            private void StartTestCommandExecute(object parameter)
            {
                StartTesting(1, Config);  // Вызов метода StartTesting
            }
        public void StartTesting(byte slaveID, TestConfigModel config)
        {
            try
            {
                // Подключение к устройству
                _serialPort = new SerialPort("COM3") // добавить выбор порта
                {
                    BaudRate = 4800,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    ReadTimeout = 1000
                };

                try
                {
                    _serialPort.Open();
                    _modbusMaster = ModbusSerialMaster.CreateRtu(_serialPort);
                    _modbusMaster.Transport.Retries = 3;
                    IsConnected = true;
                    Log("Стенд подключен.");
                    ReadRegister(slaveID, 1);  // Чтение тестового регистра для проверки подключения

                    // Начало тестирования после подключения

                    if (config.IsPreheatingTestEnabled)
                    {
                        if (PreheatingTest(Config))
                        {
                            Log("PREHEATING TEST ПРОЙДЕН");
                        }
                        else
                        {
                            Log("PREHEATING TEST: НЕ ПРОЙДЕН");
                        }
                    }
                    else
                    {
                        Log("PREHEATING TEST: НЕ ПРОВОДИЛСЯ");
                    }

                    if (config.IsRknTestEnabled)
                    {
                        if (RknTest(Config))
                        {
                            Log("RKN ТЕСТ ПРОЙДЕН");
                        }
                        else
                        {
                            Log("RKN ТЕСТ НЕ ПРОЙДЕН");
                        }
                    }
                    else
                    {
                        Log("RKN ТЕСТ НЕ ПРОВОДИЛСЯ");
                    }

                    if (config.IsBuildinTestEnabled)
                    {
                        if (SelfTest(Config))
                        {
                            Log("Самотестирование успешно");
                        }
                        else
                        {
                            Log("Самотестирование не пройдено");
                        }
                    }
                    else
                    {
                        Log("Самотестирование не проводилось");
                    }
                }
                catch (Exception ex)
                {
                    Log($"Невозможно подключиться: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка тестирования: {ex.Message}");
            }
            finally
            {
                // Завершение тестирования и закрытие соединения
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                    IsConnected = false;
                    Log("Соединение закрыто.");
                }

                // Возвращение параметров стенда в исходное состояние
                WriteRegister(2, (ushort)StartAddress.LatrConnection, 0);
                WriteRegister(2, (ushort)StartAddress.ACConnection, 0);
                WriteRegister(2, (ushort)StartAddress.LoadSwitchKey, 1); // или не 1?
                WriteRegister(2, (ushort)StartAddress.ResistanceSetting, 4);

                Log("Все параметры стенда возвращены в исходное состояние.");
            }
        }

        public bool PreheatingTest(TestConfigModel config)
            {
                try
                {
                    // 1. Подготовка к тестированию
                    WriteRegister(2, (ushort)StartAddress.LatrConnection, 1);
                    WriteRegister(2, (ushort)StartAddress.ACConnection, 0);
                    WriteRegister(2, (ushort)StartAddress.LoadSwitchKey, 0);
                    WriteRegister(2, (ushort)StartAddress.ResistanceSetting, 100);

                    MessageBox.Show("Переведите джампер PREHEATING в положение YES", "Инструкция", MessageBoxButton.OK, MessageBoxImage.Information);
                    Thread.Sleep(500);
                    MessageBox.Show("Установите напряжение на ЛАТР 230В", "Инструкция", MessageBoxButton.OK, MessageBoxImage.Information);

                    // 2. Установка температуры -30, Подключение ЛАТР и AC
                    SetRpsPreheating(-30);
                    WriteRegister(2, (ushort)StartAddress.LatrConnection, 1);
                    WriteRegister(2, (ushort)StartAddress.ACConnection, 1);

                    // 3. Проверка наличия напряжения 230V на входе
                    int readCnt = 0;
                    while ((ReadRegister(2, (ushort)StartAddress.VPresenceAtEntrance) != 1) && (readCnt < 5))
                    {
                        Thread.Sleep(1000);
                        Log("Считывание значения напряжения 230V на входе...");
                        readCnt++;
                    }
                    if (readCnt >= 5)
                    { Log("Ошибка: не удалось зафиксировать наличие 230V на входе."); return false; }
                    Log("Напряжение 230V на входе зафиксировано.");

                    // 4. Проконтролировать 230V на выходе
                    readCnt = 0;
                    bool outputDetected = false;
                    while (readCnt < config.RknStartupTimeMax)
                    {
                        if (ReadRegister(2, (ushort)StartAddress.VPresenceAtExit) == 1)
                        {
                            if (readCnt < config.RknStartupTimeMin)
                            {
                                Log($"Ошибка: напряжение 230V на выходе появилось раньше {config.RknStartupTimeMin} секунд.");
                                return false;
                            }
                            outputDetected = true;
                            Log($"Напряжение 230V на выходе появилось через {readCnt} секунд.");
                            break;
                        }
                        Thread.Sleep(1000);
                        Log("Считывание значения напряжения 230V на выходе...");
                        readCnt++;
                    }

                    if (!outputDetected)
                    {
                        Log($"Ошибка: напряжение 230V на выходе не появилось в течение {config.RknStartupTimeMax} секунд.");
                        return false;
                    }

                    // 7. Проверка перехода на -35°C, напряжение на выходе не должно отключиться
                    Log("Проверка перехода на -35°C");
                    SetRpsPreheating(-35);
                    readCnt = 0;
                    while ((ReadRegister(2, (ushort)StartAddress.VPresenceAtExit) != 1) && (readCnt < config.RknDisableTime))
                    {
                        Thread.Sleep(1000);
                        Log("Считывание значения напряжения 230V на выходе...");
                        readCnt++;
                    }

                    if (readCnt >= config.RknDisableTime)
                    {
                        Log("Ошибка: напряжение 230V на выходе должно оставаться включённым при -35°C.");
                        return false;
                    }
                    Log("Проверка работы при -35: Ok");

                    // 8. Проверка отключения при -40
                    Log("Проверка отключения при -40°C");
                    SetRpsPreheating(-40);
                    readCnt = 0;
                    while ((ReadRegister(2, (ushort)StartAddress.VPresenceAtExit) != 0) && (readCnt < config.RknDisableTime))
                    {
                        Thread.Sleep(1000);
                        Log("Считывание значения напряжения 230V на выходе...");
                        readCnt++;
                    }

                    if (readCnt >= config.RknDisableTime)
                    {
                        Log("Ошибка: напряжение 230V на выходе не отключилось при -40°C.");
                        return false;
                    }

                    Log("Напряжение 230V на выходе успешно отключилось при -40°C.");
                    // 7. Проверка перехода на -35: напряжение на выходе не должно включиться
                    Log("Проверка перехода на -35°C");
                    SetRpsPreheating(-35);
                    readCnt = 0;
                    while ((ReadRegister(2, (ushort)StartAddress.VPresenceAtExit) != 1) && (readCnt < config.RknStartupTimeMax))
                    {
                        Thread.Sleep(1000);
                        Log("Считывание значения напряжения 230V на выходе...");
                        readCnt++;
                    }

                    if (ReadRegister(2, (ushort)StartAddress.VPresenceAtExit) != 0)
                    {
                        Log("Ошибка работы на -35: RKN не должен был включиться.");
                        return false;
                    }

                    Log("Проверка работы при -35: Ok");
                    Log("Проверка узла Preheating: Ok");
                    SetRpsPreheating(-30);
                    Thread.Sleep(100);

                    return true;
                }
                catch (Exception ex)
                {
                    Log($"Не получилось выполнить тест: {ex.Message}");
                    return false;
                }
                finally
                {
                    WriteRegister(2, (ushort)StartAddress.LatrConnection, 0);
                    WriteRegister(2, (ushort)StartAddress.ACConnection, 0);
                }
            }
        public bool RknTest(TestConfigModel config)
            {
                try
                {

                    // Подключение ЛАТР и AC
                    WriteRegister(2, (ushort)StartAddress.LatrConnection, 1);
                    WriteRegister(2, (ushort)StartAddress.ACConnection, 1);


                    Log("Проверка старта узла RKN");

                    // Счётчик попыток чтения
                    int readCnt = 0;

                    // Цикл проверки состояния RKN (ожидание, пока на выходе не появится 1)
                    while ((ReadRegister(2, (ushort)StartAddress.VPresenceAtExit) != 1) && (readCnt < config.RknStartupTimeMax))
                    {
                        Thread.Sleep(1000); // Ждём 1 секунду
                        Log("Считывание состояния узла RKN...");
                        readCnt++;
                    }

                    // Проверка: если время старта не в допустимом диапазоне (меньше min или больше max)
                    if (readCnt >= config.RknStartupTimeMax || readCnt < config.RknStartupTimeMin)
                    {
                        Log($"Ошибка: Время старта узла RKN не в допуске: {readCnt} секунд.");
                        return false; // Завершаем тест с ошибкой
                    }

                    // Время старта в допустимом диапазоне
                    Log($"Время старта узла RKN при 230В в допуске: {readCnt} секунд.");



                    MessageBox.Show("Установите напряжение на ЛАТР 150В", "Инструкция", MessageBoxButton.OK, MessageBoxImage.Information);
                    Log("Проверка RKN на 150В:");
                    readCnt = 0;
                    // Проверка состояния RKN: пока не будет 0 на выходе, проверяем в течение заданного времени
                    while ((ReadRegister(2, (ushort)StartAddress.VPresenceAtExit) != 0) && (readCnt <= config.RknDisableTime))
                    {
                        Thread.Sleep(1000);
                        Log("Считывание состояния RKN на 150В...");
                        readCnt++;
                    }
                    // Если время ожидания превышено, но RKN всё ещё включён
                    if (readCnt > config.RknDisableTime)
                    {
                        Log("Ошибка: узел RKN не отключился в течение допустимого времени на 150В.");
                        return false;
                    }
                    Log("Узел RKN успешно отключился на 150В.");



                    MessageBox.Show("Установите напряжение на ЛАТР 190В", "Инструкция", MessageBoxButton.OK, MessageBoxImage.Information);
                    Log("Проверка RKN на 190В:");
                    readCnt = 0;
                    while ((ReadRegister(2, (ushort)StartAddress.VPresenceAtExit) != 1) && (readCnt < config.RknStartupTimeMax))
                    {
                        Thread.Sleep(1000);
                        Log("Считывание состояния RKN на 190В...");
                        readCnt++;
                    }

                    if (readCnt >= config.RknStartupTimeMax)
                    {
                        Log($"Ошибка: Время старта узла RKN не в допуске: {readCnt} секунд.");
                        return false;
                    }
                    Log($"Время старта узла RKN при 190В в допуске: {readCnt} секунд.");


                    Log("Проверка RKN на 250В:");
                    MessageBox.Show("Установите напряжение на ЛАТР 250В", "Инструкция", MessageBoxButton.OK, MessageBoxImage.Information);
                    readCnt = 0;
                    while ((ReadRegister(2, (ushort)StartAddress.VPresenceAtExit) != 1) && (readCnt < config.RknDisableTime))
                    {
                        Thread.Sleep(1000);
                        Log("Считывание состояния RKN на 250В...");
                        readCnt++;
                    }
                    if (readCnt >= config.RknDisableTime)
                    {
                        Log($"Ошибка: RKN не включился в течение {config.RknDisableTime} секунд.");
                        return false;
                    }
                    Log("RKN успешно включился при 250В.");

                    Log("Проверка RKN на 270В:");
                    MessageBox.Show("Установите напряжение на ЛАТР 270В", "Инструкция", MessageBoxButton.OK, MessageBoxImage.Information);
                    readCnt = 0;
                    while ((ReadRegister(2, (ushort)StartAddress.VPresenceAtExit) != 0) && (readCnt < config.RknDisableTime))
                    {
                        Thread.Sleep(1000);
                        Log("Считывание состояния RKN на 270В...");
                        readCnt++;
                    }
                    if (readCnt >= config.RknDisableTime)
                    {
                        Log($"Ошибка: RKN не отключился в течение {config.RknDisableTime} секунд.");

                        return false;
                    }
                    Log("RKN успешно отключился при 270В.");



                    Log("Проверка RKN на 230В:");
                    MessageBox.Show("Установите напряжение на ЛАТР 230В", "Инструкция", MessageBoxButton.OK, MessageBoxImage.Information);
                    Thread.Sleep(1000);
                    readCnt = 0;
                    while ((ReadRegister(2, (ushort)StartAddress.VPresenceAtExit) != 1) && (readCnt < config.RknStartupTimeMax))
                    {
                        Thread.Sleep(1000);
                        Log("Считывание состояния RKN на 230В...");
                        readCnt++;
                    }
                    if (readCnt >= config.RknStartupTimeMax)
                    {
                        Log($"Ошибка: Время старта узла RKN на 230В не в допуске: {readCnt} секунд.");
                        return false;
                    }
                    Log($"Узел RKN успешно включился при 230В за {readCnt} секунд.");



                    if (config.IsRkn380VTestEnabled)
                    {
                        Log("Проверка RKN на 380В");

                        // Отключаем ЛАТР и подаём 380В в зависимости от типа устройства

                        WriteRegister(2, (ushort)StartAddress.LatrConnection, 0);
                        Thread.Sleep(3000);
                        WriteRegister(2, (ushort)StartAddress.ACConnection, 1);



                        // Проверка наличия 380В на входе
                        readCnt = 0;
                        while (ReadRegister(2, (ushort)StartAddress.VPresenceAtEntrance) != 1 && readCnt < 10)
                        {
                            Thread.Sleep(1000);  // Пауза 1 секунда
                            Log("Считывание состояния 380В на входе...");
                            readCnt++;
                        }

                        if (ReadRegister(2, (ushort)StartAddress.VPresenceAtEntrance) != 1)
                        {

                            Log("Ошибка: Подача 380В на вход стенда не зафиксирована.");
                            return false;
                        }

                        // Проверка отключения 380В на выходе
                        readCnt = 0;
                        while (ReadRegister(2, (ushort)StartAddress.VPresenceAtExit) != 0 && readCnt < config.RknStartupTimeMax)
                        {
                            Thread.Sleep(1000);  // Пауза 1 секунда
                            Log("Считывание состояния 380В на выходе...");
                            readCnt++;
                        }

                        if (readCnt >= config.RknDisableTime)
                        {

                            Log($"Ошибка: Время отключения узла RKN при 380В не в допуске: {readCnt} секунд.");
                            return false;
                        }

                        Log($"Время отключения узла RKN при 380В в допуске: {readCnt} секунд.");
                        Thread.Sleep(10000);
                        if (config.ModelName == "RPS_STAND")
                        {
                            WriteRegister(2, (ushort)StartAddress.ACConnection, 0); // Отключаем 380В
                            Thread.Sleep(1000);
                            WriteRegister(2, (ushort)StartAddress.LatrConnection, 1);  // Включаем ЛАТР
                        }
                        else if (config.ModelName == "RPS_STAND_V4")
                        {
                            WriteRegister(2, (ushort)StartAddress.LatrConnection, 1);  // Включаем ЛАТР
                            Thread.Sleep(3000);
                            WriteRegister(2, (ushort)StartAddress.ACConnection, 1);   // Включаем AC
                        }

                        // Проверяем, что RKN включится после отключения 380В
                        Thread.Sleep(1000);
                        readCnt = 0;
                        while (ReadRegister(2, (ushort)StartAddress.VPresenceAtExit) != 1 && readCnt < config.RknStartupTimeMax)
                        {
                            Thread.Sleep(1000);
                            Log("Считывание состояния RKN после воздействия 380В...");
                            readCnt++;
                        }

                        if (readCnt >= config.RknStartupTimeMax)
                        {
                            Log($"Ошибка: Время старта узла RKN после воздействия 380В не в допуске: {readCnt} секунд.");
                            return false;
                        }

                        Log($"Время старта узла RKN при 230В (после воздействия 380В) в допуске: {readCnt} секунд.");
                    }


                    Log("Завершение тестирования RKN...");
                    if (config.ModelName == "RPS_STAND")
                    {
                        WriteRegister(2, (ushort)StartAddress.LatrConnection, 0);
                        Log("ЛАТР выключен.");
                    }
                    else if (config.ModelName == "RPS_STAND_V4")
                    {
                        WriteRegister(2, (ushort)StartAddress.ACConnection, 0);
                        Log("AC выключен.");
                    }

                    Thread.Sleep(1000);
                    if (config.PreheatingPosition == 0)
                    {
                        MessageBox.Show("Установите джампер PREHEATING в положение NO", "Инструкция", MessageBoxButton.OK, MessageBoxImage.Information);
                        Log("Ожидание установки джампера PREHEATING в положение NO.");
                    }

                    Log("Тест RKN завершён успешно.");
                    return true;

                }
                catch (Exception ex)
                {
                    Log($"Не получилось выполнить тест: {ex.Message}");
                    return false;
                }
            }
        public bool SelfTest(TestConfigModel config)
            {
                try
                {
                    Log("Самотестирование запущено...");

                    // 1. Проверка RS-485 (Чтение идентификатора устройства)
                    /* Log("Проверка RS-485...");
                     ushort expectedValue = 0x11A6; // Ожидаемое значение идентификатора 
                     ushort registerAddress = (ushort)StartAddressPlate.DeviceType; // Адрес регистра с типом устройства

                     if (CheckRps01Param(registerAddress, expectedValue, config.RpsReadDelay))
                     {
                         Log("Кнопка запуска исправна.");
                         Log("Проверка RS-485 (Чтение идентификатора): Ок");
                     }
                     else
                     {
                         Log("Ошибка чтения идентификатора. Самотестирование не пройдено.");
                         return false; // Прекратить выполнение теста, если чтение идентификатора неудачно
                     }*/

                    // 2. Проверка узла АКБ
                    Log("Проверка узла АКБ...");

                    // 2.1 Установка обратной полярности АКБ
                    WriteRegister(2, (ushort)StartAddress.ACBPolarity, 1); // Обратная полярность
                    Log("Установлена обратная полярность АКБ.");
                    Thread.Sleep(100);

                    // 2.2 Включаем АКБ
                    WriteRegister(2, (ushort)StartAddress.ACBConnection, 1); // Включение АКБ
                    Log("АКБ включен с обратной полярностью.");

                    // 2.3 Ожидание подтверждения от пользователя
                    if (!ShowConfirmation("Индикатор неправильной полярности АКБ горит?"))
                    {
                        Log("Индикатор неправильной полярности не горит. Тест не пройден.");
                        return false;
                    }

                    Thread.Sleep(100);

                    // 2.4 Установка прямой полярности АКБ
                    WriteRegister(2, (ushort)StartAddress.ACBPolarity, 0); // Нормальная полярность
                    Log("Установлена нормальная полярность АКБ.");
                    Thread.Sleep(1000);
                    Log("Тест узла АКБ успешно завершен.");

                    // 2.5 Ожидание нажатия кнопки START
                    MessageBox.Show("Нажмите кнопку START", "Инструкция", MessageBoxButton.OK, MessageBoxImage.Information);

                    Log("Проверка версии ПО платы RPS...");

                    // Проверка версии ПО
                    if (CheckRpsParam((ushort)StartAddressPlate.FirmwareVersion, config.FirmwareVersion, config.RpsReadDelay))
                    {
                        ushort firmwareVersion = ReadRegister(1, (ushort)StartAddressPlate.FirmwareVersion);
                        Log($"Версия ПО платы RPS: {firmwareVersion}");
                    }
                    else
                    {
                        ushort firmwareVersion = ReadRegister(1, (ushort)StartAddressPlate.FirmwareVersion);
                        Log($"Ошибка: Версия ПО платы RPS: {firmwareVersion}");
                        return false;
                    }

                    // Проверка температуры
                    if (CheckMinMaxParam((ushort)StartAddressPlate.BoardTemperature, config.TemperMin, config.TemperMax, config.RpsReadDelay))
                    {
                        ushort Temperature = ReadRegister(1, (ushort)StartAddressPlate.BoardTemperature);
                        Log($"Температура в норме: {Temperature}");
                    }
                    else
                    {
                        ushort Temperature = ReadRegister(1, (ushort)StartAddressPlate.BoardTemperature);
                        Log($"Ошибка. Температура не в норме: {Temperature}");
                        return false;
                    }


                    // 3. Проверка работы от АКБ
                    /*Log("Проверка работы от АКБ...");
                    if (CheckRpsParam((ushort)StartAddressPlate.PowerType, 0, config.RpsReadDelay)) // Проверка работы от АКБ
                    {
                        Log("Работа от АКБ: Ок.");
                    }
                    else
                    {
                        Log("Питания от АКБ нет.");
                        return false;
                    }*/

                    // 4. Проверка напряжения АКБ
                    Log("Проверка напряжения АКБ...");
                    if (CheckMinMaxParam((ushort)StartAddressPlate.ACBVoltage, config.AkbVoltageAcMin, config.AkbVoltageAcMax, config.RpsReadDelay))
                    {
                        ushort akbVoltage = ReadRegister(1, (ushort)StartAddressPlate.ACBVoltage);
                        Log($"Измерение напряжения АКБ: Ок ({akbVoltage} mV)");
                        Log("Проверка статуса АКБ: Ok.");
                    }
                    else
                    {
                        ushort akbVoltage = ReadRegister(1, (ushort)StartAddressPlate.ACBVoltage);
                        Log($"Измеренное напряжение АКБ не в допуске: {akbVoltage} mV");
                        return false;
                    }


                    // Проверка реле 1 
                    if (config.IsRelay1TestEnabled)
                    {
                        Log("Проверка реле AC_OK");
                        WriteRegister(1, (ushort)StartAddressPlate.OptoRelay, 1); // ас_ок
                        Thread.Sleep(100);
                        WriteRegister(1, (ushort)StartAddressPlate.OptoRelay, 1);
                        Thread.Sleep(100);
                    }

                    // Проверка реле 2
                    if (config.IsRelay2TestEnabled)
                    {
                        Log("Проверка реле RELAY");

                        int a = 0;
                        while (a < 20)
                        {
                            WriteRegister(1, (ushort)StartAddressPlate.Unused_AC_OKOptocoupler, 1);
                            Thread.Sleep(100);

                            a++;
                        }

                    }

                    // Проверка состояния реле 1
                    if (config.IsRelay1TestEnabled)
                    {
                        if (CheckRpsParam((ushort)StartAddressPlate.OptoRelay, 1, config.RpsReadDelay))
                        {
                            Log("Проверка реле RELAY: Ok");
                        }
                        else
                        {

                            Log("Ошибка проверки реле RELAY");
                            return false;
                        }
                    }

                    // Проверка состояния реле 2
                    if (config.IsRelay2TestEnabled)
                    {
                        if (CheckRpsParam((ushort)StartAddressPlate.Unused_AC_OKOptocoupler, 1, config.RpsReadDelay))
                        {
                            Log("Проверка реле AC_OK: Ok");
                        }
                        else
                        {
                            Log("Ошибка проверки реле AC_OK");
                            return false;
                        }
                    }

                    // Отключаем реле 1 и 2 
                    if (config.IsRelay1TestEnabled)
                    {
                        WriteRegister(1, (ushort)StartAddressPlate.Unused_AC_OKOptocoupler, 0);
                        Thread.Sleep(100);
                    }

                    if (config.IsRelay2TestEnabled)
                    {
                        WriteRegister(1, (ushort)StartAddressPlate.OptoRelay, 0);
                        Thread.Sleep(100);
                    }

                    // Проверяем, что реле отключилось
                    if (config.IsRelay1TestEnabled)
                    {
                        if (CheckRpsParam((ushort)StartAddressPlate.OptoRelay, 0, config.RpsReadDelay))
                        {
                            Log("Проверка выключения реле RELAY: Ok");
                        }
                        else
                        {

                            Log("Ошибка проверки выключения реле RELAY");
                            return false;
                        }
                    }

                    if (config.IsRelay2TestEnabled)
                    {
                        if (CheckRpsParam((ushort)StartAddressPlate.Unused_AC_OKOptocoupler, 0, config.RpsReadDelay))
                        {
                            Log("Проверка выключения реле AC_OK: Ok");
                        }
                        else
                        {

                            Log("Ошибка проверки выключения реле AC_OK");
                            return false;
                        }
                    }

                    // Проверка индикатора CPU
                    if (!ShowConfirmation("Индикатор CPU мигает?"))
                    {
                        Log("Индикатор CPU неисправен");
                        return false;
                    }

                    // Проверка индикатора BAT
                    if (!ShowConfirmation("Индикатор BAT мигает?"))
                    {

                        Log("Индикатор BAT неисправен");
                        return false;
                    }

                    // Проверка индикатора HL1 (52V)
                    if (!ShowConfirmation("Индикатор HL1 (52V) горит?"))
                    {

                        Log("Индикатор HL1 (52V) неисправен");
                        return false;
                    }

                    // Ожидание нажатия кнопки STOP
                    MessageBox.Show("Нажмите кнопку STOP до полного отключения RPS-01", "Инструкция", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Проверка отключения платы RPS-01
                    /*ushort manufacturerID = ReadRegister(1, (ushort)StartAddressPlate.че);
                    if (manufacturerID == 0x11A6)
                    {
                        Log("Неисправность кнопки STOP: плата не отключилась");
                        return false;
                    }*/

                    Log("Самотестирование пройдено успешно.");
                    return true;

                }
                catch (Exception ex)
                {
                    Log($"Ошибка при самотестировании: {ex.Message}");
                    return false;
                }
            }
        private bool CheckMinMaxParam(ushort registerAddress, int minValue, int maxValue, int delay)
        {
           try
                {
                    ushort value = ReadRegister(1, registerAddress);
                    Log($"Считывание {registerAddress}: {value}");

                    if (value <= maxValue && value >= minValue)
                    {
                        return true;
                    }

                    for (int attempt = 0; attempt < 3; attempt++)
                    {
                        Thread.Sleep(delay);
                        value = ReadRegister(1, registerAddress);
                        Log($"Повторное считывание {registerAddress}: {value}");

                        if (value <= maxValue && value >= minValue)
                        {
                            return true;
                        }
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    Log($"Ошибка при проверке параметров: {ex.Message}");
                    return false;
                }
            }
        private bool CheckRpsParam(ushort registerAddress, int expectedValue, int delay)
            {
                try
                {
                    ushort actualValue = ReadRegister(1, registerAddress); // Чтение регистра с идентификатором устройства (slave ID = 1)
                    Log($"Считывание значения из регистра {registerAddress}: {actualValue}");

                    if (actualValue == expectedValue)
                    {
                        return true; // Значение соответствует ожидаемому
                    }

                    // Если значения не совпадают, ожидание и повторное чтение
                    for (int attempt = 0; attempt < 3; attempt++)
                    {
                        Thread.Sleep(delay);
                        actualValue = ReadRegister(1, registerAddress);
                        Log($"Повторное считывание значения: {actualValue}");

                        if (actualValue == expectedValue)
                        {
                            return true;
                        }
                    }

                    return false; // Значение не совпало после нескольких попыток
                }
                catch (Exception ex)
                {
                    Log($"Ошибка при проверке параметра: {ex.Message}");
                    return false;
                }
            }
            private static bool ShowConfirmation(string message)
            {
                var result = MessageBox.Show(message, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            }

            public MainViewModel()
            {
                SelectJsonFileCommand = new RelayCommand(SelectJsonFile);
                StartTestingCommand = new RelayCommand(StartTestCommandExecute);

           // _logger = new Logger(main);

            // Пример логирования
            //_logger.Log("Программа запущена", LogLevel.I);
            //_logger.Log("Ошибка подключения", LogLevel.E);

        }
            private void Log(string message)
            {
                LogMessages.Add($"{DateTime.Now}: {message}");
            }
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public event PropertyChangedEventHandler PropertyChanged; // Для обновления GUI
            public ObservableCollection<string> LogMessages { get; private set; }

            public class RelayCommand : ICommand
            {
                private readonly Action<object> _execute;
                private readonly Predicate<object> _canExecute;

                public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
                {
                    _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                    _canExecute = canExecute;
                }

                public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

                public void Execute(object parameter) => _execute(parameter);

                public event EventHandler CanExecuteChanged
                {
                    add => CommandManager.RequerySuggested += value;
                    remove => CommandManager.RequerySuggested -= value;
                }
            }
        public void Dispose()
        {
            // 
        }
    }
    
    }
