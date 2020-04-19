using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Library.IO;
using Library.ClassParser;
using Library.ClassCreator;
using Library;
using Library.Database;
// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace StatEditor
{


	public partial class StatEditorForm : Form
	{
		
        enum ELOAD_STATE
        {
            configure,
            waiting_for_core,
            done
        };
        private bool m_bDirty = false;
        private Database m_StatsDatabase = null;
        private ELOAD_STATE m_eLoadState = ELOAD_STATE.configure;
        private ContextMenu m_StatListViewContextMenu = new ContextMenu();
		private ListViewItem m_ContextMenuItem = null;
        private ClassContructionAndDBHandler m_Core = null;
        private Dictionary<ClassInstance, ListViewItem> m_ObjectsToListViewItem = new Dictionary<ClassInstance, ListViewItem>();
		public StatEditorForm()
		{
			InitializeComponent();
#if DEBUG
            this.TopMost = false;
#else
            this.TopMost = true;
#endif

        }

        private void _setDirty(bool bDirty)
        {
            if( bDirty == m_bDirty)
            {
                return;
            }
            m_bDirty = bDirty;
            if(bDirty)
            {
                this.Text = "Stat Editor *";
            }
            else
            {
                this.Text = "Stat Editor";
            }
        }

        private void StatEditorForm_Shown(object sender, EventArgs e)
		{
           
            timerProcessClasses.Enabled = true;
			
		}
        

		public void log(string strMessage)
		{
			if(m_Core != null)
			{
                m_Core.log(strMessage);
			}
			
		}
        

		private void timerProcessClasses_Tick(object sender, EventArgs e)
		{

			switch(m_eLoadState)
			{
				case ELOAD_STATE.configure:
				{
                    m_eLoadState = ELOAD_STATE.waiting_for_core;
                    if (m_Core == null)
                    {
                        m_Core = new ClassContructionAndDBHandler();
                        m_Core.showProgressBar(this);
                    }
                    
                }
                break;
                case ELOAD_STATE.waiting_for_core:
				{
					if(m_Core != null &&
                        m_Core.isLoaded())
                    {
                        m_eLoadState = ELOAD_STATE.done;
                    }
					
				}
				break;				
				case ELOAD_STATE.done:
				{
                    _setDatabase("Stats");
                    
                    //object mObject = m_Core.classCreatorManager.createNewClass("KCIncludeTest");// FKCStatDefinition");// new ClassTestingObjectViewer();
                    //statObjectViewer.setObjectViewing(mObject);
                    timerProcessClasses.Enabled = false;
                    this.Focus();
                    this.Activate();
                    this.TopMost = false;
                }
				break;
			}

		}

        private ColumnHeader _addColumn(string strVariableName, PropertyFilterData mPropertyFilterData)
        {
            if(m_StatsDatabase == null)
            {
                return null;
            }
            Type mType = m_StatsDatabase.getDatabaseEntryClassType();
            if( mType == null )
            {
                log("Error - unable to add column. The class :" + m_StatsDatabase.databaseEntryClass + " could not be found.");
                return null;
            }
            PropertyInfo mProperty = mType.GetProperty(strVariableName);
            if(mProperty == null )
            {
                log("Error - unable to add column. property: " + strVariableName + " could not be found inside class: " + m_StatsDatabase.databaseEntryClass);
                return null;
            }
            string strDisplayName = strVariableName;
            object[] mAttributes = mProperty.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (mAttributes.Length != 0)
            {
                strDisplayName = (mAttributes[0] as DisplayNameAttribute).DisplayName;
                if(strDisplayName == "")
                {
                    strDisplayName = strVariableName;
                }
            }
            ColumnHeader mHeader = new ColumnHeader();
            mHeader.DisplayIndex = m_StatListView.Columns.Count;
            mHeader.Text = strDisplayName;
            if (mPropertyFilterData != null )
            { 
                if(mPropertyFilterData.OverrideName != "")
                {
                    mHeader.Text = mPropertyFilterData.OverrideName;
                }
                if (mPropertyFilterData.ColumnWidth > 0)
                {
                    mHeader.Width = mPropertyFilterData.ColumnWidth;
                }

            }

            m_StatListView.Columns.Add(mHeader);
            return mHeader;
        }

        private void _setDatabase(string strDatabase)
        {
            m_StatsDatabase = m_Core.databaseManager.getDatabase(strDatabase);      
            if( m_StatsDatabase == null)
            {
                log("ERROR - unable to find database: " + strDatabase);
            }
            m_StatListView.SuspendLayout();
            m_StatListView.Items.Clear();
            m_StatListView.Columns.Clear();
            foreach(PropertyFilterData mPropertyFilterData in m_StatsDatabase.getConfig().propertyFilters)
            {

                ColumnHeader mHeader = _addColumn(mPropertyFilterData.VariableName, mPropertyFilterData);
                
            }

            if(m_StatListView.Columns.Count == 0 )
            {
                _addColumn("m_strName", null);
            }
            foreach (ClassInstance mInstance in m_StatsDatabase.getEntries())
            {
                _addInstanceToListView(mInstance);
            }
            m_StatListView.ResumeLayout();
        }

		private void _createContextMenu(Point mousePoint)
		{

			m_StatListViewContextMenu.MenuItems.Clear();
			m_StatListViewContextMenu.MenuItems.Add("Create New Stat", _createNewStat);


			m_ContextMenuItem = m_StatListView.GetItemAt(mousePoint.X, mousePoint.Y);
			if (m_ContextMenuItem != null)
			{
				
				m_StatListViewContextMenu.MenuItems.Add("Rename " + m_ContextMenuItem.SubItems[1].Text, _renameStat);
				m_StatListViewContextMenu.MenuItems.Add("-");
				m_StatListViewContextMenu.MenuItems.Add("Delete " + m_ContextMenuItem.SubItems[1].Text, _deleteStat);
			}
		}
        

        private bool _updateListItemByInstance(ClassInstance mInstance)
        {
            if(m_ObjectsToListViewItem.ContainsKey(mInstance) == false)
            {
                MessageBox.Show("The map containing entries to the list view items appears to be out of sync.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            ListViewItem mNewListViewItem = m_ObjectsToListViewItem[mInstance];
            if( mNewListViewItem == null)
            {
                MessageBox.Show("The map containing entries to the list view items appears to be out of sync.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            string strName = mInstance.getAnyPropertyAsString("m_strName");
            if (strName == null || strName == "")
            {
                MessageBox.Show("Unable to get name from object.", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            string strGuid = mInstance.getAnyPropertyAsString("m_DatabaseGuid");
            string strType = mInstance.getAnyPropertyAsString("m_eStatType");
            string strGraph = mInstance.getAnyPropertyAsString("m_strGraph");

            bool bChanged = false;
            if (mNewListViewItem.Text != strName)
            {
                mNewListViewItem.Text = strName;
                bChanged = true;
            }
            if (mNewListViewItem.SubItems.Count < 2)
            {
                mNewListViewItem.SubItems.Add(strGuid);
                bChanged = true;
            }
            else if(mNewListViewItem.SubItems[1].Text != strGuid)
            {
                mNewListViewItem.SubItems[1].Text = strGuid;
                bChanged = true;
            }
            if (mNewListViewItem.SubItems.Count < 3)
            {
                mNewListViewItem.SubItems.Add(strType);
                bChanged = true;
            }
            else if (mNewListViewItem.SubItems[2].Text != strType)
            {
                mNewListViewItem.SubItems[2].Text = strType;
                bChanged = true;
            }
            if (mNewListViewItem.SubItems.Count < 4)
            {
                mNewListViewItem.SubItems.Add(strGraph);
                bChanged = true;
            }
            else if (mNewListViewItem.SubItems[3].Text != strGraph)
            {
                mNewListViewItem.SubItems[3].Text = strGraph;
                bChanged = true;
            }
            

            return bChanged;
        }

        private bool _addInstanceToListView(ClassInstance mInstance)
        {
            if( mInstance == null)
            {
                return false;
            }
            string strName = mInstance.getPropertyValueString("m_strName", "");
            if( strName == null || strName == "")
            {
                MessageBox.Show("Unable to get name from object.", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            ListViewItem mNewListViewItem = new ListViewItem(strName);
            m_StatListView.Items.Add(mNewListViewItem);
            m_ObjectsToListViewItem.Add(mInstance, mNewListViewItem);
            
            if( _updateListItemByInstance(mInstance))
            {
                statObjectViewer.setObjectViewing(mInstance);
                return true;
            }
            return false;
        }

		private void _createNewStat(object sender, EventArgs e)
		{
            InputBox mInputBox = new InputBox();
            mInputBox.Text = "New Stat Name";
            mInputBox.message = "Please enter in a unique name for the stat.";
            if( mInputBox.ShowDialog(this) == DialogResult.OK)
            {
                if (m_StatsDatabase.isValidName(mInputBox.inputBox))
                {
                    if(m_StatsDatabase.createEntry(mInputBox.inputBox) == EERROR_ADDING.NO_ERRORS)
                    {
                        ClassInstance mInstance = m_StatsDatabase.getEntryByName(mInputBox.inputBox);
                        if(mInstance != null)
                        {
                            _addInstanceToListView(mInstance);
                            _setDirty(true);
                            return;
                        }
                    }
                    MessageBox.Show("An error occurred adding the stat. Please check the log.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                else
                {
                    MessageBox.Show("Invalid name. Name appears to not be unique.", "invalid name", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }
		private void _renameStat(object sender, EventArgs e)
		{

		}
		private void _deleteStat(object sender, EventArgs e)
		{

		}

		private void m_StatListView_MouseDown(object sender, MouseEventArgs e)
		{
			Point mControlLocation = PointToClient(PointToScreen(m_StatListView.Location));
			Point mousePoint = new Point(e.X, e.Y);// + mControlLocation.X, e.Y + mControlLocation.Y);



			if (e.Button == MouseButtons.Right)
			{
				//context menu
				_createContextMenu(mousePoint);
				if (m_StatListViewContextMenu.MenuItems.Count != 0)
				{
					mousePoint.X += 8;
					mousePoint.Y -= 5;
					m_StatListViewContextMenu.Show(m_StatListView, mousePoint);
				}
			}
		}

        private void databasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_Core.databaseManager.showConfigDialog(this);
        }

        private void variableTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_Core.classCreatorManager.showVariableDefinitionEditor(this);
        }

        private void StatEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if( m_bDirty)
            {
                DialogResult mResult = MessageBox.Show("You have unsaved changes. Do you want to save first?", "Save first?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if( mResult == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if( mResult == DialogResult.Yes)
                {
                    if( _save() == false )
                    {
                        MessageBox.Show("Error in saving. Check log. Not quiting.", "Error?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _save();
        }

        private bool _save()
        {
            if( m_StatsDatabase.saveDatabase())
            {
                if( m_StatsDatabase.getEntrysDirtyCount() > 0)
                {
                    _setDirty(true);
                    return false;
                }
                _setDirty(false);
                return true;
            }
            return false;
        }

        public int getClassInstancesSelected(List<ClassInstance> mSelected)
        {
            mSelected.Clear();
            foreach (ListViewItem mItem in m_StatListView.SelectedItems)
            {
                ClassInstance mInstance = m_StatsDatabase.getEntryByName(mItem.Text);
                if(mInstance != null)
                {
                    mSelected.Add(mInstance);
                }
            }
            return mSelected.Count;
        }

        private void m_StatListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( m_StatListView.SelectedIndices.Count == 0 )
            {
                statObjectViewer.setObjectViewing(null);
                return;
            }
            if(m_StatListView.SelectedIndices.Count == 1)
            {
                ClassInstance mInstance = m_StatsDatabase.getEntryByName(m_StatListView.SelectedItems[0].Text);
                statObjectViewer.setObjectViewing(mInstance);
            }
            else
            {
                statObjectViewer.setObjectViewing(null);
                foreach(ListViewItem mItem in m_StatListView.SelectedItems)
                {
                    ClassInstance mInstance = m_StatsDatabase.getEntryByName(mItem.Text);
                    statObjectViewer.addObjectViewing(mInstance);
                }
            }
        }

        private void m_StatListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_StatListView.SelectedIndices.Count == 0)
            {
                return;
            }
            

            if (e.Control &&
                e.KeyCode == Keys.C)
            {
                List<ClassInstance> mInstances = new List<ClassInstance>();
                
                if(getClassInstancesSelected(mInstances) > 0)
                {
                    string strCopyString = "";
                    foreach (ClassInstance mInstance in mInstances)
                    {
                        strCopyString = strCopyString + m_StatsDatabase.getEntryName(mInstance) + Environment.NewLine;
                    }
                    strCopyString = strCopyString.Substring(0, strCopyString.Length - 1);
                    Clipboard.SetText(strCopyString);
                    e.SuppressKeyPress = true;
                }                                
            }
            else if (e.Control &&
                     e.KeyCode == Keys.G)
            {
                List<ClassInstance> mInstances = new List<ClassInstance>();
                if (getClassInstancesSelected(mInstances) > 0)
                {
                    string strCopyString = "";
                    foreach (ClassInstance mInstance in mInstances)
                    {
                        strCopyString = strCopyString + m_StatsDatabase.getEntryGuid(mInstance) + Environment.NewLine;
                    }
                    strCopyString = strCopyString.Substring(0, strCopyString.Length - 1);
                    Clipboard.SetText(strCopyString);
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void statObjectViewer_PropertyValueChanged(object m, EventArgs e)
        {
            m_StatListView.SuspendLayout();
            foreach (ListViewItem mItem in m_StatListView.SelectedItems)
            {
                int iGuid = -1;
                int.TryParse(mItem.SubItems[1].Text, out iGuid);                    
                ClassInstance mInstance = m_StatsDatabase.getEntryByGuid(iGuid);
                if (mInstance != null)
                {
                    _updateListItemByInstance(mInstance);
                    
                        
                    
                }
            }
            m_StatListView.ResumeLayout();
            _setDirty(true);
        }
    }//end class
} //end namespace
