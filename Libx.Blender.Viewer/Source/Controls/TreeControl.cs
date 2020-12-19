using Eto.Drawing;
using Eto.Forms;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable

namespace System.Runtime.CompilerServices
{
     public class IsExternalInit { }
}

namespace Libx.Blender.Controls
{

     public abstract class TreeStore : ITreeGridStore <TreeItem>, INotifyPropertyChanged 
     {
          static TreeItem[] _empty = new TreeItem[0];
          static TreeGroup[] _empty_groups = new TreeGroup[0];

          TreeItem[] _items;
          TreeGroup[] _groups;

          int _group_by;
          public int GroupBy
          {
               get => _group_by;
               set {
                    if (_group_by == value)
                         return;

                    _group_by = value;
                    _UpdateGroups ();
                    Emit (nameof (Children));
               }
          }

          #nullable disable
          public TreeStore()
          {
               Clear ();
               Update ();
          }
          #nullable enable

          public abstract IEnumerable <TreeColumn> GetColumns ();

          protected abstract TreeItem[] CreateItems ();

          public int Count => _group_by < 0 ? _items.Length : _groups.Length;

          public TreeItem this[int index] => _group_by < 0 ? _items[index] : _groups[index];

          public IEnumerable <TreeItem> Children => _group_by < 0 ? _items : _groups;

          public void Clear ()
          {
               _items = _empty;
               _groups = _empty_groups;
               _group_by = -1;
               Emit (nameof (Children));
          }

          public void Update ()
          {
               _items = CreateItems () ?? _empty;

               var i = 0;
               foreach (var item in _items)
                    item.Index = i++;

               _UpdateGroups ();
               Emit (nameof (Children));
          }

          public int GetExpandedRowIndex (int row)
          {
               if (row < 0 || _items.Length <= row)
                    return row;

               var r = row;
               while (row > 0)
               {
                    row--;
                    if (_items[row].Expanded)
                         r += _items[row].CountDescendants ();
               }
               return r;
          }

          public void Sort (Comparison <TreeItem> comparer)
          {
               if (_items.Length < 2)
                    return;

               var span = new Span <TreeItem> (_items);
               MemoryExtensions.Sort (span, comparer);

               _UpdateGroups ();
               Emit (nameof (Children));
          }

          void _UpdateGroups ()
          {
               if (_group_by < 0)
               {
                    _groups = _empty_groups;
               }
               else
               {
                    var groups = (
                         from item in _items
                         group item by item.GetValue (_group_by) into g
                         select new TreeGroup (g.Key, g.ToArray ())
                    ).ToArray ();

                    var i = 0;
                    while (i < _groups.Length && i < groups.Length)
                    {
                         groups[i].Expanded = _groups[i++].Expanded;
                    }
                    _groups = groups;
               }
          }


          public event PropertyChangedEventHandler? PropertyChanged;

