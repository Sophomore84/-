using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 风帆电池
{
    public partial class FormYanShi : FormBase
    {

        private object locker = new object();
        public FormYanShi()
        {
            InitializeComponent();
            FormExIni();
            _systemButtonManager = new SystemButtonManager(this);
        }

        private void FormYanShi_Load(object sender, EventArgs e)
        {
            comboBox11.Items.Add("A1");
            comboBox21.Items.Add("A2");
            comboBox31.Items.Add(" B");
            comboBox11.Enabled = false;
            comboBox21.Enabled = false;
            comboBox31.Enabled = false;

            comboBox14.Items.Add("请求");
            comboBox24.Items.Add("请求");
            comboBox34.Items.Add("请求");
            comboBox14.Enabled = false;
            comboBox24.Enabled = false;
            comboBox34.Enabled = false;

            comboBox15.Items.Add("01");
            comboBox25.Items.Add("01");
            comboBox35.Items.Add("01");
            comboBox15.Enabled = false;
            comboBox25.Enabled = false;
            comboBox35.Enabled = false;

            comboBox16.Items.Add("01");
            comboBox26.Items.Add("01");
            comboBox36.Items.Add("01");
            comboBox16.Enabled = false;
            comboBox26.Enabled = false;
            comboBox36.Enabled = false;

            comboBox18.Items.Add("正常");
            comboBox28.Items.Add("正常");
            comboBox38.Items.Add("正常");
            comboBox18.Enabled = false;
            comboBox28.Enabled = false;
            comboBox38.Enabled = false;

            comboBox12.Items.Add("请选择");
            comboBox12.Items.Add("入库");
            comboBox12.Items.Add("出库");

            comboBox22.Items.Add("请选择");
            comboBox22.Items.Add("入库");
            comboBox22.Items.Add("出库");

            comboBox32.Items.Add("请选择");
            comboBox32.Items.Add("退库");
            comboBox32.Items.Add("出库");


            comboBox13.Items.Add("请选择");
            comboBox23.Items.Add("请选择");
            comboBox33.Items.Add("请选择");

            comboBox17.Items.Add("请选择");
            comboBox27.Items.Add("请选择");
            comboBox37.Items.Add("请选择");

            SetComboBox(this);
        }



        private void SetComboBox(Control ctl)
        {
            foreach (Control c in ctl.Controls)//遍历本窗体所ComboBox控件
            {
                ComboBox cmbx = c as ComboBox;
                if (cmbx != null)
                {
                    if(cmbx.Items.Count>0)
                    cmbx.SelectedIndex = 0;
                }
                if(c.Controls.Count > 0)
                SetComboBox(c);

            }
        }
        //A1动做选定时
        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox12.Text== "入库")
            {
                comboBox13.Items.Clear();
                comboBox17.Items.Clear();
                comboBox16.Items.Clear();
                comboBox13.Items.Add("请选择");
                comboBox13.Items.Add("单入库");
                comboBox13.Items.Add("入库自动出空卷");
                comboBox17.Items.Add("满卷");
                comboBox16.Items.Add("01");
                comboBox13.SelectedIndex = 0;
                comboBox17.SelectedIndex = 0;
                comboBox16.SelectedIndex = 0;
            }
            else if (comboBox12.Text == "出库")
            {
                comboBox13.Items.Clear();
                comboBox17.Items.Clear();
                comboBox16.Items.Clear();
                comboBox13.Items.Add("单出库");
                comboBox16.Items.Add("卷筒");
                comboBox17.Items.Add("空卷");
                comboBox13.SelectedIndex = 0;
                comboBox17.SelectedIndex = 0;
                comboBox16.SelectedIndex = 0;
               
            }
            else
            {
                comboBox13.Items.Clear();
                comboBox17.Items.Clear();
                comboBox16.Items.Clear();
                comboBox13.Items.Add("请选择");
                comboBox16.Items.Add("请选择");
                comboBox17.Items.Add("请选择");
                comboBox13.SelectedIndex = 0;
                comboBox17.SelectedIndex = 0;
                comboBox16.SelectedIndex = 0;
            }


        }

        private void comboBox22_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox22.Text == "入库")
            {
                comboBox23.Items.Clear();
                comboBox27.Items.Clear();
                comboBox26.Items.Clear();
                comboBox23.Items.Add("请选择");
                comboBox23.Items.Add("单入库");
                comboBox23.Items.Add("入库自动出空卷");
                comboBox27.Items.Add("满卷");
                comboBox26.Items.Add("01");
                comboBox23.SelectedIndex = 0;
                comboBox27.SelectedIndex = 0;
                comboBox26.SelectedIndex = 0;
            }
            else if (comboBox22.Text == "出库")
            {
                comboBox23.Items.Clear();
                comboBox27.Items.Clear();
                comboBox26.Items.Clear();
                comboBox23.Items.Add("单出库");
                comboBox26.Items.Add("卷筒");
                comboBox27.Items.Add("空卷");
                comboBox23.SelectedIndex = 0;
                comboBox27.SelectedIndex = 0;
                comboBox26.SelectedIndex = 0;

            }
            else
            {
                comboBox23.Items.Clear();
                comboBox27.Items.Clear();
                comboBox26.Items.Clear();
                comboBox23.Items.Add("请选择");
                comboBox26.Items.Add("请选择");
                comboBox27.Items.Add("请选择");
                comboBox23.SelectedIndex = 0;
                comboBox27.SelectedIndex = 0;
                comboBox26.SelectedIndex = 0;
            }
        }
        //B区指令动作选定
        private void comboBox32_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox32.Text == "退库")
            {
                comboBox33.Items.Clear();
                comboBox37.Items.Clear();
                comboBox36.Items.Clear();
                comboBox33.Items.Add("请选择");
                comboBox33.Items.Add("单退库");
                comboBox33.Items.Add("退库自动出满卷");
                comboBox37.Items.Add("空卷");
                comboBox36.Items.Add("卷筒");
                comboBox33.SelectedIndex = 0;
                comboBox37.SelectedIndex = 0;
                comboBox36.SelectedIndex = 0;
            }
            else if (comboBox32.Text == "出库")
            {
                comboBox33.Items.Clear();
                comboBox37.Items.Clear();
                comboBox36.Items.Clear();
                comboBox33.Items.Add("单出库");
                comboBox36.Items.Add("01");
                comboBox37.Items.Add("满卷");
                comboBox33.SelectedIndex = 0;
                comboBox37.SelectedIndex = 0;
                comboBox36.SelectedIndex = 0;

            }
            else
            {
                comboBox33.Items.Clear();
                comboBox37.Items.Clear();
                comboBox36.Items.Clear();
                comboBox33.Items.Add("请选择");
                comboBox36.Items.Add("请选择");
                comboBox37.Items.Add("请选择");
                comboBox33.SelectedIndex = 0;
                comboBox37.SelectedIndex = 0;
                comboBox36.SelectedIndex = 0;
            }


        }

        //A1指令确认时
        private void button1_Click(object sender, EventArgs e)
        {
            byte[] orderbuff = new byte[10];
            if (comboBox12.Text != "请选择" && comboBox13.Text != "请选择" && comboBox16.Text != "请选择" && comboBox17.Text != "请选择")
            {
                orderbuff[0] = 0x01;
                if (comboBox12.Text == "入库")
                {
                    orderbuff[2] = 0x02;
                }
                else
                {
                    orderbuff[2] = 0x01;
                }
                if (comboBox13.Text == "入库自动出空卷")
                {
                    orderbuff[1] = 0x02;
                }
                else
                {
                    orderbuff[1] = 0x01;
                }
                orderbuff[3] = 0x01;
                orderbuff[4] = 0x01;
                if (comboBox16.Text == "卷筒")
                {
                    orderbuff[5] = 0x99;
                }
                else
                {
                    orderbuff[5] = 0x01;
                }
                if (comboBox17.Text == "空卷")
                {
                    orderbuff[6] = 0x99;
                }
                else if(comboBox17.Text == "满卷")
                {
                    orderbuff[6] = 0x02;
                }
                orderbuff[7] = 0x01;
                orderbuff[8] = 0x00;
                orderbuff[9] = 0x00;
                //转换成字符串出掉末尾的\0\0\0
                String d = frmNewMain.byteToHexStr(orderbuff).TrimEnd('\0');
                //缓存中无相应工位的指令，且主队列中也没有相同的指令（避免同一位置下的重复指令）先将对应区域的指令放进字典中
                if (!frmNewMain.orderhas.ContainsKey(orderbuff[0].ToString("00")) && !frmNewMain.dataListrequest.Contains(d) && frmNewMain.startExchageLib==false)
                {
                    lock (locker)
                    {
                        frmNewMain.orderhas.Add(orderbuff[0].ToString("00"), d);
                    }
                    frmNewMain.bread = true;
                }

            }

        }
        //A2指令确认时
        private void button2_Click(object sender, EventArgs e)
        {
            byte[] orderbuff = new byte[10];
            if (comboBox22.Text != "请选择" && comboBox23.Text != "请选择" && comboBox26.Text != "请选择" && comboBox27.Text != "请选择")
            {
                orderbuff[0] = 0x02;
                if (comboBox22.Text == "入库")
                {
                    orderbuff[2] = 0x02;
                }
                else
                {
                    orderbuff[2] = 0x01;
                }
                if (comboBox23.Text == "入库自动出空卷")
                {
                    orderbuff[1] = 0x02;
                }
                else
                {
                    orderbuff[1] = 0x01;
                }
                orderbuff[3] = 0x01;
                orderbuff[4] = 0x01;
                if (comboBox26.Text == "卷筒")
                {
                    orderbuff[5] = 0x99;
                }
                else
                {
                    orderbuff[5] = 0x01;
                }
                if (comboBox27.Text == "空卷")
                {
                    orderbuff[6] = 0x99;
                }
                else if (comboBox27.Text == "满卷")
                {
                    orderbuff[6] = 0x02;
                }
                orderbuff[7] = 0x01;
                orderbuff[8] = 0x00;
                orderbuff[9] = 0x00;
                //转换成字符串出掉末尾的\0\0\0
                String d = frmNewMain.byteToHexStr(orderbuff).TrimEnd('\0');
                //缓存中无相应工位的指令，且主队列中也没有相同的指令（避免同一位置下的重复指令）先将对应区域的指令放进字典中
                if (!frmNewMain.orderhas.ContainsKey(orderbuff[0].ToString("00")) && !frmNewMain.dataListrequest.Contains(d) && frmNewMain.startExchageLib == false)
                {
                    lock (locker)
                    {
                        frmNewMain.orderhas.Add(orderbuff[0].ToString("00"), d);
                    }
                    frmNewMain.bread = true;
                }
              }
            }

        private void button3_Click(object sender, EventArgs e)
        {
            byte[] orderbuff = new byte[10];
            if (comboBox32.Text != "请选择" && comboBox33.Text != "请选择" && comboBox36.Text != "请选择" && comboBox37.Text != "请选择")
            {
                orderbuff[0] = 0x04;
                orderbuff[1] = 0x01;
                if (comboBox32.Text == "退库")
                {
                    orderbuff[2] = 0x04;
                }
                else
                {
                    orderbuff[2] = 0x03;
                }
                if (comboBox33.Text == "退库自动出满卷")
                {
                    orderbuff[1] = 0x02;
                }
                else
                {
                    orderbuff[1] = 0x01;
                }
                orderbuff[3] = 0x01;
                orderbuff[4] = 0x01;
                if (comboBox36.Text == "卷筒")
                {
                    orderbuff[5] = 0x99;
                }
                else
                {
                    orderbuff[5] = 0x01;
                }
                if (comboBox37.Text == "空卷")
                {
                    orderbuff[6] = 0x99;
                }
                else if (comboBox37.Text == "满卷")
                {
                    orderbuff[6] = 0x02;
                }
                orderbuff[7] = 0x01;
                orderbuff[8] = 0x00;
                orderbuff[9] = 0x00;
                //转换成字符串出掉末尾的\0\0\0
                String d = frmNewMain.byteToHexStr(orderbuff).TrimEnd('\0');
                //缓存中无相应工位的指令，且主队列中也没有相同的指令（避免同一位置下的重复指令）先将对应区域的指令放进字典中
                if (!frmNewMain.orderhas.ContainsKey(orderbuff[0].ToString("00")) && !frmNewMain.dataListrequest.Contains(d) && frmNewMain.startExchageLib == false)
                {
                    lock (locker)
                    {
                        frmNewMain.orderhas.Add(orderbuff[0].ToString("00"), d);
                    }
                    frmNewMain.bread = true;
                }
            }
        }
    }
}