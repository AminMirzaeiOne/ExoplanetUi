
using System.Windows.Forms;

namespace Exoplanet.UI
{
    public partial class UIHeaderAsideMainFrame : UIHeaderMainFrame
    {
        public UIHeaderAsideMainFrame()
        {
            InitializeComponent();
            Controls.SetChildIndex(MainTabControl, 0);
            Header.Parent = this;
            Aside.Parent = this;
            MainTabControl.Parent = this;
            Aside.BringToFront();
            MainTabControl.BringToFront();
            Aside.TabControl = MainTabControl;
        }

        public override bool SelectPage(int pageIndex)
        {
            bool result = base.SelectPage(pageIndex);
            TreeNode node = Aside.GetTreeNode(pageIndex);
            if (node != null) Aside.SelectedNode = node;
            return result;
        }
    }
}