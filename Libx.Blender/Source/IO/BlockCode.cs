namespace Libx.Blender.IO;

using System;


[Flags]
public enum BlockCode : Int32
{
    DATA = ('A' << 24) | ('T' << 16) | ('A' << 8) | 'D',
    GLOB = ('B' << 24) | ('O' << 16) | ('L' << 8) | 'G',
    DNA1 = ('1' << 24) | ('A' << 16) | ('N' << 8) | 'D',
    TEST = ('T' << 24) | ('S' << 16) | ('E' << 8) | 'T',
    REND = ('D' << 24) | ('N' << 16) | ('E' << 8) | 'R',
    USER = ('R' << 24) | ('E' << 16) | ('S' << 8) | 'U',
    ENDB = ('B' << 24) | ('D' << 16) | ('N' << 8) | 'E',
    
    // https://developer.blender.org/diffusion/B/browse/master/source/blender/makesdna/DNA_ID_enums.h$56
    /// <summary> Scene </summary>
    SC = ('C' << 8) | 'S',
    /// <summary> Library </summary>
    LI = ('I' << 8) | 'L',
    /// <summary> Object </summary>
    OB = ('B' << 8) | 'O',
    /// <summary> Mesh </summary>
    ME = ('E' << 8) | 'M',
    /// <summary> Curve </summary>
    CU = ('U' << 8) | 'C',
    /// <summary> MetaBall </summary>
    MB = ('B' << 8) | 'M',
    /// <summary> Material </summary>
    MA = ('A' << 8) | 'M',
    /// <summary> Tex (Texture) </summary>
    TE = ('E' << 8) | 'T',
    /// <summary> Image </summary>
    IM = ('M' << 8) | 'I',
    /// <summary> Lattice </summary>
    LT = ('T' << 8) | 'L',
    /// <summary> Light </summary>
    LA = ('A' << 8) | 'L',
    /// <summary> Camera </summary>
    CA = ('A' << 8) | 'C',
    /// <summary> Ipo (depreciated, replaced by FCurves) </summary>
    IP = ('P' << 8) | 'I',
    /// <summary> Key (shape key) </summary>
    KE = ('E' << 8) | 'K',
    /// <summary> World </summary>
    WO = ('O' << 8) | 'W',
    /// <summary> Screen </summary>
    SR = ('R' << 8) | 'S',
    /// <summary> VFont (Vector Font) </summary>
    VF = ('F' << 8) | 'V',
    /// <summary> Text </summary>
    TX = ('X' << 8) | 'T',
    /// <summary> Speaker </summary>
    SK = ('K' << 8) | 'S',
    /// <summary> Sound </summary>
    SO = ('O' << 8) | 'S',
    /// <summary> Group </summary>
    GR = ('R' << 8) | 'G',
    /// <summary> bArmature </summary>
    AR = ('R' << 8) | 'A',
    /// <summary> bAction </summary>
    AC = ('C' << 8) | 'A',
    /// <summary> bNodeTree </summary>
    NT = ('T' << 8) | 'N',
    /// <summary> Brush </summary>
    BR = ('R' << 8) | 'B',
    /// <summary> ParticleSettings </summary>
    PA = ('A' << 8) | 'P',
    /// <summary> bGPdata, (Grease Pencil) </summary>
    GD = ('D' << 8) | 'G',
    /// <summary> WindowManager </summary>
    WM = ('M' << 8) | 'W',
    /// <summary> MovieClip </summary>
    MC = ('C' << 8) | 'M',
    /// <summary> Mask </summary>
    MS = ('S' << 8) | 'M',
    /// <summary> FreestyleLineStyle </summary>
    LS = ('S' << 8) | 'L',
    /// <summary> Palette </summary>
    PL = ('L' << 8) | 'P',
    /// <summary> PaintCurve  </summary>
    PC = ('C' << 8) | 'P',
    /// <summary> CacheFile </summary>
    CF = ('F' << 8) | 'C',
    /// <summary> WorkSpace </summary>
    WS = ('S' << 8) | 'W',
    /// <summary> LightProbe </summary>
    LP = ('P' << 8) | 'L',
    /// <summary> Hair </summary>
    HA = ('A' << 8) | 'H',
    /// <summary> PointCloud </summary>
    PT = ('T' << 8) | 'P',
    /// <summary> Volume </summary>
    VO = ('O' << 8) | 'V',
    /// <summary> Simulation </summary>
    SI = ('I' << 8) | 'S',

    /// <summary> (internal use only) Only used as 'placeholder' in .blend files for directly linked data-blocks. </summary>
    ID = ('D' << 8) | 'I',
    /// <summary> Deprecated </summary>
    SN = ('N' << 8) | 'S',
    /// <summary> NOTE! Fake IDs, needed for g.sipo->blocktype or outliner </summary>
    SQ = ('Q' << 8) | 'S',
    /// <summary> constraint </summary>
    CO = ('O' << 8) | 'C',
    /// <summary> used in outliner... </summary>
    NL = ('L' << 8) | 'N',
    /// <summary> fluidsim Ipo </summary>
    FS = ('S' << 8) | 'F',
}

public static class BlockCodeExtension
{
    public static bool IsCode4 (this BlockCode code) => ((int)code & 0xFFFF0000) != 0;
    public static bool IsCode2 (this BlockCode code) => ((int)code & 0xFFFF0000) == 0;
    public static int ToIndex (this BlockCode code)
    {
        return code switch
        {
            BlockCode.SC => 0,
            BlockCode.LI => 1,
            BlockCode.OB => 2,
            BlockCode.ME => 3,
            BlockCode.CU => 4,
            BlockCode.MB => 5,
            BlockCode.MA => 6,
            BlockCode.TE => 7,
            BlockCode.IM => 8,
            BlockCode.LT => 9,
            BlockCode.LA => 10,
            BlockCode.CA => 11,
            BlockCode.IP => 12,
            BlockCode.KE => 13,
            BlockCode.WO => 14,
            BlockCode.SR => 15,
            BlockCode.VF => 16,
            BlockCode.TX => 17,
            BlockCode.SK => 18,
            BlockCode.SO => 19,
            BlockCode.GR => 20,
            BlockCode.AR => 21,
            BlockCode.AC => 22,
            BlockCode.NT => 23,
            BlockCode.BR => 24,
            BlockCode.PA => 25,
            BlockCode.GD => 26,
            BlockCode.WM => 27,
            BlockCode.MC => 28,
            BlockCode.MS => 29,
            BlockCode.LS => 30,
            BlockCode.PL => 31,
            BlockCode.PC => 32,
            BlockCode.CF => 33,
            BlockCode.WS => 34,
            BlockCode.LP => 35,
            BlockCode.HA => 36,
            BlockCode.PT => 37,
            BlockCode.VO => 38,
            BlockCode.SI => 39,
            BlockCode.ID => 40,
            BlockCode.SN => 41,
            BlockCode.SQ => 42,
            BlockCode.CO => 43,
            BlockCode.NL => 44,
            BlockCode.FS => 45,
            _ => -1,
        };
    }
    public static int IDCount () => 46;

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
}
