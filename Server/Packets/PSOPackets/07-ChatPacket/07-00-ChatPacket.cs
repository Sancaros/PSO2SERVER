using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Contexts;
using System.Text;
using static PSO2SERVER.Packets.Handlers.ChatHandler;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class ChatPacket : Packet
    {
        public enum MessageChannel : byte
        {
            // Map channel.
            Map = 0,

            // Party channel.
            Party = 1,

            // Alliance channel.
            Alliance = 2,

            // Whisper channel.
            Whisper = 3,

            // Group channel.
            Group = 4,

            // Undefined channel.
            Undefined = 0xFF
        }

        /// Sender of the message.
        public ObjectHeader Object;

        /// Message channel.
        public byte Channel;

        public byte Unk3;
        public ushort Unk4;

        // Only included if feature is enabled
#if NGS_PACKETS
    public ushort Unk5;
    public ushort Unk6;
#endif

        public string Unk7;

        /// Message.
        public string Message;

        public ChatPacket(uint id, byte channel, string message)
        {
            Object = new ObjectHeader() 
            { 
                ID = id, 
                padding = 0,
                ObjectType = ObjectType.Player,
                MapID = 0
            };

            Unk3 = 0xFF;

            Channel = channel;

            Message = message;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            pkt.WriteStruct(Object);
            pkt.Write(Channel);
            pkt.Write(Unk3);
            pkt.Write(Unk4);

            pkt.Write((byte)0x7B);
            pkt.Write((byte)0x9D);
            pkt.Write((byte)0);
            pkt.Write((byte)0);

            pkt.WriteUtf16(Message, 0x9D3F, 0x44);
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x07, 0x00, PacketFlags.PACKED | PacketFlags.OBJECT_RELATED);
        }

        #endregion
    }
}