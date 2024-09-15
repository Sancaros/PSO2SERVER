using System;
using System.Runtime.InteropServices;

using PSO2SERVER.Models;

namespace PSO2SERVER.Packets.PSOPackets
{
    class QuestStartPacket : Packet
    {
        QuestDefiniton data;
        QuestDifficultyPacket.QuestDifficulty difficulty;

        public QuestStartPacket(QuestDefiniton data, QuestDifficultyPacket.QuestDifficulty difficulty)
        {
            this.data = data;
            this.difficulty = difficulty;
        }

        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();

            writer.Write(0x753A); // Unknown
            writer.Write((int)0); // Unknown
            writer.WriteStruct<QuestDefiniton>(data);
            writer.WriteStruct<QuestDifficultyPacket.QuestDifficulty>(difficulty);

            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x0E, 0x31);
        }
    }
}
