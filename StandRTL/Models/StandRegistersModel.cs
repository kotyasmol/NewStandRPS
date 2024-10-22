using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewStandRPS.Models
{
    public class StandRegistersModel
    {
        // Регистры для слейва 2 (стенд)
        public ushort ACConnection { get; set; }
        public ushort LatrConnection { get; set; }
        public ushort ACBConnection { get; set; }
        public ushort ACBPolarity { get; set; }
        public ushort TemperatureSimulator { get; set; }
        public ushort AC_OKRelayState { get; set; }
        public ushort RelayState { get; set; }
        public ushort LoadSwitchKey { get; set; }
        public ushort ResistanceSetting { get; set; }
        public ushort ACBVoltage { get; set; }
        public ushort ACBAmperage { get; set; }
        public ushort VPresenceAtEntrance { get; set; }
        public ushort VPresenceAtExit { get; set; }
        public ushort Sensor1Temperature { get; set; }
        public ushort Sensor2Temperature { get; set; }
        public ushort CoolerControlKey { get; set; }
        public ushort FanOffTemperature { get; set; }
        public ushort FanOnTemperature { get; set; }
        public ushort MaxRadiatorTemperature { get; set; }
        public ushort StatisticsReset { get; set; }

        // Регистры для слейва 1 (плата)
        public ushort DeviceType { get; set; }
        public ushort HardwareVersion { get; set; }
        public ushort FirmwareVersion { get; set; }
        public ushort PowerType { get; set; }
        public ushort ACBVoltagePlate { get; set; } // чтобы отличать от слейва 2
        public ushort ChargingVoltage { get; set; }
        public ushort ACBCurrent { get; set; }
        public ushort BoardTemperature { get; set; }
        public ushort BATLedStatus { get; set; }
        public ushort ACBConnectionSwitch { get; set; }
        public ushort ChargingSwitch { get; set; }
        public ushort OptoRelay { get; set; }
        public ushort FullDischargeVoltage { get; set; }
        public ushort ACBLowVoltage { get; set; }
        public ushort BatteryRunTimeEstimate { get; set; }
        public ushort TestPassFlag { get; set; }
        public ushort BoardIdentifier { get; set; }
        public ushort LTC4151HealthFlag { get; set; }
        public ushort ACBVoltageADC { get; set; }
        public ushort ACBCurrentADC { get; set; }
        public ushort TestMode { get; set; }
    }
}