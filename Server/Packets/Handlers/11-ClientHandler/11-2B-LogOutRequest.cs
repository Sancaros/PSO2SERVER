using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2SERVER.Packets.Handlers
{

    [PacketHandlerAttr(0x11, 0x2B)]
    public class LogOutRequest : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            context.Socket.Close();
        }

        #endregion
    }

}
