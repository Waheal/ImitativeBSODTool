using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImitativeBSODTool
{
    public partial class Win11 : Form
    {
        int i = 0;
        // 控制鼠标指针显示和隐藏
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        //0表示隐藏鼠标指针，1表示显示鼠标指针
        public static extern void ShowCursor(int status);
        public Win11()
        {
            InitializeComponent();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            i +=25;
            label3.Text = i.ToString() + "% 完成";
            var n = new Random();
            int a= n.Next(500, 10000);
            timer1.Interval = a;
            if (MainForm.noclose == true)
            {
                label3.Text = "100% 完成";
                timer1.Stop();
            }
            if (i == 125)
            {
                timer1.Stop();
                if (MainForm.shutdownOS == true)
                {
                    //重启电脑
                    Process process = new Process();
                    process.StartInfo.FileName = "shutdown.exe";
                    process.StartInfo.Arguments = "-r -t 0";
                    process.Start();
                }
                else
                {
                    //显示鼠标并关闭界面
                    Close();
                }
            }
        }
        private void Win11_Load(object sender, EventArgs e)
        {
            //隐藏鼠标
            ShowCursor(0);
        }

        private void Win11_FormClosing(object sender, FormClosingEventArgs e)
        {
            //显示鼠标
            ShowCursor(1);
        }
    }
}
