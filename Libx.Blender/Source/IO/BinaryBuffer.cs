namespace Libx.Blender.IO;

using System;
using System.Text;

using len_t = System.Int32;
//using adr_t = System.UInt64;

#nullable enable


public interface IBinaryReader : IBinaryBuffer
{
    len_t CurrentPosition { get; }
    bool IsEof { get; }
    void seek (len_t position);
    void jump (len_t offset);
    void jumpZ ();
    byte[] read     (len_t count);
    byte[] read_NE  (len_t count);
    byte[] read_NE  (len_t count, byte[] arr);
    byte   read_i8  ();
    sbyte  read_u8  ();
    short  read_i16 ();
    ushort read_u16 ();
    int    read_i32 ();
    uint   read_u32 ();
    long   read_i64 ();
    ulong  read_u64 ();
    float  read_f32 ();
    double read_f64 ();
    adr_t read_adr  ();
    string read_str (Encoding encoding, len_t maxSize = 512);
}

public class BinaryReader : BinaryBuffer, IBinaryReader
{
    public BinaryReader (byte[] buffer, Endian endian, bool sys64) : base (buffer, endian, sys64)
    {
    }

    //readonly byte[] a16 = new byte[2];
    //readonly byte[] a32 = new byte[4];
    //readonly byte[] a64 = new byte[8];
    //public short  read_i16 () { var r = BitConverter.ToInt16  (g16 (pos, a16), 0); pos += 2; return r; }
    //public ushort read_u16 () { var r = BitConverter.ToUInt16 (g16 (pos, a16), 0); pos += 2; return r; }
    //public int    read_i32 () { var r = BitConverter.ToInt32  (g32 (pos, a32), 0); pos += 4; return r; }
    //public uint   read_u32 () { var r = BitConverter.ToUInt32 (g32 (pos, a32), 0); pos += 4; return r; }
    //public long   read_i64 () { var r = BitConverter.ToInt64  (g64 (pos, a64), 0); pos += 8; return r; }
    //public ulong  read_u64 () { var r = BitConverter.ToUInt64 (g64 (pos, a64), 0); pos += 8; return r; }
    //public float  read_f32 () { var r = BitConverter.ToSingle (g32 (pos, a32), 0); pos += 4; return r; }
    //public double read_f64 () { var r = BitConverter.ToDouble (g64 (pos, a64), 0); pos += 8; return r; }

    T[][] newarr <T> (len_t len1, len_t len2)
    {
        var arr = new T[len1][];
        for (len_t i = 0; i < len1; i++)
            arr[i] = new T[len2];
        return arr;
    }

    len_t p;
    public len_t CurrentPosition => p;
    public bool IsEof => p >= BodyLength;

    public void seek (len_t position) => p = position;
    public void jump (len_t offset) => p += offset;
    public void jumpZ ()
    {
        for (; p < BodyLength; p++)
        {
            if (Body[p] != 0) break;
        }
    }

    byte read () => get (p++);
    public byte[] read     (len_t count) { var r = get (p, count, new byte[count]); p += count; return r; }
    public byte[] read_NE  (len_t count) { var r = gNE (p, count, new byte[count]); p += count; return r; }
    public byte[] read_NE  (len_t count, byte[] arr) { var r = gNE (p, count, arr); p += count; return r; }
    public byte   read_i8  () => get (p++);
    public sbyte  read_u8  () => (sbyte)get (p++);
    public short  read_i16 () { var r = i16 (p); p += 2; return r; }
    public ushort read_u16 () { var r = u16 (p); p += 2; return r; }
    public int    read_i32 () { var r = i32 (p); p += 4; return r; }
    public uint   read_u32 () { var r = u32 (p); p += 4; return r; }
    public long   read_i64 () { var r = i64 (p); p += 8; return r; }
    public ulong  read_u64 () { var r = u64 (p); p += 8; return r; }
    public float  read_f32 () { var r = f32 (p); p += 4; return r; }
    public double read_f64 () { var r = f64 (p); p += 8; return r; }
    public adr_t  read_adr ()
    {
        adr_t r;
        if (m_64)
        {
            r = u64_adr (p);
            p += 8;
        }
        else
        {
            r = u32_adr (p);
            p += 4;
        }
        return r;
    }
    public string read_str (Encoding encoding, len_t maxSize = 512)
    {
        if (maxSize < 0)
            throw new ArgumentOutOfRangeException (nameof (maxSize));

        var bytes = new byte[maxSize];
        len_t i = 0;
        byte b;

        for (; ; )
        {
            if (IsEof)
                    throw new Exception ($"End of stream reached, but no terminator `{0}` found");

            if (i == maxSize)
                    throw new Exception ($"Max string allocation");

            b = read ();
            bytes[i++] = b;
            if (b == 0)
                    break;
        }

        return encoding.GetString (bytes, 0, i - 1);
    }

}


