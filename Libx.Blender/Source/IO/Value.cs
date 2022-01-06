namespace Libx.Blender.IO;

using System;
using System.Text;

using len_t = System.Int32;
using adr_t = System.UInt64;

#nullable enable


public class Value
{
    readonly Part Block;
    readonly IField Field;

    public Value (Part block, IField field)
    {
        Block = block;
        Field = field;
        IsValid = block != null && field != null
                    && field.Type != null && Field.Name != null;
    }

    public bool IsValid { get; }

    public object? Data ()
    {
        var type = Field.Type;
        var offset = Field.Offset;

        return Field.GetOverType () switch
        {
            IField.OverType.Pointer      => Block.OwnerFile.GetBlock (Block.OwnerFile.Buffer.adr (Block.BodyPosition + offset)),
            IField.OverType.Structure    => Block.sub (Field),
            IField.OverType.ArrayOfTable => Block.box_array (type.SystemType, offset, Field.Size1, Field.Size2, Field.Size3),
            IField.OverType.Table        => Block.box_array (type.SystemType, offset, Field.Size1, Field.Size2),
            IField.OverType.String       => Block.strZ (Encoding.UTF8, offset, Field.Size1),
            IField.OverType.Array        => Block.box_array (type.SystemType, offset, Field.Size1),
            IField.OverType.Scalar       => Block.box (type.SystemType, offset),
            _ => throw new Exception ("")
        };
    }

    public override string ToString ()
    {
        if (IsValid == false)
            return "INVLID VALUE";

        var data = Data ();

        return Field.GetOverType () switch
        {
            IField.OverType.Pointer      => data == null ? "" : data.ToString ()!,
            IField.OverType.Structure    => data == null ? "! NULL INLINE STRUCT" : data.ToString ()!,
            IField.OverType.ArrayOfTable => "Not implemented",
            IField.OverType.Table        => "Not implemented",
            IField.OverType.String       => data == null ? "" : "\"" + data.ToString () + "\"",
            IField.OverType.Array        => data == null ? "" : "[" + string.Join (", ", (object[])data) + "]",
            IField.OverType.Scalar       => data == null ? "NULL DATA" : data.ToString ()!,
            _ => throw new Exception ("")
        };
    }

}
