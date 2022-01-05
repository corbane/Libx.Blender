namespace Libx.Blender.Controls;

using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;

using adr_t = IO.adr_t;

#nullable enable


public class BlockStore : TreeStore
{
    public IO.File? Blend { get; set; }


    #region Initialization

    public void Load (IO.File? blend)
    {
        Blend = blend;

        if (blend == null)
            Clear ();
        else
            Update ();

        Emit (nameof (Blend));
    }

    #endregion


    #region Columns

    public enum ColumnIndex
    { 
        Code = 0,
        Index, 
        Value, 
        Type, 
        Address 
    }

    public override IEnumerable <TreeColumn> GetColumns () => new TreeColumn[]
    {
        new TreeColumn ((int)ColumnIndex.Code)
        {
            HeaderText = "Code | ShortName",
            Comparison = CompareCode
        },
        new TreeColumn ((int)ColumnIndex.Index)
        {
            HeaderText = "Index",
            Comparison = CompareIndex
        },
        new TreeColumn ((int)ColumnIndex.Value)
        {
            HeaderText = "Value | [position : size]",
        },
        new TreeColumn ((int)ColumnIndex.Type)
        {
            HeaderText = "Type",
            Comparison = CompareType
        },
        new TreeColumn ((int)ColumnIndex.Address)
        {
            HeaderText = "Address",
            Comparison = CompareAddresses
        }
    };

    #endregion


    #region Rows

    protected override TreeItem[] CreateItems ()
    {
        if (Blend == null)
            return new TreeItem[0];

        var items = new TreeItem[Blend.Blocks.Count];
        foreach (var block in Blend.Blocks)
            items[block.BlockIndex] = new BlockItem (block);
        return items;
    }

    public int GetRowIndex (adr_t adr)
    {
        return Blend == null ? -1 : Blend.Blocks.IndexOf (adr);
    }

    static int CompareIndex (TreeItem a, TreeItem b)
    {
        if (a.Tag == null) return b.Tag == null ? 0 : -1;
        if (b.Tag == null) return 1;
        return ((IO.Block)a.Tag).BlockIndex.CompareTo (((IO.Block)b.Tag).BlockIndex);
    }

    static int CompareCode (TreeItem a, TreeItem b)
    {
        var tagA = a.Tag as IO.Block;
        var tagB = b.Tag as IO.Block;

        if (tagA == null) return tagB == null ? 0 : -1;
        if (tagB == null) return 1;

        var r = tagA.Code.CompareTo (tagB.Code);
        if (r != 0) return r;
        return tagA.BlockIndex.CompareTo (tagB.BlockIndex);
    }

    static int CompareType (TreeItem a, TreeItem b)
    {
        var tagA = a.Tag as IO.Block;
        var tagB = b.Tag as IO.Block;

        if (tagA == null) return tagB == null ? 0 : -1;
        if (tagB == null) return 1;

        var r = tagA.Struct.TypeIndex.CompareTo (tagB.Struct.TypeIndex);
        if (r != 0) return r;
        return tagA.BlockIndex.CompareTo (tagB.BlockIndex);
    }

    static int CompareAddresses (TreeItem a, TreeItem b)
    {
        var tagA = a.Tag as IO.Block;
        var tagB = b.Tag as IO.Block;

        if (tagA == null) return tagB == null ? 0 : -1;
        if (tagB == null) return 1;

        var r = tagA.OldAddress.CompareTo (tagB.OldAddress);
        if (r != 0) return r;
        return tagA.BlockIndex.CompareTo (tagB.BlockIndex);
    }

    #endregion
}

public class BlockItem : TreeItem
{
    public BlockItem (IO.Block block) : base (block)
    {
        BaseBackgroundColor = ColorLib.GetAssociatedColor (block);
    }

    public IO.Block Block => (IO.Block)Tag;

    public override string GetValue (int index)
    {
        return (BlockStore.ColumnIndex)index switch
        {
            BlockStore.ColumnIndex.Code    => Block.Code.ToString (),
            BlockStore.ColumnIndex.Index   => Block.BlockIndex.ToString (),
            BlockStore.ColumnIndex.Value   => $"[ {Block.HeaderPosition} : +{Block.BodyLength} ]",
            BlockStore.ColumnIndex.Type    => Block.Struct.ToString ()!,
            BlockStore.ColumnIndex.Address => Block.OldAddress.ToString (),
            _ => "Not Implemented"
        };
    }

    public override bool Expandable => true;

