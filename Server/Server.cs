﻿using System;
using System.Collections.Generic;
using System.Timers;

using PSO2SERVER.Network;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER
{
    public class Server
    {
        public static Server Instance { get; private set; }

        private readonly SocketServer _server;

        public List<Client> Clients { get; private set; }
        public DateTime StartTime { get; private set; }
        public Timer PingTimer;

        public Server()
        {
            Clients = new List<Client>();
            _server = new SocketServer(12205);
            _server.NewClient += HandleNewClient;
            Instance = this;
            StartTime = DateTime.Now;

            PingTimer = new Timer(1000 * ServerApp.Config.PingTime); // 1 Minute default
            PingTimer.Elapsed += PingClients;
            PingTimer.Start();
        }

        public void Run()
        {
            while (true)
            {
                // Run the underlying SocketServer
                _server.Run();

                // Check Clients to make sure they still exist
                foreach (var client in Clients)
                    if (client.IsClosed)
                    {
                        Clients.Remove(client);
                        break;
                    }
            }
        }

        private void HandleNewClient(SocketClient client)
        {
            var newClient = new Client(this, client);
            Clients.Add(newClient);
        }

        private void PingClients(object sender, ElapsedEventArgs e)
        {
            // Ping!
            // TODO: Disconnect a client if we don't get a response in a certain amount of time
            foreach (var client in Clients)
            {
                if (client != null && client._account != null)
                {
                    Logger.Write("[HEY] Pinging " + client._account.Username);
                    client.SendPacket(new ServerPingPacket());
                }
            }
        }
    }
}