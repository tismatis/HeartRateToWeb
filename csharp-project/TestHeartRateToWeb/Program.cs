using System;
using System.Net;
using System.IO;
using System.Text;
using HeartRateGear.Server;

namespace HeartRateGear.CLI
{
    class Program
    {
        /// <summary>
        /// Start the Heart Rate Server
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Heart Rate Server...");
            HeartRateServer server = new HeartRateServer();

            server.Start();
            Console.WriteLine("Server started. Listening on:");
            foreach (var prefix in server.Prefixes)
                Console.WriteLine(prefix);
            
            Console.ReadLine();

            Console.WriteLine("Stopping Heart Rate Server...");
            server.Stop();
        }
    }
}
