
namespace Exoplanet.UI
{
    public partial class UIAsideHeaderMainFrame : UIAsideMainFrame
    {
        public UIAsideHeaderMainFrame()
        {
            InitializeComponent();
            Controls.SetChildIndex(MainTabControl, 0);
            Header.Parent = this;
            Aside.Parent = this;
            MainTabControl.Parent = this;
            Header.BringToFront();
            MainTabControl.BringToFront();
            Aside.TabControl = MainTabControl;
        }
    }
}