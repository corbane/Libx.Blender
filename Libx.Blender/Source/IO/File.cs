namespace Libx.Blender.IO;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Libx.Blender.Utilities;

using Sys = System.IO;
using len_t = System.Int32;

#nullable enable


public readonly struct adr_t : IComparable <adr_t>, IEquatable <adr_t>
{
    readonly ulong _value;

    public adr_t (ulong value) { _value = value < 0 ? 0 : value; }

    public bool IsValid => _value > 0;

    public int CompareTo (adr_t other) => _value.CompareTo (other._value);
    public bool Equals (adr_t other) => _value == other._value;

    public override string ToString () => _value.ToString ();
}

public enum Endian
{
    Be = 86,
    Le = 118,
}

[Flags]
public enum BlockCode : Int32
{
    DATA = ('A' << 24) | ('T' << 16) | ('A' << 8) | 'D',
    GLOB = ('B' << 24) | ('O' << 16) | ('L' << 8) | 'G',
    DNA1 = ('1' << 24) | ('A' << 16) | ('N' << 8) | 'D',
    TEST = ('T' << 24) | ('S' << 16) | ('E' << 8) | 'T',
    REND = ('D' << 24) | ('N' << 16) | ('E' << 8) | 'R',
    USER = ('R' << 24) | ('E' << 16) | ('S' << 8) | 'U',
    ENDB = ('B' << 24) | ('D' << 16) | ('N' << 8) | 'E',
    /// <summary> Scene </summary>
    SC  = ('C' << 8) | 'S',
    /// <summary> Library </summary>
    LI  = ('I' << 8) | 'L',
    /// <summary> Object </summary>
    OB  = ('B' << 8) | 'O',
    /// <summary> Mesh </summary>
    ME  = ('E' << 8) | 'M',
    /// <summary> Curve </summary>
    CU  = ('U' << 8) | 'C',
    /// <summary> MetaBall </summary>
    MB  = ('B' << 8) | 'M',
    /// <summary> Material </summary>
    MA  = ('A' << 8) | 'M',
    /// <summary> Tex (Texture) </summary>
    TE  = ('E' << 8) | 'T',
    /// <summary> Image </summary>
    IM  = ('M' << 8) | 'I',
    /// <summary> Lattice </summary>
    LT  = ('T' << 8) | 'L',
    /// <summary> Light </summary>
    LA  = ('A' << 8) | 'L',
    /// <summary> Camera </summary>
    CA  = ('A' << 8) | 'C',
    /// <summary> Ipo (depreciated, replaced by FCurves) </summary>
    IP  = ('P' << 8) | 'I',
    /// <summary> Key (shape key) </summary>
    KE  = ('E' << 8) | 'K',
    /// <summary> World </summary>
    WO  = ('O' << 8) | 'W',
    /// <summary> Screen </summary>
    SR  = ('R' << 8) | 'S',
    /// <summary> VFont (Vector Font) </summary>
    VF  = ('F' << 8) | 'V',
    /// <summary> Text </summary>
    TX  = ('X' << 8) | 'T',
    /// <summary> Speaker </summary>
    SK  = ('K' << 8) | 'S',
    /// <summary> Sound </summary>
    SO  = ('O' << 8) | 'S',
    /// <summary> Group </summary>
    GR  = ('R' << 8) | 'G',
    /// <summary> bArmature </summary>
    AR  = ('R' << 8) | 'A',
    /// <summary> bAction </summary>
    AC  = ('C' << 8) | 'A',
    /// <summary> bNodeTree </summary>
    NT  = ('T' << 8) | 'N',
    /// <summary> Brush </summary>
    BR  = ('R' << 8) | 'B',
    /// <summary> ParticleSettings </summary>
    PA  = ('A' << 8) | 'P',
    /// <summary> bGPdata, (Grease Pencil) </summary>
    GD  = ('D' << 8) | 'G',
    /// <summary> WindowManager </summary>
    WM  = ('M' << 8) | 'W',
    /// <summary> MovieClip </summary>
    MC  = ('C' << 8) | 'M',
    /// <summary> Mask </summary>
    MS  = ('S' << 8) | 'M',
    /// <summary> FreestyleLineStyle </summary>
    LS  = ('S' << 8) | 'L',
    /// <summary> Palette </summary>
    PL  = ('L' << 8) | 'P',
    /// <summary> PaintCurve  </summary>
    PC  = ('C' << 8) | 'P',
    /// <summary> CacheFile </summary>
    CF  = ('F' << 8) | 'C',
    /// <summary> WorkSpace </summary>
    WS  = ('S' << 8) | 'W',
    /// <summary> LightProbe </summary>
    LP  = ('P' << 8) | 'L',
    /// <summary> Hair </summary>
    HA  = ('A' << 8) | 'H',
    /// <summary> PointCloud </summary>
    PT  = ('T' << 8) | 'P',
    /// <summary> Volume </summary>
    VO  = ('O' << 8) | 'V',
    /// <summary> Simulation </summary>
    SI  = ('I' << 8) | 'S',
    /// <summary> (internal use only) Only used as 'placeholder' in .blend files for directly linked data-blocks. </summary>
    ID  = ('D' << 8) | 'I',
    /// <summary> Deprecated </summary>
    SN  = ('N' << 8) | 'S',
    /// <summary> NOTE! Fake IDs, needed for g.sipo->blocktype or outliner </summary>
    SQ  = ('Q' << 8) | 'S',
    /// <summary> constraint </summary>
    CO  = ('O' << 8) | 'C',
    /// <summary> used in outliner... </summary>
    NL  = ('L' << 8) | 'N',
    /// <summary> fluidsim Ipo </summary>
    FS  = ('S' << 8) | 'F',
}

