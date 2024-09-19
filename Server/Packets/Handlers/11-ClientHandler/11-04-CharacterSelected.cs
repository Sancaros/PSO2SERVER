using PSO2SERVER.Database;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Party;
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
            var reader = new PacketReader(data, position, size);
            var pkt = reader.ReadStruct<CharacterSelectedPacket>();

            //Logger.Write("id {0}", charId);

            if (context.User == null)
                return;

            if (context.Character == null) // On character create, this is already set.
            {
                using (var db = new ServerEf())
                {
                    var character = db.Characters.Where(c => c.CharacterId == pkt.CharId).First();

                    if (character == null || character.Player.PlayerId != context.User.PlayerId)
                    {
                        Logger.WriteError("数据库中未找到 {0} 角色ID {1} ({2})"
                            , context.User.Username
                            , pkt.CharId
                            , context.User.PlayerId
                            );
                        context.Socket.Close();
                        return;
                    }

                    context.Character = character;
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