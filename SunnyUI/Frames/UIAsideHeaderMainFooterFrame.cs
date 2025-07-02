
namespace Exoplanet.UI
{
    public partial class UIAsideHeaderMainFooterFrame : UIAsideHeaderMainFrame
    {
        public UIAsideHeaderMainFooterFrame()
        {
            InitializeComponent();
            Controls.SetChildIndex(MainTabControl, 0);
            Header.Parent = this;
            Aside.Parent = this;
            MainTabControl.Parent = this;
            Footer.Parent = this;
            Header.BringToFront();
            Footer.BringToFront();
            MainTabControl.BringToFront();
            Aside.TabControl = MainTabControl;
        }
    }
}