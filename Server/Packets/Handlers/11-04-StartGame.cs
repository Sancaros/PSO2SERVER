using PSO2SERVER.Database;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Party;
using System.Linq;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x04)]
    public class StartGame : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            var reader = new PacketReader(data, position, size);
            var charId = reader.ReadUInt32();

            //Logger.Write("id {0}", charId);

            if (context.User == null)
                return;

            if (context.Character == null) // On character create, this is already set.
            {
                using (var db = new ServerEf())
                {
                    var character = db.Characters.Where(c => c.CharacterId == charId).First();

                    if (character == null || character.Player.PlayerId != context.User.PlayerId)
                    {
                        Logger.WriteError("数据库中未找到 {0} 角色ID {1} ({2})"
                            , context.User.Username
                            , charId
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