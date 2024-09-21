using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x04, 0x3C)]
    public class ActionUpdateHandler : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            PacketReader reader = new PacketReader(data);
            reader.ReadStruct<ObjectHeader>(); // Read the blank
            ObjectHeader actor = reader.ReadStruct<ObjectHeader>(); // Read the actor
            byte[] rest = reader.ReadBytes(32); // TODO Map this out and do stuff with it!

            foreach (var c in Server.Instance.Clients)
            {
                if (c == context || c.Character == null || c.CurrentZone != context.CurrentZone)
                    continue;
                //PacketWriter writer = new PacketWriter();
                //writer.WriteStruct(new ObjectHeader((uint)c._account.AccountId, ObjectType.Account));
                //writer.WriteStruct(actor);
                //writer.Write(rest);

                //c.SendPacket(0x4, 0x81, 0x40, writer.ToArray());
                c.SendPacket(new ActionUpdateServerPacket(c._account.AccountId, actor, rest));
            }

        }
    }
}
