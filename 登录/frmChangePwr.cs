using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace  风帆电池
{
    public partial class frmChangePwr : FormBase
    {
        public frmChangePwr()
        {
            InitializeComponent();
            FormExIni();
            _systemButtonManager = new SystemButtonManager(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string oldPwr = txtOldpwr.Text;
            string newPwr1 = txtNewPwr1.Text;
            string newPwr2 = txtNewPwr2.Text;
            //验证两次输入的密码是否一致
            if (newPwr1 == newPwr2)
            {
                //验证旧密码是否正确，先获取 登录的用户是谁
                if (checkUserPassword(oldPwr, GlobalHelper._userName))
                {
                    //修改密码
                    if (updatePassWord(newPwr2, GlobalHelper._userName))
                    {
                        MessageBox.Show("更新密码成功");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("更新密码失败");
                    }

                }
                else
                {
                    MessageBox.Show("旧密码输入错误");
                }

            }
            else
            {
                MessageBox.Show("两次输入的新密码不一致！");
            }




        }

        private bool updatePassWord(string newPwr, string _userName)
        {
            //1.连接数据库字符串
            string constr = ConfigurationManager.AppSettings["connectionstring"];
            using (SqlConnection con = new SqlConnection(constr))
            {
                //sql语句   字符串加单引号
                string sql = string.Format("update  用户账号表 set  密码='{0}'  where  账号='{1}' ", newPwr, _userName);


                //创建执行SQL语句的commad对象
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    //执行前先打开连接
                    con.Open();
                    int r = cmd.ExecuteNonQuery();
                    return r > 0;

                }
            }

        }

        //检验旧密码是否正确方法
        private bool checkUserPassword(string oldPwr, string _userName)
        {
            //1.连接数据库字符串
            string constr = ConfigurationManager.AppSettings["connectionstring"];
            using (SqlConnection con = new SqlConnection(constr))
            {
                //sql语句   字符串加单引号
                string sql = string.Format("select count(*) from  用户账号表 where  账号='{0}'and 密码='{1}' ", _userName, oldPwr);


                //创建执行SQL语句的commad对象
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    //执行前先打开连接
                    con.Open();
                    int r = (int)cmd.ExecuteScalar();
                    return r > 0;

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtOldpwr.Clear();// 登录失败时，文本内容清空
            txtNewPwr1.Clear();
            txtNewPwr2.Clear();
            txtOldpwr.Focus();//且为用户名空间获得输入焦点。
        }



    }
}
