using Eto.Forms;
using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using Libx.Blender.Controls;

#nullable enable

namespace Libx.Blender.Forms
{
     public class NamesPage : TabPage // TableTabPage
     {
          const string HeaderIndex = "Index";
          const string HeaderName = "Identifier";
          const string HeaderShortName = "Short name";

          readonly TreeControl Tree;
          readonly SearchBox IdentifierSearchBox;

          public NamesPage ()
          {
               Text = "Names";

               IdentifierSearchBox = new SearchBox ();
               IdentifierSearchBox.TextChanged += (sender, e) => Tree!.ApplyFilter (1, IdentifierSearchBox.Text);

               Tree = new TreeControl ();
               Tree.AddTextColumn (HeaderIndex, e => Tree.SortStore (CompareIndex));
               Tree.AddTextColumn (HeaderName, e => Tree.SortStore (CompareName));
               Tree.AddTextColumn (HeaderShortName, e => Tree.SortStore (CompareShortName));
               Tree.CellFormatting += Tree_CellFormatting;

               var header = new StackLayout { Orientation = Orientation.Horizontal, Style = "h-bar" };
               header.Items.Add (new Label { Text = " Search name " });
               header.Items.Add (new StackLayoutItem (IdentifierSearchBox, HorizontalAlignment.Stretch, expand: true));

               var layout = new StackLayout { Orientation = Orientation.Vertical };
               layout.Items.Add (new StackLayoutItem (header, HorizontalAlignment.Stretch));
               layout.Items.Add (new StackLayoutItem (Tree, HorizontalAlignment.Stretch, expand: true));

               Content = layout;
          }

          static Font Monospace = Fonts.Monospace (SystemFonts.User ().Size);
          private void Tree_CellFormatting (object? sender, GridCellFormatEventArgs e)
          {
               if (e.Column.ID == "1")
                    e.Font = Monospace;
          }

          // public void Fill (IO.File blend)
          // {
          //      Tree.Store.Clear ();
          //      foreach (var field in blend.Names)
          //      {
          //           var id = field.Identifier;
          //           if (char.IsLetter (id[0])) id = " " + id;
          //           if (char.IsLetter (id[1])) id = " " + id;
          //           // id = "." + id;
          // 
          //           Tree.Store.Add (
          //                new TreeGridItem (field.Index, id, field.ShortName ())
          //                { Tag = field }
          //           );
          //      }
          //      Tree.ReloadData ();
          // }

          int CompareIndex (ITreeGridItem a, ITreeGridItem b)
          {
               var tagA = ((TreeGridItem)a).Tag as IO.Name;
               var tagB = ((TreeGridItem)b).Tag as IO.Name;

               if (tagA == null) return tagB == null ? 0 : -1;
               if (tagB == null) return 1;

               return tagA.Index.CompareTo (tagB.Index);
          }
          int CompareShortName (ITreeGridItem a, ITreeGridItem b)
          {
               var tagA = ((TreeGridItem)a).Tag as IO.Name;
               var tagB = ((TreeGridItem)b).Tag as IO.Name;

               if (tagA == null) return tagB == null ? 0 : -1;
               if (tagB == null) return 1;

               var r = tagA.ShortName ().CompareTo (tagB.ShortName ());
               if (r != 0) return r;
               return tagA.Identifier.CompareTo (tagB.Identifier);
          }
          int CompareName (ITreeGridItem a, ITreeGridItem b)
          {
               var tagA = ((TreeGridItem)a).Tag as IO.Name;
               var tagB = ((TreeGridItem)b).Tag as IO.Name;

               if (tagA == null) return tagB == null ? 0 : -1;
               if (tagB == null) return 1;

               return tagA.Identifier.CompareTo (tagB.Identifier);
          }
     }
}