          protected void Emit ([CallerMemberName] string memberName = null)
          {
               Application.Instance.Invoke (() =>
                    PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (memberName))
               );
          }
     }

     public class TreeColumn : GridColumn
     {
          public delegate bool FilterH (TreeItem item);

          Comparison <TreeItem>? _comparison;

          public TreeColumn (int index)
          {
               Index = index;
               DataCell = new TextBoxCell { Binding = new TreeBinding (index) };
          }

          public int Index { get; }

          public Comparison <TreeItem>? Comparison
          {
               get => _comparison;
               set {
                    Sortable = value != null;
                    _comparison = value;
               }
          }

          public Predicate <TreeItem>? Filter { get; set; }
     }

     public class TreeBinding : IIndirectBinding <string>
     {
          int _index;

          public TreeBinding (int index)
          {
               _index = index;
          }

          public string GetValue (object dataItem)
          {
               return dataItem is TreeItem item
                    ? item.GetValue (_index)
                    : "BAD TREE ITEM";
          }

          public void SetValue (object dataItem, string value) { throw new NotImplementedException (); }
          public void Unbind () { }
          public void Update (BindingUpdateMode mode) { if (mode != BindingUpdateMode.Destination) throw new NotImplementedException (); }
     }

     public abstract class TreeItem : ITreeGridItem, ITreeGridStore<TreeItem>
     {
          public Color _background;
          public Color _foreground;

          public Color BaseBackgroundColor
          {
               get => _background;
               set => BackgroundColor = _background = value;
          }

          public Color BaseForegroundColor
          {
               get => _foreground;
               set => ForegroundColor = _foreground = value;
          }

          public TreeItem (object data)
          {
               Tag = data;
               BaseBackgroundColor = Colors.White;
               BaseForegroundColor = Colors.Black;
          }


          public ITreeGridItem? Parent { get; set; }


          public int Index { get; internal set; }

          public object Tag { get; }
          public virtual Color BackgroundColor { get; private set; }

          public virtual Color ForegroundColor { get; private set; }

          public abstract string GetValue (int index);


          public TreeItem this[int index]
          {
               get {
                    if (Children == null)
                         Children = _CreateChildren ();
                    return Children[index];
               }
          }

          bool _expanded = false;

          public TreeItem[]? Children { get; private set; }
          public int Count => Children == null ? 0 : Children.Length;
          public virtual bool Expandable => false;
          public bool Expanded 
          {
               get => _expanded;
               set {
                    if (value && Children == null)
                         Children = _CreateChildren ();
                    _expanded = value;
               }
          }

          public int CountDescendants ()
          {
               if (Children == null)
                    return 0;

               var count = Children.Length;
               foreach (var subitem in Children)
               {
                    if (Expanded)
                         count += subitem.CountDescendants ();
               }
               return count;
          }

          protected abstract TreeItem[] _CreateChildren ();

          public void OverrideForegroundColor (Color foreground, int propage = 0)
          {
               ForegroundColor = foreground;

               if (propage <= 0 || Children == null)
                    return;

               propage--;

               foreach (var subitem in Children)
               {
                    subitem.ForegroundColor = foreground;
                    if (Expanded)
                         subitem.OverrideForegroundColor (foreground, propage);
               }
          }

          public void OverrideColors (Color background, Color foreground, int propage = 0)
          {
               BackgroundColor = background;
               ForegroundColor = foreground;

               if (propage <= 0 || Children == null)
                    return;

               propage--;

               foreach (var subitem in Children)
               {
                    subitem.BackgroundColor = background;
                    subitem.ForegroundColor = foreground;
                    if (Expanded)
                         subitem.OverrideColors (background, foreground, propage);
               }
          }

          public void ResetColors (int propage = 0)
          {
               BackgroundColor = _background;
               ForegroundColor = _foreground;

               if (propage <= 0 || Children == null)
                    return;

               propage--;

               foreach (var subitem in Children)
               {
                    if (Expanded)
                         subitem.ResetColors (propage);
               }
          }
     }

     public class TreeGroup : TreeItem
     {
          static TreeItem[] _empty = new TreeItem[0];

          string _key;
          TreeItem[] _items;

          public TreeGroup (string key, TreeItem[] group) : base (group)
          {
               _key = key;
               _items = group;
          }

          public override bool Expandable => _items.Length > 0;

          public override string GetValue (int index)
          {
               return index == 0 ? _key : "";
          }

          protected override TreeItem[] _CreateChildren ()
          {
               var group = (TreeItem[])Tag;
               return group == null ? _empty : group;
          }
     }



     public class TreeControl : TreeGridView
     {
          public delegate void OnHeaderClickCallback (GridColumnEventArgs e);

          int m_current_col_index;
          readonly Dictionary <string, OnHeaderClickCallback> m_header_callbacks;

          public TreeGridItemCollection Store { get; }

          public TreeControl ()
          {
               m_current_col_index = 0;
               DataStore = Store = new TreeGridItemCollection ();
               m_header_callbacks = new ();
               ColumnHeaderClick += OnHeaderClick;
          }

          public int AddTextColumn (string title, OnHeaderClickCallback? callback = null)
          {
               var id = Columns.Count;
               Columns.Add (new GridColumn
               {
                    ID = id.ToString (),
                    HeaderText = title,
                    DataCell = new TextBoxCell (m_current_col_index++),
                    Sortable = callback != null
               });
               if (callback != null)
                    m_header_callbacks.Add (id.ToString (), callback);
               return id;
          }

          public string AddImageTextColumn (string title, OnHeaderClickCallback? callback = null)
          {
               var id = Columns.Count.ToString ();
               var cell = new ImageTextCell (m_current_col_index++, m_current_col_index++);
               Columns.Add (new GridColumn
               {
                    ID = id,
                    HeaderText = title,
                    DataCell = cell,
                    Sortable = callback != null
               });
               if (callback != null)
                    m_header_callbacks.Add (id, callback);
               return id;
          }

          void OnHeaderClick (object? sender, GridColumnEventArgs e)
          {
               if (m_header_callbacks.ContainsKey (e.Column.ID) == false)
                    return;

               m_header_callbacks[e.Column.ID] (e);
          }

          public void SortStore (Comparison <ITreeGridItem> comparison)
          {
               SelectedRows = null;
               Store.Sort (comparison);
               ReloadData ();
          }

          public void SelectDescendant (TreeGridItem item)
          {
               var current = item;

               while (current.Parent != null)
                    current = (TreeGridItem) current.Parent;
               
               var row = FindChildIndex (current);
               if (row > 0) SelectRow (row + CountDescendants (current));
          }

          public int FindChildIndex (TreeGridItem item)
          {
               var i = 0;
               foreach (TreeGridItem child in Store)
               {
                    if (child == item)
                         return i;
                    i++;
               }
               return -1;
          }

          public int CountDescendants (TreeGridItem item)
          {
               var count = item.Children.Count;
               foreach (TreeGridItem subitem in item.Children)
               {
                    if (item.Expanded)
                         count += CountDescendants (subitem);
               }
               return count;
          }


          ITreeGridItem[]? m_original_items;
          public void ApplyFilter (int cloumn_index, string? pattern)
          {
               if (cloumn_index < 0 || Columns.Count <= cloumn_index)
                    return;

               if (m_original_items == null)
               {
                    m_original_items = new ITreeGridItem[Store.Count];
                    Store.CopyTo (m_original_items, 0);
               }

               Store.Clear ();

               if (string.IsNullOrWhiteSpace (pattern))
               {
                    foreach (TreeGridItem item in m_original_items)
                         Store.Add (item);
               }
               else
               {
                    pattern = pattern!.Trim ();

                    foreach (TreeGridItem item in m_original_items)
                    {
                         if (((string)item.Values[cloumn_index]).Contains (pattern))
                              Store.Add (item);
                    }

               }

               ReloadData ();
          }
     }
}
