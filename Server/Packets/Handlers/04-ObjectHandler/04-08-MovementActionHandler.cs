using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x04, 0x08)]
    public class MovementActionHandler : PacketHandler
    {
        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            PacketReader reader = new PacketReader(data);
            reader.ReadStruct<ObjectHeader>(); // Skip blank entity header.
            var preformer = reader.ReadStruct<ObjectHeader>(); // Preformer
            byte[] preData = reader.ReadBytes(40);
            string command = reader.ReadAscii(0x922D, 0x45);
            byte[] rest = reader.ReadBytes(4);
            uint thingCount = reader.ReadMagic(0x922D, 0x45);
            byte[] things;
            PacketWriter thingWriter = new PacketWriter();
            for (int i = 0; i < thingCount; i++)
            {
                thingWriter.Write(reader.ReadBytes(4));
            }
            things = thingWriter.ToArray();
            byte[] final = reader.ReadBytes(4);


            //Logger.WriteInternal("[动作] {0} 发送动作 {1}", context.Character.Name, command);

            foreach (var c in Server.Instance.Clients)
            {
                if (c == context || c.Character == null || c.CurrentZone != context.CurrentZone)
                    continue;
                //PacketWriter output = new PacketWriter();
                //output.WriteStruct(new ObjectHeader((uint)context._account.AccountId, EntityType.Account));
                //output.WriteStruct(preformer);
                //output.Write(preData);
                //output.WriteAscii(command, 0x4315, 0x7A);
                //output.Write(rest);
                //output.WriteMagic(thingCount, 0x4315, 0x7A);
                //output.Write(things);
                //output.Write(final);

                //c.SendPacket(0x4, 0x80, 0x44, output.ToArray());

                c.SendPacket(new MovementActionServerPacket(context._account.AccountId, preformer, preData
                    , command, rest, thingCount, things, final));
            }
        }
    }

}
