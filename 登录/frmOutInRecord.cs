using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Office.Interop.Excel;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace  风帆电池
{
    public partial class frmOutInRecord : FormBase
    {
        AutoSizeFormClass asc = new AutoSizeFormClass();

        public delegate void ShowDataTable(System.Data.DataTable dt);//【1】定义一个委托类型 
        public ShowDataTable showdatatable;//【2】申明一个委托对象 
        public frmOutInRecord()
        {
            InitializeComponent();
            FormExIni();
            _systemButtonManager = new SystemButtonManager(this);
        }

        private void frmUserSelect_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }

        private void frmUserSelect_Load(object sender, EventArgs e)
        {
            //dateTimePicker2.Value = System.DateTime.Now;
            cmbGoodsCode.Items.Add("全部物料");
            string sql= "select 物料规格,物料型号 from 物料表 ";
            System.Data.DataTable dt = SqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0) //查到结果
            {
                //遍历每行0列的元素填充到combox
                cmbGoodsCode.DropDownStyle = ComboBoxStyle.DropDown;
                cmbGoodsType.DropDownStyle = ComboBoxStyle.DropDown;
                foreach (DataRow dr in dt.Rows)
                {
                    cmbGoodsCode.Items.Add(dr["物料规格"].ToString());
                    cmbGoodsType.Items.Add(dr["物料型号"].ToString());
                }
                cmbGoodsCode.Items.Remove("卷筒");
            }
            cmbGoodsCode.SelectedIndex = 0;

            string sql1 = "select 物料状态 from 物料状态表 ";
            System.Data.DataTable dt1 = SqlHelper.ExecuteDataTable(sql1);
            if (dt1.Rows.Count > 0) //查到结果
            {
                //遍历每行0列的元素填充到combox 
                cmbGoodsState.DropDownStyle = ComboBoxStyle.DropDown;
                //先向物料状态表中增加个空项
                cmbGoodsState.Items.Add("");
                foreach (DataRow dr in dt1.Rows)
                {
                    cmbGoodsState.Items.Add(dr["物料状态"].ToString());
                }
                cmbGoodsState.Items.Remove("空卷");
                cmbGoodsState.SelectedIndex = 0;
            }
        }
        //物料规格选择项改变
        private void cmbGoodsCode_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            if (cmbGoodsCode.Text == "全部物料")//如果选择了全部物料
            {
                if (cmbGoodsState.Items.Count > 0)
                cmbGoodsState.SelectedIndex = 0;
                cmbGoodsType.SelectedIndex = 0;
                cmbGoodsState.Enabled = false;
                cmbGoodsType.Enabled = false;
            }
            
            else
            {

                cmbGoodsState.Enabled = true;
                cmbGoodsType.Enabled = true;
                //获取选中项的索引值
                cmbGoodsType.SelectedIndex = cmbGoodsCode.SelectedIndex;
                cmbGoodsState.SelectedIndex = 1;
            }
        }
       

        //点击查询按钮
        private void btnselect_Click(object sender, EventArgs e)
        {
            string sql = null;
            string actionstate = null;
            string goodsstate = "满卷";
            if (cmbGoodsCode.Text == "全部物料")//如果选择了全部物料
            sql = string.Format(@"select T1.序号,T1.日期,T1.时间,T1.库存位置,T1.卷筒编号,T1.物料规格,T2.物料型号,
                         T1.物料状态,T1.检测状态,T1.生产批号,T1.出入库状态
                        from 物料表 as T2
                         right join 出入库记录表 as T1
                         on T1.物料规格 = T2.物料规格
                         where T1.物料状态 != '空卷' and T1.出入库状态=@actionstate 
                         and T1.日期 between @startdate and @enddate 
                         and T1.时间 between @starttime and @endtime");
            else
            sql = string.Format(@"select T1.序号,T1.日期,T1.时间,T1.库存位置,T1.卷筒编号,T1.物料规格,T2.物料型号,
                         T1.物料状态,T1.检测状态,T1.生产批号,T1.出入库状态
                        from 物料表 as T2
                         right join 出入库记录表 as T1
                         on T1.物料规格 = T2.物料规格
                         where T1.物料状态 != '空卷' and T1.物料规格=@goodscode 
                        and T1.物料状态=@goodsstate and T1.出入库状态=@actionstate and 
                        T1.日期 between @startdate and @enddate 
                        and T1.时间 between @starttime and @endtime");


            if (cmbGoodsState.Text == "半卷") goodsstate = "半卷";
            else if (cmbGoodsState.Text == "满卷") goodsstate = "满卷";

            //动作
            if (rabtnInlib.Checked)//入库选中
            {
                actionstate = "入库";
            }
            else if (rabtnOutLib.Checked)//出库选中
            {
                actionstate = "出库";
            }
            else if (rabtBack.Checked)//退存选中
            {
                actionstate = "退库";
            }
            else if (rabtAll.Checked)//全部选中
            {
                actionstate = "全部";
                sql = string.Format(@"select T1.序号,T1.日期,T1.时间,T1.库存位置,T1.卷筒编号,T1.物料规格,T2.物料型号,
                         T1.物料状态,T1.检测状态,T1.生产批号,T1.出入库状态
                        from 物料表 as T2
                         right join 出入库记录表 as T1
                         on T1.物料规格 = T2.物料规格
                         where T1.物料状态 != '空卷'
                        and T1.日期 between @startdate and @enddate 
                        and T1.时间 between @starttime and @endtime");
            }
   
            SqlParameter[] pms = new SqlParameter[]
            {
               new SqlParameter("@goodscode",SqlDbType.VarChar,50) { Value=cmbGoodsCode.Text},
               new SqlParameter("@goodsstate",SqlDbType.VarChar,50) { Value=goodsstate},
               new SqlParameter("@actionstate",SqlDbType.VarChar,50) { Value=actionstate},
               new SqlParameter("@startdate",SqlDbType.Date) { Value=dateTimePicker1.Value.ToString("yyyy-MM-dd")},
               new SqlParameter("@enddate",SqlDbType.Date) { Value=dateTimePicker2.Value.ToString("yyyy-MM-dd")},
               new SqlParameter("@starttime",SqlDbType.Time) { Value=dateTimePicker1.Value.ToString("HH:mm:ss")},
               new SqlParameter("@endtime",SqlDbType.Time) { Value=dateTimePicker2.Value.ToString("HH:mm:ss")}
            };
            System.Data.DataTable dt = SqlHelper.ExecuteDataTable(sql, pms);
          
            //在绑定dt前调用此方法更改数据源表中某列的值
            CellChange(dt);
            //根据Header和所有单元格的内容自动调整行的高度 
            dgvselcet.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvselcet.DataSource = dt;
            //将序号改为自动编号
            for (int i = 0; i < this.dgvselcet.Rows.Count-1; i++)
            this.dgvselcet.Rows[i].Cells[0].Value = i+1;


        }


        /// 将DataGridView中的数据保存到exl中,用流保存成xls文件. 这种方法比较好,不用引用Excel组件.
        ///
        /// DataGridView
        public static void DataTableToExl(DataGridView _DataGridView)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execl files (*.xls)|*.xls";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "导出数据到本地计算机";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName == "")
                return;
            Stream myStream;
            myStream = saveFileDialog.OpenFile();
            //StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));

            string str = "";
            try
            {
                //写标题
                for (int i = 0; i < _DataGridView.ColumnCount; i++)
                {
                    if (i > 0)
                    {
                        str += "\t";
                    }
                    str += _DataGridView.Columns[i].HeaderText;
                }
                sw.WriteLine(str);
                //写内容
                for (int j = 0; j < _DataGridView.Rows.Count; j++)
                {
                    string tempStr = "";
                    for (int k = 0; k < _DataGridView.Columns.Count; k++)
                    {
                        if (k > 0)
                        {
                            tempStr += "\t";
                        }
                        if (_DataGridView.Columns[k].HeaderText=="日期")
                        {
                            tempStr += Convert.ToDateTime( _DataGridView.Rows[j].Cells[k].Value).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                          tempStr += _DataGridView.Rows[j].Cells[k].Value.ToString();
                        }
                        
                    }
                    sw.WriteLine(tempStr);
                }
                sw.Close();
                myStream.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sw.Close();
                myStream.Close();
                if (MessageBox.Show("导出成功!你想打开这个文件吗?", "导出到...", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        process.StartInfo.FileName = saveFileDialog.FileName;
                        process.StartInfo.Verb = "Open";
                        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                        process.Start();
                    }
                    catch
                    {
                        MessageBox.Show("你的计算机中未安装Excel,不能打开该文档!");
                    }
                }
            }
        }
        //需要引用DLL组件
        /// <summary>
        /// DataGridView导出至Excel,解决问题:打开Excel文件格式与扩展名指定格式不一致
        /// </summary>
        /// <param name="dataGridView">数据源表格</param>
        /// <param name="isShowExcle">导出时是否显示excel界面</param>
        /// <returns></returns>
        //public static bool DcExcel(DataGridView dataGridView, bool isShowExcle = true)
        //{
        //    int FormatNum;//保存excel文件的格式
        //    Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
        //    string excelVersion = excel.Version;//获取你使用的excel 的版本号 


        //    //声明保存对话框 
        //    SaveFileDialog saveFileDialog = new SaveFileDialog();
        //    //默然文件后缀 
        //    saveFileDialog.DefaultExt = "xls";

        //    if (Convert.ToDouble(excelVersion) < 12)//You use Excel 97-2003
        //    {
        //        FormatNum = -4143;
        //        //文件后缀列表 
        //        saveFileDialog.Filter = "Excel(*.xls)|*.xls";
        //    }
        //    else//you use excel 2007 or later
        //    {
        //        FormatNum = 56;
        //        //文件后缀列表 
        //        saveFileDialog.Filter = "Excel(*.xls)|*.xls|Excel(2007-2016)(*.xlsx)|*.xlsx";
        //    }
        //    Form fr = dataGridView.Parent as Form;
        //    if (fr != null)//默认文件名
        //    {
        //        saveFileDialog.FileName = fr.Text;
        //    }
        //    //默然路径是系统当前路径 
        //    saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
        //    //打开保存对话框 
        //    if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
        //        return false;
        //    //返回文件路径 
        //    string fileName = saveFileDialog.FileName;
        //    if (string.IsNullOrEmpty(fileName.Trim()))
        //    { return false; }



        //    if (dataGridView.Rows.Count == 0)
        //        return false;
        //    //建立Excel对象      

        //    var objWorkbook = excel.Application.Workbooks.Add(true);
        //    excel.Visible = isShowExcle;
        //    //生成字段名称      
        //    for (int i = 0; i < dataGridView.ColumnCount; i++)
        //    {
        //        excel.Cells[1, i + 1] = dataGridView.Columns[i].HeaderText;
        //        excel.Cells[1, i + 1].Font.Bold = true;
        //    }
        //    //填充数据      
        //    for (int i = 0; i < dataGridView.RowCount - 1; i++)
        //    {
        //        for (int j = 0; j < dataGridView.ColumnCount; j++)
        //        {
        //            if (dataGridView[j, i].ValueType == typeof(string))
        //            {
        //                excel.Cells[i + 2, j + 1] = "'" + dataGridView[j, i].Value.ToString();
        //            }
        //            else
        //            {
        //                excel.Cells[i + 2, j + 1] = dataGridView[j, i].Value.ToString();
        //            }
        //        }
        //    }
        //    //Excel.XlFileFormat.xlOpenXMLWorkbook（.xlsx）
        //    //Excel.XlFileFormat.xlExcel8（Excel97 - 2003, .xls）
        //    //判断excel文件的保存格式是xls还是xlsx
        //    var format = fileName.EndsWith(".xls") ? Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8 : Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook;
        //    objWorkbook.SaveAs(fileName, format, Missing.Value, Missing.Value, Missing.Value,
        //        Missing.Value, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared, Missing.Value, Missing.Value, Missing.Value,
        //        Missing.Value, Missing.Value);
        //    return true;
        //}

        #region 导出到Excel 2003：xls文件
        private void DgvToXls(DataGridView dgv)
        {
            if (dgv.Rows.Count == 0)
            {
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execl files (*.xls)|*.xls";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "导出数据到本地计算机";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName == "")
                return;

            HSSFWorkbook wb = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)wb.CreateSheet();
            HSSFRow headRow = (HSSFRow)sheet.CreateRow(0);
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                HSSFCell headCell = (HSSFCell)headRow.CreateCell(i, CellType.String);
                headCell.SetCellValue(dgv.Columns[i].HeaderText);
            }
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                HSSFRow row = (HSSFRow)sheet.CreateRow(i + 1);
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    HSSFCell cell = (HSSFCell)row.CreateCell(j);
                    if (dgv.Rows[i].Cells[j].Value == null)
                    {
                        cell.SetCellType(CellType.Blank);
                    }
                    else
                    {
                        if (dgv.Columns[j].HeaderText == "日期" && dgv.Rows[i].Cells[j].Value != DBNull.Value)
                        {
                            cell.SetCellValue(Convert.ToDateTime(dgv.Rows[i].Cells[j].Value).ToString("yyyy-MM-dd"));
                        }
                        else
                            cell.SetCellValue((dgv.Rows[i].Cells[j].Value).ToString());
                        //if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.Int32"))
                        //{
                        //    cell.SetCellValue(Convert.ToInt32(dgv.Rows[i].Cells[j].Value));
                        //}
                        //else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.String"))
                        //{
                        //    cell.SetCellValue(dgv.Rows[i].Cells[j].Value.ToString());
                        //}
                        //else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.Single"))
                        //{
                        //    cell.SetCellValue(Convert.ToSingle(dgv.Rows[i].Cells[j].Value));
                        //}
                        //else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.Double"))
                        //{
                        //    cell.SetCellValue(Convert.ToDouble(dgv.Rows[i].Cells[j].Value));
                        //}
                        //else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.Decimal"))
                        //{
                        //    cell.SetCellValue(Convert.ToDouble(dgv.Rows[i].Cells[j].Value));
                        //}
                        //else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.DateTime"))
                        //{
                        //    cell.SetCellValue(Convert.ToDateTime(dgv.Rows[i].Cells[j].Value).ToString("yyyy-MM-dd"));  
                        //}
                        //else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.Time"))
                        //{
                        //    cell.SetCellValue(Convert.ToDateTime(dgv.Rows[i].Cells[j].Value).ToString("HH-mm-ss"));
                        //}
                    }

                }

            }
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }
            #region 保存到Excel
            using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
            {
                wb.Write(fs);
            }
            #endregion
            MessageBox.Show("恭喜，导出成功");
        }
        #endregion

        #region 导出到Excel 2007：xlsx文件
        private void DgvToXlsx(string fileName, DataGridView dgv)
        {
            if (dgv.Rows.Count == 0)
            {
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel 2007格式文件（*.xlsx）|*.xlsx";
            sfd.FileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmssms");
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            XSSFWorkbook wb = new XSSFWorkbook();
            XSSFSheet sheet = (XSSFSheet)wb.CreateSheet(fileName);
            XSSFRow headRow = (XSSFRow)sheet.CreateRow(0);
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                XSSFCell headCell = (XSSFCell)headRow.CreateCell(i, CellType.String);
                headCell.SetCellValue(dgv.Columns[i].HeaderText);
            }
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                XSSFRow row = (XSSFRow)sheet.CreateRow(i + 1);
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    XSSFCell cell = (XSSFCell)row.CreateCell(j);
                    if (dgv.Rows[i].Cells[j].Value == null)
                    {
                        cell.SetCellType(CellType.Blank);
                    }
                    else
                    {
                        if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.Int32"))
                        {
                            cell.SetCellValue(Convert.ToInt32(dgv.Rows[i].Cells[j].Value));
                        }
                        else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.String"))
                        {
                            cell.SetCellValue(dgv.Rows[i].Cells[j].Value.ToString());
                        }
                        else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.Single"))
                        {
                            cell.SetCellValue(Convert.ToSingle(dgv.Rows[i].Cells[j].Value));
                        }
                        else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.Double"))
                        {
                            cell.SetCellValue(Convert.ToDouble(dgv.Rows[i].Cells[j].Value));
                        }
                        else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.Decimal"))
                        {
                            cell.SetCellValue(Convert.ToDouble(dgv.Rows[i].Cells[j].Value));
                        }
                        else if (dgv.Rows[i].Cells[j].ValueType.FullName.Contains("System.DateTime"))
                        {
                            cell.SetCellValue(Convert.ToDateTime(dgv.Rows[i].Cells[j].Value).ToString("yyyy-MM-dd"));
                        }
                    }

                }

            }
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }
            #region 保存到Excel
            using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
            {
                wb.Write(fs);
            }
            #endregion
            MessageBox.Show("恭喜，导出成功");
        }
        #endregion








        private void btnSaveData_Click(object sender, EventArgs e)
        {

            //使用npoi导出2003  合格使用
            DgvToXls(dgvselcet);
            //DcExcel(dgvselcet);
        }

       
        //单元格类型格式化事件，可以在这个事件中把指定的列的显示内容改为任何内容，但这种改变不会影响到单元格的真正的值
        private void dgvselcet_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.RowIndex >= 0) && (e.ColumnIndex >= 0))
            {
                if (dgvselcet.Columns[e.ColumnIndex].Name.Equals("物料状态"))
                {
                    if (e.Value == null)
                    {
                        return;
                    }
                    //这里是将列物料状态的显示内容改成
                    if (e.Value.Equals("99")) { e.Value = "空卷"; }
                    else if (e.Value.Equals("05")) { e.Value = "半卷"; }
                    else if (e.Value.Equals("10")) { e.Value = "满卷"; }
                }

            }
        }

        ////在绑定dt前调用此方法更改数据源表中某列的值
        private System.Data.DataTable CellChange(System.Data.DataTable dt)
        {
            if ((dt.Rows.Count >= 0) && (dt.Columns.Count >= 0))
            {
                if (dt.Columns.Contains("物料状态"))
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["物料状态"].Equals("99"))
                            dt.Rows[i]["物料状态"] = "空卷";
                        else if (dt.Rows[i]["物料状态"].Equals("05"))
                            dt.Rows[i]["物料状态"] = "半卷";
                       else if(dt.Rows[i]["物料状态"].Equals("10"))
                            dt.Rows[i]["物料状态"] = "满卷";
                    }                   
                }
            }
            return dt;
        }

        private void btnPrintf_Click(object sender, EventArgs e)
        {

            frmPrintf frmprintf = GenericSingleton<frmPrintf>.CreateInstrance();
            showdatatable += frmprintf.GetDT;//【3】将委托变量与方法绑定 
            showdatatable((System.Data.DataTable)dgvselcet.DataSource);//【4】调用该方法 
            this.Close();
            frmprintf.Show();
        }


    }
}
