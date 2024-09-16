﻿using System;
using System.IO;
using System.Security.Cryptography;
using PSO2SERVER.Database;
using PSO2SERVER.Models;
using PSO2SERVER.Network;
using PSO2SERVER.Packets;
using PSO2SERVER.Packets.Handlers;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Zone;

namespace PSO2SERVER
{
    public class Client
    {
        internal static RSACryptoServiceProvider RsaCsp = null;
        private readonly byte[] _readBuffer;
        private readonly Server _server;
        internal ICryptoTransform InputArc4, OutputArc4;
        private int _packetId;
        private uint _readBufferSize;

        public Client(Server server, SocketClient socket)
        {
            IsClosed = false;
            _server = server;
            Socket = socket;

            socket.DataReceived += HandleDataReceived;
            socket.ConnectionLost += HandleConnectionLost;

            _readBuffer = new byte[1024 * 64];
            _readBufferSize = 0;

            InputArc4 = null;
            OutputArc4 = null;

            SendPacket(new ServerHelloPacket());
        }

        public bool IsClosed { get; private set; }
        public SocketClient Socket { get; private set; }
        // Game properties, TODO Consider moving these somewhere else
        public Player User { get; set; }
        public Character Character { get; set; }
        //public Zone.Zone CurrentZone { get; set; }
        public Map CurrentZone;
        public uint MovementTimestamp { get; internal set; }
        public Party.Party currentParty;

        public PSOLocation CurrentLocation;
        public PSOLocation LastLocation;

        private void HandleDataReceived(byte[] data, int size)
        {
            if ((_readBufferSize + size) > _readBuffer.Length)
            {
                Logger.WriteError("[接收] 接收到 {0} 字节大于预设buf长度", size);
                // Buffer overrun
                // TODO: Drop the connection when this occurs?
                return;
            }

            //Logger.Write("[接收] 接收到 {0} 字节", size);

            Array.Copy(data, 0, _readBuffer, _readBufferSize, size);

            InputArc4?.TransformBlock(_readBuffer, (int)_readBufferSize, size, _readBuffer, (int)_readBufferSize);

            _readBufferSize += (uint)size;

            // Process ALL the packets
            uint position = 0;

            while ((position + 8) <= _readBufferSize)
            {
                var packetSize =
                    _readBuffer[position] |
                    ((uint)_readBuffer[position + 1] << 8) |
                    ((uint)_readBuffer[position + 2] << 16) |
                    ((uint)_readBuffer[position + 3] << 24);

                // Minimum size, just to avoid possible infinite loops etc
                if (packetSize < 8)
                    packetSize = 8;

                // If we don't have enough data for this one...
                if (packetSize > 0x1000000 || (packetSize + position) > _readBufferSize)
                    break;

                // Now handle this one
                HandlePacket(
                    _readBuffer[position + 4], _readBuffer[position + 5],
                    _readBuffer[position + 6], _readBuffer[position + 7],
                    _readBuffer, position + 8, packetSize - 8);

                // If the connection was closed, we have no more business here
                if (IsClosed)
                    break;

                position += packetSize;
            }

            // Wherever 'position' is up to, is what was successfully processed
            if (position > 0)
            {
                if (position >= _readBufferSize)
                    _readBufferSize = 0;
                else
                {
                    Array.Copy(_readBuffer, position, _readBuffer, 0, _readBufferSize - position);
                    _readBufferSize -= position;
                }
            }
        }

        private void HandleConnectionLost()
        {
            // :(
            Logger.Write("[BYE] 连接丢失. :(");
            CurrentZone?.RemoveClient(this);
            IsClosed = true;
        }

        public void SendPacket(byte[] blob, string name = null)
        {
            var typeA = blob[4];
            var typeB = blob[5];
            var flags1 = blob[6];
            var flags2 = blob[7];
            string sendName;

            if (name != null)
            {
                sendName = $"0x{typeA:X2} - 0x{typeB:X2} ({name})";
            }
            else
            {
                sendName = $"0x{typeA:X2} - 0x{typeB:X2}";
            }

            Logger.Write($"[接收] 数据包 {sendName} (Flags {(PacketFlags)flags1}) ({blob.Length} 字节)");

            if (Logger.VerbosePackets)
            {
                var info = string.Format("[接收] 0x{0:X2} - 0x{1:X2} 数据包:", typeA, typeB);
                Logger.WriteHex(info, blob);
            }

            LogPacket(false, typeA, typeB, flags1, flags2, blob);

            OutputArc4?.TransformBlock(blob, 0, blob.Length, blob, 0);

            try
            {
                Socket.Write(blob);
            }
            catch (Exception ex)
            {
                Logger.WriteException("发送数据包错误", ex);
            }
        }

