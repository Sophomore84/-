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
using 风帆电池;

namespace 风帆电池
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //一种 登录方式
            #region  
            //// 1.采集数据
            //string loginname = txtUserName.Text.Trim();
            //string password = txtPassWord.Text;
            ////2.连接数据验证是否 登录成功
            //string constr = "server=192.168.0.193\\SQLEXPRESS;database=称重数据库;uid=sa; pwd=02090221abc;MultipleActiveResultSets=true;";
            //using (SqlConnection con = new SqlConnection(constr))
            //{
            //    //sql语句   字符串加单引号
            //    string sql = string.Format("select count(*) from  登录表 where   登录名='{0}' and 密码='{1}'", loginname,password);


            //    //创建执行SQL语句的commad对象
            //    using (SqlCommand cmd = new SqlCommand(sql, con))
            //    {
            //        //执行前先打开连接
            //        con.Open();
            //        //不会返回null;因为sql语句使用了聚合函数
            //        int count =(int)cmd.ExecuteScalar();
            //        if (count > 0)
            //        {
            //            MessageBox.Show(" 登录成功");
            //            btnChangePwd.Enabled = true;
            //            txtUserName.Clear();// 登录失败时，文本内容清空
            //            txtPassWord.Clear();
            //            txtUserName.Focus();//且为用户名空间获得输入焦点。  

            //        }
            //        else
            //        {
            //            MessageBox.Show(" 登录失败");
            //            txtUserName.Clear();// 登录失败时，文本内容清空
            //            txtPassWord.Clear();
            //            txtUserName.Focus();//且为用户名空间获得输入焦点。  
            //        }
            //    }
            //}
            #endregion

            //另一种 登录方式 
            // 1.采集数据
            string loginname = txtUserName.Text.Trim();
            string password = txtPassWord.Text;
            //2.连接数据验证是否 登录成功
            string constr = ConfigurationManager.AppSettings["connectionstring"];
            using (SqlConnection con = new SqlConnection(constr))
            {
                //sql语句   字符串加单引号
                string sql = string.Format("select * from  用户账号表 where   账号='{0}' ", loginname);
                //创建执行SQL语句的commad对象
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    //执行前先打开连接
                    con.Open();
                    //不会返回null;因为sql语句使用了聚合函数
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            //存在该用户
                            if (reader.Read())
                            {
                                //如果有对应的用户再验证密码是否正确
                                //根据索引获取密码   如过密码有可能为空时使用三元表达式
                                string psWord = reader.IsDBNull(1) ? null : reader.GetString(1);
                                if (psWord == password)
                                {
                                   
                                    //根据reader索引获取用户名
                                    GlobalHelper._userName = reader.IsDBNull(0) ? null : reader.GetString(0);
                                    if (!frmNewMain.Quanxian)
                                    {
                                        this.DialogResult = DialogResult.OK;
                                        this.Text = "密码正确";
                                       
                                        //如果是管理员登录
                                        if (GlobalHelper._userName == "admin")
                                        {
                                            MessageBox.Show("登录成功，欢迎获得管理员权限");
                                            frmNewMain.quanxianGet = true;
                                        }
                                        else
                                        {
                                            frmNewMain.quanxianGet = false;
                                            MessageBox.Show("登录成功，欢迎进入应用程序");
                                            
                                        }
                                        this.Close();
                                    }
                                    else
                                    {
                                        //在系统内如果不是是管理员登录
                                        if (GlobalHelper._userName != "admin")
                                        {
                                            MessageBox.Show("非管理员账号");
                                            return;
                                        }
                                        //在系统内获得管理员权限
                                        if (GlobalHelper._userName == "admin")
                                        {
                                            MessageBox.Show("欢迎获得管理员权限");
                                            frmNewMain.quanxianGet = true;
                                            this.Close();
                                        }

                                    }
                                    
                                }
                                else
                                {
                                    this.Text = "密码错误";
                                    txtPassWord.Clear();
                                    txtPassWord.Focus();//且为用户名空间获得输入焦点。  
                                }
                            }
                        }

                        else
                        {
                            this.Text = "用户名不存在";
                            txtUserName.Clear();// 登录失败时，文本内容清空
                            txtPassWord.Clear();
                            txtUserName.Focus();//且为用户名空间获得输入焦点。
                        }

                    }
                }
            }

        }


        private void btnCancle_Click(object sender, EventArgs e)
        {
            txtUserName.Clear();// 登录失败时，文本内容清空
            txtPassWord.Clear();
            txtUserName.Focus();//且为用户名空间获得输入焦点。
        }


        private void frmLogin_Activated(object sender, EventArgs e)
        {
            txtPassWord.Focus();
            if (frmNewMain.Quanxian)
            {
                txtUserName.Clear();//文本内容清空
                txtPassWord.Clear();
                txtUserName.Focus();//且为用户名空间获得输入焦点。
            }
        }
    }
}
