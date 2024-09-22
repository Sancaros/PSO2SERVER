using System;
using PSO2SERVER.Models;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class SystemMessagePacket : Packet
    {
        public enum MessageType : UInt32
        {
            GoldenTicker = 0,
            AdminMessage,
            AdminMessageInstant,
            SystemMessage,
            GenericMessage,
            EventInformationYellow,
            EventInformationGreen,
            ImportantMessage,
            PopupMessage,
            Undefined = 0xFFFFFFFF,
        }

        private readonly string _message;
        //private string _unk;
        private readonly MessageType _type;
        //private uint _msg_num;

        public SystemMessagePacket(string message, MessageType type)
        {
            _message = message;
            _type = type;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var writer = new PacketWriter();
            writer.WriteUtf16(_message, 0x78F7, 0xA2);
            writer.Write((UInt32) _type);

            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x19, 0x01, PacketFlags.PACKED);
        }

        #endregion
    }
}