public interface IBinaryBuffer
{
    byte[] Body { get; }
    len_t BodyPosition { get; }
    len_t BodyLength { get; }
    Endian ByteOrder { get; }
    int PointerSize { get; }

    IBinaryBuffer sub (len_t offset, len_t size);

    len_t  align (len_t pos, len_t count);

    byte   get (len_t pos);
    byte[] g16 (len_t pos, byte[] arr);
    byte[] g32 (len_t pos, byte[] arr);
    byte[] g64 (len_t pos, byte[] arr);
    byte[] get (len_t pos, len_t len, byte[] arr);
    byte[] gNE (len_t pos, len_t len, byte[] arr);
    byte[] getZ (len_t pos, len_t len, byte[] arr);
    byte[] ZgetZ (len_t pos, len_t len, byte[] arr);

    sbyte  i8 (len_t pos);
    byte   u8 (len_t pos);
    short  i16 (len_t pos);
    ushort u16 (len_t pos);
    int    i32 (len_t pos);
    uint   u32 (len_t pos);
    long   i64 (len_t pos);
    ulong  u64 (len_t pos);
    float  f32 (len_t pos);
    double f64 (len_t pos);
    adr_t  adr (len_t pos);

    string str (Encoding encoding, len_t pos, len_t len);
    string strZ (Encoding encoding, len_t pos, len_t len);
    string ZstrZ (Encoding encoding, len_t pos, len_t len);

    T[]   array <T> (Func <len_t, T> get, len_t size, len_t pos, len_t count, T[] arr);
    T[][] array <T> (Func <len_t, T> get, len_t size, len_t pos, len_t c0, len_t c1, T[][] arr);

    object       box (System.Type type, len_t pos);
    object[]     box_array (System.Type type, len_t pos, len_t c0);
    object[][]   box_array (System.Type type, len_t pos, len_t c0, len_t c1);
    object[][][] box_array (System.Type type, len_t pos, len_t c0, len_t c1, len_t c2);
}

public class BinaryBuffer : IBinaryBuffer //, IOffsetTable
{
    protected readonly byte[] m_bytes;
    protected readonly bool m_64;
    protected readonly bool m_le;
    protected readonly bool m_need_reverse;

    public BinaryBuffer (byte[] buffer, Endian endian, bool sys64)
    {
        m_bytes = buffer;
        m_64    = sys64;
        m_le    = endian == Endian.Le;
        m_need_reverse = false == (BitConverter.IsLittleEndian == m_le);
        BodyPosition = 0;
        BodyLength = buffer.Length;
    }

    public BinaryBuffer (IBinaryBuffer access, len_t offset, len_t size)
    {
        m_bytes = access.Body;
        m_64 = access.PointerSize == 8;
        m_le = access.ByteOrder == Endian.Le;
        m_need_reverse = false == (BitConverter.IsLittleEndian == m_le);
        BodyPosition = access.BodyPosition + offset;
        BodyLength   = size;
    }

    // Basé sur des cléfs
    // public virtual int pos (int index) => -1;
    // public virtual int pos (string name) => -1;

    public virtual IBinaryBuffer sub (len_t offset, len_t size) => new BinaryBuffer (this, BodyPosition + offset, size);

    public byte[] Body => m_bytes;

    public len_t BodyPosition { get; }

    public len_t BodyLength { get; }

    public Endian ByteOrder => m_le ? Endian.Le : Endian.Be;

    public int PointerSize => m_64 ? 8 : 4;

