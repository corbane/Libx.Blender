using Eto.Forms;

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable

namespace Libx.Blender.Controls
{
     public enum OutlinerFilter
     {
          Scenes,
          Collections,
          Materials,
          Objects,
          Libraries,
          Types,
     }


     public class BlockOutlinerData : INotifyPropertyChanged
     {
          public IO.BlendData? BlendCS { get; private set; }
          public IO.File? Blend { get; private set; }
          public void SetBlendFile (IO.File blend)
          {
               if (Blend == blend)
                    return;

               Blend = blend;
               BlendCS = blend == null ? null : new IO.BlendData (blend);
               Emit (nameof (Blend));
          }

          public OutlinerFilter Filter { get; private set; } = OutlinerFilter.Scenes;
          public void SetFilter (OutlinerFilter filter)
          {
               if (Filter == filter)
                    return;

               Filter = filter;
               Emit (nameof (Filter));
          }

          public bool ShowOnlyParent { get; private set; } = true;
          public void SetShowOnlyParent (bool only_parent)
          {
               if (ShowOnlyParent == only_parent)
                    return;

               ShowOnlyParent = only_parent;
               Emit (nameof (ShowOnlyParent));
          }

          public Sdna.IFileBlockObject? ActiveSdnaItem { get; private set; }
          public void SetActiveSdnaItem (Sdna.IFileBlockObject wrap)
          {
               if (ActiveSdnaItem == wrap)
                    return;

               ActiveSdnaItem = wrap;
               Emit (nameof (ActiveSdnaItem));
          }


          public event PropertyChangedEventHandler? PropertyChanged;

