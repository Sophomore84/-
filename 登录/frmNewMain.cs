using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using HslCommunication.Profinet.Siemens;
using HslCommunication.LogNet;
using ClientsLibrary;
using HslCommunication;
using System.Collections;
using Newtonsoft.Json.Linq;
using static 风帆电池.frmSieMens;
using 风帆电池;
using 风帆电池.Tb_Model;
using NPOI.SS.Formula.Functions;

namespace  风帆电池
{
    public partial class frmNewMain : FormBase
    {

        //1.声明自适应类实例
        AutoSizeFormClass asc = new AutoSizeFormClass();
        //定义PLC通讯实例
        S71KConnect s71kconnect = new S71KConnect();

        public frmLogin  log;
        
        static log4net.ILog LOG = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        
        public static SerialPort sp = new SerialPort();
        

        List<byte> buffer = new List<byte>(4096);
        //队列Queue
        public static Queue<string> dataListrequest = new Queue<string>();//请求命令队列
        public static Queue<string> dataListrequestfu = new Queue<string>();//请求命令队列辅助队列
        public static Queue<string> dataListcomplete = new Queue<string>();//完成队列

        //定义键值对集合  存储出库位置，卷筒编号+生产批号
        Dictionary<string, string> listNumBuffer = new Dictionary<string, string>();

        //锁的实例化
        private object locker = new object();
        // sqlserver操作数据线程
        private Thread trdSQLOperate;

        // 任务动作操作数据线程
        private Thread trdtaskDo;
        //任务反馈回调线程
        private Thread trdtaskCallback;
        public static bool bListen1 = false;
        public static bool bListenPlc1 = false;
        public static bool bListenPlc2 = false;
        //通讯状态（串口或网口连接正常时为true）
        public static bool bOpening = false;
        //第一路PLC1连接状态标志
        public static bool bOpening1 = false;
        //第二路PLC2连接状态标志
        public static bool bOpening2 = false;

        public static bool  Quanxian= false;
        //获得管理员权限
        public static bool quanxianGet;

        //开始倒库标志
        public static bool startExchageLib = false;
        //倒库起始库位 
        public static string startPosition = null;
        //倒库起始库位的状态
        public static string startPositionstate = null;
        //倒库目标库位
        public static string desPosition = null;
        //倒库目标库位的状态
        public static string desPositionstate = null;
       
        public static int oldexchangeLib_count;
        public static string exchangeLibposition = null;
        public static string exchangeSteps = null;
        public static string exchangeState = null;
        private bool bstartExchange = false;
        private string[] strArrayLibMess = new string[8];

        public static int diaojustate;
        public static int QuYu_BiaoQian = 10;
        private bool bListen3 = false;
        private bool bListen4 = false;
       
        private bool bClear = false;
        private bool bstartcode = true;
        //收到PLC的读指令
        public static bool bread = false;
        //收到PLC的有效完成指令
        public static bool bback = false;

        public static bool PLCStart_Position_flag = false;

        public static Byte diaojuAction = 0;
        //public string orderData;
        public static int orderhasoldcount = 0;
        public static int oldcount = 0;
        public static int oldfucount = 0;
        public static string PositionName=null;
        public static string EmptyNum = null;
        public static string GoodsNum = null;
        public static ushort?[] Position = new ushort?[3] { 0,0,0};
        public static byte[] sendDataBuff = new byte[23] {0x01,0x03,0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
       0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,0xe4,0x59};
       
        public UCTLibState userlibstate;

        private string LogSavePath { get; set; }
        /// <summary>
        /// 用来记录一般的事物日志
        /// </summary>
        public ILogNet RuntimeLogHelper { get; set; }
        /// <summary>
        /// 用来记录一般的事物日志
        /// </summary>
        private ILogNet AdviceLogHelper { get; set; }
        public frmNewMain()
        {
            //捕获所有未处理的异常并进行预处理
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //检测日志路径是否存储
            LogSavePath = Application.StartupPath + @"\Logs\系统异常";
            if (!System.IO.Directory.Exists(LogSavePath))
            {
                System.IO.Directory.CreateDirectory(LogSavePath);
            }
            // 初始化一般日志工具
            RuntimeLogHelper = new LogNetSingle(LogSavePath + @"\Error_log.txt");
    
            InitializeComponent();
            //创建用户自定义界面对象
            userlibstate = new UCTLibState();
            userlibstate.Location = panelorder.Location;
            //窗体加载时，显示库状态界面
            mygpbExchange.Controls.Add(userlibstate);
            userlibstate.Visible = true;
            //指令界面隐藏
            panelorder.Visible = false;
            mygpbExchange.Text = "库存状态";

            FormExIni();
            _systemButtonManager = new SystemButtonManager(this);
            
           
        }
       
        private void frmMain_Load(object sender, EventArgs e)
        {

            //建立PLC长连接通信
            s71kconnect.PLCInit();
            //窗体自适应
            asc.controllInitializeSize(this);
            //最大化之前设置窗体默认为Normalize
            //记录完控件的初始位置和大小后，再最大化
            this.WindowState = (System.Windows.Forms.FormWindowState)(2);
            ////0 - Normalize , 1 - Minimize,2- Maximize

            //串口数据接收事件注册
            sp.DataReceived += sp_DataReceived;
            ////启动后台处理指令的线程
            //trdSQLOperate = new Thread(new System.Threading.ThreadStart(SQLOperate));
            //trdSQLOperate.IsBackground = true;
            //trdSQLOperate.Priority = ThreadPriority.Highest;
            //trdSQLOperate.Start();

            //启动后台处理指令的线程
            trdtaskDo = new Thread(new System.Threading.ThreadStart(TaskDo));
            trdtaskDo.IsBackground = true;
            //trdtaskDo.Priority = ThreadPriority.Highest;
            //trdtaskDo.Start();

            //启动后台处理指令的线程
            trdtaskCallback = new Thread(new System.Threading.ThreadStart(TaskCallBack));
            trdtaskCallback.IsBackground = true;
            trdtaskCallback.Priority = ThreadPriority.Highest;
            trdtaskCallback.Start();

            lblEdit.Text += "                                          ";
            this.lblEdit.Alignment = ToolStripItemAlignment.Right;
            lblEdit.Visible = false;
            this.lblTimer.Alignment = ToolStripItemAlignment.Right;

            frmNewMain.dataListcomplete.Enqueue("初始化完成指令");
            
        }






        //添加（重写）关闭窗口事件
        private void frmMain_Closing(object sender, FormClosingEventArgs e)
        {
            //注意判断关闭事件Reason来源于窗体按钮，否则用菜单退出时无法退出!
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;    //取消"关闭窗口"事件
                this.WindowState = FormWindowState.Minimized;    //使关闭时窗口向右下角缩小的效果
                myIcon.Visible = true;
                this.Hide();
                return;
            }

        }
        //单击鼠标事件
        private void myIcon_MouseClick(object sender, MouseEventArgs e)
        {
            //右键退出菜单
            if (e.Button == MouseButtons.Right)
            {
                myrightMenu.Show();
            }
            //左键显示
            if(e.Button == MouseButtons.Left)
            {
                if (this.Visible == true)
                {
                    this.Hide();
                    this.ShowInTaskbar = false;
                }
                else
                {
                    this.Visible = true;
                    //先还原
                    this.WindowState = FormWindowState.Normal;
                    this.BringToFront();
                    //显示在任务栏
                    this.ShowInTaskbar = true;

                }
                
            }
        }

        //右键执行退出
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myIcon.Visible = false;

            if (sp.IsOpen)
            {
                while (bListen1 || bListenPlc1 || bListenPlc2||bListen3 || bListen4) Application.DoEvents();     
                trdSQLOperate.Abort();
                sp.Close();
               
            }
           
