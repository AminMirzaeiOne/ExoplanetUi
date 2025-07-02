
using System.Drawing;

namespace Exoplanet.UI
{
    public static class UITexture
    {
        public static TextureBrush CreateTextureBrush(Image img)
        {
            TextureBrush tb = new TextureBrush(img);
            tb.WrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
            return tb;
        }
    }
}
