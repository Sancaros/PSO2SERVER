using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static PSO2SERVER.Models.PSOPalette;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class FullPaletteInfoPacket : Packet
    {
        private Palette Palette = Palette.Create();

        public FullPaletteInfoPacket(Palette palette)
        {
            this.Palette = palette;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            Palette.WriteToStream(pkt);
            byte[] byteArray = pkt.ToArray();

            var info = string.Format("[-->] 发送的数据 (hex): {0} 字节", byteArray.Length);
            Logger.WriteHex(info, byteArray);

            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x21, 0x03, PacketFlags.None);
        }

        #endregion
    }
}