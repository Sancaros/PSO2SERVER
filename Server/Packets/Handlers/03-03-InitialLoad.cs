using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x03, 0x03)]
    public class InitialLoad : PacketHandler
    {
        // Ninji note: 3-3 may not be the correct place to do this
        // Once we have better state tracking, we should make sure that
        // 3-3 only does anything at the points where the client is supposed
        // to be sending it, etc etc

        // This seems to only ever be called once after logging in, yet is also handled by 11-3E in other places
        // Moved the actual handling into 11-3E until I can actually confirm this
        // Just insantiate a new CharacterSpawn and push it through until then
        // - Kyle

        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            // Set Player ID
            var setPlayerId = new PacketWriter();
            setPlayerId.WritePlayerHeader((uint)context.User.PlayerId);
            context.SendPacket(6, 0, 0, setPlayerId.ToArray());

            // Spawn Player
            new CharacterSpawn().HandlePacket(context, flags, data, position, size);
        }

        #endregion
    }

}
