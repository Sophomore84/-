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
    public partial class frmRightChange : FormBase
    {
        public frmRightChange()
        {
            InitializeComponent();
            FormExIni();
            _systemButtonManager = new SystemButtonManager(this);
        }
        //public string Libareaname
        //{
        //    get {return Libareaname; }
        //    set { Libareaname = value; }
        //}
        private void frmRightChange_Load(object sender, EventArgs e)
        {

            DataTable dt= orderHelper.updataLibMess(UCTLibState.LibAreaName);

            //根据Header和所有单元格的内容自动调整列的宽度 
            
            //根据Header和所有单元格的内容自动调整行的高度 
            dgvLibMess.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvLibMess.DataSource = dt;//将查询结果委托显示在主窗体
            //DataGridView1的第1列只读
            dgvLibMess.Columns[0].ReadOnly = true;
            //指定某列的宽度自适应
            dgvLibMess.Columns["生产批号"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

        }
        //点击修改按钮时，获取datagrid中的数据，
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            OutInLibRecord outinrecord = new OutInLibRecord();
            outinrecord.Position = dgvLibMess.Rows[0].Cells["库存位置"].Value.ToString();
            outinrecord.EmptyNum = dgvLibMess.Rows[0].Cells["卷筒编号"].Value.ToString()==""?null: dgvLibMess.Rows[0].Cells["卷筒编号"].Value.ToString();
            outinrecord.Goodscode = dgvLibMess.Rows[0].Cells["物料规格"].Value.ToString() == "" ? null : dgvLibMess.Rows[0].Cells["物料规格"].Value.ToString();
            outinrecord.GoodsState = dgvLibMess.Rows[0].Cells["物料状态"].Value.ToString() == "" ? null : dgvLibMess.Rows[0].Cells["物料状态"].Value.ToString();
            outinrecord.CheckState = dgvLibMess.Rows[0].Cells["检测状态"].Value.ToString() == "" ? null : dgvLibMess.Rows[0].Cells["检测状态"].Value.ToString();
            outinrecord.GoodsNum = dgvLibMess.Rows[0].Cells["生产批号"].Value.ToString() == "" ? null : dgvLibMess.Rows[0].Cells["生产批号"].Value.ToString();
            outinrecord.Date = dgvLibMess.Rows[0].Cells["日期"].Value.ToString() == "" ? null : dgvLibMess.Rows[0].Cells["日期"].Value.ToString();
            outinrecord.Time = dgvLibMess.Rows[0].Cells["时间"].Value.ToString() == "" ? null : dgvLibMess.Rows[0].Cells["时间"].Value.ToString();
            outinrecord.Notes = dgvLibMess.Rows[0].Cells["备注"].Value.ToString() == "" ? null : dgvLibMess.Rows[0].Cells["备注"].Value.ToString();
            int r=orderHelper.UpdateLibInRecord(outinrecord.Position, outinrecord.EmptyNum, outinrecord.Goodscode, outinrecord.GoodsState,
              outinrecord.CheckState, outinrecord.GoodsNum, outinrecord.Date, outinrecord.Time, outinrecord.Notes);
            if (r > 0) MessageBox.Show("更新成功");
        }
    }
}