public static class BlockCodeExtension
{
    public static bool IsCode4 (this BlockCode code) => ((int)code & 0xFFFF0000) != 0;
    public static bool IsCode2 (this BlockCode code) => ((int)code & 0xFFFF0000) == 0;
    public static int ToIndex (this BlockCode code)
    {
        return code switch
        {
            BlockCode.SC => 0,
            BlockCode.LI => 1,
            BlockCode.OB => 2,
            BlockCode.ME => 3,
            BlockCode.CU => 4,
            BlockCode.MB => 5,
            BlockCode.MA => 6,
            BlockCode.TE => 7,
            BlockCode.IM => 8,
            BlockCode.LT => 9,
            BlockCode.LA => 10,
            BlockCode.CA => 11,
            BlockCode.IP => 12,
            BlockCode.KE => 13,
            BlockCode.WO => 14,
            BlockCode.SR => 15,
            BlockCode.VF => 16,
            BlockCode.TX => 17,
            BlockCode.SK => 18,
            BlockCode.SO => 19,
            BlockCode.GR => 20,
            BlockCode.AR => 21,
            BlockCode.AC => 22,
            BlockCode.NT => 23,
            BlockCode.BR => 24,
            BlockCode.PA => 25,
            BlockCode.GD => 26,
            BlockCode.WM => 27,
            BlockCode.MC => 28,
            BlockCode.MS => 29,
            BlockCode.LS => 30,
            BlockCode.PL => 31,
            BlockCode.PC => 32,
            BlockCode.CF => 33,
            BlockCode.WS => 34,
            BlockCode.LP => 35,
            BlockCode.HA => 36,
            BlockCode.PT => 37,
            BlockCode.VO => 38,
            BlockCode.SI => 39,
            BlockCode.ID => 40,
            BlockCode.SN => 41,
            BlockCode.SQ => 42,
            BlockCode.CO => 43,
            BlockCode.NL => 44,
            BlockCode.FS => 45,
            _ => -1,
        };
    }
    public static int IDCount () => 46;
}


public partial class File
{
    public static File From (string path)
    {
        var file = Sys.File.Open (path, Sys.FileMode.Open, Sys.FileAccess.Read, Sys.FileShare.Read);
        var mem = new Sys.MemoryStream ();
        var bin = new byte[4];

        file.Read (bin, 0, 4);
        file.Seek (0, Sys.SeekOrigin.Begin);

        if ( // GZIP compressed file
            bin[0] == 0x1F &&
            bin[1] == 0x8B
        ) {
            var gz = new Sys.Compression.GZipStream (file, Sys.Compression.CompressionMode.Decompress);
            gz.CopyTo (mem);
            gz.Close ();
        }
        else if ( // Zstandard compressed file
            bin[0] == 0x28 &&
            bin[1] == 0xB5 &&
            bin[2] == 0x2F &&
            bin[3] == 0xFD
        )
        {
            var decompresor = new ZstdNet.DecompressionStream (file);
            decompresor.CopyTo (mem);
            decompresor.Close ();
        }
        else
        {
            file.CopyTo (mem);
        }

        var blend = new File (mem.ToArray ());
        mem.Close ();
        file.Close ();
        return blend;
    }

