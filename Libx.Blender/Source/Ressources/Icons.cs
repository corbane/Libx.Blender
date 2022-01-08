namespace Libx.Blender.Ressources;

using System.IO;

public static class Icons
{
    static System.Reflection.Assembly Assembly = typeof (Icons).Assembly;
    static Stream Get (string ressource) => Assembly.GetManifestResourceStream (ressource)!;
    
    public static Stream AC () => Get ("Libx.Blender.Icons.AC.png");
    public static Stream AR () => Get ("Libx.Blender.Icons.AR.png");
    public static Stream BR () => Get ("Libx.Blender.Icons.BR.png");
    public static Stream CA () => Get ("Libx.Blender.Icons.CA.png");
    public static Stream CF () => Get ("Libx.Blender.Icons.CF.png");
    public static Stream CU () => Get ("Libx.Blender.Icons.CU.png");
    public static Stream GD () => Get ("Libx.Blender.Icons.GD.png");
    public static Stream GR () => Get ("Libx.Blender.Icons.GR.png");
    public static Stream HA () => Get ("Libx.Blender.Icons.HA.png");
    public static Stream IM () => Get ("Libx.Blender.Icons.IM.png");
    public static Stream KE () => Get ("Libx.Blender.Icons.KE.png");
    public static Stream LA () => Get ("Libx.Blender.Icons.LA.png");
    public static Stream LI () => Get ("Libx.Blender.Icons.LI.png");
    public static Stream LP () => Get ("Libx.Blender.Icons.LP.png");
    public static Stream LS () => Get ("Libx.Blender.Icons.LS.png");
    public static Stream LT () => Get ("Libx.Blender.Icons.LT.png");
    public static Stream MA () => Get ("Libx.Blender.Icons.MA.png");
    public static Stream MB () => Get ("Libx.Blender.Icons.MB.png");
    public static Stream MC () => Get ("Libx.Blender.Icons.MC.png");
    public static Stream ME () => Get ("Libx.Blender.Icons.ME.png");
    public static Stream MS () => Get ("Libx.Blender.Icons.MS.png");
    public static Stream NT () => Get ("Libx.Blender.Icons.NT.png");
    public static Stream OB () => Get ("Libx.Blender.Icons.OB.png");
    public static Stream PA () => Get ("Libx.Blender.Icons.PA.png");
    public static Stream PL () => Get ("Libx.Blender.Icons.PL.png");
    public static Stream PT () => Get ("Libx.Blender.Icons.PT.png");
    public static Stream SC () => Get ("Libx.Blender.Icons.SC.png");
    public static Stream SI () => Get ("Libx.Blender.Icons.SI.png");
    public static Stream SK () => Get ("Libx.Blender.Icons.SK.png");
    public static Stream SO () => Get ("Libx.Blender.Icons.SO.png");
    public static Stream TE () => Get ("Libx.Blender.Icons.TE.png");
    public static Stream TX () => Get ("Libx.Blender.Icons.TX.png");
    public static Stream VF () => Get ("Libx.Blender.Icons.VF.png");
    public static Stream VO () => Get ("Libx.Blender.Icons.VO.png");
    public static Stream WO () => Get ("Libx.Blender.Icons.WO.png");
    public static Stream WS () => Get ("Libx.Blender.Icons.WS.png");
}

