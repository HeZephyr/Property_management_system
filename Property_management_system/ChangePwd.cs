using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Property_management_system
{
    public partial class ChangePwd : Form
    {
        static string currentusernamestr;
        public ChangePwd(string currentusername)
        {
            currentusernamestr = currentusername;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string oldpwd, newpwd, confirmpwd, sqltext, sqltext1;
                DataSet ds = new DataSet();
                oldpwd = textBox1.Text;
                newpwd = textBox2.Text;
                confirmpwd = textBox3.Text;
                int flag;//判断update命令是否执行成功
                sqltext = "select password from manageruser where username = '" + currentusernamestr + "'";
                sqltext1 = "update manageruser set password='" + newpwd + "' where username='" + currentusernamestr + "'";
                ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext, null);
                if (ds.Tables[0].Rows[0]["password"].ToString().Equals(oldpwd))
                {
                    Regex regex = new Regex(@"(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{8,30}");
                    // 判断密码格式是否符合要求
                    if (regex.IsMatch(newpwd))
                    {
                        if (newpwd == confirmpwd)
                        {
                            flag = MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext1, null);
                            if (flag == 1)
                                MessageBox.Show("修改成功！");
                            else
                                MessageBox.Show("修改失败！");
                        }
                        else
                        {
                            MessageBox.Show("两次密码输入不一致！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("输入密码格式错误！");
                    }
                }
                else
                    MessageBox.Show("原密码输入不正确！");
            }
            catch (Exception)
            {
                
            }
            
        }
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox1.Focus();
        }
    }
}
