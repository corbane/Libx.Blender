
using System;
using System.Collections.Generic;
using System.Text;

namespace Libx.Blender
{
     using Eto = Eto.Drawing;

     public static class ColorLib
     {
          const int _COLOR = 0xE5E5E5;
          const int WO_COLOR = 0xCDE3E4;
          const int SC_COLOR = 0xBACBDE;
          const int GR_COLOR = 0xCDD8E4;
          const int OB_COLOR = 0xC9E8C9;
          const int ME_COLOR = 0xB8E0B8;
          const int CU_COLOR = 0xF5DAA3;
          const int MA_COLOR = 0xF4BEBE;
          const int IM_COLOR = 0xF7D4D4;
          const int BR_COLOR = 0xF2D9F2;

          static Eto.Color ETO_COLOR = Eto.Color.FromRgb (_COLOR);
          static Eto.Color ETO_WO_COLOR = Eto.Color.FromRgb (WO_COLOR);
          static Eto.Color ETO_SC_COLOR = Eto.Color.FromRgb (SC_COLOR);
          static Eto.Color ETO_GR_COLOR = Eto.Color.FromRgb (GR_COLOR);
          static Eto.Color ETO_OB_COLOR = Eto.Color.FromRgb (OB_COLOR);
          static Eto.Color ETO_ME_COLOR = Eto.Color.FromRgb (ME_COLOR);
          static Eto.Color ETO_CU_COLOR = Eto.Color.FromRgb (CU_COLOR);
          static Eto.Color ETO_MA_COLOR = Eto.Color.FromRgb (MA_COLOR);
          static Eto.Color ETO_IM_COLOR = Eto.Color.FromRgb (IM_COLOR);
          static Eto.Color ETO_BR_COLOR = Eto.Color.FromRgb (BR_COLOR);

          public static Eto.Color GetAssociatedColor (IO.Block block)
          {
               return block.Code switch
               {
                    IO.BlockCode.WO => ETO_WO_COLOR,
                    IO.BlockCode.SC => ETO_SC_COLOR,
                    IO.BlockCode.GR => ETO_GR_COLOR,
                    IO.BlockCode.OB => ETO_OB_COLOR,
                    IO.BlockCode.ME => ETO_ME_COLOR,
                    IO.BlockCode.CU => ETO_CU_COLOR,
                    IO.BlockCode.MA => ETO_MA_COLOR,
                    IO.BlockCode.IM => ETO_IM_COLOR,
                    IO.BlockCode.BR => ETO_BR_COLOR,
                    _ => ETO_COLOR
               };
          }
     }
}
