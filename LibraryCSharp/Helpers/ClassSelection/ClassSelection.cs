using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library.Helpers.ClassSelection
{
    public partial class ClassSelection : Form
    {
        private List<string> m_ClassNames = new List<string>();
        private List<string> m_HelpText = new List<string>();
        private string m_strSelected = "";
        public ClassSelection()
        {
            InitializeComponent();
        }

        public string getSelectedClass() { return m_strSelected; }

        public void configure(List<string> mClasses, List<string> mHelpText)
        {
            lstBoxClasses.Items.Clear();
            lblHelpText.Text = "";
            if (mClasses == null ||
                mClasses.Count == 0 )
            {
                return;
            }
            m_ClassNames = new List<string>(mClasses);
            m_HelpText = new List<string>(mHelpText);
            _refresh();
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

        private void _refresh()
        {
            lstBoxClasses.Items.Clear();
            lblHelpText.Text = "";
            
            foreach (string strClass in m_ClassNames)
            {
                lstBoxClasses.Items.Add(strClass);
            }
            m_strSelected = m_ClassNames[0];
            lstBoxClasses.SelectedIndex = 0;
        }

        private void lstBoxClasses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lstBoxClasses.SelectedIndex >= 0 &&
               lstBoxClasses.SelectedIndex < m_HelpText.Count)
            {
                m_strSelected = lstBoxClasses.SelectedItem.ToString();
                for(int i = 0; i < m_ClassNames.Count; i++)
                {
                    if( m_ClassNames[i] == m_strSelected)
                    {
                        if (m_HelpText[i] != null)
                        {
                            lblHelpText.Text = m_HelpText[lstBoxClasses.SelectedIndex].Replace("//", Environment.NewLine);
                            return;
                        }

                    }
                }
            }
            lblHelpText.Text = "";
        }

        private void lstBoxClasses_DoubleClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