/*/ Add this ItemGroup to the .csprj project file.
<ItemGroup>
    <EmbeddedResource Include="Icons/AC.png" />
    <EmbeddedResource Include="Icons/AR.png" />
    <EmbeddedResource Include="Icons/BR.png" />
    <EmbeddedResource Include="Icons/CA.png" />
    <EmbeddedResource Include="Icons/CF.png" />
    <EmbeddedResource Include="Icons/CU.png" />
    <EmbeddedResource Include="Icons/GD.png" />
    <EmbeddedResource Include="Icons/GR.png" />
    <EmbeddedResource Include="Icons/HA.png" />
    <EmbeddedResource Include="Icons/IM.png" />
    <EmbeddedResource Include="Icons/KE.png" />
    <EmbeddedResource Include="Icons/LA.png" />
    <EmbeddedResource Include="Icons/LI.png" />
    <EmbeddedResource Include="Icons/LP.png" />
    <EmbeddedResource Include="Icons/LS.png" />
    <EmbeddedResource Include="Icons/LT.png" />
    <EmbeddedResource Include="Icons/MA.png" />
    <EmbeddedResource Include="Icons/MB.png" />
    <EmbeddedResource Include="Icons/MC.png" />
    <EmbeddedResource Include="Icons/ME.png" />
    <EmbeddedResource Include="Icons/MS.png" />
    <EmbeddedResource Include="Icons/NT.png" />
    <EmbeddedResource Include="Icons/OB.png" />
    <EmbeddedResource Include="Icons/PA.png" />
    <EmbeddedResource Include="Icons/PL.png" />
    <EmbeddedResource Include="Icons/PT.png" />
    <EmbeddedResource Include="Icons/SC.png" />
    <EmbeddedResource Include="Icons/SI.png" />
    <EmbeddedResource Include="Icons/SK.png" />
    <EmbeddedResource Include="Icons/SO.png" />
    <EmbeddedResource Include="Icons/TE.png" />
    <EmbeddedResource Include="Icons/TX.png" />
    <EmbeddedResource Include="Icons/VF.png" />
    <EmbeddedResource Include="Icons/VO.png" />
    <EmbeddedResource Include="Icons/WO.png" />
    <EmbeddedResource Include="Icons/WS.png" />
</ItemGroup>
/*/

