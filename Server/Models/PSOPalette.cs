using PSO2SERVER.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PSO2SERVER.Models.PSOPalette;

namespace PSO2SERVER.Models
{
    public class PSOPalette
    {
        public struct Palette
        {
            public uint CurPalette;
            public uint CurSubpalette;
            public uint CurBook;
            public WeaponPalette[] Palettes;
            public SubPalette[] Subpalettes;
            public List<uint> DefaultPas;

            // 构造函数
            public static Palette Create()
            {
                Palette palette = new Palette
                {
                    CurPalette = 0,
                    CurSubpalette = 0,
                    CurBook = 0,
                    Palettes = new WeaponPalette[6],
                    Subpalettes = new SubPalette[6],
                    DefaultPas = new List<uint>()
                };

                // 初始化 Palettes
                for (int i = 0; i < palette.Palettes.Length; i++)
                {
                    palette.Palettes[i] = WeaponPalette.Create();
                }

                // 初始化 Subpalettes（根据需要可以进行自定义初始化）
                for (int i = 0; i < palette.Subpalettes.Length; i++)
                {
                    palette.Subpalettes[i] = SubPalette.Create(); // 这里可以根据需要进行初始化
                }

                return palette;
            }

            public void ReadFromStream(PacketReader reader)
            {
                CurPalette = reader.ReadUInt32();
                CurSubpalette = reader.ReadUInt32();
                CurBook = reader.ReadUInt32();

                for (int i = 0; i < Palettes.Length; i++)
                {
                    Palettes[i].ReadFromStream(reader);
                }

                for (int i = 0; i < Subpalettes.Length; i++)
                {
                    Subpalettes[i].ReadFromStream(reader);
                }

                int defaultPasCount = reader.ReadInt32(); // 假设先读入数量
                DefaultPas.Clear();
                for (int i = 0; i < defaultPasCount; i++)
                {
                    DefaultPas.Add(reader.ReadUInt32());
                }
            }

            public void WriteToStream(PacketWriter writer)
            {
                writer.Write(CurPalette);
                writer.Write(CurSubpalette);
                writer.Write(CurBook);

                for (int i = 0; i < Palettes.Length; i++)
                {
                    Palettes[i].WriteToStream(writer);
                }

                for (int i = 0; i < Subpalettes.Length; i++)
                {
                    Subpalettes[i].WriteToStream(writer);
                }

                writer.Write(DefaultPas.Count);
                foreach (var value in DefaultPas)
                {
                    writer.Write(value);
                }
            }
        }
        public struct PalettePA
        {
            /// PA ID.
            public byte ID { get; set; }
            /// PA category.
            public byte Category { get; set; }
            public byte Unk { get; set; }
            /// PA level.
            public byte Level { get; set; }

            public PalettePA(byte id, byte category, byte unk, byte level)
            {
                ID = id;
                Category = category;
                Unk = unk;
                Level = level;
            }

            public void ReadFromStream(PacketReader reader)
            {
                ID = reader.ReadByte();
                Category = reader.ReadByte();
                Unk = reader.ReadByte();
                Level = reader.ReadByte();
            }

            public void WriteToStream(PacketWriter writer)
            {
                writer.Write(ID);
                writer.Write(Category);
                writer.Write(Unk);
                writer.Write(Level);
            }
        }

        public struct SubPalette
        {
            //        // 创建 SubPalette 实例
            //        var subPalette = SubPalette.Create();

            //// 从流中读取数据
            //using (var reader = new PacketReader(yourStream))
            //{
            //    subPalette.ReadFromStream(reader);
            //}

            //// 将数据写入流
            //using (var writer = new PacketWriter(yourStream))
            //{
            //    subPalette.WriteToStream(writer);
            //}
            /// Items in the subpalette.
            public PalettePA[] Items { get; set; }
            // 初始化数组
            public static SubPalette Create()
            {
                return new SubPalette { Items = new PalettePA[12] };
            }

            public void ReadFromStream(PacketReader reader)
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    var palettePA = new PalettePA();
                    palettePA.ReadFromStream(reader);
                    Items[i] = palettePA;
                }
            }

            public void WriteToStream(PacketWriter writer)
            {
                foreach (var item in Items)
                {
                    item.WriteToStream(writer);
                }
            }
        }

        public struct WeaponPalette
        {
            public ulong Uuid { get; set; }
            public uint Unk1 { get; set; }
            public PalettePA Unk2 { get; set; }
            public PalettePA Unk3 { get; set; }
            public PalettePA Unk4 { get; set; }
            public uint[] Unk { get; set; } // 初始化时需指定长度
            public uint PetId { get; set; }
            public PalettePA[] Skills { get; set; } // 初始化时需指定长度

            public static WeaponPalette Create()
            {
                return new WeaponPalette
                {
                    Unk = new uint[3],
                    Skills = new PalettePA[6]
                };
            }

            public void ReadFromStream(PacketReader reader)
            {
                Uuid = reader.ReadUInt64();
                Unk1 = reader.ReadUInt32();
                Unk2 = new PalettePA();
                Unk2.ReadFromStream(reader);
                Unk3 = new PalettePA();
                Unk3.ReadFromStream(reader);
                Unk4 = new PalettePA();
                Unk4.ReadFromStream(reader);
                for (int i = 0; i < Unk.Length; i++)
                {
                    Unk[i] = reader.ReadUInt32();
                }
                PetId = reader.ReadUInt32();
                for (int i = 0; i < Skills.Length; i++)
                {
                    Skills[i] = new PalettePA();
                    Skills[i].ReadFromStream(reader);
                }
            }

            public void WriteToStream(PacketWriter writer)
            {
                writer.Write(Uuid);
                writer.Write(Unk1);
                Unk2.WriteToStream(writer);
                Unk3.WriteToStream(writer);
                Unk4.WriteToStream(writer);
                foreach (var value in Unk)
                {
                    writer.Write(value);
                }
                writer.Write(PetId);
                foreach (var skill in Skills)
                {
                    skill.WriteToStream(writer);
                }
            }
        }
    }
}
