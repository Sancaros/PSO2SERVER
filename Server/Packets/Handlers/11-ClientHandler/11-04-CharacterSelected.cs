using PSO2SERVER.Database;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Party;
using System;
using System.Linq;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x04)]
    public class CharacterSelected : PacketHandler
    {
        public struct CharacterSelectedPacket
        {
            /// <summary>
            /// Selected character ID.
            /// </summary>
            public uint CharId;
            public uint Unk1;
            public uint Unk2;
        }

        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            if (context._account == null)
                return;

            var reader = new PacketReader(data, position, size);
            var pkt = reader.ReadStruct<CharacterSelectedPacket>();

            var charId = pkt.CharId;

            Logger.Write("id {0}", charId);

            if (context.Character == null) // On character create, this is already set.
            {
                using (var db = new ServerEf())
                {
                    try
                    {
                        var character = db.Characters.FirstOrDefault(c => c.Player_ID == charId);

                        if (character == null || character.Account == null || character.Account.AccountId != context._account.AccountId)
                        {
                            Logger.WriteError("数据库中未找到 {0} 角色ID {1} ({2})"
                                , context._account.Username
                                , charId
                                , context._account.AccountId
                            );
                            context.Socket.Close();
                            return;
                        }

                        context.Character = character;
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteError("查询角色时发生异常: {0}", ex.Message);
                        context.Socket.Close();
                    }
                }
            }

            // 将客户端加入空余的队伍中
            PartyManager.Instance.CreateNewParty(context);

            // 告诉客户端切换到加载界面
            context.SendPacket(new LoadingScreenTransitionPacket());

            // TODO Set area, Set character, possibly more. See PolarisLegacy for more.
        }

        #endregion
    }

}