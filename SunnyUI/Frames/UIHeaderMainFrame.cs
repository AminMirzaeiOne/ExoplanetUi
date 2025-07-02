
namespace Exoplanet.UI
{
    public partial class UIHeaderMainFrame : UIMainFrame
    {
        public UIHeaderMainFrame()
        {
            InitializeComponent();
            Controls.SetChildIndex(MainTabControl, 0);
            Header.Parent = this;
            MainTabControl.Parent = this;
            MainTabControl.BringToFront();
        }
    }
}