        public void SendPacket(byte typeA, byte typeB, byte flags, byte[] data, string name = null)
        {
            var packet = new byte[8 + data.Length];

            // TODO: Use BinaryWriter here maybe?
            var dataLen = (uint)data.Length + 8;
            packet[0] = (byte)(dataLen & 0xFF);
            packet[1] = (byte)((dataLen >> 8) & 0xFF);
            packet[2] = (byte)((dataLen >> 16) & 0xFF);
            packet[3] = (byte)((dataLen >> 24) & 0xFF);
            packet[4] = typeA;
            packet[5] = typeB;
            packet[6] = flags;
            packet[7] = 0;

            Array.Copy(data, 0, packet, 8, data.Length);

            SendPacket(packet, name);
        }

        public void SendPacket(Packet packet)
        {
            var h = packet.GetHeader();
            SendPacket(h.Type, h.Subtype, h.Flags1, packet.Build(), packet.GetType().Name);
        }

        private void HandlePacket(byte typeA, byte typeB, byte flags1, byte flags2, byte[] data, uint position,
            uint size)
        {
            var handler = PacketHandlers.GetHandlerFor(typeA, typeB);
            string packetName;
            if (handler != null)
            {
                packetName = $"0x{typeA:X2} - 0x{typeB:X2} ({handler.GetType().Name})";
            }
            else
            {
                packetName = $"0x{typeA:X2} - 0x{typeB:X2}";
            }
            Logger.Write($"[发送] 数据包 {packetName} (Flags {(PacketFlags)flags1}) ({size + 8} 字节)");
            var packet = new byte[size];
            Array.Copy(data, position, packet, 0, size);

            if (Logger.VerbosePackets && size > 0) // TODO: This is trimming too far?
            {
                var dataTrimmed = new byte[size];
                for (var i = 0; i < size; i++)
                    dataTrimmed[i] = data[i];

                var info = string.Format("[发送] 0x{0:X2} - 0x{1:X2} 数据包:", typeA, typeB);
                Logger.WriteHex(info, dataTrimmed);
            }

            LogPacket(true, typeA, typeB, flags1, flags2, packet);

            if (handler != null)
                handler.HandlePacket(this, flags1, packet, 0, size);
            else
            {
                Logger.WriteWarning("[未解析] 0x{0:X2} - 0x{1:X2} (未解析数据包) (Flags {2}) ({3} 字节)", typeA,
                    typeB, (PacketFlags)flags1, size);
                LogUnkClientPacket(typeA, typeB, flags1, flags2, packet);
            }

            // throw new NotImplementedException();
        }

        private void LogPacket(bool fromClient, byte typeA, byte typeB, byte flags1, byte flags2, byte[] packet)
        {
            // Check for and create packets directory if it doesn't exist
            var packetPath = string.Format(
                "packets/{0}/0x{1:X2} - 0x{2:X2}"
                , _server.StartTime.ToShortDateString().Replace('/', '-')
                , typeA, typeB
            );

            if (!Directory.Exists(packetPath))
                Directory.CreateDirectory(packetPath);

            var filename = string.Format("{0}/0x{1:X2}-0x{2:X2}-{3}.bin"
                , packetPath
                , typeA, typeB
                //, _packetId++
                , fromClient ? "C" : "S"
                );

            using (var stream = File.OpenWrite(filename))
            {
                if (fromClient)
                {
                    stream.WriteByte(typeA);
                    stream.WriteByte(typeB);
                    stream.WriteByte(flags1);
                    stream.WriteByte(flags2);
                }
                stream.Write(packet, 0, packet.Length);
            }
        }

        private void LogUnkClientPacket(byte typeA, byte typeB, byte flags1, byte flags2, byte[] packet)
        {
            // Check for and create packets directory if it doesn't exist
            var packetPath = string.Format(
                "UnkClientPackets/{0}/0x{1:X2} - 0x{2:X2}"
                , _server.StartTime.ToShortDateString().Replace('/', '-')
                , typeA, typeB
            );
            if (!Directory.Exists(packetPath))
                Directory.CreateDirectory(packetPath);

            var filename = string.Format("{0}/0x{1:X2}-0x{2:X2}-{3}.bin"
                , packetPath
                , typeA, typeB
                //, _packetId++
                , "C-unk"
                );

            using (var stream = File.OpenWrite(filename))
            {
                stream.WriteByte(typeA);
                stream.WriteByte(typeB);
                stream.WriteByte(flags1);
                stream.WriteByte(flags2);
                stream.Write(packet, 0, packet.Length);
            }
        }
    }
}