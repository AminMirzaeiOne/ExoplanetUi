
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Exoplanet.UI
{
    public partial class UIForm : UIBaseForm
    {
        public UIForm()
        {
            base.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;//设置最大化尺寸
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();

            FormBorderStyle = FormBorderStyle.None;
            m_aeroEnabled = false;
        }

        /// <summary>
        /// 禁止控件跟随窗体缩放
        /// </summary>
        [DefaultValue(false), Category("SunnyUI"), Description("禁止控件跟随窗体缩放")]
        public bool ZoomScaleDisabled { get; set; }

        private void SetZoomScaleRect()
        {
            if (ZoomScaleRect.Width == 0 && ZoomScaleRect.Height == 0)
            {
                ZoomScaleRect = new Rectangle(ZoomScaleSize.Width, ZoomScaleSize.Height, 0, 0);
            }

            if (ZoomScaleRect.Width == 0 && ZoomScaleRect.Height == 0)
            {
                ZoomScaleRect = new Rectangle(Left, Top, Width, Height);
            }

            ZoomScaleRectChanged?.Invoke(this, ZoomScaleRect);
        }

        public event OnZoomScaleRectChanged ZoomScaleRectChanged;

        [DefaultValue(typeof(Size), "0, 0")]
        [Description("设计界面大小"), Category("SunnyUI")]
        public Size ZoomScaleSize
        {
            get;
            set;
        }

        /// <summary>
        /// 控件缩放前在其容器里的位置
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Rectangle), "0, 0, 0, 0")]
        public Rectangle ZoomScaleRect { get; set; }

        /// <summary>
        /// 设置控件缩放比例
        /// </summary>
        /// <param name="scale">缩放比例</param>
        private void SetZoomScale()
        {
            if (ZoomScaleDisabled) return;
            if (!UIStyles.DPIScale || !UIStyles.ZoomScale) return;
            if (ZoomScaleRect.Width == 0 || ZoomScaleRect.Height == 0) return;
            if (Width == 0 || Height == 0) return;
            float scale = Math.Min(Width * 1.0f / ZoomScaleRect.Width, Height * 1.0f / ZoomScaleRect.Height);
            if (scale.EqualsFloat(0)) return;
            foreach (Control control in this.GetAllZoomScaleControls())
            {
                if (control is IZoomScale ctrl)
                {
                    UIZoomScale.SetZoomScale(control, scale);
                }
            }

            ZoomScaleChanged?.Invoke(this, scale);
        }

        public event OnZoomScaleChanged ZoomScaleChanged;

        //不显示FormBorderStyle属性
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new FormBorderStyle FormBorderStyle
        {
            get
            {
                return base.FormBorderStyle;
            }
            set
            {
                if (!Enum.IsDefined(typeof(FormBorderStyle), value))
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FormBorderStyle));
                base.FormBorderStyle = FormBorderStyle.None;
            }
        }

        protected override void CalcSystemBoxPos()
        {
            ControlBoxLeft = Width;

            if (ControlBox)
            {
                ControlBoxRect = new Rectangle(Width - 6 - 28, titleHeight / 2 - 14, 28, 28);
                ControlBoxLeft = ControlBoxRect.Left - 2;

                if (MaximizeBox)
                {
                    MaximizeBoxRect = new Rectangle(ControlBoxRect.Left - 28 - 2, ControlBoxRect.Top, 28, 28);
                    ControlBoxLeft = MaximizeBoxRect.Left - 2;
                }
                else
                {
                    MaximizeBoxRect = new Rectangle(Width + 1, Height + 1, 1, 1);
                }

                if (MinimizeBox)
                {
                    MinimizeBoxRect = new Rectangle(MaximizeBox ? MaximizeBoxRect.Left - 28 - 2 : ControlBoxRect.Left - 28 - 2, ControlBoxRect.Top, 28, 28);
                    ControlBoxLeft = MinimizeBoxRect.Left - 2;
                }
                else
                {
                    MinimizeBoxRect = new Rectangle(Width + 1, Height + 1, 1, 1);
                }

                if (ExtendBox)
                {
                    if (MinimizeBox)
                    {
                        ExtendBoxRect = new Rectangle(MinimizeBoxRect.Left - 28 - 2, ControlBoxRect.Top, 28, 28);
                    }
                    else
                    {
                        ExtendBoxRect = new Rectangle(ControlBoxRect.Left - 28 - 2, ControlBoxRect.Top, 28, 28);
                    }

                    ControlBoxLeft = ExtendBoxRect.Left - 2;
                }

                if (ControlBoxLeft != Width) ControlBoxLeft -= 6;
            }
            else
            {
                ExtendBoxRect = MaximizeBoxRect = MinimizeBoxRect = ControlBoxRect = new Rectangle(Width + 1, Height + 1, 1, 1);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (FormBorderStyle == FormBorderStyle.None && ShowTitle)
            {
                if (InControlBox)
                {
                    InControlBox = false;
                    Close();
                }

                if (InMinBox)
                {
                    InMinBox = false;
                    DoWindowStateChanged(FormWindowState.Minimized);
                    WindowState = FormWindowState.Minimized;
                }

                if (InMaxBox)
                {
                    InMaxBox = false;
                    ShowMaximize();
                }

                if (InExtendBox)
                {
                    InExtendBox = false;
                    if (ExtendMenu != null)
                    {
                        this.ShowContextMenuStrip(ExtendMenu, ExtendBoxRect.Left, TitleHeight - 1);
                    }
                    else
                    {
                        ExtendBoxClick?.Invoke(this, EventArgs.Empty);
                    }
                }
            }

            base.OnMouseClick(e);
        }

        public event EventHandler ExtendBoxClick;

        private void ShowMaximize()
        {
            Screen screen = Screen.FromPoint(MousePosition);
            base.MaximumSize = ShowFullScreen ? screen.Bounds.Size : screen.WorkingArea.Size;
            if (screen.Primary)
                MaximizedBounds = ShowFullScreen ? screen.Bounds : screen.WorkingArea;
            else
                MaximizedBounds = new Rectangle(0, 0, 0, 0);

            if (WindowState == FormWindowState.Normal)
            {
                FormEx.SetFormRoundRectRegion(this, 0);
                DoWindowStateChanged(FormWindowState.Maximized);
                WindowState = FormWindowState.Maximized;
            }
            else if (WindowState == FormWindowState.Maximized)
            {
                FormEx.SetFormRoundRectRegion(this, ShowRadius ? 5 : 0);
                DoWindowStateChanged(FormWindowState.Normal);
                WindowState = FormWindowState.Normal;
            }

            Invalidate();
        }

        private bool FormMoveMouseDown;

        /// <summary>
        /// 鼠标左键按下时，窗体的位置
        /// </summary>
        private Point FormLocation;

        /// <summary>
        /// 鼠标左键按下时，鼠标的位置
        /// </summary>
        private Point mouseOffset;

        /// <summary>
        /// 重载鼠标按下事件
        /// </summary>
        /// <param name="e">鼠标参数</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (InControlBox || InMaxBox || InMinBox || InExtendBox) return;
            if (!ShowTitle) return;
            if (e.Y > Padding.Top) return;

            if (e.Button == MouseButtons.Left && Movable)
            {
                FormMoveMouseDown = true;
                FormLocation = Location;
                mouseOffset = MousePosition;
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (!MaximizeBox) return;
            if (InControlBox || InMaxBox || InMinBox || InExtendBox) return;
            if (!ShowTitle) return;
            if (e.Y > Padding.Top) return;
            if (e.Y == 0) return;

            ShowMaximize();
        }

        private long stickyBorderTime = 5000000;

        /// <summary>
        /// 设置或获取显示器边缘停留的最大时间(ms)，默认500ms
        /// </summary>
        [Description("设置或获取在显示器边缘停留的最大时间(ms)"), Category("SunnyUI")]
        [DefaultValue(500)]
        public long StickyBorderTime
        {
            get => stickyBorderTime / 10000;
            set => stickyBorderTime = value * 10000;
        }

        /// <summary>
        /// 是否触发在显示器边缘停留事件
        /// </summary>
        private bool IsStayAtTopBorder;

        /// <summary>
        /// 显示器边缘停留事件被触发的时间
        /// </summary>
        private long TopBorderStayTicks;

        /// <summary>
        /// 重载鼠标抬起事件
        /// </summary>
        /// <param name="e">鼠标参数</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!IsDisposed && FormMoveMouseDown)
            {
                //int screenIndex = GetMouseInScreen(PointToScreen(e.Location));
                Screen screen = Screen.FromPoint(MousePosition);
                if (MousePosition.Y == screen.WorkingArea.Top && MaximizeBox && WindowState == FormWindowState.Normal)
                {
                    ShowMaximize();
                }

                // 防止窗体上移时标题栏超出容器，导致后续无法移动
                if (Top < screen.WorkingArea.Top)
                {
                    Top = screen.WorkingArea.Top;
                }

                // 防止窗体下移时标题栏超出容器，导致后续无法移动
                if (Top > screen.WorkingArea.Bottom - TitleHeight)
                {
                    Top = screen.WorkingArea.Bottom - TitleHeight;
                }
            }

            // 鼠标抬起后强行关闭粘滞并恢复鼠标移动区域
            IsStayAtTopBorder = false;
            Cursor.Clip = new Rectangle();
            FormMoveMouseDown = false;
        }

        /// <summary>
        /// 重载鼠标移动事件
        /// </summary>
        /// <param name="e">鼠标参数</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (FormMoveMouseDown && !MousePosition.Equals(mouseOffset))
            {
                if (WindowState == FormWindowState.Maximized && (Math.Abs(MousePosition.X - mouseOffset.X) >= 6 || Math.Abs(MousePosition.Y - mouseOffset.Y) >= 6))
                {
                    int MaximizedWidth = Width;
                    int LocationX = Left;
                    ShowMaximize();
                    // 计算等比例缩放后，鼠标与原位置的相对位移
                    float offsetXRatio = 1 - (float)Width / MaximizedWidth;
                    mouseOffset.X -= (int)((mouseOffset.X - LocationX) * offsetXRatio);
                }

                int offsetX = mouseOffset.X - MousePosition.X;
                int offsetY = mouseOffset.Y - MousePosition.Y;
                Rectangle WorkingArea = Screen.GetWorkingArea(this);

                // 若当前鼠标停留在容器上边缘，将会触发一个时间为MaximumBorderInterval(ms)的边缘等待，
                // 若此时结束移动，窗口将自动最大化，该功能为上下排列的多监视器提供
                // 此处判断设置为特定值的好处是，若快速移动窗体跨越监视器，很难触发停留事件
                if (MousePosition.Y - WorkingArea.Top == 0)
                {
                    if (!IsStayAtTopBorder)
                    {
                        Cursor.Clip = WorkingArea;
                        TopBorderStayTicks = DateTime.Now.Ticks;
                        IsStayAtTopBorder = true;
                    }
                    else if (DateTime.Now.Ticks - TopBorderStayTicks > stickyBorderTime)
                    {
                        Cursor.Clip = new Rectangle();
                    }
                }

                Location = new Point(FormLocation.X - offsetX, FormLocation.Y - offsetY);
            }
            else
            {
                if (FormBorderStyle == FormBorderStyle.None)
                {
                    bool inControlBox = e.Location.InRect(ControlBoxRect);
                    if (WindowState == FormWindowState.Maximized && ControlBox)
                    {
                        if (e.Location.X > ControlBoxRect.Left && e.Location.Y < TitleHeight)
                            inControlBox = true;
                    }

                    bool inMaxBox = e.Location.InRect(MaximizeBoxRect);
                    bool inMinBox = e.Location.InRect(MinimizeBoxRect);
                    bool inExtendBox = e.Location.InRect(ExtendBoxRect);
                    bool isChange = false;

                    if (inControlBox != InControlBox)
                    {
                        InControlBox = inControlBox;
                        isChange = true;
                    }

                    if (inMaxBox != InMaxBox)
                    {
                        InMaxBox = inMaxBox;
                        isChange = true;
                    }

                    if (inMinBox != InMinBox)
                    {
                        InMinBox = inMinBox;
                        isChange = true;
                    }

                    if (inExtendBox != InExtendBox)
                    {
                        InExtendBox = inExtendBox;
                        isChange = true;
                    }

                    if (isChange)
                    {
                        Invalidate();
                    }
                }
                else
                {
                    InExtendBox = InControlBox = InMaxBox = InMinBox = false;
                }
            }

            base.OnMouseMove(e);
        }

        /// <summary>
        /// 重载绘图
        /// </summary>
        /// <param name="e">绘图参数</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            if (FormBorderStyle != FormBorderStyle.None)
            {
                return;
            }

            if (ShowTitle)
            {
                e.Graphics.FillRectangle(titleColor, 0, 0, Width, TitleHeight);
                e.Graphics.DrawLine(RectColor, 0, titleHeight, Width, titleHeight);
            }

            if (ShowRect)
            {
                Point[] points;
                bool unShowRadius = !ShowRadius || WindowState == FormWindowState.Maximized ||
                                    (Width == Screen.PrimaryScreen.WorkingArea.Width &&
                                     Height == Screen.PrimaryScreen.WorkingArea.Height);
                if (unShowRadius)
                {
                    points = new[]
                    {
                        new Point(0, 0),
                        new Point(Width - 1, 0),
                        new Point(Width - 1, Height - 1),
                        new Point(0, Height - 1),
                        new Point(0, 0)
                    };
                }
                else
                {
                    points = new[]
                    {
                            new Point(0, 2),
                            new Point(2, 0),
                            new Point(Width - 1 - 2, 0),
                            new Point(Width - 1, 2),
                            new Point(Width - 1, Height - 1 - 2),
                            new Point(Width - 1 - 2, Height - 1),
                            new Point(2, Height - 1),
                            new Point(0, Height - 1 - 2),
                            new Point(0, 2)
                        };
                }

                e.Graphics.DrawLines(rectColor, points);

                if (!unShowRadius)
                {
                    e.Graphics.DrawLine(Color.FromArgb(120, rectColor), new Point(2, 1), new Point(1, 2));
                    e.Graphics.DrawLine(Color.FromArgb(120, rectColor), new Point(2, Height - 1 - 1), new Point(1, Height - 1 - 2));
                    e.Graphics.DrawLine(Color.FromArgb(120, rectColor), new Point(Width - 1 - 2, 1), new Point(Width - 1 - 1, 2));
                    e.Graphics.DrawLine(Color.FromArgb(120, rectColor), new Point(Width - 1 - 2, Height - 1 - 1), new Point(Width - 1 - 1, Height - 1 - 2));
                }
            }

            if (!ShowTitle)
            {
                return;
            }

            int titleLeft = 6;
            if (ShowIcon && Icon != null)
            {
                try
                {
                    if (IconImage != null)
                    {
                        e.Graphics.DrawImage(IconImage, new Rectangle(6, (TitleHeight - IconImageSize) / 2 + 1, IconImageSize, IconImageSize), new Rectangle(0, 0, IconImage.Width, IconImage.Height), GraphicsUnit.Pixel);
                        titleLeft = 6 + IconImageSize + 2;
                    }
                    else
                    {
                        using (Image image = IconToImage(Icon))
                        {
                            e.Graphics.DrawImage(image, 6, (TitleHeight - 24) / 2 + 1, 24, 24);
                        }

                        titleLeft = 6 + 24 + 2;
                    }
                }
                catch
                {
                    Console.WriteLine("图标转换错误");
                }
            }

            if (TextAlignment == StringAlignment.Center)
            {
                e.Graphics.DrawString(Text, TitleFont, titleForeColor, new Rectangle(0, 0, Width, TitleHeight), ContentAlignment.MiddleCenter);
            }
            else
            {
                e.Graphics.DrawString(Text, TitleFont, titleForeColor, new Rectangle(titleLeft, 0, Width, TitleHeight), ContentAlignment.MiddleLeft);
            }

            if (ControlBoxLeft != Width)
            {
                e.Graphics.FillRectangle(TitleColor, new Rectangle(ControlBoxLeft, 1, Width - ControlBoxLeft - 1, TitleHeight - 2));
            }

            e.Graphics.SetHighQuality();
            if (ControlBox)
            {
                if (InControlBox)
                {
                    if (WindowState == FormWindowState.Maximized)
                    {
                        e.Graphics.FillRectangle(ControlBoxCloseFillHoverColor, new Rectangle(ControlBoxRect.Left, 0, Width - ControlBoxRect.Left, TitleHeight));
                    }
                    else
                    {
                        if (ShowRadius)
                            e.Graphics.FillRoundRectangle(ControlBoxCloseFillHoverColor, ControlBoxRect, 5);
                        else
                            e.Graphics.FillRectangle(ControlBoxCloseFillHoverColor, ControlBoxRect);
                    }
                }

                e.Graphics.DrawLine(controlBoxForeColor,
                    ControlBoxRect.Left + ControlBoxRect.Width / 2 - 5,
                    ControlBoxRect.Top + ControlBoxRect.Height / 2 - 5,
                    ControlBoxRect.Left + ControlBoxRect.Width / 2 + 5,
                    ControlBoxRect.Top + ControlBoxRect.Height / 2 + 5);
                e.Graphics.DrawLine(controlBoxForeColor,
                    ControlBoxRect.Left + ControlBoxRect.Width / 2 - 5,
                    ControlBoxRect.Top + ControlBoxRect.Height / 2 + 5,
                    ControlBoxRect.Left + ControlBoxRect.Width / 2 + 5,
                    ControlBoxRect.Top + ControlBoxRect.Height / 2 - 5);
            }

            if (MaximizeBox)
            {
                if (InMaxBox)
                {
                    if (ShowRadius)
                        e.Graphics.FillRoundRectangle(ControlBoxFillHoverColor, MaximizeBoxRect, 5);
                    else
                        e.Graphics.FillRectangle(ControlBoxFillHoverColor, MaximizeBoxRect);
                }

                if (WindowState == FormWindowState.Maximized)
                {
                    e.Graphics.DrawRectangle(controlBoxForeColor,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 - 5,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 - 1,
                        7, 7);

                    e.Graphics.DrawLine(controlBoxForeColor,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 - 2,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 - 1,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 - 2,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 - 4);

                    e.Graphics.DrawLine(controlBoxForeColor,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 - 2,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 - 4,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 + 5,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 - 4);

                    e.Graphics.DrawLine(controlBoxForeColor,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 + 5,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 - 4,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 + 5,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 + 3);

                    e.Graphics.DrawLine(controlBoxForeColor,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 + 5,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 + 3,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 + 3,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 + 3);
                }

                if (WindowState == FormWindowState.Normal)
                {
                    e.Graphics.DrawRectangle(controlBoxForeColor,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 - 5,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 - 4,
                        10, 9);
                }
            }

            if (MinimizeBox)
            {
                if (InMinBox)
                {
                    if (ShowRadius)
                        e.Graphics.FillRoundRectangle(ControlBoxFillHoverColor, MinimizeBoxRect, 5);
                    else
                        e.Graphics.FillRectangle(ControlBoxFillHoverColor, MinimizeBoxRect);
                }

                e.Graphics.DrawLine(controlBoxForeColor,
                    MinimizeBoxRect.Left + MinimizeBoxRect.Width / 2 - 6,
                    MinimizeBoxRect.Top + MinimizeBoxRect.Height / 2,
                    MinimizeBoxRect.Left + MinimizeBoxRect.Width / 2 + 5,
                    MinimizeBoxRect.Top + MinimizeBoxRect.Height / 2);
            }

            if (ExtendBox)
            {
                if (InExtendBox)
                {
                    if (ShowRadius)
                        e.Graphics.FillRoundRectangle(ControlBoxFillHoverColor, ExtendBoxRect, 5);
                    else
                        e.Graphics.FillRectangle(ControlBoxFillHoverColor, ExtendBoxRect);
                }

                if (ExtendSymbol == 0)
                {
                    e.Graphics.DrawLine(controlBoxForeColor,
                        ExtendBoxRect.Left + ExtendBoxRect.Width / 2 - 5 - 1,
                        ExtendBoxRect.Top + ExtendBoxRect.Height / 2 - 2,
                        ExtendBoxRect.Left + ExtendBoxRect.Width / 2 - 1,
                        ExtendBoxRect.Top + ExtendBoxRect.Height / 2 + 3);

                    e.Graphics.DrawLine(controlBoxForeColor,
                        ExtendBoxRect.Left + ExtendBoxRect.Width / 2 + 5 - 1,
                        ExtendBoxRect.Top + ExtendBoxRect.Height / 2 - 2,
                        ExtendBoxRect.Left + ExtendBoxRect.Width / 2 - 1,
                        ExtendBoxRect.Top + ExtendBoxRect.Height / 2 + 3);
                }
                else
                {
                    e.Graphics.DrawFontImage(ExtendSymbol, ExtendSymbolSize, controlBoxForeColor, ExtendBoxRect, ExtendSymbolOffset.X, ExtendSymbolOffset.Y);
                }
            }

            e.Graphics.SetDefaultQuality();
        }

        /// <summary>
        /// 自定义主题风格
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("获取或设置可以自定义主题风格"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

        /// <summary>
        /// 重载控件尺寸变更
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetZoomScale();
            CalcSystemBoxPos();

            if (IsShown)
            {
                SetRadius();
            }
        }

        protected virtual void AfterSetBackColor(Color color)
        {
        }

        protected virtual void AfterSetForeColor(Color color)
        {
        }

        private bool IsShown;

        [Description("背景颜色"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "Control")]
        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            SetRadius();
            IsShown = true;
            SetZoomScaleRect();
        }

        /// <summary>
        /// 是否显示圆角
        /// </summary>
        private bool _showRadius = false;

        /// <summary>
        /// 是否显示圆角
        /// </summary>
        [Description("是否显示圆角"), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool ShowRadius
        {
            get
            {
                return (_showRadius && !_showShadow && !UIStyles.GlobalRectangle);
            }
            set
            {
                _showRadius = value;
                SetRadius();
                Invalidate();
            }
        }

        /// <summary>
        /// 是否显示阴影
        /// </summary>
        private bool _showShadow = true;

        #region 边框阴影

        /// <summary>
        /// 是否显示阴影
        /// </summary>
        [Description("是否显示阴影"), Category("SunnyUI")]
        [DefaultValue(true)]
        public bool ShowShadow
        {
            get => _showShadow;
            set
            {
                _showShadow = value;
                Invalidate();
            }
        }

        private bool m_aeroEnabled;

        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                Win32.Dwm.DwmIsCompositionEnabled(ref enabled);
                return enabled == 1;
            }

            return false;
        }

        #endregion 边框阴影

        /// <summary>
        /// 是否重绘边框样式
        /// </summary>
        private bool _showRect = true;

        /// <summary>
        /// 是否显示边框
        /// </summary>
        [Description("是否显示边框"), Category("SunnyUI")]
        [DefaultValue(true)]
        public bool ShowRect
        {
            get => _showRect;
            set
            {
                _showRect = value;
                Invalidate();
            }
        }

        private void SetRadius()
        {
            if (DesignMode)
            {
                return;
            }

            if (WindowState == FormWindowState.Maximized || UIStyles.GlobalRectangle)
            {
                FormEx.SetFormRoundRectRegion(this, 0);
            }
            else
            {
                FormEx.SetFormRoundRectRegion(this, ShowRadius ? 5 : 0);
            }

            Invalidate();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                {
                    cp.ClassStyle |= Win32.User.CS_DROPSHADOW;
                }

                if (FormBorderStyle == FormBorderStyle.None)
                {
                    // 当边框样式为FormBorderStyle.None时
                    // 点击窗体任务栏图标，可以进行最小化
                    cp.Style = cp.Style | Win32.User.WS_MINIMIZEBOX;
                    return cp;
                }

                return base.CreateParams;
            }
        }

        [Description("显示边框可拖拽调整窗体大小"), Category("SunnyUI"), DefaultValue(false)]
        public bool Resizable
        {
            get => showDragStretch;
            set => showDragStretch = value;
        }

        [Browsable(false)]
        [Description("显示边框可拖拽调整窗体大小"), Category("SunnyUI"), DefaultValue(false)]
        public bool ShowDragStretch
        {
            get => showDragStretch;
            set
            {
                showDragStretch = value;
                ShowRect = value;
                if (value) ShowRadius = false;
                SetPadding();
            }
        }

        #region 拉拽调整窗体大小

        public event HotKeyEventHandler HotKeyEventHandler;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.User.WM_ERASEBKGND)
            {
                m.Result = IntPtr.Zero;
                return;
            }

            if (m.Msg == Win32.User.WM_HOTKEY)
            {
                int hotKeyId = (int)(m.WParam);
                if (hotKeys != null && hotKeys.ContainsKey(hotKeyId))
                {
                    HotKeyEventHandler?.Invoke(this, new HotKeyEventArgs(hotKeys[hotKeyId], DateTime.Now));
                }
            }

            if (m.Msg == Win32.User.WM_ACTIVATE)
            {
                if (WindowState != FormWindowState.Minimized && lastWindowState == FormWindowState.Minimized)
                {
                    DoWindowStateChanged(WindowState, lastWindowState);
                    lastWindowState = WindowState;
                }
            }

            if (m.Msg == Win32.User.WM_ACTIVATEAPP)
            {
                if (WindowState == FormWindowState.Minimized && lastWindowState != FormWindowState.Minimized)
                {
                    DoWindowStateChanged(WindowState, lastWindowState);
                    lastWindowState = FormWindowState.Minimized;
                }
            }

            base.WndProc(ref m);

            if (m.Msg == Win32.User.WM_NCHITTEST && ShowDragStretch && WindowState == FormWindowState.Normal)
            {
                //Point vPoint = new Point((int)m.LParam & 0xFFFF, (int)m.LParam >> 16 & 0xFFFF);
                Point vPoint = new Point(MousePosition.X, MousePosition.Y);//修正有分屏后，调整窗体大小时鼠标显示左右箭头问题
                vPoint = PointToClient(vPoint);
                int dragSize = 5;
                if (vPoint.X <= dragSize)
                {
                    if (vPoint.Y <= dragSize)
                        m.Result = (IntPtr)Win32.User.HTTOPLEFT;
                    else if (vPoint.Y >= ClientSize.Height - dragSize)
                        m.Result = (IntPtr)Win32.User.HTBOTTOMLEFT;
                    else
                        m.Result = (IntPtr)Win32.User.HTLEFT;
                }
                else if (vPoint.X >= ClientSize.Width - dragSize)
                {
                    if (vPoint.Y <= dragSize)
                        m.Result = (IntPtr)Win32.User.HTTOPRIGHT;
                    else if (vPoint.Y >= ClientSize.Height - dragSize)
                        m.Result = (IntPtr)Win32.User.HTBOTTOMRIGHT;
                    else
                        m.Result = (IntPtr)Win32.User.HTRIGHT;
                }
                else if (vPoint.Y <= dragSize)
                {
                    m.Result = (IntPtr)Win32.User.HTTOP;
                }
                else if (vPoint.Y >= ClientSize.Height - dragSize)
                {
                    m.Result = (IntPtr)Win32.User.HTBOTTOM;
                }
            }

            if (m.Msg == Win32.User.WM_NCPAINT && ShowShadow && m_aeroEnabled)
            {
                var v = 2;
                Win32.Dwm.DwmSetWindowAttribute(Handle, 2, ref v, 4);
                Win32.Dwm.MARGINS margins = new Win32.Dwm.MARGINS()
                {
                    bottomHeight = 0,
                    leftWidth = 0,
                    rightWidth = 0,
                    topHeight = 1
                };

                Win32.Dwm.DwmExtendFrameIntoClientArea(Handle, ref margins);
            }
        }

        #endregion 拉拽调整窗体大小
    }
}