            ////强制所有消息中止，退出所有的窗体，但是若有托管线程（非主线程），也无法干净地退出；
            //Application.Exit();
            //这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。
            System.Environment.Exit(0);
           
        }
        /// <summary>
        /// 一个处理服务器未处理异常的方法，对该方法进行记录，方便以后的分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {      

            try
            {
                Exception error = (Exception)e.ExceptionObject;
                RuntimeLogHelper.WriteException("UnhandledException:", error);//LogHelper是写日志的类，这里，可以直接写到文件里
                //显示异常的详细信息
                MessageBox.Show("当前异常： "+e.ExceptionObject.ToString());
            }
            catch
            {

            }
        }



        //窗体改变时自适应大小
        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            //窗体状态为最小化时

            if (this.WindowState == FormWindowState.Minimized)
            {

                    //使最小化时窗口向右下角缩小的效果
                    this.WindowState = FormWindowState.Minimized;
                    this.ShowInTaskbar = false;//不显示在任务栏
                    myIcon.Visible = true;//显示托盘图标
                    this.Hide();
                
            }
           

            //窗体自适应代码
            asc.controlAutoSize(this);
        }
        
        //打开文件
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "打开";
            ofd.Filter = "所有文件|*.*";
            //if (ofd.ShowDialog() == DialogResult.OK)
                //txtDataSource.Text = ofd.FileName;//获取文件路径
        }


        private List<Thread> _threadList = new List<Thread>();   //记录产生的线程，可声明为全局公共变量 
        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            lblTimer.Text = string.Format("{0:f}", dt);
            lblEdit.Visible = false;
            
            //定时器刷新UI Listbox
             //测试代码。。。。。。。。。。。。。
                //if (_threadList.Count == 0) S71KConnect.Usertype.threadStart_flag = true;
                //if (trdtaskDo.ThreadState == (ThreadState.Running | ThreadState.Background)) S71KConnect.Usertype.threadStart_flag = false;
                //string aaaa = trdtaskDo.ThreadState.ToString();
                if (!S71KConnect.Usertype.threadStart_flag)
                {
                    if (_threadList.Count > 0)
                    {
                        trdtaskDo.Abort();
                        //trdtaskDo.Join(); 执行后线程被彻底关闭
                        S71KConnect.Usertype.threadStart_flag = true;
                        _threadList.Clear();
                    }
                    
                }
                else if(S71KConnect.Usertype.threadStart_flag)
                {
                    if (_threadList.Count == 0)
                    {
                        trdtaskDo = new Thread(new System.Threading.ThreadStart(TaskDo));
                        trdtaskDo.IsBackground = true;
                        trdtaskDo.Priority = ThreadPriority.Highest;
                        _threadList.Add(trdtaskDo);
                        trdtaskDo.Start();
                    }
                    
                }
            //强制
            if (frmNewMain.bOpening1 || frmNewMain.bOpening2)
            {
                frmNewMain.bOpening = true;
                lblState.Text = "通讯连接正常";
            }
            else
            {
                frmNewMain.bOpening = false;
                if (!frmNewMain.bOpening1 && frmNewMain.bOpening2) lblState.Text = "地面PLC通讯连接异常";
                if (!frmNewMain.bOpening2 && frmNewMain.bOpening1) lblState.Text = "动作PLC通讯连接异常";
                if (!frmNewMain.bOpening2 && !frmNewMain.bOpening1) lblState.Text = "PLC通讯未建立连接";

            }





        }

        //串口接收数据
       private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }



        //public event EventHandler exchangeLibEvent; //倒库指令count属性发生变化

  
        //倒库指令个数变化时
        private void FrmNewMain_exchangeLibEvent1(object sender, EventArgs e)
        {
            
        }


        //线程等待
        static AutoResetEvent mEvent = new AutoResetEvent(false);
        public static int Steps_Change;
        //上位机需要写入PLC的目的X,Y,Z
        public static UInt32 Write_Dest_X;
        public static UInt16 Write_Dest_Y;
        public static UInt16 Write_Dest_Z;
        public static UInt16 Write_Dest_Angle;//写入旋转小车目的角度值
        public const UInt16 Safetty_Height = 1000;//起升安全高度值常数
        public static UInt16 PoolLid_Height = 900;//池盖子的高度
        public static Byte Write_New_Flag;     //新写入PLC动作标志
        public static Byte WriteX_Enable_Signal;  //写入PLC目的坐标X的使能信号
        public static Byte WriteY_Enable_Signal;  //写入PLC目的坐标X的使能信号
        public static Byte WriteZ_Enable_Signal;  //写入PLC目的坐标X的使能信号
        public static Byte WriteAngle_Enable_Signal;  //写入旋转小车目的角度值使能信号

        public static Byte Write_Catch_Release;  //写入抓放动作


        //任务动作执行线程

        private void TaskDo()
        {
            try
            {

                while (true)
                {
                   
                    ////强制
                    //if (frmNewMain.bOpening1 || frmNewMain.bOpening2)
                    //{
                    //    frmNewMain.bOpening = true;
                    //    lblState.Text = "通讯连接正常";
                    //}
                    //else
                    //{
                    //    frmNewMain.bOpening = false;
                    //    if (!frmNewMain.bOpening1 && frmNewMain.bOpening2) lblState.Text = "地面PLC通讯连接异常";
                    //    if (!frmNewMain.bOpening2 && frmNewMain.bOpening1) lblState.Text = "动作PLC通讯连接异常";
                    //    if (!frmNewMain.bOpening2 && !frmNewMain.bOpening1) lblState.Text = "PLC通讯未建立连接";

                    //}

                    //如果字典中有指令且队列为空时，先延时10S然后取出第一项放入队列中，一旦放入队列，不可逆
                    if (orderhas.Count > 0 && dataListrequest.Count == 0)
                    {
                        //10S先改为延时500ms
                        Thread.Sleep(500);
                        lock (locker)
                        {
                            string firstorder = orderhas.GetByIndex(0).ToString();
                            //取出字典里第一个元素值，放入队列中
                            dataListrequest.Enqueue(firstorder);
                            //再将首个元素从字典里删除
                            orderhas.RemoveAt(0);
                        }
                    }
                    //任务队列有任务后，开始一系列动作
                    if (dataListrequest.Count >= 1)
                    {
                        string task = dataListrequest.Peek();//返回队列首个元素但不移除
                        PlcOrder plcorder = new PlcOrder();
                        plcorder.OrderArea = task.Substring(0, 2);
                        plcorder.OrderType = task.Substring(2, 2);
                        plcorder.OrderAction = task.Substring(4, 2);
                        plcorder.ActionState = task.Substring(6, 2);
                        plcorder.BanZuNumber = task.Substring(8, 2);
                        plcorder.OrderGoodsType = task.Substring(10, 2) == "99" ? "卷筒" : task.Substring(10, 2);
                        plcorder.OrderGoodsState = task.Substring(12, 2);
                        plcorder.OrderGoodsCheckState = task.Substring(14, 2);
                        switch (plcorder.OrderGoodsState)
                        {
                            case "99": plcorder.OrderGoodsState = "空卷"; break;
                            case "01": plcorder.OrderGoodsState = "半卷"; break;
                            case "02": plcorder.OrderGoodsState = "满卷"; break;
                            default: break;
                        }
                        if (plcorder.OrderGoodsType != "卷筒")
                        {
                            switch (plcorder.OrderGoodsCheckState)
                            {
                                case "01": plcorder.OrderGoodsCheckState = "合格"; break;
                                case "02": plcorder.OrderGoodsCheckState = "待检"; break;
                                case "03": plcorder.OrderGoodsCheckState = "报废"; break;
                                default: break;
                            }

                        }
                        else
                        {
                            plcorder.OrderGoodsCheckState = null;
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                txtCheckState.Text = "";
                                txtCheckState.Enabled = false;
                            }));
                        }

                        switch (plcorder.OrderAction)
                        {
                            case "01": plcorder.OrderAction = "取料"; break;
                            case "02": plcorder.OrderAction = "入库"; break;
                            case "03": plcorder.OrderAction = "出库"; break;
                            case "04": plcorder.OrderAction = "退库"; break;
                            default: break;
                        }
                        switch (plcorder.ActionState)
                        {
                            case "01": plcorder.ActionState = "正在执行"; break;
                            case "0F": plcorder.ActionState = "执行完成"; break;
                            default: break;
                        }
                        OrderDoMethod(plcorder.OrderArea, plcorder.OrderType, plcorder.OrderAction, plcorder.ActionState,
                            plcorder.BanZuNumber, plcorder.OrderGoodsType, plcorder.OrderGoodsState, plcorder.OrderGoodsCheckState);


                    }

                    //有倒库任务且无缓存指令且无主任务指令时，才能执行倒库功能
                    if (startExchageLib && orderhas.Count == 0 && dataListrequest.Count == 0)
                    {
                        //倒库任务队列
                        Queue<string> exchangeLibrequest = new Queue<string>();//倒库队列

                        if (desPositionstate == "空位")
                        {
                            if (exchangeLibrequest.Count == 0)
                            {
                                //入倒库队列
                                exchangeLibrequest.Enqueue(startPosition + "," + desPosition);
                                this.BeginInvoke(new MethodInvoker(delegate
                                {
                                    listBoxShowCurrentOrder.Items.Add(startPosition + '→' + desPosition);
                                }));

                            }
                        }
                        else //目标位非空时
                        {
                            string positioncenter = null;
                            //先从库中查出一个中间空位
                            orderHelper.SelectEmptyLocation(out positioncenter);
                            if (positioncenter != null)
                            {
                                exchangeLibrequest.Enqueue(startPosition + "," + positioncenter);
                                exchangeLibrequest.Enqueue(desPosition + "," + startPosition);
                                exchangeLibrequest.Enqueue(positioncenter + "," + desPosition);
                                this.BeginInvoke(new MethodInvoker(delegate
                                {
                                    listBoxShowCurrentOrder.Items.Add(startPosition + '→' + positioncenter);
                                    listBoxShowCurrentOrder.Items.Add(desPosition + '→' + startPosition);
                                    listBoxShowCurrentOrder.Items.Add(positioncenter + '→' + desPosition);
                                }));

                            }
                            else //库中已无空位置
                            {
                                startExchageLib = false;//清标志，不执行倒库操作
                                continue;
                            }
                        }
                        if (exchangeLibrequest.Count > 0)//倒库任务个数大于0时
                        {
                            Auto_Exchange_LibMethod(exchangeLibrequest);
                            startExchageLib = false;

                        }

                    }
                }
            }
            catch
            {
          
                //    MessageBox.Show(trdtaskDo.ThreadState.ToString());
                //if (trdtaskDo.ThreadState == ThreadState.Aborted)
                //    MessageBox.Show("线程被终止了");

            }

            finally
            {

            }

        
       }


        private void Auto_Exchange_LibMethod(Queue<string> queue)
        {
            UInt32 tb_positionX = 0;
            UInt16 tb_positionY = 0;
            UInt16 tb_positionZ = 0;
            Byte catch_or_realse = 0;
            DataTable dt = null;
            do
            {
                string  strExchange = queue.Peek();
                string[] strArray = strExchange.Split(',');

                //先查询出对应库位坐标，以及该库位上对应的卷筒编号，物料信息等
                orderHelper.exchangeLibMess(strArray[0], out tb_positionX, out tb_positionY, out tb_positionZ);
                //输出当前库位中含有的信息，显示到UI
                string[] buffArray = orderHelper.exchangeLibMess(strArray[0]);
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    txtSteps.Text = "第一步";
                    txtActionState.Text = "正在执行";
                    txtAction.Text = "抓取";
                    txtGWposition.Enabled = false;
                    txtPosition.Text = strArray[0]; //库存的位置
                    txtSelctEmptyNum.Text = buffArray[0];
                    txtGoodsCode.Text = buffArray[1];
                    txtGoodsState.Text = buffArray[2];
                    txtCheckState.Text = buffArray[3];
                }));
                catch_or_realse = 2;
                DoStepsAction(tb_positionX, tb_positionY, tb_positionZ, catch_or_realse);
                //抓取完成后
                //将该库位上的详细信息放进缓存区
                strArrayLibMess = orderHelper.exchangeLibMess(strArray[0]);

                //再根据查到的卷筒编号及位置重新更新库存记录
                orderHelper.UpdateLibInRecord(strArray[0], null, null, null, null, null,
                   null, null, null);
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    txtlibActionState.Text = "库中记录完成";//将数据库执行结果委托显示在主窗体 

                }));


                /*********************第二步去往目标位置放下*******************************/
                //先查询出对应空库位坐标
                orderHelper.exchangeLibMess(strArray[1], out tb_positionX, out tb_positionY, out tb_positionZ);
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    txtSteps.Text = "第二步";
                    txtActionState.Text = "正在执行";
                    txtAction.Text = "放下";
                    txtGWposition.Enabled = false;
                    txtPosition.Text = strArray[1]; //库存的位置

                }));
                catch_or_realse = 1;
                DoStepsAction(tb_positionX, tb_positionY, tb_positionZ, catch_or_realse);
                //执行完之后
                //将上次放进缓存区的信息，更新到当前空库位上
                orderHelper.UpdateLibInRecord(strArray[1], strArrayLibMess);
                for (int i = 0; i < strArrayLibMess.Length; i++)
                {
                    strArrayLibMess[i] = null;
                }
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    txtlibActionState.Text = "库中记录完成";//将数据库执行结果委托显示在主窗体  
                    listBoxShowCurrentOrder.Items.RemoveAt(0);//移除首位                
                }));

                queue.Dequeue();//执行完成之后移除倒库队列的第一个元素

            }
            while (queue.Count > 0);
        }



        //新构造方法执行指令一系列动作
        private void OrderDoMethod(string orderArea, string orderType, string orderAction, string orderActionState,
            string banzu, string goodsCode, string goodsState, string checkstate)
        {
            UInt32 tb_positionX = 0;
            UInt16 tb_positionY = 0;
            UInt16 tb_positionZ = 0;
            Byte catch_or_realse = 0;
            DataTable dt=null;

            //switch (orderArea)
            //{
            //    case "01": orderArea = "A1"; break;
            //    case "02": orderArea = "A2"; break;
            //    case "03": orderArea = "抽检区"; break;
            //    case "04": orderArea = "B"; break;
            //    default: break;
            //}
            //首先判断单双指令
            if (orderType == "01")//单指令给出两个坐标位置
            {
                //再判断执行什么动作
                if (orderAction == "取料" || orderAction == "出库")//取料或出库
                {
                    //先按条件查出库中位置坐标
                    if (orderArea == "01" || orderArea == "02")//A区
                    {
                        //先按条件查出库中卷筒位置坐标，输出卷筒编号
                        dt = orderHelper.SelectOutEmpty(goodsCode, goodsState, out tb_positionX,
                               out tb_positionY, out tb_positionZ, out PositionName, out EmptyNum, out GoodsNum);
                    }
                    else if (orderArea == "04" || orderArea == "03")//B区物料出库或抽检区
                    {
                        //先按条件查出库中带有物料属性的卷筒位置坐标，输出卷筒编号及物料规格属性
                        dt = orderHelper.SelectOutGoods(goodsCode, goodsState, checkstate,
                            out tb_positionX, out tb_positionY, out tb_positionZ, out PositionName, out EmptyNum, out GoodsNum);
                    }

            

                    if (PositionName != null)//若库中有结果
                    {

                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            dgvPosition.DataSource = dt;//将查询结果委托显示在主窗体 
                            txtSteps.Text = "第一步";
                            txtActionState.Text = orderActionState;
                            txtAction.Text = orderAction;
                            txtGWposition.Text = orderArea;
                            txtPosition.Text = PositionName; //起始位
                            txtSelctEmptyNum.Text = EmptyNum;
                            txtGoodsCode.Text = goodsCode;
                            txtGoodsState.Text = goodsState;
                            txtCheckState.Text = checkstate;

                        }));

                    }

                    else  //查不到到结果 ,库中无空卷
                    {
                        dataListrequest.Dequeue();//再移除主请求队列第一个指令元素
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            dgvPosition.DataSource = dt;//将查询结果委托显示在主窗体
                            txtErrorPut.Text = "库中无相应规格的物料";//将查询结果委托显示在主窗体                  
                        }));
                        //提示到触摸屏
                        //OperateResult result = frmSieMens.siemensTcpNet1.Write("M521", 0x01);
                        return;

                    }
                    /************************第一步***************************/
                    //抓取
                    catch_or_realse = 2;
                    //调用动作方法，PLC自动执行一系列动作
                    DoStepsAction(tb_positionX, tb_positionY, tb_positionZ, catch_or_realse);
                    //抓取执行完成后更新库记录

                    OutInLibRecord outinrecordstart = new OutInLibRecord();
                    //从库中删除卷筒记录，且把卷筒编号及要去往的区域存入临时缓存区中
                    outinrecordstart.Date = DateTime.Now.ToString("yyyy-MM-dd");        // 2008-09-04
                    outinrecordstart.Time = DateTime.Now.ToString("HH:mm:ss");        // 23:05:57

                    //再根据查到的卷筒编号及位置重新更新库存记录
                    orderHelper.UpdateLibInRecord(PositionName, null, null, null, null, null,
                        null, null, null);
                    //再根据库位置更新库存状态
                    orderHelper.UpdateLibState(PositionName);
                    //在出入库记录表中插入一条出库记录
                    orderHelper.InsertOutInRecord(outinrecordstart.Date, outinrecordstart.Time, PositionName, EmptyNum,
                        goodsCode, goodsState, checkstate, GoodsNum, orderAction);

                    /************************第二步***************************/
                    //按区域位值条件查出卷筒将去往的位置坐标
                    dt = orderHelper.SelectLocation(orderArea, out tb_positionX, out tb_positionY, out tb_positionZ);
                    if (dt.Rows.Count > 0)
                    {
                        //查出工位坐标绑定到
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            dgvPosition.DataSource = dt;//将查询结果委托显示在主窗体  
                            txtSteps.Text = "第二步"; //执行步奏
                        }));
                    }
                    else
                    {
                        MessageBox.Show("数据库查询发生错误！");
                        return;
                    }
                    //放
                    catch_or_realse = 1;
                    //调用动作方法，PLC自动执行一系列动作
                    DoStepsAction(tb_positionX, tb_positionY, tb_positionZ, catch_or_realse);
                    //执行完成后更新库记录



                    //把卷筒编号及去往的区域存入临时缓存区中
                    //然后将卷筒编号及物料的编号和去往的位置编号放进全局缓存区集合中
                    if (!listNumBuffer.ContainsKey(orderArea))
                    {
                        //集合中没有对应位置的缓存记录，直接添加进去
                        listNumBuffer.Add(orderArea, EmptyNum + "," + GoodsNum);
                    }
                    else
                    {
                        //集合中有对应位置的缓存记录，先移除再添加
                        listNumBuffer.Remove(orderArea);
                        //再把最新的键值添加进去
                        listNumBuffer.Add(orderArea, EmptyNum + "," + GoodsNum);
                    }

                    //同时将关于卷筒去向的工位信息直接更新到数据库中存储
                    orderHelper.insertBuffer(orderArea, EmptyNum, GoodsNum);

                    dataListrequest.Dequeue();//再移除主请求队列第一个指令元素
                }
                else if (orderAction == "入库" || orderAction == "退库")//入库或退库
                {
                    //入库退库动作前先查询库中是否有空位。。。。
                    dt = orderHelper.SelectEmptyLocation(out PositionName);
                    if (dt.Rows.Count > 0) //有空位才会执行下步动作
                    {
                        //把临时缓存区中卷筒编号及去往的区域 与进库位置进行比对
                        //判断缓存区是否含有关于此工位，卷筒的缓存
                        if (listNumBuffer.ContainsKey(orderArea))//如果存在此工位的缓存记录
                        {
                            string strNumBuff = listNumBuffer[orderArea];//通过键获取值字符串卷筒编号+生产批号
                            string[] strArray = strNumBuff.Split(',');
                            //当入库完成，比对成功将卷筒编号从缓存区入库
                            EmptyNum = strArray[0];//卷筒编号
                            //将获得的卷筒编号显示在触摸屏上，进行校对
                            GoodsNum = strArray[1];//物料编号，入库时为空
                        }
                        else
                        {
                            //如果字典中不存在关于此工位的缓存
                            //在数据库缓存中查找
                            orderHelper.selectBuffer(orderArea, out EmptyNum, out GoodsNum);
                            if (EmptyNum == null)//数据库中也没有关于次工位卷筒的记录
                            {
                                //此时需要提示到触摸屏输入此时的卷筒编号信息？？？？？？？？？？？？？？？？？？
                            }

                        }
                        /*************************************第一步********************************************/
                        //按区域位值条件查出带编号的卷筒从哪个工位坐标进库  
                        dt = orderHelper.SelectLocation(orderArea, out tb_positionX, out tb_positionY, out tb_positionZ);
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            dgvPosition.DataSource = dt;//将查询结果委托显示在主窗体 
                            txtSteps.Text = "第一步";
                            txtActionState.Text = orderActionState;
                            txtAction.Text = orderAction;
                            txtGWposition.Text = orderArea;
                            txtPosition.Text = PositionName;
                            txtSelctEmptyNum.Text = EmptyNum;
                            txtGoodsCode.Text = goodsCode;
                            txtGoodsState.Text = goodsState;
                            txtCheckState.Text = checkstate;

                        }));

                        //抓取
                        catch_or_realse = 2;
                        //调用动作方法，PLC自动执行一系列动作
                        DoStepsAction(tb_positionX, tb_positionY, tb_positionZ, catch_or_realse);
                        //抓取执行完成后更新库记录



                        /*************************************第二步********************************************/
                        //先按条件查出库中空位的位置坐标
                        dt = orderHelper.SelectEmptyLocation(out tb_positionX, out tb_positionY, out tb_positionZ, out PositionName);
                        if (dt.Rows.Count > 0)
                        {
                            //查出工位坐标绑定到
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                dgvPosition.DataSource = dt;//将查询结果委托显示在主窗体  
                                txtSteps.Text = "第二步"; //执行步奏
                            }));
                        }
                        else
                        {
                            MessageBox.Show("数据库查询发生错误！");
                            return;
                        }
                        //放
                        catch_or_realse = 1;
                        //调用动作方法，PLC自动执行一系列动作
                        DoStepsAction(tb_positionX, tb_positionY, tb_positionZ, catch_or_realse);
                        //执行完成后更新库记录

                        OutInLibRecord Libinrecordstart = new OutInLibRecord();
                        Libinrecordstart.Date = DateTime.Now.ToString("yyyy-MM-dd");        // 2008-09-04
                        Libinrecordstart.Time = DateTime.Now.ToString("HH:mm:ss");        // 23:05:57

                        //入库时，将卷筒编号从缓存区入库，同时给加生产批号标签
                        if (orderArea == "01" || orderArea == "02")//A区
                        {
                            //入库执行完成时，
                            if (Common.BanzuRecord != banzu)//班组切换后计算编号清零
                            {
                                Common.Nume = 0;//从零开始计
                                Common.BanzuRecord = banzu;
                            }
                            Common.Nume++;//编号自加
                            string sdate = Libinrecordstart.Date.Replace("-", "").Substring(2, 6);
                            GoodsNum = sdate + banzu + Common.Nume.ToString().PadLeft(2, '0'); //更新生产批号
                            //更新库存记录
                            orderHelper.UpdateLibInRecord(PositionName, EmptyNum, goodsCode, goodsState, checkstate, GoodsNum,
                                Libinrecordstart.Date, Libinrecordstart.Time, orderAction);
                            //再根据库位置更新库存状态
                            orderHelper.UpdateLibState(PositionName);
                            //再向出入库记录表中插入一条出库记录
                            orderHelper.InsertOutInRecord(Libinrecordstart.Date, Libinrecordstart.Time, PositionName, EmptyNum,
                                goodsCode, goodsState, checkstate, GoodsNum, orderAction);

                            //此时再将字典中的对应的键值对清除
                            //取出后清除对应的键值对
                            listNumBuffer.Remove(orderArea);
                            //同时将数据库中的缓存记录清除
                            orderHelper.clearBuffer(orderArea);

                            dataListrequest.Dequeue();//再移除主请求队列第一个指令元素

                        }
                        //退库时，将卷筒编号从缓存区入库，同时根据剩料多少决定
                        if (orderArea == "03" || orderArea == "04")//抽检区和  B区
                        {
                            if (goodsState == "空卷")//如果不带有物料退库，则只有卷筒编号退库，生产批号清除掉
                            {
                                GoodsNum = null;
                                checkstate = null;
                                //更新库存记录
                                orderHelper.UpdateLibInRecord(PositionName, EmptyNum, goodsCode, goodsState, checkstate, GoodsNum,
                                    Libinrecordstart.Date, Libinrecordstart.Time, orderAction);
                                //再根据库位置更新库存状态
                                orderHelper.UpdateLibState(PositionName);
                                //再向出入库记录表中插入一条出库记录
                                orderHelper.InsertOutInRecord(Libinrecordstart.Date, Libinrecordstart.Time, PositionName, EmptyNum,
                                    goodsCode, goodsState, checkstate, GoodsNum, orderAction);

                                //此时再将字典中的对应的键值对清除
                                //取出后清除对应的键值对
                                listNumBuffer.Remove(orderArea);
                                //同时将数据库中的缓存记录清除
                                orderHelper.clearBuffer(orderArea);

                                dataListrequest.Dequeue();//再移除主请求队列第一个指令元素
                            }

                            else //如果带有物料退库，则卷筒编号+生产批号一块退库   
                            {
                                //《退库时考虑工位是否需要输入物料规格》
                                //以需要选物料规格进行
                                //更新库存记录
                                orderHelper.UpdateLibInRecord(PositionName, EmptyNum, goodsCode, goodsState, checkstate, GoodsNum,
                                    Libinrecordstart.Date, Libinrecordstart.Time, orderAction);
                                //再根据库位置更新库存状态
                                orderHelper.UpdateLibState(PositionName);
                                //再向出入库记录表中插入一条出库记录
                                orderHelper.InsertOutInRecord(Libinrecordstart.Date, Libinrecordstart.Time, PositionName, EmptyNum,
                                    goodsCode, goodsState, checkstate, GoodsNum, orderAction);

                                //此时再将字典中的对应的键值对清除
                                //取出后清除对应的键值对
                                listNumBuffer.Remove(orderArea);
                                //同时将数据库中的缓存记录清除
                                orderHelper.clearBuffer(orderArea);
                                dataListrequest.Dequeue();//再移除主请求队列第一个指令元素
                            }

                        }

                    }
                    else //无空位
                    {

                        dataListrequest.Dequeue();//移除主请求队列第一个指令元素
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            dgvPosition.DataSource = dt;//将查询结果委托显示在主窗体
                            txtErrorPut.Text = "库中已无空位置";//将查询结果委托显示在主窗体 

                        }));

                        //提示到触摸屏  库中无空位置
                        //OperateResult result = frmSieMens.siemensTcpNet1.Write("M521", 0x04);

                    }

                }

            }
            else if (orderType == "02")//入库或退库双指令给出四个坐标位置分四步走
            {
                if (orderAction == "入库" || orderAction == "退库")//入库或退库时双指令
                {
                    //入库退库动作前先查询库中是否有空位。。。。
                    dt = orderHelper.SelectEmptyLocation(out PositionName);
                    if (dt.Rows.Count > 0) //有空位才会执行下步动作
                    {
                        //把临时缓存区中卷筒编号及去往的区域 与进库位置进行比对
                        //判断缓存区是否含有关于此工位，卷筒的缓存
                        if (listNumBuffer.ContainsKey(orderArea))//如果存在此工位的缓存记录
                        {
                            string strNumBuff = listNumBuffer[orderArea];//通过键获取值字符串卷筒编号+生产批号
                            string[] strArray = strNumBuff.Split(',');
                            //当入库完成，比对成功将卷筒编号从缓存区入库
                            EmptyNum = strArray[0];//卷筒编号
                            //将获得的卷筒编号显示在触摸屏上，进行校对
                            GoodsNum = strArray[1];//物料编号，入库时为空
                        }
                        else
                        {
                            //如果字典中不存在关于此工位的缓存
                            //在数据库缓存中查找
                            orderHelper.selectBuffer(orderArea, out EmptyNum, out GoodsNum);
                            if (EmptyNum == null)//数据库中也没有关于次工位卷筒的记录
                            {
                                //此时需要提示到触摸屏输入此时的卷筒编号信息？？？？？？？？？？？？？？？？？？
                            }

                        }
                        /*************************************第一步********************************************/
                        //按区域位值条件查出带编号的卷筒从哪个工位坐标进库  
                        dt = orderHelper.SelectLocation(orderArea, out tb_positionX, out tb_positionY, out tb_positionZ);
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            dgvPosition.DataSource = dt;//将查询结果委托显示在主窗体 
                            txtSteps.Text = "第一步";
                            txtActionState.Text = orderActionState;
                            txtAction.Text = orderAction;
                            txtGWposition.Text = orderArea;
                            txtPosition.Text = PositionName;
                            txtSelctEmptyNum.Text = EmptyNum;
                            txtGoodsCode.Text = goodsCode;
                            txtGoodsState.Text = goodsState;
                            txtCheckState.Text = checkstate;

                        }));

                        //抓取
                        catch_or_realse = 2;
                        //调用动作方法，PLC自动执行一系列动作
                        DoStepsAction(tb_positionX, tb_positionY, tb_positionZ, catch_or_realse);
                        //抓取执行完成后更新库记录



                        /*************************************第二步********************************************/
                        //先按条件查出库中空位的位置坐标
                        dt = orderHelper.SelectEmptyLocation(out tb_positionX, out tb_positionY, out tb_positionZ, out PositionName);
                        if (dt.Rows.Count > 0)
                        {
                            //查出工位坐标绑定到
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                dgvPosition.DataSource = dt;//将查询结果委托显示在主窗体  
                                txtSteps.Text = "第二步"; //执行步奏
                            }));
                        }
                        else
                        {
                            MessageBox.Show("数据库查询发生错误！");
                            return;
                        }
                        //放
                        catch_or_realse = 1;
                        //调用动作方法，PLC自动执行一系列动作
                        DoStepsAction(tb_positionX, tb_positionY, tb_positionZ, catch_or_realse);
                        //执行完成后更新库记录

                        OutInLibRecord Libinrecordstart = new OutInLibRecord();
                        Libinrecordstart.Date = DateTime.Now.ToString("yyyy-MM-dd");        // 2008-09-04
                        Libinrecordstart.Time = DateTime.Now.ToString("HH:mm:ss");        // 23:05:57

                        //入库时，将卷筒编号从缓存区入库，同时给加生产批号标签
                        if (orderArea == "01" || orderArea == "02")//A区
                        {
                            //入库执行完成时，
                            if (Common.BanzuRecord != banzu)//班组切换后计算编号清零
                            {
                                Common.Nume = 0;//从零开始计
                                Common.BanzuRecord = banzu;
                            }
                            Common.Nume++;//编号自加
                            string sdate = Libinrecordstart.Date.Replace("-", "").Substring(2, 6);
                            GoodsNum = sdate + banzu + Common.Nume.ToString().PadLeft(2, '0'); //更新生产批号
                            //更新库存记录
                            orderHelper.UpdateLibInRecord(PositionName, EmptyNum, goodsCode, goodsState, checkstate, GoodsNum,
                                Libinrecordstart.Date, Libinrecordstart.Time, orderAction);
                            //再根据库位置更新库存状态
                            orderHelper.UpdateLibState(PositionName);
                            //再向出入库记录表中插入一条出库记录
                            orderHelper.InsertOutInRecord(Libinrecordstart.Date, Libinrecordstart.Time, PositionName, EmptyNum,
                                goodsCode, goodsState, checkstate, GoodsNum, orderAction);

                            //此时再将字典中的对应的键值对清除
                            //取出后清除对应的键值对
                            listNumBuffer.Remove(orderArea);
                            //同时将数据库中的缓存记录清除
                            orderHelper.clearBuffer(orderArea);

                        }
                        //退库时，将卷筒编号从缓存区入库，同时根据剩料多少决定
                        if (orderArea == "03" || orderArea == "04")//抽检区和  B区
                        {
                            if (goodsState == "空卷")//如果不带有物料退库，则只有卷筒编号退库，生产批号清除掉
                            {
                                GoodsNum = null;
                                checkstate = null;
                                //更新库存记录
                                orderHelper.UpdateLibInRecord(PositionName, EmptyNum, goodsCode, goodsState, checkstate, GoodsNum,
                                    Libinrecordstart.Date, Libinrecordstart.Time, orderAction);
                                //再根据库位置更新库存状态
                                orderHelper.UpdateLibState(PositionName);
                                //再向出入库记录表中插入一条出库记录
                                orderHelper.InsertOutInRecord(Libinrecordstart.Date, Libinrecordstart.Time, PositionName, EmptyNum,
                                    goodsCode, goodsState, checkstate, GoodsNum, orderAction);

                                //此时再将字典中的对应的键值对清除
                                //取出后清除对应的键值对
                                listNumBuffer.Remove(orderArea);
                                //同时将数据库中的缓存记录清除
                                orderHelper.clearBuffer(orderArea);
                            }

                            else //如果带有物料退库，则卷筒编号+生产批号一块退库   
                            {
                                //《退库时考虑工位是否需要输入物料规格》
                                //以需要选物料规格进行
                                //更新库存记录
                                orderHelper.UpdateLibInRecord(PositionName, EmptyNum, goodsCode, goodsState, checkstate, GoodsNum,
                                    Libinrecordstart.Date, Libinrecordstart.Time, orderAction);
                                //再根据库位置更新库存状态
                                orderHelper.UpdateLibState(PositionName);
                                //再向出入库记录表中插入一条出库记录
                                orderHelper.InsertOutInRecord(Libinrecordstart.Date, Libinrecordstart.Time, PositionName, EmptyNum,
                                    goodsCode, goodsState, checkstate, GoodsNum, orderAction);

                                //此时再将字典中的对应的键值对清除
                                //取出后清除对应的键值对
                                listNumBuffer.Remove(orderArea);
                                //同时将数据库中的缓存记录清除
                                orderHelper.clearBuffer(orderArea);
                            }

                        }
                        /***************************************多动作的自动出库动作**************************************/
                        //先按条件查出库中位置坐标
                        if (orderArea == "01" || orderArea == "02")//A区
                        {
                            goodsCode = "卷筒";
                            goodsState = "空卷";
                            //先按条件查出库中卷筒位置坐标，输出卷筒编号
                            dt = orderHelper.SelectOutEmpty(goodsCode, goodsState, out tb_positionX,
                                  out tb_positionY, out tb_positionZ, out PositionName, out EmptyNum, out GoodsNum);
                        }
                        else if (orderArea == "04" || orderArea == "03")//B区物料出库或抽检区
                        {
                            //先根据HMI输入的需求查找
                            //goodsCode = S71KConnect.Usertype.ReturnLib_GoodsType;
                            //goodsState = S71KConnect.Usertype.ReturnLib_GoodsState;

                            /********演示操作时**********/
                            goodsCode = "01";
                            goodsState = "满卷";

                            checkstate = "合格";
                            //
                            //先按条件查出库中带有物料属性的卷筒位置坐标，输出卷筒编号及物料规格属性
                            dt = orderHelper.SelectOutGoods(goodsCode, goodsState, checkstate,
                                out tb_positionX, out tb_positionY, out tb_positionZ, out PositionName, out EmptyNum, out GoodsNum);
                        }


                        if (PositionName != null)//若库中有结果
                        {

                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                dgvPosition.DataSource = dt;//将查询结果委托显示在主窗体 
                                txtSteps.Text = "第三步";
                                txtActionState.Text = orderActionState;
                                txtAction.Text = orderAction;
                                txtGWposition.Text = orderArea;
                                txtPosition.Text = PositionName;
                                txtSelctEmptyNum.Text = EmptyNum;
                                txtGoodsCode.Text = goodsCode;
                                txtGoodsState.Text = goodsState;
                                txtCheckState.Text = checkstate;

                            }));

                        }

                        else  //查不到到结果 ,库中相应规格的物料（含空卷筒）
                        {
                            dataListrequest.Dequeue();//再移除主请求队列第一个指令元素
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                dgvPosition.DataSource = dt;//将查询结果委托显示在主窗体
                                txtErrorPut.Text = "库中无相应规格的物料";//将查询结果委托显示在主窗体                  
                            }));
                            //提示到触摸屏
                            //OperateResult result = frmSieMens.siemensTcpNet1.Write("M521", 0x01);
                            return;

                        }
                        /************************第三步***************************/
                        //抓取
                        catch_or_realse = 2;
                        //调用动作方法，PLC自动执行一系列动作
                        DoStepsAction(tb_positionX, tb_positionY, tb_positionZ, catch_or_realse);
                        //抓取执行完成后更新库记录

                        OutInLibRecord outinrecordstart = new OutInLibRecord();
                        //从库中删除卷筒记录，且把卷筒编号及要去往的区域存入临时缓存区中
                        outinrecordstart.Date = DateTime.Now.ToString("yyyy-MM-dd");        // 2008-09-04
                        outinrecordstart.Time = DateTime.Now.ToString("HH:mm:ss");        // 23:05:57

                        //再根据查到的卷筒编号及位置重新更新库存记录
                        orderHelper.UpdateLibInRecord(PositionName, null, null, null, null, null,
                            null, null, null);
                        //再根据库位置更新库存状态
                        orderHelper.UpdateLibState(PositionName);
                        //在出入库记录表中插入一条出库记录
                        orderHelper.InsertOutInRecord(outinrecordstart.Date, outinrecordstart.Time, PositionName, EmptyNum,
                            goodsCode, goodsState, checkstate, GoodsNum, orderAction);

                        /************************第四步***************************/
                        //按区域位值条件查出卷筒将去往的位置坐标
                        dt = orderHelper.SelectLocation(orderArea, out tb_positionX, out tb_positionY, out tb_positionZ);
                        if (dt.Rows.Count > 0)
                        {
                            //查出工位坐标绑定到
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                dgvPosition.DataSource = dt;//将查询结果委托显示在主窗体  
                                txtSteps.Text = "第四步"; //执行步奏
                            }));
                        }
                        else
                        {
                            MessageBox.Show("数据库查询发生错误！");
                            return;
                        }
                        //放
                        catch_or_realse = 1;
                        //调用动作方法，PLC自动执行一系列动作
                        DoStepsAction(tb_positionX, tb_positionY, tb_positionZ, catch_or_realse);
                        //执行完成后更新库记录

                        //把卷筒编号及去往的区域存入临时缓存区中
                        //然后将卷筒编号及物料的编号和去往的位置编号放进全局缓存区集合中
                        if (!listNumBuffer.ContainsKey(orderArea))
                        {
                            //集合中没有对应位置的缓存记录，直接添加进去
                            listNumBuffer.Add(orderArea, EmptyNum + "," + GoodsNum);
                        }
                        else
                        {
                            //集合中有对应位置的缓存记录，先移除再添加
                            listNumBuffer.Remove(orderArea);
                            //再把最新的键值添加进去
                            listNumBuffer.Add(orderArea, EmptyNum + "," + GoodsNum);
                        }

                        //同时将关于卷筒去向的工位信息直接更新到数据库中存储
                        orderHelper.insertBuffer(orderArea, EmptyNum, GoodsNum);

                        dataListrequest.Dequeue();//再移除主请求队列第一个指令元素

                    }
                    else //无空位
                    {

                        dataListrequest.Dequeue();//移除主请求队列第一个指令元素
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            dgvPosition.DataSource = dt;//将查询结果委托显示在主窗体
                            txtErrorPut.Text = "库中已无空位置";//将查询结果委托显示在主窗体 

                        }));

                        //提示到触摸屏  库中无空位置
                        //OperateResult result = frmSieMens.siemensTcpNet1.Write("M521", 0x04);
                        return;

                    }

                }


            }


       }

        private void DoStepsAction(uint tb_positionX, ushort tb_positionY, ushort tb_positionZ,Byte write_catch_release)
        {
            //此时需要将传送带的X,Y写入PLC，之后等待PLC执行完成信号
            Write_PLC_Dest_X(tb_positionX);
            Write_PLC_Dest_Y(tb_positionY);
            //x,y使能信号
            Open_WriteX_Enable_Signal();
            Open_WriteY_Enable_Signal();
            //新步奏写入标志
            Write_PLC_New_Flag();
            Steps_Change = 1;
            mEvent.WaitOne();
           

            //再下降
            Write_PLC_Dest_Z(tb_positionZ);
            Open_WriteZ_Enable_Signal();
            //新步奏写入标志
            Write_PLC_New_Flag();
            mEvent.WaitOne();

            //执行抓取或者放下动作
            Catch_Release_Enable_Signal(write_catch_release);
            //新步奏写入标志
            Write_PLC_New_Flag();
            mEvent.WaitOne();

            //再执行上升回到安全高度
            Lift_Safe_Height();
            Open_WriteZ_Enable_Signal();
            //新步奏写入标志
            Write_PLC_New_Flag();
            mEvent.WaitOne();
            //等待
        }









        //任务执行状态反馈回调线程
        private void TaskCallBack()
        {
            while (true)
            {
                if (dataListrequest.Count >= 1 || startExchageLib)
                {
                    //进行一系列状态到位反馈，然后任务线程才能继续往下执行
                    //X,Y到位信号
                    if (S71KConnect.Usertype.X_Arrive_Signal == 1 && S71KConnect.Usertype.Y_Arrive_Signal == 1)
                    {
                        if ((S71KConnect.Usertype.PLC_Current_X - Write_Dest_X <= 10) && ((int)S71KConnect.Usertype.PLC_Current_X - (int)Write_Dest_X >= -10) && (S71KConnect.Usertype.PLC_Current_Y - Write_Dest_Y <= 10) && ((int)S71KConnect.Usertype.PLC_Current_Y - (int)Write_Dest_Y >= -10))
                        {
                            if (Steps_Change == 1 && Write_New_Flag == 1)
                            {
                                //关闭X,Y使能
                                Close_WriteX_Enable_Signal();
                                Close_WriteY_Enable_Signal();
                                Clear_PLC_New_Flag();
                                Steps_Change++;
                                //调用发通知方法
                                mEvent.Set();
                                //Thread.Sleep(100);

                            }
                        }
                    }
                    //Z到位
                    if (S71KConnect.Usertype.Z_Arrive_Signal == 1)
                    {
                        if ((S71KConnect.Usertype.PLC_Current_Z - Write_Dest_Z <= 5) && ((int)S71KConnect.Usertype.PLC_Current_Z - (int)Write_Dest_Z>= -5))
                        {
                            if (Write_New_Flag == 1)
                            {
                                if (Steps_Change == 2 || Steps_Change == 4)
                                {
                                    //关闭Z使能
                                    Close_WriteZ_Enable_Signal();
                                    Clear_PLC_New_Flag();
                                    Steps_Change++;
                                    if (Steps_Change == 5)
                                    {
                                        Steps_Change = 0;
                                    }
                                    //调用发通知方法
                                    mEvent.Set();
                                    //Thread.Sleep(100);
                                }
                            }
                        }
                    }
                    //执行动作后夹具状态与要执行的动作一致
                    if (S71KConnect.Usertype.Clamp_State == Write_Catch_Release)
                    {
                        if (Steps_Change == 3 && Write_New_Flag == 1)
                        {
                            Clear_PLC_New_Flag();
                            Steps_Change++;
                            //调用发通知方法
                            mEvent.Set();
                            //Thread.Sleep(100);
                        }
                    }


                }

                if (orderhasoldcount != orderhas.Count && oldcount == dataListrequest.Count)
                {
                    orderhasoldcount = orderhas.Count;

                    if (dataListrequest.Count > 0)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            listBoxShowCurrentOrder.Items.Clear();//清除指令当前显示队列
                            foreach (string str in dataListrequest)
                            {
                                //将代码转成汉字指令
                                //再回显到任务队列
                                listBoxShowCurrentOrder.Items.Add(CurrentOrder_Exchange(InsertFormat(str, 2, " ")));
                            }
                            if (orderhas.Count > 0)
                            {
                                for (int i = 0; i < orderhas.Count; i++)
                                {
                                    //字符串每两个字符中间加个空格
                                    listBoxShowCurrentOrder.Items.Add(CurrentOrder_Exchange(InsertFormat(orderhas.GetByIndex(i).ToString(), 2, " ")));

                                }
                            }
                        }));
                    }
                    else
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            listBoxShowCurrentOrder.Items.Clear();//清除指令当前显示队列
                            if (orderhas.Count > 0)
                            {
                                for (int i = 0; i < orderhas.Count; i++)
                                {
                                    //字符串每两个字符中间加个空格
                                    listBoxShowCurrentOrder.Items.Add(CurrentOrder_Exchange(InsertFormat(orderhas.GetByIndex(i).ToString(), 2, " ")));

                                }
                            }
                        }));

                    }
                }
                if (oldcount != dataListrequest.Count)//主队列元素个数发生变化时
                {
                    //主队列元素个数
                    oldcount = dataListrequest.Count;
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        listBoxOrder.Items.Clear();//主请求指令队列
                        listBoxShowCurrentOrder.Items.Clear();//请求指令当前显示队列
                        if (dataListrequest.Count > 0)
                        {
                            foreach (string str in dataListrequest)
                            {
                                //字符串每两个字符中间加个空格
                                listBoxOrder.Items.Add(InsertFormat(str, 2, " "));
                                ////将代码转成汉字指令
                                //再回显到任务队列
                                listBoxShowCurrentOrder.Items.Add(CurrentOrder_Exchange(InsertFormat(str, 2, " ")));

                            }
                        }
                    }));
                }



                }

        }
        /************************上位机向PLC写入的一系列分步奏动作操作****************************/
        //为了区别完成动作的重复判断使用，每次写入PLC新任务时，同时写入个新写入的标志，对应动作执行完成后，标志清零。
        private void Write_PLC_New_Flag()
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M535", 0x01);
            Write_New_Flag = 1;
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Write_PLC_New_Flag();
            }

            if (Write_New_Flag!= S71KConnect.Usertype.ReadBack_Write_New_Flag)
            {
                Write_PLC_New_Flag();
            }
        }
        //PLC反馈完成信号时，清零
        private void Clear_PLC_New_Flag()
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M535", 0x00);
            Write_New_Flag = 0;
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Clear_PLC_New_Flag();
            }
        }

        //向PLC写入需要旋转的角度，及使能信号
        private void Write_PLC_Dest_Angle(UInt16 write_dest_angle)
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M538", Write_Dest_Angle);
            Write_Dest_Angle = write_dest_angle;
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Write_PLC_Dest_Angle(write_dest_angle);
            }
        }
        //打开使能信号
        private void Open_WriteAngle_Enable_Signal()
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M533", 1);
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Open_WriteAngle_Enable_Signal();
            }
        }
        //关闭使能信号
        private void Close_WriteAngle_Enable_Signal()
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M533", 0);
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Close_WriteAngle_Enable_Signal();
            }
        }

        //向PLC写入X，及使能信号
        private void Write_PLC_Dest_X(uint write_dest_X)
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M520", write_dest_X);
            Write_Dest_X = write_dest_X;

            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Write_PLC_Dest_X(write_dest_X);
            }
            if (S71KConnect.Usertype.ReadBack_Dest_X != Write_Dest_X)
            {
                Write_PLC_Dest_X(write_dest_X);
            }
        }
        //打开使能信号
        private void Open_WriteX_Enable_Signal()
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M530", 1);
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Open_WriteX_Enable_Signal();
            }
        }
        //关闭使能信号
        private void Close_WriteX_Enable_Signal()
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M530", 0);
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Close_WriteX_Enable_Signal();
            }
        }

        //向PLC写入Y，及使能信号
        private void Write_PLC_Dest_Y(ushort write_dest_Y)
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M524", write_dest_Y);
            Write_Dest_Y = write_dest_Y;
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Write_PLC_Dest_Y(write_dest_Y);
            }
            if (S71KConnect.Usertype.ReadBack_Dest_Y != Write_Dest_Y)
            {
                Write_PLC_Dest_Y(write_dest_Y);
            }
        }
        //打开使能信号
        private void Open_WriteY_Enable_Signal()
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M531", 1);
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Open_WriteY_Enable_Signal();
            }
        }
        //关闭使能信号
        private void Close_WriteY_Enable_Signal()
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M531", 0);
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Close_WriteY_Enable_Signal();
            }
        }

        //向PLC写入Z，及使能信号
        private void Write_PLC_Dest_Z(ushort write_dest_z)
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M526", write_dest_z);
            Write_Dest_Z = write_dest_z;
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Write_PLC_Dest_Z(write_dest_z);
            }
            if (Write_Dest_Z!= S71KConnect.Usertype.ReadBack_Dest_Z)
            {
                Write_PLC_Dest_Z(write_dest_z);
            }
        }
        //打开使能信号
        private void Open_WriteZ_Enable_Signal()
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M532", 1);
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Open_WriteZ_Enable_Signal();
            }
        }
        //关闭使能信号
        private void Close_WriteZ_Enable_Signal()
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M532", 0);
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Close_WriteZ_Enable_Signal();
            }
        }



        //向PLC写入抓放动作信号
        private void Catch_Release_Enable_Signal(Byte write_catch_release)
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M534", write_catch_release);
            Write_Catch_Release = write_catch_release;
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Catch_Release_Enable_Signal(write_catch_release);
            }
            if (Write_Catch_Release != S71KConnect.Usertype.ReadBack_Catch_Release_Enable_Signal)
            {
                Catch_Release_Enable_Signal(write_catch_release);
            }
        }

        //向PLC写入起升安全高度值
        private void Lift_Safe_Height()
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M526", Safetty_Height);
            Write_Dest_Z = Safetty_Height;
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Lift_Safe_Height();
            }
            if (Write_Dest_Z != S71KConnect.Usertype.ReadBack_Dest_Z)
            {
                Lift_Safe_Height();
            }
        }

        //向PLC写入下降到池盖子的高度
        private void Down_PoolLid_Height()
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M526", PoolLid_Height);
            Write_Dest_Z = PoolLid_Height;
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Down_PoolLid_Height();
            }
        }
        //向PLC写入下降的高度
        private void Write_Down_Height(UInt16 down_height)
        {
            bool isSuccess;
            OperateResult result = S71KConnect.siemensTcpNet2_Write.Write("M526", down_height);
            Write_Dest_Z = down_height;
            isSuccess = result.IsSuccess;
            if (!isSuccess)
            {
                Write_Down_Height(down_height);
            }
        }


































  
        private void OpenChangeTxt()
        {
           
            txtGoodsCode.Clear();
            txtGoodsCode.Clear();
            txtGoodsState.Clear();
            txtAction.Clear();
            txtPosition.Clear();
            txtSteps.Clear();
            txtGWposition.Clear();
            txtActionState.Clear();
            txtlibActionState.Clear();
            txtErrorPut.Clear();
            listBoxOrder.Items.Clear();
            listBoxAction.Items.Clear();
            listBoxShowCurrentOrder.Items.Clear();

            //datagrid 清空
            dgvPosition.DataSource = null;
            //队列 清空
            dataListrequest.Clear();
            dataListrequestfu.Clear();
            if (bstartcode == false) bstartcode =true;
            oldcount = 0;
           }

        //清空指令文本
        private void bClearTxt()
        {
           if(bClear)
            {
                txtGoodsCode.Clear();
                txtGoodsState.Clear();
                txtCheckState.Clear();
                txtSelctEmptyNum.Clear();
                txtAction.Clear();
                txtPosition.Clear();
                txtGWposition.Clear();
                txtActionState.Clear();
                txtSteps.Clear();
                txtlibActionState.Clear();
                dgvPosition.DataSource = null;
                //库执行结果和异常显示不要清除
                //txtlibActionState.Clear();
                //txtErrorPut.Clear();
                bClear = false;

            }
            
        }


        //十六进制的字符串转成数组
        public static byte[] GetByteArray(string shex)
        {
            string[] ssArray = shex.Split(' ');
            List<byte> bytList = new List<byte>();
            foreach (var s in ssArray)
            {
                //将十六进制的字符串转换成数值             
                bytList.Add(Convert.ToByte(s, 16));
            }
            //返回字节数组          
            return bytList.ToArray();
       }

        /// <summary>
        /// 字符串转换16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] strToHexByte(string hexString)
         {
            hexString = hexString.Replace(" ", "");
             if ((hexString.Length % 2) != 0)
                hexString += " ";
             byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i<returnBytes.Length; i++)
                 returnBytes[i] = Convert.ToByte(hexString.Substring(i* 2, 2).Replace(" ",""), 16);
             return returnBytes;
        }
  
        //显示当前上位机串口接收数据
        
        private void receiveBackShow(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.AppendFormat("{0:x2}" + " ", data[i]);
            }
            if (!this.IsHandleCreated) return;
            //使用异步委托（BeginInvoke）
            this.BeginInvoke(new MethodInvoker(delegate
            {
            }));

        }
        //显示当前上位机应答数据
        public void sendDataBackShow(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.AppendFormat("{0:x2}" + " ", data[i]);
            }
            if (!this.IsHandleCreated) return;
            //使用异步委托（BeginInvoke）
            this.BeginInvoke(new MethodInvoker(delegate
            {
           
            }));

        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        //替换字符串指定位置的字符
        private string strReplace(string str)
        {

            //使用StringBuilder
            StringBuilder b = new StringBuilder(str);
            //将字符串第8个字符17替换为1
            b.Replace("F", "1", 7, 1); 
            str = b.ToString();
            return str;
        }

        //替换字符串指定位置的字符
        private string strReplace(string str,string replacestr)
        {

            //使用StringBuilder
            StringBuilder b = new StringBuilder(str);
            //将字符串第18个字符0替换为replacestr
            b.Replace("0", replacestr, 17, 1);
           
            str = b.ToString();
            return str;
        }


        /// <summary>  
        /// 每隔n个字符插入一个字符  
        /// </summary>  
        /// <param name="input">源字符串</param>  
        /// <param name="interval">间隔字符数</param>  
        /// <param name="value">待插入值</param>  
        /// <returns>返回新生成字符串</returns>  
        public static string InsertFormat(string input, int interval, string value)
        {
            for (int i = interval; i < input.Length; i += interval + 1)
                input = input.Insert(i, value);
            return input;
        }
        /// <summary>
        /// 输入到显示区域
        /// </summary>
        /// <param name="content"></param>
        private void AddContent(string content)
         {

            if (!this.IsHandleCreated) return;
            //使用异步委托（BeginInvoke）
            this.BeginInvoke(new MethodInvoker(delegate
            {
            }));
        }


        //点击弹出用户报表界面
        private void btnselect_Click(object sender, EventArgs e)
        {
            frmOutInRecord frmuserselect = GenericSingleton<frmOutInRecord>.CreateInstrance();
            frmuserselect.Show();
        }
     
        
        //指令界面可见不可见选择事件
        private void ckbordervisible_CheckedChanged(object sender, EventArgs e)
        {
            //指令可见
            if (ckbordervisible.CheckState == CheckState.Checked)
            { 
                //指令界面显示
                panelorder.Visible = true;
                //状态界面隐藏
                //userlibstate.Visible =false;
                mygpbExchange.Text = "后台指令";
            }
            else
            {
                //指令界面隐藏
                panelorder.Visible = false;
                //状态界面显示
                //userlibstate.Visible = true;
                mygpbExchange.Text = "库存状态";
            }
        }
        //重写listbox Drawiteam 事件
        private void listBoxOrder_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Set the DrawMode property to draw fixed sized items.
            listBoxOrder.DrawMode = DrawMode.OwnerDrawFixed;
            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Define the default color of the brush as black.
            Brush myBrush = Brushes.Black;

            // Determine the color of the brush to draw each item based on the index of the item to draw.
           if(e.Index==0)
            {
              myBrush = Brushes.Red;               
            }
           else
                myBrush = Brushes.Black;
            if (e.Index >= 0)
            {
                // Draw the current item text based on the current Font and the custom brush settings.
                e.Graphics.DrawString(listBoxOrder.Items[e.Index].ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
            }
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }
        //重写listBoxCurrentOrder Drawiteam 事件
        private void listBoxCurrentOrder_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Set the DrawMode property to draw fixed sized items.
            listBoxShowCurrentOrder.DrawMode = DrawMode.OwnerDrawFixed;
            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Define the default color of the brush as black.
            Brush myBrush = Brushes.Black;

            // Determine the color of the brush to draw each item based on the index of the item to draw.
            if (e.Index == 0)
            {
                myBrush = Brushes.Red;
            }
            else
                myBrush = Brushes.Black;

            if (e.Index >= 0)
            {
                e.Graphics.DrawString(listBoxShowCurrentOrder.Items[e.Index].ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
            }
                
                
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }
        //用个方法将listbox中的代码指令改为对应的文字指令
        private string CurrentOrder_Exchange(string str)
        {
            string strnew = "";
            string[] ss = new string[8];
            string[] sArray = str.Split(' ');
            switch (sArray[0])
            {
                case "01": ss[0] = "A1位置"; break;
                case "02": ss[0] = "A2位置"; break;
                case "03": ss[0] = "抽检区"; break;
                case "04": ss[0] = "B位置 "; break;
                default: break;
            }
            switch (sArray[1])
            {
                case "01": ss[1] = "单独"; break;
                case "02": ss[1] = "双指令"; break;

                default: break;
            }
            switch (sArray[2])
            {
                case "01": ss[2] = "取料"; break;
                case "02": ss[2] = "入库"; break;
                case "03": ss[2] = "出库"; break;
                case "04": ss[2] = "退卷"; break;
                default: break;
            }
            if (ss[2] == "入库" && ss[1] == "双指令")
            {
                ss[1] = "物料入库";
                ss[2] = "自动出空筒";
            }
            if (ss[2] == "退卷" && ss[1] == "双指令")
            {
                ss[1] = "卷筒退库";
                ss[2] = "自动上料";
            }


            switch (sArray[3])
            {
                case "01": ss[3] = "等待执行"; break;
                case "02": ss[3] = "执行完成"; break;

                default: break;
            }
            switch (sArray[4])
            {
                case "01": ss[4] = "01班组"; break;
                case "02": ss[4] = "02班组"; break;
                case "03": ss[4] = "03班组"; break;
                default: break;
            }
            if (sArray[5] == "99")
            {
                ss[5] = "物料规格： 卷筒";
            }
            else
            {
                ss[5] = "物料规格： " + sArray[5];
            }

            switch (sArray[6])
            {
                case "99": ss[6] = "空卷状态"; break;
                case "01": ss[6] = "半卷状态"; break;
                case "02": ss[6] = "满卷状态"; break;
                default: break;
            }
            if (sArray[5] != "99")
            {
                switch (sArray[7])
                {
                    case "01": ss[7] = "检测状态： 合格"; break;
                    case "02": ss[7] = "检测状态： 待检"; break;
                    case "03": ss[7] = "检测状态： 报废"; break;
                    default: break;
                }
            }
            return strnew = String.Join(",", ss);


        }


        private void listBoxAction_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Set the DrawMode property to draw fixed sized items.
            listBoxAction.DrawMode = DrawMode.OwnerDrawFixed;
            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Define the default color of the brush as black.
            Brush myBrush = Brushes.Black;

            // Determine the color of the brush to draw each item based on the index of the item to draw.
            if (e.Index == 0)
            {
                myBrush = Brushes.Red;
            }
            else
                myBrush = Brushes.Black;
            if (e.Index >= 0)
            {
                // Draw the current item text based on the current Font and the custom brush settings.
                e.Graphics.DrawString(listBoxAction.Items[e.Index].ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
            }
               // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

        //菜单查询库存记录
        private void 库存记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCurrentLib frmcurrentlib = GenericSingleton<frmCurrentLib>.CreateInstrance();
            frmcurrentlib.Show();
        }

        private void 物料出入库记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOutInRecord frmoutinrecord = GenericSingleton<frmOutInRecord>.CreateInstrance();
            frmoutinrecord.Show();
        }

        private void 空筒出入库记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEmptyRecord frmemptyrecord = GenericSingleton<frmEmptyRecord>.CreateInstrance();
            frmemptyrecord.Show();
        }

        private void 手动增删卷筒ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddEmpty frmaddempty = GenericSingleton<frmAddEmpty>.CreateInstrance();
            frmaddempty.Show();
        }

        
        //查看日志记录
        private void 查看ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormYanShi frmyanshi= GenericSingleton<FormYanShi>.CreateInstrance();
            frmyanshi.Show();
            //FormLogView fln = GenericSingleton<FormLogView>.CreateInstrance();
            //fln.Show();
        }
        //系列网口通讯
        private void 网口ToolStripMenuItem_Click(object sender, EventArgs e)
        {


            frmSieMens frmsiemens = GenericSingleton<frmSieMens>.CreateInstrance();
            s71kconnect.SendMsgEvent += frmsiemens.ShowReadContent;

           
            frmsiemens.Show();
        }


        //修改用户密码
        private void 修改密码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmChangePwr frmchangepwr = GenericSingleton<frmChangePwr>.CreateInstrance();
            frmchangepwr.Show();
        }
       
        //退出系统
        private void 退出ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //消息框中需要显示哪些按钮，此处显示“确定”和“取消”

            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;

            //"确定要退出吗？"是对话框的显示信息，"退出系统"是对话框的标题

            //默认情况下，如MessageBox.Show("确定要退出吗？")只显示一个“确定”按钮。
            DialogResult dr = MessageBox.Show("确定要退出管理系统吗?", "退出系统", messButton);

            if (dr == DialogResult.OK)//如果点击“确定”按钮
            {


                myIcon.Visible = false;

                if (sp.IsOpen)
                {
                    while (bListen1 || bListenPlc1 || bListenPlc2|| bListen3 || bListen4) Application.DoEvents();
                    trdSQLOperate.Abort();
                    sp.Close();
                    //frmSieMens.siemensTcpNet1.ConnectClose();
                    S71KConnect.siemensTcpNet2_Read.ConnectClose();
                }

                ////强制所有消息中止，退出所有的窗体，但是若有托管线程（非主线程），也无法干净地退出；
                //Application.Exit();
                //这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。
                System.Environment.Exit(0);
            }
            else
            {
                return;
            }
        }

      

        private void frmNewMain_Activated(object sender, EventArgs e)
        {
            if (frmNewMain.quanxianGet)
            {
                切换为用户模式ToolStripMenuItem.Enabled = true;
                管理员权限ToolStripMenuItem.Enabled = false;
                添加ToolStripMenuItem.Enabled = true;
                ckbordervisible.Visible = true;
            }

            else
            {
                切换为用户模式ToolStripMenuItem.Enabled = false;
                管理员权限ToolStripMenuItem.Enabled = true;
                添加ToolStripMenuItem.Enabled = false;
                ckbordervisible.Visible = false;
            }
        }
        //获得管理员权限
        private void 管理员权限ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quanxian = true;
            frmLogin frmLogin = GenericSingleton<frmLogin>.CreateInstrance();
            frmLogin.Show();
        }

        private void 切换为用户模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
       
            DialogResult dr = MessageBox.Show("确定要切换为用户模式吗?", "用户模式", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)//如果点击“确定”按钮
            {
                frmNewMain.quanxianGet = false;
                GlobalHelper._userName = "风帆电池";
              
            }
            //使用代码激活窗体焦点事件
            this.frmNewMain_Activated(this,null);
            return;
        }
        //任务队列指令右键删除功能
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBoxShowCurrentOrder.SelectedItems.Count > 0)
            { 
                //获取选定项的索引
                int orderitem = listBoxShowCurrentOrder.SelectedIndex;
                if (orderitem == 0 && dataListrequest.Count > 0)//选中正在执行的第一项
                {
                    //先不允许删除功能
                    return; 
                }
                else //等待执行的队列
                {
                    if (dataListrequest.Count == 0)
                    {
                        //选中的索引即为缓存中的元素索引
                        lock (locker)
                        {
                            //再将对应元素从字典里删除
                            if (orderhas.Count>0)
                            orderhas.RemoveAt(orderitem);
                        }

                    }
                    else
                    {

                        lock (locker)
                        {
                            if (orderhas.Count > 0)
                            //再将对应元素从字典里删除
                            orderhas.RemoveAt(orderitem-1);
                        }
                    }
                   

                }

            }    
        }








        /// <summary>
        /// 自定义一个MyComparer 继承IComparer 接口  强制返回-1 使集合不再按键排序，而是按照添加的顺序自动排序
        /// </summary>
        public static SortedList orderhas = new SortedList(new MyComparer());
        public class MyComparer : IComparer
        {
            public int Compare(object oldkey, object newkey)
            {
                //每次向键值对集合SortedList中添加新元素时，自动比较新键与旧键  
                //比较结果 oldkey>newkey return 1  oldkey==newkey return 0  oldkey<newkey return -1
                //用来判断是否包含某键
                if (oldkey.ToString()==newkey.ToString())
                {
                    return 0;
                }
                else
                return -1;
            }
        }

















    }
}
