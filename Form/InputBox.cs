using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Export.Forms
{
    public partial class InputBox : Form
    {
        /// <summary>
        /// 最终返回值
        /// </summary>
        public string value
        {
            get;
            private set;
        } = "";

        /// <summary>
        /// 初始化输入框
        /// </summary>
        public InputBox():this("请输入内容:")
        {
            
        }

        /// <summary>
        /// 初始化输入框
        /// </summary>
        /// <param name="tishi">提示内容</param>
        public InputBox(string tishi) : this(tishi, "")
        {

        }


        /// <summary>
        /// 初始化输入框
        /// </summary>
        /// <param name="tishi">提示内容</param>
        /// <param name="value">输入框内已有内容</param>
        public InputBox(string tishi,string value)
        {
            InitializeComponent();
            this.label1.Text = tishi;
            this.textBox1.Text = value;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            this.value = textBox1.Text;
            this.Close();
        }

        private void InputBox_Load(object sender, EventArgs e)
        {
            this.ShowInTheCurrentScreenCenter();
        }        
    }

    /// <summary>
    /// 输入框追加功能
    /// </summary>
    public static class InputBoxAdd
    { 
        /// <summary>
        /// 将窗体显示在当前窗口的中心
        /// </summary>
        public static void ShowInTheCurrentScreenCenter(this Form form)
        {
           
            // 获取鼠标的当前位置
            Point mousePoint = Control.MousePosition;

            // 确定包含鼠标指针的屏幕
            Screen currentScreen = Screen.FromPoint(mousePoint);

            // 获取这个屏幕的工作区
            Rectangle workingArea = currentScreen.WorkingArea;

            // 计算窗体的新位置
            int x = workingArea.Left + workingArea.Width / 2 - form.Width / 2;
            int y = workingArea.Top + workingArea.Height / 2 - form.Height / 2;

            // 设置窗体的新位置
            form.Location = new Point(x, y);
        }
    }
}
