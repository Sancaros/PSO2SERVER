using System.Collections.Generic;
using PSO2SERVER.Packets.PSOPackets;


namespace PSO2SERVER.Models
{
    public class Flags
    {
        public enum FlagType : uint
        {
            /// Flag is account related.
            Account,
            /// Flag is character related.
            Character,
        }

        private List<byte> flags = new List<byte>();
        private List<uint> paramsList = new List<uint>();

        public void Set(int id, byte val)
        {
            int index = id / 8;
            byte bitIndex = (byte)(id % 8);

            while (flags.Count <= index)
                flags.Add(0);

            flags[index] = SetBit(flags[index], bitIndex, val);
        }

        public byte Get(int id)
        {
            int index = id / 8;
            byte bitIndex = (byte)(id % 8);

            if (index >= flags.Count)
                return 0;

            return (byte)((flags[index] & (1 << bitIndex)) >> bitIndex);
        }

        public void SetParam(int id, uint val)
        {
            while (paramsList.Count <= id)
                paramsList.Add(0);

            paramsList[id] = val;
        }

        public uint GetParam(int id)
        {
            if (id >= paramsList.Count)
                return 0;

            return paramsList[id];
        }

        public AccountFlagsPacket ToAccountFlags()
        {
            return new AccountFlagsPacket
            {
                Flags = new List<byte>(flags),
                Params = new List<uint>(paramsList)
            };
        }

        public CharacterFlagsPacket ToCharFlags()
        {
            return new CharacterFlagsPacket
            {
                Flags = new List<byte>(flags),
                Params = new List<uint>(paramsList)
            };
        }

        private static byte SetBit(byte byteValue, byte index, byte val)
        {
            byteValue &= (byte)~(1 << index);
            return (byte)(byteValue | ((val & 1) << index));
        }
    }

}
