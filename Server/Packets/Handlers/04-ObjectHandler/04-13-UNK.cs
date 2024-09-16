using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSO2SERVER.Models;
using PSO2SERVER.Zone;

namespace PSO2SERVER.Packets.Handlers
{
    //[PacketHandlerAttr(0x04, 0x13)]
    //class _04_13_UNK : PacketHandler
    //{
    //    public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
    //    {
    //        //if (context.currentParty.currentQuest == null)
    //        //    return;

    //        //// TODO: WTF terribad hax?
    //        //if (context.CurrentLocation.PosZ >= 20)
    //        //{
    //        //    var instanceName = String.Format("{0}-{1}", context.currentParty.currentQuest.name, context.User.Nickname);

    //        //    Map forest = ZoneManager.Instance.MapFromInstance("area1", instanceName);
    //        //    forest.SpawnClient(context, new PSOLocation(0, 1, 0, -0, -37, 0.314f, 145.5f));
    //        //}
    //    }
    //}
}
