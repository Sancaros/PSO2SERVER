using System;
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
        AuthList
    }

    public class QueryServer
    {
        public static List<Task> RunningServers = new List<Task>();

        private readonly QueryMode _mode;
        private readonly int _port;

        public QueryServer(QueryMode mode, string desc, int port)
        {
            _mode = mode;
            _port = port;
            var queryTask = Task.Run(() => RunAsync());
            RunningServers.Add(queryTask);
            Logger.WriteInternal("[监听] 监听" + desc + "端口 " + port);
        }

        private async Task RunAsync()
        {
            Func<Socket, Task> connectionHandler;
            switch (_mode)
            {
                case QueryMode.AuthList:
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

        public ShipStatus CheckShipStatus(int port)
        {
            //TODO 还有其他状态要判断
            if (PortChecker.IsPortListening(port))
            {
                return ShipStatus.Online;
            }

            return ShipStatus.Offline;
        }

        private async Task DoShipListAsync(Socket socket)
        {
            var writer = new PacketWriter();
            var entries = new List<ShipEntry>();

            for (var i = 1; i <= 10; i++)
            {
                var entry = new ShipEntry
                {
                    order = (ushort)i,
                    number = (uint)i,
                    //status = i == 2 ? ShipStatus.Online : ShipStatus.Full, // Maybe move to Config?
                    status = CheckShipStatus(ServerApp.ServerShipProt + (100 * (i - 1))), // Maybe move to Config?
                    name = String.Format("Ship{0:0#}", i),
                    ip = ServerApp.BindAddress.GetAddressBytes()
                };
                entries.Add(entry);
            }

            // Assuming header size: 8 bytes + (size of ShipEntry * number of entries) + 12 bytes
            int headerSize = 8;
            int shipEntrySize = Marshal.SizeOf(typeof(ShipEntry));
            int totalSize = headerSize + (shipEntrySize * entries.Count) + 12;
            PacketHeader header = new PacketHeader(totalSize, 0x11, 0x3D, 0x04, 0x00);

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
