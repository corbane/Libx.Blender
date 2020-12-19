using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

#nullable enable

namespace Libx.Blender.Controls
{
     public class PreviewControl : ImageView
     {
          // public void Fill (IO.File blend)
          // {
          //      var blocks = blend.GetByCode (IO.BlockCode.TEST);
          //      //var blocks = blend.GetByCode ("TEST");
          //      if (blocks.Length != 1) return;
          // 
          //      var TEST = blocks[0];
          //      if (TEST.Body == null || TEST.BodyLength == 0) return;
          // 
          //      var width = TEST.i32 (0);
          //      var height = TEST.i32 (4);
          //      var length = width * height * 4;
          //      if (length != TEST.BodyLength - 8)
          //           return;
          //      
          //      var buf = TEST.u8_array (8, TEST.BodyLength - 8);
          //      //if (blend.Endian == IO.Endian.Le) Array.Reverse (buf);
          //      
          //      var bitmap = new Bitmap (width, height, PixelFormat.Format32bppRgba);
          //      using (var pixels = bitmap.Lock ())
          //      {
          //           Marshal.Copy (buf, 0, pixels.Data, buf.Length);
          //      }
          //      Image = bitmap;
          //      Invalidate ();
          // }
     }


}
