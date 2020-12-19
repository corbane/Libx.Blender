using Eto;
using Eto.Forms;
using Libx.Blender.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libx.Blender.Forms
{
     class ConnectionForm : Form
     {
          readonly TrayIndicator Tray;
          readonly ConnectionControlData Data;

          public ConnectionForm ()
          {
               Data = new ConnectionControlData ();
               Content = new ConnectionControl (Data);

               ShowInTaskbar = false;
               Tray = new TrayIndicator ();
               Tray.Image = IconLib.Empty;
               Tray.Title = "Eto Test App";
               Tray.Activated += (o, e) => MessageBox.Show ("Hello World!!!");
          }

          protected override void OnLoadComplete (EventArgs e)
          {
               base.OnLoadComplete (e);
               Tray.Show ();
          }

          protected override void OnUnLoad (EventArgs e)
          {
               base.OnUnLoad (e);
               Tray.Hide ();
          }
     }
}

#if WINDOWS

namespace Libx.Blender.Forms.Wpf
{

     #nullable disable

     public class Wpf_ConnectionForm
     {
          [STAThread]
          public static void Main (string[ ] args)
          {
               new Application (Platforms.Wpf).Run (new Forms.ConnectionForm ());
          }
     }

}

#endif