using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace  风帆电池
{
    public partial class frmCurrentLib : FormBase
    {
        //声明自适应类实例
        AutoSizeFormClass asclib = new AutoSizeFormClass();


        public delegate void ShowDataTable(System.Data.DataTable dt);//【1】定义一个委托类型 
        public ShowDataTable showdatatable;//【2】申明一个委托对象 
        public frmCurrentLib()
        {
            InitializeComponent();
            FormExIni();
            _systemButtonManager = new SystemButtonManager(this);
        }

        private void frmCurrentLib_Load(object sender, EventArgs e)
        {
            //窗体加载时，查询当前库存记录
            string sql = @"select T1.序号,T1.库存位置,T1.卷筒编号,T1.物料规格,T2.物料型号,
                         T1.物料状态,T1.检测状态,T1.生产批号,T1.日期,T1.时间,T1.备注
                        from 物料表 as T2
                         right join 库存记录表 as T1
                       on T1.物料规格 = T2.物料规格";
             DataTable dt = SqlHelper.ExecuteDataTable(sql);
            //根据Header和所有单元格的内容自动调整行的高度 
            dgvselcet.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvselcet.DataSource = dt;
            //将序号改为自动编号
            for (int i = 0; i < this.dgvselcet.Rows.Count - 1; i++)
                this.dgvselcet.Rows[i].Cells[0].Value = i + 1;

        }

        private void frmCurrentLib_SizeChanged(object sender, EventArgs e)
        {
            asclib.controlAutoSize(this);
        }

        //点击打印
        private void btnPrintfLib_Click(object sender, EventArgs e)
        {
           
            frmPrintf frmprintf = GenericSingleton<frmPrintf>.CreateInstrance();
            showdatatable += frmprintf.GetDT;//【3】将委托变量与方法绑定 
            showdatatable((System.Data.DataTable)dgvselcet.DataSource);//【4】调用该方法 
            this.Close();                                                           
            frmprintf.Show();
        }
        //将数据导出到Excel
        private void btnEXcelData_Click(object sender, EventArgs e)
        {
            //使用npoi导出2003  合格使用
            DgvToXls(dgvselcet);
        }

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
                        if (dgv.Columns[j].HeaderText == "日期" && dgv.Rows[i].Cells[j].Value!= DBNull.Value)
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
    }
}
