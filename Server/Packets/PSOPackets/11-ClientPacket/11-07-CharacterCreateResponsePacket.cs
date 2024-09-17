using Mysqlx;
using PSO2SERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace PSO2SERVER.Packets.PSOPackets
{
    public class CharacterCreateResponsePacket : Packet
    {
        public enum CharacterCreationStatus
        {
            /// 角色创建成功.
            Success,
            /// 显示数据空白错误信息.
            EmptyError,
            /// 角色数量上限错误.
            LimitReached,
            /// AC不足无法创建角色.
            NoAC,
            /// 常规系统报错.
            SystemError,
        }

        private CharacterCreationStatus status;
        private uint userid;

        public CharacterCreateResponsePacket(CharacterCreationStatus status, uint userid)
        {
            this.status = status;
            this.userid = userid;
        }

        #region implemented abstract members of Packet

        public override byte[] Build()
        {
            var pkt = new PacketWriter();
            pkt.Write((int)status);
            pkt.Write(userid);
            return pkt.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x11, 0x07, PacketFlags.None);
        }

        #endregion
    }
}