/*/ Add this method to the BlockCodeExtension class.
#region Auto generated (see .scripts/gen-icons.csx)

static Eto.Drawing.Bitmap? m_AC;
static Eto.Drawing.Bitmap? m_AR;
static Eto.Drawing.Bitmap? m_BR;
static Eto.Drawing.Bitmap? m_CA;
static Eto.Drawing.Bitmap? m_CF;
static Eto.Drawing.Bitmap? m_CU;
static Eto.Drawing.Bitmap? m_GD;
static Eto.Drawing.Bitmap? m_GR;
static Eto.Drawing.Bitmap? m_HA;
static Eto.Drawing.Bitmap? m_IM;
static Eto.Drawing.Bitmap? m_KE;
static Eto.Drawing.Bitmap? m_LA;
static Eto.Drawing.Bitmap? m_LI;
static Eto.Drawing.Bitmap? m_LP;
static Eto.Drawing.Bitmap? m_LS;
static Eto.Drawing.Bitmap? m_LT;
static Eto.Drawing.Bitmap? m_MA;
static Eto.Drawing.Bitmap? m_MB;
static Eto.Drawing.Bitmap? m_MC;
static Eto.Drawing.Bitmap? m_ME;
static Eto.Drawing.Bitmap? m_MS;
static Eto.Drawing.Bitmap? m_NT;
static Eto.Drawing.Bitmap? m_OB;
static Eto.Drawing.Bitmap? m_PA;
static Eto.Drawing.Bitmap? m_PL;
static Eto.Drawing.Bitmap? m_PT;
static Eto.Drawing.Bitmap? m_SC;
static Eto.Drawing.Bitmap? m_SI;
static Eto.Drawing.Bitmap? m_SK;
static Eto.Drawing.Bitmap? m_SO;
static Eto.Drawing.Bitmap? m_TE;
static Eto.Drawing.Bitmap? m_TX;
static Eto.Drawing.Bitmap? m_VF;
static Eto.Drawing.Bitmap? m_VO;
static Eto.Drawing.Bitmap? m_WO;
static Eto.Drawing.Bitmap? m_WS;
static Eto.Drawing.Bitmap? m_none;

public static Eto.Drawing.Bitmap ToEtoBitmap (this BlockCode code)
{
    return code switch
    {
        BlockCode.AC => m_AC ??= new Eto.Drawing.Bitmap (Ressources.Icons.AC ()),
        BlockCode.AR => m_AR ??= new Eto.Drawing.Bitmap (Ressources.Icons.AR ()),
        BlockCode.BR => m_BR ??= new Eto.Drawing.Bitmap (Ressources.Icons.BR ()),
        BlockCode.CA => m_CA ??= new Eto.Drawing.Bitmap (Ressources.Icons.CA ()),
        BlockCode.CF => m_CF ??= new Eto.Drawing.Bitmap (Ressources.Icons.CF ()),
        BlockCode.CU => m_CU ??= new Eto.Drawing.Bitmap (Ressources.Icons.CU ()),
        BlockCode.GD => m_GD ??= new Eto.Drawing.Bitmap (Ressources.Icons.GD ()),
        BlockCode.GR => m_GR ??= new Eto.Drawing.Bitmap (Ressources.Icons.GR ()),
        BlockCode.HA => m_HA ??= new Eto.Drawing.Bitmap (Ressources.Icons.HA ()),
        BlockCode.IM => m_IM ??= new Eto.Drawing.Bitmap (Ressources.Icons.IM ()),
        BlockCode.KE => m_KE ??= new Eto.Drawing.Bitmap (Ressources.Icons.KE ()),
        BlockCode.LA => m_LA ??= new Eto.Drawing.Bitmap (Ressources.Icons.LA ()),
        BlockCode.LI => m_LI ??= new Eto.Drawing.Bitmap (Ressources.Icons.LI ()),
        BlockCode.LP => m_LP ??= new Eto.Drawing.Bitmap (Ressources.Icons.LP ()),
        BlockCode.LS => m_LS ??= new Eto.Drawing.Bitmap (Ressources.Icons.LS ()),
        BlockCode.LT => m_LT ??= new Eto.Drawing.Bitmap (Ressources.Icons.LT ()),
        BlockCode.MA => m_MA ??= new Eto.Drawing.Bitmap (Ressources.Icons.MA ()),
        BlockCode.MB => m_MB ??= new Eto.Drawing.Bitmap (Ressources.Icons.MB ()),
        BlockCode.MC => m_MC ??= new Eto.Drawing.Bitmap (Ressources.Icons.MC ()),
        BlockCode.ME => m_ME ??= new Eto.Drawing.Bitmap (Ressources.Icons.ME ()),
        BlockCode.MS => m_MS ??= new Eto.Drawing.Bitmap (Ressources.Icons.MS ()),
        BlockCode.NT => m_NT ??= new Eto.Drawing.Bitmap (Ressources.Icons.NT ()),
        BlockCode.OB => m_OB ??= new Eto.Drawing.Bitmap (Ressources.Icons.OB ()),
        BlockCode.PA => m_PA ??= new Eto.Drawing.Bitmap (Ressources.Icons.PA ()),
        BlockCode.PL => m_PL ??= new Eto.Drawing.Bitmap (Ressources.Icons.PL ()),
        BlockCode.PT => m_PT ??= new Eto.Drawing.Bitmap (Ressources.Icons.PT ()),
        BlockCode.SC => m_SC ??= new Eto.Drawing.Bitmap (Ressources.Icons.SC ()),
        BlockCode.SI => m_SI ??= new Eto.Drawing.Bitmap (Ressources.Icons.SI ()),
        BlockCode.SK => m_SK ??= new Eto.Drawing.Bitmap (Ressources.Icons.SK ()),
        BlockCode.SO => m_SO ??= new Eto.Drawing.Bitmap (Ressources.Icons.SO ()),
        BlockCode.TE => m_TE ??= new Eto.Drawing.Bitmap (Ressources.Icons.TE ()),
        BlockCode.TX => m_TX ??= new Eto.Drawing.Bitmap (Ressources.Icons.TX ()),
        BlockCode.VF => m_VF ??= new Eto.Drawing.Bitmap (Ressources.Icons.VF ()),
        BlockCode.VO => m_VO ??= new Eto.Drawing.Bitmap (Ressources.Icons.VO ()),
        BlockCode.WO => m_WO ??= new Eto.Drawing.Bitmap (Ressources.Icons.WO ()),
        BlockCode.WS => m_WS ??= new Eto.Drawing.Bitmap (Ressources.Icons.WS ()),
        _ => m_none ??= new Eto.Drawing.Bitmap (20, 20, Eto.Drawing.PixelFormat.Format32bppRgba)
    };
}

#endregion
/*/
