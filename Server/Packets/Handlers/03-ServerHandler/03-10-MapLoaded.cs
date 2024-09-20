using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x03, 0x10)]
    public class MapLoaded : PacketHandler
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MapLoadedPacket
        {
            /// Loaded zone object.
            public ObjectHeader MapObject;

            /// Unknown data, 32 bytes.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public byte[] Unk;

            // 可选构造函数
            public MapLoadedPacket(ObjectHeader mapObject)
            {
                MapObject = mapObject;
                Unk = new byte[0x20];
            }
        }

        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            if (context._account == null || context.Character == null)
                return;

            context.SendPacket(new LoadingScreenRemovePacket());
        }

        #endregion
    }
}
