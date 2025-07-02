
namespace Exoplanet.UI
{
    public partial class UIHeaderAsideMainFooterFrame : UIHeaderAsideMainFrame
    {
        public UIHeaderAsideMainFooterFrame()
        {
            InitializeComponent();

            Controls.SetChildIndex(MainTabControl, 0);
            Header.Parent = this;
            Aside.Parent = this;
            MainTabControl.Parent = this;
            Footer.Parent = this;
            Aside.BringToFront();
            Footer.BringToFront();
            MainTabControl.BringToFront();
            Aside.TabControl = MainTabControl;
        }
    }
}