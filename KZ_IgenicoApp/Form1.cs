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
                int error = pos.Purchase(1200);
                if (error == 0)
                {
                    MessageBox.Show(pos.resp.VisualHostResponse);
                    if (pos.resp.ResponseCode == 0 && pos.resp.ResponseCode != null)
                        MessageBox.Show(pos.resp.Slip);
                }
                else
                    MessageBox.Show($"Result Error: {error}");
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

        private void btnSettlement_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show($"Result: {pos.Service()}");
            }
            catch (Exception ex)
            {
                pos.Close();
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            pos.Cancel();
        }
    }
}
