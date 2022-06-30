using System;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;
namespace Property_management_system
{
    public partial class Othersetting : Form
    {
        static string usernamestr;
        public Othersetting()
        {
            InitializeComponent();
        }
        public Othersetting(string usernamestr)
        {
            Othersetting.usernamestr = usernamestr;
            InitializeComponent();
        }
        public bool IsNum(String strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");
            return !objNotNumberPattern.IsMatch(strNumber) && !objTwoDotPattern.IsMatch(strNumber) && !objTwoMinusPattern.IsMatch(strNumber) && objNumberPattern.IsMatch(strNumber);
        }
        private void Othersetting_Load(object sender, EventArgs e)
        {
            try
            {
                string shuiprice, dianprice, wangprice, wuyeprice, sqltext;
                sqltext = "select * from othersetting where bianhao='1'";
                DataSet ds = new DataSet();
                ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext, null);
                shuiprice = ds.Tables[0].Rows[0][1].ToString();
                dianprice = ds.Tables[0].Rows[0][2].ToString();
                wangprice = ds.Tables[0].Rows[0][3].ToString();
                wuyeprice = ds.Tables[0].Rows[0][4].ToString();
                textBox1.Text = shuiprice;
                textBox2.Text = dianprice;
                textBox3.Text = wangprice;
                textBox4.Text = wuyeprice;
                if (usernamestr == "root")
                {
                    // 超级管理员允许修改收费项目
                    button1.Visible = true;
                    textBox1.ReadOnly = false;
                    textBox2.ReadOnly = false;
                    textBox3.ReadOnly = false;
                    textBox4.ReadOnly = false;
                }
                else
                {
                    button1.Visible = false;
                    textBox1.ReadOnly = true;
                    textBox2.ReadOnly = true;
                    textBox3.ReadOnly = true;
                    textBox4.ReadOnly = true;
                }
            }
            catch (Exception)
            {

            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int flag;
                string shuiprice, dianprice, wangprice, wuyeprice, yue, sqltext;
                shuiprice = textBox1.Text;
                dianprice = textBox2.Text;
                wangprice = textBox3.Text;
                wuyeprice = textBox4.Text;
                sqltext="update othersetting set shuiprice='"+shuiprice+"',dianprice='"+dianprice+"',wangprice='"+wangprice+"',wuyeprice='"+wuyeprice+"' where bianhao='1'";
                flag=MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext, null);
                if (flag == 1)
                    MessageBox.Show("修改成功！");
                else
                    MessageBox.Show("修改失败！");
            }
            catch (Exception)
            {

            }
        }
    }
}
