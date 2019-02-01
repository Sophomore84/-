using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace  风帆电池
{
    public partial class frmModBusRtu : FormBase
    {

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        private bool bInitial = false;
        private bool bWriteINI = false;

        List<byte> buffer = new List<byte>(4096);
        

        public frmModBusRtu()
        {
            InitializeComponent();
            FormExIni();
            _systemButtonManager = new SystemButtonManager(this);
        }

        

        private void frmModBusRtu_Load(object sender, EventArgs e)
        {
            cbSerialPort.DropDownStyle = ComboBoxStyle.DropDown;
            //自己手动添加串口号
            cbSerialPort.Items.Add("COM1");
            cbSerialPort.Items.Add("COM2");
            cbSerialPort.Items.Add("COM3");
            cbSerialPort.Items.Add("COM4");
            cbSerialPort.Items.Add("COM5");
            cbSerialPort.Items.Add("COM6");
            cbSerialPort.Items.Add("COM7");
            cbSerialPort.Items.Add("COM8");

            cbBaudrate.DropDownStyle = ComboBoxStyle.DropDown;
            cbBaudrate.Items.Add("9600");
            cbBaudrate.Items.Add("14400");
            cbBaudrate.Items.Add("19200");
            cbBaudrate.Items.Add("38400");
            cbBaudrate.Items.Add("56000");
            cbBaudrate.Items.Add("57600");
            cbBaudrate.Items.Add("115200");
            cbBaudrate.Items.Add("128000");

            cbDatabit.DropDownStyle = ComboBoxStyle.DropDown;
            cbDatabit.Items.Add("5");
            cbDatabit.Items.Add("6");
            cbDatabit.Items.Add("7");
            cbDatabit.Items.Add("8");

            cbStopbit.DropDownStyle = ComboBoxStyle.DropDown;
            cbStopbit.Items.Add("1");
            cbStopbit.Items.Add("1.5");
            cbStopbit.Items.Add("2");

            cbTimeout.DropDownStyle = ComboBoxStyle.DropDown;
            cbTimeout.Items.Add("300");
            cbTimeout.Items.Add("500");
            cbTimeout.Items.Add("700");
            cbTimeout.Items.Add("1000");
            cbTimeout.Items.Add("1500");

            ReadStringINI();
            bInitial = true;

            
            
            btnClose.Enabled = false;
           
        }

        //选用串口号改变事件
        private void cbSerialPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bInitial)
            {
                bWriteINI = true;
            }
        }

        //读取INI文件
        private void ReadStringINI()
        {
            StringBuilder temp = new StringBuilder();
            GetPrivateProfileString("SerialPort", "SerialPort", "读取异常", temp, 255, Application.StartupPath + "\\config.ini");
            cbSerialPort.Text = temp.ToString();
            GetPrivateProfileString("SerialPort", "Baudrate", "读取异常", temp, 255, Application.StartupPath + "\\config.ini");
            cbBaudrate.Text = temp.ToString();
            GetPrivateProfileString("SerialPort", "Databit", "读取异常", temp, 255, Application.StartupPath + "\\config.ini");
            cbDatabit.Text = temp.ToString();
            GetPrivateProfileString("SerialPort", "Stopbit", "读取异常", temp, 255, Application.StartupPath + "\\config.ini");
            cbStopbit.Text = temp.ToString();
            GetPrivateProfileString("SerialPort", "Timeout", "读取异常", temp, 255, Application.StartupPath + "\\config.ini");
            cbTimeout.Text = temp.ToString();

        }

        private void WriteStringINI()
        {
            //将选中串口号写入文档中
            WritePrivateProfileString("SerialPort", "SerialPort", cbSerialPort.Text, Application.StartupPath + "\\config.ini");
            WritePrivateProfileString("SerialPort", "Baudrate", cbBaudrate.Text, Application.StartupPath + "\\config.ini");
            WritePrivateProfileString("SerialPort", "Databit", cbDatabit.Text, Application.StartupPath + "\\config.ini");
            WritePrivateProfileString("SerialPort", "Stopbit", cbStopbit.Text, Application.StartupPath + "\\config.ini");
            WritePrivateProfileString("SerialPort", "Timeout", cbTimeout.Text, Application.StartupPath + "\\config.ini");

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (!frmNewMain.bOpening)
                {
                    if (!(frmNewMain.sp.IsOpen))
                    {

                        frmNewMain.sp.PortName = cbSerialPort.Text;
                        frmNewMain.sp.BaudRate = Convert.ToInt32(cbBaudrate.Text);
                        frmNewMain.sp.DataBits = Convert.ToInt32(cbDatabit.Text);
                        frmNewMain.sp.StopBits = (StopBits)Convert.ToInt32(cbStopbit.Text);
                        frmNewMain.sp.ReadTimeout = Convert.ToInt32(cbTimeout.Text);
                        frmNewMain.sp.Open();
                        lblState.Text = "串口成功打开";
                        //串口通讯打开
                        frmNewMain.bOpening = true;
                        if (bWriteINI) WriteStringINI();
                        bWriteINI = false;
                        btnClose.Enabled = true;
                        btnOpen.Enabled = false;
                        //打开串口后串口号选择框使能为false
                        cbSerialOpen();

                    }
                    else
                        frmNewMain.sp.Close();
                }
                else
                {
                    MessageBox.Show("请先断开其他通信连接！");
                }
            }
            catch (Exception ex)
            {
                frmNewMain.sp.Close();
                lblState.Text = "发生错误，串口未成功打开";
                frmNewMain.bOpening = false;
                MessageBox.Show(ex.Message);
                return;
            }
        }
        //关闭串口
        private void btnClose_Click(object sender, EventArgs e)
        {
            //消息框中需要显示哪些按钮，此处显示“确定”和“取消”

            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;

            //"确定要退出吗？"是对话框的显示信息，"退出系统"是对话框的标题

            //默认情况下，如MessageBox.Show("确定要退出吗？")只显示一个“确定”按钮。
            DialogResult dr = MessageBox.Show("确定要关闭串口吗?", "关闭串口", messButton);

            if (dr == DialogResult.OK)//如果点击“确定”按钮
            {
                btnOpen.Enabled = true;
                btnClose.Enabled = false;
                frmNewMain.sp.Close();
                lblState.Text = "串口已经关闭";
                frmNewMain.bOpening = false;
                cbSerialClose();
            }
            else
            {
                return;
            }
        }
        //打开串口时复选框不可用
        private void cbSerialOpen()
        {
            cbSerialPort.Enabled = false;
            cbBaudrate.Enabled = false;
            cbDatabit.Enabled = false;
            cbStopbit.Enabled = false;
            cbTimeout.Enabled = false;
        }
        //关闭串口时复选框可用
        private void cbSerialClose()
        {
            cbSerialPort.Enabled = true;
            cbBaudrate.Enabled = true;
            cbDatabit.Enabled = true;
            cbStopbit.Enabled = true;
            cbTimeout.Enabled = true;
        }

        private void frmModBusRtu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;    //取消"关闭窗口"事件
                this.Hide();
                return;
            }
        }
    }
}
