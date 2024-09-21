using System;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;
using static PSO2SERVER.Models.Flags;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x23, 0x02)]
    class SetFlagHandler : PacketHandler
    {
        public struct SetFlagPacket
        {
            /// Flag type.
            public FlagType flag_type { get; set; }
            /// Flag ID.
            public uint id { get; set; }
            /// Flag value.
            public uint value { get; set; }
        }

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            if (context._account == null || context.Character == null)
                return;

            //var info = string.Format("[<--] 接收到的数据 (hex): {0} 字节", data.Length);
            //Logger.WriteHex(info, data);

            var reader = new PacketReader(data);
            var flag = reader.ReadStruct<SetFlagPacket>();

            if(context._flags == null)
                context._flags = new Flags();

            if (flag.flag_type == FlagType.Account)
                context._flags.Set((int)flag.id, (byte)flag.value);
        }
    }
}
