using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NewStandRPS.Models
{
    public class TestConfigModel
    {
        [JsonProperty("model_name")]
        public string ModelName { get; set; }

        [JsonProperty("firmware_path")]
        public string FirmwarePath { get; set; }

        [JsonProperty("as4_autoprogram_path")]
        public string As4AutoProgramPath { get; set; }

        [JsonProperty("equipment_field_use")]
        public int EquipmentFieldUse { get; set; }

        [JsonProperty("equipment_str")]
        public string EquipmentStr { get; set; }



        [JsonProperty("1.Проверка узла preheating")]
        public bool IsPreheatingTestEnabled { get; set; }

        [JsonProperty("preheating_test")]
        public int PreheatingTest { get; set; }

        [JsonProperty("Состояние джампера preheating после окончания теста")]
        public bool IsPreheatingPositionEnabled { get; set; }

        [JsonProperty("preheating_position")]
        public int PreheatingPosition { get; set; }




        [JsonProperty("2. тестирование узла RKN")]
        public bool IsRknTestEnabled { get; set; }

        [JsonProperty("rkn_test")]
        public int RknTest { get; set; }

        [JsonProperty("Максимальное время старта RKN")]
        public bool IsRknStartupTimeMaxEnabled { get; set; }

        [JsonProperty("rkn_startup_time_max")]
        public int RknStartupTimeMax { get; set; }

        [JsonProperty("Минимальное время старта RKN")]
        public bool IsRknStartupTimeMinEnabled { get; set; }

        [JsonProperty("rkn_startup_time_min")]
        public int RknStartupTimeMin { get; set; }

        [JsonProperty("Время на отключение выхода в RKN")]
        public bool IsRknDisableTimeEnabled { get; set; }

        [JsonProperty("rkn_disable_time")]
        public int RknDisableTime { get; set; }



        [JsonProperty("3. Подавать 380В на плату")]
        public bool IsRkn380VTestEnabled { get; set; }

        [JsonProperty("rkn_380v_test")]
        public int Rkn380VTest { get; set; }




        [JsonProperty("4. Cамотестирование RPS-01")] // вроде готово 
        public bool IsBuildinTestEnabled { get; set; }

        [JsonProperty("buildin_test")]
        public int BuildinTest { get; set; }

        [JsonProperty("Температура на плате")]
        public bool IsTemperMinMaxEnabled { get; set; }

        [JsonProperty("temper_min")]
        public int TemperMin { get; set; }

        [JsonProperty("temper_max")]
        public int TemperMax { get; set; }

        [JsonProperty("Проверка реле RELAY")]
        public bool IsRelay1TestEnabled { get; set; }

        [JsonProperty("relay1_test")]
        public int Relay1Test { get; set; }

        [JsonProperty("Проверка реле AC_OK")]
        public bool IsRelay2TestEnabled { get; set; }

        [JsonProperty("relay2_test")]
        public int Relay2Test { get; set; }

        [JsonProperty("Версия ПО на плате RPS-01")]
        public bool IsFirmwareVersionEnabled { get; set; }

        [JsonProperty("fw_version")]
        public int FirmwareVersion { get; set; }




        [JsonProperty("5. Тестирование узла заряда")]
        public bool IsChargingTestEnabled { get; set; }

        [JsonProperty("charging_test")]
        public int ChargingTest { get; set; }

        [JsonProperty("Напряжение на АКБ при разрядке")]
        public bool IsAkbDischargeVoltageEnabled { get; set; }

        [JsonProperty("akb_voltage_ac_min")]
        public int AkbVoltageAcMin { get; set; }

        [JsonProperty("akb_voltage_ac_max")]
        public int AkbVoltageAcMax { get; set; }

        [JsonProperty("Напряжение на АКБ при зарядке")]
        public bool IsAkbChargeVoltageEnabled { get; set; }

        [JsonProperty("akb_charge_voltage_min")]
        public int AkbChargeVoltageMin { get; set; }

        [JsonProperty("akb_charge_voltage_max")]
        public int AkbChargeVoltageMax { get; set; }

        [JsonProperty("load_XX_test")]
        public int LoadXXTest { get; set; }

        [JsonProperty("load_XX_current_min")]
        public int LoadXXCurrentMin { get; set; }

        [JsonProperty("load_XX_current_max")]
        public int LoadXXCurrentMax { get; set; }

        [JsonProperty("Для старого стенда")]
        public bool IsOldStandEnabled { get; set; }

        [JsonProperty("load_16ohm_test")]
        public int Load16OhmTest { get; set; }

        [JsonProperty("load_16ohm_current_min")]
        public int Load16OhmCurrentMin { get; set; }

        [JsonProperty("load_16ohm_current_max")]
        public int Load16OhmCurrentMax { get; set; }

        [JsonProperty("load_16ohm_voltage_min")]
        public int Load16OhmVoltageMin { get; set; }

        [JsonProperty("load_16ohm_voltage_max")]
        public int Load16OhmVoltageMax { get; set; }

        [JsonProperty("load_22ohm_test")]
        public int Load22OhmTest { get; set; }

        [JsonProperty("load_22ohm_current_min")]
        public int Load22OhmCurrentMin { get; set; }

        [JsonProperty("load_22ohm_current_max")]
        public int Load22OhmCurrentMax { get; set; }

        [JsonProperty("load_22ohm_voltage_min")]
        public int Load22OhmVoltageMin { get; set; }

        [JsonProperty("load_22ohm_voltage_max")]
        public int Load22OhmVoltageMax { get; set; }

        [JsonProperty("load_68ohm_test")]
        public int Load68OhmTest { get; set; }

        [JsonProperty("load_68ohm_current_min")]
        public int Load68OhmCurrentMin { get; set; }

        [JsonProperty("load_68ohm_current_max")]
        public int Load68OhmCurrentMax { get; set; }

        [JsonProperty("load_68ohm_voltage_min")]
        public int Load68OhmVoltageMin { get; set; }

        [JsonProperty("load_68ohm_voltage_max")]
        public int Load68OhmVoltageMax { get; set; }

        [JsonProperty("для нового стенда")]
        public bool IsNewStandEnabled { get; set; }

        [JsonProperty("load_15ohm_test")]
        public int Load15OhmTest { get; set; }

        [JsonProperty("load_15ohm_current_min")]
        public int Load15OhmCurrentMin { get; set; }

        [JsonProperty("load_15ohm_current_max")]
        public int Load15OhmCurrentMax { get; set; }

        [JsonProperty("load_15ohm_voltage_min")]
        public int Load15OhmVoltageMin { get; set; }

        [JsonProperty("load_15ohm_voltage_max")]
        public int Load15OhmVoltageMax { get; set; }

        [JsonProperty("измерения с платы RPS-01 для нагрузки 15 Ом")]
        public bool IsLoad15OhmRps01TestEnabled { get; set; }

        [JsonProperty("load_15ohm_rps01_current_min")]
        public int Load15OhmRps01CurrentMin { get; set; }

        [JsonProperty("load_15ohm_rps01_current_max")]
        public int Load15OhmRps01CurrentMax { get; set; }

        [JsonProperty("load_15ohm_rps01_voltage_min")]
        public int Load15OhmRps01VoltageMin { get; set; }

        [JsonProperty("load_15ohm_rps01_voltage_max")]
        public int Load15OhmRps01VoltageMax { get; set; }

        [JsonProperty("load_20ohm_test")]
        public int Load20OhmTest { get; set; }

        [JsonProperty("load_20ohm_current_min")]
        public int Load20OhmCurrentMin { get; set; }

        [JsonProperty("load_20ohm_current_max")]
        public int Load20OhmCurrentMax { get; set; }

        [JsonProperty("load_20ohm_voltage_min")]
        public int Load20OhmVoltageMin { get; set; }

        [JsonProperty("load_20ohm_voltage_max")]
        public int Load20OhmVoltageMax { get; set; }

        [JsonProperty("load_30ohm_test")]
        public int Load30OhmTest { get; set; }

        [JsonProperty("load_30ohm_current_min")]
        public int Load30OhmCurrentMin { get; set; }

        [JsonProperty("load_30ohm_current_max")]
        public int Load30OhmCurrentMax { get; set; }

        [JsonProperty("load_30ohm_voltage_min")]
        public int Load30OhmVoltageMin { get; set; }

        [JsonProperty("load_30ohm_voltage_max")]
        public int Load30OhmVoltageMax { get; set; }

        [JsonProperty("load_50ohm_test")]
        public int Load50OhmTest { get; set; }

        [JsonProperty("load_50ohm_current_min")]
        public int Load50OhmCurrentMin { get; set; }

        [JsonProperty("load_50ohm_current_max")]
        public int Load50OhmCurrentMax { get; set; }

        [JsonProperty("load_50ohm_voltage_min")]
        public int Load50OhmVoltageMin { get; set; }

        [JsonProperty("load_50ohm_voltage_max")]
        public int Load50OhmVoltageMax { get; set; }

        [JsonProperty("load_70ohm_test")]
        public int Load70OhmTest { get; set; }

        [JsonProperty("load_70ohm_current_min")]
        public int Load70OhmCurrentMin { get; set; }

        [JsonProperty("load_70ohm_current_max")]
        public int Load70OhmCurrentMax { get; set; }

        [JsonProperty("load_70ohm_voltage_min")]
        public int Load70OhmVoltageMin { get; set; }

        [JsonProperty("load_70ohm_voltage_max")]
        public int Load70OhmVoltageMax { get; set; }

        [JsonProperty("load_100ohm_test")]
        public int Load100OhmTest { get; set; }

        [JsonProperty("load_100ohm_current_min")]
        public int Load100OhmCurrentMin { get; set; }

        [JsonProperty("load_100ohm_current_max")]
        public int Load100OhmCurrentMax { get; set; }

        [JsonProperty("load_100ohm_voltage_min")]
        public int Load100OhmVoltageMin { get; set; }

        [JsonProperty("load_100ohm_voltage_max")]
        public int Load100OhmVoltageMax { get; set; }

        [JsonProperty("Задержка на считывание параметров с платы RPS-01")]
        public bool IsRpsReadDelayEnabled { get; set; }

        [JsonProperty("rps_read_delay")]
        public int RpsReadDelay { get; set; }
    }
}
