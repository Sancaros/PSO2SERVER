﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using PSO2SERVER.Models;
using PSO2SERVER.Packets;
using System.Threading.Tasks;

namespace PSO2SERVER
{
    public enum QueryMode
    {
        ShipList,/*12100 - 12900*/
        Block
    }

    public class QueryServer
    {
        public static List<Task> RunningServers = new List<Task>();

        private readonly QueryMode _mode;
        private readonly int _port;

        public QueryServer(QueryMode mode, int port)
        {
            _mode = mode;
            _port = port;
            var queryTask = Task.Run(() => RunAsync());
            RunningServers.Add(queryTask);
            Logger.WriteInternal("[QSP] 开始监听端口 " + port);
        }

        private async Task RunAsync()
        {
            Func<Socket, Task> connectionHandler;
            switch (_mode)
            {
                case QueryMode.Block:
                    connectionHandler = DoBlockBalanceAsync;
                    break;
                case QueryMode.ShipList:
                    connectionHandler = DoShipListAsync;
                    break;
                default:
                    connectionHandler = DoShipListAsync;
                    break;
            }

            var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                Blocking = true
            };
            var ep = new IPEndPoint(IPAddress.Any, _port);
            serverSocket.Bind(ep); // TODO: Custom bind address.
            serverSocket.Listen(5);

            while (true)
            {
                var newConnection = await Task.Factory.FromAsync(serverSocket.BeginAccept, serverSocket.EndAccept, null);
                _ = connectionHandler(newConnection); // Fire-and-forget pattern for handling connections
            }
        }

        private async Task DoShipListAsync(Socket socket)
        {
            var writer = new PacketWriter();
            var entries = new List<ShipEntry>();

            for (var i = 1; i < 11; i++)
            {
                var entry = new ShipEntry
                {
                    order = (ushort)i,
                    number = (uint)i,
                    status = i == 2 ? ShipStatus.Online : ShipStatus.Offline, // Maybe move to Config?
                    name = String.Format("Ship{0:0#}", i),
                    ip = ServerApp.BindAddress.GetAddressBytes()
                };
                entries.Add(entry);
            }
            PacketHeader header = new PacketHeader(8 + Marshal.SizeOf(typeof(ShipEntry)) * entries.Count + 12, 0x11, 0x3D, 0x4, 0x0);
            writer.WriteStruct(header);
            writer.WriteMagic((uint)entries.Count, 0xE418, 81);
            foreach (var entry in entries)
                writer.WriteStruct(entry);

            writer.Write((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
            writer.Write(1);

            var buffer = writer.ToArray();
            await Task.Factory.FromAsync(
                (cb, state) => socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, cb, state),
                socket.EndSend,
                null);
            socket.Close();
        }

        private async Task DoBlockBalanceAsync(Socket socket)
        {
            var writer = new PacketWriter();
            writer.WriteStruct(new PacketHeader(0x90, 0x11, 0x2C, 0x0, 0x0));
            writer.Write(new byte[0x68 - 8]);
            writer.Write(ServerApp.BindAddress.GetAddressBytes());
            writer.Write((UInt16)12205);
            writer.Write(new byte[0x90 - 0x6A]);

            var buffer = writer.ToArray();
            await Task.Factory.FromAsync(
                (cb, state) => socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, cb, state),
                socket.EndSend,
                null);
            socket.Close();
        }
    }
}