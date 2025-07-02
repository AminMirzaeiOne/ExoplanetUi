
using Sunny.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Drawing.FontConverter;

namespace Exoplanet.UI
{
    /// <summary>
    /// Theme Style Management Class
    /// </summary>
    public static class UIStyles
    {
        public static bool GlobalFont { get; set; } = false;

        public static bool GlobalRectangle { get; set; } = false;

        /// <summary>
        /// Multi-language support
        /// </summary>
        public static bool MultiLanguageSupport { get; set; } = false;

        public static bool DPIScale { get; set; }

        public static bool ZoomScale { get; set; }

        [Editor("System.Drawing.Design.FontNameEditor", "System.Drawing.Design.UITypeEditor")]
        [TypeConverter(typeof(FontNameConverter))]
        public static string GlobalFontName { get; set; } = "SimSun";

        public static int GlobalFontScale { get; set; } = 100;

        private static readonly ConcurrentDictionary<string, byte> FontCharSets = new();

        // GdiCharSet
        // A byte value specifying the GDI charset used by this font. Default is 1.
        // Charset           Value
        // ANSI              0
        // DEFAULT           1
        // SYMBOL            2
        // SHIFTJIS          128
        // HANGEUL           129
        // HANGUL            129
        // GB2312            134
        // Chinese BIG5      136
        // OEM               255
        // JOHAB             130
        // Hebrew            177
        // Arabic            178
        // Greek             161
        // Turkish           162
        // Vietnamese        163
        // Thai              222
        // EASTEUROPE        238
        // Russian           204
        // MAC               77
        // Baltic            186

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName;
        }

        internal static byte GetGdiCharSet(string fontName)
        {
            if (FontCharSets.ContainsKey(fontName)) return FontCharSets[fontName];
            using Font font = new Font(fontName, 16);
            LOGFONT obj = new LOGFONT();
            font.ToLogFont(obj);
            FontCharSets.TryAdd(fontName, obj.lfCharSet);
            return obj.lfCharSet;
        }

        internal static float DefaultFontSize = 12;
        internal static float DefaultSubFontSize = 9;

        /// <summary>
        /// Default font
        /// </summary>
        internal static Font Font()
        {
            byte gdiCharSet = GetGdiCharSet(System.Drawing.SystemFonts.DefaultFont.Name);
            return new Font(familyName: System.Drawing.SystemFonts.DefaultFont.Name, DefaultFontSize, FontStyle.Regular, GraphicsUnit.Point, gdiCharSet);
        }

        /// <summary>
        /// Default sub font
        /// </summary>
        internal static Font SubFont()
        {
            byte gdiCharSet = GetGdiCharSet(System.Drawing.SystemFonts.DefaultFont.Name);
            return new Font(System.Drawing.SystemFonts.DefaultFont.Name, DefaultSubFontSize, FontStyle.Regular, GraphicsUnit.Point, gdiCharSet);
        }

        public static List<UIStyle> PopularStyles()
        {
            List<UIStyle> styles = new List<UIStyle>();
            foreach (UIStyle style in Enum.GetValues(typeof(UIStyle)))
            {
                if (style.Value() >= UIStyle.Blue.Value() && style.Value() < UIStyle.Colorful.Value())
                {
                    styles.Add(style);
                }
            }

            return styles;
        }

        public static readonly UIBaseStyle Inherited = new UIInheritedStyle();

        /// <summary>
        /// Custom style
        /// </summary>
        private static readonly UIBaseStyle Custom = new UICustomStyle();

        /// <summary>
        /// Blue
        /// </summary>
        public static readonly UIBaseStyle Blue = new UIBlueStyle();

        /// <summary>
        /// Orange
        /// </summary>
        public static readonly UIBaseStyle Orange = new UIOrangeStyle();

        /// <summary>
        /// Gray
        /// </summary>
        public static readonly UIBaseStyle Gray = new UIGrayStyle();

        /// <summary>
        /// Green
        /// </summary>
        public static readonly UIBaseStyle Green = new UIGreenStyle();

        /// <summary>
        /// Red
        /// </summary>
        public static readonly UIBaseStyle Red = new UIRedStyle();

        /// <summary>
        /// Dark Blue
        /// </summary>
        public static readonly UIBaseStyle DarkBlue = new UIDarkBlueStyle();

        /// <summary>
        /// Black
        /// </summary>
        public static readonly UIBaseStyle Black = new UIBlackStyle();

        /// <summary>
        /// Purple
        /// </summary>
        public static readonly UIBaseStyle Purple = new UIPurpleStyle();

        /// <summary>
        /// Colorful
        /// </summary>
        private static readonly UIColorfulStyle Colorful = new UIColorfulStyle();

        public static void InitColorful(Color styleColor, Color foreColor)
        {
            Colorful.Init(styleColor, foreColor);
            SetStyle(UIStyle.Colorful);
        }

        internal static readonly ConcurrentDictionary<UIStyle, UIBaseStyle> Styles = new();
        internal static readonly ConcurrentDictionary<Guid, UIBaseForm> Forms = new();
        internal static readonly ConcurrentDictionary<Guid, UIPage> Pages = new();

        /// <summary>
        /// Menu color collection
        /// </summary>
        public static readonly ConcurrentDictionary<UIMenuStyle, UIMenuColor> MenuColors = new();

