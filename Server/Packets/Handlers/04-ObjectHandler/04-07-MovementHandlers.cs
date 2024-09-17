using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;
using System;
using System.Runtime.InteropServices;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x04, 0x07)]
    public class MovementHandler : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            PacketReader reader = new PacketReader(data);
            // This packet is "Compressed" basically.
            reader.ReadBytes(6); // Get past the junk
            // For simplicity's sake, read the 3 flag bytes into a big int
            byte[] flagBytes = reader.ReadBytes(3);
            uint dataFlags = flagBytes[0];
            dataFlags |= (uint)(flagBytes[1] << 8);
            dataFlags |= (uint)(flagBytes[2] << 16);

            PackedData theFlags = (PackedData)dataFlags;

            // Debug
            //Logger.WriteInternal("[移动] Movement 数据包来自 {0} 包含 {1} 数据.", context.Character.Name, theFlags);

            // TODO: Maybe do this better someday
            MovementPacket.FullMovementData dstData = new MovementPacket.FullMovementData();

            if (theFlags.HasFlag(PackedData.ENT1_ID))
            {
                dstData.entity1.ID = (uint)reader.ReadUInt64();
            }
            if (theFlags.HasFlag(PackedData.ENT1_TYPE))
            {
                dstData.entity1.EntityType = (EntityType)reader.ReadUInt16();
            }
            if (theFlags.HasFlag(PackedData.ENT1_A))
            {
                dstData.entity1.Unknown_A = reader.ReadUInt16();
            }
            if (theFlags.HasFlag(PackedData.ENT2_ID))
            {
                dstData.entity1.ID = (uint)reader.ReadUInt64();
            }
            if (theFlags.HasFlag(PackedData.ENT2_TYPE))
            {
                dstData.entity1.EntityType = (EntityType)reader.ReadUInt16();
            }
            if (theFlags.HasFlag(PackedData.ENT2_A))
            {
                dstData.entity1.Unknown_A = reader.ReadUInt16();
            }
            if (theFlags.HasFlag(PackedData.TIMESTAMP))
            {
                dstData.timestamp = reader.ReadUInt32();
                context.MovementTimestamp = dstData.timestamp;
            }
            if (theFlags.HasFlag(PackedData.ROT_X))
            {
                dstData.rotation.x = reader.ReadUInt16();
                context.CurrentLocation.RotX = Helper.FloatFromHalfPrecision(dstData.rotation.x);
            }
            if (theFlags.HasFlag(PackedData.ROT_Y))
            {
                dstData.rotation.y = reader.ReadUInt16();
                context.CurrentLocation.RotY = Helper.FloatFromHalfPrecision(dstData.rotation.y);
            }
            if (theFlags.HasFlag(PackedData.ROT_Z))
            {
                dstData.rotation.z = reader.ReadUInt16();
                context.CurrentLocation.RotZ = Helper.FloatFromHalfPrecision(dstData.rotation.z);
            }
            if (theFlags.HasFlag(PackedData.ROT_W))
            {
                dstData.rotation.w = reader.ReadUInt16();
                context.CurrentLocation.RotW = Helper.FloatFromHalfPrecision(dstData.rotation.w);
            }
            if (theFlags.HasFlag(PackedData.CUR_X))
            {
                dstData.currentPos.x = reader.ReadUInt16();
                context.CurrentLocation.PosX = Helper.FloatFromHalfPrecision(dstData.currentPos.x);
            }
            if (theFlags.HasFlag(PackedData.CUR_Y))
            {
                dstData.currentPos.y = reader.ReadUInt16();
                context.CurrentLocation.PosY = Helper.FloatFromHalfPrecision(dstData.currentPos.y);
            }
            if (theFlags.HasFlag(PackedData.CUR_Z))
            {
                dstData.currentPos.z = reader.ReadUInt16();
                context.CurrentLocation.PosZ = Helper.FloatFromHalfPrecision(dstData.currentPos.z);
            }
            if (theFlags.HasFlag(PackedData.UNKNOWN4))
            {
                dstData.Unknown2 = reader.ReadUInt16();
            }
            if (theFlags.HasFlag(PackedData.UNK_X))
            {
                dstData.unknownPos.x = reader.ReadUInt16();
                context.LastLocation.PosX = Helper.FloatFromHalfPrecision(dstData.unknownPos.x);
            }
            if (theFlags.HasFlag(PackedData.UNK_Y))
            {
                dstData.unknownPos.y = reader.ReadUInt16();
                context.LastLocation.PosY = Helper.FloatFromHalfPrecision(dstData.unknownPos.y);
            }
            if (theFlags.HasFlag(PackedData.UNK_Z))
            {
                dstData.unknownPos.z = reader.ReadUInt16();
                context.LastLocation.PosZ = Helper.FloatFromHalfPrecision(dstData.unknownPos.z);
            }
            if (theFlags.HasFlag(PackedData.UNKNOWN5))
            {
                dstData.Unknown3 = reader.ReadUInt16();
            }
            if (theFlags.HasFlag(PackedData.UNKNOWN6))
            {
                if (theFlags.HasFlag(PackedData.UNKNOWN7))
                {
                    dstData.Unknown4 = reader.ReadByte();
                }
                else
                {
                    dstData.Unknown4 = reader.ReadUInt32();
                }
            }


            //Logger.WriteInternal("[移动] 玩家 {0} 移动中 (坐标: X{1}, Y{2}, Z{3})", context.Character.Name, context.CurrentLocation.PosX,
                //context.CurrentLocation.PosY, context.CurrentLocation.PosZ);

            foreach (var c in Server.Instance.Clients)
            {
                if (c.Character == null || c == context || c.CurrentZone != context.CurrentZone)
                    continue;

                c.SendPacket(0x04, 0x07, flags, data);
                //c.SendPacket(new MovementPacket(dstData));
            }
        }

        #endregion
    }

    [Flags]
    public enum PackedData : Int32
    {
        ENT1_ID = 1,
        ENT1_TYPE = 2,
        ENT1_A = 4,
        ENT2_ID = 8,
        ENT2_TYPE = 0x10,
        ENT2_A = 0x20,
        TIMESTAMP = 0x40,
        ROT_X = 0x80,
        ROT_Y = 0x100,
        ROT_Z = 0x200,
        ROT_W = 0x400,
        CUR_X = 0x800,
        CUR_Y = 0x1000,
        CUR_Z = 0x2000,
        UNKNOWN4 = 0x4000,
        UNK_X = 0x8000,
        UNK_Y = 0x10000,
        UNK_Z = 0x20000,
        UNKNOWN5 = 0x40000,
        UNKNOWN6 = 0x80000,
        UNKNOWN7 = 0x100000
    }


}