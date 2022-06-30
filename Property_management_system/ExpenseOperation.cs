using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Property_management_system
{
    public partial class ExpenseOperation : Form
    {
        static double shuiprice, dianprice, wangprice, wuyeprice;
        public ExpenseOperation()
        {
            InitializeComponent();
        }
        private void Billsinf_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            //获取水电费用单价和物业费、宽带费
            try
            {
                string sqltext;
                sqltext = "select * from othersetting where bianhao = '1'";
                DataSet ds = new DataSet();
                ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext, null);
                shuiprice = double.Parse(ds.Tables[0].Rows[0][1].ToString());
                dianprice = double.Parse(ds.Tables[0].Rows[0][2].ToString());
                wangprice = double.Parse(ds.Tables[0].Rows[0][3].ToString());
                wuyeprice = double.Parse(ds.Tables[0].Rows[0][4].ToString());
            }
            catch (Exception)
            {

            }
            //初始化datagridview
            try
            {
                string sqltext1 = "select * from billsinf ";
                billsdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext1, null).Tables[0].DefaultView;
                billsdataGridView.Columns["danhao"].HeaderText = "单号";
                billsdataGridView.Columns["roomId"].HeaderText = "房间编号";
                billsdataGridView.Columns["username"].HeaderText = "住户姓名";
                billsdataGridView.Columns["predianbiao"].HeaderText = "上次电表数";
                billsdataGridView.Columns["dianbiao"].HeaderText = "本次电表数";
                billsdataGridView.Columns["dianbiaojine"].HeaderText = "应缴电费"; // 差值 * 电费单价
                billsdataGridView.Columns["preshuibiao"].HeaderText = "上次水表数";
                billsdataGridView.Columns["shuibiao"].HeaderText = "本次水表数";
                billsdataGridView.Columns["shuibiaojine"].HeaderText = "应缴水费"; // 差值* 水费单价
                billsdataGridView.Columns["jine"].HeaderText = "应缴费金额"; // 水费 + 电费 + 物业费 + 宽带费
            }
            catch (Exception)
            {

            }
            //获取添加选项卡中的combobox数据并将索引置0
            try
            {
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
                string sqltext1 = "select roomId from userinf; ";
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext1, null);
                dt = ds.Tables[0];
                List<string> roomList = new List<string>(); ;
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        roomList.Add(dt.Rows[i][0].ToString());
                    }
                }
                roomList.Sort();
                foreach(string roomId in roomList)
                {
                    comboBox2.Items.Add(roomId);
                    comboBox3.Items.Add(roomId);
                }
                comboBox2.SelectedIndex = 0;
                comboBox3.SelectedIndex = 0;
            }
            catch (Exception)
            {

            }
            //修改选项卡中加载数据的加载按钮点击后单号combobox初始化
            try
            {
                comboBox5.Items.Clear();
                string sqltext1 = "select * from billsinf";
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext1, null);
                dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        comboBox5.Items.Add(dt.Rows[i][0].ToString());
                    }
                }
                comboBox5.SelectedIndex = 0;
            }
            catch (Exception)
            {

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string roomId, username, predianbiao, dianbiao, dianbiaojine, preshuibiao, shuibiao, shuibiaojine, jine, sqltext, sqltext1;
                roomId = comboBox2.SelectedItem.ToString();
                // 根据房间号确定姓名，
                sqltext1 = "select * from userinf where roomId = '" + roomId + "';";
                if (!MySqlHelper.IsExist(sqltext1))
                {
                    MessageBox.Show("此房间号不存在！");
                } else {
                    sqltext1 = "select username from userinf where roomId = '" + roomId + "';";
                    DataSet ds = new DataSet();
                    DataTable dt = new DataTable();
                    ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext1, null);
                    username = ds.Tables[0].Rows[0][0].ToString();
                    predianbiao = textBox3.Text;
                    dianbiao = textBox4.Text;
                    preshuibiao = textBox5.Text;
                    shuibiao = textBox6.Text;
                    dianbiaojine = ((double.Parse(dianbiao) - double.Parse(predianbiao)) * dianprice).ToString();
                    shuibiaojine = ((double.Parse(shuibiao) - double.Parse(preshuibiao)) * shuiprice).ToString();
                    jine = (double.Parse(dianbiaojine) + double.Parse(shuibiaojine) + wangprice + wuyeprice).ToString();
                    sqltext = "insert into billsinf(roomId, username, predianbiao, dianbiao, preshuibiao, shuibiao, dianbiaojine, shuibiaojine, jine) values" +
                        "('" + roomId + "','" + username + "','" + predianbiao + "','" + dianbiao + "','" + preshuibiao + "','" + shuibiao + "','" + dianbiaojine +
                        "','" + shuibiaojine + "','" + jine + "')";
                    MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext, null);
                    billsdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, "select * from billsinf ", null).Tables[0].DefaultView;
                    Billsinf_Load(sender, e);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("此住户缴费编号已经存在！");
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string roomId, sqltext;
                roomId = comboBox3.SelectedItem.ToString();
                sqltext = "select * from billsinf where roomId = '" + roomId + "'";
                DataSet ds = new DataSet();
                ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext, null);
                textBox10.Text = ds.Tables[0].Rows[0][3].ToString();
                textBox9.Text = ds.Tables[0].Rows[0][4].ToString();
                textBox8.Text = ds.Tables[0].Rows[0][5].ToString();
                textBox7.Text = ds.Tables[0].Rows[0][6].ToString();
            }
            catch (Exception)
            {

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (billsdataGridView.Rows.Count == 0)
            {
                MessageBox.Show("没有数据可导出!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.FileName = null;
            saveFileDialog.Title = "保存";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream stream = saveFileDialog.OpenFile();
                StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.GetEncoding(-0));
                string strLine = "";
                try
                {
                    //表头
                    for (int i = 0; i < billsdataGridView.ColumnCount; i++)
                    {
                        if (i > 0)
                            strLine += ",";
                        strLine += billsdataGridView.Columns[i].HeaderText;
                    }
                    strLine.Remove(strLine.Length - 1);
                    sw.WriteLine(strLine);
                    strLine = "";
                    //表的内容
                    for (int j = 0; j < billsdataGridView.Rows.Count; j++)
                    {
                        strLine = "";
                        int colCount = billsdataGridView.Columns.Count;
                        for (int k = 0; k < colCount; k++)
                        {
                            if (k > 0 && k < colCount)
                                strLine += ",";
                            if (billsdataGridView.Rows[j].Cells[k].Value == null)
                                strLine += "";
                            else
                            {
                                string cell = billsdataGridView.Rows[j].Cells[k].Value.ToString().Trim();
                                //防止里面含有特殊符号
                                cell = cell.Replace("\"", "\"\"");
                                cell = "\"" + cell + "\"";
                                strLine += cell;
                            }
                        }
                        sw.WriteLine(strLine);
                    }
                    sw.Close();
                    stream.Close();
                    MessageBox.Show("数据被导出到：" + saveFileDialog.FileName.ToString(), "导出完毕", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "导出错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string roomId, predianbiao, dianbiao, dianbiaojine, preshuibiao, shuibiao, shuibiaojine, jine, sqltext;
                roomId = comboBox3.Items.ToString();
                predianbiao = textBox10.Text;
                dianbiao = textBox9.Text;
                preshuibiao = textBox8.Text;
                shuibiao = textBox7.Text;
                sqltext = "select * from billsinf where roomId = '" + roomId + "';";
                if (!MySqlHelper.IsExist(sqltext))
                {
                    MessageBox.Show("此房间不存在水电费信息，请核实是否所属！");
                }
                else
                {
                    dianbiaojine = ((double.Parse(dianbiao) - double.Parse(predianbiao)) * dianprice).ToString();
                    shuibiaojine = ((double.Parse(shuibiao) - double.Parse(preshuibiao)) * shuiprice).ToString();
                    jine = (double.Parse(dianbiaojine) + double.Parse(shuibiaojine) + wangprice + wuyeprice).ToString();
                    sqltext = "update billsinf set predianbiao='" + predianbiao + "',dianbiao='" + dianbiao + "',preshuibiao='" + preshuibiao + 
                        "',shuibiao='" + shuibiao + "',dianbiaojine='" + dianbiaojine + "',shuibiajineo='" + shuibiaojine + "',jine='" + jine + 
                        "' where roomId = '" + roomId + "'";
                    MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext, null);
                    billsdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, "select * from billsinf ", null).Tables[0].DefaultView;
                }
            }
            catch (Exception)
            {

            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                // 询问是否删除
                if (MessageBox.Show("您确定要删除吗？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string danhao, sqltext;
                    danhao = comboBox5.SelectedItem.ToString();
                    sqltext = "delete from billsinf where danhao='" + danhao + "'";
                    MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext, null);
                    billsdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, "select * from billsinf ", null).Tables[0].DefaultView;
                    Billsinf_Load(sender, e);
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
                string chaxunfangshi = null, sqltext1;
                if (comboBox1.SelectedItem.ToString().Equals("住户姓名"))
                    chaxunfangshi = "username";
                else if (comboBox1.SelectedItem.ToString().Equals("房间编号"))
                    chaxunfangshi = "roomId";
                sqltext1 = "select * from billsinf where " + chaxunfangshi + " like '%" + textBox1.Text + "%'";
                billsdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext1, null).Tables[0].DefaultView;
            }
            catch (Exception)
            {

            }

        }
    }
}
