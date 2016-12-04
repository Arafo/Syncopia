using UnityEngine;
using System.Collections;
using System.ComponentModel;
using System;

public class Enumerations {

    #region Enumerators
    public enum E_SHIPS {
        TESTSHIP,
        FLYER,
        HYPER
    }

    public enum E_DIFFICULTY {
        EASY,
        MEDIUM,
        HARD
    }

    public enum E_GAMEMODE {
        ARCADE,
        TIMETRIAL,
        SEASSON
    }

    public enum E_AA {
        OFF,
        [Description("FXAA")]
        FXAA,
        [Description("FXAA (Extreme)")]
        FXAA2,
        [Description("SMAA (Ultra)")]
        SMAA,
        [Description("MSAA x2")]
        MSAAx2,
        [Description("MSAA x4")]
        MSAAx4,
        [Description("MSAA x8")]
        MSAAx8,
        [Description("TAA (8 samples)")]
        TAAx8,
        [Description("TAA (32 samples)")]
        TAAx32,
        [Description("TAA (Sharpening)")]
        TAASharpening,
        [Description("MSAA x2 + TAA")]
        MSAAx2TAA,
        [Description("MSAA x4 + TAA")]
        MSAAx4TAA,
        [Description("MSAA x8 + TAA")]
        MSAAx8TAA,
        [Description("SSAA x2")]
        SSAAx2,
        [Description("SSAA x4")]
        SSAAx4,
        [Description("SSAA x8")]
        SSAAx8
    }

    #endregion
}

public static class EnumsHelperExtension {
    public static string ToDescription(this Enum value) {
        DescriptionAttribute[] da = (DescriptionAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(DescriptionAttribute), false);
        return da.Length > 0 ? da[0].Description : value.ToString();
    }
}
