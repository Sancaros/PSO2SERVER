using PSO2SERVER.Database;
using PSO2SERVER.Packets.PSOPackets;
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

            context.SendPacket(new CharacterListPacket(context.User.PlayerId));
        }

        #endregion
    }
}