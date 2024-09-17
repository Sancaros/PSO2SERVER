using System;
using System.IO;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Database;

// This file is to hold all packet handlers that require no logic to respond to, or require less than 5 lines of logic.

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x54)]
    public class CreateCharacterTwo : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            //var writer = new PacketWriter();
            //writer.Write((uint) 0);

            //context.SendPacket(0x11, 0x55, 0x0, writer.ToArray());

            context.SendPacket(new CreateCharacterTwoResponsePacket(0));
        }

        #endregion
    }
}