using System;
using System.IO;
using System.Net;
using log4net;
using log4net.Config;
using PocketProxy.Utils;

namespace PocketProxy
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("PocketProxy");
        private static PocketProxy _pocketProxy;

        private static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config.xml"));

            LoadAndStart();

            Console.WriteLine("Type \"exit\" to shutdown safely!");
            string input = string.Empty;
            while (input != "exit")
            {
                input = Console.ReadLine();
                if (input == "reload")
                {
                    _pocketProxy.Stop();
                    Console.Clear();

                    LoadAndStart();
                }
            }

            Log.Info("Shutting server down!");
            _pocketProxy.Stop();
        }

        private static void LoadAndStart()
        {
            Log.Info("Loading settings...");
            IniFile ini = new IniFile(Path.Combine(Environment.CurrentDirectory, "config.ini"));

            string serverIp = ini.Read("server-ip", "Target Server");
            if (string.IsNullOrEmpty(serverIp))
            {
                ini.Write("server-ip", "localhost", "Target Server");
                serverIp = "localhost";
            }

            string portTemp = ini.Read("server-port", "Target Server");
            if (string.IsNullOrEmpty(portTemp))
            {
                ini.Write("server-port", "19132", "Target Server");
                portTemp = "19132";
            }
            int port = Convert.ToInt32(portTemp);

            string proxyIp = ini.Read("server-ip", "Proxy");
            if (string.IsNullOrEmpty(proxyIp))
            {
                ini.Write("server-ip", "0.0.0.0", "Proxy");
                proxyIp = "0.0.0.0";
            }

            portTemp = ini.Read("server-port", "Proxy");
            if (string.IsNullOrEmpty(portTemp))
            {
                ini.Write("server-port", "25565", "Proxy");
                portTemp = "25565";
            }
            int proxyPort = Convert.ToInt32(portTemp);

            Log.Info("Starting proxy...");
            var server = new IPEndPoint(HostResolver.ResolveAddress(serverIp), port);
            _pocketProxy = new PocketProxy(proxyIp, proxyPort, server);
            _pocketProxy.Start();
        }
    }
}