    File (byte[] bin)
    {
        // HEADER

        if (
            bin[0] != (byte)'B' ||
            bin[1] != (byte)'L' ||
            bin[2] != (byte)'E' ||
            bin[3] != (byte)'N' ||
            bin[4] != (byte)'D' ||
            bin[5] != (byte)'E' ||
            bin[6] != (byte)'R'
        ) throw new Exception ("not a blender file");

        var psize = bin[7];
        PointerSize = psize == '_' ? 4
                    : psize == '-' ? 8
                    : throw new Exception ($"undefined pointer size code '{psize}'");

        var endian = bin[8];
        Endian = endian == 'v' ? Endian.Le
                : endian == 'V' ? Endian.Be
                : throw new Exception ($"undefined endianess code '{endian}'");

        Version = Encoding.ASCII.GetString (new[] { bin[9], bin[10], bin[11] });

        Buffer = new BinaryReader (bin, Endian, PointerSize == 8);
        Buffer.seek (12);

        // BLOCKS
        // +--------+--------+--------+--------+
        // |               Code                |
        // +--------+--------+--------+--------+
        // |            BodyLength             |
        // +--------+--------+--------+--------+~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~+  
        // |~        OldAddress  32bits        or        OldAddress  64bits        |
        // +--------+--------+--------+--------+~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~+  
        // |            StructIndex            |
        // +--------+--------+--------+--------+~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~+
        // |              Count                |          byte*BodyLength          | 
        // +--------+--------+--------+--------+~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~+

        Block block;
        BlockCode code;
        var bin4  = new byte[4];
        var index = 0;
        var b_builder = new ArrayBuilder <Block> (1000);
        for(;;)
        {
            Buffer.read_NE (4, bin4);
            code = bin4[0] != 0
                    ? /* CODE */ (BlockCode) ((bin4[3] << 24) | (bin4[2] << 16) | (bin4[1] << 8) | bin4[0])
                    : /* 00ID */ (BlockCode) ((bin4[3] << 8) | bin4[2]);
                    
            if (code == BlockCode.ENDB) break;

            block = new Block (
                    this, index, Buffer.CurrentPosition - 4,
                    code, Buffer.read_i32 (), Buffer.read_adr (), Buffer.read_u32 (), Buffer.read_u32 ()
            );
            Buffer.jump (block.BodyLength);

            if (code == BlockCode.REND) continue;

            b_builder.Add (block);
            index++;
        }

        Blocks = new BlockTable (b_builder);

        //  DNA1 Block
        // +--------+--------+--------+--------+
        // |  'S'   |  'D'   |  'N'   |  'A'   |
        // +--------+--------+--------+--------+
        // |  'N'   |  'A'   |  'M'   |  'E'   |
        // +--------+--------+--------+--------+~~~~~~~~~~~~~~~~~~~~~~~~~~+~~~~~~~~+
        // |             NameCount             |     UTF8\0*NameCount     |   ?    |
        // +--------+--------+--------+--------+~~~~~~~~~~~~~~~~~~~~~~~~~~+~~~~~~~~+
        // |  'T'   |  'Y'   |  'P'   |  'E'   |
        // +--------+--------+--------+--------+~~~~~~~~~~~~~~~~~~~~~~~~~~+~~~~~~~~+
        // |             TypeCount             |     UTF8\0*TypeCount     |   ?    |
        // +--------+--------+--------+--------+~~~~~~~~~~~~~~~~~~~~~~~~~~+~~~~~~~~+
        // |  'T'   |  'L'   |  'E'   |  'N'   |     short*TypeCount      |   ?    |
        // +--------+--------+--------+--------+~~~~~~~~~~~~~~~~~~~~~~~~~~+~~~~~~~~+
        // |  'S'   |  'T'   |  'R'   |  'C'   |
        // +--------+--------+--------+--------+~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~+~~~~~~~~+
        // |                                   |  +--------+--------+--------+--------+~~~~~~~~~~~~~~~~~+              |        |
        // |            StructCount            |  |    TypeIndex    |   FieldCount    |~   SdnaField   ~| * FieldCount |   ?    |
        // |                                   |  +--------+--------+--------+--------+~~~~~~~~~~~~~~~~~+              |        |
        // +--------+--------+--------+--------+~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~+~~~~~~~~+

        var dna1 = Blocks[Blocks.Count - 1];
        Buffer.seek (dna1.BodyPosition);

        byte[] id = new byte[4];
        void ReadMagic (string magic)
        {
            Buffer.read_NE (4, id);
            if ( id[0] != magic[0] || id[1] != magic[1] ||
                    id[2] != magic[2] || id[3] != magic[3]
            ) throw new Exception ("Not a SNA block file");
        }

        ReadMagic ("SDNA");

        ReadMagic ("NAME");
        NameCount = Buffer.read_i32 ();
        var nbuilder = new ArrayBuilder <Name> (NameCount);
        for (index = 0; index < NameCount; index++)
        {
            nbuilder.Add (new Name (index, Buffer.read_str (Encoding.UTF8)));
        }
        Names = nbuilder.ToArray ();
        Buffer.jumpZ ();
        // Buffer.seek (Buffer.align (Buffer.CurrentPosition, 4));
        // The following value should be aligned, but in some files it is not.
        // The first error was seen with Blender 2.9

        ReadMagic ("TYPE");
        TypeCount = Buffer.read_i32 ();
        var tbuilder = new ArrayBuilder <Type> (TypeCount);
        for (index = 0; index < TypeCount; index++)
        {
            tbuilder.Add (new Type (this, index, Buffer.read_str (Encoding.UTF8)));
        }
        Types = tbuilder.ToArray ();
        Buffer.jumpZ ();
        // Buffer.seek (Buffer.align (Buffer.CurrentPosition, 4));

        ReadMagic ("TLEN");
        var lbuilder = new ArrayBuilder <ushort> (TypeCount);
        for (index = 0 ; index < TypeCount ; index++)
        {
            var size = Buffer.read_u16 ();
            lbuilder.Add (size);
        }
        Lengths = lbuilder.ToArray ();
        Buffer.jumpZ ();
        // Buffer.seek (Buffer.align (Buffer.CurrentPosition, 4));

        ReadMagic ("STRC");
        StructCount = Buffer.read_i32 ();
        var sbuilder = new ArrayBuilder <Struct> (StructCount);
        Struct st;
        for (index = 0; index < StructCount; index++)
        {
            st = new Struct (this, Buffer.CurrentPosition, Buffer.read_i16 (), Buffer.read_i16 ());
            ((Type)st.Type).Struct = st;
            Buffer.jump (Field.SIZE * st.FieldCount);

            sbuilder.Add (st);
        }
        Structs = sbuilder.ToArray ();
    }

