using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace  风帆电池
{
    public static class orderHelper
    {

        //出库执行查询卷筒返回Datetable,输出位置，坐标及卷筒编号
        public static DataTable SelectOutEmpty(string goodsCode, string goodsState,
          out UInt32 X, out UInt16 Y, out UInt16 Z, out string PositionName,out string EmptyNum,out string GoodsNum)
        {
            X = 0;
            Y = 0;
            Z = 0;
            PositionName = null;
            EmptyNum = null;
            GoodsNum = null;
            DataTable dt = null;
            //此时需要连接数据库查询库中满足条件的位置，将货物编号，规格状态传进SQL语句参数中          
            string sqlSelect = ConfigurationManager.ConnectionStrings
           ["sqlOutEmpty"].ConnectionString;

            SqlParameter[] pms = new SqlParameter[]
            {
                            new SqlParameter("@goodscode",SqlDbType.VarChar,50) { Value="卷筒"},
                            new SqlParameter("@goodsstate",SqlDbType.VarChar,50) { Value="空卷"}
            };
            dt = SqlHelper.ExecuteDataTable(sqlSelect, pms);
            if (dt.Rows.Count > 0) //查到结果
            {
                EmptyNum= dt.Rows[0]["卷筒编号"].ToString();
                GoodsNum = dt.Rows[0]["生产批号"] is DBNull? null:dt.Rows[0]["生产批号"].ToString();
                PositionName = dt.Rows[0]["库存位置"].ToString();
                X = Convert.ToUInt32(dt.Rows[0]["X坐标"]);
                Y = Convert.ToUInt16(dt.Rows[0]["Y坐标"]);
                Z = Convert.ToUInt16(dt.Rows[0]["Z坐标"]);
            }

            return dt;
        }

        //入库自动执行出库查询卷筒返回Datetable,输出位置，坐标及卷筒编号
        public static DataTable SelectOutEmpty(out ushort? X, out ushort? Y, out ushort? Z,
            out string PositionName, out string EmptyNum, out string GoodsNum)
        {
            X = null;
            Y = null;
            Z = null;
            PositionName = null;
            EmptyNum = null;
            GoodsNum = null;
            DataTable dt = null;
            //此时需要连接数据库查询库中满足条件的位置，将货物编号，规格状态传进SQL语句参数中          
            string sqlSelect = ConfigurationManager.ConnectionStrings
           ["sqlAutoOutEmpty"].ConnectionString;
            dt = SqlHelper.ExecuteDataTable(sqlSelect);
            if (dt.Rows.Count > 0) //查到结果
            {
                EmptyNum = dt.Rows[0]["卷筒编号"].ToString();
                GoodsNum = dt.Rows[0]["生产批号"].ToString();
                PositionName = dt.Rows[0]["库存位置"].ToString();
                X = (ushort)Convert.ToInt32(dt.Rows[0]["X坐标"]);
                Y = (ushort)Convert.ToInt32(dt.Rows[0]["Y坐标"]);
                Z = (ushort)Convert.ToInt32(dt.Rows[0]["Z坐标"]);
            }

            return dt;
        }


        //出库根据物料规格，状态及检测状态执行查询返回Datetable,输出位置，坐标,卷筒编号，物料规格
        public static DataTable SelectOutGoods(string goodsCode, string goodsState, string checkstate,
          out UInt32 X, out UInt16 Y, out UInt16 Z, out string PositionName, out string EmptyNum, out string GoodsNum)
        {
            X = 0;
            Y = 0;
            Z = 0;
            PositionName = null;
            EmptyNum = null;
            GoodsNum = null;
            DataTable dt = null;
            //此时需要连接数据库查询库中满足条件的位置，将货物编号，规格状态传进SQL语句参数中          
            string sqlSelect = ConfigurationManager.ConnectionStrings
           ["sqlOutGoods"].ConnectionString;

            SqlParameter[] pms = new SqlParameter[]
            {
                            new SqlParameter("@goodscode",SqlDbType.VarChar,50) { Value=goodsCode},
                            new SqlParameter("@goodsstate",SqlDbType.VarChar,50) { Value=goodsState},
                            new SqlParameter("@checkstate",SqlDbType.VarChar,50) { Value=checkstate}
            };
            dt = SqlHelper.ExecuteDataTable(sqlSelect, pms);
            if (dt.Rows.Count > 0) //查到结果
            {
                EmptyNum = dt.Rows[0]["卷筒编号"].ToString();
                GoodsNum = dt.Rows[0]["生产批号"].ToString();
                PositionName = dt.Rows[0]["库存位置"].ToString();
                X = Convert.ToUInt32(dt.Rows[0]["X坐标"]);
                Y = Convert.ToUInt16(dt.Rows[0]["Y坐标"]);
                Z = Convert.ToUInt16(dt.Rows[0]["Z坐标"]);

            }

            return dt;
        }


      
        //根据区域编号查询出工位坐标值，返回Datetable,输出工位名及坐标
        public static DataTable SelectLocation(string orderarea, out UInt32 X, out UInt16 Y, out UInt16 Z)
        {
            X = 0;
            Y = 0;
            Z = 0;
            DataTable dt = null;
            string sqlSelect = ConfigurationManager.ConnectionStrings
          ["sqlOutLocation"].ConnectionString;

            SqlParameter[] pms = new SqlParameter[]
            {
                            new SqlParameter("@orderarea",SqlDbType.VarChar,50) { Value=orderarea}
            };
            dt = SqlHelper.ExecuteDataTable(sqlSelect, pms);
            if (dt.Rows.Count > 0) //查到结果
            {
                X = Convert.ToUInt32(dt.Rows[0]["X坐标"]);
                Y = Convert.ToUInt16(dt.Rows[0]["Y坐标"]);
                Z = Convert.ToUInt16(dt.Rows[0]["Z坐标"]);
            }

            return dt;
        }


        //入库退库前先按条件查询库中是否有空位坐标值，返回Datetable
        public static DataTable SelectEmptyLocation()
        {
            string sqlSelect = ConfigurationManager.ConnectionStrings
          ["sqlOutEmptyLocation"].ConnectionString;

            DataTable dt = SqlHelper.ExecuteDataTable(sqlSelect);
            return dt;
        }
        //入库退库前先按条件查询库中是否有空位坐标值，返回Datetable
        public static DataTable SelectEmptyLocation(out string positionName)
        {
            positionName = null;
            string sqlSelect = ConfigurationManager.ConnectionStrings
          ["sqlOutEmptyLocation"].ConnectionString;

            DataTable dt = SqlHelper.ExecuteDataTable(sqlSelect);
            if (dt.Rows.Count > 0) //查到结果
            {
                positionName = dt.Rows[0]["库存位置"].ToString();
            }
            return dt;
        }


        //查询出库中空位坐标值，返回Datetable,输出库位名及坐标
        public static DataTable SelectEmptyLocation(out UInt32 X, out UInt16 Y, out UInt16 Z, out string PositionName)
        {
            X = 0;
            Y = 0;
            Z = 0;
            DataTable dt = null;
            PositionName = null;
            string sqlSelect = ConfigurationManager.ConnectionStrings
          ["sqlOutEmptyLocation"].ConnectionString;

            dt = SqlHelper.ExecuteDataTable(sqlSelect);
            if (dt.Rows.Count > 0) //查到结果
            {
                PositionName = dt.Rows[0]["库存位置"].ToString();
                X = Convert.ToUInt32(dt.Rows[0]["X坐标"]);
                Y = Convert.ToUInt16(dt.Rows[0]["Y坐标"]);
                Z = Convert.ToUInt16(dt.Rows[0]["Z坐标"]);
            }

            return dt;
        }


        //向出入库记录表执行插入语句
        public static int InsertOutInRecord(object outinrecordDate, object outinrecordTime, object outinrecordPosition, object outinrecordEmptyNum,
            object outinrecordGoodscode, object outinrecordGoodsState,object outinrecordCheckState, object outinrecordGoodsNum, object outinrecordOutInstate)
        {

            //sql语句  insert into  
            string sqlinsert = ConfigurationManager.ConnectionStrings
           ["insertoutinlibrecord"].ConnectionString;

            SqlParameter[] pms = new SqlParameter[]
            {
                new SqlParameter("@date",SqlDbType.Date) { Value=outinrecordDate==null?DBNull.Value:outinrecordDate},
                new SqlParameter("@time",SqlDbType.Time) { Value=outinrecordTime==null?DBNull.Value:outinrecordTime},
                new SqlParameter("@goodscode",SqlDbType.VarChar,50) { Value=outinrecordGoodscode==null?DBNull.Value:outinrecordGoodscode},
                new SqlParameter("@goodsstate",SqlDbType.VarChar,50) { Value=outinrecordGoodsState==null?DBNull.Value:outinrecordGoodsState},
                new SqlParameter("@checkstate",SqlDbType.VarChar,50) { Value=outinrecordCheckState==null?DBNull.Value:outinrecordCheckState},
                new SqlParameter("@goodsnum",SqlDbType.VarChar,50) { Value=outinrecordGoodsNum==null?DBNull.Value:outinrecordGoodsNum},
                new SqlParameter("@emptynum",SqlDbType.VarChar,50) { Value=outinrecordEmptyNum==null?DBNull.Value:outinrecordEmptyNum},
                new SqlParameter("@position",SqlDbType.VarChar,50) { Value=outinrecordPosition},
                new SqlParameter("@outinstate",SqlDbType.VarChar,50) { Value=outinrecordOutInstate==null?DBNull.Value:outinrecordOutInstate}
            };
            return SqlHelper.ExecuteNonQuery(sqlinsert, pms);

        }





        /*****************************************待删除的代码******************************/
        ////入库（退库）时向库存记录表执行插入语句
        public static int InsertLibInRecord(string outinrecordDate, string outinrecordTime, string outinrecordGoodscode, string outinrecordGoodsState,
            string outinrecordCheckState, string outinrecordGoodsNum, string outinrecordEmptyNum, string outinrecordPosition, string outinrecordNotes)
        {

            //sql语句  insert into   
            string sqlinsert = ConfigurationManager.ConnectionStrings
           ["insertlibinrecord"].ConnectionString;

            SqlParameter[] pms = new SqlParameter[]
            {
                    new SqlParameter("@date",SqlDbType.Date) { Value=outinrecordDate},
                    new SqlParameter("@time",SqlDbType.Time) { Value=outinrecordTime},
                    new SqlParameter("@goodscode",SqlDbType.VarChar,50) { Value=outinrecordGoodscode},
                    new SqlParameter("@goodsstate",SqlDbType.VarChar,50) { Value=outinrecordGoodsState},
                    new SqlParameter("@checkstate",SqlDbType.VarChar,50) { Value=outinrecordCheckState},
                    new SqlParameter("@goodsnum",SqlDbType.VarChar,50) { Value=outinrecordGoodsNum==null?DBNull.Value:(object)outinrecordGoodsNum},
                    new SqlParameter("@emptynum",SqlDbType.VarChar,50) { Value=outinrecordEmptyNum},
                    new SqlParameter("@position",SqlDbType.VarChar,50) { Value=outinrecordPosition},
                    new SqlParameter("@notes",SqlDbType.VarChar,50) { Value=outinrecordNotes}
            };
            return SqlHelper.ExecuteNonQuery(sqlinsert, pms);

        }

        ////出库时根据查询出的库存位置向库存记录表执行删除一条记录
        public static int DeleteLibInRecord(string outinrecordPosition)
        {
            //sql语句  delete   
            string sqldelete = ConfigurationManager.ConnectionStrings
           ["deletelibinrecord"].ConnectionString;

            SqlParameter[] pms = new SqlParameter[]
            {

                    new SqlParameter("@position",SqlDbType.VarChar,50) { Value=outinrecordPosition}

            };
            return SqlHelper.ExecuteNonQuery(sqldelete, pms);

        }
      /***********************************************************************/
 






        //出库时根据查询出的库存位置向 库存记录表 执行更新库存中一条记录
        public static int UpdateLibInRecord(object outinrecordPosition,object outinrecordEmptyNum, object outinrecordGoodscode, object outinrecordGoodsState, object outinrecordCheckState,
             object outinrecordGoodsNum, object outinrecordDate, object outinrecordTime ,object outinrecordNotes)
        {
            //sql语句  delete   
            string sqlupdate = ConfigurationManager.ConnectionStrings
           ["updatelibrecord"].ConnectionString;

            SqlParameter[] pms = new SqlParameter[]
            {
                new SqlParameter("@date",SqlDbType.Date) { Value=outinrecordDate==null?DBNull.Value:outinrecordDate},
                new SqlParameter("@time",SqlDbType.Time) { Value=outinrecordTime==null?DBNull.Value:outinrecordTime},
                new SqlParameter("@goodscode",SqlDbType.VarChar,50) { Value=outinrecordGoodscode==null?DBNull.Value:outinrecordGoodscode},
                new SqlParameter("@goodsstate",SqlDbType.VarChar,50) { Value=outinrecordGoodsState==null?DBNull.Value:outinrecordGoodsState},
                new SqlParameter("@checkstate",SqlDbType.VarChar,50) { Value=outinrecordCheckState==null?DBNull.Value:outinrecordCheckState},
                new SqlParameter("@goodsnum",SqlDbType.VarChar,50) { Value=outinrecordGoodsNum==null?DBNull.Value:outinrecordGoodsNum},
                new SqlParameter("@emptynum",SqlDbType.VarChar,50) { Value=outinrecordEmptyNum==null?DBNull.Value:outinrecordEmptyNum},
                new SqlParameter("@notes",SqlDbType.VarChar,50) { Value=outinrecordNotes==null?DBNull.Value:outinrecordNotes},
                new SqlParameter("@position",SqlDbType.VarChar,50) { Value=outinrecordPosition}

            };
            return SqlHelper.ExecuteNonQuery(sqlupdate, pms);

        }



        //出库、入库时根据查询出的库存位置更新 库位状态表 中库位状态
        public static int UpdateLibState(string outinrecordPosition)
        {
            //sql语句  update  
            string sqlupdate = ConfigurationManager.ConnectionStrings
           ["updatelibstate"].ConnectionString;

            SqlParameter[] pms = new SqlParameter[]
            {

                new SqlParameter("@position",SqlDbType.VarChar,50) { Value=outinrecordPosition}

            };
            return SqlHelper.ExecuteNonQuery(sqlupdate, pms);

        }








        


        //手动向库存记录表中执行删除语句，删除卷筒编号及卷筒上所有信息，只留下库存位置
        public static int DeleteEmptyCodeRecord(string emptycode)
        {

            SqlParameter[] pms = new SqlParameter[]
            {

                new SqlParameter("@emptycode",SqlDbType.VarChar,50) { Value=emptycode}

            };
            //sql语句 实际上执行的是update语句  
            string sqldelete = ConfigurationManager.ConnectionStrings
           ["deletelibemptycode"].ConnectionString;
            int t = SqlHelper.ExecuteNonQuery(sqldelete, pms);

            return t;
        }


        //手动向库存记录表中执行更新语句，插入空卷筒
        public static int InsertEmptyCodeRecord(string emptycode,string position)
        {
            //sql语句 实际上执行的是update语句  
            string sqlinsert = ConfigurationManager.ConnectionStrings
           ["insertlibemptycode"].ConnectionString;
            SqlParameter[] pms = new SqlParameter[]
            {
                new SqlParameter("@position",SqlDbType.VarChar,50) { Value=position},
                new SqlParameter("@emptycode",SqlDbType.VarChar,50) { Value=emptycode},
                new SqlParameter("@date",SqlDbType.Date) { Value=DateTime.Now.ToString("yyyy-MM-dd")},
                new SqlParameter("@time",SqlDbType.Time) { Value=DateTime.Now.ToString("HH:mm:ss")},
                new SqlParameter("@goodscode",SqlDbType.VarChar,50) { Value="卷筒"},
                new SqlParameter("@goodsstate",SqlDbType.VarChar,50) { Value="空卷"},
                new SqlParameter("@checkstate",SqlDbType.VarChar,50) { Value=DBNull.Value},
                new SqlParameter("@goodsnum",SqlDbType.VarChar,50) { Value=DBNull.Value},
                
                new SqlParameter("@notes",SqlDbType.VarChar,50) { Value="入库"},
                

            };
            
            int t = SqlHelper.ExecuteNonQuery(sqlinsert, pms);

            return t;
        }

        //手动向物料表中执行插入语句，新增物料规格
        public static int InsertGoodsCodeRecord(string goodscode)
        {

            //sql语句  insert into  
            string sqlinsert = ConfigurationManager.ConnectionStrings
           ["insertgoodscode"].ConnectionString;

            SqlParameter[] pms = new SqlParameter[]
            {
                new SqlParameter("@goodscode",SqlDbType.VarChar,50) { Value=goodscode}

            };
            return SqlHelper.ExecuteNonQuery(sqlinsert, pms);

        }
        //手动向物料表中执行插入语句，删除物料规格
        public static int DeleteGoodsCodeRecord(string goodscode)
        {
            //sql语句  delete   
            string sqldelete = ConfigurationManager.ConnectionStrings
           ["deletegoodscode"].ConnectionString;

            SqlParameter[] pms = new SqlParameter[]
            {

                new SqlParameter("@goodscode",SqlDbType.VarChar,50) { Value=goodscode}

            };
            return SqlHelper.ExecuteNonQuery(sqldelete, pms);

        }


        //手动更新库位状态表中库位状态
        public static int UpdateLibState(string Position, string libstate)
        {
            string sql = "update 库状态表 set 库状态=0 where 库存位置=@position";
            if (libstate == "空位")
            {
                sql = "update 库状态表 set 库状态=0 where 库存位置=@position";
            }
            else
            {
                sql = "update 库状态表 set 库状态=1 where 库存位置=@position";
            }

            SqlParameter[] pms = new SqlParameter[]
            {

                new SqlParameter("@position",SqlDbType.VarChar,50) { Value=Position}

            };
            return SqlHelper.ExecuteNonQuery(sql, pms);

        }

        //查询当前库位状态   空卷 半卷  满卷  空位
        public static string SelectToolTip(string Position,out string posistate)
        {
            posistate = "空位";
            //此时需要连接数据库查询库中满足条件的位置，将货物编号，规格状态传进SQL语句参数中          
            string sql = ConfigurationManager.ConnectionStrings
          ["sqlLibAreaMess"].ConnectionString;
            SqlParameter[] pms = new SqlParameter[]
            {
                            new SqlParameter("@position",SqlDbType.VarChar,50) { Value=Position}
            };
            DataTable dt = SqlHelper.ExecuteDataTable(sql, pms);
            if (dt.Rows.Count > 0) //查到结果
            {
                if (dt.Rows[0]["物料规格"] is DBNull)
                {
                    posistate = "空位";
                }
                else if (dt.Rows[0]["物料规格"].ToString() == "卷筒")
                {
                    posistate =  dt.Rows[0]["物料状态"].ToString();
                }
                else
                {
                    posistate =  dt.Rows[0]["物料状态"].ToString();
                }
            }
            return posistate;


        }





        //跟据库存位置查询单个库存信息，显示到提示框
        public static string SelectToolTip(string Position)
        {
            string str = "空位";
            //此时需要连接数据库查询库中满足条件的位置，将货物编号，规格状态传进SQL语句参数中          
            string sql = ConfigurationManager.ConnectionStrings
          ["sqlLibAreaMess"].ConnectionString;
            SqlParameter[] pms = new SqlParameter[]
            {
                            new SqlParameter("@position",SqlDbType.VarChar,50) { Value=Position}
            };
            DataTable dt = SqlHelper.ExecuteDataTable(sql, pms);
            if (dt.Rows.Count > 0) //查到结果
            {
                if (dt.Rows[0]["物料规格"] is DBNull)
                {
                    str = "空位";
                }
                else if (dt.Rows[0]["物料规格"].ToString() == "卷筒")
                {
                    str = "卷筒编号:"+ dt.Rows[0]["卷筒编号"].ToString()+"\r\n"+ "物料状态:" + dt.Rows[0]["物料状态"].ToString() + "\r\n";
                }
                else
                {
                    str = "卷筒编号:" + dt.Rows[0]["卷筒编号"].ToString() + "\r\n" + "物料型号:" + dt.Rows[0]["物料型号"].ToString() + "\r\n" + "物料状态:" + dt.Rows[0]["物料状态"].ToString() + "\r\n" + "检测状态:" + dt.Rows[0]["检测状态"].ToString() + "\r\n" + "生产批号:" + dt.Rows[0]["生产批号"].ToString() + "\r\n";
                }
            }

            return str;
        }

        


        //根据库存位置查询单个库存信息，便于直观修改属性
        public static DataTable updataLibMess(string Position)
        {
            
            //此时需要连接数据库查询库中满足条件的位置，将货物编号，规格状态传进SQL语句参数中          
            string sql = ConfigurationManager.ConnectionStrings
          ["updataLibAreaMess"].ConnectionString;
            SqlParameter[] pms = new SqlParameter[]
            {
                            new SqlParameter("@position",SqlDbType.VarChar,50) { Value=Position}
            };
            DataTable dt = SqlHelper.ExecuteDataTable(sql, pms);

            return dt;
        }

        //倒库时查询单个库存位置
        public static DataTable exchangeLibMess(string Position, out UInt32 X, out UInt16 Y, out UInt16 Z)
        {
            X = 0; Y = 0; Z = 0;

            //此时需要连接数据库查询库中满足条件的位置，将货物编号，规格状态传进SQL语句参数中          
            string sql = ConfigurationManager.ConnectionStrings
          ["exchangePosition"].ConnectionString;
            SqlParameter[] pms = new SqlParameter[]
            {
                            new SqlParameter("@libposition",SqlDbType.VarChar,50) { Value=Position}
            };
            DataTable dt = SqlHelper.ExecuteDataTable(sql, pms);
            if (dt.Rows.Count > 0)
            {
                X = Convert.ToUInt32(dt.Rows[0]["X坐标"]);
                Y = Convert.ToUInt16(dt.Rows[0]["Y坐标"]);
                Z = Convert.ToUInt16(dt.Rows[0]["Z坐标"]);

            }
            return dt;
        }

        //倒库时根据库存位置查询单个库存信息，便于直观修改属性
        public static string[] exchangeLibMess(string Position)
        {
            string[] strArray = new string[8];

            //此时需要连接数据库查询库中满足条件的位置，将货物编号，规格状态传进SQL语句参数中          
            string sql = ConfigurationManager.ConnectionStrings
          ["exchangeLibAreaMess"].ConnectionString;
            SqlParameter[] pms = new SqlParameter[]
            {
                            new SqlParameter("@position",SqlDbType.VarChar,50) { Value=Position}
            };
            DataTable dt = SqlHelper.ExecuteDataTable(sql, pms);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    if(i==5)
                    strArray[i] = dt.Rows[0][i].Equals(DBNull.Value) ? null : Convert.ToDateTime(dt.Rows[0][i]).ToString("yyyy-MM-dd");
                    else
                    strArray[i] = dt.Rows[0][i].Equals(DBNull.Value) ? null : dt.Rows[0][i].ToString();
                  
                }
               
            }
            return strArray;
        }

        //倒库时根据库存位置向 库存记录表 执行更新库存中一条记录
        public static int UpdateLibInRecord(string libposition,string[] strarray)
        {
            
            //sql语句  delete   
            string sqlupdate = ConfigurationManager.ConnectionStrings
           ["updatelibrecord"].ConnectionString;

            SqlParameter[] pms = new SqlParameter[]
            {
                new SqlParameter("@position",SqlDbType.VarChar,50) { Value=libposition},
                new SqlParameter("@emptynum",SqlDbType.VarChar,50) { Value=strarray[0]==null?DBNull.Value:(object)strarray[0]},
                new SqlParameter("@goodscode",SqlDbType.VarChar,50) { Value=strarray[1]==null?DBNull.Value:(object)strarray[1]},
                new SqlParameter("@goodsstate",SqlDbType.VarChar,50) { Value=strarray[2]==null?DBNull.Value:(object)strarray[2]},
                new SqlParameter("@checkstate",SqlDbType.VarChar,50) { Value=strarray[3]==null?DBNull.Value:(object)strarray[3]},
                new SqlParameter("@goodsnum",SqlDbType.VarChar,50) { Value=strarray[4]==null?DBNull.Value:(object)strarray[4]},
                new SqlParameter("@date",SqlDbType.Date) { Value=strarray[5]==null?DBNull.Value:(object)strarray[5]},
                new SqlParameter("@time",SqlDbType.Time) { Value=strarray[6]==null?DBNull.Value:(object)strarray[6]},
                new SqlParameter("@notes",SqlDbType.VarChar,50) { Value=strarray[7]==null?DBNull.Value:(object)strarray[7]}
                

            };
            return SqlHelper.ExecuteNonQuery(sqlupdate, pms);

        }


        //出库完成时根据去往的工位位置代码，将卷筒编号信息和物料批号信息存入缓存中，返回Datetable
        public static int insertBuffer(string area, string emptynum ,string goodsnum)
        {
            string sql = ConfigurationManager.ConnectionStrings
          ["updatebuffrecord"].ConnectionString;
            SqlParameter[] pms = new SqlParameter[]
            {
                new SqlParameter("@goodsnum",SqlDbType.VarChar,50) { Value=goodsnum==null?DBNull.Value:(object)goodsnum},
                new SqlParameter("@emptynum",SqlDbType.VarChar,50) { Value=emptynum==null?DBNull.Value:(object)emptynum},
                new SqlParameter("@area",SqlDbType.VarChar,50) { Value=area}

            };
            return SqlHelper.ExecuteNonQuery(sql, pms);
        }

        //跟据工位位置代码查询关于卷筒编号的记录
        public static DataTable selectBuffer(string area,out string emptynum, out string goodsnum)
        {
            emptynum = null;
            goodsnum = null;
            //此时需要连接数据库查询库中满足条件的位置，将货物编号，规格状态传进SQL语句参数中          
            string sql = ConfigurationManager.ConnectionStrings
          ["selectbuffrecord"].ConnectionString;
            SqlParameter[] pms = new SqlParameter[]
            {
                            new SqlParameter("@area",SqlDbType.VarChar,50) { Value=area}
            };
            DataTable dt = SqlHelper.ExecuteDataTable(sql, pms);
            if (dt.Rows.Count > 0)
            {
                emptynum = dt.Rows[0]["卷筒编号"] is DBNull ? null : dt.Rows[0]["卷筒编号"].ToString();
                goodsnum = dt.Rows[0]["生产批号"] is DBNull ? null : dt.Rows[0]["生产批号"].ToString();
            }
            return dt;
         }

        //出库完成时根据去往的工位位置代码，将卷筒编号信息和物料批号信息存入缓存中，返回Datetable
        public static int clearBuffer(string area)
        {
            string sql = ConfigurationManager.ConnectionStrings
          ["clearbuffrecord"].ConnectionString;
            SqlParameter[] pms = new SqlParameter[]
            {
                new SqlParameter("@goodsnum",SqlDbType.VarChar,50) { Value=DBNull.Value},
                new SqlParameter("@emptynum",SqlDbType.VarChar,50) { Value=DBNull.Value},
                new SqlParameter("@area",SqlDbType.VarChar,50) { Value=area}

            };
            return SqlHelper.ExecuteNonQuery(sql, pms);
        }



    }
}