using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Library.UnitType.Wrappers;
using Library.UnitType.Forms;

namespace Library.UnitType
{
	public class UnitTypeTreeViewManager
	{



		ContextMenu m_UnitTypesContextMenu = new ContextMenu();
		private Color m_BackgroundColor;
		private ChildSelectForm m_ChildSelectForm = null;
		private UnitTypeCategory m_ActiveCategory = null;
		private UnitTypeManager m_Manager = null;		
		private string m_strCategory = "";
		private TreeView m_TreeView = null;
		private List<UnitTypeTreeNode> m_UnitTypeTreeNodes = new List<UnitTypeTreeNode>();
		private Dictionary<TreeNode, UnitTypeTreeNode> m_TreeNodesToUnitTypeTreeNode = new Dictionary<TreeNode, UnitTypeTreeNode>();
		private string m_strUnitTypeFilterByString = "";
		private string m_strUnitTypeFilterByUnitType = "";
		private bool m_bAllowForUnitTypeCreation = true;
		private bool m_bAllowForUnitTypeDeletion = true;
		private bool m_bAllowForUnitTypeRenaming = true;

		public UnitTypeTreeViewManager(UnitTypeManager mManager, string strCategory, TreeView mTreeView, bool bAllowForCreation, bool bAllowForDeletion, bool bAllowForRenaming)
		{
			m_Manager = mManager;
			
			m_TreeView = mTreeView;
			m_bAllowForUnitTypeCreation = bAllowForCreation;
			m_bAllowForUnitTypeDeletion = bAllowForDeletion;
			m_bAllowForUnitTypeRenaming = bAllowForRenaming;
			if (m_TreeView != null &&
				m_Manager != null)
			{
				m_TreeView.Font = new Font(m_TreeView.Font, FontStyle.Bold);
				m_BackgroundColor = m_TreeView.BackColor;
				m_Manager.registerTreeViewManager(this);
				_registerFunctions();
				setCategory(strCategory); //set category does a refresh
				//requestRefresh();
			}

			
		}

		public void setCategory(string strCategory)
		{
			if (m_strCategory != strCategory)
			{
				m_strCategory = (strCategory != null)?strCategory:"";
				requestRefresh();
			}
		}
		public UnitTypeCategory getUnitTypeCategory()
		{
			if(m_ActiveCategory != null)
			{
				return m_ActiveCategory;
			}
			m_ActiveCategory = m_Manager.getCategory(m_strCategory);
			return m_ActiveCategory;
		}

		public bool isValid()
		{
			if( m_strCategory == "" ||
				m_Manager.getCategory(m_strCategory) == null )
			{
				return false;
			}
			return true;
		}

		public UnitType getRootUnitType()
		{
			
			if (m_ActiveCategory == null ||
				m_TreeView.SelectedNode == null ||
				m_TreeNodesToUnitTypeTreeNode.ContainsKey(m_TreeView.SelectedNode) == false)
			{
				return null;
			}
			TreeNode mTreeNode = m_TreeView.SelectedNode;
			while( mTreeNode.Parent != null )
			{
				mTreeNode = mTreeNode.Parent;
			}
			return m_ActiveCategory.getUnitTypeByCodeName(mTreeNode.Text);
		}
		public UnitType getSelectedUnitType(bool bHasToBeRoot)
		{
			UnitTypeCategory mCategory = getUnitTypeCategory();
			if (mCategory == null || 
				m_TreeView.SelectedNode == null ||
				m_TreeNodesToUnitTypeTreeNode.ContainsKey(m_TreeView.SelectedNode) == false )
			{
				return null;
			}
			if (bHasToBeRoot &&
				m_TreeView.SelectedNode.Parent != null )
			{
				return null;
			}

			return mCategory.getUnitTypeByCodeName(m_TreeView.SelectedNode.Text);
		}

