using System.Collections.Generic;

namespace BNDL_Explorer.Classes
{
    class Constants
    {
        public enum ImageListTypes
        {
            FOLDER = 0,
            IMAGE = 1,
            DOCUMENT,
        };

        public enum SupportedExtensions
        {
            BNDL = 0,
            LVBNDL = 1,
            TEX,
        };

        public static Dictionary<SupportedExtensions, string> extensions = new Dictionary<SupportedExtensions, string>
        { 
            { SupportedExtensions.BNDL, ".bndl" }, 
            { SupportedExtensions.LVBNDL, ".lvbndl" },
            { SupportedExtensions.TEX, ".tex" }
        };
        public static float appVersion = 0.04f;
        public static string openDialogBNDLFilter = "FEAR 2 bndl|*.bndl|FEAR 2 lvbndl|*.lvbndl";
        public static string anyDialogDDSFilter = "DDS Texture|*.dds";
        public static string anyDialogAnyFilter = "Any File|*.*";
        public static int streamBufferLength = 81920;
    }
}
