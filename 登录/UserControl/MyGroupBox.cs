using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace  风帆电池
{
    public partial class MyGroupBox : System.Windows.Forms.GroupBox
    {
        //增加一个BoderColor属性和BoderSize属性
        private Color mBorderColor = Color.Black;

        [Browsable(true), Description("边框颜色"), Category("自定义分组")]
        public Color BorderColor
        {
            get { return mBorderColor; }
            set { mBorderColor = value; }
        }

        private float _BorderSize = 1;

        [Browsable(true), Description("边框粗细"), Category("自定义分组")]
        public float BorderSize
        {
            get { return _BorderSize; }
            set
            {
                _BorderSize = value;
                this.Invalidate();
            }
        }


        //设置标题字体颜色位置等
        private Font _titleFont = new Font("宋体", 10, FontStyle.Regular);
        private Color _titleColor = Color.Green;
        private int _radius = 10;
        private int _tiltePos = 10;
        private const int WM_ERASEBKGND = 0x0014;
        private const int WM_PAINT = 0xF;
        [DefaultValue(typeof(Color), "Green"), Description("标题颜色")]
        public Color TitleColor
        {
            get { return _titleColor; }
            set
            {
                _titleColor = value;
                base.Invalidate();
            }
        }

        [DefaultValue(typeof(Font), ""), Description("标题字体设置")]
        public Font TitleFont
        {
            get { return _titleFont; }
            set
            {
                _titleFont = value;
                base.Invalidate();
            }
        }


        [DefaultValue(typeof(int), "30"), Description("圆角弧度大小")]
        public int Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                base.Invalidate();
            }
        }

        [DefaultValue(typeof(int), "10"), Description("标题位置")]
        public int TiltePos
        {
            get { return _tiltePos; }
            set
            {
                _tiltePos = value;
                base.Invalidate();
            }
        }



        public MyGroupBox() : base()
        {
            InitializeComponent();
        }

        public MyGroupBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                base.WndProc(ref m);
                if (m.Msg == WM_PAINT)
                {
                    if (this.Radius > 0)
                    {
                        using (Graphics g = Graphics.FromHwnd(this.Handle))
                        {
                            Rectangle r = new Rectangle();
                            r.Width = this.Width;
                            r.Height = this.Height;
                            DrawBorder(g, r, this.Radius);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void DrawBorder(Graphics g, Rectangle rect, int radius)
        {
            rect.Width -= 1;
            rect.Height -= 1;


            using (Pen pen = new Pen(this.BorderColor, this.BorderSize))
            {
                g.Clear(this.BackColor);
                g.DrawString(this.Text, this.TitleFont, new SolidBrush(this.TitleColor), radius + this.TiltePos, 0);

                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                GraphicsPath path = new GraphicsPath();

                float height = g.MeasureString(this.Text, this.TitleFont).Height / 2;
                float width = g.MeasureString(this.Text, this.TitleFont).Width;

                path.AddArc(rect.X, rect.Y + height, radius, radius, 180, 90);//左上角弧线   
                path.AddLine(radius, rect.Y + height, radius + this.TiltePos, rect.Y + height);

                path.StartFigure();

                path.AddLine(radius + this.TiltePos + width, rect.Y + height, rect.Right - radius, rect.Y + height);

                path.AddArc(rect.Right - radius, rect.Y + height, radius, radius, 270, 90);//右上角弧线   
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);

                path.StartFigure();

                path.AddArc(rect.X, rect.Y + height, radius, radius, -90, -90);//左上角弧线   
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, -180, -90);


                g.DrawPath(pen, path);
            }
        }


        // 重写OnPaint
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    var vSize = e.Graphics.MeasureString(this.Text, this.Font);

        //    e.Graphics.Clear(this.BackColor);
        //    e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), 10, 1);
        //    Pen vPen = new Pen(this.mBorderColor, this.BorderSize); // 用属性颜色来画边框颜色
        //    e.Graphics.DrawLine(vPen, 1, vSize.Height / 2, 8, vSize.Height / 2);
        //    e.Graphics.DrawLine(vPen, vSize.Width + 8, vSize.Height / 2, this.Width - 2, vSize.Height / 2);
        //    e.Graphics.DrawLine(vPen, 1, vSize.Height / 2, 1, this.Height - 2);
        //    e.Graphics.DrawLine(vPen, 1, this.Height - 2, this.Width - 2, this.Height - 2);
        //    e.Graphics.DrawLine(vPen, this.Width - 2, vSize.Height / 2, this.Width - 2, this.Height - 2);
        //}
    }
}
