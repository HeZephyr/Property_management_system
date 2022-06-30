using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Property_management_system
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void labelclose_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login login = new Login();
            login.Show();
        }

        private void loginbutton_Click(object sender, EventArgs e)
        {
            string username, password, confirmPassword;
            username = textBox.Text;
            password = textBox1.Text;
            confirmPassword = textBox2.Text;
            // 判断格式是否符合规范
            if (username != "" && password != "" && confirmPassword != "")
            {
                Regex regex = new Regex(@"(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{8,30}");
                // 判断密码格式是否符合要求
                if (regex.IsMatch(password)) 
                {
                    if (password == confirmPassword)
                    {
                        // 符合要求，再要求输入密钥
                        string pm = Interaction.InputBox("请输入管理员注册邀请码！", "输入邀请码", "", 100, 100);
                        if (pm != "123456")
                        {
                            MessageBox.Show("请输入正确的邀请码！");
                        }
                        else
                        {
                            try
                            {
                                string sqltext = "insert manageruser(username, password) values('" + username + "', '" + password + "');";
                                MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, sqltext, null);
                                MessageBox.Show("注册成功！");
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("该管理员已经存在！");
                            }
                        }
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
            {
                MessageBox.Show("注册信息请填写完整！");
            }
        }
    }
}
