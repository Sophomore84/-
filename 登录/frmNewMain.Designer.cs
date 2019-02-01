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
namespace 风帆电池
{
    public partial class frmNewMain : FormBase
    {
        #region Field

        //窗体圆角矩形半径
        private int _radius = 5;

        //是否允许窗体改变大小
        private bool _canResize = true;

        private Image _fringe  = Image.FromFile(@".\Res\fringe_bkg.png");
        private Image _formBkg = Image.FromFile(@".\Res\FormBkg\bkg_main.jpg");
        

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

            if (m.Msg == 0x0014) // 禁掉清除背景消息
                return;
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNewMain));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.myIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.myrightMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblState = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTimer = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblEdit = new System.Windows.Forms.ToolStripStatusLabel();
            this.myGroupBox1 = new 风帆电池.MyGroupBox(this.components);
            this.label11 = new System.Windows.Forms.Label();
            this.txtGWposition = new System.Windows.Forms.TextBox();
            this.txtSteps = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCheckState = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSelctEmptyNum = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtGoodsCode = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtGoodsState = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtAction = new System.Windows.Forms.TextBox();
            this.txtlibActionState = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txtPosition = new System.Windows.Forms.TextBox();
            this.txtErrorPut = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtActionState = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.myMenu1 = new 风帆电池.MyMenu(this.components);
            this.系统ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.管理员权限ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.切换为用户模式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.修改密码ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.查询ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.库存记录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.出入库记录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.空筒出入库记录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.添加ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.手动增加卷筒ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.通讯ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.系列ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.网口ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.日志ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.查看ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ckbordervisible = new System.Windows.Forms.CheckBox();
            this.mygpbExchange = new 风帆电池.MyGroupBox(this.components);
            this.panelorder = new System.Windows.Forms.Panel();
            this.mygpborderhasbuff = new 风帆电池.MyGroupBox(this.components);
            this.listBoxorderBuff = new System.Windows.Forms.ListBox();
            this.mygpbActionQue = new 风帆电池.MyGroupBox(this.components);
            this.listBoxAction = new System.Windows.Forms.ListBox();
            this.mygpbResults = new 风帆电池.MyGroupBox(this.components);
            this.dgvPosition = new System.Windows.Forms.DataGridView();
            this.mygpbOrderQue = new 风帆电池.MyGroupBox(this.components);
            this.listBoxOrder = new System.Windows.Forms.ListBox();
            this.myGroupBox6 = new 风帆电池.MyGroupBox(this.components);
            this.listBoxShowCurrentOrder = new System.Windows.Forms.ListBox();
            this.cMenuOrderRight = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.返回ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.myrightMenu.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.myGroupBox1.SuspendLayout();
            this.myMenu1.SuspendLayout();
            this.mygpbExchange.SuspendLayout();
            this.panelorder.SuspendLayout();
            this.mygpborderhasbuff.SuspendLayout();
            this.mygpbActionQue.SuspendLayout();
            this.mygpbResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPosition)).BeginInit();
            this.mygpbOrderQue.SuspendLayout();
            this.myGroupBox6.SuspendLayout();
            this.cMenuOrderRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // myIcon
            // 
            this.myIcon.ContextMenuStrip = this.myrightMenu;
            this.myIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("myIcon.Icon")));
            this.myIcon.Text = "风帆电池管理系统\r\n\r\n";
            this.myIcon.Visible = true;
            this.myIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.myIcon_MouseClick);
            // 
            // myrightMenu
            // 
            this.myrightMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.退出ToolStripMenuItem});
            this.myrightMenu.Name = "myMenu";
            this.myrightMenu.Size = new System.Drawing.Size(119, 34);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(118, 30);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblState,
            this.lblTimer,
            this.lblEdit});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 720);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1210, 22);
            this.statusStrip1.TabIndex = 40;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblState
            // 
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(92, 17);
            this.lblState.Text = "通信未建立连接";
            // 
            // lblTimer
            // 
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(41, 17);
            this.lblTimer.Text = "Timer";
            // 
            // lblEdit
            // 
            this.lblEdit.Name = "lblEdit";
            this.lblEdit.Size = new System.Drawing.Size(44, 17);
            this.lblEdit.Text = "lblEdit";
            // 
            // myGroupBox1
            // 
            this.myGroupBox1.BorderColor = System.Drawing.Color.Red;
            this.myGroupBox1.BorderSize = 1.5F;
            this.myGroupBox1.Controls.Add(this.label11);
            this.myGroupBox1.Controls.Add(this.txtGWposition);
            this.myGroupBox1.Controls.Add(this.txtSteps);
            this.myGroupBox1.Controls.Add(this.label3);
            this.myGroupBox1.Controls.Add(this.label2);
            this.myGroupBox1.Controls.Add(this.txtCheckState);
            this.myGroupBox1.Controls.Add(this.label1);
            this.myGroupBox1.Controls.Add(this.txtSelctEmptyNum);
            this.myGroupBox1.Controls.Add(this.label7);
            this.myGroupBox1.Controls.Add(this.txtGoodsCode);
            this.myGroupBox1.Controls.Add(this.label8);
            this.myGroupBox1.Controls.Add(this.txtGoodsState);
            this.myGroupBox1.Controls.Add(this.label9);
            this.myGroupBox1.Controls.Add(this.txtAction);
            this.myGroupBox1.Controls.Add(this.txtlibActionState);
            this.myGroupBox1.Controls.Add(this.label10);
            this.myGroupBox1.Controls.Add(this.label15);
            this.myGroupBox1.Controls.Add(this.txtPosition);
            this.myGroupBox1.Controls.Add(this.txtErrorPut);
            this.myGroupBox1.Controls.Add(this.label14);
            this.myGroupBox1.Controls.Add(this.txtActionState);
            this.myGroupBox1.Controls.Add(this.label12);
            this.myGroupBox1.Location = new System.Drawing.Point(12, 529);
            this.myGroupBox1.Name = "myGroupBox1";
            this.myGroupBox1.Radius = 20;
            this.myGroupBox1.Size = new System.Drawing.Size(1163, 178);
            this.myGroupBox1.TabIndex = 78;
            this.myGroupBox1.TabStop = false;
            this.myGroupBox1.Text = "执行状态:";
            this.myGroupBox1.TitleColor = System.Drawing.Color.Blue;
            this.myGroupBox1.TitleFont = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(26, 76);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(90, 22);
            this.label11.TabIndex = 70;
            this.label11.Text = "生产工位：";
            // 
            // txtGWposition
            // 
            this.txtGWposition.Location = new System.Drawing.Point(122, 75);
            this.txtGWposition.Name = "txtGWposition";
            this.txtGWposition.ReadOnly = true;
            this.txtGWposition.Size = new System.Drawing.Size(136, 25);
            this.txtGWposition.TabIndex = 3;
            // 
            // txtSteps
            // 
            this.txtSteps.Location = new System.Drawing.Point(122, 25);
            this.txtSteps.Name = "txtSteps";
            this.txtSteps.ReadOnly = true;
            this.txtSteps.Size = new System.Drawing.Size(136, 25);
            this.txtSteps.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(26, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 22);
            this.label3.TabIndex = 82;
            this.label3.Text = "动作步奏：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(846, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 22);
            this.label2.TabIndex = 80;
            this.label2.Text = "检测结果：";
            // 
            // txtCheckState
            // 
            this.txtCheckState.Location = new System.Drawing.Point(962, 130);
            this.txtCheckState.Name = "txtCheckState";
            this.txtCheckState.ReadOnly = true;
            this.txtCheckState.Size = new System.Drawing.Size(136, 25);
            this.txtCheckState.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(26, 133);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 22);
            this.label1.TabIndex = 78;
            this.label1.Text = "卷筒编号：";
            // 
            // txtSelctEmptyNum
            // 
            this.txtSelctEmptyNum.Location = new System.Drawing.Point(122, 132);
            this.txtSelctEmptyNum.Name = "txtSelctEmptyNum";
            this.txtSelctEmptyNum.ReadOnly = true;
            this.txtSelctEmptyNum.Size = new System.Drawing.Size(136, 25);
            this.txtSelctEmptyNum.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(312, 133);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 22);
            this.label7.TabIndex = 62;
            this.label7.Text = "规格型号：";
            // 
            // txtGoodsCode
            // 
            this.txtGoodsCode.Location = new System.Drawing.Point(408, 133);
            this.txtGoodsCode.Name = "txtGoodsCode";
            this.txtGoodsCode.ReadOnly = true;
            this.txtGoodsCode.Size = new System.Drawing.Size(136, 25);
            this.txtGoodsCode.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(581, 133);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 22);
            this.label8.TabIndex = 64;
            this.label8.Text = "物料状态：";
            // 
            // txtGoodsState
            // 
            this.txtGoodsState.Location = new System.Drawing.Point(676, 130);
            this.txtGoodsState.Name = "txtGoodsState";
            this.txtGoodsState.ReadOnly = true;
            this.txtGoodsState.Size = new System.Drawing.Size(136, 25);
            this.txtGoodsState.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(580, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 22);
            this.label9.TabIndex = 66;
            this.label9.Text = "执行操作：";
            // 
            // txtAction
            // 
            this.txtAction.Location = new System.Drawing.Point(676, 21);
            this.txtAction.Name = "txtAction";
            this.txtAction.ReadOnly = true;
            this.txtAction.Size = new System.Drawing.Size(136, 25);
            this.txtAction.TabIndex = 2;
            // 
            // txtlibActionState
            // 
            this.txtlibActionState.BackColor = System.Drawing.SystemColors.Control;
            this.txtlibActionState.Location = new System.Drawing.Point(122, 195);
            this.txtlibActionState.Name = "txtlibActionState";
            this.txtlibActionState.ReadOnly = true;
            this.txtlibActionState.Size = new System.Drawing.Size(136, 25);
            this.txtlibActionState.TabIndex = 77;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(312, 75);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(90, 22);
            this.label10.TabIndex = 68;
            this.label10.Text = "库存位置：";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.Location = new System.Drawing.Point(26, 195);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(90, 22);
            this.label15.TabIndex = 76;
            this.label15.Text = "库中记录：";
            // 
            // txtPosition
            // 
            this.txtPosition.Location = new System.Drawing.Point(408, 76);
            this.txtPosition.Name = "txtPosition";
            this.txtPosition.ReadOnly = true;
            this.txtPosition.Size = new System.Drawing.Size(136, 25);
            this.txtPosition.TabIndex = 4;
            // 
            // txtErrorPut
            // 
            this.txtErrorPut.Location = new System.Drawing.Point(408, 192);
            this.txtErrorPut.Name = "txtErrorPut";
            this.txtErrorPut.ReadOnly = true;
            this.txtErrorPut.Size = new System.Drawing.Size(136, 25);
            this.txtErrorPut.TabIndex = 75;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(312, 195);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(90, 22);
            this.label14.TabIndex = 74;
            this.label14.Text = "异常输出：";
            // 
            // txtActionState
            // 
            this.txtActionState.Location = new System.Drawing.Point(408, 21);
            this.txtActionState.Name = "txtActionState";
            this.txtActionState.ReadOnly = true;
            this.txtActionState.Size = new System.Drawing.Size(136, 25);
            this.txtActionState.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(312, 21);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(90, 22);
            this.label12.TabIndex = 72;
            this.label12.Text = "执行状态：";
            // 
            // myMenu1
            // 
            this.myMenu1.AutoSize = false;
            this.myMenu1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.myMenu1.Dock = System.Windows.Forms.DockStyle.None;
            this.myMenu1.ForeColor = System.Drawing.Color.Black;
            this.myMenu1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.myMenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.系统ToolStripMenuItem,
            this.查询ToolStripMenuItem,
            this.添加ToolStripMenuItem,
            this.通讯ToolStripMenuItem,
            this.日志ToolStripMenuItem});
            this.myMenu1.Location = new System.Drawing.Point(0, 2);
            this.myMenu1.Name = "myMenu1";
            this.myMenu1.Size = new System.Drawing.Size(486, 44);
            this.myMenu1.TabIndex = 85;
            this.myMenu1.Text = "myMenu1";
            // 
            // 系统ToolStripMenuItem
            // 
            this.系统ToolStripMenuItem.AutoSize = false;
            this.系统ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.管理员权限ToolStripMenuItem,
            this.切换为用户模式ToolStripMenuItem,
            this.修改密码ToolStripMenuItem,
            this.退出ToolStripMenuItem1});
            this.系统ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.系统ToolStripMenuItem.Name = "系统ToolStripMenuItem";
            this.系统ToolStripMenuItem.Size = new System.Drawing.Size(94, 30);
            this.系统ToolStripMenuItem.Text = "系统";
            // 
            // 管理员权限ToolStripMenuItem
            // 
            this.管理员权限ToolStripMenuItem.Name = "管理员权限ToolStripMenuItem";
            this.管理员权限ToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.管理员权限ToolStripMenuItem.Text = "管理员权限";
            this.管理员权限ToolStripMenuItem.Click += new System.EventHandler(this.管理员权限ToolStripMenuItem_Click);
            // 
            // 切换为用户模式ToolStripMenuItem
            // 
            this.切换为用户模式ToolStripMenuItem.Name = "切换为用户模式ToolStripMenuItem";
            this.切换为用户模式ToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.切换为用户模式ToolStripMenuItem.Text = "切换为用户模式";
            this.切换为用户模式ToolStripMenuItem.Click += new System.EventHandler(this.切换为用户模式ToolStripMenuItem_Click);
            // 
            // 修改密码ToolStripMenuItem
            // 
            this.修改密码ToolStripMenuItem.Name = "修改密码ToolStripMenuItem";
            this.修改密码ToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.修改密码ToolStripMenuItem.Text = "修改登录密码";
            this.修改密码ToolStripMenuItem.Click += new System.EventHandler(this.修改密码ToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem1
            // 
            this.退出ToolStripMenuItem1.Name = "退出ToolStripMenuItem1";
            this.退出ToolStripMenuItem1.Size = new System.Drawing.Size(217, 30);
            this.退出ToolStripMenuItem1.Text = "退出";
            this.退出ToolStripMenuItem1.Click += new System.EventHandler(this.退出ToolStripMenuItem1_Click);
            // 
            // 查询ToolStripMenuItem
            // 
            this.查询ToolStripMenuItem.AutoSize = false;
            this.查询ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.库存记录ToolStripMenuItem,
            this.出入库记录ToolStripMenuItem,
            this.空筒出入库记录ToolStripMenuItem});
            this.查询ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.查询ToolStripMenuItem.Name = "查询ToolStripMenuItem";
            this.查询ToolStripMenuItem.Size = new System.Drawing.Size(80, 30);
            this.查询ToolStripMenuItem.Text = "查询";
            // 
            // 库存记录ToolStripMenuItem
            // 
            this.库存记录ToolStripMenuItem.Name = "库存记录ToolStripMenuItem";
            this.库存记录ToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.库存记录ToolStripMenuItem.Text = "库存记录";
            this.库存记录ToolStripMenuItem.Click += new System.EventHandler(this.库存记录ToolStripMenuItem_Click);
            // 
            // 出入库记录ToolStripMenuItem
            // 
            this.出入库记录ToolStripMenuItem.Name = "出入库记录ToolStripMenuItem";
            this.出入库记录ToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.出入库记录ToolStripMenuItem.Text = "物料出入库记录";
            this.出入库记录ToolStripMenuItem.Click += new System.EventHandler(this.物料出入库记录ToolStripMenuItem_Click);
            // 
            // 空筒出入库记录ToolStripMenuItem
            // 
            this.空筒出入库记录ToolStripMenuItem.Name = "空筒出入库记录ToolStripMenuItem";
            this.空筒出入库记录ToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.空筒出入库记录ToolStripMenuItem.Text = "空筒出入库记录";
            this.空筒出入库记录ToolStripMenuItem.Click += new System.EventHandler(this.空筒出入库记录ToolStripMenuItem_Click);
            // 
            // 添加ToolStripMenuItem
            // 
            this.添加ToolStripMenuItem.AutoSize = false;
            this.添加ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.手动增加卷筒ToolStripMenuItem});
            this.添加ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.添加ToolStripMenuItem.Name = "添加ToolStripMenuItem";
            this.添加ToolStripMenuItem.Size = new System.Drawing.Size(94, 30);
            this.添加ToolStripMenuItem.Text = "添加";
            // 
            // 手动增加卷筒ToolStripMenuItem
            // 
            this.手动增加卷筒ToolStripMenuItem.Name = "手动增加卷筒ToolStripMenuItem";
            this.手动增加卷筒ToolStripMenuItem.Size = new System.Drawing.Size(198, 30);
            this.手动增加卷筒ToolStripMenuItem.Text = "手动增删卷筒";
            this.手动增加卷筒ToolStripMenuItem.Click += new System.EventHandler(this.手动增删卷筒ToolStripMenuItem_Click);
            // 
            // 通讯ToolStripMenuItem
            // 
            this.通讯ToolStripMenuItem.AutoSize = false;
            this.通讯ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.系列ToolStripMenuItem});
            this.通讯ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.通讯ToolStripMenuItem.Name = "通讯ToolStripMenuItem";
            this.通讯ToolStripMenuItem.Size = new System.Drawing.Size(94, 30);
            this.通讯ToolStripMenuItem.Text = "通讯";
            // 
            // 系列ToolStripMenuItem
            // 
            this.系列ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.网口ToolStripMenuItem});
            this.系列ToolStripMenuItem.Name = "系列ToolStripMenuItem";
            this.系列ToolStripMenuItem.Size = new System.Drawing.Size(201, 30);
            this.系列ToolStripMenuItem.Text = "西门子S7系列";
            // 
            // 网口ToolStripMenuItem
            // 
            this.网口ToolStripMenuItem.Name = "网口ToolStripMenuItem";
            this.网口ToolStripMenuItem.Size = new System.Drawing.Size(122, 30);
            this.网口ToolStripMenuItem.Text = "网口";
            this.网口ToolStripMenuItem.Click += new System.EventHandler(this.网口ToolStripMenuItem_Click);
            // 
            // 日志ToolStripMenuItem
            // 
            this.日志ToolStripMenuItem.AutoSize = false;
            this.日志ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.查看ToolStripMenuItem});
            this.日志ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.日志ToolStripMenuItem.Name = "日志ToolStripMenuItem";
            this.日志ToolStripMenuItem.Size = new System.Drawing.Size(94, 30);
            this.日志ToolStripMenuItem.Text = "演示";
            // 
            // 查看ToolStripMenuItem
            // 
            this.查看ToolStripMenuItem.Name = "查看ToolStripMenuItem";
            this.查看ToolStripMenuItem.Size = new System.Drawing.Size(122, 30);
            this.查看ToolStripMenuItem.Text = "指令";
            this.查看ToolStripMenuItem.Click += new System.EventHandler(this.查看ToolStripMenuItem_Click);
            // 
            // ckbordervisible
            // 
            this.ckbordervisible.AutoSize = true;
            this.ckbordervisible.Location = new System.Drawing.Point(168, 2);
            this.ckbordervisible.Name = "ckbordervisible";
            this.ckbordervisible.Size = new System.Drawing.Size(80, 23);
            this.ckbordervisible.TabIndex = 72;
            this.ckbordervisible.Text = "指令可见";
            this.ckbordervisible.UseVisualStyleBackColor = true;
            this.ckbordervisible.CheckedChanged += new System.EventHandler(this.ckbordervisible_CheckedChanged);
            // 
            // mygpbExchange
            // 
            this.mygpbExchange.BorderColor = System.Drawing.Color.Red;
            this.mygpbExchange.BorderSize = 1.5F;
            this.mygpbExchange.Controls.Add(this.panelorder);
            this.mygpbExchange.Controls.Add(this.ckbordervisible);
            this.mygpbExchange.Location = new System.Drawing.Point(12, 49);
            this.mygpbExchange.Name = "mygpbExchange";
            this.mygpbExchange.Radius = 20;
            this.mygpbExchange.Size = new System.Drawing.Size(1163, 315);
            this.mygpbExchange.TabIndex = 79;
            this.mygpbExchange.TabStop = false;
            this.mygpbExchange.Text = "指令界面";
            this.mygpbExchange.TitleColor = System.Drawing.Color.Blue;
            this.mygpbExchange.TitleFont = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // panelorder
            // 
            this.panelorder.Controls.Add(this.mygpborderhasbuff);
            this.panelorder.Controls.Add(this.mygpbActionQue);
            this.panelorder.Controls.Add(this.mygpbResults);
            this.panelorder.Controls.Add(this.mygpbOrderQue);
            this.panelorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelorder.Location = new System.Drawing.Point(3, 21);
            this.panelorder.Name = "panelorder";
            this.panelorder.Size = new System.Drawing.Size(1157, 291);
            this.panelorder.TabIndex = 2;
            // 
            // mygpborderhasbuff
            // 
            this.mygpborderhasbuff.BorderColor = System.Drawing.Color.DarkRed;
            this.mygpborderhasbuff.BorderSize = 1F;
            this.mygpborderhasbuff.Controls.Add(this.listBoxorderBuff);
            this.mygpborderhasbuff.Location = new System.Drawing.Point(12, 114);
            this.mygpborderhasbuff.Name = "mygpborderhasbuff";
            this.mygpborderhasbuff.Radius = 10;
            this.mygpborderhasbuff.Size = new System.Drawing.Size(510, 124);
            this.mygpborderhasbuff.TabIndex = 94;
            this.mygpborderhasbuff.TabStop = false;
            this.mygpborderhasbuff.Text = "指令缓存队列";
            this.mygpborderhasbuff.TitleFont = new System.Drawing.Font("宋体", 10F);
            // 
            // listBoxorderBuff
            // 
            this.listBoxorderBuff.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.listBoxorderBuff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxorderBuff.FormattingEnabled = true;
            this.listBoxorderBuff.HorizontalScrollbar = true;
            this.listBoxorderBuff.ItemHeight = 19;
            this.listBoxorderBuff.Location = new System.Drawing.Point(3, 21);
            this.listBoxorderBuff.Name = "listBoxorderBuff";
            this.listBoxorderBuff.ScrollAlwaysVisible = true;
            this.listBoxorderBuff.Size = new System.Drawing.Size(504, 100);
            this.listBoxorderBuff.TabIndex = 1;
            // 
            // mygpbActionQue
            // 
            this.mygpbActionQue.BorderColor = System.Drawing.Color.DarkRed;
            this.mygpbActionQue.BorderSize = 1F;
            this.mygpbActionQue.Controls.Add(this.listBoxAction);
            this.mygpbActionQue.Location = new System.Drawing.Point(578, 6);
            this.mygpbActionQue.Name = "mygpbActionQue";
            this.mygpbActionQue.Radius = 10;
            this.mygpbActionQue.Size = new System.Drawing.Size(550, 105);
            this.mygpbActionQue.TabIndex = 93;
            this.mygpbActionQue.TabStop = false;
            this.mygpbActionQue.Text = "分步执行队列";
            this.mygpbActionQue.TitleFont = new System.Drawing.Font("宋体", 10F);
            // 
            // listBoxAction
            // 
            this.listBoxAction.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.listBoxAction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxAction.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxAction.FormattingEnabled = true;
            this.listBoxAction.HorizontalScrollbar = true;
            this.listBoxAction.ItemHeight = 19;
            this.listBoxAction.Location = new System.Drawing.Point(3, 21);
            this.listBoxAction.Name = "listBoxAction";
            this.listBoxAction.ScrollAlwaysVisible = true;
            this.listBoxAction.Size = new System.Drawing.Size(544, 81);
            this.listBoxAction.TabIndex = 1;
            this.listBoxAction.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxAction_DrawItem);
            // 
            // mygpbResults
            // 
            this.mygpbResults.BorderColor = System.Drawing.Color.DarkRed;
            this.mygpbResults.BorderSize = 1F;
            this.mygpbResults.Controls.Add(this.dgvPosition);
            this.mygpbResults.Location = new System.Drawing.Point(578, 117);
            this.mygpbResults.Name = "mygpbResults";
            this.mygpbResults.Radius = 10;
            this.mygpbResults.Size = new System.Drawing.Size(556, 121);
            this.mygpbResults.TabIndex = 89;
            this.mygpbResults.TabStop = false;
            this.mygpbResults.Text = "库中查询结果";
            this.mygpbResults.TitleFont = new System.Drawing.Font("宋体", 10F);
            // 
            // dgvPosition
            // 
            this.dgvPosition.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgvPosition.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPosition.Location = new System.Drawing.Point(3, 21);
            this.dgvPosition.Name = "dgvPosition";
            this.dgvPosition.ReadOnly = true;
            this.dgvPosition.RowTemplate.Height = 23;
            this.dgvPosition.Size = new System.Drawing.Size(550, 97);
            this.dgvPosition.TabIndex = 1;
            // 
            // mygpbOrderQue
            // 
            this.mygpbOrderQue.BorderColor = System.Drawing.Color.DarkRed;
            this.mygpbOrderQue.BorderSize = 1F;
            this.mygpbOrderQue.Controls.Add(this.listBoxOrder);
            this.mygpbOrderQue.Location = new System.Drawing.Point(12, 3);
            this.mygpbOrderQue.Name = "mygpbOrderQue";
            this.mygpbOrderQue.Radius = 10;
            this.mygpbOrderQue.Size = new System.Drawing.Size(510, 108);
            this.mygpbOrderQue.TabIndex = 88;
            this.mygpbOrderQue.TabStop = false;
            this.mygpbOrderQue.Text = "指令主队列";
            this.mygpbOrderQue.TitleFont = new System.Drawing.Font("宋体", 10F);
            // 
            // listBoxOrder
            // 
            this.listBoxOrder.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.listBoxOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxOrder.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxOrder.FormattingEnabled = true;
            this.listBoxOrder.HorizontalScrollbar = true;
            this.listBoxOrder.ItemHeight = 19;
            this.listBoxOrder.Location = new System.Drawing.Point(3, 21);
            this.listBoxOrder.Name = "listBoxOrder";
            this.listBoxOrder.ScrollAlwaysVisible = true;
            this.listBoxOrder.Size = new System.Drawing.Size(504, 84);
            this.listBoxOrder.TabIndex = 1;
            this.listBoxOrder.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxOrder_DrawItem);
            // 
            // myGroupBox6
            // 
            this.myGroupBox6.BorderColor = System.Drawing.Color.Red;
            this.myGroupBox6.BorderSize = 1.5F;
            this.myGroupBox6.Controls.Add(this.listBoxShowCurrentOrder);
            this.myGroupBox6.Location = new System.Drawing.Point(12, 370);
            this.myGroupBox6.Name = "myGroupBox6";
            this.myGroupBox6.Radius = 10;
            this.myGroupBox6.Size = new System.Drawing.Size(1164, 153);
            this.myGroupBox6.TabIndex = 86;
            this.myGroupBox6.TabStop = false;
            this.myGroupBox6.Text = "作业队列：";
            this.myGroupBox6.TitleColor = System.Drawing.Color.Blue;
            this.myGroupBox6.TitleFont = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // listBoxShowCurrentOrder
            // 
            this.listBoxShowCurrentOrder.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.listBoxShowCurrentOrder.ContextMenuStrip = this.cMenuOrderRight;
            this.listBoxShowCurrentOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxShowCurrentOrder.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxShowCurrentOrder.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBoxShowCurrentOrder.FormattingEnabled = true;
            this.listBoxShowCurrentOrder.HorizontalScrollbar = true;
            this.listBoxShowCurrentOrder.ItemHeight = 19;
            this.listBoxShowCurrentOrder.Location = new System.Drawing.Point(3, 21);
            this.listBoxShowCurrentOrder.Name = "listBoxShowCurrentOrder";
            this.listBoxShowCurrentOrder.ScrollAlwaysVisible = true;
            this.listBoxShowCurrentOrder.Size = new System.Drawing.Size(1158, 129);
            this.listBoxShowCurrentOrder.TabIndex = 2;
            this.listBoxShowCurrentOrder.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxCurrentOrder_DrawItem);
            // 
            // cMenuOrderRight
            // 
            this.cMenuOrderRight.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除ToolStripMenuItem,
            this.返回ToolStripMenuItem});
            this.cMenuOrderRight.Name = "cMenuOrderRight";
            this.cMenuOrderRight.Size = new System.Drawing.Size(119, 64);
            // 
            // 删除ToolStripMenuItem
            // 
            this.删除ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            this.删除ToolStripMenuItem.Size = new System.Drawing.Size(118, 30);
            this.删除ToolStripMenuItem.Text = "删除";
            this.删除ToolStripMenuItem.Click += new System.EventHandler(this.删除ToolStripMenuItem_Click);
            // 
            // 返回ToolStripMenuItem
            // 
            this.返回ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.返回ToolStripMenuItem.Name = "返回ToolStripMenuItem";
            this.返回ToolStripMenuItem.Size = new System.Drawing.Size(118, 30);
            this.返回ToolStripMenuItem.Text = "返回";
            // 
            // frmNewMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1210, 742);
            this.Controls.Add(this.myGroupBox6);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.myGroupBox1);
            this.Controls.Add(this.mygpbExchange);
            this.Controls.Add(this.myMenu1);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MainMenuStrip = this.myMenu1;
            this.Name = "frmNewMain";
            this.ShowIcon = false;
            this.Activated += new System.EventHandler(this.frmNewMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_Closing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.SizeChanged += new System.EventHandler(this.frmMain_SizeChanged);
            this.myrightMenu.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.myGroupBox1.ResumeLayout(false);
            this.myGroupBox1.PerformLayout();
            this.myMenu1.ResumeLayout(false);
            this.myMenu1.PerformLayout();
            this.mygpbExchange.ResumeLayout(false);
            this.mygpbExchange.PerformLayout();
            this.panelorder.ResumeLayout(false);
            this.mygpborderhasbuff.ResumeLayout(false);
            this.mygpbActionQue.ResumeLayout(false);
            this.mygpbResults.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPosition)).EndInit();
            this.mygpbOrderQue.ResumeLayout(false);
            this.myGroupBox6.ResumeLayout(false);
            this.cMenuOrderRight.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private IContainer components;

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

        private OpenFileDialog openFileDialog1;
        private NotifyIcon myIcon;
        private Timer timer1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblState;
        private ContextMenuStrip myrightMenu;
        private ToolStripMenuItem 退出ToolStripMenuItem;
        private TextBox txtlibActionState;
        private Label label15;
        private TextBox txtActionState;
        private Label label12;
        private TextBox txtGWposition;
        private Label label11;
        private TextBox txtPosition;
        private Label label10;
        private TextBox txtAction;
        private Label label9;
        private TextBox txtGoodsState;
        private Label label8;
        private TextBox txtGoodsCode;
        private Label label7;
        private MyGroupBox myGroupBox1;
        private Label label1;
        private TextBox txtSelctEmptyNum;
        private Label label2;
        private TextBox txtCheckState;
        private TextBox txtSteps;
        private Label label3;
        private MyMenu myMenu1;
        private ToolStripMenuItem 系统ToolStripMenuItem;
        private ToolStripMenuItem 修改密码ToolStripMenuItem;
        private ToolStripMenuItem 查询ToolStripMenuItem;
        private ToolStripMenuItem 库存记录ToolStripMenuItem;
        private ToolStripMenuItem 出入库记录ToolStripMenuItem;
        private ToolStripMenuItem 空筒出入库记录ToolStripMenuItem;
        private ToolStripMenuItem 添加ToolStripMenuItem;
        private ToolStripMenuItem 手动增加卷筒ToolStripMenuItem;
        private ToolStripMenuItem 通讯ToolStripMenuItem;
        private ToolStripMenuItem 系列ToolStripMenuItem;
        private ToolStripMenuItem 网口ToolStripMenuItem;
        private ToolStripStatusLabel lblTimer;
        private ToolStripStatusLabel lblEdit;
        private CheckBox ckbordervisible;
        private MyGroupBox mygpbExchange;
        private Panel panelorder;
        private MyGroupBox mygpbActionQue;
        private ListBox listBoxAction;
        private MyGroupBox mygpbResults;
        private DataGridView dgvPosition;
        private MyGroupBox mygpbOrderQue;
        private ListBox listBoxOrder;
        private MyGroupBox myGroupBox6;
        private ListBox listBoxShowCurrentOrder;
        private ToolStripMenuItem 退出ToolStripMenuItem1;
        private ToolStripMenuItem 管理员权限ToolStripMenuItem;
        private ToolStripMenuItem 切换为用户模式ToolStripMenuItem;
        private ContextMenuStrip cMenuOrderRight;
        private ToolStripMenuItem 删除ToolStripMenuItem;
        private ToolStripMenuItem 返回ToolStripMenuItem;
        private MyGroupBox mygpborderhasbuff;
        private ListBox listBoxorderBuff;
        private TextBox txtErrorPut;
        private Label label14;
        private ToolStripMenuItem 日志ToolStripMenuItem;
        private ToolStripMenuItem 查看ToolStripMenuItem;
    }
}