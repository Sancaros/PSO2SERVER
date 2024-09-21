using System;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x19, 0x1C)]
    class Unk191CPacketHandler : PacketHandler
    {
        public struct Unk191CPacket
        {
            public uint Unk1; // 对应 Rust 的 u32
            public uint Unk2; // 对应 Rust 的 u32
            public uint Unk3; // 对应 Rust 的 u32
            public uint Unk4; // 对应 Rust 的 u32
            public float Unk5; // 对应 Rust 的 f32
            public float Unk6; // 对应 Rust 的 f32
            public float Unk7; // 对应 Rust 的 f32

            // 可选：可以添加构造函数来初始化结构体
            public Unk191CPacket(uint unk1, uint unk2, uint unk3, uint unk4, float unk5, float unk6, float unk7)
            {
                Unk1 = unk1;
                Unk2 = unk2;
                Unk3 = unk3;
                Unk4 = unk4;
                Unk5 = unk5;
                Unk6 = unk6;
                Unk7 = unk7;
            }
        }

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            var info = string.Format("[<--] 接收到的数据 (hex): {0} 字节", data.Length);
            Logger.WriteHex(info, data);
        }
    }
}