    protected override TreeItem[] _CreateChildren ()
    {
        var block = (IO.Block) Tag;
        var fields = block.Struct.Fields;
        var children = new ValueItem[fields.Count];

        var i = 0;
        ValueItem child;
        foreach (var field in block.Struct.Fields)
        {
            child = new ValueItem (block, field);
            child.Parent = this;
            children[i++] = child;
        }

        return children;
    }
}

public class ValueItem : TreeItem
{
    static ValueItem[] _empty = new ValueItem[0];

    public ValueItem (IO.Part block, IO.IField field) : base (new IO.Value (block, field))
    {
        Field = field;
    }

    public readonly IO.IField Field;

    public override string GetValue (int index)
    {
        return (BlockStore.ColumnIndex)index switch
        {
            BlockStore.ColumnIndex.Code    => Field.Name.ToString (),
            BlockStore.ColumnIndex.Index   => Field.Index.ToString (),
            BlockStore.ColumnIndex.Value   => Tag.ToString ()!,
            BlockStore.ColumnIndex.Type    => Field.Type.ToString ()!,
            BlockStore.ColumnIndex.Address => "",
            _ => "Not Implemented"
        };
    }

    public override bool Expandable => Field.Type.Struct != null;

    protected override TreeItem[] _CreateChildren ()
    {
        var data = ((IO.Value)Tag).Data ();
        if (data == null)
            return _empty;

        var sub_block = data as IO.Part;
        if (sub_block == null)
            return _empty;

        var fields = sub_block.Struct.Fields;
        var children = new ValueItem[fields.Count];

        var i = 0;
        ValueItem child;
        foreach (var field in sub_block.Struct.Fields)
        {
            child = new ValueItem (sub_block, field);
            child.Parent = this;
            children[i++] = child;
        }
        return children;
    }
}

public class BlockTable : TreeGridView // TreeControl
{
    TreeItem? _active_item;

    public readonly BlockStore Store;

    public BlockTable ()
    {
        DataStore = Store = new BlockStore ();
        Store.PropertyChanged += _OnDataChanged;

        foreach (var column in Store.GetColumns ())
            Columns.Add (column);
               
        CellFormatting      += _OnFormatCell;
        ColumnHeaderClick   += _OnHeaderClick;
        SelectedItemChanged += _OnSelectedItemChanged;
        CellDoubleClick     += _OnDoubleClick;
        Collapsing          += _OnExpandingCollapsing;
        Expanding           += _OnExpandingCollapsing;

        Store.GroupBy = (int)BlockStore.ColumnIndex.Code;
    }


    public void SelectOldAddress (adr_t address)
    {
        var i = Store.GetRowIndex (address);
        if (i < 0)
            return;

        var row = Store.GetExpandedRowIndex (i);
        if (row < 0)
            return;

        _SetActiveItem (Store[i]);
        ScrollToRow (row);
    }


    void _OnDataChanged (object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof (BlockStore.Blend))
        {
            SelectedRows = null;
            GC.Collect ();
            ReloadData ();
        }
    }

    void _OnFormatCell (object? sender, GridCellFormatEventArgs e)
    {
        if (e.Item is TreeItem item)
        {
            e.BackgroundColor = item.BackgroundColor;
            e.ForegroundColor = item.ForegroundColor;
        }
    }

    void _OnHeaderClick (object? sender, GridColumnEventArgs e)
    {
        SelectedRows = null;
        Invalidate (invalidateChildren: false);

        if (e.Column is TreeColumn column && column.Comparison != null)
        {
            Store.Sort (column.Comparison);
        }

        ReloadData ();
    }

    void _OnSelectedItemChanged (object? sender, EventArgs e)
    {
        if (SelectedItem is TreeItem item)
            _SetActiveItem (item);
    }

    void _OnDoubleClick (object? sender, GridCellMouseEventArgs e)
    {
        if (e.Item is TreeItem item)
        {
            item.Expanded = !item.Expanded;
            _SetActiveItem (item);
        }
    }

    void _OnExpandingCollapsing (object? sender, TreeGridViewItemCancelEventArgs e)
    {
        if (e.Item is TreeItem item)
            _SetActiveItem (item);
    }

    void _SetActiveItem (TreeItem? item)
    {
        var need_redraw = false;
        if (_active_item != null)
        {
            _active_item.ResetColors (propage: 1);
            need_redraw = true;
        }

        _active_item = item;

        if (item != null)
        {
            item.OverrideForegroundColor (Colors.DarkBlue, propage: 1);
            need_redraw = true;
        }

        if (need_redraw)
        {
            SelectedRows = null;
            Invalidate (invalidateChildren: false);
            ReloadItem (item);
        }
    }
}
