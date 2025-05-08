using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HeartRateGear.Server
{
    public class HeartRateServer
    {
        /// <summary>
        /// The HTTP server.
        /// </summary>
        private readonly HttpListener _server;
        
        /// <summary>
        /// Variable is used to stop the server.
        /// </summary>
        private bool _tryStop;
        
        /// <summary>
        /// The servicing task, it is used to handle incoming requests.
        /// </summary>
        private Task _servicingTask;
        
        /// <summary>
        /// Heart rate value.
        /// </summary>
        private int _heartRate;
        
        /// <summary>
        /// Determine if the server is started.
        /// </summary>
        public bool IsServerStarted => _server is { IsListening: true };
        
        /// <summary>
        /// Expose the prefixes from the server.
        /// </summary>
        public HttpListenerPrefixCollection Prefixes => _server.Prefixes;
        
        /// <summary>
        /// This method is called when the heart rate is updated.
        /// </summary>
        public event EventHandler<int> HeartRateUpdated;

        /// <summary>
        /// Constructor for the HeartRateServer class.
        /// </summary>
        /// <param name="port">by default 6547</param>
        public HeartRateServer(int port = 6547)
        {
            _server = new HttpListener();
            _tryStop = false;
            
            var port1 = port;
            _server = new HttpListener();
            string hostName = Dns.GetHostName();

            _server.Prefixes.Add("http://127.0.0.1:" + port1 + "/");
            _server.Prefixes.Add("http://localhost:" + port1 + "/");

            foreach (IPAddress ip in Dns.GetHostEntry(hostName).AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    _server.Prefixes.Add("http://" + ip + ":"+ port1 +"/");
            
            HeartRateUpdated += OnHeartRateUpdated;
        }

        /// <summary>
        /// Start the HTTP server.
        /// </summary>
        public void Start()
        {
            _server.Start();
            _servicingTask = Servicing();
        }

        /// <summary>
        /// Stop the HTTP server.
        /// </summary>
        public void Stop()
        {
            _server.Stop();
            _tryStop = true;
            _servicingTask.Wait();
        }

        /// <summary>
        /// This is the main loop of the server.
        /// </summary>
        private async Task Servicing()
        {
            while (!_tryStop)
            {
                var context = await _server.GetContextAsync();
                var request = context.Request;
                var response = context.Response;

                // Add CORS headers
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "GET");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
                
                string responseString;

                Console.WriteLine($"{DateTime.UtcNow} - {request.HttpMethod} {request.Url} {request.UserHostAddress}");
                
                switch (request.HttpMethod)
                {
                    case "GET":
                        if (!request.Url.LocalPath.StartsWith("/obs"))
                        {
                            responseString = HandleGetRequest(request, response);
                            break;
                        }

                        if (String.IsNullOrEmpty(request.Url.LocalPath.Substring(4)))
                        {
                            response.StatusCode = 302;
                            response.RedirectLocation = "/obs/";
                            responseString = "Redirecting to /obs/";
                            break;
                        }
                        
                        if (String.IsNullOrEmpty(request.Url.LocalPath.Substring(5)))
                        {
                            responseString = File.ReadAllText($"{Environment.CurrentDirectory}/www/index.html");
                            break;
                        }
                            
                        string path = request.Url.LocalPath.Substring(5);
                        if (!File.Exists($"{Environment.CurrentDirectory}/www/{path}"))
                        {
                            response.StatusCode = 404;
                            responseString = "File not found";
                            break;
                        }
                            
                        responseString = File.ReadAllText($"{Environment.CurrentDirectory}/www/{path}");
                        break;
                    case "POST":
                        responseString = HandlePostRequest(request, response);
                        break;
                    default:
                        response.StatusCode = 405;
                        responseString = "Method not allowed";
                        break;
                }

                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                using var output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// This method handles GET requests.
        /// </summary>
        /// <returns>Content of response</returns>
        private string HandleGetRequest(HttpListenerRequest request, HttpListenerResponse response) => _heartRate.ToString();

        /// <summary>
        /// This method handles POST requests.
        /// </summary>
        /// <returns>Content of response</returns>
        private string HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            var dataText = new StreamReader(request.InputStream, request.ContentEncoding).ReadToEnd();
            var clean = HttpUtility.ParseQueryString(dataText);

            if(!int.TryParse(clean.Get("rate"), out int result))
                throw new HttpListenerException(400, "Invalid heart rate value.");
            
            HeartRateUpdated?.Invoke(this, result == 0 ? -1 : result);

            return "OK";
        }
        
        /// <summary>
        /// This method is called when the heart rate is updated.
        /// It writes the heart rate to a file and update the local value of _heartRate.
        /// </summary>
        private void OnHeartRateUpdated(object sender, int rate)
        {
            try
            {
                File.WriteAllText($"{Environment.CurrentDirectory}/hr.txt", rate.ToString());
            }
            catch
            {
                Console.WriteLine("We weren't able to write the heart rate to the file.");
            }
            
            _heartRate = rate;
        }
    }
}