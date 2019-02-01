using HslCommunication;
using HslCommunication.LogNet;
using HslCommunication.Profinet.Siemens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static 风帆电池.frmNewMain;
using static 风帆电池.frmSieMens;

namespace 风帆电池
{
    //定义数据类型枚举
    public enum DataType
    {
        Bool,
        Byte,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Float,
        Int64,
        UInt64,
        Double,
        String
    }
    public class S71KConnect
    {
        public static UserType Usertype = new UserType();
        public static SiemensS7Net siemensTcpNet1;
        public static bool PlcisSuccess1;
        private bool isReadingPlc1 = false;         // 是否启动的标志，可以用来暂停项目
        private int failed1 = 0;                    // 连续失败此处，连续三次失败就报警
        private Thread threadReadPlc1 = null;
        public static bool readPLC1Success;

        public static SiemensS7Net siemensTcpNet2_Read;
        public static bool PlcisSuccess2_Read;
        private bool isReadingPlc2_Read = false;         // 是否启动的标志，可以用来暂停项目
        private int failed2_Read = 0;                    // 连续失败此处，连续三次失败就报警
        private Thread threadReadPlc2 = null;
        public static bool readPLC2Success;





        public static SiemensS7Net siemensTcpNet2_Write;
        public static bool PlcisSuccess2_Write;
       /* private bool isReadingPlc2_Write = false; */        // 是否启动的标志，可以用来暂停项目
       /* private int failed2_Write = 0;*/                    // 连续失败此处，连续三次失败就报警
        //private Thread threadReadPlc2 = null;
        //public static bool readPLC2Success;


        public static bool Success_flag = false;
        //public static Dictionary<string, string> orderhas = new Dictionary<string, string>();
        //此处将字典 Dictionary<string, string>改为 SortedList排序字典
        //当元素增加或移除时索引会自动改变


        List<byte> buffer1 = new List<byte>(100);
        List<byte> buffer2 = new List<byte>(100);

        //同步锁
        private object locker = new object();

        public int old_orderhas_count = 0;
        public static string autoGoodscode;
        public static string autoGoodssatate;

        private bool threadstart_plc1 = false;
        private bool threadstart_plc2 = false;

        #region 方法（推荐）--事件方式
        //增加event关键字
        //定 义消息发布的事件  事件是委托的一个特殊实例  事件只能在类的内部触发执行
        public event EventHandler SendMsgEvent; //使用默认的事件处理委托
        #endregion


        public static bool Start_system_flag = false;


