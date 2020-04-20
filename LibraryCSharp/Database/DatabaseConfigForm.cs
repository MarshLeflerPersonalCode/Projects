using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Library.Database
{
    public partial class DatabaseConfigForm : Form
    {
        private bool m_bDirty = false;
        private DatabaseManager m_DatabaseManager = null;
        public DatabaseConfigForm(DatabaseManager mDatabaseManager)
        {
            InitializeComponent();
            m_DatabaseManager = mDatabaseManager;
            _refresh();
            
        }

        private void _updateDirty(bool bDirty)
        {
            m_bDirty = bDirty;
            if( bDirty )
            {
                this.Text = "Database Config Form *";
            }
            else
            {
                this.Text = "Database Config Form";
            }
        }

        private void _refresh()
        {
            txtBoxRootFolder.Text = Path.GetFullPath( m_DatabaseManager.getDatabaseDirectory());
            lstViewDatabases.Items.Clear();
            foreach (Database mDatabase in m_DatabaseManager.databases)
            {
                ListViewItem mNewItem = new ListViewItem(mDatabase.databaseName);                
                mNewItem.SubItems.Add(mDatabase.getDatabaseConfigPathAndFile());
                mNewItem.SubItems.Add(mDatabase.databaseEntryClass);
                lstViewDatabases.Items.Add(mNewItem);
            }
            if(lstViewDatabases.Items.Count > 0 )
            {
                lstViewDatabases.Items[lstViewDatabases.Items.Count - 1].Focused = true;
                lstViewDatabases.Items[lstViewDatabases.Items.Count - 1].Selected = true;
            }
            _updateDirty(m_bDirty);
        }

        private void lstViewDatabases_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( lstViewDatabases.SelectedItems == null ||
                lstViewDatabases.SelectedItems.Count == 0 )
            {
                propertyGridDBView.SelectedObject = null;
                return;
            }
            foreach (Database mDatabase in m_DatabaseManager.databases)
            {
                if(mDatabase.databaseName == lstViewDatabases.SelectedItems[0].SubItems[0].Text)
                {
                    propertyGridDBView.SelectedObject = mDatabase.getConfig();
                    return;
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _save();
        }

        private void _save()
        {
            foreach (Database mDatabase in m_DatabaseManager.databases)
            {
                mDatabase.saveDatabase();
            }
            _updateDirty(false);
        }

        private void propertyGridDBView_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            _updateDirty(true);
        }

        private void DatabaseConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(m_bDirty)
            {
                if(MessageBox.Show("Do you want to save changes before closing?", "Save Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _save();
                    //e.Cancel = true;                    
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            InputBox mInputBox = new InputBox();
            mInputBox.Text = "Database Name";
            mInputBox.message = "Please enter in the database name to create.";
            if(mInputBox.ShowDialog(this) == DialogResult.OK)
            {
                if(m_DatabaseManager.createDatabase(mInputBox.inputBox))
                {
                    _refresh();
                    _updateDirty(true);
                }
            }
        }
    }
}
