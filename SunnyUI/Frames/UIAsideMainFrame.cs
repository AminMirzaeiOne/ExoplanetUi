
using System.Windows.Forms;

namespace Exoplanet.UI
{
    public partial class UIAsideMainFrame : UIMainFrame
    {
        public UIAsideMainFrame()
        {
            InitializeComponent();
            Controls.SetChildIndex(MainTabControl, 0);
            Aside.Parent = this;
            MainTabControl.Parent = this;
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