    public virtual byte   get  (len_t pos) => Body[BodyPosition + pos];
    public virtual byte[] g16 (len_t pos, byte[] arr)
    {
        pos += BodyPosition;
        if (m_need_reverse)
        {
            arr[0] = Body[pos + 1];
            arr[1] = Body[pos];
        }
        else
        {
            arr[0] = Body[pos];
            arr[1] = Body[pos + 1];
        }
        return arr;
    }
    public virtual byte[] g32 (len_t pos, byte[] arr)
    {
        pos += BodyPosition;
        if (m_need_reverse)
        {
            arr[0] = Body[pos + 3];
            arr[1] = Body[pos + 2];
            arr[2] = Body[pos + 1];
            arr[3] = Body[pos];
        }
        else
        {
            arr[0] = Body[pos];
            arr[1] = Body[pos + 1];
            arr[2] = Body[pos + 2];
            arr[3] = Body[pos + 3];
        }
        return arr;
    }
    public virtual byte[] g64 (len_t pos, byte[] arr)
    {
        pos += BodyPosition;
        if (m_need_reverse)
        {
            arr[0] = Body[pos + 7];
            arr[1] = Body[pos + 6];
            arr[2] = Body[pos + 5];
            arr[3] = Body[pos + 4];
            arr[4] = Body[pos + 3];
            arr[5] = Body[pos + 2];
            arr[6] = Body[pos + 1];
            arr[7] = Body[pos];
        }
        else
        {
            arr[0] = Body[pos];
            arr[1] = Body[pos + 1];
            arr[2] = Body[pos + 2];
            arr[3] = Body[pos + 3];
            arr[4] = Body[pos + 4];
            arr[5] = Body[pos + 5];
            arr[6] = Body[pos + 6];
            arr[7] = Body[pos + 7];
        }
        return arr;
    }
    public virtual byte[] get (len_t pos, len_t len, byte[] arr)
    {
        pos += BodyPosition;
        if (m_need_reverse)
            for (len_t i = 0, j = len - 1; i < len; i++, j--) arr[j] = Body[pos + i];
        else
            for (len_t i = 0; i < len; i++) arr[i] = Body[pos + i];
        return arr;
    }
    public virtual byte[] gNE (len_t pos, len_t len, byte[] arr)
    {
        pos += BodyPosition;
        for (len_t i = 0; i < len; i++) arr[i] = Body[pos + i];
        return arr;
    }
    public virtual byte[] getZ (len_t pos, len_t len, byte[] arr)
    {
        pos += BodyPosition;
        byte b;
        len_t i;
        for (i = 0; i < len; i++)
        {
            b = Body[pos + i];
            if (b == 0) break;
            arr[i] = b;
        }
        if (i < len)
        {
            var trimed = new byte[i];
            Array.Copy (arr, 0, trimed, 0, i);
            arr = trimed;
        }
        //if (m_need_reverse) Array.Reverse (bytes, 0, bytes.Length);
        return arr;
    }
    public virtual byte[] ZgetZ (len_t pos, len_t len, byte[] arr)
    {
        var p = BodyPosition + pos;
        len_t i;
        for (i = 0; i < len; i++)
        {
            if (Body[p + i] != 0) break;
        }
        return getZ (pos + i, len - i, arr);
    }

    public len_t align (len_t pos, len_t count)
    {
        pos += BodyPosition;
        var n = (count - pos) % count;
        if (n < 0) n += count;
        return pos + n;
    }


    readonly byte[] a16 = new byte[2];
    readonly byte[] a32 = new byte[4];
    readonly byte[] a64 = new byte[8];
          
    T[][] newarr <T> (len_t len1, len_t len2)
    {
        var arr = new T[len1][];
        for (len_t i = 0; i < len1; i++)
            arr[i] = new T[len2];
        return arr;
    }
    T[][][] newarr <T> (len_t len1, len_t len2, len_t len3)
    {
        var arr = new T[len1][][];
        for (len_t i = 0; i < len1; i++)
        {
            arr[i] = new T[len2][];
            for (len_t j = 0; j < len1; j++)
                    arr[i][j] = new T[len3];
        }
        return arr;
    }

    public sbyte  i8  (len_t p) => (sbyte) get (p);
    public byte   u8  (len_t p) => get (p);
    public short  i16 (len_t p) => BitConverter.ToInt16  (g16 (p, a16), 0);
    public ushort u16 (len_t p) => BitConverter.ToUInt16 (g16 (p, a16), 0);
    public int    i32 (len_t p) => BitConverter.ToInt32  (g32 (p, a32), 0);
    public uint   u32 (len_t p) => BitConverter.ToUInt32 (g32 (p, a32), 0);
    public long   i64 (len_t p) => BitConverter.ToInt64  (g64 (p, a64), 0);
    public ulong  u64 (len_t p) => BitConverter.ToUInt64 (g64 (p, a64), 0);
    public float  f32 (len_t p) => BitConverter.ToSingle (g32 (p, a32), 0);
    public double f64 (len_t p) => BitConverter.ToDouble (g64 (p, a64), 0);
    public adr_t  adr (len_t p) => m_64 ? u64_adr (p) : u32_adr (p);

