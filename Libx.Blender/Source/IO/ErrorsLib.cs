using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libx.Blender.IO
{
     public class ReadError : Exception
     {
          public ReadError (string message) : base (message)
          {

          }
     }
}
