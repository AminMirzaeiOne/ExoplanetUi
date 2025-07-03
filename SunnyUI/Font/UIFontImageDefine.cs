

using System;
using System.Drawing;

namespace Exoplanet.UI
{
    public interface ISymbol
    {
        int Symbol { get; set; }

        int SymbolSize { get; set; }

        Point SymbolOffset { get; set; }

        int SymbolRotate { get; set; }
    }

    public class SymbolValue
    {
        /// <summary>
        /// 字体图标
        /// </summary>
        public int Symbol { get; set; }

        public UISymbolType SymbolType { get; set; }

        public string Name { get; set; }

        public bool IsRed { get; set; }

        public SymbolValue()
        {

        }

        public SymbolValue(int symbol, string name, UISymbolType symbolType)
        {
            Symbol = symbol;
            SymbolType = symbolType;
            Name = name;
        }

        public override string ToString()
        {
            if (Name.IsValid())
                return Name + Environment.NewLine + Value.ToString();
            else
                return Value.ToString();
        }

        public int Value => Symbol + (int)SymbolType * 100000;
    }

    public enum UISymbolType
    {
        FontAwesomeV4 = 0,
        FontAwesomeV6Brands = 1,
        FontAwesomeV6Regular = 2,
        FontAwesomeV6Solid = 3,
        ElegantIcons = 4,
        MaterialIcons = 5
    }
}