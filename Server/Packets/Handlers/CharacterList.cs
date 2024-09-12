using PSO2SERVER.Database;
using System.Linq;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x02)]
    public class CharacterList : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            if (context.User == null)
                return;

            var writer = new PacketWriter();

            using (var db = new ServerEf())
            {
                var chars = db.Characters
                    .Where(w => w.Player.PlayerId == context.User.PlayerId)
                    .OrderBy(o => o.CharacterId) // TODO: Order by last played
                    .Select(s => s);

                writer.Write((uint)chars.Count()); // Number of characters

                for (var i = 0; i < 0x4; i++) // Whatever this is
                    writer.Write((byte)0);

                foreach (var ch in chars)
                {
                    writer.Write((uint)ch.CharacterId);
                    writer.Write((uint)context.User.PlayerId);

                    for (var i = 0; i < 0x10; i++)
                        writer.Write((byte)0);

                    writer.WriteFixedLengthUtf16(ch.Name, 16);
                    writer.Write((uint)0);

                    writer.WriteStruct(ch.Looks); // Note: Pre-Episode 4 created looks doesn't seem to work anymore
                    writer.WriteStruct(ch.Jobs);

                    for (var i = 0; i < 0xFC; i++)
                        writer.Write((byte)0);
                }
            }

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

            context.SendPacket(0x11, 0x03, 0, writer.ToArray());
        }

        #endregion
    }
}