    public adr_t  u32_adr (len_t p) => new adr_t (BitConverter.ToUInt32 (g32 (p, a32), 0));
    public adr_t  u64_adr (len_t p) => new adr_t (BitConverter.ToUInt64 (g64 (p, a64), 0));

    // Basé sur des cléfs
    // public byte  u8  (string k) => get (pos (k)); 
    // public short i16 (string k) => BitConverter.ToInt16  (g16 (pos (k), a16), 0); 
    // public int   i32 (string k) => BitConverter.ToInt32  (g32 (pos (k), a32), 0); 
    // public float f32 (string k) => BitConverter.ToSingle (g32 (pos (k), a32), 0); 
    // public adr_t adr (string k) { var p = pos (k); return p < 0 ? new adr_t (0) : adr (p); }
    // public byte  u8  (string k, byte  d) { var p = pos (k); return p < 0 ? d : get (p); }
    // public short i16 (string k, short d) { var p = pos (k); return p < 0 ? d : BitConverter.ToInt16 (g16 (p, a16), 0); }
    // public int   i32 (string k, int   d) { var p = pos (k); return p < 0 ? d : BitConverter.ToInt32 (g32 (p, a32), 0); }
    // public float f32 (string k, float d) { var p = pos (k); return p < 0 ? d : BitConverter.ToSingle (g32 (p, a32), 0); }



    public T[] array <T> (Func <len_t, T> get, len_t size, len_t pos, len_t count, T[] arr)
    {
        for (len_t i = 0; i < count; i++)
            arr[i] = get (pos + size * i);
        return arr;
    }
          
    public byte   [] u8_array  (len_t p, len_t c0, len_t s = 1) => array (get, s, p, c0, new byte   [c0]);
    public short  [] i16_array (len_t p, len_t c0, len_t s = 2) => array (i16, s, p, c0, new short  [c0]);
    public int    [] i32_array (len_t p, len_t c0, len_t s = 4) => array (i32, s, p, c0, new int    [c0]);
    public float  [] f32_array (len_t p, len_t c0, len_t s = 4) => array (f32, s, p, c0, new float  [c0]);
    public adr_t  [] adr_array (len_t p, len_t c0) => m_64
        ? array (u64_adr, 8, p, c0, new adr_t[c0])
        : array (u32_adr, 4, p, c0, new adr_t[c0]);
          
    // Basé sur des cléfs
    // public byte   [] u8_array  (string k, len_t c0, len_t step = 1) => array (u8 , step, pos (k), c0, new byte   [c0]);
    // public short  [] i16_array (string k, len_t c0, len_t step = 2) => array (i16, step, pos (k), c0, new short  [c0]);
    // public int    [] i32_array (string k, len_t c0, len_t step = 4) => array (i32, step, pos (k), c0, new int    [c0]);
    // public float  [] f32_array (string k, len_t c0, len_t step = 4) => array (f32, step, pos (k), c0, new float  [c0]);



    public T[][] array <T> (Func <len_t, T> get, len_t size, len_t pos, len_t c0, len_t c1, T[][] arr)
    {
        len_t c = 0;
        for (len_t i = 0; i < c0; i++)
        {
            for (len_t j = 0; j < c1; j++, c++)
                    arr[i][j] = get (pos + size * c);
        }
        return arr;
    }

    public byte   [][] u8_array  (len_t pos, len_t c0, len_t c1, len_t s = 1) => array (get , s, pos, c0, c1, newarr <byte> (c0, c1));
    public short  [][] i16_array (len_t pos, len_t c0, len_t c1, len_t s = 2) => array (i16, s, pos, c0, c1, newarr <short> (c0, c1));
    public int    [][] i32_array (len_t pos, len_t c0, len_t c1, len_t s = 4) => array (i32, s, pos, c0, c1, newarr <int> (c0, c1));
    public float  [][] f32_array (len_t pos, len_t c0, len_t c1, len_t s = 4) => array (f32, s, pos, c0, c1, newarr <float> (c0, c1));
    public adr_t  [][] adr_array (len_t pos, len_t c0, len_t c1) => m_64 
        ? array (u64_adr, 8, pos, c0, c1, newarr <adr_t> (c0, c1))
        : array (u32_adr, 4, pos, c0, c1, newarr <adr_t> (c0, c1));

