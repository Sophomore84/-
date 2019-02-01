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
    public partial class frmEmptyRecord : FormBase
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
            this.btnPrintf = new System.Windows.Forms.Button();
            this.btnSaveData = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvselcet = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnselect = new System.Windows.Forms.Button();
            this.myGroupBox3 = new  风帆电池.MyGroupBox(this.components);
            this.rabtAll = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.rabtnOutLib = new System.Windows.Forms.RadioButton();
            this.rabtBack = new System.Windows.Forms.RadioButton();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.rabtnInlib = new System.Windows.Forms.RadioButton();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvselcet)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.myGroupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPrintf
            // 
            this.btnPrintf.Location = new System.Drawing.Point(30, 290);
            this.btnPrintf.Name = "btnPrintf";
            this.btnPrintf.Size = new System.Drawing.Size(90, 40);
            this.btnPrintf.TabIndex = 3;
            this.btnPrintf.Text = "打印";
            this.btnPrintf.UseVisualStyleBackColor = true;
            this.btnPrintf.Click += new System.EventHandler(this.btnPrintf_Click);
            // 
            // btnSaveData
            // 
            this.btnSaveData.Location = new System.Drawing.Point(153, 290);
            this.btnSaveData.Name = "btnSaveData";
            this.btnSaveData.Size = new System.Drawing.Size(90, 40);
            this.btnSaveData.TabIndex = 2;
            this.btnSaveData.Text = "导出数据";
            this.btnSaveData.UseVisualStyleBackColor = true;
            this.btnSaveData.Click += new System.EventHandler(this.btnSaveData_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvselcet);
            this.groupBox2.Location = new System.Drawing.Point(27, 350);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(955, 350);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // dgvselcet
            // 
            this.dgvselcet.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgvselcet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvselcet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvselcet.Location = new System.Drawing.Point(3, 21);
            this.dgvselcet.Name = "dgvselcet";
            this.dgvselcet.RowTemplate.Height = 23;
            this.dgvselcet.Size = new System.Drawing.Size(949, 326);
            this.dgvselcet.TabIndex = 0;
            this.dgvselcet.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvselcet_CellFormatting);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnselect);
            this.groupBox1.Controls.Add(this.myGroupBox3);
            this.groupBox1.Location = new System.Drawing.Point(27, 29);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(955, 255);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnselect
            // 
            this.btnselect.Location = new System.Drawing.Point(850, 211);
            this.btnselect.Name = "btnselect";
            this.btnselect.Size = new System.Drawing.Size(75, 31);
            this.btnselect.TabIndex = 20;
            this.btnselect.Text = "查询";
            this.btnselect.UseVisualStyleBackColor = true;
            this.btnselect.Click += new System.EventHandler(this.btnselect_Click);
            // 
            // myGroupBox3
            // 
            this.myGroupBox3.BorderColor = System.Drawing.Color.Black;
            this.myGroupBox3.BorderSize = 1F;
            this.myGroupBox3.Controls.Add(this.rabtAll);
            this.myGroupBox3.Controls.Add(this.label4);
            this.myGroupBox3.Controls.Add(this.rabtnOutLib);
            this.myGroupBox3.Controls.Add(this.rabtBack);
            this.myGroupBox3.Controls.Add(this.dateTimePicker2);
            this.myGroupBox3.Controls.Add(this.rabtnInlib);
            this.myGroupBox3.Controls.Add(this.dateTimePicker1);
            this.myGroupBox3.Controls.Add(this.label3);
            this.myGroupBox3.Location = new System.Drawing.Point(22, 23);
            this.myGroupBox3.Name = "myGroupBox3";
            this.myGroupBox3.Radius = 10;
            this.myGroupBox3.Size = new System.Drawing.Size(903, 155);
            this.myGroupBox3.TabIndex = 27;
            this.myGroupBox3.TabStop = false;
            this.myGroupBox3.Text = "卷筒";
            this.myGroupBox3.TitleFont = new System.Drawing.Font("宋体", 10F);
            // 
            // rabtAll
            // 
            this.rabtAll.AutoSize = true;
            this.rabtAll.Location = new System.Drawing.Point(712, 103);
            this.rabtAll.Name = "rabtAll";
            this.rabtAll.Size = new System.Drawing.Size(53, 23);
            this.rabtAll.TabIndex = 23;
            this.rabtAll.Text = "全部";
            this.rabtAll.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(321, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 19);
            this.label4.TabIndex = 6;
            this.label4.Text = "结束时间：";
            // 
            // rabtnOutLib
            // 
            this.rabtnOutLib.AutoSize = true;
            this.rabtnOutLib.Location = new System.Drawing.Point(236, 103);
            this.rabtnOutLib.Name = "rabtnOutLib";
            this.rabtnOutLib.Size = new System.Drawing.Size(53, 23);
            this.rabtnOutLib.TabIndex = 18;
            this.rabtnOutLib.Text = "取料";
            this.rabtnOutLib.UseVisualStyleBackColor = true;
            // 
            // rabtBack
            // 
            this.rabtBack.AutoSize = true;
            this.rabtBack.Location = new System.Drawing.Point(495, 103);
            this.rabtBack.Name = "rabtBack";
            this.rabtBack.Size = new System.Drawing.Size(53, 23);
            this.rabtBack.TabIndex = 22;
            this.rabtBack.Text = "退库";
            this.rabtBack.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker2.Location = new System.Drawing.Point(401, 21);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 25);
            this.dateTimePicker2.TabIndex = 13;
            this.dateTimePicker2.Value = new System.DateTime(2018, 8, 9, 23, 59, 0, 0);
            // 
            // rabtnInlib
            // 
            this.rabtnInlib.AutoSize = true;
            this.rabtnInlib.Checked = true;
            this.rabtnInlib.Location = new System.Drawing.Point(11, 103);
            this.rabtnInlib.Name = "rabtnInlib";
            this.rabtnInlib.Size = new System.Drawing.Size(53, 23);
            this.rabtnInlib.TabIndex = 17;
            this.rabtnInlib.TabStop = true;
            this.rabtnInlib.Text = "入库";
            this.rabtnInlib.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(83, 21);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 25);
            this.dateTimePicker1.TabIndex = 12;
            this.dateTimePicker1.Value = new System.DateTime(2018, 8, 1, 0, 0, 0, 0);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 19);
            this.label3.TabIndex = 4;
            this.label3.Text = "开始时间：";
            // 
            // frmEmptyRecord
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1088, 752);
            this.Controls.Add(this.btnPrintf);
            this.Controls.Add(this.btnSaveData);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmEmptyRecord";
            this.Text = "出入库记录";
            this.Load += new System.EventHandler(this.frmUserSelect_Load);
            this.SizeChanged += new System.EventHandler(this.frmUserSelect_SizeChanged);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvselcet)).EndInit();
            this.groupBox1.ResumeLayout(false);
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

        private GroupBox groupBox1;
        private DateTimePicker dateTimePicker2;
        private DateTimePicker dateTimePicker1;
        private Label label4;
        private Label label3;
        private RadioButton rabtnOutLib;
        private RadioButton rabtnInlib;
        private GroupBox groupBox2;
        private DataGridView dgvselcet;
        private Button btnselect;
        private Button btnSaveData;
        private Button btnPrintf;
        private RadioButton rabtBack;
        private IContainer components;
        private MyGroupBox myGroupBox3;
        private RadioButton rabtAll;
    }
}