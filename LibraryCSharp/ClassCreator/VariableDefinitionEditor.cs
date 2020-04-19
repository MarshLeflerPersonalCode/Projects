using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library.ClassCreator
{
    public partial class VariableDefinitionEditor : Form
    {
        private bool m_bDirty = false;
        private ClassCreatorManager m_ClassCreator = null;
        public VariableDefinitionEditor(ClassCreatorManager mManager)
        {
            InitializeComponent();
            m_ClassCreator = mManager;
            _refresh();
        }

        private void _updateDirty(bool bDirty)
        {
            m_bDirty = bDirty;
            if(m_bDirty)
            {
                this.Text = "Variable Definition Editor *";
            }
            else
            {
                this.Text = "Variable Definition Editor";
            }
        }

        private void _refresh()
        {
            lstBoxVariables.Items.Clear();
            foreach(VariableDefinition mDefinition in m_ClassCreator.variableDefinitionHandler.m_VariableDefinitions)
            {
                lstBoxVariables.Items.Add(mDefinition.variableName);
            }
            if (lstBoxVariables.Items.Count > 0)
            {
                lstBoxVariables.SelectedIndex = lstBoxVariables.Items.Count - 1;
            }
        }

        private void propertyGridVariables_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            _updateDirty(true);
            int iIndex = lstBoxVariables.SelectedIndex;
            if (iIndex < 0 ||
                iIndex >= m_ClassCreator.variableDefinitionHandler.m_VariableDefinitions.Count)
            {
                propertyGridVariables.SelectedObject = null;
                return;
            }
            if(lstBoxVariables.Items[iIndex].ToString() != m_ClassCreator.variableDefinitionHandler.m_VariableDefinitions[iIndex].variableName)
            {
                lstBoxVariables.Items[iIndex] = m_ClassCreator.variableDefinitionHandler.m_VariableDefinitions[iIndex].variableName;
            }
        }

        private void lstBoxVariables_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iIndex = lstBoxVariables.SelectedIndex;
            if (iIndex < 0 ||
                iIndex >= m_ClassCreator.variableDefinitionHandler.m_VariableDefinitions.Count)
            {
                propertyGridVariables.SelectedObject = null;
                return;
            }
            propertyGridVariables.SelectedObject = m_ClassCreator.variableDefinitionHandler.m_VariableDefinitions[iIndex];

        }

        private void VariableDefinitionEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if( m_bDirty)
            {
                DialogResult mResult = MessageBox.Show("Do you want to save changes first?", "Save Changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if ( mResult == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if( mResult == DialogResult.Yes)
                {
                    m_ClassCreator.variableDefinitionHandler.save();
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            VariableDefinition mDefinition = new VariableDefinition();
            mDefinition.variableName = "NEW VARIABLE";
            m_ClassCreator.variableDefinitionHandler.addVariable(mDefinition);
            _refresh();
            _updateDirty(true);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int iIndex = lstBoxVariables.SelectedIndex;
            if (iIndex < 0 ||
                iIndex >= m_ClassCreator.variableDefinitionHandler.m_VariableDefinitions.Count)
            {
                propertyGridVariables.SelectedObject = null;
                return;
            }
            DialogResult mResult = MessageBox.Show("Are you sure you want to delete:" + m_ClassCreator.variableDefinitionHandler.m_VariableDefinitions[iIndex].variableName, "Delete?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (mResult == DialogResult.Yes)
            {
                m_ClassCreator.variableDefinitionHandler.m_VariableDefinitions.RemoveAt(iIndex);
                _refresh();
                _updateDirty(true);
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_ClassCreator.variableDefinitionHandler.save();
            _updateDirty(false);
        }
    }
}
