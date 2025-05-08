using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using VRCOSC.App.SDK.Modules.Heartrate;

namespace HeartRateGear.VRCOSCModule;

public class HeartRateGearProvider : HeartrateProvider
{
    public override bool IsConnected => true;
    public override bool IsReceiving => _heartRate > 0;

    private int _heartRate;

    private WebClient? _webClient;
    private bool _isRunning = true;
    
    public override async Task Initialise()
    {
        await base.Initialise();
        
        _webClient = new WebClient();
    }

    public void RequestUpdate()
    {
        if (_webClient == null)
        {
            Log("WebClient is null");
            return;
        }

        string s = "";
            
        try
        {
            s = _webClient.DownloadString("http://localhost:6547/heartRate");
        }
        catch (WebException e)
        {
            Log($"Error receiving data: {e.Message}");
            return;
        }

        if (string.IsNullOrWhiteSpace(s) || string.IsNullOrEmpty(s) || !int.TryParse(s, out int heartRate))
            return; // Data invalid or empty
            
        _heartRate = heartRate;

        if (_isRunning)
            OnHeartrateUpdate?.Invoke(Math.Clamp(heartRate, 0, 999));
    }

    public override Task Teardown()
    {
        _isRunning = false;
        
        if(_webClient != null)
            _webClient.Dispose();
        
        return Task.CompletedTask;
    }
}