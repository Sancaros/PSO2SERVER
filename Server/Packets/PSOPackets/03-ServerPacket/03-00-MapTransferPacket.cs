using PSO2SERVER.Models;
using PSO2SERVER.Zone;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static PSO2SERVER.Zone.Map;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class MapTransferPacket : Packet
    {
        private readonly Map _map;
        private readonly int _playerid;

        public MapTransferPacket(Map map, int PlayerId)
        {
            _map = map;
            _playerid = PlayerId;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            PacketWriter writer = new PacketWriter();
            writer.WriteStruct(new ObjectHeader(3, EntityType.Map));
            writer.WriteStruct(new ObjectHeader((uint)_playerid, EntityType.Player));
            writer.Write(0x1); // 8 Zeros
            writer.Write(0); // 8 Zeros
            writer.Write(~(uint)_map.Type); // F4 FF FF FF
            writer.Write(_map.MapID); // Map ID maybe
            writer.Write((uint)_map.Flags);
            writer.Write(_map.GenerationArgs.seed); // 81 8F E6 19 (Maybe seed)
            writer.Write(_map.VariantID); // Randomgen enable / disable maybe
            writer.Write(_map.GenerationArgs.xsize); // X Size
            writer.Write(_map.GenerationArgs.ysize); // Y Size
            writer.Write(1);
            writer.Write(1);
            writer.Write(~0); // FF FF FF FF FF FF FF FF
            writer.Write(0x301);

            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x03, 0x00, PacketFlags.None);
        }

        #endregion
    }
}