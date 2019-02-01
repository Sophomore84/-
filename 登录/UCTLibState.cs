using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace  风帆电池
{
    public partial class UCTLibState : UserControl
    {
        public static int libEmptycount;
        public static string LibAreaName = null;
        //AutoSizeFormClass asc = new AutoSizeFormClass();
        public UCTLibState()
        {
            InitializeComponent();
            timer2.Start();
        }

        private void UCTLibState_SizeChanged(object sender, EventArgs e)
        {
            //asc.controlAutoSize(this);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //timer2.Stop();
            string s1 = @" select  count(*) from 库存记录表 as T where  T.检测状态='合格'";
            DataTable dt1 = SqlHelper.ExecuteDataTable(s1);
            lalhege.Text = dt1.Rows[0][0].ToString();

            string s2 = @" select  count(*) from 库存记录表 as T where  T.检测状态='待检'";
            DataTable dt2 = SqlHelper.ExecuteDataTable(s2);
            laldaijian.Text = dt2.Rows[0][0].ToString();

            string s3 = @" select  count(*) from 库存记录表 as T where  T.检测状态='报废'";
            DataTable dt3 = SqlHelper.ExecuteDataTable(s3);
            lalbaofei.Text = dt3.Rows[0][0].ToString();

            string s4 = @" select  count(*) from 库存记录表 as T where  T.物料状态='空卷'";
            DataTable dt4 = SqlHelper.ExecuteDataTable(s4);
            lalkongjuan.Text = dt4.Rows[0][0].ToString();

            string s5 = @" select  count(*) from 库存记录表 as T where  T.物料状态 is null and T.卷筒编号 is null ";
            DataTable dt5 = SqlHelper.ExecuteDataTable(s5);
            lalkongwei.Text = dt5.Rows[0][0].ToString();
            libEmptycount = Convert.ToInt32(dt5.Rows[0][0]);
            //sql语句
            string sql = @" select T.库存位置 ,T.物料状态 from 库存记录表 as T";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);
            string[] strstate = new string[30];
            for (int i = 0; i < 30; i++)
            {
                if (dt.Rows[i]["物料状态"] is DBNull)
                    strstate[i] = null;
                else
                strstate[i] = dt.Rows[i]["物料状态"].ToString();

            }
            int a = 0;
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is PictureBox)
                {
                    PictureBox mypic = ctrl as PictureBox;

                    switch (strstate[a])
                    {

                        case "空卷":
                            mypic.Image = Image.FromFile(Application.StartupPath + "//仓库状态/1.jpg");//相对路径查找
                            break;
                        case "半卷":
                            mypic.Image = Image.FromFile(Application.StartupPath + "//仓库状态/2.jpg");
                            break;
                        case "满卷":
                            mypic.Image = Image.FromFile(Application.StartupPath + "//仓库状态/3.jpg");
                            break;
                        case null:
                            mypic.Image = null;
                            break;
                        default: break;
                    }
                    a++;
                    if (a >= 30) break;
                }
            }
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {

            string str=orderHelper.SelectToolTip("C01");

            toolTip1.SetToolTip(this.C01, str);
        }

        
        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C02");
            toolTip1.SetToolTip(this.C02, str);
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C03");
            toolTip1.SetToolTip(this.C03, str);
        }

        private void pictureBox4_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C04");
            toolTip1.SetToolTip(this.C04, str);
        }

        private void pictureBox5_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C05");
            toolTip1.SetToolTip(this.C05, str);
        }

        private void pictureBox6_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C06");
            toolTip1.SetToolTip(this.C06, str);
        }

        private void pictureBox7_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C07");
            toolTip1.SetToolTip(this.C07, str);
        }

        private void pictureBox8_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C08");
            toolTip1.SetToolTip(this.C08, str);
        }
        private void pictureBox9_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C09");
            toolTip1.SetToolTip(this.C09, str);
        }
        private void pictureBox10_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C10");
            toolTip1.SetToolTip(this.C10, str);
        }
        private void pictureBox11_MouseEnter(object sender, EventArgs e)
        {

            string str = orderHelper.SelectToolTip("C11");

            toolTip1.SetToolTip(this.C11, str);
        }


        private void pictureBox12_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C12");
            toolTip1.SetToolTip(this.C12, str);
        }

        private void pictureBox13_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C13");
            toolTip1.SetToolTip(this.C13, str);
        }

        private void pictureBox14_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C14");
            toolTip1.SetToolTip(this.C14, str);
        }

        private void pictureBox15_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C15");
            toolTip1.SetToolTip(this.C15, str);
        }

        private void pictureBox16_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C16");
            toolTip1.SetToolTip(this.C16, str);
        }

        private void pictureBox17_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C17");
            toolTip1.SetToolTip(this.C17, str);
        }

        private void pictureBox18_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C18");
            toolTip1.SetToolTip(this.C18, str);
        }
        private void pictureBox19_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C19");
            toolTip1.SetToolTip(this.C19, str);
        }
        private void pictureBox20_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C20");
            toolTip1.SetToolTip(this.C20, str);
        }
        private void pictureBox21_MouseEnter(object sender, EventArgs e)
        {

            string str = orderHelper.SelectToolTip("C21");

            toolTip1.SetToolTip(this.C21, str);
        }


        private void pictureBox22_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C22");
            toolTip1.SetToolTip(this.C22, str);
        }

        private void pictureBox23_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C23");
            toolTip1.SetToolTip(this.C23, str);
        }

        private void pictureBox24_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C24");
            toolTip1.SetToolTip(this.C24, str);
        }

        private void pictureBox25_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C25");
            toolTip1.SetToolTip(this.C25, str);
        }

        private void pictureBox26_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C26");
            toolTip1.SetToolTip(this.C26, str);
        }

        private void pictureBox27_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C27");
            toolTip1.SetToolTip(this.C27, str);
        }

        private void pictureBox28_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C28");
            toolTip1.SetToolTip(this.C28, str);
        }
        private void pictureBox29_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C29");
            toolTip1.SetToolTip(this.C29, str);
        }
        private void pictureBox30_MouseEnter(object sender, EventArgs e)
        {
            string str = orderHelper.SelectToolTip("C30");
            toolTip1.SetToolTip(this.C30, str);
        }

        private void conMenu1_Opening(object sender, CancelEventArgs e)
        {
            string whichcontrol_name = (sender as ContextMenuStrip).SourceControl.Name;
            LibAreaName = whichcontrol_name;
            //MessageBox.Show(LibAreaName);
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmNewMain.quanxianGet)//获得管理员权限后
            {
                //弹出详细信息界面，
                frmRightChange frmrightchange = GenericSingleton<frmRightChange>.CreateInstrance();
                //frmrightchange.Libareaname = LibAreaName;
                frmrightchange.Show();
            }
            else
                return;

        }
        //获得管理员权限后具有倒库功能  1将物料从A放到B(空位)，2将物料A与物料B进行位置调换
        private void 倒库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmNewMain.quanxianGet)//获得管理员权限后
            {
                string libstate = null;
                orderHelper.SelectToolTip(LibAreaName, out libstate);
                if (libstate != "空位" && libEmptycount>0)
                {
                    //弹出详细信息界面，
                    frmLibExchange frmlibexchange = GenericSingleton<frmLibExchange>.CreateInstrance();
                    frmlibexchange.Show();
                }
                
                else
                    return;
            }
            else
                return;
        }

        private void 自动修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAutoAlter frmautoalter = GenericSingleton<frmAutoAlter>.CreateInstrance();
            frmautoalter.Show();
        }
    }
}
