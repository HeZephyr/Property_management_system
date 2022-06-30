using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Property_management_system
{
    public partial class MainMenu : Form
    {
        static string usernamestr;
        public MainMenu()
        {
            usernamestr = "admin";
            InitializeComponent();
        }

        public MainMenu(string user)
        {
            usernamestr = user;
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = "物业管理系统       当前用户：" + usernamestr;
            // 判断是否为root
            if (usernamestr.Equals("root"))
            {
                toolStripButton3.Visible = true;
                toolStripButton2.Visible = true;
            }
            else
            {
                toolStripButton3.Visible = false;
                toolStripButton2.Visible = false;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Controls.Clear();
            Userinf userinf = new Userinf();
            userinf.TopLevel = false;
            userinf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            userinf.WindowState = FormWindowState.Normal;
            userinf.Dock = DockStyle.Fill;
            userinf.KeyPreview = true;
            userinf.Parent = splitContainer1.Panel2;   
            userinf.Show();
        }
        private void ImportData(string filePath)
        {
            if (filePath.Length == 0)
            {
                MessageBox.Show("请选择要导入的CSV文件");
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    //不读第一行的标题
                    sr.ReadLine();

                    string row_text = "";
                    string Exceptions = "";

                    List<string> sqlList = new List<string>();

                    while ((row_text = sr.ReadLine()) != null)
                    {
                        string[] datas = row_text.Split(',');
                        string sql = "insert into userinf (roomId,level,seat,username,sex,idCard,phone,uid) values ('" + 
                            datas[0] + "','" + datas[1] + "','" + datas[2] + "','" + datas[3] + "','" + datas[4] + "','" + datas[5] 
                            + "','" + datas[6] + "','" + datas[7] + "')";
                        sqlList.Add(sql);
                    }
                    try
                    {
                        MySqlHelper.ExSqlList(sqlList, out Exceptions);

                        MessageBox.Show("导入数据成功，共导入行数：" + sqlList.Count.ToString());
                    }
                    catch (Exception ex)
                    {
                        Exceptions = ex.Message.ToString();
                        MessageBox.Show("导入数据出错，错误信息：" + Exceptions);
                    }
                }
            }
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false; //该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "CSV文件(*.csv)|*.csv";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filename = dialog.FileName;
                // MessageBox.Show(file);
                ImportData(filename);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Managerinf minf = new Managerinf();
            minf.ShowDialog();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Controls.Clear();
            ExpenseOperation binf = new ExpenseOperation();
            binf.TopLevel = false;
            binf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            binf.WindowState = FormWindowState.Normal;
            binf.Dock = DockStyle.Fill;
            binf.KeyPreview = true;
            binf.Parent = splitContainer1.Panel2;
            binf.Show();
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Othersetting oset = new Othersetting(usernamestr); // 打开设置窗口
            oset.ShowDialog();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Controls.Clear();
            Financeinf finance = new Financeinf();
            finance.TopLevel = false;
            finance.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            finance.WindowState = FormWindowState.Normal;
            finance.Dock = DockStyle.Fill;
            finance.KeyPreview = true;
            finance.Parent = splitContainer1.Panel2;
            finance.Show();
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ChangePwd cupe = new ChangePwd(usernamestr);
            cupe.ShowDialog();
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            this.Hide();   
            login.Show();
        }
    }
}
