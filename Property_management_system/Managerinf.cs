using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Property_management_system
{
    public partial class Managerinf : Form
    {
        public Managerinf()
        {
            InitializeComponent();
        }

        private void Managerinf_Load(object sender, EventArgs e)
        {
            try
            {
                string sqltext1 = "select * from manageruser ";
                managerdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext1, null).Tables[0].DefaultView;
                managerdataGridView.Columns["username"].HeaderText = "用户名";
                managerdataGridView.Columns["password"].HeaderText = "密码";
            }
            catch (Exception)
            {

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string username, password;
                username = textBox1.Text;
                password = textBox2.Text;
                if (IsSafe(password))
                {
                    // 添加之前 Select 检查一下
                    String sql = "SELECT username FROM `manageruser` WHERE username = " + "'" + username + "'";
                    MySqlDataReader Flag = MySqlHelper.ExecuteReader(MySqlHelper.Conn, CommandType.Text, sql, null);
                    if (Flag.HasRows)
                    {
                        MessageBox.Show("用户名存在,请重新输入");
                    }
                    else
                    {
                        string sqltext1 = "insert into manageruser values('" + username + "','" + password + "')";
                        MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext1, null);
                        managerdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, "select * from manageruser ", null).Tables[0].DefaultView;
                        MessageBox.Show("新用户添加成功!");
                        textBox1.Text = "";
                        textBox2.Text = "";
                        textBox1.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("密码格式错误,请重新输入");
                    textBox2.Focus();
                }
            }
            catch (Exception)
            {

            }
        }
        public bool IsSafe(String NewPassword)
        {
            Regex objLen = new Regex("^.{3,16}$");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");
            return objLen.IsMatch(NewPassword);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string username, password;
                username = textBox1.Text;
                password = textBox2.Text;
                string sqltext1 = "update manageruser set password='" + password + "' where username = '" + username + "'";
                MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext1, null);
                managerdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, "select * from manageruser ", null).Tables[0].DefaultView;
            }
            catch (Exception)
            {

            }   
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("您确定要删除吗？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string username;
                    username = textBox1.Text;
                    string sqltext1 = "delete from manageruser where username ='" + username + "'";
                    MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext1, null);
                    managerdataGridView.DataSource = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, "select * from manageruser ", null).Tables[0].DefaultView;
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
