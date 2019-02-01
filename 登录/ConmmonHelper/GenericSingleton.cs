using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace  风帆电池
{
    //单例模式泛型类  显示单个窗体
    public class GenericSingleton<T> where T : Form, new()
    {
        private static T t = null;
        public static T CreateInstrance()
        {
            if (null == t || t.IsDisposed)
            {
                t = new T();
            }
            return t;
        }
    }
}
