using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 风帆电池
{
    /// <summary>
    /// 拥有ToolTip属性的Form基类
    /// </summary>
    public class FormBase : Form
    {
        private ToolTip _toolTip;

        public FormBase()
            : base()
        {
            _toolTip = new ToolTip();
        }

        internal ToolTip ToolTip
        {
            get { return _toolTip; }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _toolTip.Dispose();
            }
        }
    }
}
