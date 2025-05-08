using System;
using System.Windows;
using System.Windows.Media;

namespace HeartRateGear.Web
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Client for the HeartRateServerNotify
        /// </summary>
        private HeartRateServerNotify _client;

        /// <summary>
        /// Constructor for the MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _client = new HeartRateServerNotify(6547);
            Receiver.DataContext = _client;
            ListboxIPs.ItemsSource = _client.Prefixes;
        }

        /// <summary>
        /// When the button is clicked, it will start or stop the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonServer_Click(object sender, RoutedEventArgs e)
        {
            string status = "OFF";

            if (_client.IsServerStarted) {
                status = "OFF";
                ButtonServer.Content = "START SERVER";
                BoxServerStatus.Background = new SolidColorBrush(Color.FromRgb(231, 76, 60)); // #e74c3c rgb(231, 76, 60)
                
                _client.Stop();
            }
            else
            {
                try
                {
                    RegenerateServerClient();
                    
                    _client.Start();
                    status = "ON";
                    ButtonServer.Content = "STOP SERVER";
                    BoxServerStatus.Background = new SolidColorBrush(Color.FromRgb(44, 204, 113)); // #2ecc71
                }
                catch (AccessViolationException exp)
                {
                    MessageBox.Show("Please launch the program as admin, to create the Web Server.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            LabelServerStatus.Content = status;
        }

        /// <summary>
        /// When pressed, copy the selected IP address to the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyActiveIP_Click(object sender, RoutedEventArgs e)
        {
            string ip = ListboxIPs.SelectedItem?.ToString();
            
            if (String.IsNullOrEmpty(ip))
            {
                MessageBox.Show("Please select an address.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Clipboard.SetText(ip);
            }
            catch (Exception ex)
            {
                //?
            }
        }

        /// <summary>
        /// When the port is changed, it will regenerate the server client
        /// </summary>
        private void RegenerateServerClient()
        {
            int port = 6547;
            
            if(!String.IsNullOrEmpty(TextboxPort.Text) && int.TryParse(TextboxPort.Text, out int newPort))
                port = newPort;
            
            _client = new HeartRateServerNotify(port);
            Receiver.DataContext = _client;
            ListboxIPs.ItemsSource = _client.Prefixes;
        }
    }
}
