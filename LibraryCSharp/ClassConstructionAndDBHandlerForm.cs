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
    public partial class ClassConstructionAndDBHandlerForm : Form
    {
        ClassConstructionAndDBHandlerConfig m_Config = null;
        private bool m_bDirty = false;
        public ClassConstructionAndDBHandlerForm(ClassConstructionAndDBHandlerConfig mConfig)
        {
            InitializeComponent();
            m_Config = mConfig;
            propertyGridSettings.SelectedObject = m_Config;
        }

        private void setDirty(bool bDirty)
        {
            m_bDirty = bDirty;
            if( m_bDirty)
            {
                this.Text = "Editor Settings *";
            }
            else
            {
                this.Text = "Editor Settings";
            }
        }

        private void _save()
        {
            m_Config.save();
            setDirty(false);
        }

        private void ClassConstructionAndDBHandlerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if( m_bDirty)
            {
                DialogResult mResult = MessageBox.Show("Do you want to save changes?", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if( mResult == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if( mResult == DialogResult.Yes )
                {
                    _save();
                }
            }
        }

        private void propertyGridSettings_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            setDirty(true);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _save();
        }
    }
}
