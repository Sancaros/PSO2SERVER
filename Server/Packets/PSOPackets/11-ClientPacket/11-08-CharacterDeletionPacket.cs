using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class CharacterDeletionPacket : Packet
    {
        public DeletionStatus Status { get; }

        public enum DeletionStatus : uint
        {
            /// <summary>
            /// Character has items which prevent deletion.
            /// </summary>
            UndeletableItems,

            /// <summary>
            /// Character has been scheduled for deletion.
            /// </summary>
            Success
        }

        public struct ItemId
        {
            /// <summary>
            /// Item type.
            /// </summary>
            public ushort ItemType { get; set; }

            /// <summary>
            /// Item category.
            /// </summary>
            public ushort Id { get; set; }

            /// <summary>
            /// Item ID after appraisal.
            /// </summary>
            public ushort Unk3 { get; set; }

            /// <summary>
            /// Item ID.
            /// </summary>
            public ushort SubId { get; set; }
        }

        public struct CharacterDeletePacket
        {
            /// <summary>
            /// Deletion request status.
            /// </summary>
            public DeletionStatus Status { get; set; }

            public uint Unk1 { get; set; }

            public List<ItemId> Unk2 { get; set; }

            public List<ItemId> Unk3 { get; set; }

            public List<ItemId> Unk4 { get; set; }

            public List<ItemId> Unk5 { get; set; }

            public List<ItemId> Unk6 { get; set; }
        }

        public CharacterDeletionPacket(DeletionStatus status)
        {
            Status = status;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            pkt.Write((uint)Status);
            pkt.Write((uint)0);
            pkt.WriteMagic(0, 0x33D4, 0xC4);
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x11, 0x08, PacketFlags.PACKED);
        }

        #endregion
    }
}