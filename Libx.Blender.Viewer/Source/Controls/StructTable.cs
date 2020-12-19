using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable

namespace Libx.Blender.Controls
{
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


          public event PropertyChangedEventHandler? PropertyChanged;

          void Emit ([CallerMemberName] string memberName = null)
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
               }
          }

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
                              new TreeGridItem (
                                   /* Index  */ i++,
                                   /* Name   */ mapitem.Path,
                                   /* Offset */ mapitem.Offset,
                                   /* Size   */ mapitem.Field.Size,
                                   /* Type   */ mapitem.Field.Type.Struct == null ? "" : mapitem.Field.Type.Struct.ToString ()
                              )
                              { Tag = null }
                         );
                    }
               }
               else
               {
                    foreach (var field in structure.Fields)
                         item.Children.Add (_CreateItem (i++, field));
               }
          }
          
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

     }
}
