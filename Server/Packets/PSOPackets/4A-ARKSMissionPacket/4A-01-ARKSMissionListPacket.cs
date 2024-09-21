using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class ARKSMissionListPacket : Packet
    {
        public struct MissionListPacket
        {
            public uint Unk1; // 对应 Rust 的 u32
            public List<Mission> Missions; // 使用 List 替代 Vec
            public uint DailyUpdate; // 对应 Rust 的 u32
            public uint WeeklyUpdate; // 对应 Rust 的 u32
            public uint TierUpdate; // 对应 Rust 的 u32

            // 不使用构造函数，而是使用默认值
            public static MissionListPacket Create()
            {
                return new MissionListPacket
                {
                    Missions = new List<Mission>(),
                    Unk1 = 0,
                    DailyUpdate = 0,
                    WeeklyUpdate = 0,
                    TierUpdate = 0
                };
            }
        }

        MissionListPacket pkt {  get; set; }

        public ARKSMissionListPacket(Mission mission)
        {
            pkt = MissionListPacket.Create(); // 使用工厂方法初始化 pkt
            // 初始化 pkt 或添加初始任务
            pkt.Missions.Add(mission);
        }

        // 增加任务
        public void AddMission(Mission mission)
        {
            pkt.Missions.Add(mission);
        }

        // 删除任务
        public bool RemoveMission(Mission mission)
        {
            return pkt.Missions.Remove(mission);
        }

        // 更新任务
        public void UpdateMission(int index, Mission mission)
        {
            if (index >= 0 && index < pkt.Missions.Count)
            {
                pkt.Missions[index] = mission;
            }
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var writer = new PacketWriter();
            return writer.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x4A, 0x01, PacketFlags.None);
        }

        #endregion
    }
}