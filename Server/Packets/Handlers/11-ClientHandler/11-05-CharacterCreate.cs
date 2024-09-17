﻿using System.IO;
using System.Data.Entity;
using PSO2SERVER.Models;
using PSO2SERVER.Packets.PSOPackets;
using PSO2SERVER.Database;
using System.Linq;
using System;
using System.Runtime.InteropServices;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x05)]
    public class CharacterCreate : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            if (context.User == null)
                return;

            var reader = new PacketReader(data, position, size);
            //var info = string.Format("[<--] 接收到的数据 (hex): ");
            //Logger.WriteHex(info, data);
            var setting = reader.ReadStruct<Character.CharParam>();

            var name = reader.ReadFixedLengthUtf16(16);//玩家名称 宽字符

            //reader.BaseStream.Seek(0x04, SeekOrigin.Current); // Padding
            var looks = reader.ReadStruct<Character.LooksParam>();
            var jobs = reader.ReadStruct<Character.JobParam>();

            //Logger.WriteInternal("[CHR] {0} 创建了名为 {1} 的新角色.", context.User.Username, name);
            var newCharacter = new Character
            {
                Name = name,
                Jobs = jobs,
                Looks = looks,
                Player = context.User
            };

            // Add to database
            using (var db = new ServerEf())
            {
                // Check if any characters exist for this player
                var existingCharacters = db.Characters.Where(c => c.Player.PlayerId == context.User.PlayerId).ToList();
                if (existingCharacters.Count > 0)
                {
                    // Increment ID if characters already exist
                    newCharacter.CharacterId = existingCharacters.Max(c => c.CharacterId) + 1;
                }
                else
                {
                    // Start with ID 1 if no characters exist
                    newCharacter.CharacterId = 1;
                }

                newCharacter.player_id = context.User.PlayerId;
                newCharacter.unk1 = setting.unk1;
                newCharacter.voice_type = setting.voice_type;
                newCharacter.unk2 = setting.unk2;
                newCharacter.voice_pitch = setting.voice_pitch;

                //Logger.Write("newCharacter.CharacterId {0} {1}", newCharacter.CharacterId, context.User.PlayerId);

                db.Characters.Add(newCharacter);
                db.Entry(newCharacter.Player).State = EntityState.Modified;
                db.SaveChanges();
            }

            // Assign character to player
            context.Character = newCharacter;

            // Set Player ID
            context.SendPacket(new CharacterCreateResponsePacket(CharacterCreateResponsePacket.CharacterCreationStatus.Success, (uint)context.User.PlayerId));

            // Spawn
            context.SendPacket(new NoPayloadPacket(0x11, 0x3E));
        }

        #endregion
    }
}