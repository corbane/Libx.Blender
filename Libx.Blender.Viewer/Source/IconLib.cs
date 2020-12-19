
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#nullable enable

namespace Libx.Blender
{
     using Eto = Eto.Drawing;
     using Sys = System.Drawing;

     public static class IconLib
     {
          static Eto.Image? m_blank;

          static Eto.Image? m_scene;
          static Eto.Image? m_world;
          static Eto.Image? m_layer;
          static Eto.Image? m_collection;

          static Eto.Image? m_object;
          static Eto.Image? m_empty;
          static Eto.Image? m_mesh;
          static Eto.Image? m_curve;
          static Eto.Image? m_camera;
          static Eto.Image? m_lamp;

          static Eto.Image? m_modifier_list;

          static Eto.Image? m_material_list;
          static Eto.Image? m_link_material;
          static Eto.Image? m_material;
          static Eto.Image? m_mode_material;
          static Eto.Image? m_texture;
          static Eto.Image? m_image;

          static Eto.Image? m_brush;

          static Eto.Image? m_armature;

          static Eto.Image? m_property_list;
          static Eto.Image? m_property;

          static Eto.Image? m_error;

          public static Eto.Image Blank        => m_blank ??= new Eto.Bitmap (Ressources.Icons.Blank ());
          public static Eto.Image PropertyList => m_property_list ??= new Eto.Bitmap (Ressources.Icons.PropertyList ());
          public static Eto.Image Property     => m_property ??= new Eto.Bitmap (Ressources.Icons.Property ());

          public static Eto.Image Object       => m_object ??= new Eto.Bitmap (Ressources.Icons.Object ());
          public static Eto.Image Empty        => m_empty   ??= new Eto.Bitmap (Ressources.Icons.Empty ());
          public static Eto.Image Mesh         => m_mesh    ??= new Eto.Bitmap (Ressources.Icons.Mesh ());
          public static Eto.Image Curve        => m_curve   ??= new Eto.Bitmap (Ressources.Icons.Curve ());
          public static Eto.Image Camera       => m_camera  ??= new Eto.Bitmap (Ressources.Icons.Camera ());
          public static Eto.Image Light        => m_lamp  ??= new Eto.Bitmap (Ressources.Icons.Light ());

          public static Eto.Image Scene        => m_scene ??= new Eto.Bitmap (Ressources.Icons.Scene ());
          public static Eto.Image ViewLayer    => m_layer ??= new Eto.Bitmap (Ressources.Icons.ViewLayer ());
          public static Eto.Image World        => m_world ??= new Eto.Bitmap (Ressources.Icons.World ());
          public static Eto.Image Collection   => m_collection ??= new Eto.Bitmap (Ressources.Icons.Collection ());

          public static Eto.Image ModifierList => m_modifier_list ??= new Eto.Bitmap (Ressources.Icons.ModifierList ());

          public static Eto.Image Armature     => m_modifier_list ??= new Eto.Bitmap (Ressources.Icons.Armature ());

          public static Eto.Image MaterialList => m_material_list ??= new Eto.Bitmap (Ressources.Icons.MaterialList ());
          public static Eto.Image LinkMaterial => m_link_material ??= new Eto.Bitmap (Ressources.Icons.LinkMaterial ());
          public static Eto.Image Material     => m_material ??= new Eto.Bitmap (Ressources.Icons.Material ());
          public static Eto.Image NodeMaterial => m_mode_material ??= new Eto.Bitmap (Ressources.Icons.NodeMaterial ());
          public static Eto.Image Image        => m_image ??= new Eto.Bitmap (Ressources.Icons.Image ());
          public static Eto.Image Texture      => m_texture ??= new Eto.Bitmap (Ressources.Icons.Texture ());
          public static Eto.Image Brush        => m_brush ??= new Eto.Bitmap (Ressources.Icons.Brush ());
          public static Eto.Image Palette      => m_property ??= new Eto.Bitmap (Ressources.Icons.Palette ());

          public static Eto.Image NodeValue    => m_property ??= new Eto.Bitmap (Ressources.Icons.Property ());

          public static Eto.Image Error         => m_error ??= new Eto.Bitmap (Ressources.Icons.Error ());

          public static Eto.Image GetAssociatedImage (IO.Block? block)
          {
               if (block == null)
                    return Empty;

               return block.Code switch
               {
                    IO.BlockCode.WO => World,
                    IO.BlockCode.SC => Scene,
                    IO.BlockCode.GR => Collection,
                    IO.BlockCode.OB => Blank,
                    IO.BlockCode.CA => Camera,
                    IO.BlockCode.LA => Light,
                    IO.BlockCode.ME => Mesh,
                    IO.BlockCode.CU => Curve,
                    IO.BlockCode.MA => Material,
                    IO.BlockCode.IM => Image,
                    IO.BlockCode.BR => Brush,
                    IO.BlockCode.AR => Armature,
                    _ => Blank
               };
          }
     }
}
