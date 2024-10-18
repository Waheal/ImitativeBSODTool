using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace ImitativeBSODTool
{
    public partial class MainForm : Form
    {
        private bool IsLoaded = false;
        public static bool shutdownOS = false;
        public static bool noclose = false;
        //检测U盘属性
        public const int WM_DEVICECHANGE = 0x219;//U盘插入后，OS的底层会自动检测到，然后向应用程序发送“硬件设备状态改变“的消息
        public const int DBT_DEVICEARRIVAL = 0x8000;  //就是用来表示U盘可用的。一个设备或媒体已被插入一块，现在可用。
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;  //一个设备或媒体片已被删除。
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //显示系统版本
            label1.Text = GetOSVersion();
            //识别系统版本并获取相应的蓝屏样式
            if (GetOSVersion().IndexOf("7") != -1)
            {
                comboBox1.SelectedIndex = 0;
            }
            if (GetOSVersion().IndexOf("8") != -1)
            {
                comboBox1.SelectedIndex = 1;
            }
            if (GetOSVersion().IndexOf("10") != -1)
            {
                comboBox1.SelectedIndex = 2;
            }
            if (GetOSVersion().IndexOf("11") != -1)
            {
                comboBox1.SelectedIndex = 3;
            }

            if (File.Exists("IBSODTConfig.json"))
            {
                JObject jsonObject = JObject.Parse(File.ReadAllText("IBSODTConfig.json", Encoding.UTF8));
                if (jsonObject["CheckUSB"] != null && (bool)jsonObject["CheckUSB"] == true)
                {
                    checkBox1.Checked = true;
                }
                if (jsonObject["AutoRestartSys"] != null && (bool)jsonObject["AutoRestartSys"] == true)
                {
                    checkBox2.Checked = true;
                }
                if (jsonObject["AutoStopImitative"] != null && (bool)jsonObject["AutoStopImitative"] == false)
                {
                    checkBox3.Checked = true;
                }
                if (jsonObject["TimerInterval"] != null && !string.IsNullOrEmpty(jsonObject["TimerInterval"].ToString()))
                {
                    checkBox4.Checked = true;
                    textBox1.Text = jsonObject["TimerInterval"].ToString();
                    try
                    {
                        //开启timer1
                        timer1.Interval = int.Parse(textBox1.Text + "000");
                        timer1.Start();
                        button3.Text = "停止";
                        textBox1.Enabled = false;
                    }
                    catch (Exception a)
                    {
                        MessageBox.Show("出现错误\n" + a);
                    }
                }
                if (jsonObject["AutoRunSoftware"] != null && (bool)jsonObject["AutoRunSoftware"] == true)
                {
                    checkBox5.Checked = true;
                }
                if (jsonObject["AutoHideSoftware"] != null && (bool)jsonObject["AutoHideSoftware"] == true)
                {
                    checkBox6.Checked = true;
                    WindowState = FormWindowState.Minimized;
                    timer2.Start();
                }
            }
            IsLoaded = true;
        }

        private void WriteConfig(string Key, JToken Obj)
        {
            if (IsLoaded == false)
            {
                return;
            }
            try
            {
                if (!File.Exists("IBSODTConfig.json"))
                {
                    File.WriteAllText("IBSODTConfig.json", string.Format("{{{0}}}", "\n"));
                }
                string jsonString = File.ReadAllText("IBSODTConfig.json", Encoding.UTF8);
                JObject jobject = JObject.Parse(jsonString);
                if (jobject.ContainsKey(Key))
                {
                    jobject[Key] = Obj;
                }
                else
                {
                    jobject.Add(Key, Obj);
                }
                string convertString = Convert.ToString(jobject);
                File.WriteAllText("IBSODTConfig.json", convertString, Encoding.UTF8);
            }
            catch
            {
                MessageBox.Show("err");
            }
        }

        //获取系统版本
        public static string GetOSVersion()
        {
            var name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                        select x.GetPropertyValue("Caption")).FirstOrDefault();
            return name != null ? name.ToString() : "Unknown";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //根据选择的样式开始蓝屏
            if (comboBox1.SelectedIndex == 0)
            {
                Form form = new Win7();
                form.Show();
            }
            if (comboBox1.SelectedIndex == 1)
            {
                Form form = new Win8();
                form.Show();
            }
            if (comboBox1.SelectedIndex == 2)
            {
                Form form = new Win10();
                form.Show();
            }
            if (comboBox1.SelectedIndex == 3)
            {
                Form form = new Win11();
                form.Show();
            }
        }

        //检测U盘插入开始蓝屏
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox4.Checked = false;
                textBox1.Enabled = false;
                button3.Enabled = false;
                WriteConfig("CheckUSB", true);
            }
            else
            {
                WriteConfig("CheckUSB", false);
            }
        }

        //蓝屏进度100%时自动重启
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox3.Checked = false;
                shutdownOS = true;
                WriteConfig("AutoRestartSys", true);
            }
            else
            {
                shutdownOS = false;
                WriteConfig("AutoRestartSys", false);
            }
        }

        //一直蓝屏（不自动退出）
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                checkBox2.Checked = false;
                noclose = true;
                WriteConfig("AutoStopImitative", false);
                if (IsLoaded)
                {
                    MessageBox.Show("按Alt+F4即可结束蓝屏");
                }
            }
            else
            {
                noclose = false;
                WriteConfig("AutoStopImitative", true);
            }
        }

        //指定倒计时后开始蓝屏
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                checkBox1.Checked = false;
                textBox1.Enabled = true;
                button3.Enabled = true;
            }
            else
            {
                WriteConfig("TimerInterval", string.Empty);
            }
        }

        //检测U盘插入，判断是否蓝屏
        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == WM_DEVICECHANGE)
                {
                    switch (m.WParam.ToInt32())
                    {
                        case DBT_DEVICEARRIVAL://U盘插入
                            //MessageBox.Show("新的U盘已插入！");
                            if (checkBox1.Checked == true)
                            {
                                if (comboBox1.SelectedIndex == 0)
                                {
                                    Form form = new Win7();
                                    form.Show();
                                }
                                if (comboBox1.SelectedIndex == 1)
                                {
                                    Form form = new Win8();
                                    form.Show();
                                }
                                if (comboBox1.SelectedIndex == 2)
                                {
                                    Form form = new Win10();
                                    form.Show();
                                }
                                if (comboBox1.SelectedIndex == 3)
                                {
                                    Form form = new Win11();
                                    form.Show();
                                }
                            }
                            break;
                            /*
                        case DBT_DEVICEREMOVECOMPLETE: //U盘卸载
                            MessageBox.Show("U盘已拔出！");
                            break;*/
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            base.WndProc(ref m);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            WriteConfig("TimerInterval", textBox1.Text);
        }

        //timer1事件
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            button3.Text = "开始倒计时";
            textBox1.Enabled = true;
            if (comboBox1.SelectedIndex == 0)
            {
                Form form = new Win7();
                form.Show();
            }
            if (comboBox1.SelectedIndex == 1)
            {
                Form form = new Win8();
                form.Show();
            }
            if (comboBox1.SelectedIndex == 2)
            {
                Form form = new Win10();
                form.Show();
            }
            if (comboBox1.SelectedIndex == 3)
            {
                Form form = new Win11();
                form.Show();
            }
        }

        //开始计时按钮单击事件
        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text.ToString() == "开始倒计时")
            {
                try
                {
                    //开启timer1
                    timer1.Interval = int.Parse(textBox1.Text + "000");
                    timer1.Start();
                    button3.Text = "停止";
                    textBox1.Enabled = false;
                }
                catch (Exception a)
                {
                    MessageBox.Show("出现错误\n" + a);
                }
            }
            else
            {
                //计时中再次单击事件
                timer1.Stop();
                button3.Text = "开始倒计时";
                textBox1.Enabled = true;
            }

        }

        //托盘图标双击事件
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1.Visible = false;
            Visible = true;
        }

        //右键托盘图标点击退出事件
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
            {
                // 获取当前用户的注册表启动项
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                // 如果启动项中不存在该应用程序，则添加该应用程序
                if (regKey.GetValue("ImitativeBSODTool") == null)
                {
                    regKey.SetValue("ImitativeBSODTool", System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
                WriteConfig("AutoRunSoftware", true);
            }
            else
            {
                // 获取当前用户的注册表启动项
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                // 如果启动项中存在该应用程序，则删除该应用程序
                if (regKey.GetValue("ImitativeBSODTool") != null)
                {
                    regKey.DeleteValue("ImitativeBSODTool");
                }
                WriteConfig("AutoRunSoftware", false);
            }
        }

        //隐藏至托盘
        private void button4_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = true;
            Visible = false;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked == true)
            {
                WriteConfig("AutoHideSoftware", true);
            }
            else
            {
                WriteConfig("AutoHideSoftware", false);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (IsLoaded)
            {
                notifyIcon1.Visible = true;
                Visible = false;
                timer2.Stop();
            }
        }
    }
}
