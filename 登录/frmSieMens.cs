using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.BasicFramework;
using HslCommunication.Controls;
using HslCommunication.Core;
using HslCommunication.Profinet.Siemens;
using HslCommunication.Enthernet;
using HslCommunication.LogNet;
using Newtonsoft.Json.Linq;
using HslCommunication.Core.Net;
using System.Threading;
using System.Collections;

namespace 风帆电池
{
    public partial class frmSieMens : FormBase
    {
        public frmSieMens()
        {
            InitializeComponent();

            FormExIni();
            _systemButtonManager = new SystemButtonManager(this);

        }


        /// <summary>
        /// 读取PLC指定寄存器的值，结果回显到界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">读取PLC指定寄存器指定数据类型的结果</param>
        /// <param name="address">PLC指定寄存器</param>
        /// <param name="textBox">界面显示控件textbox</param>
        private void readResultRender<T>(OperateResult<T> result, string address, TextBox textBox)
        {
            bool isSuccess = result.IsSuccess;
            if (isSuccess)
            {
                textBox.AppendText(DateTime.Now.ToString("[HH:mm:ss] ") + string.Format("[{0}] {1}{2}", address, result.Content, Environment.NewLine));
            }
            else
            {
                MessageBox.Show(DateTime.Now.ToString("[HH:mm:ss] ") + string.Format("[{0}] 读取失败{1}原因：{2}", address, Environment.NewLine, result.ToMessageShowString()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="address"></param>
        private void writeResultRender(OperateResult result, string address)
        {
            bool isSuccess = result.IsSuccess;
            if (isSuccess)
            {
                MessageBox.Show(DateTime.Now.ToString("[HH:mm:ss] ") + string.Format("[{0}] 写入成功", address));
            }
            else
            {
                MessageBox.Show(DateTime.Now.ToString("[HH:mm:ss] ") + string.Format("[{0}] 写入失败{1}原因：{2}", address, Environment.NewLine, result.ToMessageShowString()));
            }
        }


        // 只是用来显示连接失败的错误信息
        private void ShowFailedMessage(int failed, string strplc)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int, string>(ShowFailedMessage), failed, strplc);
                return;
            }
            if (failed == 0)
            {
                textBox4.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + strplc + "连接成功！" + Environment.NewLine);
            }
            else
                textBox4.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + strplc + "第" + failed + "次读取失败！" + Environment.NewLine);
        }

