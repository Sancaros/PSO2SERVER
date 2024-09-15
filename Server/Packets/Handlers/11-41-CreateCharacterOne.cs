using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x41)]
    public class CreateCharacterOne : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            var writer = new PacketWriter();
            writer.Write((uint)0);
            writer.Write((uint)0);
            writer.Write((uint)0);
            writer.Write((uint)0);

            context.SendPacket(0x11, 0x42, 0x0, writer.ToArray());
        }

        #endregion
    }

}
