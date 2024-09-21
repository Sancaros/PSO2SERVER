using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2SERVER.Models
{
    public unsafe struct Unk2Struct
    {
        public fixed uint Unk[0x40]; // Fixed length array equivalent in C#

    }

    public class Mission
    {
        public uint MissionType;       // Mission type.
        public uint StartDate;         // Mission start timestamp.
        public uint EndDate;           // Mission end timestamp.
        public uint Id;                // Mission ID.
        public uint Unk5;
        public uint CompletionDate;    // Last completion timestamp.
        public uint Unk7;
        public uint Unk8;
        public uint Unk9;
        public uint Unk10;
        public uint Unk11;
        public uint Unk12;
        public uint Unk13;
        public uint Unk14;
        public uint Unk15;
    }
}
