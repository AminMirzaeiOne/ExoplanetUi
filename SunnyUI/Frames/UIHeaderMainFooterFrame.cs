
namespace Exoplanet.UI
{
    public partial class UIHeaderMainFooterFrame : UIHeaderMainFrame
    {
        public UIHeaderMainFooterFrame()
        {
            InitializeComponent();
            Controls.SetChildIndex(MainTabControl, 0);
            Header.Parent = this;
            Footer.Parent = this;
            MainTabControl.Parent = this;
            MainTabControl.BringToFront();
        }
    }
}