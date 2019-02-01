using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace  风帆电池
{
    public partial class frmPrintf : FormBase
    {
        DGVPrinter dgvPrinter;
        
        public  frmPrintf()
        {
            InitializeComponent();
            FormExIni();
            _systemButtonManager = new SystemButtonManager(this);
            
            
            //克隆选中的行
            //for (int i = 0; i < rowColl.Count; i++)
            //{
            //    DataRow dataRow = (rowColl[i].DataBoundItem as DataRowView).Row;
            //    gridSelectDT.ImportRow(dataRow);
            //}
            //this.dgvPrintf.DataSource = gridSelectDT;

           
            dgvPrinter = new DGVPrinter();
            dgvPrinter.SourceDGV = dgvPrintf;
        }

        //得到窗体1的DataTable的方法
        public void GetDT(DataTable totaldt)
        {
            DataTable TotalDT =totaldt;
            if (TotalDT != null)
            {  //克隆一个表结构
                DataTable gridSelectDT = TotalDT.Clone();
                this.dgvPrintf.DataSource = TotalDT;
            }
        }

        private void btn_print_Click(object sender, EventArgs e)
        {
            //dgvPrinter.mainTitle = "主标题";
            //dgvPrinter.subTitle = "副标题";
            dgvPrinter.PrintDataGridView(dgvPrintf);
        }

        private void btn_Setting_Click(object sender, EventArgs e)
        {
            dgvPrinter.SetupPage();
        }

        private void btn_preview_Click(object sender, EventArgs e)
        {
            dgvPrinter.PrintPreview();
        }

    }

}