    // Basé sur des cléfs
    // public byte   [][] u8_array  (string k, len_t c0, len_t c1, len_t step = 1) => array (u8 , step, pos (k), c0, c1, newarr <byte  > (c0, c1));
    // public short  [][] i16_array (string k, len_t c0, len_t c1, len_t step = 2) => array (i16, step, pos (k), c0, c1, newarr <short > (c0, c1));
    // public int    [][] i32_array (string k, len_t c0, len_t c1, len_t step = 4) => array (i32, step, pos (k), c0, c1, newarr <int   > (c0, c1));
    // public float  [][] f32_array (string k, len_t c0, len_t c1, len_t step = 4) => array (f32, step, pos (k), c0, c1, newarr <float > (c0, c1));



    public T[ ][][] array <T> (Func <len_t, T> get, len_t size, len_t pos, len_t c0, len_t c1, len_t c2, T[][][] arr)
    {
        len_t c = 0;
        for (len_t i = 0; i < c0; i++)
        {
            for (len_t j = 0; j < c1; j++)
            {
                    for (len_t k = 0; k < c2; k++, c++)
                        arr[i][j][k] = get (pos + size * c);
            }
        }
        return arr;
    }
          
    public byte   [][][] u8_array  (len_t p, len_t c0, len_t c1, len_t c2, len_t s = 1) => array (get, s, p, c0, c1, c2, newarr <byte> (c0, c1, c2));
    public short  [][][] i16_array (len_t p, len_t c0, len_t c1, len_t c2, len_t s = 2) => array (i16, s, p, c0, c1, c2, newarr <short> (c0, c1, c2));
    public int    [][][] i32_array (len_t p, len_t c0, len_t c1, len_t c2, len_t s = 4) => array (i32, s, p, c0, c1, c2, newarr <int> (c0, c1, c2));
    public float  [][][] f32_array (len_t p, len_t c0, len_t c1, len_t c2, len_t s = 4) => array (f32, s, p, c0, c1, c2, newarr <float> (c0, c1, c2));
    public adr_t  [][][] adr_array (len_t p, len_t c0, len_t c1, len_t c2) => m_64
        ? array (u64_adr, 8, p, c0, c1, c2, newarr <adr_t> (c0, c1, c2))
        : array (u32_adr, 4, p, c0, c1, c2, newarr <adr_t> (c0, c1, c2));



    public string str   (Encoding encoding, len_t p, len_t len) => encoding.GetString (gNE (p, len, new byte[len]));
    public string strZ  (Encoding encoding, len_t p, len_t len) => encoding.GetString (getZ (p, len, new byte[len]));
    public string ZstrZ (Encoding encoding, len_t p, len_t len) => encoding.GetString (ZgetZ (p, len, new byte[len]));
          
    // public string str   (string k, Encoding encoding, len_t len, string d) { var p = pos (k); return p < 0 ? d : encoding.GetString (gNE   (p, len, new byte[len])); }
    // public string strZ  (string k, Encoding encoding, len_t len, string d) { var p = pos (k); return p < 0 ? d : encoding.GetString (getZ  (p, len, new byte[len])); }
    // public string ZstrZ (string k, Encoding encoding, len_t len, string d) { var p = pos (k); return p < 0 ? d : encoding.GetString (ZgetZ (p, len, new byte[len])); }

    public static string TrimZsZ (string text)
    {
        var length = text.Length;
        var start = 0;
        while (start < length && text[start] == 0) start++;
        if (start == length) return "";
        var end = start + 1;
        while (end < length && text[end] != 0) end++;
        return text.Substring (start, end - start);
    }



    Func<len_t, object> boxfunc<T> (Func<len_t, T> get)
    {
        return (len_t p) => get (p);
    }

    public object box (System.Type type, len_t pos)
    {
        if (type == TypeLib.i8_t)  return get (pos);
        if (type == TypeLib.u8_t)  return i8  (pos);
        if (type == TypeLib.i16_t) return i16 (pos);
        if (type == TypeLib.u16_t) return u16 (pos);
        if (type == TypeLib.i32_t) return i32 (pos);
        if (type == TypeLib.u32_t) return u32 (pos);
        if (type == TypeLib.i64_t) return i64 (pos);
        if (type == TypeLib.u64_t) return u64 (pos);
        if (type == TypeLib.f32_t) return f32 (pos);
        if (type == TypeLib.f64_t) return f64 (pos);
        throw new ArgumentException ($"Unknown scalar type \"{type}\"");
    }

