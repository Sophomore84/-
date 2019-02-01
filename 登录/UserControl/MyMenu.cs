using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace  风帆电池
{
    public partial class MyMenu : MenuStrip
    {
        public MyMenu()
        {
            InitializeComponent();
            this.Renderer = new MyMenuRender();//设置渲染
            //设置Style支持透明背景色
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.FromArgb(0, 0, 0, 0);
        }

        public MyMenu(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            this.Renderer = new MyMenuRender();//设置渲染
            //设置Style支持透明背景色
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.FromArgb(0, 0, 0, 0);
        }
    }
}