		private void _registerFunctions()
		{
			m_TreeView.LabelEdit = m_bAllowForUnitTypeRenaming;
			if (m_bAllowForUnitTypeRenaming)
			{
				m_TreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(_treeViewAfterLabelEdit);				
			}
			m_TreeView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(_treeViewBeforeLabelEdit);
			m_TreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(_treeViewBeforeSelect);
			m_TreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(_treeViewNodeMouseClick);
			m_TreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(_treeViewNodeMouseClick);
			m_TreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(_treeViewNodeMouseDoubleClick);
			m_TreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(_treeViewViewMouseDown);
		}
		private void _treeViewAfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			string strCodeName = UnitType.convertToCodeName(e.Label);
			if (m_TreeView.SelectedNode == null ||
				m_TreeNodesToUnitTypeTreeNode.ContainsKey(m_TreeView.SelectedNode) == false ||
				m_ActiveCategory.getUnitTypeByCodeName(strCodeName) != null )	//something is already named this
				
			{
				//error
				e.CancelEdit = true;
				return;
			}
			UnitType mUnitType = m_TreeNodesToUnitTypeTreeNode[m_TreeView.SelectedNode].getUnitType();
			if( mUnitType != null )
			{
				
				mUnitType.unitTypeName = strCodeName;

				if( mUnitType.unitTypeName != strCodeName )
				{
					e.CancelEdit = true;
				}
				
			}
			else
			{
				e.CancelEdit = true;
			}
		}

