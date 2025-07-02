
using System.ComponentModel;
using static System.Drawing.FontConverter;

namespace Exoplanet.UI
{
    /// <summary>
    /// Theme Style Manager Class
    /// </summary>
    [Description("Theme Style Management Control")]
    public class UIStyleManager : Component
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UIStyleManager()
        {
            Version = UIGlobal.Version;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container">Component container</param>
        public UIStyleManager(IContainer container) : this()
        {
            container.Add(this);
            Version = UIGlobal.Version;
        }

        /// <summary>
        /// Theme style
        /// </summary>
        [DefaultValue(UIStyle.Inherited), Description("Theme Style"), Category("SunnyUI")]
        [Browsable(false)]
        public UIStyle Style
        {
            get => UIStyles.Style;
            set => UIStyles.SetStyle(value);
        }

        [DefaultValue(false), Description("DPI Scaling"), Category("SunnyUI")]
        public bool DPIScale
        {
            get => UIStyles.DPIScale;
            set => UIStyles.DPIScale = value;
        }

        [Editor("System.Drawing.Design.FontNameEditor", "System.Drawing.Design.UITypeEditor")]
        [TypeConverter(typeof(FontNameConverter))]
        [DefaultValue("SimSun")]
        [Description("Adjustable font name when global font setting is enabled"), Category("SunnyUI")]
        public string GlobalFontName
        {
            get => UIStyles.GlobalFontName;
            set => UIStyles.GlobalFontName = value;
        }

        [DefaultValue(100)]
        [Description("Adjustable font size scaling percentage when global font setting is enabled (default 100 %)"), Category("SunnyUI")]
        public int GlobalFontScale
        {
            get => UIStyles.GlobalFontScale;
            set => UIStyles.GlobalFontScale = value;
        }

        [DefaultValue(false)]
        [Description("Enable global font setting"), Category("SunnyUI")]
        public bool GlobalFont
        {
            get => UIStyles.GlobalFont;
            set => UIStyles.GlobalFont = value;
        }

        [DefaultValue(false)]
        [Description("Enable global rectangle setting; disables rounded corners for all controls"), Category("SunnyUI")]
        public bool GlobalRectangle
        {
            get => UIStyles.GlobalRectangle;
            set => UIStyles.GlobalRectangle = value;
        }

        [DefaultValue(false)]
        [Description("Multilanguage support"), Category("SunnyUI")]
        public bool MultiLanguageSupport
        {
            get => UIStyles.MultiLanguageSupport;
            set => UIStyles.MultiLanguageSupport = value;
        }

        /// <summary>
        /// Version
        /// </summary>
        public string Version
        {
            get;
        }
    }

}