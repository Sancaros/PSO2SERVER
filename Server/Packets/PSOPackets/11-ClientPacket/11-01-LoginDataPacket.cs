using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mysqlx;
using PSO2SERVER.Models;

namespace PSO2SERVER.Packets.PSOPackets
{
    class LoginDataPacket : Packet
    {
        public enum LoginStatus : UInt32
        {
            /// <summary>
            /// Login was successful.
            /// </summary>
            Success = 0,

            /// <summary>
            /// Login failed.
            /// </summary>
            Failure = 1,

            /// <summary>
            /// Undefined status.
            /// </summary>
            Undefined = 0xFFFFFFFF
        }

        public LoginStatus Status;
        public string Error;
        public ObjectHeader Player;
        public string BlockName;
        public float Unk1;
        public uint Unk2;
        public uint LevelCap;
        public uint LevelCap2;
        public uint Unk5;
        public float Unk6;
        public float Unk7;
        public uint Unk8;
        public float Unk9;
        public float Unk10;
        public uint Unk11;
        public float Unk12;
        public uint Unk13;
        public float[] Unk14; // Length: 10
        public float[] Unk15; // Length: 21
        public float Unk16;
        public float Unk17;
        public float[] Unk18; // Length: 9
        public uint[] Unk19;   // Length: 2
        public uint Unk20;
        public uint Unk21;
        public float[] Unk22;  // Length: 3
        public uint Unk23;
        public float Unk24;
        public float Unk25;
        public uint Unk26;
        public byte[] Unk27;    // Length: 12
        public string Unk28;
        public uint Unk29;
        public string Unk30;
        public uint Unk31;

        private string ReadFixedString(PacketReader reader, int length)
        {
            byte[] bytes = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes).TrimEnd('\0');
        }
        public void ReadFromStream(PacketReader reader)
        {
            Status = (LoginStatus)reader.ReadInt32();
            Error = ReadFixedString(reader, 32); // 0x20 = 32
            Player.ReadFromStream(reader);
            BlockName = ReadFixedString(reader, 32);
            Unk1 = reader.ReadSingle();
            Unk2 = reader.ReadUInt32();
            LevelCap = reader.ReadUInt32();
            LevelCap2 = reader.ReadUInt32();
            Unk5 = reader.ReadUInt32();
            Unk6 = reader.ReadSingle();
            Unk7 = reader.ReadSingle();
            Unk8 = reader.ReadUInt32();
            Unk9 = reader.ReadSingle();
            Unk10 = reader.ReadSingle();
            Unk11 = reader.ReadUInt32();
            Unk12 = reader.ReadSingle();
            Unk13 = reader.ReadUInt32();

            Unk14 = new float[10];
            for (int i = 0; i < Unk14.Length; i++)
            {
                Unk14[i] = reader.ReadSingle();
            }

            Unk15 = new float[21];
            for (int i = 0; i < Unk15.Length; i++)
            {
                Unk15[i] = reader.ReadSingle();
            }

            Unk16 = reader.ReadSingle();
            Unk17 = reader.ReadSingle();

            Unk18 = new float[9];
            for (int i = 0; i < Unk18.Length; i++)
            {
                Unk18[i] = reader.ReadSingle();
            }

            Unk19 = new uint[2];
            for (int i = 0; i < Unk19.Length; i++)
            {
                Unk19[i] = reader.ReadUInt32();
            }

            Unk20 = reader.ReadUInt32();
            Unk21 = reader.ReadUInt32();

            Unk22 = new float[3];
            for (int i = 0; i < Unk22.Length; i++)
            {
                Unk22[i] = reader.ReadSingle();
            }

            Unk23 = reader.ReadUInt32();
            Unk24 = reader.ReadSingle();
            Unk25 = reader.ReadSingle();
            Unk26 = reader.ReadUInt32();
            Unk27 = reader.ReadBytes(12);
            Unk28 = ReadFixedString(reader, 32);
            Unk29 = reader.ReadUInt32();
            Unk30 = ReadFixedString(reader, 32);
            Unk31 = reader.ReadUInt32();
        }