        //主窗体事件处理方法
        public void ShowReadContent(object sender, EventArgs e)
        {
            if (IsDisposed)
                return;

            //拿到父窗体的传来的文本
            MyEventArg arg = e as MyEventArg;
            //PLC1的请求指令区10字节
            byte[] arry1 = new byte[10];
            //PLC2的写入分步指令区10字节
            byte[] arry2 = new byte[10];

            if (arg.strplc1 == "plc1" && arg.buffer1.Count > 0)
            {

                int waringflag = arg.buffer1[20];
                Array.Copy(arg.buffer1.ToArray(), 0, arry1, 0, 10);
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    textBox10.Text = "HMI指令区：" + SoftBasic.ByteToHexString(arry1, ' ') + "\r\n" +
                "A1工位指令撤销状态：" + arg.buffer1[10].ToString() + "\r\n" +
                "A2工位指令撤销状态：" + arg.buffer1[11].ToString() + "\r\n" +
                "抽检工位指令撤销状态：" + arg.buffer1[12].ToString() + "\r\n" +
                "B工位指令撤销状态：" + arg.buffer1[13].ToString() + "\r\n" +
                "地面PLC报警状态：" + arg.buffer1[20].ToString() + "\r\n" +
                "夹具状态不对应报警：" + arg.buffer1[21].ToString() + "\r\n" +
                "退库自动出库物料规格：" + arg.buffer1[14].ToString() + "\r\n" +
                "退库自动出库物料状态：" + arg.buffer1[15].ToString() + "\r\n";

                }));
            }

            if (arg.strplc2 == "plc2" && arg.buffer2.Count > 0)
            {
                //Array.Copy(arg.buffer2.ToArray(), 10, arry2, 0, 10);


                //int WX = (int)(arg.buffer2[30] * 256) + arg.buffer2[31];
                //int WY = (int)(arg.buffer2[32] * 256) + arg.buffer2[33];
                //int WZ = (int)(arg.buffer2[34] * 256) + arg.buffer2[35];
                ////int startflag = arry[36];
                ////int actionflag = arry[37];

                //int RX = (int)(arg.buffer2[40] * 256) + arg.buffer2[41];
                //int RY = (int)(arg.buffer2[42] * 256) + arg.buffer2[43];
                //int RZ = (int)(arg.buffer2[44] * 256) + arg.buffer2[45];
                //string s1 = S71KConnect.Success_flag == true ? "true" : "false";
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    textBox6.Text ="实时坐标：" + "  X坐标：" + arg.userdata.PLC_Current_X.ToString() + "  Y坐标：" + arg.userdata.PLC_Current_Y.ToString() + "  Z坐标：" + arg.userdata.PLC_Current_Z.ToString() + "\r\n"+
                            "X到位标志：" + arg.userdata.X_Arrive_Signal.ToString() + "  Y到位标志：" + arg.userdata.Y_Arrive_Signal.ToString() + "  Z到位标志：" + arg.userdata.Z_Arrive_Signal.ToString() + "\r\n" +
                            "夹具开闭状态：" + arg.userdata.Clamp_State.ToString() + "  写入动作回读：" + arg.userdata.ReadBack_Catch_Release_Enable_Signal.ToString() + "\r\n" +
                            "新坐标写入标志：" + arg.userdata.ReadBack_Write_New_Flag.ToString() + "\r\n" +
                            "目的坐标：" + "  X坐标：" + arg.userdata.ReadBack_Dest_X.ToString() + "  Y坐标：" + arg.userdata.ReadBack_Dest_Y.ToString() + "  Z坐标：" + arg.userdata.ReadBack_Dest_Z.ToString() + "\r\n"+
                            "动作值：" + frmNewMain.Write_Catch_Release.ToString() + "\r\n" +
                            "需要写入的值：" + "  X坐标：" + frmNewMain.Write_Dest_X.ToString() + "  Y坐标：" + frmNewMain.Write_Dest_Y.ToString() + "  Z坐标：" + frmNewMain.Write_Dest_Z.ToString() + "\r\n" +
                            "初始到位信号：" + arg.buffer2[14].ToString() + "抱闸信号：" + arg.buffer2[15].ToString() + "开始启动信号：" + arg.buffer2[16].ToString() + "\r\n";
                }));

            }

            this.BeginInvoke(new MethodInvoker(delegate
            {
                if (arg.failed1 == 0)
                textBox4.Text = "地面PLC读取成功";
            else
                textBox4.Text = "地面PLC读取失败";
            if (arg.failed2 == 0)
                textBox5.Text = "执行PLC读取成功";
            else
                textBox5.Text = "执行PLC读取失败";
            }));

        }


        // 读取成功时，显示结果数据
        private void ShowReadContent(OperateResult<byte[]> result, string strplc)
        {
            // 本方法是考虑了后台线程调用的情况
            if (InvokeRequired)
            {
                // 如果是后台调用显示UI，那么就使用委托来切换到前台显示
                Invoke(new Action<OperateResult<byte[]>, string>(ShowReadContent), result, strplc);
                return;
            }
            byte[] arry = result.Content;
            //PLC1的请求指令区10字节
            byte[] arry1 = new byte[10];
            //PLC2的写入分步指令区10字节
            byte[] arry2 = new byte[10];

            if (strplc == "PLC1")
            {
                int waringflag = arry[20];
                Array.Copy(arry, 0, arry1, 0, 10);
                textBox10.Text = "读指令区：" + SoftBasic.ByteToHexString(arry1, ' ') + "\r\n" +
                "A1工位指令撤销状态：" + arry[10].ToString() + "\r\n" +
                "A2工位指令撤销状态：" + arry[11].ToString() + "\r\n" +
                "抽检工位指令撤销状态：" + arry[12].ToString() + "\r\n" +
                "B工位指令撤销状态：" + arry[13].ToString() + "\r\n" +
                "地面PLC报警状态：" + arry[20].ToString() + "\r\n" +
                "夹具状态不对应报警：" + arry[21].ToString() + "\r\n" +
                "退库自动出库物料规格：" + arry[14].ToString() + "\r\n" +
                "退库自动出库物料状态：" + arry[15].ToString() + "\r\n";
            }
            if (strplc == "PLC2")
            {
                Array.Copy(arry, 10, arry2, 0, 10);
                //Array.Copy(arry, 40, arry3, 0, 10);


                int WX = (int)(arry[30] * 256) + arry[31];
                int WY = (int)(arry[32] * 256) + arry[33];
                int WZ = (int)(arry[34] * 256) + arry[35];
                //int startflag = arry[36];
                //int actionflag = arry[37];

                int RX = (int)(arry[40] * 256) + arry[41];
                int RY = (int)(arry[42] * 256) + arry[43];
                int RZ = (int)(arry[44] * 256) + arry[45];
                textBox6.Text = "写指令区：" + SoftBasic.ByteToHexString(arry2, ' ') + "\r\n" +
                            "夹具状态：" + arry[28].ToString() + "\r\n" +
                            "新写入坐标标志：" + arry[29].ToString() + "\r\n" +
                            "写入坐标：" + "X坐标：" + WX.ToString() + "  Y坐标：" + WY.ToString() + "  Z坐标：" + WZ.ToString() + "\r\n" +
                            "启动信号：" + arry[36].ToString() + "  抓放动作：" + arry[37].ToString() + "  区域标签：" + arry[38].ToString() + "\r\n" +
                            "请求指令工位来源：" + arry[39].ToString() + "\r\n" +
                            "实时坐标：" + "X坐标：" + RX.ToString() + "  Y坐标：" + RY.ToString() + "  Z坐标：" + RZ.ToString() + "\r\n" +
                            "PLC X,Y到位标志：" + arry[46].ToString() + "\r\n" +
                            "抓放完成标志：" + arry[47].ToString() + " " + "  允许下降标志：" + arry[48].ToString() + "\r\n";

            }

        }

        public class MyEventArg : EventArgs
        {
            //传递主窗体的数据信息
            public List<byte> buffer1;
            public string strplc1;
            public int failed1;

            public List<byte> buffer2;
            public string strplc2;
            public int failed2;

            public UserType userdata;
        }








        #region 定时器块


        /*********************************************************************************************
         * 
         *    功能说明：
         *    定时器块实现的功能是当连续3次读取PLC数据失败时，就将窗口进行闪烁。
         *    重新连接上时，就显示信号成功。
         * 
         *********************************************************************************************/


        private System.Windows.Forms.Timer timer = null;
        private bool m_isRedBackColor = false;

        private void TimerInitialization()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //// 每秒执行的代码
            //if (failed1 > 3)
            //{
            //    // 交替闪烁界面
            //    m_isRedBackColor = !m_isRedBackColor;
            //    if (m_isRedBackColor)
            //    {
            //        BackColor = Color.Tomato;
            //    }
            //    else
            //    {
            //        BackColor = SystemColors.Control;
            //    }
            //}
            //else
            //{
            //    // 复原颜色
            //    BackColor = SystemColors.Control;
            //    m_isRedBackColor = false;
            //}


        }

        #endregion


    }

}

