using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace Property_management_system
{
    public partial class Userinf : Form
    {
        public Userinf()
        {
            InitializeComponent();
            comboBox3.SelectedIndex = 0;
            comboBoxPayState.SelectedIndex = 0;
            comboBoxUpdateSex.SelectedIndex = 0;
            comboBoxUpdatePayState.SelectedIndex = 0;
        }
        public static bool IsNum(string value)
        {
            return Regex.IsMatch(value, @"^\d+$");
        }
        private void Userinf_Load(object sender, EventArgs e)
        {
            try
            {
                comboBox1.SelectedIndex = 0;
                comboBoxSex.SelectedIndex = 0;
                string sqltext1 = "select * from userinf; ";
                
                userdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext1, null).Tables[0].DefaultView;
                userdataGridView.AutoGenerateColumns = false;
                userdataGridView.Columns["roomId"].HeaderText = "门牌号";
                userdataGridView.Columns["level"].HeaderText = "楼层";
                userdataGridView.Columns["seat"].HeaderText = "座次";
                userdataGridView.Columns["username"].HeaderText = "住户姓名";
                userdataGridView.Columns["idCard"].HeaderText = "身份证号";
                userdataGridView.Columns["sex"].HeaderText = "性别";
                userdataGridView.Columns["phone"].HeaderText = "电话";
                userdataGridView.Columns["uid"].HeaderText = "UUID";

            }
            catch (Exception)
            {
                MessageBox.Show("数据初始化错误！");
            }
            
        }
        /**
         * 查询按钮点击方法
         */
        private void button1_Click(object sender, EventArgs e)
        {
            string chaxunfangshi = "num",sqltext1;
            if (comboBox1.SelectedItem.ToString().Equals("住户姓名"))
                chaxunfangshi = "username";
            else if (comboBox1.SelectedItem.ToString().Equals("住户电话"))
                chaxunfangshi = "phone";
            else if (comboBox1.SelectedItem.ToString().Equals("房间号"))
                chaxunfangshi = "roomId";
            try
            {
                sqltext1 = "select * from userinf where " + chaxunfangshi + " like '%" + textBox1.Text + "%'";
                userdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext1, null).Tables[0].DefaultView;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }

        }
        /**
         * 检查房间号是否已经在数据库中存在
         */
        private bool checkRoomId(string roomId)
        {
            string sqltext = "select * from userinf where roomId = '" + roomId + "';";
            return !MySqlHelper.IsExist(sqltext);
        }
        /**
         * 检查身份证号是否符合规范
         */
        private bool checkIdCard(string Id)
        {
            if (Id.Length != 18)
            {
                return false;
            }
            long n;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }
        /**
         * 使用正则表达式检查电话格式是否符合规范
         */
        public static bool checkPhone(string phone)
        {
            Regex regex = new Regex("^1[34578]\\d{9}$");
            return regex.IsMatch(phone);
        }
        /**
         * 添加用户方法
         */
        private void buttonadd_Click(object sender, EventArgs e)
        {
            try
            {
                string roomId, level, seat, idCard, username, sex, phone, uid ,pay_state; // 其中level和seat由roomId得到
                string sqltext;
                roomId = textBoxRoomId.Text;
                idCard = textBoxIdCard.Text;
                username = textBoxname.Text;
                sex = comboBoxSex.SelectedItem.ToString();
                phone = textBoxPhone.Text;
                uid = textBoxWechat.Text;

                if ("已缴费".Equals(comboBoxPayState.SelectedItem.ToString()))
                {
                    pay_state = "1";
                }
                else
                {
                    pay_state = "0";
                }
                if ((roomId.Length == 0) || (idCard.Length == 0) || (uid.Length == 0) || (phone.Length == 0) || (sex.Length == 0))
                {
                    MessageBox.Show("信息未填写完整！");
                } else if (!checkIdCard(idCard))
                {
                    MessageBox.Show("身份证号格式不正确！");
                } else if (!checkPhone(phone))
                {
                    MessageBox.Show("电话格式不正确！");
                }
                else if (!checkRoomId(roomId))
                {
                    MessageBox.Show("门牌号已经存在！");
                } 
                else
                {
                    level = roomId.Substring(0, 1);
                    seat = roomId.Substring(1, 1);
                    // MessageBox.Show(roomId + " " + level + " " + seat + " " + username + " " + sex + " " + idCard + " " + phone + " " + uid + pay_state);
                    sqltext = "insert into userinf(roomId, level, seat, username, sex, idCard, phone, uid, pay_state) values" +
                        "('" + roomId + "','" + level + "','" + seat + "','" + username + "','"+ sex + "','" + idCard + "','" + 
                        phone + "','" + uid + "','" + pay_state  + "')";
                    MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext, null);
                    userdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, "select * from userinf ", null).Tables[0].DefaultView;
                    MessageBox.Show("成功添加住户信息！");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
                MessageBox.Show("该住户已存在！");
            }
        }
        /**
         * 删除住户信息
         */
        private void button4_Click(object sender, EventArgs e)
        { 
            try
            {
                if (MessageBox.Show("您确定要删除吗？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string sqltext, idCard;
                    idCard = textBox18.Text;
                    sqltext = "delete from userinf where idCard = '" + idCard + "'";
                    MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext, null);
                    // 刷新数据
                    userdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, "select * from userinf ", null).Tables[0].DefaultView;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("要删除的用户不存在！");
            }
            
        }
        private void buttonupdate_Click(object sender, EventArgs e)
        {
            try
            {
                string roomId, level, seat, idCard, username, sex, phone, uid, pay_state; // 其中level和seat由roomId得到
                string sqltext;
                roomId = textBoxUpdateRoomId.Text;
                idCard = textBoxUpdateIdCard.Text;
                username = textBoxUpdateName.Text;
                sex = comboBoxUpdateSex.SelectedItem.ToString();
                phone = textBoxUpdatePhone.Text;
                uid = textBoxUpdateUid.Text;
                if("已缴费".Equals(comboBoxUpdatePayState.SelectedItem.ToString()))
                {
                    pay_state = "1";
                }else pay_state = "0";
                if ((roomId.Length == 0) || (idCard.Length == 0) || (uid.Length == 0) || (phone.Length == 0) || (sex.Length == 0))
                {
                    MessageBox.Show("信息未填写完整！");
                }
                else if (!checkIdCard(idCard))
                {
                    MessageBox.Show("身份证号格式不正确！");
                }
                else if (!checkPhone(phone))
                {
                    MessageBox.Show("电话格式不正确！");
                }
                else
                {
                    level = roomId.Substring(0, 1);
                    seat = roomId.Substring(1, 1);
                    sqltext = "update userinf set idCard = '" + idCard + "',level = '" + level + "',seat = '" + seat + "',username ='"
                        + username + "',sex='" + sex + "',phone='" + phone + "', uid='" + uid + "' ,pay_state = '" + pay_state +
                        "' where roomId ='" + roomId + "';";
                    MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext, null);
                    userdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, "select * from userinf ", null).Tables[0].DefaultView;
                    MessageBox.Show("修改住户信息成功！");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("房间号输入错误");
            }
            
        }
        /**
         * 用于加载需要修改的数据
         */

        private void buttonjiazai_Click(object sender, EventArgs e)
        {
            try
            {
                string roomId = textBoxUpdateRoomId.Text;
                string sqltext;
                sqltext = "select * from userinf where roomId ='" + roomId + "'";
                DataSet ds = new DataSet();
                ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext, null);
                textBoxUpdateRoomId.Text = ds.Tables[0].Rows[0][0].ToString();
                textBoxUpdateName.Text = ds.Tables[0].Rows[0][3].ToString();
                comboBoxUpdateSex.Text = ds.Tables[0].Rows[0][4].ToString();
                textBoxUpdateIdCard.Text = ds.Tables[0].Rows[0][5].ToString();
                textBoxUpdatePhone.Text = ds.Tables[0].Rows[0][6].ToString();
                textBoxUpdateUid.Text = ds.Tables[0].Rows[0][7].ToString();
                if ("True".Equals(ds.Tables[0].Rows[0][8].ToString()))
                {
                    comboBoxUpdatePayState.Text = "已缴费";
                }
                else
                {
                    comboBoxUpdatePayState.Text = "未缴费";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("身份证号输入错误！");
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string chaxunfangshi = "num", sqltext1;
            if (comboBox3.SelectedItem.ToString().Equals("身份证号"))
                chaxunfangshi = "idCard";
            else if (comboBox3.SelectedItem.ToString().Equals("姓名"))
                chaxunfangshi = "username";
            else if (comboBox3.SelectedItem.ToString().Equals("电话"))
                chaxunfangshi = "phone";
            else if (comboBox3.SelectedItem.ToString().Equals("房间号"))
                chaxunfangshi = "roomId";
            try
            {
                sqltext1 = "select * from userinf where " + chaxunfangshi + " like '%" + textBox6.Text + "%'";
                userdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext1, null).Tables[0].DefaultView;
            }
            catch (Exception)
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string chaxunfangshi = "num", sqltext1;
            if (comboBox3.SelectedItem.ToString().Equals("身份证号"))
                chaxunfangshi = "idCard";
            else if (comboBox3.SelectedItem.ToString().Equals("姓名"))
                chaxunfangshi = "username";
            else if (comboBox3.SelectedItem.ToString().Equals("电话"))
                chaxunfangshi = "phone";
            else if (comboBox3.SelectedItem.ToString().Equals("房间号"))
            {
                chaxunfangshi = "roomId";
            }
            try
            {
                sqltext1 = "select * from userinf where " + chaxunfangshi + " like '%" + textBox6.Text + "%'";
                DataTable dt = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext1, null).Tables[0];
                var v = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    // 推送消息
                    // MessageBox.Show(dr["pay_state"].ToString());
                    if (dr["pay_state"].ToString() == "0")
                    {
                        Get_WeChat(dr["roomId"].ToString(), dr["username"].ToString(), dr["uid"].ToString());
                    }
                    else
                    {
                        v.Add(dr["roomId"].ToString() + dr["username"].ToString());
                    }
                }
                if (v.Count() > 0)
                {
                    string message = "";
                    foreach (string s in v)
                    {
                        message += (s + "\n");
                    }
                    message += "已缴纳租金！";
                    MessageBox.Show(message);
                }
            }
            catch (Exception)
            {

            }
        }
        private void Get_WeChat(string roomId, string name, string uid)
        {
            // 查询到该用户应该要支付的租金
            string sqltext = "select jine from billsinf where roomId ='" + roomId + "'";
            DataSet ds = new DataSet();
            ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext, null);
            string jine = ds.Tables[0].Rows[0][0].ToString();
            string content = "您好，亲爱的" + roomId + "住户" + name + "先生,截止" + DateTime.Now.ToLongDateString().ToString() + "您还未支付水电费等费用，共计" + 
                jine + "元, 请及时支付!";
            string key = "http://wxpusher.zjiecode.com/api/send/message/?appToken=AT_dwLZt3OratGHpEtUuy0tlCRPjjoJOEmP";
            string url = key + "&content=" + content + "&uid=" + uid;
            Uri httpURL = new Uri(url);
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(httpURL);
            httpReq.GetResponse();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                String sqltext1 = "select * from userinf where pay_state = 0" ;
                DataTable dt = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext1, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    Get_WeChat(dr["roomId"].ToString(), dr["username"].ToString(), dr["uid"].ToString());
                }
            }
            catch (Exception)
            {

            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (userdataGridView.Rows.Count == 0)
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
                    for (int i = 0; i < userdataGridView.ColumnCount; i++)
                    {
                        if (i > 0)
                            strLine += ",";
                        strLine += userdataGridView.Columns[i].HeaderText;
                    }
                    strLine.Remove(strLine.Length - 1);
                    sw.WriteLine(strLine);
                    strLine = "";
                    //表的内容
                    for (int j = 0; j < userdataGridView.Rows.Count; j++)
                    {
                        strLine = "";
                        int colCount = userdataGridView.Columns.Count;
                        for (int k = 0; k < colCount; k++)
                        {
                            if (k > 0 && k < colCount)
                                strLine += ",";
                            if (userdataGridView.Rows[j].Cells[k].Value == null)
                                strLine += "";
                            else
                            {
                                string cell = userdataGridView.Rows[j].Cells[k].Value.ToString().Trim();
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
    }
}
