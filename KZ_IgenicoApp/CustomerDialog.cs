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
    public partial class CustomerDialog : Form
    {
        public CustomerDialog()
        {
            InitializeComponent();
        }

        public CustomerDialog(string text)
        {
            InitializeComponent();
            lblInfo.Text = text;
        }
    }
}
