namespace Libx.Blender.IO;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using len_t = System.Int32;

#nullable enable


// public interface IOffsetTable
// {
//      // public len_t pos (int index);
//      // public len_t pos (string name);
// }

public class Data
{
    IBinaryBuffer buf;

    public Data (IBinaryBuffer buffer, IStruct format)
    { 
        buf = buffer;
        Struct = format;
    }

    public IBinaryBuffer Buffer => buf;
    IStruct Struct { get; }

    public Data sub (string key)
    {
        var f = Struct.Fields[key];
        Debug.Assert (f.Type.Struct != null);
        return new Data (buf.sub (f.Offset, f.Size), f.Type.Struct);
    }

    public len_t pos (int index)
    {
        return index < 0 || index <= Struct.Fields.Count ? -1
            : Struct.Fields[index].Offset;
    }

    public len_t pos (string name)
    {
        var f = Struct.Fields.Find (name);
        return f == null ? -1 : f.Offset;
    }

    // Data ptr (adr_t adr);
    // Data ptr (len_t p);
    // Data ptr (string k);

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

    public byte  u8  (string k) => buf.get (pos (k));
    public short i16 (string k) => BitConverter.ToInt16 (buf.g16 (pos (k), a16), 0);
    public int   i32 (string k) => BitConverter.ToInt32 (buf.g32 (pos (k), a32), 0);
    public float f32 (string k) => BitConverter.ToSingle (buf.g32 (pos (k), a32), 0);
    public adr_t adr (string k) { var p = pos (k); return p < 0 ? new adr_t (0) : buf.adr (p); }

    public byte  u8  (string k, byte d)  { var p = pos (k); return p < 0 ? d : buf.get (p); }
    public short i16 (string k, short d) { var p = pos (k); return p < 0 ? d : BitConverter.ToInt16  (buf.g16 (p, a16), 0); }
    public int   i32 (string k, int d)   { var p = pos (k); return p < 0 ? d : BitConverter.ToInt32  (buf.g32 (p, a32), 0); }
    public float f32 (string k, float d) { var p = pos (k); return p < 0 ? d : BitConverter.ToSingle (buf.g32 (p, a32), 0); }
          
    public byte   [] u8_array  (string k, len_t c0, len_t step = 1) => buf.array (buf.u8 , step, pos (k), c0, new byte   [c0]);
    public short  [] i16_array (string k, len_t c0, len_t step = 2) => buf.array (buf.i16, step, pos (k), c0, new short  [c0]);
    public int    [] i32_array (string k, len_t c0, len_t step = 4) => buf.array (buf.i32, step, pos (k), c0, new int    [c0]);
    public float  [] f32_array (string k, len_t c0, len_t step = 4) => buf.array (buf.f32, step, pos (k), c0, new float  [c0]);
          
    public byte   [][] u8_array  (string k, len_t c0, len_t c1, len_t step = 1) => buf.array (buf.u8 , step, pos (k), c0, c1, newarr <byte  > (c0, c1));
    public short  [][] i16_array (string k, len_t c0, len_t c1, len_t step = 2) => buf.array (buf.i16, step, pos (k), c0, c1, newarr <short > (c0, c1));
    public int    [][] i32_array (string k, len_t c0, len_t c1, len_t step = 4) => buf.array (buf.i32, step, pos (k), c0, c1, newarr <int   > (c0, c1));
    public float  [][] f32_array (string k, len_t c0, len_t c1, len_t step = 4) => buf.array (buf.f32, step, pos (k), c0, c1, newarr <float > (c0, c1));
          
    public string str   (string k, Encoding encoding, len_t len, string d) { var p = pos (k); return p < 0 ? d : encoding.GetString (buf.gNE   (p, len, new byte[len])); }
    public string strZ  (string k, Encoding encoding, len_t len, string d) { var p = pos (k); return p < 0 ? d : encoding.GetString (buf.getZ  (p, len, new byte[len])); }
    public string ZstrZ (string k, Encoding encoding, len_t len, string d) { var p = pos (k); return p < 0 ? d : encoding.GetString (buf.ZgetZ (p, len, new byte[len])); }

}

public abstract class Table <K, T> : IEnumerable<T> where K : IComparable <K>
{
    readonly T[] _items;
    readonly int _count;

    public Table (int count)
    {
        _count = count;
        _items = new T[count];
    }

    public Table (T[] items)
    {
        _count = items.Length;
        _items = items;
    }



    public int Count => _count;

    public T this[int index] => _items[index];

    public virtual int IndexOf (K key)
    {
        for (var i = 0; i < _count; i++)
        {
            if (GetKey (_items[i]).CompareTo (key) == 0)
                    return i;
        }
        return -1;
    }

    protected void set (int index, T value)
    {
        _items[index] = value;
    }

    public T this[K key]
    {
        get {
            var i = IndexOf (key);
            return i < 0
                    ? throw new Exception ($"Cannot find the item {key}")
                    : _items[i];
        }
    }

    public virtual T? Find (K key)
    {
        var i = IndexOf (key);
        if (i < 0)
            return default;

        return _items[i];
    }

    public abstract K GetKey (T item);

    public IEnumerator<T> GetEnumerator ()
    {
        for (var i = 0; i < Count; i++)
            yield return _items[i];
    }

    IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();

}

public interface IStruct : IComparable <IStruct> // IOffsetTable
{
    IType Type { get; }
    Table <string, IField> Fields { get; }
    len_t BufferPosition { get; }
    //len_t TotalSize { get; }
}

public interface IField //: IComparable <IField>
{
    string Name { get; }
    int Index { get; }
    IType Type { get; }
    len_t Offset { get; }
    len_t Size { get; }

    bool IsPointer { get; }
    bool IsMethodPointer { get; }
    bool IsArray { get; }
    bool IsMatrix { get; }

    int Level { get; }
    len_t Size1 { get; }
    len_t Size2 { get; }
    len_t Size3 { get; }
    len_t TotalCount { get; }


    enum OverType : short
    {
        Unknown      = 1 << 1,
        Pointer      = 2 << 1,
        Structure    = 4 << 1,
        ArrayOfTable = 8 << 1,
        Table        = 16 << 1,
        Array        = 32 << 1,
        String       = 64 << 1,
        Scalar       = 128 << 1
    }

    // enum SubType : byte
    // {
    //      UNKNOWN = 0, VOID,
    //      CHAR, UCHAR,
    //      SHORT, USHORT,
    //      INT, UINT,
    //      LONG, ULONG,
    //      FLOAT, DOUBLE,
    //      ADDRESS, STRUCT,
    //      LIST_BASE, LINK,
    // }

    OverType GetOverType ();
}

public interface IType : IComparable<IType>
{
    int Index { get; }
    string Name { get; }
    IStruct? Struct { get; }
    System.Type SystemType { get; }
    len_t Size { get; }
}
