
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace Libx.Blender.IO
{
     public enum BlenderRenderEngine { CYCLES, BLENDER_EEVEE }

     public enum BlenderImageFormat
     {
          BMP, IRIS, PNG, JPEG, JPEG2000, TARGA, TARGA_RAW, CINEON, DPX,
          OPEN_EXR_MULTILAYER, OPEN_EXR, HDR, TIFF, AVI_JPEG, AVI_RAW
     }

}