		private void _treeViewNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if( e.Node != null &&
				e.Node.NodeFont.Style == FontStyle.Italic)
			{
				
			}
		}
		private void _treeViewNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
		}
		private void _treeViewViewMouseDown(object sender, MouseEventArgs e)
		{
			Form mForm = m_TreeView.FindForm();
			Point mControlLocation = mForm.PointToClient(mForm.PointToScreen(m_TreeView.Location));
			Point mousePoint = new Point(e.X, e.Y);// + mControlLocation.X, e.Y + mControlLocation.Y);
			TreeNode mNode = m_TreeView.GetNodeAt(mousePoint);
			if (mNode == null ||
				mNode.Parent == null ||
				mNode.Parent.Parent == null)
			{
				m_TreeView.SelectedNode = mNode;
			}
			else
			{
				m_TreeView.SelectedNode = null;
			}

			if( e.Button == MouseButtons.Right)
			{
				//context menu
				_createContextMenu();
				if (m_UnitTypesContextMenu.MenuItems.Count != 0)
				{
					mousePoint.X += 8;
					mousePoint.Y -= 5;
					m_UnitTypesContextMenu.Show(m_TreeView, mousePoint);
				}
			}
			
		}

		private void _treeViewBeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			//we can only edit the root node's name
			e.CancelEdit = (e.Node.Text != "ANY" && e.Node.Parent == null) ? false : true;
		}

		private void _treeViewBeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			e.Cancel = (e.Node.Parent == null || e.Node.Parent.Parent == null) ? false : true;
		}


		private void _createContextMenu()
		{
			if( m_TreeView == null ||
				m_ActiveCategory == null )
			{
				return;
			}
			if( m_bAllowForUnitTypeCreation == false &&
				m_bAllowForUnitTypeDeletion == false &&
				m_bAllowForUnitTypeRenaming == false )
			{
				return;	//we can't do anything so lets not add anything to do
			}
			m_UnitTypesContextMenu.MenuItems.Clear();
			if (m_bAllowForUnitTypeCreation)
			{
				m_UnitTypesContextMenu.MenuItems.Add("New", _contextMenuNewUnitType);
			}

			if ( m_TreeView.SelectedNode != null )
			{
				bool bIsParentUnitType = (m_TreeView.SelectedNode.Parent == null) ? true : false;
				
				if (m_bAllowForUnitTypeCreation)
				{
					if(bIsParentUnitType)
					{
						//we can only do this if it's the parent
						m_UnitTypesContextMenu.MenuItems.Add("Add Child Unit Type", _contextMenuAddChildUnitType);
					}
					m_UnitTypesContextMenu.MenuItems.Add("-");	//new will be on top
				}
				
				
				if (m_bAllowForUnitTypeDeletion)
				{
					if (bIsParentUnitType)
					{
						if(m_TreeView.SelectedNode.Text == "ANY")
						{
							m_UnitTypesContextMenu.MenuItems.Add("Cannot remove ANY");
						}
						else
						{
							m_UnitTypesContextMenu.MenuItems.Add("Delete Unit Type", _contextMenuDeleteUnitType);
						}
						
					}
					else
					{
						if (m_TreeView.SelectedNode.Parent.Parent == null)	//we can only delete the first child in the hiecharacy
						{
							m_UnitTypesContextMenu.MenuItems.Add("Delete Child", _contextMenuDeleteChildUnitType);
						}
					}
				}
			}



		}

		private void _contextMenuNewUnitType(object sender, EventArgs e)
		{
			if(m_ActiveCategory == null)
			{
				return;
			}
			string strValidName = "NEW";
			int iCount = 1;
			while(m_ActiveCategory.getUnitTypeByUnsafeName(strValidName) !=null)
			{
				strValidName = "New" + iCount.ToString();
				iCount++;
			}

			UnitType mUnitType = m_ActiveCategory.createUnitType(strValidName);
			if( mUnitType == null )
			{
				MessageBox.Show("Error in creating new Unit Type.");
				return;
			}
			foreach(TreeNode mNodes in m_TreeView.Nodes)
			{
				if( mNodes.Text == mUnitType.unitTypeName)
				{
					m_TreeView.SelectedNode = mNodes;
					return;
				}
			}

		}
		private void _contextMenuAddChildUnitType(object sender, EventArgs e)
		{
			if(m_ActiveCategory == null )
			{
				return;
			}
			//make sure we first can get the selected unit type
			UnitType mSelectedUnitType = getSelectedUnitType(false);
			if( mSelectedUnitType == null )
			{
				//we can't do anything if nothing is selected
				return; 
			}
			if (m_ChildSelectForm == null )
			{
				m_ChildSelectForm = new ChildSelectForm();
			}
			m_ChildSelectForm.unitTypeCategory = m_ActiveCategory;
			m_ChildSelectForm.filterByString = m_strUnitTypeFilterByString;
			m_ChildSelectForm.addUnitTypeToHide(mSelectedUnitType);
			foreach(string strChildren in mSelectedUnitType.fullHierarchyOfChildren)
			{				
				m_ChildSelectForm.addUnitTypeToHide(m_ActiveCategory.getUnitTypeByCodeName(strChildren));
			}
			if ( m_ChildSelectForm.ShowDialog() == DialogResult.OK)
			{
				List<string> mSelectedUnitTypes = m_ChildSelectForm.getSelectedUnitTypes();
				foreach(string strUnitTypeName in mSelectedUnitTypes)
				{
					//we need to add children
					mSelectedUnitType.addChildByCodeName(strUnitTypeName);
				}
				
			}


		}
		private void _contextMenuDeleteUnitType(object sender, EventArgs e)
		{

			UnitType mUnitType = getSelectedUnitType(true);
			if( mUnitType == null ||
				m_ActiveCategory == null )
			{
				return;
			}
			m_ActiveCategory.deleteUnitType(mUnitType);
		}
		private void _contextMenuDeleteChildUnitType(object sender, EventArgs e)
		{
			
			TreeNode mSelectedNode = m_TreeView.SelectedNode;
			if (mSelectedNode == null ||
				mSelectedNode.Parent == null ||			//we aren't a child if the root is null
				mSelectedNode.Parent.Parent != null )	//we are a child but to deep, can't delete this
			{
				return;
			}
			UnitType mUnitType = getRootUnitType();
			if( mUnitType != null )
			{
				mUnitType.removeChildByName(mSelectedNode.Text);
			}

		}
		//sets the filter for the unit type
		public string unitTypeFilterString
		{
			get
			{
				return m_strUnitTypeFilterByString;
			}
			set
			{
				m_strUnitTypeFilterByString = value.ToUpper();
				requestRefresh();
			}
		}
		//sets the filter for the unit type
		public string unitTypeFilterByUnitType
		{
			get
			{
				return m_strUnitTypeFilterByUnitType;
			}

			set
			{
				m_strUnitTypeFilterByUnitType = UnitType.convertToCodeName(value);
				requestRefresh();
			}
		}

		public UnitTypeManager getUnitTypeManager() { return m_Manager; }

		
		public TreeView getTreeView() { return m_TreeView; }

		public void requestRefresh()
		{

			m_ActiveCategory = null;
			m_ActiveCategory = getUnitTypeCategory();

			TreeNode mSelectedTreeNode = m_TreeView.SelectedNode;
			UnitTypeTreeNode mUnitTypeTreeNode = (mSelectedTreeNode != null && m_TreeNodesToUnitTypeTreeNode.ContainsKey(mSelectedTreeNode)) ? m_TreeNodesToUnitTypeTreeNode[mSelectedTreeNode] : null;
			m_TreeNodesToUnitTypeTreeNode.Clear();

			if (m_ActiveCategory == null)
			{
				m_TreeView.BackColor = Color.DarkGray;
				m_TreeView.Enabled = false;
				return;
			}
			m_TreeView.BackColor = m_BackgroundColor;
			m_TreeView.Enabled = true;
			List<UnitType> mUnitTypes = m_ActiveCategory.getUnitTypes();
			for (int iIndex = 0; iIndex < m_UnitTypeTreeNodes.Count; iIndex++)
			{
				if (m_UnitTypeTreeNodes[iIndex].getNeedsToBeDeleted())
				{
					m_UnitTypeTreeNodes.RemoveAt(iIndex);
					iIndex--;
					continue;
				}
				m_UnitTypeTreeNodes[iIndex].refreshTreeNodes();
			}
			TreeNode mTreeNodeCreatedForSelect = null;
			if (mUnitTypes.Count != m_UnitTypeTreeNodes.Count)
			{
				//a new one was added! We'll walk it backwards because most of the time new unittypes get added to the end.
				for (int iUnitTypeIndex = mUnitTypes.Count - 1; iUnitTypeIndex >= 0; iUnitTypeIndex--)
				{
					bool bFound = false;
					UnitType mUnitType = mUnitTypes[iUnitTypeIndex];
					foreach (UnitTypeTreeNode mNode in m_UnitTypeTreeNodes)
					{
						if (mNode.getUnitType() == mUnitType)
						{
							bFound = true;
							break;
						}
					}
					if (bFound == false)
					{
						UnitTypeTreeNode mNewTreeNode = new UnitTypeTreeNode(this, mUnitType);
						m_UnitTypeTreeNodes.Add(mNewTreeNode);
						mTreeNodeCreatedForSelect = mNewTreeNode.getTreeNode();
						if (mUnitTypes.Count == m_UnitTypeTreeNodes.Count)
						{
							break;  //we are done.
						}
					}
				}
			}
			foreach (UnitTypeTreeNode mNode in m_UnitTypeTreeNodes)
			{				
				_addTreeNodeAndChildrenToDictionary(mNode, mNode.getTreeNode());
			}
			m_TreeView.Sort();
			if(m_TreeView.SelectedNode == null &&
				mSelectedTreeNode != null )
			{
				m_TreeView.SelectedNode = mSelectedTreeNode;
			}
			if(m_TreeView.SelectedNode == null &&
				mUnitTypeTreeNode != null )
			{
				m_TreeView.SelectedNode = mUnitTypeTreeNode.getTreeNode();				
			}
			if( m_TreeView.SelectedNode == null &&
				mTreeNodeCreatedForSelect != null )
			{
				m_TreeView.SelectedNode = mTreeNodeCreatedForSelect;
			}
		}

		private void _addTreeNodeAndChildrenToDictionary(UnitTypeTreeNode mOwner, TreeNode mTreeNode)
		{
			if( mTreeNode == null )
			{
				return;
			}
			m_TreeNodesToUnitTypeTreeNode.Add(mTreeNode, mOwner);
			foreach(TreeNode mChildNode in mTreeNode.Nodes)
			{
				_addTreeNodeAndChildrenToDictionary(mOwner, mChildNode);
			}
		}
	}

		
}
