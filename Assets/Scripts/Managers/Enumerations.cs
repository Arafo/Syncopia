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

    public enum E_LANGUAGE {
        [Description("English")]
        ENGLISH,
        [Description("Spanish")]
        SPANISH
    }

    public enum E_AA {
        OFF,
        [Description("FXAA")]
        FXAA,
        [Description("FXAA (Extreme)")]
        FXAA2,
        [Description("SMAA (Ultra)")]
        SMAA,
        [Description("MSAAx2")]
        MSAAx2,
        [Description("MSAAx4")]
        MSAAx4,
        [Description("MSAAx8")]
        MSAAx8,
        [Description("TAAx8")]
        TAAx8,
        [Description("TAAx32")]
        TAAx32,
        [Description("TAAS")]
        TAASharpening,
        [Description("MSAAx2 + TAA")]
        MSAAx2TAA,
        [Description("MSAAx4 + TAA")]
        MSAAx4TAA,
        [Description("MSAAx8 + TAA")]
        MSAAx8TAA,
        [Description("SSAAx2")]
        SSAAx2,
        [Description("SSAAx4")]
        SSAAx4,
        [Description("SSAAx8")]
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