        public void PLCInit()
        {
            LogNet = new LogNetDateTime(Application.StartupPath + "\\Logs\\通讯异常", GenerateMode.ByEveryDay); // 创建日志器，按每天存储不同的文件
            LogNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;              // 设置存储日志前的一些额外操作
            //天车A的PLC（本车PLC）
            //siemensTcpNet1 =new SiemensS7Net(SiemensPLCS.S1200, "192.168.0.4") { ConnectTimeOut = 1000 }
            siemensTcpNet1 = new SiemensS7Net(SiemensPLCS.S1500, "192.168.0.4") { ConnectTimeOut = 1000 };
            //天车B的PLC（邻车PLC）
            siemensTcpNet2_Read = new SiemensS7Net(SiemensPLCS.S1500, "192.168.0.1") { ConnectTimeOut = 1000 };

            siemensTcpNet2_Write = new SiemensS7Net(SiemensPLCS.S1500, "192.168.0.1") { ConnectTimeOut = 1000 };


            siemensTcpNet1.LogNet = LogNet;       // 设置统一的日志记录器
            siemensTcpNet2_Read.LogNet = LogNet;
            siemensTcpNet2_Read.LogNet = LogNet;
            // 启动后台读取的线程
            threadReadPlc1 = new Thread(new System.Threading.ThreadStart(ThreadBackgroundReadPlc1));
            threadReadPlc1.IsBackground = true;
            threadReadPlc1.Priority = ThreadPriority.AboveNormal;

            siemensTcpNet2_Read.LogNet = LogNet;       // 设置统一的日志记录器
            siemensTcpNet2_Read.ConnectTimeOut = 1000;
            // 启动后台读取的线程
            threadReadPlc2 = new Thread(new System.Threading.ThreadStart(ThreadBackgroundReadPlc2));
            threadReadPlc2.IsBackground = true;
            threadReadPlc2.Priority = ThreadPriority.AboveNormal;
            //建立PLC1读取长连接
            PlcisSuccess1 = ConnectPLC1();
            //建立PLC2读取长连接
            PlcisSuccess2_Read = ConnectPLC2();
            
            //建立PLC2写长连接
            PlcisSuccess2_Write = ConnectPLC2_Write();
            //开启线程

            threadstart_plc1 = StartPLC1Read();
            //开启线程
            threadstart_plc2 = StartPLC2Read();
            //PLC连接成功后先给夹具一个张开状态
        }
        /// <summary>
        /// 长连接(调用此方法就是使用了长连接，如果不调用直接读取数据，那就是短连接)
        /// </summary>
        /// <returns></returns>
        private static bool ConnectPLC1()
        {
            OperateResult connect = siemensTcpNet1.ConnectServer();
            if (connect.IsSuccess) return true;
            else return false;
        }
        /// <summary>
        /// 断开连接，也就是关闭了长连接，如果再去请求数据，就变成了短连接
        /// </summary>
        private static void DisconnectPLC1()
        {
            siemensTcpNet1.ConnectClose();
        }
        /// <summary>
        /// 长连接(调用此方法就是使用了长连接，如果不调用直接读取数据，那就是短连接)
        /// </summary>
        /// <returns></returns>
        private bool ConnectPLC2()
        {
            OperateResult connect = siemensTcpNet2_Read.ConnectServer();
            if (connect.IsSuccess) return true;
            else return false;
        }
        /// <summary>
        /// 断开连接，也就是关闭了长连接，如果再去请求数据，就变成了短连接
        /// </summary>
        private void DisconnectPLC2()
        {
            siemensTcpNet2_Read.ConnectClose();
        }


        /// <summary>
        /// 长连接(调用此方法就是使用了长连接，如果不调用直接读取数据，那就是短连接)
        /// </summary>
        /// <returns></returns>
        private bool ConnectPLC2_Write()
        {
            OperateResult connect = siemensTcpNet2_Write.ConnectServer();
            if (connect.IsSuccess) return true;
            else return false;
        }
        /// <summary>
        /// 断开连接，也就是关闭了长连接，如果再去请求数据，就变成了短连接
        /// </summary>
        private void DisconnectPLC2_Write()
        {
            siemensTcpNet2_Write.ConnectClose();
        }






        /// <summary>
        /// 启动读取PLC数据线程
        /// </summary>
        private bool StartPLC1Read()
        {
            // 启动后台读取的线程
            threadReadPlc1.Start();
            return true;
        }

        /// <summary>
        /// 停止读取PLC数据线程
        /// </summary>
        private void StopPLC1Read()
        {
            // 结束后台读取的线程
            threadReadPlc1.Abort();
        }

        /// <summary>
        /// 启动读取PLC数据线程
        /// </summary>
        private bool StartPLC2Read()
        {
            // 启动后台读取的线程
            threadReadPlc2.Start();
            return true;
        }

        /// <summary>
        /// 停止读取PLC数据线程
        /// </summary>
        private void StopPLC2Read()
        {
            // 结束后台读取的线程
            threadReadPlc2.Abort();
        }