        private void WriteFixedString(PacketWriter writer, string str, int length)
        {
            byte[] bytes = new byte[length];
            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            Array.Copy(strBytes, bytes, Math.Min(strBytes.Length, length));
            writer.Write(bytes);
        }

        public void WriteToStream(PacketWriter writer)
        {
            writer.Write((int)Status);
            writer.WriteUtf16(Error, 0x8BA4, 0xB6);

            if (Player.ID == 0)
            {
                for (var i = 0; i < 0xEC; i++)
                    writer.Write((byte)0);
            }
            else
            {
                //WriteFixedString(writer, Error, 32);
                Player.WriteToStream(writer);
                WriteFixedString(writer, BlockName, 32);
                writer.Write(Unk1);
                writer.Write(Unk2);
                writer.Write(LevelCap);
                writer.Write(LevelCap2);
                writer.Write(Unk5);
                writer.Write(Unk6);
                writer.Write(Unk7);
                writer.Write(Unk8);
                writer.Write(Unk9);
                writer.Write(Unk10);
                writer.Write(Unk11);
                writer.Write(Unk12);
                writer.Write(Unk13);

                foreach (var val in Unk14)
                {
                    writer.Write(val);
                }

                foreach (var val in Unk15)
                {
                    writer.Write(val);
                }

                writer.Write(Unk16);
                writer.Write(Unk17);

                foreach (var val in Unk18)
                {
                    writer.Write(val);
                }

                foreach (var val in Unk19)
                {
                    writer.Write(val);
                }

                writer.Write(Unk20);
                writer.Write(Unk21);

                foreach (var val in Unk22)
                {
                    writer.Write(val);
                }

                writer.Write(Unk23);
                writer.Write(Unk24);
                writer.Write(Unk25);
                writer.Write(Unk26);
                writer.Write(Unk27);
                WriteFixedString(writer, Unk28, 32);
                writer.Write(Unk29);
                WriteFixedString(writer, Unk30, 32);
                writer.Write(Unk31);
            }
        }

        public LoginDataPacket(string blockName, string error, uint userid)
        {
            Status = (userid == 0) ? LoginStatus.Failure : LoginStatus.Success;
            Error = error;
            Player.ID = userid;
            Player.ObjectType = ObjectType.Player;
            BlockName = blockName;
        }

        public override byte[] Build()
        {
            var resp = new PacketWriter();
            resp.Write((uint)Status); // Status flag: 0=success, 1=error
            resp.WriteUtf16(Error, 0x8BA4, 0xB6);

            if (Player.ID == 0)
            {
                for (var i = 0; i < 0xEC; i++)
                    resp.Write((byte)0);
                return resp.ToArray();
            }

            // TODO: Explore this data! Some if it seems really important. (May contain level cap setting + more)

            resp.WriteStruct(Player);
            resp.WriteFixedLengthUtf16(BlockName, 0x20); // This is right
            // Set things to "default" values; Dunno these purposes yet.
            resp.Write(0x42700000); //0
            resp.Write(7);          //4
            resp.Write(0xA);        //8 - Level Cap!
            resp.Write(1);          //C
            resp.Write(0x41200000); //10
            resp.Write(0x40A00000); //14
            resp.Write(11);         //18
            resp.Write(0x3F800000); //1C (1 as a float)
            resp.Write(0x42960000); //20
            resp.Write(40);         //24
            resp.Write(0x41200000); //28
            resp.Write(1);          //2C?
            resp.Write(1120403456); //30

            //WHAT
            for (int i = 0; i < 10; i++)
            {
                resp.Write(1065353216);
            }
            //ARE
            for (int i = 0; i < 21; i++)
            {
                resp.Write(1120403456);
            }
            //THESE?
            resp.Write(0x91A2B);    //B0
            resp.Write(0x91A2B);    //B4

            resp.WriteBytes(0, 12);

            return resp.ToArray();
        }

        public override PacketHeader GetHeader()
        {
            return new PacketHeader(0x11, 0x01, PacketFlags.PACKED);
        }
    }
}
