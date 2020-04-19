using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library
{
    public partial class InputBox : Form
    {
        public InputBox()
        {
            InitializeComponent();
        }

        public string inputBox
        {
            get
            {
                return txtBox.Text;
            }
            set
            {
                txtBox.Text = value;
            }
        }
        public string message
        {
            get
            {
                return txtMessage.Text;
            }
            set
            {
                txtMessage.Text = value;
            }
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if( e.KeyCode == Keys.Enter) //enter
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