    // HEADER

    public IBinaryReader Buffer { get; }
    public int PointerSize { get; }
    public Endian Endian { get; }
    public string Version { get; }

    // BLOCKS

    public BlockTable Blocks { get; }

    // DNA1

    public int NameCount { get; }
    public int TypeCount { get; }
    public int StructCount { get; }
    internal Name[] Names { get; }
    public Type[] Types { get; }
    public ushort[] Lengths { get; }
    public Struct[] Structs { get; }


    public IEnumerable <Block> GetByCode (BlockCode code)
    {
        return Blocks[code];
    }

    public Block? GetBlock (adr_t address)
    {
        return address.IsValid == false
            ? null
            : Blocks.Find (address);
    }

    static int CompareBlockAddress (Block a, Block b)
    {
        return a.OldAddress.CompareTo (b.OldAddress);
    }

}

public class BlockTable : Table <adr_t, Block>
{
    static IEnumerable <Block> _empty = new Block[0];

    readonly AddressItem[] _adrs;
    readonly int _adrs_count;
    Dictionary <BlockCode, List <Block>> _codes;

    readonly struct AddressItem
    {
        public readonly adr_t Address;
        public readonly int Index;
               
        public AddressItem (adr_t address, int index)
        {
            Address = address;
            Index = index;
        }

        public override string ToString () => $"{Address} -> {Index}";
    }

