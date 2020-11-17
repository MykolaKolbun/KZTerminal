using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KZ_IgenicoApp
{
    public partial class Form1 : Form
    {
        TrposiXLib pos;
        public Form1()
        {
            InitializeComponent();
            pos = new TrposiXLib();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

            try
            {
                MessageBox.Show($"Result: {pos.Init()}");
            }
            catch (Exception ex)
            {
                pos.Close();
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }

        private void btnPurch_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show($"Result: {pos.Purchase(1000)}");
            }
            catch (Exception ex)
            {
                pos.Close();
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            pos.Close();
            this.Close();
        }
    }
}