          void Emit ([CallerMemberName] string memberName = null)
          {
               Application.Instance.Invoke (() =>
                    PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (memberName))
               );
          }
     }


     public class BlockOutliner : StackLayout
     {
          public readonly BlockOutlinerData Data;

          readonly TreeControl Tree;
          readonly DropDown ViewSelector;

          public BlockOutliner (): this (new BlockOutlinerData ())
          { }

          public BlockOutliner (BlockOutlinerData data)
          {
               Data = data;
               Data.PropertyChanged += _OnDataChanged;

               Orientation = Orientation.Vertical;

               Tree = new TreeControl ();
               Tree.AddImageTextColumn ("");
               Tree.CellDoubleClick += _OnDoubleClick;
               Tree.SelectedRowsChanged += _OnSelectedRowsChanged;
               Tree.ShowHeader = false;
               Tree.RowHeight = 30;

               ViewSelector = new DropDown
               {
                    SelectedIndex = 0,
                    Items =
                    {
                         new ListItem { Text = nameof (OutlinerFilter.Scenes), Tag = OutlinerFilter.Scenes },
                         new ListItem { Text = nameof (OutlinerFilter.Collections), Tag = OutlinerFilter.Collections },
                         new ListItem { Text = nameof (OutlinerFilter.Materials), Tag = OutlinerFilter.Materials },
                         new ListItem { Text = nameof (OutlinerFilter.Objects), Tag = OutlinerFilter.Objects },
                         new ListItem { Text = nameof (OutlinerFilter.Libraries), Tag = OutlinerFilter.Libraries },
                    }
               };
               ViewSelector.SelectedKeyChanged += _OnFilterChanged;

               Items.Add (new StackLayoutItem (ViewSelector, HorizontalAlignment.Stretch));
               Items.Add (new StackLayoutItem (Tree, HorizontalAlignment.Stretch, expand: true));

          }


          void _OnFilterChanged (object? sender, EventArgs e)
          {
               if (sender == null)
                    return;

               var item = (ListItem) ((DropDown)sender).SelectedValue;
               if (item == null)
                    return;

               Data.SetFilter ((OutlinerFilter) item.Tag);
          }

          void _OnSelectedRowsChanged (object? sender, EventArgs e)
          {
               
               var item = Tree.SelectedItem as TreeGridItem;
               if (item == null)
                    return;

               if (item.Tag is Sdna.IFileBlockObject wrap)
                    Data.SetActiveSdnaItem (wrap);
          }


          void _OnDataChanged (object? sender, PropertyChangedEventArgs e)
          {
               switch (e.PropertyName)
               {
               case nameof (BlockOutlinerData.Blend):

                    if (Data.Blend == null)
                    {
                         Tree.Store.Clear ();
                         Tree.ReloadData ();
                    }
                    else
                    {
                         _UpdateTree ();
                    }
                    break;

               case nameof (BlockOutlinerData.Filter):

                    _UpdateTree ();
                    break;

               // case nameof (OutlinerData.ActiveSdnaItem):
               // 
               //      if (Data.ActiveSdnaItem != null)
               //           OnItemSelected?.Invoke (Data.ActiveSdnaItem);
               //      break;
               }
          }

          void _UpdateTree ()
          {
               Tree.Store.Clear ();
               
               if (Data.Filter == OutlinerFilter.Scenes)
                    _AppendScenes (Tree.Store);

               else if (Data.Filter == OutlinerFilter.Collections)
                    _AppendCollections (Tree.Store);

               else if (Data.Filter == OutlinerFilter.Materials)
                    _AppendMaterials (Tree.Store);

               else if (Data.Filter == OutlinerFilter.Objects)
                    _AppendObjects (Tree.Store);

               else if (Data.Filter == OutlinerFilter.Libraries)
                    _AppendLibraries (Tree.Store);

               Tree.ReloadData ();
          }

          void _OnDoubleClick (object? sender, GridCellMouseEventArgs e)
          {
               var item = e.Item as TreeGridItem;
               if (item == null || e.Row < 0) return;

               item.Expanded = !item.Expanded;
               if (item.Expanded == false || item.Children.Count > 0)
               {
                    Tree.ReloadData ();
                    return;
               }

               if (item.Tag is Sdna.Collection collection)
                    _ToggleCollection (item.Children, collection);

               else if (item.Tag is Sdna.Scene scene)
                    _ToggleScene (item.Children, scene);

               else if (item.Tag is Sdna.Object obj)
                    _ToggleObject (item.Children, obj);

               else if (item.Tag is Sdna.Material mat)
                    _ToggleMaterial (item.Children, mat);

               else if (item.Tag is Sdna.Node node)
                    _ToggleNode (item.Children, node);

               Tree.ReloadData ();
          }


          void _AppendScenes (IList<ITreeGridItem> target)
          {
               if (Data.Blend == null)
                    return;

               foreach (var sc in Data.BlendCS!.Scenes)
               {
                    target.Add (
                         new TreeGridItem (IconLib.Scene, sc.Id.Name ())
                         { Tag = sc }
                    );
               }
          }

          void _AppendCollections (IList<ITreeGridItem> target)
          {
               if (Data.Blend == null)
                    return;

               foreach (var gr in Data.BlendCS!.Collections)
               {
                    target.Add (
                         new TreeGridItem (IconLib.Collection, gr.Id.Name ())
                         { Tag = gr }
                    );
               }
          }

          void _AppendMaterials (IList<ITreeGridItem> target)
          {
               if (Data.Blend == null)
                    return;

               foreach (var ma in Data.BlendCS!.Materials)
               {
                    target.Add (
                         new TreeGridItem (IconLib.Material, ma.Id.Name ())
                         { Tag = ma }
                    );
               }
          }

          void _AppendObjects (IList <ITreeGridItem> target)
          {
               if (Data.Blend == null)
                    return;

               if (Data.ShowOnlyParent)
               {
                    foreach (var ob in Data.BlendCS!.Objects)
                    {
                         if (ob.Parent () == null)
                              target.Add (_CreateObjectItem (ob));
                    }
               }
               else
               {
                    foreach (var ob in Data.BlendCS!.Objects)
                         target.Add (_CreateObjectItem (ob));
               }
          }

          void _AppendLibraries (IList<ITreeGridItem> target)
          {
               if (Data.Blend == null)
                    return;

               foreach (var li in Data.BlendCS!.Libraries)
               {
                    target.Add (
                         new TreeGridItem (IconLib.Blank, li.Path ())
                         { Tag = li }
                    );
               }
          }


          void _ToggleScene (IList<ITreeGridItem> target, Sdna.Scene scene)
          {
               var master_collection = scene.GetMasterCollection (null);
               if (master_collection != null)
               {
                    target.Add (
                         new TreeGridItem (IconLib.Collection, master_collection.Id.Name ())
                         { Tag = master_collection }
                    );
               }
          }

          void _ToggleCollection (IList<ITreeGridItem> target, Sdna.Collection collection)
          {
               foreach (var child in collection.Children ())
               {
                    var gr = child.Collection;
                    target.Add (
                         new TreeGridItem (IconLib.Collection, gr.Id.Name ())
                         { Tag = gr }
                    );
               }

               foreach (var child in collection.Objects ())
               {
                    target.Add (_CreateObjectItem (child.Object));
               }
          }

          void _ToggleObject (IList<ITreeGridItem> target, Sdna.Object ob)
          {
               var data = ob.Data ();
               if (data == null) return;

               // var properties = new TreeGridItem (IconLib.PropertyList, "Properties");
               // item.Children.Add (properties);

               var modifiers  = new TreeGridItem (IconLib.ModifierList, "Modifiers");
               var materials  = new TreeGridItem (IconLib.MaterialList, "Materials");

               foreach (var modifier in ob.Modifiers ())
               {
                    modifiers.Children.Add (
                         new TreeGridItem (IconLib.ModifierList, modifier.ModifierData.Name)
                         { Tag = modifier }
                    );
               }


               foreach (var mat in ob.Materials ())
               {
                    materials.Children.Add (
                         new TreeGridItem (IconLib.Material, mat.Id.Name ())
                         { Tag = mat }
                    );
               }

               if (modifiers.Children.Count > 0) target.Add (modifiers);
               if (materials.Children.Count > 0) target.Add (materials);

               switch (ob.Type)
               {
               case Sdna.Object.Types.Armature: break;
               case Sdna.Object.Types.Empty   : break;
               case Sdna.Object.Types.Mesh    : _ToggleMesh (target, new Sdna.Mesh (data)); break;
               case Sdna.Object.Types.Curve   : break;
               case Sdna.Object.Types.Surface : break;
               case Sdna.Object.Types.Font    : break;
               case Sdna.Object.Types.MetaBall: break;
               case Sdna.Object.Types.Lamp    : break;
               case Sdna.Object.Types.Camera  : break;
               case Sdna.Object.Types.Speaker : break;
               case Sdna.Object.Types.Lattice : break;
               }
          }

          void _ToggleMesh (IList<ITreeGridItem> target, Sdna.Mesh obj)
          {
               var materials = new TreeGridItem (IconLib.MaterialList, "Materials");

               try
               {
                    foreach (var mat in obj.Materials ())
                    {
                         if (mat.IsLinked)
                         {
                              materials.Children.Add (
                                   new TreeGridItem (IconLib.LinkMaterial, mat.Id.Name ())
                                   { Tag = mat.Id }
                              );
                         }
                         else
                         {
                              materials.Children.Add (
                                   new TreeGridItem (IconLib.Material, mat.Id.Name ())
                                   { Tag = mat }
                              );
                         }
                    }
               }
               catch (Exception error)
               {
                    target.Add (
                         new TreeGridItem (IconLib.Error, error.Message)
                         { Tag = error }
                    );
               }

               if (materials.Children.Count > 0)
                    target.Add (materials);
          }

          // void _ToggleCurve (TreeGridItem item, SdnaCurve obj)
          // {
          //      var materials = new TreeGridItem (IconLib.MaterialList, "Materials");
          //
          //      foreach (var mat in obj.Materials ())
          //      {
          //           materials.Children.Add (
          //                new TreeGridItem (IconLib.Material, mat.ID ().Name ())
          //                { Tag = mat }
          //           );
          //      }
          //
          //      if (materials.Children.Count > 0)
          //           item.Children.Add (materials);
          // }

          void _ToggleMaterial (IList<ITreeGridItem> target, Sdna.Material mat)
          {
               var tree = mat.NodeTree ();

               if (tree == null)
                    return;

               foreach (var node in tree.Nodes ())
               {
                    target.Add (
                         new TreeGridItem (IconLib.NodeMaterial, node.IdName (def: null) ?? node.Name ())
                         { Tag = node }
                    );
               }
          }

          void _ToggleNode (IList<ITreeGridItem> target, Sdna.Node node)
          {
               foreach (var socket in node.Inputs ())
               {
                    target.Add (
                         new TreeGridItem (IconLib.NodeValue, socket.Type.ToString () + " " + socket.Identifier)
                         { Tag = socket }
                    );
               }
          }

          TreeGridItem _CreateObjectItem (Sdna.Object ob)
          {
               return new TreeGridItem (IconLib.GetAssociatedImage (ob.Data ()), ob.ID ().Name ())
               { Tag = ob };
          }
     }
}
