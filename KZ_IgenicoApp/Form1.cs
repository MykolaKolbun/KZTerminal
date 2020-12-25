using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KZIngenicoXLib;

namespace KZ_IgenicoApp
{
    public partial class Form1 : Form
    {
        IPos pos1;

        [DllImport("user32.dll")]
        static public extern bool OemToCharA(char[] lpszSrc, [Out] StringBuilder lpszDst);

        [DllImport("user32.dll")]
        static public extern bool OemToChar(IntPtr lpszSrc, [Out] StringBuilder lpszDst);
        public Form1()
        {
            InitializeComponent();
            pos1 = new Pos();

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

            try
            {
                int res = pos1.Initialize(@"c:\My Programming\.Net\KZ_Ingenico_EPI\Supply folder\trgui.dll", @"c:\My Programming\.Net\KZ_Ingenico_EPI\Supply folder\setup.txt");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }

        private void btnPurch_Click(object sender, EventArgs e)
        {
            try
            {
                int error = pos1.StartPurchase(12, "01", "1000002587");
                int temp = 0;
                while(pos1.lastError == 2)
                {
                    if(temp != pos1.LastStatMsgCode)
                    {
                        StringBuilder outStr = new StringBuilder();
                        OemToCharA((pos1.LastStatMsgDescription+'\0').ToArray(), outStr);
                        richTextBox1.AppendText(outStr.ToString());
                        temp = pos1.LastStatMsgCode;
                    }
                }
                if (pos1.lastError == 0)
                {
                    if (pos1.ResponseCode == 0)
                    {
                        MessageBox.Show(pos1.Receipt);
                    }
                    else
                    {
                        MessageBox.Show(pos1.LastErrorDescription);
                    }
                }
                
            }
            catch (Exception ex)
            {
                //pos.Close();
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //pos.Close();
            this.Close();
        }

        private void btnSettlement_Click(object sender, EventArgs e)
        {}

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //pos.Cancel();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //pos1.Add(1,2, out double b);
            //pos1.Connect1();
            
            //if (res == 0)
            //{
            //    pos1.Purchase1(12, "01", "100000258");
            //}
            //MessageBox.Show("b.ToString()");
        }
    }
}
