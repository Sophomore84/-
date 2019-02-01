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
    public partial class frmLibExchange : FormBase
    {
        public frmLibExchange()
        {
            InitializeComponent();
            FormExIni();
            _systemButtonManager = new SystemButtonManager(this);
        }

        private void frmLibExchange_Load(object sender, EventArgs e)
        {
            textsrPosition.Text = UCTLibState.LibAreaName;
            string sql1 = " select 库存位置 from 库存记录表 ";
            System.Data.DataTable dt1 = SqlHelper.ExecuteDataTable(sql1);
            cmbdesPosition.Items.Clear();
            cmbdesPosition.Items.Add("请选择目标库位");
            if (dt1.Rows.Count > 0) //查到结果
            {
                cmbdesPosition.DropDownStyle = ComboBoxStyle.DropDown;
                foreach (DataRow dr in dt1.Rows)
                {
                    cmbdesPosition.Items.Add(dr["库存位置"].ToString());
                }
                cmbdesPosition.SelectedIndex = 0;
            }
         }

  

        //点击确认倒库
        private void button1_Click(object sender, EventArgs e)
        {
           

            if (cmbdesPosition.SelectedIndex != 0 && cmbdesPosition.Text != textsrPosition.Text)
            {
                //没有倒库任务时
                if (!frmNewMain.startExchageLib)
                {
                   
                    //获取起始库位
                    frmNewMain.startPosition = textsrPosition.Text;
                    //其实库位状态
                    orderHelper.SelectToolTip(frmNewMain.startPosition, out frmNewMain.startPositionstate);
                    //获取目标库位
                    frmNewMain.desPosition = cmbdesPosition.Text;
                    //目标库位状态
                    orderHelper.SelectToolTip(frmNewMain.desPosition, out frmNewMain.desPositionstate);
                    //倒库完成时，以上都清空一次
                    //关闭窗体
                    this.Close();
                    frmNewMain.startExchageLib = true;
                }
                else
                {
                    MessageBox.Show("正在进行其他倒库任务，请等待！");
                }
                
            }
            else
            {
                MessageBox.Show("目标库位选择有误！");
            }
        }
    }
}
