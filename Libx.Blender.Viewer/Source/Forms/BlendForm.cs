using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using Libx.Blender.Controls;

#nullable enable

namespace Libx.Blender.Forms
{
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

          void Emit ([CallerMemberName] string memberName = null)
          {

               Application.Instance.Invoke (() =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName))
               );
          }
     }

     public class BlendForm : Form
     {
          readonly BlendFormData Data;

          // Left bar
          readonly Button SelectBlend;

          // Main panel
          readonly BlockTable BlockTable;

          readonly StructTable TypeTable;
          readonly SearchBox NameSearchBox;

          // Status bar
          readonly CheckBox Flatten;
          readonly CheckBox Synchronize;
          readonly CheckBox OnlyParent;

          public BlendForm ()
          {
               Data = new BlendFormData ();
               Data.SetSynchronizeView (true);

               // Left bar
               SelectBlend = new Button ();

               // Main panel
               BlockTable = new BlockTable ();

               //TypesPage = new TypesPage ();
               TypeTable = new StructTable ();
               NameSearchBox = new SearchBox ();

               // Status bar
               Flatten = new CheckBox ();
               Synchronize = new CheckBox ();
               OnlyParent = new CheckBox ();

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
               AllowDrop  = true;
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


          // Header bar

          Control _CreateHeaderBar ()
          {
               SelectBlend.Text = "Open file";
               SelectBlend.Click += _OnOpen;

               return new StackLayout
               {
                    Orientation = Orientation.Horizontal,
                    Style = "h-bar",
                    Items =
                    {
                         SelectBlend,
                         null,
                    }
               };
          }


          void _OnOpen (object? sender, EventArgs e)
          {
               var dialog = new OpenFileDialog ();
               dialog.Filters.Add (new FileFilter ("blend file", ".blend"));
               dialog.Directory = new Uri (@"E:\Projet\Rhino\DevX\Libx.Blender\Demo");

               if (dialog.ShowDialog (this) == DialogResult.Ok)
               {
                    var path = dialog.Filenames.First ();
                    var blend = IO.File.From (path);

                    BlockTable.Store.Load (blend);
                    TypeTable.Data.SetBlendFile (blend);
               }
          }

          Control _CreateMainLayout ()
          {
               Synchronize.Text = "Syncronize";
               Synchronize.Checked = Data.SynchronizeView;
               Synchronize.CheckedChanged += (sender, e) => Data.SetSynchronizeView (Synchronize.Checked ?? false);

               OnlyParent.Text = "Only parent";

               Flatten.Text = "flatten";
               Flatten.CheckedChanged += _OnFlattenChange;

               NameSearchBox.TextChanged += (sender, e) => TypeTable.ApplyFilter (1, NameSearchBox.Text);

               var left = new StackLayout
               {
                    Orientation = Orientation.Vertical,
                    Width = 500,
                    Spacing = 3,
                    Items = {
                         new StackLayoutItem (NameSearchBox, HorizontalAlignment.Stretch),
                         Flatten,
                         new StackLayoutItem (TypeTable, HorizontalAlignment.Stretch, expand: true)
                    }
               };

               return new Splitter
               {
                    Orientation = Orientation.Horizontal,
                    Panel1 = left,
                    Panel2 = BlockTable
               };
                
          }


          void _OnFlattenChange (object? sender, EventArgs e)
          {
               TypeTable.Data.SetFlatten (Flatten.Checked ?? false);
          }

     }


}

#if WINDOWS

#nullable disable

namespace Libx.Blender.Forms.Wpf
{

     public class Wpf_BlendForm
     {
          [STAThread]
          public static void Main (string[] args)
          {
               new Application (Platforms.Wpf).Run (new BlendForm ());
          }
     }

}

#endif
