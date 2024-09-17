using PSO2SERVER.Models;
using System;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class CreateCharacterTwoResponsePacket : Packet
    {
        private readonly uint _referral_flag;

        public struct CreateCharacter2ResponsePacket
        {
            public uint ReferralFlag { get; }

            public CreateCharacter2ResponsePacket(uint referral_flag)
            {
                ReferralFlag = referral_flag;
            }
        }

        public CreateCharacterTwoResponsePacket(uint referral_flag)
        {
            _referral_flag = referral_flag;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            pkt.WriteStruct(new CreateCharacter2ResponsePacket(_referral_flag));
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x11, 0x55, PacketFlags.None);
        }

        #endregion
    }
}