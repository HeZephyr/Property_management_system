using System;
using System.Windows.Forms;
using System.Data;

namespace Property_management_system
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        private void labelclose_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
        private void loginbutton_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();
                string username, password;
                string sqltext1;
                username = usertextBox.Text; // 获取账号和密码
                password = textBoxpwd.Text;
                sqltext1 = "select password from manageruser where username = '" + username + "'";
                ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, sqltext1, null);
                if (ds.Tables[0].Rows.Count < 1)
                {
                    MessageBox.Show("用户名不存在");
                }
                else
                {
                    string truepwd = ds.Tables[0].Rows[0]["password"].ToString();
                    if (truepwd.CompareTo(password) == 0)
                    {
                        MainMenu mainform = new MainMenu(username);
                        this.Hide();
                        mainform.Show();
                    }
                    else
                        MessageBox.Show("密码错误");
                }
            }
            catch (Exception)
            {

            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Register register = new Register();
            register.Show();
        }
    }
}