    public BlockTable (ArrayBuilder <Block> builder) : base (builder.ToArray ())
    {
        _adrs_count = 0;
        _adrs  = new AddressItem[Count];
        _codes = new (BlockCodeExtension.IDCount ());

        foreach (var b in this)
        {
            if (b.OldAddress.IsValid)
            {
                    _adrs[_adrs_count++] = new AddressItem (b.OldAddress, b.BlockIndex);
            }

            if (b.Code.IsCode2 ())
            {
                    if (_codes.ContainsKey (b.Code))
                        _codes[b.Code].Add (b);
                    else
                        _codes.Add (b.Code, new List<Block> { b });
            }
        }

        ArrayLib.Sort (_adrs, 0, _adrs_count, (a, b) => a.Address.CompareTo (b.Address));
    }

    public IEnumerable <Block> this[BlockCode code]
    {
        get => _codes.ContainsKey (code)
            ? _codes[code]
            : _empty;
    }

    public override adr_t GetKey (Block item) => item.OldAddress;

    public override int IndexOf (adr_t adr)
    {
        if (adr.IsValid == false)
            return -1;

        var i = ArrayLib.BinarySearch <AddressItem> (_adrs, 0, _adrs_count, (item) => item.Address.CompareTo (adr));
        return i < 0 ? -1 : _adrs[i].Index;
    }
}


public class Part : BinaryBuffer
{
    public Part (Part bin, IField field) : base (bin, field.Offset, field.Size)
    {
        if (field.Type.Struct == null)
            throw new ArgumentException ($"Field \"{field}\" is not a structure", nameof (field));

        OwnerFile = bin.OwnerFile;
        Struct = field.Type.Struct;
    }

    #nullable disable
    internal Part (File blend, len_t offset, len_t size) : base (blend.Buffer, offset, size)
    {
        OwnerFile = blend;
        Struct = null;
    }
    #nullable enable

    public virtual File OwnerFile { get; }
    public virtual IStruct Struct { get; }

    public override string ToString () => Struct.ToString ();

    public virtual Part sub (IField field) => new Part (this, field);
    public virtual Part sub (string field_name) => new Part (this, Struct.Fields[field_name]);


    public virtual Block? ptr (adr_t adr, Block? def = null) => OwnerFile.GetBlock (adr) ?? def;
    public virtual Block? ptr (len_t p, Block? def = null) => OwnerFile.GetBlock (adr (p)) ?? def;
    public virtual Block? ptr (string k, Block? def = null) => ptr (Struct.Fields[k].Offset, def);

    //public override len_t pos (int index)
    public len_t pos (int index)
    {
        var fields = Struct.Fields;
        return index < 0 || index <= fields.Count ? -1
            : fields[index].Offset;
    }
          
    //public override len_t pos (string name)
    public len_t pos (string name)
    {
        var f = Struct.Fields.Find (name);
        return f == null ? -1 : f.Offset;
    }
}

public partial class Block : Part
{
    public const int HeaderSize32 = 20;
    public const int HeaderSize64 = 24;

    public Block (
        File blend,
        int index,
        len_t header_position,
        BlockCode code,
        int body_length,
        adr_t old_address,
        uint struct_index,
        uint count
    )
        : base (
                blend,
                header_position + (blend.PointerSize == 4 ? HeaderSize32 : HeaderSize64),
                body_length
        )
    {
        BlockIndex     = index;
        OwnerFile      = blend;
        HeaderPosition = header_position;
        Code           = code;
        OldAddress     = old_address;
        StructIndex    = struct_index;
        Repeat         = count;
    }

    public len_t HeaderPosition { get; }

    public override File OwnerFile { get; }
    public override Struct Struct => OwnerFile.Structs[StructIndex];

    public uint Repeat { get; }
    public int BlockIndex { get; }
    public BlockCode Code { get; }
    public adr_t OldAddress { get; }
    public uint StructIndex { get; }

    public override string ToString () => $"{Code} {Struct}";
}

