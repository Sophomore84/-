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
    public partial class frmAutoAlter:FormBase
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
                    cp.ExStyle |= (int)WindowStyle.WS_CLIPCHILDREN;  //防止因窗体控件太多出现闪烁
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
            this.button1 = new System.Windows.Forms.Button();
            this.myGroupBox1 = new 风帆电池.MyGroupBox(this.components);
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.myGroupBox2 = new 风帆电池.MyGroupBox(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.myGroupBox3 = new 风帆电池.MyGroupBox(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.myGroupBox1.SuspendLayout();
            this.myGroupBox2.SuspendLayout();
            this.myGroupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(523, 38);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(167, 57);
            this.button1.TabIndex = 1;
            this.button1.Text = "修改为空位";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // myGroupBox1
            // 
            this.myGroupBox1.BorderColor = System.Drawing.Color.Black;
            this.myGroupBox1.BorderSize = 1F;
            this.myGroupBox1.Controls.Add(this.textBox3);
            this.myGroupBox1.Controls.Add(this.label8);
            this.myGroupBox1.Controls.Add(this.button1);
            this.myGroupBox1.Location = new System.Drawing.Point(29, 48);
            this.myGroupBox1.Name = "myGroupBox1";
            this.myGroupBox1.Radius = 10;
            this.myGroupBox1.Size = new System.Drawing.Size(754, 105);
            this.myGroupBox1.TabIndex = 2;
            this.myGroupBox1.TabStop = false;
            this.myGroupBox1.Text = "自动修改为空位:";
            this.myGroupBox1.TitleFont = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // textBox3
            // 
            this.textBox3.Enabled = false;
            this.textBox3.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox3.Location = new System.Drawing.Point(168, 50);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(205, 34);
            this.textBox3.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(17, 53);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(145, 26);
            this.label8.TabIndex = 5;
            this.label8.Text = "当前库存位置：";
            // 
            // myGroupBox2
            // 
            this.myGroupBox2.BorderColor = System.Drawing.Color.Black;
            this.myGroupBox2.BorderSize = 1F;
            this.myGroupBox2.Controls.Add(this.textBox1);
            this.myGroupBox2.Controls.Add(this.label2);
            this.myGroupBox2.Controls.Add(this.button2);
            this.myGroupBox2.Location = new System.Drawing.Point(29, 169);
            this.myGroupBox2.Name = "myGroupBox2";
            this.myGroupBox2.Radius = 10;
            this.myGroupBox2.Size = new System.Drawing.Size(754, 123);
            this.myGroupBox2.TabIndex = 3;
            this.myGroupBox2.TabStop = false;
            this.myGroupBox2.Text = "自动修改为空卷筒:";
            this.myGroupBox2.TitleFont = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.Location = new System.Drawing.Point(168, 55);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(205, 34);
            this.textBox1.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(17, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 26);
            this.label2.TabIndex = 3;
            this.label2.Text = "当前卷筒编号：";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(523, 38);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(167, 57);
            this.button2.TabIndex = 1;
            this.button2.Text = "修改为空卷";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // myGroupBox3
            // 
            this.myGroupBox3.BorderColor = System.Drawing.Color.Black;
            this.myGroupBox3.BorderSize = 1F;
            this.myGroupBox3.Controls.Add(this.label7);
            this.myGroupBox3.Controls.Add(this.label6);
            this.myGroupBox3.Controls.Add(this.label5);
            this.myGroupBox3.Controls.Add(this.label4);
            this.myGroupBox3.Controls.Add(this.label3);
            this.myGroupBox3.Controls.Add(this.textBox2);
            this.myGroupBox3.Controls.Add(this.label1);
            this.myGroupBox3.Controls.Add(this.button3);
            this.myGroupBox3.Location = new System.Drawing.Point(29, 312);
            this.myGroupBox3.Name = "myGroupBox3";
            this.myGroupBox3.Radius = 10;
            this.myGroupBox3.Size = new System.Drawing.Size(754, 306);
            this.myGroupBox3.TabIndex = 4;
            this.myGroupBox3.TabStop = false;
            this.myGroupBox3.Text = "自动修改满卷:";
            this.myGroupBox3.TitleFont = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(17, 258);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(354, 26);
            this.label7.TabIndex = 9;
            this.label7.Text = "满卷更为具体的修改请使用手动修改！！";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(203, 209);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(297, 26);
            this.label6.TabIndex = 8;
            this.label6.Text = "生产批号根据当前日期随机生成！";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(402, 165);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(145, 26);
            this.label5.TabIndex = 7;
            this.label5.Text = "检测结果：合格";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(198, 165);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(175, 26);
            this.label4.TabIndex = 6;
            this.label4.Text = "01 规格型号的满卷";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(17, 165);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(170, 26);
            this.label3.TabIndex = 5;
            this.label3.Text = "默认自动修改为 ：";
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox2.Location = new System.Drawing.Point(168, 55);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(205, 34);
            this.textBox2.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(17, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 26);
            this.label1.TabIndex = 3;
            this.label1.Text = "当前卷筒编号：";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.Location = new System.Drawing.Point(523, 38);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(167, 57);
            this.button3.TabIndex = 1;
            this.button3.Text = "修改为满卷";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // frmAutoAlter
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(866, 630);
            this.Controls.Add(this.myGroupBox3);
            this.Controls.Add(this.myGroupBox2);
            this.Controls.Add(this.myGroupBox1);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmAutoAlter";
            this.Text = "风帆电池项目";
            this.Activated += new System.EventHandler(this.frmAutoAlter_Activated);
            this.Load += new System.EventHandler(this.frmAutoAlter_Load);
            this.myGroupBox1.ResumeLayout(false);
            this.myGroupBox1.PerformLayout();
            this.myGroupBox2.ResumeLayout(false);
            this.myGroupBox2.PerformLayout();
            this.myGroupBox3.ResumeLayout(false);
            this.myGroupBox3.PerformLayout();
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
        private IContainer components;
        private Button button1;
        private MyGroupBox myGroupBox1;
        private MyGroupBox myGroupBox2;
        private TextBox textBox1;
        private Label label2;
        private Button button2;
        private MyGroupBox myGroupBox3;
        private Label label5;
        private Label label4;
        private Label label3;
        private TextBox textBox2;
        private Label label1;
        private Button button3;
        private Label label7;
        private Label label6;
        private TextBox textBox3;
        private Label label8;
    }
}