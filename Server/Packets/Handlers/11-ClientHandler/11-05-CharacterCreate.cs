using System.IO;
using System.Data.Entity;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Database;
using System.Linq;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x05)]
    public class CharacterCreate : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            if (context._account == null)
                return;



            var reader = new PacketReader(data, position, size);
            var info = string.Format("[<--] 接收到的数据 (hex): {0} 字节", data.Length);
            Logger.WriteHex(info, data);

            reader.ReadBytes(12); // 12 unknown bytes
            reader.ReadByte(); // VoiceType
            reader.ReadBytes(5); // 5 unknown bytes
            reader.ReadUInt16(); // VoiceData
            var name = reader.ReadFixedLengthUtf16(16);

            reader.BaseStream.Seek(0x4, SeekOrigin.Current); // Padding
            var looks = reader.ReadStruct<Character.LooksParam>();
            var jobs = reader.ReadStruct<Character.JobParam>();

            Logger.WriteInternal("[CHR] {0} 创建了名为 {1} 的新角色.", context._account.Username, name);
            var newCharacter = new Character
            {
                Name = name,
                Jobs = jobs,
                Looks = looks,
                Account = context._account
            };

            // Add to database
            using (var db = new ServerEf())
            {
                // Check if any characters exist for this player
                var existingCharacters = db.Characters.Where(c => c.Account.AccountId == context._account.AccountId).ToList();
                if (existingCharacters.Count > 0)
                {
                    // Increment ID if characters already exist
                    newCharacter.Character_ID = existingCharacters.Max(c => c.Character_ID) + 1;
                }
                else
                {
                    // Start with ID 1 if no characters exist
                    newCharacter.Character_ID = 1;
                }

                newCharacter.Player_ID = context._account.AccountId;

                //Logger.Write("newCharacter.CharacterId {0} {1}", newCharacter.CharacterId, context._account.AccountId);

                db.Characters.Add(newCharacter);
                db.Entry(newCharacter.Account).State = EntityState.Modified;
                db.SaveChanges();
            }

            // Assign character to player
            context.Character = newCharacter;

            // Set Account ID
            context.SendPacket(new CharacterCreateResponsePacket(CharacterCreateResponsePacket.CharacterCreationStatus.Success, (uint)context._account.AccountId));

            // Spawn
            context.SendPacket(new NoPayloadPacket(0x11, 0x3E));
        }

        #endregion
    }
}