public class Struct : IStruct
{
    public const len_t HEADER_SIZE = 4;

    public Struct (File blend, len_t file_position, short type_index, short field_count)
    {
        OwnerFile    = blend;
        BufferPosition = file_position;
        TypeIndex    = type_index;
        FieldCount   = field_count;
        Type         = blend.Types[TypeIndex];
        Fields       = new FieldTable (this, field_count);
    }


    public File OwnerFile { get; }
    public short TypeIndex { get; }
    public short FieldCount { get; }


    public IType Type { get; }
    public Table <string, IField> Fields { get; }
    public len_t BufferPosition { get; }
    public int CompareTo (IStruct? other) => other == null ? 1 : Type.Index.CompareTo (other.Type.Index);
    //len_t m_totsize = -1;
    //public len_t TotalSize
    //{
    //     get {
    //          if (m_totsize == -1)
    //          {
    //               m_totsize = 0;
    //               foreach (var field in Fields)
    //                    m_totsize += OwnerFile.Lengths[field.TypeIndex];
    //          }
    //          return m_totsize;
    //     }
    //}

    public len_t pos (int index)
    {
        return index < 0 || index <= Fields.Count ? -1
            : Fields[index].Offset;
    }

    public len_t pos (string name)
    {
        var f = Fields.Find (name);
        return f == null ? -1 : f.Offset;
    }

    public override string ToString () => Type.Name;
}

public class FieldTable : Table <string, IField>
{
    public FieldTable (Struct st, int count) : base (count)
    {
        var offset = 0;
        Field field;

        for (var index = 0; index < count; index++)
        {
            field = new Field (st, index, offset);
            set (index, field);
            //Items[index] = field;
            offset += field.Size;
        }
    }

    public override string GetKey (IField item) => item.Name;

    public static IEnumerable <(string Path, len_t Offset, IField Field)> Flatten (Table <string, IField> table)
    {
        var SEPARATOR = ".";
        var stack = new Stack <(string path, len_t offset, Table <string, IField> st, int index)> ();
        stack.Push (("", 0, table, 0));
          
        while (stack.Count > 0)
        {
            var (path, offset, st, index) = stack.Pop ();
                    
            for (; index < st.Count;)
            {
                    var field = st[index];
                    if (field.Type.Struct == null || field.IsPointer)
                    {
                        yield return (
                            path + field.Name,
                            offset + field.Offset,
                            field
                        );
                        index++;
                    }
                    else
                    {
                        stack.Push ((path, offset, st, index + 1));
                        st = field.Type.Struct.Fields;
                        index = 0;
                        path += field.Name + SEPARATOR;
                        offset += field.Offset;
                    }
            }
        }
    }
}

// +--------+--------+--------+--------+
// |    TypeIndex    |    NameIndex    |
// +--------+--------+--------+--------+

public class Field : IField
{
    public const len_t SIZE = 4;

    public Field (Struct structure, int index, len_t offset)
    {
        IsValid = true;

        var blend = structure.OwnerFile;
        var pos   = structure.BufferPosition + Struct.HEADER_SIZE + 4 * index;

        Index     = index;
        TypeIndex = blend.Buffer.i16 (pos);
        NameIndex = blend.Buffer.i16 (pos + 2);
        Offset    = offset;

        Type = blend.Types[TypeIndex];
        name = blend.Names[NameIndex];
        Size = name.IsPointer
            ? blend.PointerSize * name.TotalCount
            : blend.Lengths[TypeIndex] * name.TotalCount;
    }

    readonly Name name;

    public bool IsValid { get; }

    public int Index { get; }
    public short TypeIndex { get; }
    public short NameIndex { get; }
    public len_t Offset { get; }
    public IType Type { get; }
    public string Name => name.Identifier;
    public len_t Size { get; }


    public bool IsPointer => name.IsPointer;
    public bool IsMethodPointer => name.IsMethodPointer;
    public bool IsArray => name.IsArray;
    public bool IsMatrix => name.IsMatrix;

    public int Level => name.Level;
    public len_t Size1 => name.Size1;
    public len_t Size2 => name.Size2;
    public len_t Size3 => name.Size2;
    public len_t TotalCount => name.TotalCount;


