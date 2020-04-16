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
using Library.IO;
using Library.ClassParser;
using Library.ClassCreator;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace StatEditor
{
	enum ELOAD_STATE
	{
		GetFileList,
		CompareFileList,
		BuildClassStructures,
		WaitForClassStructuresToBeBuilt,
		CreateCSharpClasses,
		CompileCSharpClasses,
		Done
	};

	public partial class StatEditorForm : Form
	{
		private ELOAD_STATE m_eLoadState = ELOAD_STATE.GetFileList;
		ContextMenu m_StatListViewContextMenu = new ContextMenu();
		private ProgressBar m_ProgressBar = null;
		private LogFile m_LogFile = null;
		private ClassCreatorManager m_ClassManager = new ClassCreatorManager();
		private ClassParserManager m_ClassParser = new ClassParserManager();
		private ListViewItem m_ContextMenuItem = null;
		public StatEditorForm()
		{
			InitializeComponent();
			_createLogFile();
			

		}

		private void StatEditorForm_Shown(object sender, EventArgs e)
		{
			_showProgressBar();
			_findClasses();
			timerProcessClasses.Enabled = true;
			
		}

		

		private void _showProgressBar()
		{
			m_ProgressBar = new ProgressBar();
			m_ProgressBar.Show(this);
		}

		private void _createLogFile()
		{
			if (m_LogFile == null)
			{
				m_LogFile = new LogFile("./StatEditor.log");
				m_ClassParser.logFile = m_LogFile;
				m_ClassManager.logFile = m_LogFile;
			}
		}

		public void log(string strMessage)
		{
			if(m_ProgressBar != null)
			{
				m_ProgressBar.setMessage(strMessage);
			}
			m_LogFile.log(strMessage);
		}

		private void _findClasses()
		{
			string[] mFiles = Directory.GetFiles(@"D:\Personal\Projects\CoreClasses\", "*.h", SearchOption.AllDirectories);
			foreach(string mFile in mFiles)
			{
				m_ClassParser.addFileToParse(mFile);
			}
			log("Processing " + mFiles.Length + " files.");
		}

		private void timerProcessClasses_Tick(object sender, EventArgs e)
		{

			switch(m_eLoadState)
			{
				case ELOAD_STATE.GetFileList:
					{
						_findClasses();
						m_eLoadState = ELOAD_STATE.CompareFileList;
					}
					break;
				case ELOAD_STATE.CompareFileList:
					{
						m_ClassParser.compareToCachedData("");
						m_eLoadState++;
					}
					break;
				case ELOAD_STATE.BuildClassStructures:
					{
						m_ClassParser.buildClassStructures();
						m_eLoadState++;
					}
					break;
				case ELOAD_STATE.WaitForClassStructuresToBeBuilt:
					{
						if (m_ClassParser.getDoneParsingClassesOnThreads())
						{
							m_ClassParser.saveCachedData("classparser.cache");
							m_eLoadState = ELOAD_STATE.CreateCSharpClasses;
						}
					}
					break;
				case ELOAD_STATE.CreateCSharpClasses:
					{
						m_ClassManager.intialize(m_ClassParser, "classcompile.dll");
						m_eLoadState = ELOAD_STATE.CompileCSharpClasses;
					}
					break;
				case ELOAD_STATE.CompileCSharpClasses:
					{
						if (m_ClassManager.isDoneParsingAndCompiling())
						{
							m_eLoadState = ELOAD_STATE.Done;
							string strErrors = m_ClassManager.getErrorsAsString();
							if (strErrors != null && strErrors.Length != 0)
							{
								MessageBox.Show(this, "Error in Compiling. Check Log for more details.\n\n" + strErrors, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}							
						}
					}
					break;
				case ELOAD_STATE.Done:
					{
                    object mObject = m_ClassManager.createNewClass("KCIncludeTest");// FKCStatDefinition");// new ClassTestingObjectViewer();
						statObjectViewer.setObjectViewing(mObject);

						timerProcessClasses.Enabled = false;
						m_ProgressBar.Hide();
						m_ProgressBar = null;
					}
					break;
			}

		}

		private void testToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ClassTestingObjectViewer mObject = new ClassTestingObjectViewer();
			statObjectViewer.setObjectViewing(mObject);
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

		private void _createNewStat(object sender, EventArgs e)
		{

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
	}//end class
} //end namespace
