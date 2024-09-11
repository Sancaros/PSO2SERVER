using PSO2SERVER.Models;

namespace PSO2SERVER.Packets
{
    public abstract class Packet
    {
        public abstract byte[] Build();
        public abstract PacketHeader GetHeader();
    }
}