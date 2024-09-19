using PSO2SERVER.Database;
using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class CharacterListPacket : Packet
    {

        // Ninji note: This packet may be followed by extra data,
        // after a fixed-length array of character data structures.
        // Needs more investigation at some point.
        // --- 
        // CK note: Extra data is likely current equipment, playtime, etc.
        // All of that data is currently unaccounted for at the moment.
        //忍者注意:这个包后面可能有额外的数据，
        //在固定长度的字符数据结构数组之后。
        //需要更多的调查。
        // ---
        // CK注:额外的数据可能是当前设备，游戏时间等。
        //所有这些数据目前都是未知的。

        private int _PlayerId;

        /// <summary>
        /// Available characters.
        /// </summary>
        public Character[] Characters { get; set; }

        //public Item[][] EquippedItems { get; set; } = new Item[10][];

        /// <summary>
        /// Character play times.
        /// </summary>
        public uint[] PlayTimes { get; set; } = new uint[30];

        /// <summary>
        /// Character deletion flags (flag, deletion timestamp).
        /// </summary>
        public (uint Flag, uint Timestamp)[] DeletionFlags { get; set; } = new (uint, uint)[30];

        /// <summary>
        /// Character ship transfer flags.
        /// </summary>
        public (uint Flag, uint Transfer)[] TransferFlags { get; set; } = new (uint, uint)[30];

        /// <summary>
        /// Account accessory flag (?).
        /// </summary>
        public ushort AccountAccessory { get; set; }

        /// <summary>
        /// Login survey flag.
        /// </summary>
        public uint LoginSurvey { get; set; }

        /// <summary>
        /// Ad flag (on global 12 star unit ad).
        /// </summary>
        public uint Ad { get; set; }

        public CharacterListPacket(int PlayerId)
        {
            _PlayerId = PlayerId;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var writer = new PacketWriter();

            using (var db = new ServerEf())
            {
                var chars = db.Characters
                    .Where(w => w.Player.PlayerId == _PlayerId)
                    .OrderBy(o => o.CharacterId) // TODO: Order by last played
                    .Select(s => s);

                writer.Write((uint)chars.Count()); // Number of characters

                for (var i = 0; i < 0x4; i++) // Whatever this is
                    writer.Write((byte)0);

                foreach (var ch in chars)
                {
                    writer.Write(ch.CharacterId);
                    writer.Write(ch.player_id);
                    writer.Write(ch.unk1);
                    writer.Write(ch.voice_type);
                    writer.Write(ch.unk2);
                    writer.Write(ch.voice_pitch);
                    writer.WriteFixedLengthUtf16(ch.Name, 16);
                    writer.Write((uint)0);
                    writer.WriteStruct(ch.Looks);
                    writer.WriteStruct(ch.Jobs);

                    for (var i = 0; i < 0x90; i++)
                        writer.Write((byte)0);
                }
            }

            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x11, 0x03, PacketFlags.None);
        }

        #endregion
    }
}