        //读取PLC1 地面PLC的线程
        private void ThreadBackgroundReadPlc1()
        {
            // 此处假设我们读取的是西门子PLC的数据，其实三菱的数据读取原理是一样的，可以仿照西门子的开发
            while (true)
            {
                frmNewMain.bListenPlc1 = true;
                HslCommunication.OperateResult<JObject> read = null;
                // 这里仅仅演示了西门子的数据读取
                HslCommunication.OperateResult<byte[]> buff = siemensTcpNet1.Read("M500", 50);
                ////读取字符串数据
                //HslCommunication.OperateResult<string> strbuff = siemensTcpNet1.ReadString("M522", 10);

                bool isSuccess = buff.IsSuccess;
                if (isSuccess)
                {
                    //首先返回一个读取成功的对象
                    read = HslCommunication.OperateResult.CreateSuccessResult(new JObject()
                    {
                    });
                    //访问成功
                    readPLC1Success = read.IsSuccess;
                    //集合先清空
                    buffer1.Clear();
                    //读取回来的数据显示到txtbox
                    //ShowReadContent(buff, "PLC1");

                    buffer1.AddRange(buff.Content);//将指定集合的元素添加到集合buffer的末尾
                    if (buffer1[3] == 0x01 && buffer1[0] != 0x00 && frmNewMain.startExchageLib == false)//有动作请求指令且没有倒库指令时
                    {
                        byte[] data = new byte[10];
                        buffer1.CopyTo(0, data, 0, 10);
                        //转换成字符串出掉末尾的\0\0\0
                        String d = frmNewMain.byteToHexStr(data).TrimEnd('\0');
                        //缓存中无相应工位的指令，且主队列中也没有相同的指令（避免同一位置下的重复指令）先将对应区域的指令放进字典中
                        if (!frmNewMain.orderhas.ContainsKey(buffer1[0].ToString("00")) && !frmNewMain.dataListrequest.Contains(d))
                        {
                            lock (locker)
                            {
                                frmNewMain.orderhas.Add(buffer1[0].ToString("00"), d);
                            }
                            frmNewMain.bread = true;
                            //将PLC请求指令区清零
                            byte[] buffWrite_clear = new byte[10];
                            OperateResult result1 = siemensTcpNet1.Write("M500", buffWrite_clear);

                        }
                        else //直接将PLC请求指令区清零
                        {
                            byte[] buffWrite_clear = new byte[10];
                            OperateResult result1 = siemensTcpNet1.Write("M500", buffWrite_clear);
                        }

                        //退库自动出卷时需要判断MB514，MB515
                        if (buffer1[14] != 0x00) autoGoodscode = buffer1[14].ToString("00");
                        if (buffer1[15] == 0x01) autoGoodssatate = "半卷";
                        else if (buffer1[15] == 0x02) autoGoodssatate = "满卷";

                    }
                    if (frmNewMain.orderhas.Count > 0)
                    {
                        //如果触摸屏有A1工位撤销指令状态过来，且此时字典中有指令
                        if (buffer1[10] != 0)
                        {
                            if (frmNewMain.orderhas.ContainsKey(buffer1[10].ToString()))
                            {
                                lock (locker)
                                {
                                    frmNewMain.orderhas.Remove(buffer1[10].ToString());//移除对应项
                                }
                            }
                            siemensTcpNet1.Write("M510", 0);
                        }
                        //如果触摸屏有A2工位撤销指令状态过来，且此时字典中有指令
                        if (buffer1[11] != 0)
                        {
                            if (frmNewMain.orderhas.ContainsKey(buffer1[11].ToString()))
                            {
                                lock (locker)
                                {
                                    frmNewMain.orderhas.Remove(buffer1[11].ToString());//移除对应项
                                }
                            }

                            siemensTcpNet1.Write("M511", 0);
                        }
                        //如果触摸屏有抽检工位撤销指令状态过来，且此时字典中有指令
                        if (buffer1[12] != 0)
                        {
                            if (frmNewMain.orderhas.ContainsKey(buffer1[12].ToString()))
                            {
                                lock (locker)
                                {
                                    frmNewMain.orderhas.Remove(buffer1[12].ToString());//移除对应项
                                }
                            }

                            siemensTcpNet1.Write("M512", 0);
                        }
                        //如果触摸屏有B工位撤销指令状态过来，且此时字典中有对应工位的指令指令
                        if (buffer1[13] != 0)
                        {
                            if (frmNewMain.orderhas.ContainsKey(buffer1[13].ToString()))
                            {
                                lock (locker)
                                {
                                    frmNewMain.orderhas.Remove(buffer1[13].ToString());//移除对应项
                                }
                            }
                            siemensTcpNet1.Write("M513", 0);
                        }


                    }
                    else//字典中无指令时
                    {
                        //清撤销指令状态区
                        byte[] order_clear = new byte[4];
                        siemensTcpNet1.Write("M510", order_clear);
                        siemensTcpNet1.Write("M520", 0);
                        siemensTcpNet1.Write("M521", 0);
                    }

                    //获取字符串
                    string strKT = Encoding.ASCII.GetString(buff.Content, 22, 10);




                }
                else
                {
                    read = HslCommunication.OperateResult.CreateFailedResult<JObject>(buff);
                }


                if (read.IsSuccess)
                {
                    if (failed1 != 0)//断线重读成功
                    {
                        failed1 = 0;
                        //ShowFailedMessage(failed1, "地面PLC");//显示连接成功
                    }
                    else
                        failed1 = 0; // 读取失败次数清空

                    frmNewMain.bOpening1 = true;
                }
                else
                {
                    frmNewMain.bOpening1 = false;

                    failed1++;
                    //ShowFailedMessage(failed1, "地面PLC");  // 显示出来读取失败的情况
                }


                Thread.Sleep(10);                      // 两次读取的时间间隔
            }

        }

