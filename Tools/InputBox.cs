using System;
using System.Windows.Forms;

namespace Export.Tools
{
    class InputBox : Form
    {
        /// <summary>
        /// 提示显示
        /// </summary>
        Label label;
        /// <summary>
        /// 输入框
        /// </summary>
        TextBox textBox;
        /// <summary>
        /// 确认按钮
        /// </summary>
        Button buttonOK;

        /// <summary>
        /// 最终返回值
        /// </summary>
        public string value
        {
            get;
            private set;
        } = "";

        public InputBox():this("请输入内容: ")
        {

        }

        public InputBox(string tishi)
        {
            label = new Label() { Left = 50, Top = 20, Width = 200, Text = tishi };
            textBox = new TextBox() { Left = 50, Top = 50, Width = 200 };
            buttonOK = new Button() { Text = "OK", Left = 150, Width = 100, Top = 80 };
            buttonOK.Click += new EventHandler(ButtonOK_Click);
            Controls.Add(label);
            Controls.Add(textBox);
            Controls.Add(buttonOK);
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            this.value = textBox.Text;
            this.Close();
        }
    }
}
