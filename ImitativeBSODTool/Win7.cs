using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ImitativeBSODTool
{
    public partial class Win7 : Form
    {
        int i = 0;
        // 控制鼠标指针显示和隐藏
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        //0表示隐藏鼠标指针，1表示显示鼠标指针
        public static extern void ShowCursor(int status);
        public Win7()
        {
            InitializeComponent();
            x = this.Width;
            y = this.Height;
            setTag(this);
        }
        private float x;//定义当前窗体的宽度
        private float y;//定义当前窗体的高度
        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ";" + con.Height + ";" + con.Left + ";" + con.Top + ";" + con.Font.Size;
                if (con.Controls.Count > 0)
                {
                    setTag(con);
                }
            }
        }
        private void setControls(float newx, float newy, Control cons)
        {
            //遍历窗体中的控件，重新设置控件的值
            foreach (Control con in cons.Controls)
            {
                //获取控件的Tag属性值，并分割后存储字符串数组
                if (con.Tag != null)
                {
                    string[] mytag = con.Tag.ToString().Split(new char[] { ';' });
                    //根据窗体缩放的比例确定控件的值
                    con.Width = Convert.ToInt32(System.Convert.ToSingle(mytag[0]) * newx);//宽度
                    con.Height = Convert.ToInt32(System.Convert.ToSingle(mytag[1]) * newy);//高度
                    con.Left = Convert.ToInt32(System.Convert.ToSingle(mytag[2]) * newx);//左边距
                    con.Top = Convert.ToInt32(System.Convert.ToSingle(mytag[3]) * newy);//顶边距
                    Single currentSize = System.Convert.ToSingle(mytag[4]) * newy;//字体大小
                    con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                    if (con.Controls.Count > 0)
                    {
                        setControls(newx, newy, con);
                    }
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            i += 5;
            label2.Text = i.ToString();
            var n = new Random();
            int a = n.Next(500, 5000);
            timer1.Interval = a;
            if (MainForm.noclose == true)
            {
                label2.Text = "255";
                timer1.Stop();
            }
            if (i == 55)
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

        private void Win7_Load(object sender, EventArgs e)
        {
            //隐藏鼠标
            ShowCursor(0);
            float newx = (this.Width) / x;
            float newy = (this.Height) / y;
            setControls(newx, newy, this);
        }

        private void Win7_FormClosing(object sender, FormClosingEventArgs e)
        {
            //显示鼠标
            ShowCursor(1);
        }
    }
}
