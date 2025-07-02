
using System;
using System.Collections.Generic;

namespace Exoplanet.UI
{
    public interface IFrame
    {
        UITabControl MainTabControl { get; }

        UIPage AddPage(UIPage page, int pageIndex);

        UIPage AddPage(UIPage page, Guid pageGuid);

        UIPage AddPage(UIPage page);

        bool SelectPage(int pageIndex);

        bool SelectPage(Guid pageGuid);

        UIPage GetPage(int pageIndex);

        UIPage GetPage(Guid pageGuid);

        bool TopMost { get; set; }

        bool RemovePage(int pageIndex);

        bool RemovePage(Guid pageGuid);

        bool ExistPage(int pageIndex);

        bool ExistPage(Guid pageGuid);

        void Init();

        void Final();

        T GetPage<T>() where T : UIPage;

        List<T> GetPages<T>() where T : UIPage;

        UIPage SelectedPage { get; }
    }

    public class UIPageParamsArgs : EventArgs
    {
        public UIPage SourcePage { get; set; }

        public UIPage DestPage { get; set; }

        public object Value { get; set; }

        public bool Handled { get; set; } = false;

        public UIPageParamsArgs()
        {

        }

        public UIPageParamsArgs(UIPage sourcePage, UIPage destPage, object value)
        {
            SourcePage = sourcePage;
            DestPage = destPage;
            Value = value;
        }
    }

    public delegate void OnReceiveParams(object sender, UIPageParamsArgs e);
}
