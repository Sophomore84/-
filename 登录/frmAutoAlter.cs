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
    public partial class frmAutoAlter : FormBase
    {

        private DataTable dt = null;

        public static int Control_number = 0;
        public frmAutoAlter()
        {
            InitializeComponent();
            FormExIni();
            _systemButtonManager = new SystemButtonManager(this);
        }

        private void frmAutoAlter_Load(object sender, EventArgs e)
        {
            //查询出当前库位的详细信息
            dt = orderHelper.updataLibMess(UCTLibState.LibAreaName);
            if (dt != null)
            {
                textBox1.Text= dt.Rows[0]["卷筒编号"].Equals(DBNull.Value) ? null : dt.Rows[0]["卷筒编号"].ToString();
                textBox2.Text= dt.Rows[0]["卷筒编号"].Equals(DBNull.Value) ? null : dt.Rows[0]["卷筒编号"].ToString();
                textBox3.Text = dt.Rows[0]["库存位置"].ToString();

            }
            if (textBox1.Text == null || textBox1.Text == "") textBox1.Text = "请输入卷筒编号";
            if (textBox2.Text == null || textBox2.Text == "") textBox2.Text = "请输入卷筒编号";
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //一键修改为空位
            OutInLibRecord outinrecord = new OutInLibRecord();
            outinrecord.Position = dt.Rows[0]["库存位置"].ToString();
            outinrecord.EmptyNum = null;
            outinrecord.Goodscode = null;
            outinrecord.GoodsState = null ;
            outinrecord.CheckState = null ;
            outinrecord.GoodsNum = null ;
            outinrecord.Date = null;
            outinrecord.Time = null ;
            outinrecord.Notes = null ;
            int r = orderHelper.UpdateLibInRecord(outinrecord.Position, outinrecord.EmptyNum, outinrecord.Goodscode, outinrecord.GoodsState,
              outinrecord.CheckState, outinrecord.GoodsNum, outinrecord.Date, outinrecord.Time, outinrecord.Notes);
            if (r > 0)
            {
                MessageBox.Show("更新成功");
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //一键修改为空卷
            OutInLibRecord outinrecord = new OutInLibRecord();
            outinrecord.Date = DateTime.Now.ToString("yyyy-MM-dd");        // 2008-09-04
            outinrecord.Time = DateTime.Now.ToString("HH:mm:ss");
            outinrecord.Position = dt.Rows[0]["库存位置"].ToString();
            outinrecord.EmptyNum = textBox1.Text.ToString()==null?null: textBox1.Text.ToString();
            outinrecord.Goodscode = "卷筒";
            outinrecord.GoodsState = "空卷";
            outinrecord.CheckState = null ;
            outinrecord.GoodsNum =  null ;
            outinrecord.Date = outinrecord.Date;
            outinrecord.Time = outinrecord.Time;
            outinrecord.Notes = "退库";
            int r = orderHelper.UpdateLibInRecord(outinrecord.Position, outinrecord.EmptyNum, outinrecord.Goodscode, outinrecord.GoodsState,
              outinrecord.CheckState, outinrecord.GoodsNum, outinrecord.Date, outinrecord.Time, outinrecord.Notes);
            if (r > 0)
            {
                MessageBox.Show("更新成功");
                this.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Control_number++;
            //一键修改为满卷
            OutInLibRecord outinrecord = new OutInLibRecord();
            outinrecord.Date = DateTime.Now.ToString("yyyy-MM-dd");        // 2008-09-04
            outinrecord.Time = DateTime.Now.ToString("HH:mm:ss");

            outinrecord.Position = dt.Rows[0]["库存位置"].ToString();
            outinrecord.EmptyNum = textBox2.Text.ToString() == null ? null : textBox2.Text.ToString();
            outinrecord.Goodscode = "01";
            outinrecord.GoodsState = "满卷";
            outinrecord.CheckState = "合格";
            outinrecord.GoodsNum = outinrecord.Date.Replace("-", "").Substring(2, 6)+"01"+ Control_number.ToString().PadLeft(2, '0');
            outinrecord.Date = outinrecord.Date;
            outinrecord.Time = outinrecord.Time;
            outinrecord.Notes = "入库";
            int r = orderHelper.UpdateLibInRecord(outinrecord.Position, outinrecord.EmptyNum, outinrecord.Goodscode, outinrecord.GoodsState,
              outinrecord.CheckState, outinrecord.GoodsNum, outinrecord.Date, outinrecord.Time, outinrecord.Notes);
            if (r > 0)
            {
                MessageBox.Show("更新成功");
                this.Close();
            }
        }

        private void frmAutoAlter_Activated(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