        static UIStyles()
        {
            AddStyle(Inherited);
            AddStyle(Custom);
            AddStyle(Blue);
            AddStyle(Orange);
            AddStyle(Gray);
            AddStyle(Green);
            AddStyle(Red);
            AddStyle(DarkBlue);

            AddStyle(new UIBaseStyle().Init(UIColor.LayuiGreen, UIStyle.LayuiGreen, Color.White, UIFontColor.Primary));
            AddStyle(new UIBaseStyle().Init(UIColor.LayuiRed, UIStyle.LayuiRed, Color.White, UIFontColor.Primary));
            AddStyle(new UIBaseStyle().Init(UIColor.LayuiOrange, UIStyle.LayuiOrange, Color.White, UIFontColor.Primary));

            AddStyle(Black);
            AddStyle(Purple);

            AddStyle(Colorful);

            MenuColors.TryAdd(UIMenuStyle.Custom, new UIMenuCustomColor());
            MenuColors.TryAdd(UIMenuStyle.Black, new UIMenuBlackColor());
            MenuColors.TryAdd(UIMenuStyle.White, new UIMenuWhiteColor());

            UIBuiltInResources lcn = new zh_CN_Resources();
            UIBuiltInResources len = new en_US_Resources();
            BuiltInResources.TryAdd(lcn.CultureInfo.LCID, lcn);
            BuiltInResources.TryAdd(len.CultureInfo.LCID, len);
        }

        /// <summary>
        /// Theme style integer value
        /// </summary>
        /// <param name="style">Theme style</param>
        /// <returns>Integer value</returns>
        public static int Value(this UIStyle style)
        {
            return (int)style;
        }

        /// <summary>
        /// Register form
        /// </summary>
        /// <param name="form">Form</param>
        public static bool Register(this UIBaseForm form)
        {
            if (!Forms.ContainsKey(form.Guid))
            {
                Forms.Upsert(form.Guid, form);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Register page
        /// </summary>
        /// <param name="page">Page</param>
        public static bool Register(this UIPage page)
        {
            if (!Pages.ContainsKey(page.Guid))
            {
                Pages.Upsert(page.Guid, page);
                return true;
            }

            return false;
        }

        public static List<T> GetPages<T>() where T : UIPage
        {
            List<T> result = new();
            foreach (var page in Pages)
            {
                if (page is T pg)
                    result.Add(pg);
            }

            return result;
        }

        /// <summary>
        /// Unregister form
        /// </summary>
        /// <param name="form">Form</param>
        public static void UnRegister(this UIBaseForm form)
        {
            Forms.TryRemove(form.Guid, out _);
        }

        /// <summary>
        /// Unregister page
        /// </summary>
        /// <param name="page">Page</param>
        public static void UnRegister(this UIPage page)
        {
            Pages.TryRemove(page.Guid, out _);
        }

        /// <summary>
        /// Get theme style
        /// </summary>
        /// <param name="style">Theme style name</param>
        /// <returns>Theme style</returns>
        public static UIBaseStyle GetStyleColor(UIStyle style)
        {
            if (Styles.ContainsKey(style))
            {
                return Styles[style];
            }

            Style = UIStyle.Blue;
            return Styles[Style];
        }

        public static UIBaseStyle ActiveStyleColor => GetStyleColor(Style);

        private static void AddStyle(UIBaseStyle uiColor)
        {
            if (Styles.ContainsKey(uiColor.Name))
            {
                MessageBox.Show(uiColor.Name + " is already exist.");
            }

            Styles.TryAdd(uiColor.Name, uiColor);
        }

        /// <summary>
        /// Theme Style
        /// </summary>
        public static UIStyle Style { get; private set; } = UIStyle.Inherited;

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="style">Theme style</param>
        public static void SetStyle(UIStyle style)
        {
            if (style != UIStyle.Colorful && Style == style) return;
            Style = style;
            if (!style.IsValid()) return;

            foreach (var form in Forms.Values)
            {
                form.SetInheritedStyle(style);
            }

            foreach (var page in Pages.Values)
            {
                page.SetInheritedStyle(style);
            }
        }

        public static void Render()
        {
            SetStyle(Style);
        }

        public static void SetDPIScale()
        {
            foreach (var form in Forms.Values)
            {
                if (UIDPIScale.NeedSetDPIFont())
                    form.SetDPIScale();
            }

            foreach (var page in Pages.Values)
            {
                if (UIDPIScale.NeedSetDPIFont())
                    page.SetDPIScale();
            }
        }

        public static void Translate()
        {
            foreach (var form in Forms.Values)
            {
                form.Translate();
            }
        }

        private static CultureInfo _cultureInfo = CultureInfos.zh_CN;
        public static CultureInfo CultureInfo
        {
            get => _cultureInfo;
            set
            {
                if (value == null) return;

                if (value.LCID != _cultureInfo.LCID)
                {
                    if (BuiltInResources.TryGetValue(value.LCID, out var cultureInfo))
                    {
                        _cultureInfo = cultureInfo.CultureInfo;
                    }
                    else
                    {
                        _cultureInfo = CultureInfos.zh_CN;
                    }

                    UIStyles.Translate();
                }
            }
        }

        public static readonly ConcurrentDictionary<int, UIBuiltInResources> BuiltInResources = new();
        public static UIBuiltInResources CurrentResources => BuiltInResources[CultureInfo.LCID];
    }

}
