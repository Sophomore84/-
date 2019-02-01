using 风帆电池;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
namespace  风帆电池
{
    public partial class frmChangePwr : FormBase
    {
        #region Field

        //窗体圆角矩形半径
        private int _radius = 5;

        //是否允许窗体改变大小
        private bool _canResize = true;

        private Image _fringe = Image.FromFile(@".\Res\fringe_bkg.png");
        private Image _formBkg = Image.FromFile(@".\Res\FormBkg\bkg_flower.jpg");

        //系统按钮管理器
        private SystemButtonManager _systemButtonManager;

        #endregion

        #region Constructor

   

        #endregion

        #region Properties

        [DefaultValue(typeof(byte), "5")]
        public int Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                if (_radius != value)
                {
                    _radius = value;
                    this.Invalidate();
                }
            }
        }

        public bool CanResize
        {
            get
            {
                return _canResize;
            }
            set
            {
                if (_canResize != value)
                {
                    _canResize = value;
                }
            }
        }

        public override Image BackgroundImage
        {
            get
            {
                return _formBkg;
            }
            set
            {
                if (_formBkg != value)
                {
                    _formBkg = value;
                    Invalidate();
                }
            }
        }

        internal Rectangle IconRect
        {
            get
            {
                if (base.ShowIcon && base.Icon != null)
                {
                    return new Rectangle(8, 6, SystemInformation.SmallIconSize.Width, SystemInformation.SmallIconSize.Width);
                }
                return Rectangle.Empty;
            }
        }

        internal Rectangle TextRect
        {
            get
            {
                if (base.Text.Length != 0)
                {
                    return new Rectangle(IconRect.Right + 2, 4, Width - (8 + IconRect.Width + 2), Font.Height);
                }
                return Rectangle.Empty;
            }
        }

        internal SystemButtonManager SystemButtonManager
        {
            get
            {
                if (_systemButtonManager == null)
                {
                    _systemButtonManager = new SystemButtonManager(this);
                }
                return _systemButtonManager;
            }
        }

        #endregion

        #region Overrides

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!DesignMode)
                {
                    if (MaximizeBox) { cp.Style |= (int)WindowStyle.WS_MAXIMIZEBOX; }
                    if (MinimizeBox) { cp.Style |= (int)WindowStyle.WS_MINIMIZEBOX; }
                    //cp.ExStyle |= (int)WindowStyle.WS_CLIPCHILDREN;  //防止因窗体控件太多出现闪烁
                    cp.ClassStyle |= (int)ClassStyle.CS_DropSHADOW;  //实现窗体边框阴影效果
                }
                return cp;
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            RenderHelper.SetFormRoundRectRgn(this, Radius);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RenderHelper.SetFormRoundRectRgn(this, Radius);
            UpdateSystemButtonRect();
            UpdateMaxButton();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Win32.WM_ERASEBKGND:
                    m.Result = IntPtr.Zero;
                    break;
                case Win32.WM_NCHITTEST:
                    WmNcHitTest(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            SystemButtonManager.ProcessMouseOperate(e.Location, MouseOperate.Move);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                SystemButtonManager.ProcessMouseOperate(e.Location, MouseOperate.Down);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                SystemButtonManager.ProcessMouseOperate(e.Location, MouseOperate.Up);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            SystemButtonManager.ProcessMouseOperate(Point.Empty, MouseOperate.Leave);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //draw BackgroundImage
            e.Graphics.DrawImage(_formBkg, ClientRectangle, new Rectangle(0, 0, _formBkg.Width, _formBkg.Height), GraphicsUnit.Pixel);

            //draw form main part
            RenderHelper.DrawFromAlphaMainPart(this, e.Graphics);

            //draw system buttons
            SystemButtonManager.DrawSystemButtons(e.Graphics);

            //draw fringe
            RenderHelper.DrawFormFringe(this, e.Graphics, _fringe, Radius);

            //draw icon
            if (Icon != null && ShowIcon)
            {
                e.Graphics.DrawIcon(Icon, IconRect);
            }

            //draw text
            if (Text.Length != 0)
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    Text, Font,
                    TextRect,
                    Color.White,
                    TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
            }



        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_systemButtonManager != null)
                {
                    _systemButtonManager.Dispose();
                    _systemButtonManager = null;
                    _formBkg.Dispose();
                    _formBkg = null;
                    _fringe.Dispose();
                    _fringe = null;
                }
            }
        }

        #endregion

        #region Private Methods

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.myGroupBox1 = new 风帆电池.MyGroupBox(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtNewPwr2 = new System.Windows.Forms.TextBox();
            this.txtNewPwr1 = new System.Windows.Forms.TextBox();
            this.txtOldpwr = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.myGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // myGroupBox1
            // 
            this.myGroupBox1.BorderColor = System.Drawing.Color.Black;
            this.myGroupBox1.BorderSize = 1F;
            this.myGroupBox1.Controls.Add(this.button2);
            this.myGroupBox1.Controls.Add(this.button1);
            this.myGroupBox1.Controls.Add(this.txtNewPwr2);
            this.myGroupBox1.Controls.Add(this.txtNewPwr1);
            this.myGroupBox1.Controls.Add(this.txtOldpwr);
            this.myGroupBox1.Controls.Add(this.label3);
            this.myGroupBox1.Controls.Add(this.label2);
            this.myGroupBox1.Controls.Add(this.label1);
            this.myGroupBox1.Location = new System.Drawing.Point(33, 26);
            this.myGroupBox1.Name = "myGroupBox1";
            this.myGroupBox1.Radius = 10;
            this.myGroupBox1.Size = new System.Drawing.Size(536, 342);
            this.myGroupBox1.TabIndex = 0;
            this.myGroupBox1.TabStop = false;
            this.myGroupBox1.TitleFont = new System.Drawing.Font("宋体", 10F);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(284, 238);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 33);
            this.button2.TabIndex = 15;
            this.button2.Text = "清空";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(105, 238);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 33);
            this.button1.TabIndex = 14;
            this.button1.Text = "修改";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtNewPwr2
            // 
            this.txtNewPwr2.Location = new System.Drawing.Point(154, 166);
            this.txtNewPwr2.Name = "txtNewPwr2";
            this.txtNewPwr2.Size = new System.Drawing.Size(230, 25);
            this.txtNewPwr2.TabIndex = 13;
            // 
            // txtNewPwr1
            // 
            this.txtNewPwr1.Location = new System.Drawing.Point(154, 110);
            this.txtNewPwr1.Name = "txtNewPwr1";
            this.txtNewPwr1.Size = new System.Drawing.Size(230, 25);
            this.txtNewPwr1.TabIndex = 12;
            // 
            // txtOldpwr
            // 
            this.txtOldpwr.Location = new System.Drawing.Point(154, 59);
            this.txtOldpwr.Name = "txtOldpwr";
            this.txtOldpwr.Size = new System.Drawing.Size(230, 25);
            this.txtOldpwr.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(70, 169);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 19);
            this.label3.TabIndex = 10;
            this.label3.Text = "确认新密码：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(70, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 19);
            this.label2.TabIndex = 9;
            this.label2.Text = "输入新密码：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(94, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 19);
            this.label1.TabIndex = 8;
            this.label1.Text = "旧密码：";
            // 
            // frmChangePwr
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(621, 425);
            this.Controls.Add(this.myGroupBox1);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmChangePwr";
            this.Text = "修改密码";
            this.myGroupBox1.ResumeLayout(false);
            this.myGroupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        private void FormExIni()
        {
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            SetStyles();
        }

        private void SetStyles()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
        }

        private void WmNcHitTest(ref Message m)  //调整窗体大小
        {
            int wparam = m.LParam.ToInt32();
            Point mouseLocation = new Point(RenderHelper.LOWORD(wparam), RenderHelper.HIWORD(wparam));
            mouseLocation = PointToClient(mouseLocation);

            if (WindowState != FormWindowState.Maximized)
            {
                if (CanResize == true)
                {
                    if (mouseLocation.X < 5 && mouseLocation.Y < 5)
                    {
                        m.Result = new IntPtr(Win32.HTTOPLEFT);
                        return;
                    }

                    if (mouseLocation.X > Width - 5 && mouseLocation.Y < 5)
                    {
                        m.Result = new IntPtr(Win32.HTTOPRIGHT);
                        return;
                    }

                    if (mouseLocation.X < 5 && mouseLocation.Y > Height - 5)
                    {
                        m.Result = new IntPtr(Win32.HTBOTTOMLEFT);
                        return;
                    }

                    if (mouseLocation.X > Width - 5 && mouseLocation.Y > Height - 5)
                    {
                        m.Result = new IntPtr(Win32.HTBOTTOMRIGHT);
                        return;
                    }

                    if (mouseLocation.Y < 3)
                    {
                        m.Result = new IntPtr(Win32.HTTOP);
                        return;
                    }

                    if (mouseLocation.Y > Height - 3)
                    {
                        m.Result = new IntPtr(Win32.HTBOTTOM);
                        return;
                    }

                    if (mouseLocation.X < 3)
                    {
                        m.Result = new IntPtr(Win32.HTLEFT);
                        return;
                    }

                    if (mouseLocation.X > Width - 3)
                    {
                        m.Result = new IntPtr(Win32.HTRIGHT);
                        return;
                    }
                }
            }
            m.Result = new IntPtr(Win32.HTCLIENT);
        }

        private void UpdateMaxButton()  //根据窗体的状态更换最大(还原)系统按钮
        {
            bool isMax = WindowState == FormWindowState.Maximized;
            if (isMax)
            {
                SystemButtonManager.SystemButtonArray[1].NormalImg = Image.FromFile(@".\Res\SystemButton\restore_normal.png");
                SystemButtonManager.SystemButtonArray[1].HighLightImg = Image.FromFile(@".\Res\SystemButton\restore_highlight.png");
                SystemButtonManager.SystemButtonArray[1].PressedImg = Image.FromFile(@".\Res\SystemButton\restore_press.png");
                SystemButtonManager.SystemButtonArray[1].ToolTip = "还原";
            }
            else
            {
                SystemButtonManager.SystemButtonArray[1].NormalImg = Image.FromFile(@".\Res\SystemButton\max_normal.png");
                SystemButtonManager.SystemButtonArray[1].HighLightImg = Image.FromFile(@".\Res\SystemButton\max_highlight.png");
                SystemButtonManager.SystemButtonArray[1].PressedImg = Image.FromFile(@".\Res\SystemButton\max_press.png");
                SystemButtonManager.SystemButtonArray[1].ToolTip = "最大化";
            }
        }

        protected void UpdateSystemButtonRect()
        {
            bool isShowMaxButton = MaximizeBox;
            bool isShowMinButton = MinimizeBox;
            Rectangle closeRect = new Rectangle(
                    Width - SystemButtonManager.SystemButtonArray[0].NormalImg.Width,
                    -1,
                    SystemButtonManager.SystemButtonArray[0].NormalImg.Width,
                    SystemButtonManager.SystemButtonArray[0].NormalImg.Height);

            //update close button location rect.
            SystemButtonManager.SystemButtonArray[0].LocationRect = closeRect;

            //Max
            if (isShowMaxButton)
            {
                SystemButtonManager.SystemButtonArray[1].LocationRect = new Rectangle(
                    closeRect.X - SystemButtonManager.SystemButtonArray[1].NormalImg.Width,
                    -1,
                    SystemButtonManager.SystemButtonArray[1].NormalImg.Width,
                    SystemButtonManager.SystemButtonArray[1].NormalImg.Height);
            }
            else
            {
                SystemButtonManager.SystemButtonArray[1].LocationRect = Rectangle.Empty;
            }

            //Min
            if (!isShowMinButton)
            {
                SystemButtonManager.SystemButtonArray[2].LocationRect = Rectangle.Empty;
                return;
            }
            if (isShowMaxButton)
            {
                SystemButtonManager.SystemButtonArray[2].LocationRect = new Rectangle(
                    SystemButtonManager.SystemButtonArray[1].LocationRect.X - SystemButtonManager.SystemButtonArray[2].NormalImg.Width,
                    -1,
                    SystemButtonManager.SystemButtonArray[2].NormalImg.Width,
                    SystemButtonManager.SystemButtonArray[2].NormalImg.Height);
            }
            else
            {
                SystemButtonManager.SystemButtonArray[2].LocationRect = new Rectangle(
                   closeRect.X - SystemButtonManager.SystemButtonArray[2].NormalImg.Width,
                   -1,
                   SystemButtonManager.SystemButtonArray[2].NormalImg.Width,
                   SystemButtonManager.SystemButtonArray[2].NormalImg.Height);
            }
        }


        #endregion

        private MyGroupBox myGroupBox1;
        private IContainer components;
        private Button button2;
        private Button button1;
        private TextBox txtNewPwr2;
        private TextBox txtNewPwr1;
        private TextBox txtOldpwr;
        private Label label3;
        private Label label2;
        private Label label1;
    }
}