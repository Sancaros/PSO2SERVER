using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class CreateCharacterOneResponsePacket : Packet
    {
        public struct CreateCharacter1ResponsePacket
        {
            /// <summary>
            /// Creation status.
            /// </summary>
            public uint Status;

            public uint Unk2;

            public uint UsedSmth;

            /// <summary>
            /// Required AC to buy a character creation pass.
            /// </summary>
            public uint ReqAc;
            public CreateCharacter1ResponsePacket(uint status, uint unk2, uint usedSmth, uint reqAc) : this()
            {
                Status = status;
                Unk2 = unk2;
                UsedSmth = usedSmth;
                ReqAc = reqAc;
            }
        }

        public CreateCharacterOneResponsePacket()
        {
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            pkt.WriteStruct(new CreateCharacter1ResponsePacket(0, 0, 0, 0));
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x11, 0x42, PacketFlags.None);
        }

        #endregion
    }
}