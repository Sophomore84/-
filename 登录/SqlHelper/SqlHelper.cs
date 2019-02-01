using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  风帆电池
{
    public static class SqlHelper
    {
        //定义一个连接字符串
        //readonly修饰的变量，只能在初始化的时候赋值，以及在构造函数中赋值
        //其他地方只能读取不能设置值
        //private static readonly string conStr = ConfigurationManager.ConnectionStrings
        //    ["ConStr"].ConnectionString;

        private static readonly string conStr = ConfigurationManager.AppSettings["connectionstring"];

        
        //1.执行增（insert）、删（delete）、改（update）的方法
        //ExecuteNonQuery()
        public static int ExecuteNonQuery(string sql, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (pms != null)
                    {
                        cmd.Parameters.AddRange(pms);                
                    }
                    con.Open();
                    int t=cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return t;
                }
            }
        }


        //2.执行查询返回单个值的方法
        //ExecuteScalar()
        public static object ExecuteScalar(string sql, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (pms != null)
                    {
                        cmd.Parameters.AddRange(pms);
                    }
                    con.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }


        //3.执行查询，返回多行多列的方法
        //ExecuteReader()
        public static SqlDataReader ExecuteReader(string sql, params SqlParameter[] pms)
        {
            SqlConnection con = new SqlConnection(conStr);
            
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (pms != null)
                    {
                        cmd.Parameters.AddRange(pms);
                    }
                    try
                    {
                        con.Open();
                        //System.Data.CommandBehavior.CloseConnection这个枚举参数，表示将来使用完毕
                        //SqlDataReader后，在关闭reader的同时，在SqlDataReader内部会将关联的Connection关闭
                        return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

                    }
                    catch
                    {
                        con.Close();
                        con.Dispose();
                        throw;//把异常抛上去
                    }
                }
        }
        //4.查询数据返回DataTable
        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] pms)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conStr))
            {
                if (pms != null)
                {
                   //把可变参数加到内部封装的Cmmand中
                    adapter.SelectCommand.Parameters.AddRange(pms);
                }
                adapter.Fill(dt);

            }
            return dt;

        }



    }
}
