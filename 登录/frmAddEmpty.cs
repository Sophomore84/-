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
    public partial class frmAddEmpty : FormBase
    {

        public frmAddEmpty()
        {
            InitializeComponent();
            FormExIni();
            _systemButtonManager = new SystemButtonManager(this);
        }

        private void frmAddEmpty_Load(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
           //加载前先清空
            cmbexistEmpty.Items.Clear();
            cmbEmptyInPosition.Items.Clear();
            cmbEmptyPosition.Items.Clear();
            cmbNewEmpty.Clear();
            string sql = " select 库存位置,卷筒编号 from 库存记录表 where 卷筒编号 is not null";
            System.Data.DataTable dt = SqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0) //查到结果
            {
                //遍历每行0列的元素填充到combox
                cmbexistEmpty.DropDownStyle = ComboBoxStyle.DropDown;
                cmbEmptyInPosition.DropDownStyle = ComboBoxStyle.DropDown;

                foreach (DataRow dr in dt.Rows)
                {
                    cmbexistEmpty.Items.Add(dr["卷筒编号"].ToString());
                    cmbEmptyInPosition.Items.Add(dr["库存位置"].ToString());

                }
                cmbexistEmpty.SelectedIndex = 0;
                cmbEmptyInPosition.SelectedIndex = 0;
            }



            string sql1 = " select 库存位置 from 库存记录表 where 卷筒编号 is null and 物料状态 is null";
            System.Data.DataTable dt1 = SqlHelper.ExecuteDataTable(sql1);
            if (dt1.Rows.Count > 0) //查到结果
            {
                cmbEmptyPosition.DropDownStyle = ComboBoxStyle.DropDown;
                foreach (DataRow dr in dt1.Rows)
                {
                    cmbEmptyPosition.Items.Add(dr["库存位置"].ToString());
                }
                cmbEmptyPosition.SelectedIndex = 0;
            }
        }

        //卷筒编号选择改变时，对应的库存位置也做变动
        private void cmbexistEmpty_SelectedIndexChanged(object sender, EventArgs e)
        {
            //库存位置获取选中项的索引值
            cmbEmptyInPosition.SelectedIndex = cmbexistEmpty.SelectedIndex;
        }
        //卷筒编号对应的库存位置选择改变时，对应的卷筒编号也做变动
        private void cmbEmptyInPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbexistEmpty.SelectedIndex = cmbEmptyInPosition.SelectedIndex;
        }


        //删除库存中卷筒
        private void btnDeleteEmpty_Click(object sender, EventArgs e)
        {
            int n=orderHelper.DeleteEmptyCodeRecord(cmbexistEmpty.Text.ToString());
            if (n > 0)
            {
                MessageBox.Show("删除成功！");
                UpdateUI();
            }
            else
                MessageBox.Show("删除失败，请检查数据库连接是否正常！");
           

        }
        //添加库中卷筒
        private void btnAddEmpty_Click(object sender, EventArgs e)
        {
            if (!cmbexistEmpty.Items.Contains(cmbNewEmpty.Text))
            {
                int n = orderHelper.InsertEmptyCodeRecord(cmbNewEmpty.Text, cmbEmptyPosition.Text);
                if (n > 0)
                {
                    MessageBox.Show("执行成功");
                    UpdateUI();
                }
                else
                {
                    MessageBox.Show("执行失败");
                }
                
            }
            else
            {
                MessageBox.Show("数据库中已有此物料规格，请重新输入！");
            }
        }
    }
}
