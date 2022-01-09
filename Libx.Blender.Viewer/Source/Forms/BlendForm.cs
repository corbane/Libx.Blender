namespace Libx.Blender.Forms;

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Eto.Drawing;
using Eto.Forms;
using Libx.Blender.Controls;

#nullable enable

enum BlendFormPanels
{
    BlockTable,
    Editor,
}

class BlendFormData : INotifyPropertyChanged
{
    public string? Error { get; private set; }
    public void SetError (string error)
    {
        if (error == null)
        {
            Error = null;
        }
        else
        {
            Error = error;
            Emit (nameof (Error));
        }
    }


    public BlendFormPanels ActivePanel { get; private set; } = BlendFormPanels.BlockTable;
    public void SetActivePanel (BlendFormPanels panel)
    {
        if (ActivePanel == panel)
            return;

        ActivePanel = panel;
        Emit (nameof (ActivePanel));
    }

    public bool SynchronizeView { get; private set; }
    public void SetSynchronizeView (bool sync)
    {
        if (SynchronizeView == sync)
            return;

        SynchronizeView = sync;
        Emit (nameof (SynchronizeView));
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    void Emit ([CallerMemberName] string? memberName = null)
    {

        Application.Instance.Invoke (() =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName))
        );
    }
}

public class BlendForm : Form
{
    [STAThread]
    public static void Main (string[] args)
    {
        // new Application (Platforms.Wpf).Run (new BlendForm ());
        new Application ().Run (new BlendForm ());
    }


    readonly BlendFormData Data;

    // Left bar
    readonly Button SelectBlend;
    readonly Label BlendInfos;

    // Main panel
    readonly BlockTable BlockTable;

    readonly StructTable StructTable;
    readonly SearchBox NameSearchBox;

    // Status bar
    readonly CheckBox Flatten;
    readonly CheckBox Synchronize;

    public BlendForm ()
    {
        Data = new BlendFormData ();
        Data.SetSynchronizeView (true);

        // Top bar
        SelectBlend = new Button ();
        BlendInfos  = new Label ();

        // Main panel
        BlockTable = new BlockTable ();

        //TypesPage = new TypesPage ();
        StructTable   = new StructTable ();
        NameSearchBox = new SearchBox ();

        // Status bar
        Flatten     = new CheckBox ();
        Synchronize = new CheckBox ();

        // Styles
        Eto.Style.Add <StackLayout> ("h-bar", h =>
        {
            h.Padding = new Padding (0, 0, 0, 5);
            h.Spacing = 5;
        });

        // Window
        Title      = "Blend File Viewer";
        ClientSize = new Size (1400, 800);
        Padding    = new Padding (5);
        Content    = new StackLayout
        { 
            Orientation = Orientation.Vertical,
            Items =
            {
                    new StackLayoutItem (_CreateHeaderBar (), HorizontalAlignment.Stretch),
                    new StackLayoutItem (_CreateMainLayout (), HorizontalAlignment.Stretch, expand: true),
            }
        };
    }

    public void LoadPath (string path)
    {
        try
        {
            var blend = IO.File.From (path);

            Title = path + " | Blender File Viewer";
            BlendInfos.Text
                = "blender: "        + blend.Version
                + " | endian: "      + blend.Endian
                + " | platform: "    + (blend.PointerSize == 4 ? "32" : "64") + " bits"
                + " | compression: " + blend.Compression
                + " | loading in "   + blend.LoadingTime + "s";
            BlockTable.Store.Load (blend);
            StructTable.Data.SetBlendFile (blend);
        }
        catch (Exception)
        {

            throw;
        }
    }


    #region Header

    Control _CreateHeaderBar ()
    {
        SelectBlend.Text = "Open file";
        SelectBlend.Click += _OnOpen;
        SelectBlend.AllowDrop = true;
        SelectBlend.DragEnter += _OnDragEnter;
        SelectBlend.DragDrop += _OnDragDrop;

        return new StackLayout
        {
            Orientation = Orientation.Horizontal,
            Style = "h-bar",
            Items =
            {
                    SelectBlend,
                    BlendInfos,
                    null
            }
        };
    }

    void _OnOpen (object? sender, EventArgs e)
    {
        var dialog = new OpenFileDialog ();
        dialog.Filters.Add (new FileFilter ("blend file", ".blend"));
        dialog.Directory = new Uri (@"E:\Projet\Rhino\DevX\Libx.Blender\Demo");

        if (dialog.ShowDialog (this) == DialogResult.Ok)
            LoadPath (dialog.Filenames.First ());
    }

    void _OnDragEnter (object? sender, DragEventArgs e)
    {
        e.Effects = DragEffects.All;
    }

    void _OnDragDrop (object? sender, DragEventArgs e)
    {
        if (e.Data.ContainsUris)
            LoadPath (e.Data.Uris[0].LocalPath);
    }

    #endregion


    Control _CreateMainLayout ()
    {
        Synchronize.Text = "Syncronize";
        Synchronize.Checked = Data.SynchronizeView;
        Synchronize.CheckedChanged += (sender, e) => Data.SetSynchronizeView (Synchronize.Checked ?? false);

        return new Splitter
        {
            Orientation = Orientation.Horizontal,
            Panel1 = _CreateStructsPane (),
            Panel2 = BlockTable
        };
                
    }

    #region Structs

    Control _CreateStructsPane ()
    {
        Flatten.Text = "flatten";
        Flatten.CheckedChanged += _OnFlattenChange;

        NameSearchBox.TextChanged += _OnSearchTextChanged;
        StructTable.Data.PropertyChanged += _OnStructTableState;
        StructTable.CellClick += _OnCellClick;

        return new StackLayout
        {
            Orientation = Orientation.Vertical,
            Width = 500,
            Spacing = 3,
            Items = {
                    new StackLayoutItem (NameSearchBox, HorizontalAlignment.Stretch),
                    Flatten,
                    new StackLayoutItem (StructTable, HorizontalAlignment.Stretch, expand: true)
            }
        };
    }

    void _OnStructTableState (object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof (StructTableData.Filter):

                if (StructTable.Data.Filter == NameSearchBox.Text ||
                    StructTable.Data.FilterColumn != (int)StructTable.ColumnIndex.Name
                ) break;

                NameSearchBox.Text = StructTable.Data.Filter;
                break;
        }
    }

    private void _OnSearchTextChanged (object? sender, EventArgs e)
    {
        StructTable.Data.SetFilter ((int)StructTable.ColumnIndex.Name, NameSearchBox.Text);
    }

    void _OnFlattenChange (object? sender, EventArgs e)
    {
        StructTable.Data.SetFlatten (Flatten.Checked ?? false);
    }

    void _OnCellClick (object? sender, GridCellMouseEventArgs e)
    {
        if (e.Modifiers != Keys.Control)
            return;

        if (e.Column != (int)StructTable.ColumnIndex.Type)
            return;

        if (e.Item is not TreeGridItem item || item.Tag is not IO.Type type)
            return;

        if (String.IsNullOrWhiteSpace (type.Name))
            return;

        StructTable.Data.SetFilter ((int)StructTable.ColumnIndex.Name, type.Name);
    }

    #endregion
}
