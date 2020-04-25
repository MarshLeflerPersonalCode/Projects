using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library.IO;
using Library.ClassCreator;
using System.Reflection;
using Library.ClassParser;
using Library.Helpers.ClassSelection;

namespace Library.Database
{
    public partial class DatabaseEditorControl : UserControl
    {
        private int m_iLastSelected = -1;
        private Database m_ActiveDatabase = null;
        private Dictionary<ClassInstance, ListViewItem> m_ObjectsToListViewItem = new Dictionary<ClassInstance, ListViewItem>();
        private ContextMenu m_StatListViewContextMenu = new ContextMenu();
        private ListViewItem m_ContextMenuItem = null;
        public DatabaseEditorControl()
        {
            InitializeComponent();
            this.statObjectViewer.SelectedObjectsHaveChangedProperties += new CustomControls.ObjectViewer.SelectedObjectsHaveChangedPropertiesHandler(this.statObjectViewer_SelectedObjectsHaveChangedProperties);
            this.statObjectViewer.RequestTypeOverride += new CustomControls.ObjectViewer.RequestTypeOverrideHandler(this.statObjectViewer_RequestTypeOverride);
            isDirty = false;
        }

        public LogFile logFile { get; set; }
        public void log(string strMessage)
        {
            if( logFile != null)
            {
                logFile.log(strMessage);
            }
        }

        public bool isDirty { get; set; }

        public Database database { get { return m_ActiveDatabase; } }
        public void setDatabase(Database mDatabase, LogFile mLogFile)
        {
            logFile = mLogFile;
            m_ActiveDatabase = mDatabase;
            if (m_ActiveDatabase == null)
            {
                log("ERROR - database control had null database passed to it.");
                return;
            }

            m_StatListView.SuspendLayout();
            m_StatListView.MultiSelect = m_ActiveDatabase.getConfig().multiSelect;
            m_StatListView.Items.Clear();
            m_StatListView.Columns.Clear();
            foreach (PropertyFilterData mPropertyFilterData in m_ActiveDatabase.getConfig().propertyFilters)
            {
                ColumnHeader mHeader = _addColumn(mPropertyFilterData.VariableName, mPropertyFilterData);
            }

            if (m_StatListView.Columns.Count == 0)
            {
                _addColumn("m_strName", null);
            }
            _filter(false);
            m_StatListView.ResumeLayout();
            isDirty = false;
        }

