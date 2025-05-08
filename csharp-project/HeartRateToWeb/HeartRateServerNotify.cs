using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using HeartRateGear.Server;

namespace HeartRateGear.Web
{
    public class HeartRateServerNotify : INotifyPropertyChanged
    {
        /// <summary>
        /// Used for data binding
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Represents the HeartRateServer
        /// </summary>
        private readonly HeartRateServer _server;
        
        /// <summary>
        /// Expose the IsServerStarted property
        /// </summary>
        public bool IsServerStarted => _server is { IsServerStarted: true };
        
        /// <summary>
        /// Expose the Prefixes property
        /// </summary>
        public HttpListenerPrefixCollection Prefixes => _server.Prefixes;

        /// <summary>
        /// Expose the Last Update property
        /// </summary>
        public string LastUpdate
        {
            get => _lastUpdate.ToString("HH:mm:ss");
            set
            {
                _lastUpdate = DateTime.Parse(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastUpdate)));
            }
        }
        
        /// <summary>
        /// Represent the last time we received a heart rate
        /// </summary>
        private DateTime _lastUpdate;

        /// <summary>
        /// Expose the HeartRate property
        /// </summary>
        public string HeartRate
        {
            get => _heartRate.ToString();
            set
            {
                _heartRate = int.Parse(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HeartRate)));
            }
        }

        /// <summary>
        /// Represent the last heart rate recorded
        /// </summary>
        private int _heartRate;
        
        /// <summary>
        /// Expose the Server.Start
        /// </summary>
        public void Start() => _server.Start();

        /// <summary>
        /// Expose the Server.Stop
        /// </summary>
        public void Stop() => _server.Stop();

        /// <summary>
        /// Constructor for the HeartRateServerNotify
        /// </summary>
        /// <param name="port"></param>
        public HeartRateServerNotify(int port)
        {
            _server = new HeartRateServer(port);
            _server.HeartRateUpdated += OnHeartRateUpdated;
        }

        /// <summary>
        /// When the heart rate is updated, we will update the LastUpdate and HeartRate properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHeartRateUpdated(object sender, int e)
        {
            LastUpdate = DateTime.Now.ToString("HH:mm:ss");;
            HeartRate = e.ToString();
        }
    }
}