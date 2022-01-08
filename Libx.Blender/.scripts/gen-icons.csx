
/*/
    To generate the `Icons.cs` resource file, run this command in the` Libx.Blender` folder:
    > dotnet-script .scripts/gen-icons.csx -f Icons/*.png > Source/Ressources/Icons.cs
/*/

#r "lib/CommandLineParser/CommandLine.dll"

using System;
using System.Collections.Generic;
using System.IO;

using CommandLine;

public class Options
{
    [Option('f', "files", Required = true, HelpText = "Input files to be processed.")]
    public IEnumerable <string> Inputs { get; set; }
}

const string PREFIX = "Libx.Blender.Icons.";
const string BASE_PATH = "Icons/";

Parser.Default.ParseArguments<Options>(Args).WithParsed<Options>(o =>
{
    var inputs = o.Inputs.ToList ();
    inputs.Sort ();

    var max = 0;
    var names = new Dictionary <string, string> (inputs.Count);
    foreach (var path in o.Inputs)
    {
        if (path.Contains ('*')) throw new ArgumentException (
            "You must run this script in a Linux type terminal.\n" +
            "You can use Git Bash: https://git-scm.com/downloads"
        );

        var n = Path.GetFileNameWithoutExtension (path);

        if (max < n.Length)
            max = n.Length;

        names.Add (n, Path.GetFileName (path));
    }


    #region Icons.cs

    Console.WriteLine ("namespace Libx.Blender.Ressources;");
    Console.WriteLine ();
    Console.WriteLine ("using System.IO;");
    Console.WriteLine ();
    Console.WriteLine ("public static class Icons");
    Console.WriteLine ("{");
    Console.WriteLine ("    static System.Reflection.Assembly Assembly = typeof (Icons).Assembly;");
    Console.WriteLine ("    static Stream Get (string ressource) => Assembly.GetManifestResourceStream (ressource)!;");
    Console.WriteLine ("    ");
    foreach (var item in names) Console.WriteLine (
        "    public static Stream " + item.Key.PadRight (max) + " () => Get (\"" + PREFIX + item.Value + "\");"
    );
    Console.WriteLine ("}");

    #endregion

    #region Libx.Blender.csprj

    Console.WriteLine ();
    Console.WriteLine ("/*/ Add this ItemGroup to the .csprj project file.");
    Console.WriteLine ("<ItemGroup>");
    foreach (var item in names) Console.WriteLine (
        "    <EmbeddedResource Include=\"" + BASE_PATH + item.Value + "\" />"
    );
    Console.WriteLine ("</ItemGroup>");
    Console.WriteLine ("/*/");

    #endregion

    #region BlockCodeExtension

    Console.WriteLine ();
    Console.WriteLine ("/*/ Add this method to the BlockCodeExtension class.");
    Console.WriteLine ("#region Auto generated (see .scripts/gen-icons.csx)");
    Console.WriteLine ();
    foreach (var item in names) Console.WriteLine (
        "static Eto.Drawing.Bitmap? m_" + item.Key + ";"
    );
    Console.WriteLine ("static Eto.Drawing.Bitmap? m_none;");
    Console.WriteLine ();
    Console.WriteLine ("public static Eto.Drawing.Bitmap ToEtoBitmap (this BlockCode code)");
    Console.WriteLine ("{");
    Console.WriteLine ("    return code switch");
    Console.WriteLine ("    {");
    foreach (var item in names) Console.WriteLine (
        "        BlockCode." + item.Key.PadRight (max) + " => m_" + item.Key + " ??= new Eto.Drawing.Bitmap (Ressources.Icons." + item.Key + " ()),"
    );
    Console.WriteLine ("        _ => m_none ??= new Eto.Drawing.Bitmap (20, 20, Eto.Drawing.PixelFormat.Format32bppRgba)");
    Console.WriteLine ("    };");
    Console.WriteLine ("}");
    Console.WriteLine ();
    Console.WriteLine ("#endregion");

    Console.WriteLine ("/*/");
    
    #endregion

});