        private ColumnHeader _addColumn(string strVariableName, PropertyFilterData mPropertyFilterData)
        {
            if (m_ActiveDatabase == null)
            {
                return null;
            }
            Type mType = m_ActiveDatabase.getDatabaseEntryClassType();
            if (mType == null)
            {
                log("Error - unable to add column. The class :" + m_ActiveDatabase.databaseEntryClass + " could not be found.");
                return null;
            }
            PropertyInfo mProperty = mType.GetProperty(strVariableName);
            if (mProperty == null)
            {
                log("Error - unable to add column. property: " + strVariableName + " could not be found inside class: " + m_ActiveDatabase.databaseEntryClass);
                return null;
            }
            string strDisplayName = strVariableName;
            object[] mAttributes = mProperty.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (mAttributes.Length != 0)
            {
                strDisplayName = (mAttributes[0] as DisplayNameAttribute).DisplayName;
                if (strDisplayName == "")
                {
                    strDisplayName = strVariableName;
                }
            }
            ColumnHeader mHeader = new ColumnHeader();
            mHeader.DisplayIndex = m_StatListView.Columns.Count;
            mHeader.Text = strDisplayName;
            if (mPropertyFilterData != null)
            {
                if (mPropertyFilterData.OverrideName != "")
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

        private void _filter(bool bSuspendLayout)
        {
            if (bSuspendLayout)
            {
                m_StatListView.SuspendLayout();
            }
            m_ObjectsToListViewItem.Clear();
            m_StatListView.Items.Clear();
            //todo show all
            foreach (ClassInstance mInstance in m_ActiveDatabase.getEntries())
            {
                _addInstanceToListView(mInstance);
            }
            if (bSuspendLayout)
            {
                m_StatListView.ResumeLayout();
            }
        }

        private void timerForFilter_Tick(object sender, EventArgs e)
        {
            _filter(true);
            timerForFilter.Enabled = false;
        }

        private void txtBoxFilterListView_TextChanged(object sender, EventArgs e)
        {
            timerForFilter.Enabled = true;
        }

        private void _createContextMenu(Point mousePoint)
        {

            m_StatListViewContextMenu.MenuItems.Clear();
            m_StatListViewContextMenu.MenuItems.Add("Create New Entry", _createNewStat);


            m_ContextMenuItem = m_StatListView.GetItemAt(mousePoint.X, mousePoint.Y);
            if (m_ContextMenuItem != null)
            {

                if (m_StatListView.SelectedItems.Count == 1)
                {
                    m_StatListViewContextMenu.MenuItems.Add("Rename " + m_ContextMenuItem.SubItems[0].Text, _renameStat);
                }
                m_StatListViewContextMenu.MenuItems.Add("-");
                m_StatListViewContextMenu.MenuItems.Add("Delete " + m_ContextMenuItem.SubItems[0].Text, _deleteStat);
            }
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
        private void _deleteStat(object sender, EventArgs e)
        {
            List<ClassInstance> mList = new List<ClassInstance>();
            getClassInstancesSelected(mList);
            if (mList.Count == 0)
            {
                return;
            }

            string strInstanceNames = "";
            foreach (ClassInstance mInstance in mList)
            {
                strInstanceNames = strInstanceNames + m_ActiveDatabase.getEntryName(mInstance) + Environment.NewLine;
            }
            strInstanceNames = strInstanceNames.Substring(0, strInstanceNames.Length - 1);
            DialogResult mResult = MessageBox.Show("Are you sure you want to delete(" + mList.Count + "): " + Environment.NewLine + strInstanceNames, "Delete?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (mResult == DialogResult.Yes)
            {
                List<ListViewItem> mItems = new List<ListViewItem>();
                foreach (ListViewItem mItem in m_StatListView.SelectedItems)
                {
                    mItems.Add(mItem);
                }
                foreach (ListViewItem mItem in mItems)
                {
                    m_StatListView.Items.Remove(mItem);
                }
                foreach (ClassInstance mInstance in mList)
                {
                    m_ActiveDatabase.deleteEntry(mInstance);
                }

            }
        }

        public int getClassInstancesSelected(List<ClassInstance> mSelected)
        {
            mSelected.Clear();
            foreach (ListViewItem mItem in m_StatListView.SelectedItems)
            {
                ClassInstance mInstance = m_ActiveDatabase.getEntryByName(mItem.Text);
                if (mInstance != null)
                {
                    mSelected.Add(mInstance);
                }
            }
            return mSelected.Count;
        }

        private void m_StatListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_StatListView.SelectedIndices.Count == 1 &&
                m_iLastSelected == m_StatListView.SelectedIndices[0])
            {
                
                return;
            }
            if (m_StatListView.SelectedIndices.Count == 0)
            {
                statObjectViewer.setObjectViewing(null);
                return;
            }
            
            if (m_StatListView.SelectedIndices.Count == 1)
            {
                m_iLastSelected = m_StatListView.SelectedIndices[0];
                ClassInstance mInstance = m_ActiveDatabase.getEntryByName(m_StatListView.SelectedItems[0].Text);
                statObjectViewer.setObjectViewing(mInstance);
            }
            else
            {
                statObjectViewer.setObjectViewing(null);
                foreach (ListViewItem mItem in m_StatListView.SelectedItems)
                {
                    ClassInstance mInstance = m_ActiveDatabase.getEntryByName(mItem.Text);
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

                if (getClassInstancesSelected(mInstances) > 0)
                {
                    string strCopyString = "";
                    foreach (ClassInstance mInstance in mInstances)
                    {
                        strCopyString = strCopyString + m_ActiveDatabase.getEntryName(mInstance) + Environment.NewLine;
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
                        strCopyString = strCopyString + m_ActiveDatabase.getEntryGuid(mInstance) + Environment.NewLine;
                    }
                    strCopyString = strCopyString.Substring(0, strCopyString.Length - 1);
                    Clipboard.SetText(strCopyString);
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void statObjectViewer_PropertyValueChanged(object m, EventArgs e)
        {
            
        }

        private void statObjectViewer_SelectedObjectsHaveChangedProperties(List<object> mObjects)
        {
            //TODO
            m_StatListView.SuspendLayout();
            foreach (ListViewItem mItem in m_StatListView.SelectedItems)
            {
                int iGuid = -1;
                int.TryParse(mItem.SubItems[1].Text, out iGuid);
                ClassInstance mInstance = m_ActiveDatabase.getEntryByGuid(iGuid);
                if (mInstance != null)
                {
                    mInstance.m_bIsDirty = true;
                    _updateListItemByInstance(mInstance);



                }
            }
            m_StatListView.ResumeLayout();
            isDirty = true;
        }

        private void m_StatListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            switch (m_StatListView.Sorting)
            {
                case SortOrder.Ascending:
                    m_StatListView.Sorting = SortOrder.Descending;
                    break;
                case SortOrder.Descending:
                    m_StatListView.Sorting = SortOrder.Ascending;
                    break;
            }
            m_StatListView.ListViewItemSorter = new ListViewComparer(e.Column, m_StatListView.Sorting);
            m_StatListView.Sort();
        }


        private bool _addInstanceToListView(ClassInstance mInstance)
        {
            if (mInstance == null)
            {
                return false;
            }
            string strName = mInstance.getPropertyValueString("m_strName", "");
            if (strName == null || strName == "")
            {
                MessageBox.Show("Unable to get name from object.", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (txtBoxFilterListView.Text != "" &&
                strName.ToUpper().Contains(txtBoxFilterListView.Text.ToUpper()) == false)
            {
                return false;
            }

            ListViewItem mNewListViewItem = new ListViewItem(strName);
            m_StatListView.Items.Add(mNewListViewItem);
            m_ObjectsToListViewItem.Add(mInstance, mNewListViewItem);

            if (_updateListItemByInstance(mInstance))
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
            if (mInputBox.ShowDialog(this) == DialogResult.OK)
            {
                if (m_ActiveDatabase.isValidName(mInputBox.inputBox))
                {
                    if (m_ActiveDatabase.createEntry(mInputBox.inputBox) == EERROR_ADDING.NO_ERRORS)
                    {
                        ClassInstance mInstance = m_ActiveDatabase.getEntryByName(mInputBox.inputBox);
                        if (mInstance != null)
                        {
                            _addInstanceToListView(mInstance);
                            isDirty = true;
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
            if (m_StatListView.SelectedItems.Count != 1)
            {
                return;
            }
            InputBox mInputBox = new InputBox();
            mInputBox.Text = "Rename";
            mInputBox.inputBox = m_StatListView.SelectedItems[0].Text;
            mInputBox.message = "Please enter in a NEW unique name.";
            if (mInputBox.ShowDialog(this) == DialogResult.OK)
            {
                if (m_ActiveDatabase.isValidName(mInputBox.inputBox))
                {
                    ClassInstance mInstance = m_ActiveDatabase.getEntryByName(m_StatListView.SelectedItems[0].Text);
                    if (mInstance != null &&
                        m_ActiveDatabase.renameEntry(mInstance, mInputBox.inputBox))
                    {
                        if (_updateListItemByInstance(mInstance))
                        {
                            isDirty = true;
                        }
                        return;
                    }


                }
                MessageBox.Show("Invalid name. Name appears to not be unique.", "invalid name", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            }
        }


        private bool _updateListItemByInstance(ClassInstance mInstance)
        {
            if (m_ObjectsToListViewItem.ContainsKey(mInstance) == false)
            {
                MessageBox.Show("The map containing entries to the list view items appears to be out of sync.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            ListViewItem mNewListViewItem = m_ObjectsToListViewItem[mInstance];
            if (mNewListViewItem == null)
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
            else if (mNewListViewItem.SubItems[1].Text != strGuid)
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

        public bool save()
        {
            if (m_ActiveDatabase.saveDatabase(false))
            {
                if (m_ActiveDatabase.getEntrysDirtyCount() > 0)
                {                    
                    return false;
                }
            }
            isDirty = false;
            return true;
        }


        public Type statObjectViewer_RequestTypeOverride(Type mTypeCreating)
        {
            foreach(ClassCreatorManager mClassCreatorManager in m_ActiveDatabase.getDatabaseManager().classCreators)
            {
                ClassParserManager mParser = mClassCreatorManager.getClassParser();
                if( mParser != null &&
                    mParser.getProjectWrapper() != null)
                {
                    ProjectWrapper mWrapper = mParser.getProjectWrapper();
                    List<ClassStructure> mList = mWrapper.getClassesInheritingFromClass(mTypeCreating.Name);
                    if( mList != null &&
                        mList.Count > 1)
                    {
                        List<string> mClasses = new List<string>();
                        List<string> mHelpText = new List<string>();
                        foreach(ClassStructure mClass in mList)
                        {
                            mClasses.Add(mClass.name);
                            mHelpText.Add(mClass.comment);
                        }
                        ClassSelection mClassSelection = new ClassSelection();
                        mClassSelection.configure(mClasses, mHelpText);
                        if(mClassSelection.ShowDialog(this) == DialogResult.Cancel)
                        {
                            return null;
                        }
                        return mClassCreatorManager.getClassType(mClassSelection.getSelectedClass());
                    }
                }
            }
            return mTypeCreating;
        }

    } //end of class
}//end of namespace