    public object[] box_array (System.Type type, len_t pos, len_t c0)
    {
        if (type == TypeLib.i8_t)  return array (boxfunc (get), 1, pos, c0, new object[c0]);
        if (type == TypeLib.u8_t)  return array (boxfunc (i8 ), 1, pos, c0, new object[c0]);
        if (type == TypeLib.i16_t) return array (boxfunc (i16), 2, pos, c0, new object[c0]);
        if (type == TypeLib.u16_t) return array (boxfunc (u16), 2, pos, c0, new object[c0]);
        if (type == TypeLib.i32_t) return array (boxfunc (i32), 4, pos, c0, new object[c0]);
        if (type == TypeLib.u32_t) return array (boxfunc (u32), 4, pos, c0, new object[c0]);
        if (type == TypeLib.i64_t) return array (boxfunc (i64), 8, pos, c0, new object[c0]);
        if (type == TypeLib.u64_t) return array (boxfunc (u64), 8, pos, c0, new object[c0]);
        if (type == TypeLib.f32_t) return array (boxfunc (f32), 4, pos, c0, new object[c0]);
        if (type == TypeLib.f64_t) return array (boxfunc (f64), 8, pos, c0, new object[c0]);
        throw new ArgumentException ($"Unknown scalar type \"{type}\"");
    }

    public object[][] box_array (System.Type type, len_t pos, len_t c0, len_t c1)
    {
        if (type == TypeLib.i8_t)  return array (boxfunc (get), 1, pos, c0, c1, newarr <object> (c0, c1));
        if (type == TypeLib.u8_t)  return array (boxfunc (u8 ), 1, pos, c0, c1, newarr <object> (c0, c1));
        if (type == TypeLib.i16_t) return array (boxfunc (i16), 2, pos, c0, c1, newarr <object> (c0, c1));
        if (type == TypeLib.u16_t) return array (boxfunc (u16), 2, pos, c0, c1, newarr <object> (c0, c1));
        if (type == TypeLib.i32_t) return array (boxfunc (i32), 4, pos, c0, c1, newarr <object> (c0, c1));
        if (type == TypeLib.u32_t) return array (boxfunc (u32), 4, pos, c0, c1, newarr <object> (c0, c1));
        if (type == TypeLib.i64_t) return array (boxfunc (i64), 8, pos, c0, c1, newarr <object> (c0, c1));
        if (type == TypeLib.u64_t) return array (boxfunc (u64), 8, pos, c0, c1, newarr <object> (c0, c1));
        if (type == TypeLib.f32_t) return array (boxfunc (f32), 4, pos, c0, c1, newarr <object> (c0, c1));
        if (type == TypeLib.f64_t) return array (boxfunc (f64), 8, pos, c0, c1, newarr <object> (c0, c1));
        throw new ArgumentException ($"Unknown scalar type \"{type}\"");
    }
          
    public object[][][] box_array (System.Type type, len_t pos, len_t c0, len_t c1, len_t c2)
    {
        if (type == TypeLib.i8_t)  return array (boxfunc (get), 1, pos, c0, c1, c2, newarr <object> (c0, c1, c2));
        if (type == TypeLib.u8_t)  return array (boxfunc (i8 ), 1, pos, c0, c1, c2, newarr <object> (c0, c1, c2));
        if (type == TypeLib.i16_t) return array (boxfunc (i16), 2, pos, c0, c1, c2, newarr <object> (c0, c1, c2));
        if (type == TypeLib.u16_t) return array (boxfunc (u16), 2, pos, c0, c1, c2, newarr <object> (c0, c1, c2));
        if (type == TypeLib.i32_t) return array (boxfunc (i32), 4, pos, c0, c1, c2, newarr <object> (c0, c1, c2));
        if (type == TypeLib.u32_t) return array (boxfunc (u32), 4, pos, c0, c1, c2, newarr <object> (c0, c1, c2));
        if (type == TypeLib.i64_t) return array (boxfunc (i64), 8, pos, c0, c1, c2, newarr <object> (c0, c1, c2));
        if (type == TypeLib.u64_t) return array (boxfunc (u64), 8, pos, c0, c1, c2, newarr <object> (c0, c1, c2));
        if (type == TypeLib.f32_t) return array (boxfunc (f32), 4, pos, c0, c1, c2, newarr <object> (c0, c1, c2));
        if (type == TypeLib.f64_t) return array (boxfunc (f64), 8, pos, c0, c1, c2, newarr <object> (c0, c1, c2));
        throw new ArgumentException ($"Unknown scalar type \"{type}\"");
    }


}
