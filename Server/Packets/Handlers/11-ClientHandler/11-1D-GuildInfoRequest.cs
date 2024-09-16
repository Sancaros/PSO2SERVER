using PSO2SERVER.Packets.PSOPackets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2SERVER.Packets.Handlers
{

    [PacketHandlerAttr(0x11, 0x1D)]
    public class GuildInfoRequest : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            var reader = new PacketReader(data);
            reader.BaseStream.Seek(0xC, SeekOrigin.Begin);
            var id = reader.ReadUInt32();

            foreach (var client in ServerApp.Instance.Server.Clients)
            {
                if (client.Character.CharacterId == id)
                {
                    var infoPacket = new GuildInfoPacket(context.Character);
                    context.SendPacket(infoPacket);
                    Logger.Write("[NFO] Sent guild info to " + client.Character.CharacterId);
                    break;
                }
            }
        }

        #endregion
    }
}