    public override string ToString () => $" {Type} {Name} ";

    public IField.OverType GetOverType ()
    {
        return name.IsPointer ? IField.OverType.Pointer
            : Type.Struct != null ? IField.OverType.Structure
            : name.Level == 3 ? IField.OverType.ArrayOfTable
            : name.Level == 2 ? IField.OverType.Table
            : name.Level == 1 && Type.Name == "char" && name.Size1 != 1 ? IField.OverType.String
            : name.Level == 1 ? IField.OverType.Array : IField.OverType.Scalar;
    }
}

public static class TypeLib
{
    public readonly static System.Type char_t  = typeof (byte);

    public readonly static System.Type i8_t  = typeof (byte);
    public readonly static System.Type u8_t  = typeof (sbyte);
    public readonly static System.Type i16_t = typeof (short);
    public readonly static System.Type u16_t = typeof (ushort);
    public readonly static System.Type i32_t = typeof (int);
    public readonly static System.Type u32_t = typeof (uint);
    public readonly static System.Type i64_t = typeof (long);
    public readonly static System.Type u64_t = typeof (ulong);
    public readonly static System.Type f32_t = typeof (float);
    public readonly static System.Type f64_t = typeof (double);

    public readonly static Dictionary <string, (string CName, System.Type SystemType, int Size)> Scalars = new ()
    {
        { "char"     , ("char"     , i8_t , 1) },
        { "uchar"    , ("uchar"    , u8_t , 1) },
        { "short"    , ("short"    , i16_t, 2) },
        { "ushort"   , ("ushort"   , u16_t, 2) },
        { "int"      , ("int"      , i32_t, 4) },
        { "uint"     , ("uint"     , u32_t, 4) },
        { "long"     , ("long"     , i32_t, 4) },
        { "ulong"    , ("ulong"    , u32_t, 4) },
        { "float"    , ("float"    , f32_t, 4) },
        { "double"   , ("double"   , f64_t, 8) },
        { "int64_t"  , ("int64_t"  , i64_t, 8) },
        { "uint64_t" , ("uint64_t" , u64_t, 8) },
    };
}

public class Type : IType
{
    public Type (File blend, int index, string str)
    {
        OwnerFile = blend;
        Index = index;
        Name = str;
    }

    public int Index { get; }
    public string Name { get; }
    public IStruct? Struct { get; internal set; }
    public System.Type SystemType => TypeLib.Scalars[Name].SystemType;
    public len_t Size => OwnerFile.Lengths[Index];
    public int CompareTo (IType? other) => other == null ? 1 : Name.CompareTo (other.Name);

    readonly File OwnerFile;

    public override string ToString () => Name;
}

readonly struct Name
{
    public Name (int index, string str)
    {
        Identifier = str;
        Index = index;
     
        IsPointer = str.IndexOf ('*') != -1;
        IsMethodPointer = str.IndexOf ("(*") != -1;
     
        Level = 0;
        Size1 = Size2 = Size3 = 1;
        var i = str.IndexOf ('[');
        if (i < 0) return;
     
        Level++;
        var s = str.Substring (i + 1);
        i = s.IndexOf (']');
        Size1 = int.Parse (s.Substring (0, i));
     
        i = s.IndexOf ('[');
        if (i < 0) return;
     
        Level++;
        s = s.Substring (i + 1);
        i = s.IndexOf (']');
        Size2 = int.Parse (s.Substring (0, i));
     
        i = s.IndexOf ('[');
        if (i < 0) return;
     
        Level++;
        s = s.Substring (i + 1);
        i = s.IndexOf (']');
        Size3 = int.Parse (s.Substring (0, i));
     
        #if DEBUG
        i = s.IndexOf ('[');
        if (i != -1) throw new Exception ("Multi array");
        #endif
    }

    public readonly string Identifier ;
    public readonly int Index;
    public readonly bool IsPointer;
    public readonly bool IsMethodPointer;

    public readonly int Level;

    public readonly len_t Size1;
    public readonly len_t Size2;
    public readonly len_t Size3;
    public len_t TotalCount => Size1 * Size2 * Size3;
    public bool IsArray => Level > 0;
    public bool IsMatrix => Level == 2;
     
    public override string ToString () => Identifier;
}
