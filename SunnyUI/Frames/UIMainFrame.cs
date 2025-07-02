
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Exoplanet.UI
{
    public partial class UIMainFrame : UIForm
    {
        public UIMainFrame()
        {
            InitializeComponent();
            MainContainer.TabVisible = false;
            MainContainer.BringToFront();
            MainContainer.TabPages.Clear();

            MainContainer.BeforeRemoveTabPage += MainContainer_BeforeRemoveTabPage;
            MainContainer.AfterRemoveTabPage += MainContainer_AfterRemoveTabPage;
        }

        private void MainContainer_AfterRemoveTabPage(object sender, int index)
        {
            AfterRemoveTabPage?.Invoke(this, index);
        }

        private bool MainContainer_BeforeRemoveTabPage(object sender, int index)
        {
            return BeforeRemoveTabPage == null || BeforeRemoveTabPage.Invoke(this, index);
        }

        public event UITabControl.OnBeforeRemoveTabPage BeforeRemoveTabPage;

        public event UITabControl.OnAfterRemoveTabPage AfterRemoveTabPage;


        protected override void OnShown(EventArgs e)
        {
            MainContainer.BringToFront();
            base.OnShown(e);
        }

        public bool TabVisible
        {
            get => MainContainer.TabVisible;
            set => MainContainer.TabVisible = value;
        }

        public bool TabShowCloseButton
        {
            get => MainContainer.ShowCloseButton;
            set => MainContainer.ShowCloseButton = value;
        }

        public bool TabShowActiveCloseButton
        {
            get => MainContainer.ShowActiveCloseButton;
            set => MainContainer.ShowActiveCloseButton = value;
        }

        private void MainContainer_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (Selecting != null && e.TabPage != null)
            {
                List<UIPage> pages = e.TabPage.GetControls<UIPage>();
                Selecting?.Invoke(this, e, pages.Count == 0 ? null : pages[0]);
            }
        }

        public delegate void OnSelecting(object sender, TabControlCancelEventArgs e, UIPage page);

        [Description("页面选择事件"), Category("SunnyUI")]
        public event OnSelecting Selecting;
    }
}