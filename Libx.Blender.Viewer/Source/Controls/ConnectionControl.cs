using Eto.Drawing;
using Eto.Forms;

using Libx.Blender.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Libx.Blender.Controls
{
     public class ConnectionControlData : BlenderState
     {
          // Command Preview


          public IO.BlenderRenderEngine RenderEngine { get; set; } = IO.BlenderRenderEngine.BLENDER_EEVEE;

          public string? RenderPreviewPath { get; private set; }

          public Bitmap? RenderPreview { get; private set; }

          public void ClearRenderPreview ()
          {
               RenderPreview = null;
               RenderPreviewPath = null;
               Emit (nameof (RenderPreview));
          }

          public void AskRenderPreview (string out_name, params string[] object_names)
          {
               Ask (new CommandGetPreview ()
               {
                    FilePath = LoadedPath,
                    OutName = out_name,
                    Includes = object_names,
                    Format = IO.BlenderImageFormat.PNG,
                    Engine = RenderEngine
               });
          }


          // INotifyPropertyChanged


          public ConnectionControlData ()
          {
               PropertyChanged += OnConnectionDataChanged;
          }

          private void OnConnectionDataChanged (object sender, PropertyChangedEventArgs e)
          {
               switch (e.PropertyName)
               {
               case nameof (Command):

                    if (Command is CommandGetPreview)
                    {
                         var json = CommandGetPreview.GetResponse (CommandResponse);
                         if (json.Result == "OK")
                         {
                              RenderPreview = new Bitmap (json.OutPath);
                              RenderPreviewPath = json.OutPath;
                         }
                         else
                         {
                              RenderPreview = null;
                              RenderPreviewPath = null;
                              SetError (json.Error);
                         }
                         Emit (nameof (RenderPreview));
                         return;
                    }
                    break;
               }
          }


          public override event PropertyChangedEventHandler? PropertyChanged;

          protected override void Emit ([CallerMemberName] string memberName = null)
          {
               Application.Instance.Invoke (() =>
                    PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (memberName))
               );
          }
     }

     class ConnectionControl : StackLayout
     {
          public ConnectionControlData Data { get; }

          readonly RichTextArea  LogArea;

          readonly Button ToggleConnection;

          public ConnectionControl () : this (new ConnectionControlData ())
          { }

          public ConnectionControl (ConnectionControlData state)
          {
               Orientation = Orientation.Vertical;

               Data = state;
               Data.PropertyChanged += OnStateChanged;

               //Preview = new ImageView { Width = 256, Height = 256, BackgroundColor = Colors.Blue };
               LogArea = new RichTextArea ();

               ToggleConnection = new Button { Text = "Connect..." };
               ToggleConnection.Click += OnToggleConnection;

               Items.Add (new StackLayoutItem (ToggleConnection, HorizontalAlignment.Stretch));
               Items.Add (new StackLayoutItem (LogArea, HorizontalAlignment.Stretch, expand: true));
          }

          private void OnToggleConnection (object? sender, EventArgs e)
          {
               if (Data.Connection == null)
                    Data.CreateConnection ();
               else
                    Data.AskDisconnect ();
          }

          private void OnStateChanged (object sender, PropertyChangedEventArgs e)
          {
               switch (e.PropertyName)
               {
               case nameof (ConnectionControlData.LoadedPath):
                    break;

               // case nameof (ConnectionControlData.RenderPreview):
               // 
               //      if (Data.RenderPreview == null)
               //      {
               // 
               //      }
               //      else
               //      {
               //           Preview.Image = Data.RenderPreview;
               //           Preview.Invalidate ();
               // 
               //           AddLog ($"Preview updated: {Data.RenderPreviewPath}");
               //      }
               // 
               //      break;

               case nameof (ConnectionControlData.Connection):

                    if (Data.Connection == null)
                    {
                         ToggleConnection.Text = "Connect ...";
                    }
                    else
                    {
                         ToggleConnection.Text = "Disconnect ...";

                         var cnc = Data.Connection;
                         if (cnc.Process != null)
                         {
                              AddLog ("Connected on external process\n"
                                   + $"Server {cnc.EndPoint.Address}:{cnc.EndPoint.Port}\n");
                         }
                         else
                         {
                              AddLog ($"Connected on internal process\n"
                                   + $"Server {cnc.EndPoint.Address}:{cnc.EndPoint.Port}\n");
                         }
                    }
                    break;

               case nameof (ConnectionControlData.ConnectionInfos):
                    if (Data.ConnectionInfos != null)
                    {
                         AddLog ($"Server cache: {Data.ConnectionInfos.CachePath}\n");
                    }
                    break;

               case nameof (ConnectionControlData.Error):

                    AddError (Data.Error!);
                    break;
               }
          }

          public void AddLog (string log)
          {
               LogArea.SelectionForeground = Colors.Black;
               LogArea.Text += "\n" + log;
          }

          public void AddError (string log)
          {
               LogArea.SelectionForeground = Colors.DarkRed;
               LogArea.Text += "\n" + log;
          }
     }

}
