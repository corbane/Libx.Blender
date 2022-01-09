namespace Libx.Blender.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Eto.Forms;

using len_t = System.Int32;


/*/ This control is used to display the structure types defined in the DNA1 block.
    eg. uint, ID, Object, Camera, ...

    There are two display modes, a tree mode and a flat mode.
    - The tree view groups properties within their structure types.
    - Flat view shows all properties on one level.
      This mode displays the internal recording of the Blender file.
    To change the display mode use `StructTable.Data.SetFlatten` method.

    Structures can be filtered, use `StructTable.Data.SetFilter`.
    Currently the filtering algorithm is not powerful, see `TreeControl.ApplyFilter`.

    `StructTableData` implements` INotifyPropertyChanged`.
    Use this interface to capture changes.
/*/


public class StructTableData : INotifyPropertyChanged
{
    public IO.File? Blend { get; private set; }
    public void SetBlendFile (IO.File blend)
    {
        if (Blend == blend)
            return;

        Blend = blend;
        Emit (nameof (Blend));
    }

    public bool Flatten { get; private set; }
    public void SetFlatten (bool flatten)
    {
        if (Flatten == flatten)
            return;

        Flatten = flatten;
        Emit (nameof (Flatten));
    }

    public string? Filter { get; private set; }
    public int FilterColumn { get; private set; }
    public void SetFilter (int column, string filter)
    {
        if (Filter == filter && FilterColumn == column)
            return;

        Filter = filter;
        FilterColumn = column;
        Emit (nameof (Filter));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void Emit ([CallerMemberName] string? memberName = null)
    {
        Application.Instance.Invoke (() =>
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (memberName))
        );
    }
}

public class StructTable : TreeControl
{
    const string HeaderIndex  = "Index";
    const string HeaderName   = "Struct type | Field name";
    const string HeaderOffset = "Offset";
    const string HeaderSize   = "Size";
    const string HeaderType   = "Type";
    public enum ColumnIndex
    {
        Index = 0,
        Name,
        Offset,
        Size,
        Type   
    }

    public StructTableData Data { get; }

    public StructTable () : this (new StructTableData ()) { }

    public StructTable (StructTableData data)
    {
        Data = data;
        Data.PropertyChanged += _OnStateChanged;

        AddTextColumn (HeaderIndex, e => SortStore (_CompareTypeByIndex));
        AddTextColumn (HeaderName, e => SortStore (_CompareTypeByName));
        AddTextColumn (HeaderOffset);
        AddTextColumn (HeaderSize, e => SortStore (_CompareTypeBySize));
        AddTextColumn (HeaderType);
        CellDoubleClick += _OnCellDoubleClick;
    }

    #region Store

    void _OnStateChanged (object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
        case nameof (StructTableData.Blend):

            Store.Clear ();

            if (Data.Blend == null)
            {
            }
            else
            {
                    foreach (var type in Data.Blend.Types)
                        Store.Add (_CreateItem (type));
            }

            ReloadData ();
            break;

        case nameof (StructTableData.Flatten):

            foreach (TreeGridItem item in Store)
            {
                if (item.Expanded)
                {
                    item.Children.Clear ();
                    if (item.Tag is IO.IType type && type.Struct != null)
                        _ToggleType (item, type);
                }
            }
            ReloadData ();
            break;

        case nameof (StructTableData.Filter):

            ApplyFilter ((int)ColumnIndex.Name, Data.Filter);
            break;
        }
    }

    #endregion

    #region Rows

    TreeGridItem _CreateItem (IO.IType type)
    {
        #nullable disable // m_blend == null
        return new TreeGridItem (
            /* Index  */ type.Index,
            /* Name   */ type.Name,
            /* Offset */ null,
            /* Size   */ Data.Blend.Lengths[type.Index], // type.Size.ToString (),
            /* Type   */ null
        ) { Tag = type };
        #nullable enable
    }
    TreeGridItem _CreateItem (int index, IO.IField field)
    {
        return new TreeGridItem (
            /* Index  */ index,
            /* Name   */ field.Name.ToString (),
            /* Offset */ field.Offset,
            /* Size   */ field.Size,
            /* Type   */ field.Type.Struct == null ? field.Type.Name : field.Type.Struct.ToString ()
        ) { Tag = field.Type };
    }
    TreeGridItem _CreateFlattenItem (int index, (string Path, len_t Offset, IO.IField Field) item)
    {
        return new TreeGridItem (
            /* Index  */ index,
            /* Name   */ item.Path,
            /* Offset */ item.Offset,
            /* Size   */ item.Field.Size,
            /* Type   */ item.Field.Type.Struct == null ? item.Field.Type.Name : item.Field.Type.Struct.ToString ()
        ) { Tag = item.Field.Type };
    }

    // Expand or collapse the row `TreeGridItem item`.
    void _ToggleType (TreeGridItem item, IO.IType type)
    {
        var structure = type.Struct;
        if (structure == null) return;

        var i = 0;
        if (Data.Flatten)
        {
            foreach (var mapitem in IO.FieldTable.Flatten (structure.Fields))
            {
                item.Children.Add (
                    _CreateFlattenItem (i++, mapitem)
                );
            }
        }
        else
        {
            foreach (var field in structure.Fields)
                item.Children.Add (_CreateItem (i++, field));
        }
    }

    // Comparators

    int _CompareTypeByIndex (ITreeGridItem a, ITreeGridItem b)
    {
        var tagA = ((TreeGridItem)a).Tag as IO.Type;
        var tagB = ((TreeGridItem)b).Tag as IO.Type;

        if (tagA == null) return tagB == null ? 0 : -1;
        if (tagB == null) return 1;

        return tagA.Index.CompareTo (tagB.Index);
    }

    int _CompareTypeByName (ITreeGridItem a, ITreeGridItem b)
    {
        var tagA = ((TreeGridItem)a).Tag as IO.Type;
        var tagB = ((TreeGridItem)b).Tag as IO.Type;

        if (tagA == null) return tagB == null ? 0 : -1;
        if (tagB == null) return 1;

        return tagA.Name.CompareTo (tagB.Name);
    }

    int _CompareTypeBySize (ITreeGridItem a, ITreeGridItem b)
    {
        var tagA = ((TreeGridItem)a).Tag as IO.Type;
        var tagB = ((TreeGridItem)b).Tag as IO.Type;

        if (tagA == null) return tagB == null ? 0 : -1;
        if (tagB == null) return 1;

        var lenA = Data.Blend!.Lengths[tagA.Index];
        var lenB = Data.Blend!.Lengths[tagB.Index];

        //var r = tagA.Size.CompareTo (tagB.Size);
        var r = lenA.CompareTo (lenB);
        if (r != 0) return r;
        return tagA.Name.CompareTo (tagB.Name);
    }

    #endregion

    #region Cells

    // Callback when a cell is double-clicked.
    // The goal is to expand or reduce the line.
    void _OnCellDoubleClick (object? sender, GridCellMouseEventArgs e)
    {
        var item = e.Item as TreeGridItem;
        if (item == null || e.Row < 0) return;

        item.Expanded = !item.Expanded;
        if (item.Expanded == false || item.Children.Count > 0)
        {
            //SelectedRows = null;
            ReloadData ();
            return;
        }

        if (item.Tag is IO.Type type)
            _ToggleType (item, type);

        //SelectedRows = null;
        ReloadData ();
    }

    #endregion
}
