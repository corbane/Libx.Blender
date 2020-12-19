using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libx.Blender.Ressources
{
     public static class Icons
     {
          static System.Reflection.Assembly Assembly = typeof (Icons).Assembly;
          static Stream Get (string ressource) => Assembly.GetManifestResourceStream (ressource);

          public static Stream Blank ()        => Get ("Libx.Blender.Icons.Blank.png");
          public static Stream Armature ()     => Get ("Libx.Blender.Icons.Armature.png");
          public static Stream Blender ()      => Get ("Libx.Blender.Icons.Blender.png");
          public static Stream Brush ()        => Get ("Libx.Blender.Icons.Brush.png");
          public static Stream Camera ()       => Get ("Libx.Blender.Icons.Camera.png");
          public static Stream Collection ()   => Get ("Libx.Blender.Icons.Collection.png");
          public static Stream Connection ()   => Get ("Libx.Blender.Icons.Connection.png");
          public static Stream Curve ()        => Get ("Libx.Blender.Icons.Curve.png");
          public static Stream Empty ()        => Get ("Libx.Blender.Icons.Empty.png");
          public static Stream Error ()        => Get ("Libx.Blender.Icons.Error.png");
          public static Stream Image ()        => Get ("Libx.Blender.Icons.Image.png");
          public static Stream Lattice ()      => Get ("Libx.Blender.Icons.Lattice.png");
          public static Stream Light ()        => Get ("Libx.Blender.Icons.Light.png");
          public static Stream LightProbe ()   => Get ("Libx.Blender.Icons.LightProbe.png");
          public static Stream LinkMaterial () => Get ("Libx.Blender.Icons.LinkMaterial.png");
          public static Stream Material ()     => Get ("Libx.Blender.Icons.Material.png");
          public static Stream MaterialList () => Get ("Libx.Blender.Icons.MaterialList.png");
          public static Stream Mesh ()         => Get ("Libx.Blender.Icons.Mesh.png");
          public static Stream MetaBall ()     => Get ("Libx.Blender.Icons.MetaBall.png");
          public static Stream ModifierList () => Get ("Libx.Blender.Icons.ModifierList.png");
          public static Stream NodeMaterial () => Get ("Libx.Blender.Icons.NodeMaterial.png");
          public static Stream Object ()       => Get ("Libx.Blender.Icons.Object.png");
          public static Stream Property ()     => Get ("Libx.Blender.Icons.Property.png");
          public static Stream PropertyList () => Get ("Libx.Blender.Icons.PropertyList.png");
          public static Stream Palette ()      => Get ("Libx.Blender.Icons.Palette.png");
          public static Stream Scene ()        => Get ("Libx.Blender.Icons.Scene.png");
          public static Stream Speaker ()      => Get ("Libx.Blender.Icons.Speaker.png");
          public static Stream Surface ()      => Get ("Libx.Blender.Icons.Surface.png");
          public static Stream Text ()         => Get ("Libx.Blender.Icons.Text.png");
          public static Stream TextBlock ()    => Get ("Libx.Blender.Icons.TextBlock.png");
          public static Stream Texture ()      => Get ("Libx.Blender.Icons.Texture.png");
          public static Stream ViewLayer ()    => Get ("Libx.Blender.Icons.ViewLayer.png");
          public static Stream World ()        => Get ("Libx.Blender.Icons.World.png");
     }
}
