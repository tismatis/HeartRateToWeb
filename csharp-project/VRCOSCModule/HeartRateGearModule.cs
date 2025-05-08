using System;
using System.Threading.Tasks;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Modules.Heartrate;

namespace HeartRateGear.VRCOSCModule
{
    [ModuleTitle("HeartRateGear")]
    [ModuleDescription("This module permit to take the heart rate from the HeartRateGear server.")]
    [ModuleType(ModuleType.Health)]
    public class HeartRateGearModule : HeartrateModule<HeartRateGearProvider>
    {
        /// <summary>
        /// Create an instance of the HeartRateGearProvider
        /// </summary>
        /// <returns></returns>
        protected override HeartRateGearProvider CreateProvider() => new();
        
        private DateTime _lastUpdate;

        /// <summary>
        /// Override the OnPreLoad method to set up the module settings
        /// </summary>
        protected override void OnPreLoad()
        {
            CreateTextBox(HeartRateGearSettings.IpAddress, "IP Address", "The Ip address of the server HeartRateGear.", "http://127.0.0.1:6547/");
            CreateSlider(HeartRateGearSettings.RecurrenceTime, "Recurrence Time", "The time between each request to the server (in second).", 3500, 500, 15000, 500);
            
            CreateGroup("Client Settings", HeartRateGearSettings.IpAddress, HeartRateGearSettings.RecurrenceTime);
            
            base.OnPreLoad();
        }
        
        /// <summary>
        /// Override the OnModuleStart method to check if the IP address is set
        /// </summary>
        /// <returns></returns>
        protected override Task<bool> OnModuleStart()
        {
            if (string.IsNullOrEmpty(GetSettingValue<string>(HeartRateGearSettings.IpAddress)))
            {
                Log("Cannot connect to the server, IP address is not set.");
                return Task.FromResult(false);
            }
            
            if (GetSettingValue<int>(HeartRateGearSettings.RecurrenceTime) < 500 || GetSettingValue<int>(HeartRateGearSettings.RecurrenceTime) > 15000)
            {
                Log("Recurrence time is not valid.");
                return Task.FromResult(false);
            }

            return base.OnModuleStart();
        }
        
        /// <summary>
        /// Request an update from the HeartRateGearProvider
        /// </summary>
        [ModuleUpdate(ModuleUpdateMode.ChatBox, true, 10000)]
        private void RequestNewHeartBeatData()
        {
            if ((DateTime.Now - _lastUpdate).TotalMilliseconds <
                GetSettingValue<int>(HeartRateGearSettings.RecurrenceTime))
                return;
            
            HeartrateProvider?.RequestUpdate();
            _lastUpdate = DateTime.Now;
        }
        
        /// <summary>
        /// Settings for the HeartRateGear module
        /// </summary>
        private enum HeartRateGearSettings
        {
            IpAddress,
            RecurrenceTime
        }
    }
}

