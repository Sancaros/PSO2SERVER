using System;
using System.IO;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Database;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x06)]
    public class DeleteCharacter : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            var reader = new PacketReader(data);
            var id = reader.ReadInt32();

            Logger.Write("[CHR] {0} 正在删除ID {1} 的角色", context.User.Username, id);

            // Delete Character
            using (var db = new ServerEf())
            {

                foreach (var character in db.Characters)
                    if (character.CharacterId == id)
                    {
                        db.Characters.Remove(character);
                        db.ChangeTracker.DetectChanges();
                        break;
                    }

                // Detect the deletion and save the Database
                if (db.ChangeTracker.HasChanges())
                    db.SaveChanges();
            }

            // Disconnect for now
            // TODO: What do we do after a deletion?
            context.Socket.Close();
        }

        #endregion
    }

}
