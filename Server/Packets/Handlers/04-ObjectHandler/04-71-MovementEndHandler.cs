using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Packets.Handlers
{

    [PacketHandlerAttr(0x04, 0x71)]
    public class MovementEndHandler : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            PacketReader reader = new PacketReader(data);
            MovementPacket.FullMovementData movData = reader.ReadStruct<MovementPacket.FullMovementData>();

            if (movData.entity1.ID == 0 && movData.entity2.ID != 0)
                movData.entity1 = movData.entity2;


            movData.timestamp = 0;
            // This could be simplified
            PacketWriter writer = new PacketWriter();
            writer.WriteStruct(movData);

            //Logger.WriteInternal("[移动] 玩家 {0} 停止移动 (坐标:{1}, {2}, {3})", context.Character.Name,
            //    Helper.FloatFromHalfPrecision(movData.currentPos.x), Helper.FloatFromHalfPrecision(movData.currentPos.y),
            //    Helper.FloatFromHalfPrecision(movData.currentPos.z));

            foreach (var c in Server.Instance.Clients)
            {
                if (c == context || c.Character == null || c.CurrentZone != context.CurrentZone)
                    continue;

                c.SendPacket(0x04, 0x71, 0x40, writer.ToArray());
            }
        }

        #endregion
    }

}
