using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KZ_Ingenico_TransportLibApp
{
    public partial class Form1 : Form
    {
        TrposX_TLV pos;
        public Form1()
        {
            InitializeComponent();
            pos = new TrposX_TLV();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pos.Connect("10.10.50.112:20500");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pos.Purchase(10);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pos.FindPort("10.10.50.112", out int port);
            this.lblPort.Text = port.ToString();
        }
    }
}