        //读写PLC2  1500的线程
        private void ThreadBackgroundReadPlc2()
        {

            // 此处假设我们读取的是西门子PLC的数据，其实三菱的数据读取原理是一样的，可以仿照西门子的开发
            while (true)
            {
                //触发事件
                //EventArgs,写一个子类继承该类，子类中添加需要封装的数据信息，此处只需要传递string信息，详见MyEventArgs
                if (SendMsgEvent != null)
                {
                    SendMsgEvent(this, new MyEventArg()
                    {
                        buffer1 = buffer1,
                        strplc1 = "plc1",
                        failed1 = failed1,
                        buffer2 = buffer2,
                        strplc2 = "plc2",
                        failed2 = failed2_Read,
                        userdata= Usertype
                    });
                }

                frmNewMain.bListenPlc2 = true;
                HslCommunication.OperateResult<JObject> read = null;
                // 这里仅仅演示了西门子的数据读取
                HslCommunication.OperateResult<byte[]> buff = siemensTcpNet2_Read.Read("M500", 50);
                bool isSuccess2 = buff.IsSuccess;
                if (isSuccess2)
                {
                    //首先返回一个读取成功的对象
                    read = HslCommunication.OperateResult.CreateSuccessResult(new JObject()
                    {
                    });
                    //PLC2访问成功
                    readPLC2Success = read.IsSuccess;
                    ////临时用先清写指令区
                    //byte[] buffWrite_clear0 = new byte[10];
                    //OperateResult result0 = frmSieMens.siemensTcpNet2.Write("M510", buffWrite_clear0);

                    //集合先清空
                    buffer2.Clear();
                    //读取回来的数据显示到txtbox
                    //ShowReadContent(buff, "PLC2");

                    buffer2.AddRange(buff.Content);//将指定集合的元素添加到集合buffer的末尾

                    ///**************************SICK扫描数据**************************/
                    //Usertype.SICK_CastNumber = buff.Content[5];
                    //Usertype.SICK_CastType = buff.Content[6];
                    //Usertype.SICK_CastCentreX = siemensTcpNet1.ByteTransform.TransUInt16(buff.Content, 7);
                    //Usertype.SICK_CastCentreY = siemensTcpNet1.ByteTransform.TransUInt16(buff.Content, 9);
                    //Usertype.SICK_PoolLidHas = buff.Content[11];

                    /**************************PLC反馈数据**************************/
                    Usertype.PLC_Current_X = siemensTcpNet1.ByteTransform.TransUInt32(buff.Content, 0);
                    Usertype.PLC_Current_Y = siemensTcpNet1.ByteTransform.TransUInt16(buff.Content, 4);
                    Usertype.PLC_Current_Z = siemensTcpNet1.ByteTransform.TransUInt16(buff.Content, 6);
                    
                    Usertype.X_Arrive_Signal = buff.Content[10];
                    Usertype.Y_Arrive_Signal = buff.Content[11];
                    Usertype.Z_Arrive_Signal = buff.Content[12];
                    Usertype.Clamp_State = buff.Content[13];

                    //Usertype.Person_Confirm_PoolLid = buff.Content[36];

                    /**************************回读上位机写进PLC的数据**************************/

                    Usertype.ReadBack_Dest_X = siemensTcpNet1.ByteTransform.TransUInt32(buff.Content, 20);
                    Usertype.ReadBack_Dest_Y = siemensTcpNet1.ByteTransform.TransUInt16(buff.Content, 24);
                    Usertype.ReadBack_Dest_Z = siemensTcpNet1.ByteTransform.TransUInt16(buff.Content, 26);
                   
                    Usertype.ReadBack_XEnable_Signal = buff.Content[30];
                    Usertype.ReadBack_YEnable_Signal = buff.Content[31];
                    Usertype.ReadBack_ZEnable_Signal = buff.Content[32];
                   
                    Usertype.ReadBack_Catch_Release_Enable_Signal = buff.Content[34];
                    Usertype.ReadBack_Write_New_Flag = buff.Content[35];



                    if (frmNewMain.dataListrequest.Count == 0 && !frmNewMain.startExchageLib)//指令请求队列中没有任何指令时,且没有倒库任务时
                    {

                        if (Start_system_flag == false)
                        {

                            //使能信号先清零
                            siemensTcpNet2_Read.Write("M530", 0x00);
                            siemensTcpNet2_Read.Write("M531", 0x00);
                            siemensTcpNet2_Read.Write("M532", 0x00);
                            Start_system_flag = true;
                        }

                    }
                    //初始位值到位，抱闸打开，开始 三个信号同时有
                    if (buffer2[14] == 0x01 && buffer2[15] == 0x01 && buffer2[16] == 0x01)//上位机收到PLC自动演示信号
                    {
                        //开始将自动演示指令放进指令缓存区
                        string s1 = "01020201010102010000";
                        string s2 = "02020201010102010000";
                        string s3 = "04020401019999010000";
                        string sqlupdate = @"update 库存记录表
                                            set
                                            物料规格 = '卷筒',
                                            物料状态 = '空卷',
                                            检测状态 = NULL,
                                            生产批号 = NULL
                                            where 物料状态 is not null";
                        if (frmNewMain.orderhas.Count == 0 && frmNewMain.dataListrequest.Count == 0)
                        {

                            SqlHelper.ExecuteNonQuery(sqlupdate);
                            //首先将库中卷筒变为空卷
                            lock (locker)
                            {
                                frmNewMain.orderhas.Add("01", s1);
                                frmNewMain.orderhas.Add("02", s2);
                                frmNewMain.orderhas.Add("04", s3);
                            }

                        }
                        //线程开启标志
                        Usertype.threadStart_flag = true;

                    }
                    else
                    {
                        //线程关闭
                        Usertype.threadStart_flag = false;
                        //清空任务队列
                        lock (locker)
                        {
                            frmNewMain.orderhas.Clear();
                            frmNewMain.dataListrequest.Clear();
                        }
                    }


                }
                else
                {
                    read = HslCommunication.OperateResult.CreateFailedResult<JObject>(buff);
                }


                if (read.IsSuccess)
                {
                    if (failed2_Read != 0)//断线重读成功
                    {
                        failed2_Read = 0;
                        //ShowFailedMessage(failed2, "执行动作PLC");//显示连接成功
                    }
                    else
                        failed2_Read = 0; // 读取失败次数清空

                    frmNewMain.bOpening2 = true;
                }
                else
                {
                    frmNewMain.bOpening2 = false;
                    failed2_Read++;
                    //ShowFailedMessage(failed2, "执行动作PLC");  // 显示出来读取失败的情况
                }

                Thread.Sleep(10);              // 两次读取的时间间隔
            }

        }

















        #region 日志块

        /// <summary>
        /// 系统的日志记录器
        /// </summary>
        private ILogNet LogNet { get; set; }

        private void LogNet_BeforeSaveToFile(object sender, HslEventArgs e)
        {

        }
        #endregion
    }
}
