
using System.ComponentModel;
using System.Drawing;

namespace Exoplanet.UI
{
    /// <summary>
    /// 菜单主题样色
    /// </summary>
    public enum UIMenuStyle
    {
        /// <summary>
        /// 自定义
        /// </summary>
        [Description("Custom")]
        Custom,

        /// <summary>
        /// 黑
        /// </summary>
        [Description("Black")]
        Black,

        /// <summary>
        /// 白
        /// </summary>
        [Description("White")]
        White
    }

    public abstract class UIMenuColor
    {
        public abstract UIMenuStyle Style { get; }
        public virtual Color BackColor => Color.FromArgb(56, 56, 56);
        public virtual Color SelectedColor => Color.FromArgb(36, 36, 36);
        public virtual Color SelectedColor2 => Color.FromArgb(36, 36, 36);
        public virtual Color UnSelectedForeColor => Color.FromArgb(240, 240, 240);
        public virtual Color HoverColor => Color.FromArgb(76, 76, 76);
        public virtual Color SecondBackColor => Color.FromArgb(66, 66, 66);

        public override string ToString()
        {
            return Style.Description();
        }
    }

    public class UIMenuCustomColor : UIMenuColor
    {
        public override UIMenuStyle Style => UIMenuStyle.Custom;
    }

    public class UIMenuBlackColor : UIMenuColor
    {
        public override UIMenuStyle Style => UIMenuStyle.Black;
    }

    public class UIMenuWhiteColor : UIMenuColor
    {
        public override UIMenuStyle Style => UIMenuStyle.White;
        public override Color BackColor => Color.FromArgb(240, 240, 240);
        public override Color SelectedColor => Color.FromArgb(250, 250, 250);
        public override Color SelectedColor2 => Color.FromArgb(250, 250, 250);
        public override Color UnSelectedForeColor => UIFontColor.Primary;
        public override Color HoverColor => Color.FromArgb(230, 230, 230);
        public override Color SecondBackColor => Color.FromArgb(235, 235, 235);
    }
}