using PSO2SERVER.Database;
using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
                    .OrderBy(o => o.Character_ID) // TODO: Order by last played
                    .Select(s => s);

                writer.Write((uint)chars.Count()); // Number of characters

                for (var i = 0; i < 0x4; i++) // Whatever this is
                    writer.Write((byte)0);

                foreach (var ch in chars)
                {
                    writer.Write((uint)ch.Character_ID);
                    writer.Write((uint)_PlayerId);

                    for (var i = 0; i < 0x10; i++)
                        writer.Write((byte)0);

                    writer.WriteFixedLengthUtf16(ch.Name, 16);
                    //Logger.WriteInternal("[CHR] 新增名为 {0} 的新角色.", ch.Name);
                    writer.Write((uint)0);

                    writer.WriteStruct(ch.Looks); // Note: Pre-Episode 4 created looks doesn't seem to work anymore
                    writer.WriteStruct(ch.Jobs);

                    for (var i = 0; i < 0x94; i++)
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