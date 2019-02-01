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
    public partial class frmAddEmpty : FormBase
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
            this.myGroupBox1 = new  风帆电池.MyGroupBox(this.components);
            this.cmbEmptyInPosition = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.cmbEmptyPosition = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.btnDeleteEmpty = new System.Windows.Forms.Button();
            this.btnAddEmpty = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.cmbNewEmpty = new System.Windows.Forms.TextBox();
            this.cmbexistEmpty = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.myGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // myGroupBox1
            // 
            this.myGroupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.myGroupBox1.BorderColor = System.Drawing.Color.Black;
            this.myGroupBox1.BorderSize = 2F;
            this.myGroupBox1.Controls.Add(this.cmbEmptyInPosition);
            this.myGroupBox1.Controls.Add(this.label20);
            this.myGroupBox1.Controls.Add(this.cmbEmptyPosition);
            this.myGroupBox1.Controls.Add(this.label19);
            this.myGroupBox1.Controls.Add(this.btnDeleteEmpty);
            this.myGroupBox1.Controls.Add(this.btnAddEmpty);
            this.myGroupBox1.Controls.Add(this.label15);
            this.myGroupBox1.Controls.Add(this.cmbNewEmpty);
            this.myGroupBox1.Controls.Add(this.cmbexistEmpty);
            this.myGroupBox1.Controls.Add(this.label16);
            this.myGroupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.myGroupBox1.Location = new System.Drawing.Point(36, 51);
            this.myGroupBox1.Name = "myGroupBox1";
            this.myGroupBox1.Radius = 10;
            this.myGroupBox1.Size = new System.Drawing.Size(940, 232);
            this.myGroupBox1.TabIndex = 25;
            this.myGroupBox1.TabStop = false;
            this.myGroupBox1.Text = "卷筒增添，删除";
            this.myGroupBox1.TitleFont = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // cmbEmptyInPosition
            // 
            this.cmbEmptyInPosition.FormattingEnabled = true;
            this.cmbEmptyInPosition.IntegralHeight = false;
            this.cmbEmptyInPosition.Location = new System.Drawing.Point(570, 52);
            this.cmbEmptyInPosition.Name = "cmbEmptyInPosition";
            this.cmbEmptyInPosition.Size = new System.Drawing.Size(158, 27);
            this.cmbEmptyInPosition.TabIndex = 5;
            this.cmbEmptyInPosition.SelectedIndexChanged += new System.EventHandler(this.cmbEmptyInPosition_SelectedIndexChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label20.Location = new System.Drawing.Point(419, 53);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(145, 26);
            this.label20.TabIndex = 47;
            this.label20.Text = "卷筒所在位置：";
            // 
            // cmbEmptyPosition
            // 
            this.cmbEmptyPosition.FormattingEnabled = true;
            this.cmbEmptyPosition.IntegralHeight = false;
            this.cmbEmptyPosition.Location = new System.Drawing.Point(570, 122);
            this.cmbEmptyPosition.Name = "cmbEmptyPosition";
            this.cmbEmptyPosition.Size = new System.Drawing.Size(158, 27);
            this.cmbEmptyPosition.TabIndex = 7;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label19.Location = new System.Drawing.Point(419, 120);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(145, 26);
            this.label19.TabIndex = 45;
            this.label19.Text = "卷筒入库位置：";
            // 
            // btnDeleteEmpty
            // 
            this.btnDeleteEmpty.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDeleteEmpty.Location = new System.Drawing.Point(820, 39);
            this.btnDeleteEmpty.Name = "btnDeleteEmpty";
            this.btnDeleteEmpty.Size = new System.Drawing.Size(109, 38);
            this.btnDeleteEmpty.TabIndex = 9;
            this.btnDeleteEmpty.Text = "删除卷筒";
            this.btnDeleteEmpty.UseVisualStyleBackColor = true;
            this.btnDeleteEmpty.Click += new System.EventHandler(this.btnDeleteEmpty_Click);
            // 
            // btnAddEmpty
            // 
            this.btnAddEmpty.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAddEmpty.Location = new System.Drawing.Point(820, 111);
            this.btnAddEmpty.Name = "btnAddEmpty";
            this.btnAddEmpty.Size = new System.Drawing.Size(112, 38);
            this.btnAddEmpty.TabIndex = 8;
            this.btnAddEmpty.Text = "新增卷筒";
            this.btnAddEmpty.UseVisualStyleBackColor = true;
            this.btnAddEmpty.Click += new System.EventHandler(this.btnAddEmpty_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.Location = new System.Drawing.Point(17, 126);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(145, 26);
            this.label15.TabIndex = 41;
            this.label15.Text = "新增卷筒编号：";
            // 
            // cmbNewEmpty
            // 
            this.cmbNewEmpty.Location = new System.Drawing.Point(168, 126);
            this.cmbNewEmpty.MaxLength = 20;
            this.cmbNewEmpty.Name = "cmbNewEmpty";
            this.cmbNewEmpty.Size = new System.Drawing.Size(169, 25);
            this.cmbNewEmpty.TabIndex = 0;
            // 
            // cmbexistEmpty
            // 
            this.cmbexistEmpty.FormattingEnabled = true;
            this.cmbexistEmpty.IntegralHeight = false;
            this.cmbexistEmpty.Location = new System.Drawing.Point(168, 53);
            this.cmbexistEmpty.Name = "cmbexistEmpty";
            this.cmbexistEmpty.Size = new System.Drawing.Size(169, 27);
            this.cmbexistEmpty.TabIndex = 4;
            this.cmbexistEmpty.SelectedIndexChanged += new System.EventHandler(this.cmbexistEmpty_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.Location = new System.Drawing.Point(17, 51);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(145, 26);
            this.label16.TabIndex = 39;
            this.label16.Text = "已有卷筒编号：";
            // 
            // frmAddEmpty
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1017, 344);
            this.Controls.Add(this.myGroupBox1);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmAddEmpty";
            this.Text = "卷筒增删";
            this.Load += new System.EventHandler(this.frmAddEmpty_Load);
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
        private ComboBox cmbEmptyInPosition;
        private Label label20;
        private ComboBox cmbEmptyPosition;
        private Label label19;
        private Button btnDeleteEmpty;
        private Button btnAddEmpty;
        private Label label15;
        private TextBox cmbNewEmpty;
        private ComboBox cmbexistEmpty;
        private Label label16;
    }
}