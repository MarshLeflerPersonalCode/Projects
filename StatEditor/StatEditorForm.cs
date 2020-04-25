using System;
using System.Collections.Generic;
using System.Collections;
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
using Library.UnitType;
using Library.UnitType.Forms;
using Library.Database;
// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace StatEditor
{


	public partial class StatEditorForm : Form, IDatabaseCallbacks
	{
		
        enum ELOAD_STATE
        {
            configure,
            waiting_for_core,
            done
        };
        
        private bool m_bDirty = false;
        
        private ELOAD_STATE m_eLoadState = ELOAD_STATE.configure;        		
        private ClassContructionAndDBHandler m_Core = null;        
        private UnitTypeControl m_UnitTypeControl = new UnitTypeControl();
        private List<DatabaseEditorControl> m_EditorControls = new List<DatabaseEditorControl>();
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
                this.Text = "Data Editor *";
            }
            else
            {
                this.Text = "Data Editor";
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
                        m_UnitTypeControl.configure(m_Core.unitTypeManager);
                        m_eLoadState = ELOAD_STATE.done;
                    }
					
				}
				break;				
				case ELOAD_STATE.done:
				{
                    _createDatabaseTabs();
                    m_Core.databaseManager.getDatabase("Stats").databaseCallbacks.Add(this);




                    //object mObject = m_Core.classCreatorManager.createNewClass("KCIncludeTest");// FKCStatDefinition");// new ClassTestingObjectViewer();
                    //statObjectViewer.setObjectViewing(mObject);
                    timerProcessClasses.Enabled = false;
                    this.Focus();
                    this.Activate();
                    this.TopMost = false;
                    updateTimer.Enabled = true;

                }
				break;
			}

		}

        private void _createDatabaseTabs()
        {
            
            tabs.SuspendLayout();
            foreach (Database mDatabase in m_Core.databaseManager.databases)
            {
                tabs.TabPages.Add(mDatabase.databaseName);
                TabPage mTab = tabs.TabPages[tabs.TabPages.Count - 1];
                //mTab.ImageIndex
                DatabaseEditorControl mControl = new DatabaseEditorControl();
                mTab.Controls.Add(mControl);
                mControl.Dock = DockStyle.Fill;
                mControl.setDatabase(mDatabase, m_Core.getLogFile());
                m_EditorControls.Add(mControl);                                
            }
            tabs.TabPages.Add("Unit Types");
            TabPage mUnitTypeTab = tabs.TabPages[tabs.TabPages.Count - 1];
            mUnitTypeTab.Controls.Add(m_UnitTypeControl);
            m_UnitTypeControl.Dock = DockStyle.Fill;
            tabs.ResumeLayout();
            

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
            bool bEverythingSavedOkay = true;
            if(m_Core.unitTypeManager.needsToSave)
            {
                if( m_Core.unitTypeManager.save() == false )
                {
                    bEverythingSavedOkay = false;
                    log("Error in saving Unit Types");
                }
            }
            foreach (DatabaseEditorControl mControl in m_EditorControls)
            {
                if (mControl.isDirty)
                {
                    if(mControl.save() == false )
                    {
                        bEverythingSavedOkay = false;
                        log("Error in saving database : " + mControl.database.databaseName);

                    }

                }
            }
            _setDirty(!bEverythingSavedOkay);

            
            return bEverythingSavedOkay;
        }


        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_Core.showEditorConfigForm(this);
        }

        private void unitTypeConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_Core.unitTypeManager.showUnitTypeManagerConfigForm(this);
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if( m_bDirty )
            {
                return;
            }
            if( m_Core.unitTypeManager.needsToSave)
            {
                _setDirty(true);
                return;
            }
            foreach(DatabaseEditorControl mControl in m_EditorControls)
            {
                if(mControl.isDirty)
                {
                    _setDirty(true);
                    return;
                }
            }
        }

        public void notifyOfNewEntry(Database mDatabase, ClassInstance mNewEntry)
        {

        }
        public void notifyOfPreSave(Database mDatabase)
        {
            //stats need to know what stats they need to modify
            if(mDatabase.databaseName=="Stats")
            {
                _hookupStats(mDatabase);
            }
        }
        public void notifyOfPostSave(Database mDatabase)
        {

        }

        private void _hookupStats(Database mDatabase)
        {
            //stats need to know what stats they need to modify
            foreach( ClassInstance mInstance in mDatabase.getEntries())
            {
                if( mInstance.m_bIsDirty)
                {
                    string strStatName = mDatabase.getEntryName(mInstance);
                    IList mFunctions = mInstance.getPropertyValueList("m_MathFunctions");                                        
                    if(mFunctions == null )
                    {
                        continue;
                    }
                    foreach (object mObject in mFunctions)
                    {
                        ClassInstance mFunction = mObject as ClassInstance;
                        string strStat = mFunction.getPropertyValueString("m_strStat", "");
                        if(strStat != "")
                        {
                            ClassInstance mInstanceToPossiblyDirt = mDatabase.getEntryByName(strStat);
                            if( mInstanceToPossiblyDirt == null )
                            {
                                log("ERROR - stat " + strStatName + " has a math function(" + mFunction.GetType().Name + ") that is pointing to a stat that doesn't exist - " + strStat);
                                continue;
                            }
                            List<string> mStringRef = mInstanceToPossiblyDirt.getPropertyValueList("m_StatsReferencing") as List<string>;
                            if(mStringRef.Contains(strStatName) == false )
                            {
                                mStringRef.Add(strStatName);
                                mInstanceToPossiblyDirt.m_bIsDirty = true;
                            }
                        }
                    }
                }
            }
        }


    }//end class
} //end namespace
