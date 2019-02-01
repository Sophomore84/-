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
    public partial class FormYanShi : FormBase
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
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox17 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox16 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox15 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox14 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox13 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox12 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox18 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox11 = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.myGroupBox3 = new 风帆电池.MyGroupBox(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.comboBox28 = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.comboBox27 = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.comboBox26 = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.comboBox25 = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.comboBox24 = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.comboBox21 = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.comboBox22 = new System.Windows.Forms.ComboBox();
            this.label23 = new System.Windows.Forms.Label();
            this.comboBox23 = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.myGroupBox2 = new 风帆电池.MyGroupBox(this.components);
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox38 = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBox37 = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBox36 = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBox35 = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBox34 = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBox33 = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.comboBox32 = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.comboBox31 = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.myGroupBox1.SuspendLayout();
            this.myGroupBox3.SuspendLayout();
            this.myGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // myGroupBox1
            // 
            this.myGroupBox1.BorderColor = System.Drawing.Color.Black;
            this.myGroupBox1.BorderSize = 1F;
            this.myGroupBox1.Controls.Add(this.button1);
            this.myGroupBox1.Controls.Add(this.comboBox17);
            this.myGroupBox1.Controls.Add(this.label7);
            this.myGroupBox1.Controls.Add(this.comboBox16);
            this.myGroupBox1.Controls.Add(this.label6);
            this.myGroupBox1.Controls.Add(this.comboBox15);
            this.myGroupBox1.Controls.Add(this.label5);
            this.myGroupBox1.Controls.Add(this.comboBox14);
            this.myGroupBox1.Controls.Add(this.label4);
            this.myGroupBox1.Controls.Add(this.comboBox13);
            this.myGroupBox1.Controls.Add(this.label3);
            this.myGroupBox1.Controls.Add(this.comboBox12);
            this.myGroupBox1.Controls.Add(this.label2);
            this.myGroupBox1.Controls.Add(this.comboBox18);
            this.myGroupBox1.Controls.Add(this.label1);
            this.myGroupBox1.Controls.Add(this.comboBox11);
            this.myGroupBox1.Controls.Add(this.label16);
            this.myGroupBox1.Location = new System.Drawing.Point(39, 32);
            this.myGroupBox1.Name = "myGroupBox1";
            this.myGroupBox1.Radius = 10;
            this.myGroupBox1.Size = new System.Drawing.Size(827, 208);
            this.myGroupBox1.TabIndex = 0;
            this.myGroupBox1.TabStop = false;
            this.myGroupBox1.Text = "A1区";
            this.myGroupBox1.TitleFont = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(584, 155);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(143, 41);
            this.button1.TabIndex = 56;
            this.button1.Text = "确认";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox17
            // 
            this.comboBox17.FormattingEnabled = true;
            this.comboBox17.IntegralHeight = false;
            this.comboBox17.Location = new System.Drawing.Point(121, 160);
            this.comboBox17.Name = "comboBox17";
            this.comboBox17.Size = new System.Drawing.Size(138, 27);
            this.comboBox17.TabIndex = 54;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(296, 161);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 26);
            this.label7.TabIndex = 55;
            this.label7.Text = "检测：";
            // 
            // comboBox16
            // 
            this.comboBox16.FormattingEnabled = true;
            this.comboBox16.IntegralHeight = false;
            this.comboBox16.Location = new System.Drawing.Point(662, 104);
            this.comboBox16.Name = "comboBox16";
            this.comboBox16.Size = new System.Drawing.Size(138, 27);
            this.comboBox16.TabIndex = 52;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(17, 161);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 26);
            this.label6.TabIndex = 53;
            this.label6.Text = "物料状态：";
            // 
            // comboBox15
            // 
            this.comboBox15.FormattingEnabled = true;
            this.comboBox15.IntegralHeight = false;
            this.comboBox15.Location = new System.Drawing.Point(371, 107);
            this.comboBox15.Name = "comboBox15";
            this.comboBox15.Size = new System.Drawing.Size(138, 27);
            this.comboBox15.TabIndex = 50;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(557, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 26);
            this.label5.TabIndex = 51;
            this.label5.Text = "物料规格：";
            // 
            // comboBox14
            // 
            this.comboBox14.FormattingEnabled = true;
            this.comboBox14.IntegralHeight = false;
            this.comboBox14.Location = new System.Drawing.Point(122, 101);
            this.comboBox14.Name = "comboBox14";
            this.comboBox14.Size = new System.Drawing.Size(138, 27);
            this.comboBox14.TabIndex = 48;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(296, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 26);
            this.label4.TabIndex = 49;
            this.label4.Text = "班组 :";
            // 
            // comboBox13
            // 
            this.comboBox13.FormattingEnabled = true;
            this.comboBox13.IntegralHeight = false;
            this.comboBox13.Location = new System.Drawing.Point(662, 40);
            this.comboBox13.Name = "comboBox13";
            this.comboBox13.Size = new System.Drawing.Size(138, 27);
            this.comboBox13.TabIndex = 46;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(17, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 26);
            this.label3.TabIndex = 47;
            this.label3.Text = "指令状态 :";
            // 
            // comboBox12
            // 
            this.comboBox12.FormattingEnabled = true;
            this.comboBox12.IntegralHeight = false;
            this.comboBox12.Location = new System.Drawing.Point(371, 41);
            this.comboBox12.Name = "comboBox12";
            this.comboBox12.Size = new System.Drawing.Size(138, 27);
            this.comboBox12.TabIndex = 44;
            this.comboBox12.SelectedIndexChanged += new System.EventHandler(this.comboBox12_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(557, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 26);
            this.label2.TabIndex = 45;
            this.label2.Text = "动作方式 :";
            // 
            // comboBox18
            // 
            this.comboBox18.FormattingEnabled = true;
            this.comboBox18.IntegralHeight = false;
            this.comboBox18.Location = new System.Drawing.Point(371, 160);
            this.comboBox18.Name = "comboBox18";
            this.comboBox18.Size = new System.Drawing.Size(138, 27);
            this.comboBox18.TabIndex = 42;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(293, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 26);
            this.label1.TabIndex = 43;
            this.label1.Text = "动作：";
            // 
            // comboBox11
            // 
            this.comboBox11.FormattingEnabled = true;
            this.comboBox11.IntegralHeight = false;
            this.comboBox11.Location = new System.Drawing.Point(121, 40);
            this.comboBox11.Name = "comboBox11";
            this.comboBox11.Size = new System.Drawing.Size(138, 27);
            this.comboBox11.TabIndex = 40;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.Location = new System.Drawing.Point(17, 44);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(107, 26);
            this.label16.TabIndex = 41;
            this.label16.Text = "工位区域：";
            // 
            // myGroupBox3
            // 
            this.myGroupBox3.BorderColor = System.Drawing.Color.Black;
            this.myGroupBox3.BorderSize = 1F;
            this.myGroupBox3.Controls.Add(this.button2);
            this.myGroupBox3.Controls.Add(this.comboBox28);
            this.myGroupBox3.Controls.Add(this.label17);
            this.myGroupBox3.Controls.Add(this.comboBox27);
            this.myGroupBox3.Controls.Add(this.label18);
            this.myGroupBox3.Controls.Add(this.comboBox26);
            this.myGroupBox3.Controls.Add(this.label19);
            this.myGroupBox3.Controls.Add(this.comboBox25);
            this.myGroupBox3.Controls.Add(this.label20);
            this.myGroupBox3.Controls.Add(this.comboBox24);
            this.myGroupBox3.Controls.Add(this.label21);
            this.myGroupBox3.Controls.Add(this.comboBox21);
            this.myGroupBox3.Controls.Add(this.label22);
            this.myGroupBox3.Controls.Add(this.comboBox22);
            this.myGroupBox3.Controls.Add(this.label23);
            this.myGroupBox3.Controls.Add(this.comboBox23);
            this.myGroupBox3.Controls.Add(this.label24);
            this.myGroupBox3.Location = new System.Drawing.Point(39, 246);
            this.myGroupBox3.Name = "myGroupBox3";
            this.myGroupBox3.Radius = 10;
            this.myGroupBox3.Size = new System.Drawing.Size(827, 208);
            this.myGroupBox3.TabIndex = 2;
            this.myGroupBox3.TabStop = false;
            this.myGroupBox3.Text = "A2区";
            this.myGroupBox3.TitleFont = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(584, 155);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(143, 41);
            this.button2.TabIndex = 56;
            this.button2.Text = "确认";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // comboBox28
            // 
            this.comboBox28.FormattingEnabled = true;
            this.comboBox28.IntegralHeight = false;
            this.comboBox28.Location = new System.Drawing.Point(371, 161);
            this.comboBox28.Name = "comboBox28";
            this.comboBox28.Size = new System.Drawing.Size(138, 27);
            this.comboBox28.TabIndex = 54;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.Location = new System.Drawing.Point(296, 161);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(69, 26);
            this.label17.TabIndex = 55;
            this.label17.Text = "检测：";
            // 
            // comboBox27
            // 
            this.comboBox27.FormattingEnabled = true;
            this.comboBox27.IntegralHeight = false;
            this.comboBox27.Location = new System.Drawing.Point(121, 160);
            this.comboBox27.Name = "comboBox27";
            this.comboBox27.Size = new System.Drawing.Size(138, 27);
            this.comboBox27.TabIndex = 52;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label18.Location = new System.Drawing.Point(17, 161);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(107, 26);
            this.label18.TabIndex = 53;
            this.label18.Text = "物料状态：";
            // 
            // comboBox26
            // 
            this.comboBox26.FormattingEnabled = true;
            this.comboBox26.IntegralHeight = false;
            this.comboBox26.Location = new System.Drawing.Point(662, 104);
            this.comboBox26.Name = "comboBox26";
            this.comboBox26.Size = new System.Drawing.Size(138, 27);
            this.comboBox26.TabIndex = 50;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label19.Location = new System.Drawing.Point(557, 105);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(107, 26);
            this.label19.TabIndex = 51;
            this.label19.Text = "物料规格：";
            // 
            // comboBox25
            // 
            this.comboBox25.FormattingEnabled = true;
            this.comboBox25.IntegralHeight = false;
            this.comboBox25.Location = new System.Drawing.Point(371, 103);
            this.comboBox25.Name = "comboBox25";
            this.comboBox25.Size = new System.Drawing.Size(138, 27);
            this.comboBox25.TabIndex = 48;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label20.Location = new System.Drawing.Point(296, 104);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(61, 26);
            this.label20.TabIndex = 49;
            this.label20.Text = "班组 :";
            // 
            // comboBox24
            // 
            this.comboBox24.FormattingEnabled = true;
            this.comboBox24.IntegralHeight = false;
            this.comboBox24.Location = new System.Drawing.Point(121, 104);
            this.comboBox24.Name = "comboBox24";
            this.comboBox24.Size = new System.Drawing.Size(138, 27);
            this.comboBox24.TabIndex = 46;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label21.Location = new System.Drawing.Point(17, 102);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(99, 26);
            this.label21.TabIndex = 47;
            this.label21.Text = "指令状态 :";
            // 
            // comboBox21
            // 
            this.comboBox21.FormattingEnabled = true;
            this.comboBox21.IntegralHeight = false;
            this.comboBox21.Location = new System.Drawing.Point(122, 40);
            this.comboBox21.Name = "comboBox21";
            this.comboBox21.Size = new System.Drawing.Size(138, 27);
            this.comboBox21.TabIndex = 44;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label22.Location = new System.Drawing.Point(557, 41);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(99, 26);
            this.label22.TabIndex = 45;
            this.label22.Text = "动作方式 :";
            // 
            // comboBox22
            // 
            this.comboBox22.FormattingEnabled = true;
            this.comboBox22.IntegralHeight = false;
            this.comboBox22.Location = new System.Drawing.Point(368, 40);
            this.comboBox22.Name = "comboBox22";
            this.comboBox22.Size = new System.Drawing.Size(138, 27);
            this.comboBox22.TabIndex = 42;
            this.comboBox22.SelectedIndexChanged += new System.EventHandler(this.comboBox22_SelectedIndexChanged);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label23.Location = new System.Drawing.Point(293, 41);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(69, 26);
            this.label23.TabIndex = 43;
            this.label23.Text = "动作：";
            // 
            // comboBox23
            // 
            this.comboBox23.FormattingEnabled = true;
            this.comboBox23.IntegralHeight = false;
            this.comboBox23.Location = new System.Drawing.Point(662, 40);
            this.comboBox23.Name = "comboBox23";
            this.comboBox23.Size = new System.Drawing.Size(138, 27);
            this.comboBox23.TabIndex = 40;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label24.Location = new System.Drawing.Point(17, 44);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(107, 26);
            this.label24.TabIndex = 41;
            this.label24.Text = "工位区域：";
            // 
            // myGroupBox2
            // 
            this.myGroupBox2.BorderColor = System.Drawing.Color.Black;
            this.myGroupBox2.BorderSize = 1F;
            this.myGroupBox2.Controls.Add(this.button3);
            this.myGroupBox2.Controls.Add(this.comboBox38);
            this.myGroupBox2.Controls.Add(this.label8);
            this.myGroupBox2.Controls.Add(this.comboBox37);
            this.myGroupBox2.Controls.Add(this.label9);
            this.myGroupBox2.Controls.Add(this.comboBox36);
            this.myGroupBox2.Controls.Add(this.label10);
            this.myGroupBox2.Controls.Add(this.comboBox35);
            this.myGroupBox2.Controls.Add(this.label11);
            this.myGroupBox2.Controls.Add(this.comboBox34);
            this.myGroupBox2.Controls.Add(this.label12);
            this.myGroupBox2.Controls.Add(this.comboBox33);
            this.myGroupBox2.Controls.Add(this.label13);
            this.myGroupBox2.Controls.Add(this.comboBox32);
            this.myGroupBox2.Controls.Add(this.label14);
            this.myGroupBox2.Controls.Add(this.comboBox31);
            this.myGroupBox2.Controls.Add(this.label15);
            this.myGroupBox2.Location = new System.Drawing.Point(39, 460);
            this.myGroupBox2.Name = "myGroupBox2";
            this.myGroupBox2.Radius = 10;
            this.myGroupBox2.Size = new System.Drawing.Size(827, 228);
            this.myGroupBox2.TabIndex = 3;
            this.myGroupBox2.TabStop = false;
            this.myGroupBox2.Text = "B区";
            this.myGroupBox2.TitleFont = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(584, 155);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(143, 41);
            this.button3.TabIndex = 56;
            this.button3.Text = "确认";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBox38
            // 
            this.comboBox38.FormattingEnabled = true;
            this.comboBox38.IntegralHeight = false;
            this.comboBox38.Location = new System.Drawing.Point(371, 161);
            this.comboBox38.Name = "comboBox38";
            this.comboBox38.Size = new System.Drawing.Size(138, 27);
            this.comboBox38.TabIndex = 54;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(296, 161);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 26);
            this.label8.TabIndex = 55;
            this.label8.Text = "检测：";
            // 
            // comboBox37
            // 
            this.comboBox37.FormattingEnabled = true;
            this.comboBox37.IntegralHeight = false;
            this.comboBox37.Location = new System.Drawing.Point(121, 160);
            this.comboBox37.Name = "comboBox37";
            this.comboBox37.Size = new System.Drawing.Size(138, 27);
            this.comboBox37.TabIndex = 52;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(17, 161);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(107, 26);
            this.label9.TabIndex = 53;
            this.label9.Text = "物料状态：";
            // 
            // comboBox36
            // 
            this.comboBox36.FormattingEnabled = true;
            this.comboBox36.IntegralHeight = false;
            this.comboBox36.Location = new System.Drawing.Point(662, 104);
            this.comboBox36.Name = "comboBox36";
            this.comboBox36.Size = new System.Drawing.Size(138, 27);
            this.comboBox36.TabIndex = 50;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(557, 105);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(107, 26);
            this.label10.TabIndex = 51;
            this.label10.Text = "物料规格：";
            // 
            // comboBox35
            // 
            this.comboBox35.FormattingEnabled = true;
            this.comboBox35.IntegralHeight = false;
            this.comboBox35.Location = new System.Drawing.Point(371, 103);
            this.comboBox35.Name = "comboBox35";
            this.comboBox35.Size = new System.Drawing.Size(138, 27);
            this.comboBox35.TabIndex = 48;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(296, 104);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 26);
            this.label11.TabIndex = 49;
            this.label11.Text = "班组 :";
            // 
            // comboBox34
            // 
            this.comboBox34.FormattingEnabled = true;
            this.comboBox34.IntegralHeight = false;
            this.comboBox34.Location = new System.Drawing.Point(121, 104);
            this.comboBox34.Name = "comboBox34";
            this.comboBox34.Size = new System.Drawing.Size(138, 27);
            this.comboBox34.TabIndex = 46;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(17, 102);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(99, 26);
            this.label12.TabIndex = 47;
            this.label12.Text = "指令状态 :";
            // 
            // comboBox33
            // 
            this.comboBox33.FormattingEnabled = true;
            this.comboBox33.IntegralHeight = false;
            this.comboBox33.Location = new System.Drawing.Point(662, 40);
            this.comboBox33.Name = "comboBox33";
            this.comboBox33.Size = new System.Drawing.Size(138, 27);
            this.comboBox33.TabIndex = 44;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(557, 41);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(99, 26);
            this.label13.TabIndex = 45;
            this.label13.Text = "动作方式 :";
            // 
            // comboBox32
            // 
            this.comboBox32.FormattingEnabled = true;
            this.comboBox32.IntegralHeight = false;
            this.comboBox32.Location = new System.Drawing.Point(368, 40);
            this.comboBox32.Name = "comboBox32";
            this.comboBox32.Size = new System.Drawing.Size(138, 27);
            this.comboBox32.TabIndex = 42;
            this.comboBox32.SelectedIndexChanged += new System.EventHandler(this.comboBox32_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(293, 41);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(69, 26);
            this.label14.TabIndex = 43;
            this.label14.Text = "动作：";
            // 
            // comboBox31
            // 
            this.comboBox31.FormattingEnabled = true;
            this.comboBox31.IntegralHeight = false;
            this.comboBox31.Location = new System.Drawing.Point(121, 40);
            this.comboBox31.Name = "comboBox31";
            this.comboBox31.Size = new System.Drawing.Size(138, 27);
            this.comboBox31.TabIndex = 40;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.Location = new System.Drawing.Point(17, 44);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(107, 26);
            this.label15.TabIndex = 41;
            this.label15.Text = "工位区域：";
            // 
            // FormYanShi
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1040, 700);
            this.Controls.Add(this.myGroupBox2);
            this.Controls.Add(this.myGroupBox3);
            this.Controls.Add(this.myGroupBox1);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormYanShi";
            this.Text = "风帆电池项目";
            this.Load += new System.EventHandler(this.FormYanShi_Load);
            this.myGroupBox1.ResumeLayout(false);
            this.myGroupBox1.PerformLayout();
            this.myGroupBox3.ResumeLayout(false);
            this.myGroupBox3.PerformLayout();
            this.myGroupBox2.ResumeLayout(false);
            this.myGroupBox2.PerformLayout();
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
        private MyGroupBox myGroupBox1;
        private Button button1;
        private ComboBox comboBox17;
        private Label label7;
        private ComboBox comboBox16;
        private Label label6;
        private ComboBox comboBox15;
        private Label label5;
        private ComboBox comboBox14;
        private Label label4;
        private ComboBox comboBox13;
        private Label label3;
        private ComboBox comboBox12;
        private Label label2;
        private ComboBox comboBox18;
        private Label label1;
        private ComboBox comboBox11;
        private Label label16;
        private MyGroupBox myGroupBox3;
        private Button button2;
        private ComboBox comboBox28;
        private Label label17;
        private ComboBox comboBox27;
        private Label label18;
        private ComboBox comboBox26;
        private Label label19;
        private ComboBox comboBox25;
        private Label label20;
        private ComboBox comboBox24;
        private Label label21;
        private ComboBox comboBox21;
        private Label label22;
        private ComboBox comboBox22;
        private Label label23;
        private ComboBox comboBox23;
        private Label label24;
        private MyGroupBox myGroupBox2;
        private Button button3;
        private ComboBox comboBox38;
        private Label label8;
        private ComboBox comboBox37;
        private Label label9;
        private ComboBox comboBox36;
        private Label label10;
        private ComboBox comboBox35;
        private Label label11;
        private ComboBox comboBox34;
        private Label label12;
        private ComboBox comboBox33;
        private Label label13;
        private ComboBox comboBox32;
        private Label label14;
        private ComboBox comboBox31;
        private Label label15;
    }
}